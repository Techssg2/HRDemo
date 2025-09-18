using Aeon.Academy.API.Mappers;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Services;
using Aeon.HR.Infrastructure;
using Aeon.HR.Infrastructure.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.Module
{
    public class ModuleDashboard : IModuleDashboard
    {
        private readonly ITrainingRequestService trainingRequestService;
        private readonly ITrainingReportService trainingReportService;
        private readonly ITrainingInvitationService trainingInvitationService;
        private readonly IUserService userService;
        private NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public ModuleDashboard(ITrainingRequestService trainingRequestService, ITrainingReportService trainingReportService, ITrainingInvitationService trainingInvitationService, IUserService userService)
        {
            this.trainingRequestService = trainingRequestService;
            this.trainingReportService = trainingReportService;
            this.trainingInvitationService = trainingInvitationService;
            this.userService = userService;
        }

        public IList<object> GetMyItems(Guid userId)
        {
            var list = new List<object>();
            try
            {
                var requests = trainingRequestService.ListByUserId(userId);
                var myItems = requests.ToMyItemDtos();
                list = myItems.Cast<object>().ToList();

                var reports = trainingReportService.ListByUser(userId);
                var reportItem = reports.ToMyItemDtos();
                list.AddRange(reportItem.Cast<object>().ToList());
            }
            catch (Exception ex)
            {
                logger.Info($"User ID: {userId}");
                logger.Error(ex, "GetMyItems");
                logger.Info($"StackTrace: {ex.StackTrace}");
            }
            return list;
        }

        public IList<object> GetMyTasks(Guid userId, QueryArgs args)
        {
            var list = new List<object>();
            try
            {
                var departmentIds = userService.GetUserDepartmentMappingsByUserId(userId)
                    .Where(m => m.DepartmentId.HasValue)
                    .Select(m => m.DepartmentId.Value).Distinct().ToList();

                var requests = trainingRequestService.ListByDepartmentIds(userId, departmentIds, args.Page, args.Limit);
                var myTasks = requests.ToMyTaskDtos();
                list = myTasks.Cast<object>().ToList();

                var report = trainingReportService.ListByDepartmentIds(userId, departmentIds, args.Page, args.Limit);
                var reportTasks = report.ToMyTaskDtos();
                list.AddRange(reportTasks.Cast<object>().ToList());

                var responseInvitations = trainingInvitationService.ListPendingByUserId(userId, args.Page, args.Limit);
                var myResponseInvitationTasks = responseInvitations.ToMyTaskDtos(userId, TrainingInvitationStatus.SendInvitation);
                list.AddRange(myResponseInvitationTasks.Cast<object>().ToList());

                var waitingCreateReports = trainingInvitationService.ListAfterReportByUserId(userId, args.Page, args.Limit);
                var myWaitingReportsTasks = waitingCreateReports.ToMyTaskDtos(userId, TrainingInvitationStatus.WaitingForAfterReport);
                list.AddRange(myWaitingReportsTasks.Cast<object>().ToList());
            }
            catch (Exception ex)
            {
                logger.Info($"User ID: {userId}");
                logger.Error(ex, "GetMyTasks");
                logger.Info($"StackTrace: {ex.StackTrace}");
            }
            return list;
        }
        public IList<object> GetJobTasks()
        {
            var list = new List<object>();

            var requests = trainingRequestService.ListTasks();
            var requestTasks = new List<object>();
            foreach (var task in requests)
            {
                requestTasks.Add(new
                {
                    User = task.User.ToUserDto(),
                    Edoc2Tasks = task.Edoc2Tasks.ToMyTaskDtos()
                });
            }
            list = requestTasks.Cast<object>().ToList();

            var reports = trainingReportService.ListTasks();
            var reportTasks = new List<object>();
            foreach (var task in reports)
            {
                reportTasks.Add(new
                {
                    User = task.User.ToUserDto(),
                    Edoc2Tasks = task.Edoc2Tasks.ToMyTaskDtos()
                });
            }
            list.AddRange(reportTasks.Cast<object>().ToList());

            return list;
        }
    }
}