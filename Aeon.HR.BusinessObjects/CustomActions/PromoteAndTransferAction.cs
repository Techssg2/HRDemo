using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.CompleteActions
{

    [Action(Type = typeof(PromoteAndTransfer))]
    public class PromoteAndTransferAction : ICustomAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, string lastOutcome, IDashboardBO dashboardBO, IWorkflowBO workflowBO)
        {
            if (lastOutcome == "Submitted")
            {
                PromoteAndTransfer promoteTransferItem = await uow.GetRepository<PromoteAndTransfer>(true).FindByIdAsync(itemId);
               if(promoteTransferItem != null)
                {
                    promoteTransferItem.NewDeptOrLine.EnableForPromoteActing = false;
                    uow.GetRepository<PromoteAndTransfer>().Update(promoteTransferItem);
                    await uow.CommitAsync();
                }
            }
            else if (lastOutcome == "Rejected")
            {
                PromoteAndTransfer promoteTransferItem = await uow.GetRepository<PromoteAndTransfer>(true).FindByIdAsync(itemId);
               if(promoteTransferItem != null)
                {
                    promoteTransferItem.NewDeptOrLine.EnableForPromoteActing = true;
                    uow.GetRepository<PromoteAndTransfer>().Update(promoteTransferItem);
                    await uow.CommitAsync();
                }
            }
        }
    }
}
