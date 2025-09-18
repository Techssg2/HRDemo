using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.BTA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.RequestToChangeActions
{
    [Action(Type = typeof(TargetPlan))]
    public class TargetPlan_RequestToChangeAction : IRequestToChangeAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger, string stepName)
        {
            //if (stepName.Equals("admin manager approval", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    string[] workFlowApply = new[] { "BTA for G1, G2, G3, G4 HQ", "BTA for G5 up (HQ)", "BTA for G4, G5 Store", "BTA for G1, G2, G3 Store" };
            //    var currentWf = await uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => !x.IsCompleted && x.ItemId == itemId, "Created desc");
            //    if (currentWf != null && workFlowApply.Contains(currentWf.WorkflowName))
            //    {
            //        var businessTrip = await uow.GetRepository<BusinessTripApplication>(true).FindByIdAsync(itemId);
            //        if (businessTrip != null && businessTrip.Status.Equals("requested to change", StringComparison.CurrentCultureIgnoreCase))
            //        {
            //            businessTrip.IsRequestToChange = true;
            //            await uow.CommitAsync();
            //        }
            //    }
            //}

        }
    }
}
