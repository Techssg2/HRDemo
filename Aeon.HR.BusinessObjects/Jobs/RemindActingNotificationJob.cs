using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class RemindActingNotificationJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IEmailNotification _emailNotification;
        private readonly IWorkflowBO _workflowBo;
        private int dayRemindManagerActing { get; set; }
        public RemindActingNotificationJob(ILogger logger, IUnitOfWork uow, IEmailNotification emailNotification, IWorkflowBO workflowBO)
        {
            _logger = logger;
            _uow = uow;
            _emailNotification = emailNotification;
            _workflowBo = workflowBO;
            dayRemindManagerActing = int.Parse(ConfigurationManager.AppSettings["dayOfMonthRemindManagerActing"]);

        }

        public void SendNotifications()
        {
            var remindActingForManagers = GetRemindActingForManager();
            if (remindActingForManagers != null && remindActingForManagers.Any())
            {
                var itemIds = remindActingForManagers.Select(x => x.Id);
                var actingTasks = GetActingTasks(itemIds);
                foreach (var task in actingTasks)
                {
                    try
                    {
                        _workflowBo.SendEmailNotificationForApprover(EmailTemplateName.ForApprover, task);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                    }
                }
            }
        }
        private IEnumerable<ActingViewModel> GetRemindActingForManager()
        {
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            List<ActingViewModel> result = new List<ActingViewModel>();
            try
            {
                var tempItems = _uow.GetRepository<Acting>(true).FindByAsync<ActingViewModel>(x => x.Status == "Waiting for Appraiser 1" || x.Status == "Waiting for Appraiser 2", "Created asc").GetAwaiter().GetResult();
                if (tempItems.Any())
                {
                    foreach (var item in tempItems)
                    {
                        //CR324
                        var maxDate = GetMaxPeriodDate(item);

                        if (maxDate.HasValue && (maxDate.Value.Date.Subtract(now.Date).Days == 15 ||
                            maxDate.Value.Date.Subtract(now.Date).Days == 20 || maxDate.Value.Date.Subtract(now.Date).Days == 10))
                        {
                            result.Add(item);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at RemindActingNotificationJob: {ex.Message}");
            }
            return result;
        }
        private DateTimeOffset? GetMaxPeriodDate(ActingViewModel acting)
        {
            DateTimeOffset? maxPeriodDate = null;
            DateTimeOffset? result = null;
            try
            {
                var properties = acting.GetType().GetProperties();
                var periodToDates = new List<DateTimeOffset>();
                if (properties != null && properties.Any())
                {
                    foreach (var property in properties)
                    {
                        if ((property.Name == "Period1To" || property.Name == "Period2To" || property.Name == "Period3To" || property.Name == "Period4To") && property.GetValue(acting) != null)
                        {
                            periodToDates.Add((DateTimeOffset)property.GetValue(acting));
                        }
                    }
                }
                if (periodToDates.Any())
                {
                    maxPeriodDate = periodToDates.Max(x => x);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at GetMaxPeriodDate: {ex.Message}");
            }
            if (maxPeriodDate.HasValue)
            {
                //if (maxPeriodDate.Value.Day > dayRemindManagerActing)
                //{
                //    result = new DateTimeOffset(new DateTime(maxPeriodDate.Value.Year, maxPeriodDate.Value.Month, dayRemindManagerActing));
                //}
                //else
                //{
                //    result = new DateTimeOffset(new DateTime(maxPeriodDate.Value.Year, maxPeriodDate.Value.Month - 1, dayRemindManagerActing));
                //}

                result = new DateTimeOffset(new DateTime(maxPeriodDate.Value.Year, maxPeriodDate.Value.Month, maxPeriodDate.Value.Day));
            }

            return result;
        }
        private List<EdocTaskViewModel> GetActingTasks(IEnumerable<Guid> actingIds)
        {
            var edoc2Tasks = new List<EdocTaskViewModel>();
            var groupDept = new List<NotificationUserViewModel>();
            var tasks = _uow.GetRepository<WorkflowTask>(true).FindBy(x => !x.IsCompleted && x.IsTurnedOffSendNotification && actingIds.Contains(x.ItemId)).ToList();
            var deptTasks = tasks.Where(x => x.AssignedToDepartmentId.HasValue);
            foreach (var deptTask in deptTasks)
            {
                if (!groupDept.Any(x => x.DepartmentId == deptTask.AssignedToDepartmentId && x.DepartmentGroup == deptTask.AssignedToDepartmentGroup))
                {
                    var users = _uow.GetRepository<User>(true).FindBy(x => !string.IsNullOrEmpty(x.Email) && x.UserDepartmentMappings.Any(t => t.Role == deptTask.AssignedToDepartmentGroup && t.DepartmentId == deptTask.AssignedToDepartmentId));
                    foreach (var user in users)
                    {
                        groupDept.Add(new NotificationUserViewModel { UserFullName = user.FullName, UserEmail = user.Email, UserId = user.Id, DepartmentId = deptTask.AssignedToDepartmentId.Value, DepartmentGroup = deptTask.AssignedToDepartmentGroup });
                    }
                }
            }
            var assistanceNodes = _uow.GetRepository<Department>(true).FindBy<DepartmentViewModel>(x => x.UserDepartmentMappings.Any(y => y.IsHeadCount && y.Role == Group.Assistance));
            var userMappingDepartments = _uow.GetRepository<UserDepartmentMapping>(true).FindBy<UserDepartmentMappingViewModel>(x => assistanceNodes.Any(y => y.Id == x.DepartmentId));
            var allUserIds = groupDept.Select(x => x.UserId).Distinct().ToList();
            var uIds = tasks.Where(x => x.AssignedToId.HasValue).Select(x => x.AssignedToId.Value).ToList();
            foreach (var uId in uIds)
            {
                if (!allUserIds.Contains(uId))
                {
                    allUserIds.Add(uId);
                }
            }
            foreach (var userId in allUserIds)
            {
                NotificationUserViewModel notiUser = null;
                if (groupDept.Any())
                {
                    notiUser = groupDept.FirstOrDefault(x => x.UserId == userId);
                }
                else
                {
                    var currentApprover = _uow.GetRepository<User>(true).FindByIdAsync(userId).GetAwaiter().GetResult();
                    if (currentApprover != null)
                    {
                        notiUser = new NotificationUserViewModel { UserFullName = currentApprover.FullName, UserEmail = currentApprover.Email, UserId = currentApprover.Id };
                    }
                }
                if (notiUser != null)
                {
                    var userTasks = tasks.Where(x => x.AssignedToId == userId || (x.AssignedToDepartmentId == notiUser.DepartmentId && x.AssignedToDepartmentGroup == notiUser.DepartmentGroup)).Select(x => Mapper.Map<WorkflowTaskViewModel>(x)).ToList();
                    //SendEmailNotificationForApprover(EmailTemplateName.ForApprover, userTasks, notiUser);
                    edoc2Tasks.Add(new EdocTaskViewModel
                    {
                        User = notiUser,
                        Edoc2Tasks = userTasks
                    });
                }
            }
            return edoc2Tasks;
        }
    }
}
