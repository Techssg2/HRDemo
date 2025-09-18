using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.CompleteActions
{
    [Action(Type = typeof(TargetPlan))]
    public class TargetPlanCompleteAction : ICompleteAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger)
        {
            var currentTargetPlanDetails = await uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.TargetPlanId == itemId);
            if (currentTargetPlanDetails.Any())
            {
                var groupSAPCodes = currentTargetPlanDetails.Where(t => t.Type == Infrastructure.Enums.TypeTargetPlan.Target1).GroupBy(x => x.SAPCode);
                var sapCodes = groupSAPCodes.Select(x => x.Key);
                var users = await uow.GetRepository<User>().FindByAsync(x => sapCodes.Contains(x.SAPCode));
                foreach (var user in users)
                {
                    var isStore = await uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.UserId == user.Id && x.IsHeadCount && x.Department.IsStore);
                    if (isStore)
                    {
                        var resignation = await uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.UserSAPCode == user.SAPCode && x.Status.Contains("Completed"), "OfficialResignationDate desc");
                        var target = currentTargetPlanDetails.FirstOrDefault(x => x.SAPCode == user.SAPCode && x.Type == Infrastructure.Enums.TypeTargetPlan.Target1);
                        if (target != null)
                        {
                            var dataPRD = new RedundantPRDDataDTO();
                            if (!string.IsNullOrEmpty(user.RedundantPRD))
                            {
                                dataPRD = JsonConvert.DeserializeObject<RedundantPRDDataDTO>(user.RedundantPRD);
                            }
                            var qualityPRDInCurrentMonth = 0.0;
                            var periodFrom = target.TargetPlan.PeriodFromDate;
                            var periodTo = target.TargetPlan.PeriodToDate;
                            var startD = periodFrom.ToLocalTime();
                            var endD = periodTo.ToLocalTime();
                            if (user.StartDate.HasValue)
                            {
                                if (user.StartDate > startD && user.StartDate < endD)
                                {
                                    startD = user.StartDate.Value.Date;
                                }
                                else if (user.StartDate > endD)
                                {
                                    startD = endD;
                                }
                            }
                            if (resignation != null)
                            {
                                var officialDate = resignation.OfficialResignationDate.Date;
                                if (officialDate > user.StartDate.Value.Date && officialDate < endD)
                                {
                                    endD = officialDate;
                                }

                            }
                            while (startD <= endD)
                            {
                                if (startD.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    qualityPRDInCurrentMonth++;
                                }
                                startD = startD.AddDays(1);
                            }
                            var removeData = dataPRD.JsonData.Where(x => x.Year == endD.Year && endD.Month == x.Month);
                            if (removeData.Any())
                            {
                                dataPRD.JsonData = dataPRD.JsonData.Where(x => !removeData.Contains(x)).ToList();
                            }
                            dataPRD.JsonData.Add(new RedundantPRDDTO
                            {
                                Year = endD.Year,
                                Month = endD.Month,
                                PRDRemain = qualityPRDInCurrentMonth == 5 && qualityPRDInCurrentMonth - target.PRDQuality.Value > 0 ? qualityPRDInCurrentMonth - target.PRDQuality.Value : 0
                            }); ;
                            user.RedundantPRD = JsonConvert.SerializeObject(dataPRD);
                        }
                        await uow.CommitAsync();
                    }
                }
            }

        }
    }
}
