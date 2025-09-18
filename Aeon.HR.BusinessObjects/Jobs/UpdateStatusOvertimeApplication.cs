using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class UpdateStatusOvertimeApplication
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IEmployeeBO _employee;
        private readonly ISSGExBO _bo;
        private readonly List<string> ignoreStatus = new List<string> { Const.Status.cancelled, Const.Status.completed, Const.Status.requestedToChange, Const.Status.cancelled, Const.Status.outOfPeriod, Const.Status.rejected };
        
        public UpdateStatusOvertimeApplication(ILogger logger, IUnitOfWork uow, IEmployeeBO employee, ISSGExBO bo)
        {
            _logger = logger;
            _uow = uow;
            _employee = employee;
            _bo = bo;
        }
        public async Task<bool> DoWork()
        {
            _logger.LogInformation("Start update OT");

            DateTimeOffset fromDate = DateTimeOffset.Now.AddDays(-5);
            DateTimeOffset toDate = DateTimeOffset.Now;

            var overtimeApplications = await _uow.GetRepository<OvertimeApplication>().FindByAsync(x => x.Created <= toDate && x.Created >= fromDate && !string.IsNullOrEmpty(x.Status) && !ignoreStatus.Contains(x.Status));
            if (overtimeApplications.Any())
            {
                foreach(var overtime in overtimeApplications)
                {
                    var wfInstance = (await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.ItemId == overtime.Id && overtime.ReferenceNumber == x.ItemReferenceNumber && x.IsCompleted, "created desc")).FirstOrDefault();
                    if (wfInstance == null) continue;
                    var workflowHistory = (await _uow.GetRepository<WorkflowHistory>().FindByAsync(x => x.IsStepCompleted && x.InstanceId == wfInstance.Id && x.VoteType == VoteType.Approve, "created desc")).FirstOrDefault();
                    if (workflowHistory != null)
                    {
                        _logger.LogInformation("Start updating OT" + overtime.ReferenceNumber + " - " + overtime.Status);
                        // cap nhat lai phieu cu
                        overtime.OldStatus = overtime.Status;
                        overtime.Status = Const.Status.completed;
                        _uow.GetRepository<OvertimeApplication>().Update(overtime);
                    }
                }
                await _uow.CommitAsync();
            }
            return true;
        }
    }
}
