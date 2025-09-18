using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class PromoteAndTransferJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly ISettingBO _settingUser;
        public PromoteAndTransferJob(ILogger logger, IUnitOfWork unitOfWork, ISettingBO settingUser)
        {
            _logger = logger;
            _uow = unitOfWork;
            _settingUser = settingUser;
        }
        public async Task<bool> Sync()
        {
			try
			{
                await MoveUserDepartment();
                return true;
            }
			catch (Exception ex)
			{
                _logger.LogError("Error at method: MoveUserDepartmentJob " + ex.Message);
                return true;
            }
            
        }
        public async Task MoveUserDepartment()
        {
            var newDate = new DateTimeOffset();
            var promoteAndTransfer = await _uow.GetRepository<PromoteAndTransfer>(true).GetAllAsync<PromoteAndTransferViewModel>();
            promoteAndTransfer = promoteAndTransfer.Where(x => x.EffectiveDate != newDate && (DateTime.Now.Date) == (x.EffectiveDate.Hour >= 17 ? x.EffectiveDate.ToLocalTime().Date : x.EffectiveDate.Date) && x.Status == "Completed").ToList();
            if (promoteAndTransfer.Any())
            {
                var userMappings = new List<UserDepartmentMapping>();
                try
                {
                    foreach (var item in promoteAndTransfer)
                    {
                        bool userDepartmentIsExist = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(i => i.DepartmentId == item.NewDeptOrLineId && i.UserId == item.UserId && i.IsHeadCount);
                        _logger.LogInformation($"{item.ReferenceNumber} - Have User Mapping to Department: {userDepartmentIsExist}, new Department = {item.NewDeptOrLineCode}, user: {item.FullName}");
                        if (!userDepartmentIsExist)
                        {
                            var userMapping = new UserDepartmentMapping();
                            userMapping.DepartmentId = item.NewDeptOrLineId;
                            userMapping.UserId = item.UserId;
                            userMapping.Role = Infrastructure.Enums.Group.Member;
                            userMapping.IsHeadCount = true;
                            userMappings.Add(userMapping);
                            // DELETE
                            var deleteUserDepartment = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(i => i.UserId == item.UserId && i.DepartmentId == item.CurrentDepartmentId);
							if (deleteUserDepartment != null)
							{
                                _uow.GetRepository<UserDepartmentMapping>().Delete(deleteUserDepartment);
                                _logger.LogInformation($"Remove UserDepartmentMapping: Department = {item.CurrentDepartment}, user: {item.FullName}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error at method: MoveUserDepartmentJob " + ex.Message);
                }
                _uow.GetRepository<UserDepartmentMapping>().Add(userMappings);
                await _uow.CommitAsync();
                _logger.LogInformation($"MoveUserDepartmentJob commit OK");
            }
        }
    }
}