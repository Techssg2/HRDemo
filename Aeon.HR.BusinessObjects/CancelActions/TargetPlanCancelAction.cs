using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.CustomActions
{
    [Action(Type = typeof(TargetPlan))]
    public class TargetPlanCancelOrRequestedToChangeAction : ICancelAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Cancel Target Plan: {itemId}");
                var targetPlan = await uow.GetRepository<TargetPlan>(true).FindByIdAsync(itemId);
                if (targetPlan != null)
                {
                    var targetPlanDetails = await uow.GetRepository<TargetPlanDetail>(true).FindByAsync(x => x.TargetPlanId == targetPlan.Id);
                    if (targetPlanDetails.Any())
                    {
                        var sapCodes = targetPlanDetails.Select(x => x.SAPCode);
                        var pendingTargetPlanDetails = await uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => sapCodes.Contains(x.SAPCode) && x.PeriodId == targetPlan.PeriodId);
                        if (pendingTargetPlanDetails.Any())
                        {
                            foreach (var item in pendingTargetPlanDetails)
                            {
                                // enable cho user lam lai nhung bi case user da lam target roi sau do lam do nghi viec
                                /*item.IsSent = false;
                                item.IsSubmitted = false;
                                item.TargetPlanDetailId = null;*/
                                // Task #9911: Fix trường hợp cancel Target plan nhưng không xóa được Target plan draft
                                uow.GetRepository<PendingTargetPlanDetail>().Delete(item);
                            }
                        }
                    }
                    targetPlan.Status = "Cancelled";
                    var currentWf = await uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => !x.IsCompleted && x.ItemId == itemId, "Created desc");
                    if (currentWf != null)
                    {
                        currentWf.IsCompleted = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Cancel Target Exception: {ex.Message}");
            }

            await uow.CommitAsync();
        }
    }
}
