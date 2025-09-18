using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.CustomActions
{
    [Action(Type = typeof(ResignationApplication))]
    public class ResignationSubmitAction : ICustomAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, string lastOutcome, IDashboardBO dashboardBO, IWorkflowBO workflowBO)
        {
            if (lastOutcome == "Submitted")
            {
                var existItem = await uow.GetRepository<ResignationApplication>().FindByIdAsync(itemId);
                if (existItem != null && !existItem.IsExpiredLaborContractDate)
                {
                    var now = DateTimeOffset.Now;
                    var instances = await uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.ItemId == existItem.Id, "Created desc");
                    if (instances.Any() && instances.Count() > 1)
                    {
                        var firstInstance = instances.LastOrDefault();
                        var histories = await uow.GetRepository<WorkflowHistory>().FindByAsync<WorkflowHistoryViewModel>(x => x.InstanceId == firstInstance.Id && x.Outcome == "Submitted", "Created asc");
                        if (histories != null && histories.Any())
                        {
                            var firstHistory = histories.FirstOrDefault();
                            if (firstHistory != null)
                            {
                                now = firstHistory.Created;
                            }
                        }
                    }

                    var expectedDate = now.Date.AddDays(GetNumberDayFromContractType(existItem.ContractTypeCode));
                    if (expectedDate > existItem.OfficialResignationDate)
                    {
                        existItem.OfficialResignationDate = expectedDate;
                        existItem.SuggestionForLastWorkingDay = existItem.OfficialResignationDate.AddDays(-1);
                        await uow.CommitAsync();
                    }

                }
            }
        }
        private int GetNumberDayFromContractType(ContractType type)
        {
            switch (type)
            {
                case ContractType.OneDays:
                    return 1;
                case ContractType.ThreeDays:
                    return 3;
                case ContractType.ThirtyDays:
                    return 30;
                case ContractType.FourtyFive:
                    return 45;
            }
            return 0;
        }
    }
}
