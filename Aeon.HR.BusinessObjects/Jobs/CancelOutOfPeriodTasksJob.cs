using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
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
    public class CancelOutOfPeriodTasksJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;

        public CancelOutOfPeriodTasksJob(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task DoCancelOutOfPeriodTasksJob()
        {
            try
            {
                DaysConfigurationHelper daysConfigHelper = new DaysConfigurationHelper(_uow);
                if (daysConfigHelper.DeadlineOfSubmittingCABStore == DateTime.Now.Day)
                {
                    _logger.LogInformation($"Start run DoCancelOutOfPeriodTasksJob for store at {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                    await StartProcess(daysConfigHelper, true);
                }
                if (daysConfigHelper.DeadlineOfSubmittingCABHQ == DateTime.Now.Day)
                {
                    _logger.LogInformation($"Start run DoCancelOutOfPeriodTasksJob for HQ at {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    await StartProcess(daysConfigHelper, false);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("DoCancelOutOfPeriodTasksJob: " + ex.Message + " StackTrace: " + ex.StackTrace);
            }
        }

        private async Task StartProcess(DaysConfigurationHelper daysConfigHelper, bool isStore)
        {
            try
            {
                List<Guid> itemIDs = new List<Guid>();
                #region Date range
                DateTime now = DateTime.Now;
                DateTime startTime = new DateTime(now.Year, 1, 1);
                DateTime endTime = new DateTime(now.Year, now.Month, daysConfigHelper.SalaryPeriodTo);
				/*if (daysConfigHelper.DeadlineOfSubmittingCABApplication < daysConfigHelper.SalaryPeriodTo)
                {
                    endTime = new DateTime(now.Year, now.Month - 1, daysConfigHelper.SalaryPeriodTo);
                }*/
				if (isStore)
				{
                    if (daysConfigHelper.DeadlineOfSubmittingCABStore < daysConfigHelper.SalaryPeriodTo)
                    {
                        endTime = new DateTime(now.Year, now.Month - 1, daysConfigHelper.SalaryPeriodTo);
                    }
                }
				else
				{
                    if (daysConfigHelper.DeadlineOfSubmittingCABHQ < daysConfigHelper.SalaryPeriodTo)
                    {
                        endTime = new DateTime(now.Year, now.Month - 1, daysConfigHelper.SalaryPeriodTo);
                    }
                }
                #endregion

                #region LeaveApplication
                IEnumerable<LeaveApplication> leaveApplications = _uow.GetLeaveApplications_OutOfPeriod(startTime, endTime, isStore);
                if (leaveApplications != null && leaveApplications.Any())
                {
                    itemIDs.AddRange(leaveApplications.Where(x => x != null).Select(x => x.Id).ToList());
                }
                #endregion

                #region ShiftExchangeApplication
                IEnumerable<ShiftExchangeApplication> shiftExchangeApplications = _uow.GetShiftExchangeApplication_OutOfPeriod(startTime, endTime, isStore);
                if (shiftExchangeApplications != null && shiftExchangeApplications.Any())
                {
                    itemIDs.AddRange(shiftExchangeApplications.Where(x => x != null).Select(x => x.Id).ToList());
                }
                #endregion

                #region OvertimeApplication
                IEnumerable<OvertimeApplication> overtimeApplications = _uow.GetOvertimeApplication_OutOfPeriod(startTime, endTime, isStore);
                if (overtimeApplications != null && overtimeApplications.Any())
                {
                    itemIDs.AddRange(overtimeApplications.Where(x => x != null).Select(x => x.Id).ToList());
                }
                #endregion

                #region MissingTimeClock
                IEnumerable<MissingTimeClock> missingTimeClocks = _uow.GetMissingTimeClock_OutOfPeriod(startTime, endTime, isStore);
                if (missingTimeClocks != null && missingTimeClocks.Any())
                {
                    itemIDs.AddRange(missingTimeClocks.Where(x => x != null).Select(x => x.Id).ToList());
                }
                #endregion

                #region BusinessTripApplication
                IEnumerable<BusinessTripApplication> businessTripApplications = _uow.GetBusinessTripApplication_OutOfPeriod(startTime, endTime, isStore);
                if (businessTripApplications != null && businessTripApplications.Any())
                {
                    itemIDs.AddRange(businessTripApplications.Where(x => x != null).Select(x => x.Id).ToList());
                }
                #endregion

                #region ResignationApplication
                IEnumerable<ResignationApplication> resignationApplications = _uow.GetResignationApplication_OutOfPeriod(startTime, endTime, isStore);
                if (resignationApplications != null && resignationApplications.Any())
                {
                    itemIDs.AddRange(resignationApplications.Where(x => x != null).Select(x => x.Id).ToList());
                }
                #endregion

                if (itemIDs != null && itemIDs.Any())
                {
                    IEnumerable<WorkflowInstance> workflowInstances = await _uow.GetWorkflowInstance_ByItemIDs(itemIDs);
                    if (workflowInstances != null && workflowInstances.Any())
                    {
                        workflowInstances = workflowInstances.Where(x => x != null && (!x.IsCompleted));
                        if (workflowInstances != null && workflowInstances.Any())
                        {
                            string allItems = workflowInstances.Where(x => x != null && x.Id != null).Select(x => x.Id.ToString()).ToList().Aggregate((x, y) => x + ";#" + y);
                            _logger.LogInformation($"List WF Instance ID will set as OOP on {now.ToString("yyyy-MM-dd")}. Total {workflowInstances.Count()} items", allItems);
                            _logger.LogInformation($"List WF Instance ID will set as OOP on {now.ToString("yyyy-MM-dd")}. {workflowInstances.Select(x => x.ItemReferenceNumber).Aggregate((x, y) => x + "; " + y)} items", allItems);
                            workflowInstances.UpdateAsOutOfPeriod(_uow);
                        }
                        else
                        {
                            _logger.LogInformation($"Not found any workflowInstances need to cancel");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Not found any workflowInstances need to cancel");
                    }
                }
                else
                {
                    _logger.LogInformation($"Not found any items need to cancel");
                }
            }
            catch(Exception ex)
            {
            }
        }
    }
}
