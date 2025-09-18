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
    [Action(Type = typeof(PromoteAndTransfer))]
    public class PromoteTransferCancelAction : ICancelAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Cancel Promote And Transfer: {itemId}");
                var promoteAndTransfer = await uow.GetRepository<PromoteAndTransfer>(true).FindByIdAsync(itemId);
                if (promoteAndTransfer != null && promoteAndTransfer.NewDeptOrLine != null)
                {

                    logger.LogInformation($"Set Department ID: {promoteAndTransfer.NewDeptOrLineId} - Name: {promoteAndTransfer.NewDeptOrLineName} as EnableForPromoteActing = true");
                    promoteAndTransfer.NewDeptOrLine.EnableForPromoteActing = true; 
                    uow.GetRepository<PromoteAndTransfer>().Update(promoteAndTransfer);
                    await uow.CommitAsync();
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
