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

    [Action(Type = typeof(OvertimeApplication))]
    public class FillActualHourAction : ICustomAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, string lastOutcome, IDashboardBO dashboardBO, IWorkflowBO workflowBO)
        {
            if (lastOutcome == "PLApproved")
            {
                var details = await uow.GetRepository<OvertimeApplicationDetail>(true).FindByAsync(x => x.OvertimeApplicationId == itemId);
                foreach (var detail in details)
                {
                    detail.ActualHoursFrom = detail.ProposalHoursFrom;
                    detail.ActualHoursTo = detail.ProposalHoursTo;
                }
                await uow.CommitAsync();
            }
        }
    }
}
