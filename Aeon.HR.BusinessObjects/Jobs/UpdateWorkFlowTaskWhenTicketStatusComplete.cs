using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
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
    public class UpdateWorkFlowTaskWhenTicketStatusComplete
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IEmployeeBO _employee;
        private readonly ISSGExBO _bo;

        public UpdateWorkFlowTaskWhenTicketStatusComplete(ILogger logger, IUnitOfWork uow, IEmployeeBO employee, ISSGExBO bo)
        {
            _logger = logger;
            _uow = uow;
            _employee = employee;
            _bo = bo;
        }

        public async Task<bool> DoWork()
        {
            var wfTaskCompleteIsFalses = await _uow.GetRepository<WorkflowTask>(true).FindByAsync(x => x.IsCompleted == false);
            if(wfTaskCompleteIsFalses.Any())
            {
               foreach(var wfTask in wfTaskCompleteIsFalses)
               {
                    switch(wfTask.ItemType)
                    {
                        case "BusinessTripApplication":
                            var recordBTA = await _uow.GetRepository<BusinessTripApplication>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if(recordBTA != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "Acting":
                            var recordActing = await _uow.GetRepository<Acting>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordActing != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "LeaveApplication":
                            var recordLeave= await _uow.GetRepository<LeaveApplication>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordLeave != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "MissingTimeClock":
                            var recordMissing= await _uow.GetRepository<MissingTimeClock>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordMissing != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "OvertimeApplication":
                            var recordOverTime = await _uow.GetRepository<OvertimeApplication>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordOverTime != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "PromoteAndTransfer":
                            var recordPromote = await _uow.GetRepository<PromoteAndTransfer>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordPromote != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "RequestToHire":
                            var recordRequest = await _uow.GetRepository<RequestToHire>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordRequest != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "ResignationApplication":
                            var recordResignation = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordResignation != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "ShiftExchangeApplication":
                            var recordShiftExchange = await _uow.GetRepository<ShiftExchangeApplication>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordShiftExchange != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        case "TargetPlan":
                            var recordTargetPLan = await _uow.GetRepository<TargetPlan>(true).GetSingleAsync(x => x.Id == wfTask.ItemId && x.Status == "Completed");
                            if (recordTargetPLan != null)
                            {
                                wfTask.IsCompleted = true;
                            }
                            break;
                        default:
                            break;
                    }
               }
            }
            await _uow.CommitAsync();
            return true;
        }
    }
}
