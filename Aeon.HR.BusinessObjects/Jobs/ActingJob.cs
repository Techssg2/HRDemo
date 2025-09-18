using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class ActingJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        public ActingJob(ILogger logger, IUnitOfWork unitOfWork, ISettingBO settingUser)
        {
            _logger = logger;
            _uow = unitOfWork;
        }
        public async Task<bool> Sync()
        {
            await MoveUserDepartment();
            return true;
        }
        public async Task MoveUserDepartment()
        {
            _logger.LogInformation("Header: MoveUserDepartmentJob Actings: ");
            var newDate = new DateTimeOffset();
            var acting = await _uow.GetRepository<Acting>(true).GetAllAsync<ActingViewModel>();
            acting = acting.Where(x => x.Period1To != newDate && DateTime.Now.Date == x.Period1To).ToList();
            if (acting.Any())
            {
                var userMappings = new List<UserDepartmentMapping>();
                try
                {
                    foreach (var item in acting)
                    {
                        if (!await CheckEmployeeConfirmation(item.Id))
                        {
                            continue;
                        }
                        bool userDepartmentIsExist = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(i => i.DepartmentId == item.DepartmentId && i.UserId == item.UserId && i.IsHeadCount);
                        if (!userDepartmentIsExist)
                        {

                            var userMapping = new UserDepartmentMapping();
                            userMapping.DepartmentId = item.DepartmentId;
                            userMapping.UserId = item.UserId;
                            userMapping.Role = Infrastructure.Enums.Group.Member;
                            userMapping.IsHeadCount = true;
                            userMappings.Add(userMapping);
                            
                            // DELETE
                            var deleteUserDepartment = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(i => i.UserId == item.UserId && i.IsHeadCount);
                            _logger.LogInformation("Info: MoveUserDepartmentJob Actings: ", deleteUserDepartment.UserId +"Dept: "+deleteUserDepartment.DepartmentId);
                            _uow.GetRepository<UserDepartmentMapping>().Delete(deleteUserDepartment);

                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error at method: MoveUserDepartmentJob", ex.Message);
                }
                _uow.GetRepository<UserDepartmentMapping>().Add(userMappings);
                await _uow.CommitAsync();
            }
        }

        public async Task<bool> CheckEmployeeConfirmation(Guid? Id)
        {
            var result = new ResultDTO();
            var acting = await _uow.GetRepository<Acting>().GetSingleAsync<ActingViewModel>(x => x.Id == Id);
            var wfInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == Id, "Created desc");
            if (wfInstance != null)
            {
                var wfStep = wfInstance.WorkflowData.Steps.FirstOrDefault(x => x.StepName == "Acting Employee Confirmation" && x.SuccessVote == "Accept");
                var wfTask = await _uow.GetRepository<WorkflowTask>().GetSingleAsync(x => x.WorkflowInstanceId == wfInstance.Id
                    && x.Status == "Waiting for Acting Employee Confirmation" && x.IsCompleted, "Created desc");

                if (wfTask != null)
                {
                    return true;
                }
                else { return true; }
            }
            else { return true; }

        }
    }
}
