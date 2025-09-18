using Aeon.HR.BusinessObjects.Attributes;
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

namespace Aeon.HR.BusinessObjects.CompleteActions
{

    [Action(Type = typeof(OvertimeApplication))]
    public class FillActualHourCompleteAction : ICompleteAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger)
        {
            try
            {
                
                logger.LogInformation("Status item before create new round before get item: " + itemId);
                 
                var item = await uow.GetRepository<OvertimeApplication>(true).FindByIdAsync(itemId);
                if (item.ReferenceNumber.Contains("OVE")) { 
                    logger.LogInformation("Status item before create new round : " + item.Status + " REF : " + item.ReferenceNumber);
                } 
               
                if (item.Status == "Actual Hour Filled")
                {
                    var details = await uow.GetRepository<OvertimeApplicationDetail>(true).FindByAsync(x => x.OvertimeApplicationId == item.Id);
                    var isFollowPlan = true;
                    var instance = await uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.ItemId
                             == itemId, "created desc");
                    if (instance.WorkflowName.ToLower().Contains("revoke"))
                    {
                        isFollowPlan = false;
                    }
                    else
                    {
                        foreach (var detail in details)
                        {
                            if (detail.ProposalHoursTo != detail.ActualHoursTo || detail.ActualHoursFrom != detail.ProposalHoursFrom)
                            {

                                isFollowPlan = false;
                                break;
                            }
                        }
                    }

                    logger.LogInformation("isFollowPlan : " + isFollowPlan + " REF : " + item.ReferenceNumber);
                    if (isFollowPlan)
                    {
                        item.Status = "Completed"; 

                        uow.GetRepository<OvertimeApplication>().Update(item); 
                        logger.LogInformation("Before CommitAsync FillActial - status : " + item.Status + " REF"+ item.ReferenceNumber);

                        await uow.CommitAsync();

                        var ove = await uow.GetRepository<OvertimeApplication>().FindByIdAsync(item.Id);
                        logger.LogInformation("After CommitAsync FillActial - status : " + ove.Status + " REF"+ ove.ReferenceNumber);

                        await workflowBO.PushData(item);
                    }
                    else
                    {
                        var result = await workflowBO.GetWorkflowStatusByItemId(itemId, true);
                        if (result != null)
                        {
                            var wfStatus = result.Object as WorkflowStatusViewModel;
                            var qualifiedTemplate = wfStatus.WorkflowButtons.FirstOrDefault();
                            if (qualifiedTemplate != null)
                            {
                                await workflowBO.StartWorkflow(qualifiedTemplate.Id, item.Id);
                                await workflowBO.Vote(new ViewModels.Args.VoteArgs() { ItemId = itemId, Vote = Infrastructure.Enums.VoteType.Approve });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                logger.LogError("Execute New Round For OT : " + itemId + " " + ex.Message);
            }
            
        }
    }
}
