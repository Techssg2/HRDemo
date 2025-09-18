using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetPlanTesting.ImportData;

namespace Aeon.HR.BusinessObjects.CompleteActions
{
    [Action(Type = typeof(ResignationApplication))]
    public class ResignationCompleteAction : ICompleteAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger)
        {
            List<string> ignoreStatus = new List<string>() { "Rejected", "Cancelled", "Completed" };
            bool isCheckTargetPlan = true;
            //neu resignationApplication co status la Completed thi xoa ngay cua targetPlanDetail tu ngay OfficialResignationDate cua phieu resignation
            var resignationApplication = await uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.Id == itemId);
            if (resignationApplication != null)
            {
                TargetPlanPeriod targetPeriod = await uow.GetRepository<TargetPlanPeriod>().GetSingleAsync(x => x.FromDate <= resignationApplication.OfficialResignationDate && x.ToDate >= resignationApplication.OfficialResignationDate);
                if (targetPeriod != null)
                {
                    //Cap nhat Target cho phieu target
                    var targetPlanDetails = await uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.SAPCode == resignationApplication.UserSAPCode && x.TargetPlan.PeriodId == targetPeriod.Id && !ignoreStatus.Contains(x.TargetPlan.Status));
                    if (targetPlanDetails.Any())
                    {
                        foreach (var targetPlanDetail in targetPlanDetails)
                        {
                            if (targetPlanDetail.Type == Infrastructure.Enums.TypeTargetPlan.Target1)
                            {
                                var targets = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(targetPlanDetail.JsonData);
                                //trong targets neu co target nao co date >= officialResignationDate thi xoa target do
                                foreach (var target in targets)
                                {
                                    DateTime targetDate = DateTime.ParseExact(target.date, "yyyyMMdd", CultureInfo.InvariantCulture);
                                    if (targetDate >= resignationApplication.OfficialResignationDate.Date)
                                    {
                                        target.value = "";
                                    }
                                }
                                targetPlanDetail.ALHQuality = targets.Where(x => x.value.Contains("AL")).Count();
                                targetPlanDetail.ERDQuality = targets.Where(x => x.value.Contains("ERD")).Count();
                                targetPlanDetail.PRDQuality = targets.Where(x => x.value.Contains("PRD")).Count();
                                targetPlanDetail.DOFLQuality = targets.Where(x => x.value.Contains("DOFL")).Count();
                                targetPlanDetail.JsonData = JsonConvert.SerializeObject(targets);
                            }
                            else if (targetPlanDetail.Type == Infrastructure.Enums.TypeTargetPlan.Target2)
                            {
                                var targets = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(targetPlanDetail.JsonData);
                                //trong targets neu co target nao co date >= officialResignationDate thi xoa target do
                                foreach (var target in targets)
                                {
                                    DateTime targetDate = DateTime.ParseExact(target.date, "yyyyMMdd", CultureInfo.InvariantCulture);
                                    if (targetDate >= resignationApplication.OfficialResignationDate.Date)
                                    {
                                        target.value = "";
                                    }
                                }
                                targetPlanDetail.ALHQuality = (float)targets.Where(x => x.value.Contains("ALH1") || x.value.Contains("ALH2")).Count() / 2;
                                targetPlanDetail.ERDQuality = (float)targets.Where(x => x.value.Contains("ERD1") || x.value.Contains("ERD2")).Count() / 2;
                                targetPlanDetail.PRDQuality = (float)targets.Where(x => x.value.Contains("PRD1") || x.value.Contains("PRD2")).Count() / 2;
                                targetPlanDetail.DOFLQuality = (float)targets.Where(x => x.value.Contains("DOH1") || x.value.Contains("DOH2")).Count() / 2;
                                targetPlanDetail.JsonData = JsonConvert.SerializeObject(targets);
                            }
                        }
                        await uow.CommitAsync();
                    }


                    //Cap nhat Target cho phieu pending target
                    var pendingTargetPlanDetails = await uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => x.SAPCode == resignationApplication.UserSAPCode && x.PeriodId == targetPeriod.Id);
                    if (pendingTargetPlanDetails.Any())
                    {
                        foreach (var pendingTargetPlanDetail in pendingTargetPlanDetails)
                        {
                            if (pendingTargetPlanDetail.Type == Infrastructure.Enums.TypeTargetPlan.Target1)
                            {
                                var pendingtargets = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(pendingTargetPlanDetail.JsonData);
                                //trong targets neu co target nao co date >= officialResignationDate thi xoa target do
                                foreach (var pendingtarget in pendingtargets)
                                {
                                    DateTime pendingtargetDate = DateTime.ParseExact(pendingtarget.date, "yyyyMMdd", CultureInfo.InvariantCulture);
                                    if (pendingtargetDate >= resignationApplication.OfficialResignationDate.Date)
                                    {
                                        pendingtarget.value = "";
                                    }
                                }
                                pendingTargetPlanDetail.ALHQuality = pendingtargets.Where(x => x.value.Contains("AL")).Count();
                                pendingTargetPlanDetail.ERDQuality = pendingtargets.Where(x => x.value.Contains("ERD")).Count();
                                pendingTargetPlanDetail.PRDQuality = pendingtargets.Where(x => x.value.Contains("PRD")).Count();
                                pendingTargetPlanDetail.DOFLQuality = pendingtargets.Where(x => x.value.Contains("DOFL")).Count();
                                pendingTargetPlanDetail.JsonData = JsonConvert.SerializeObject(pendingtargets);
                            }
                            else if (pendingTargetPlanDetail.Type == Infrastructure.Enums.TypeTargetPlan.Target2)
                            {
                                var pendingtargets = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(pendingTargetPlanDetail.JsonData);
                                //trong targets neu co target nao co date >= officialResignationDate thi xoa target do
                                foreach (var pendingtarget in pendingtargets)
                                {
                                    DateTime pendingtargetDate = DateTime.ParseExact(pendingtarget.date, "yyyyMMdd", CultureInfo.InvariantCulture);
                                    if (pendingtargetDate >= resignationApplication.OfficialResignationDate.Date)
                                    {
                                        pendingtarget.value = "";
                                    }
                                }
                                pendingTargetPlanDetail.ALHQuality = (float)pendingtargets.Where(x => x.value.Contains("ALH1") || x.value.Contains("ALH2")).Count() / 2;
                                pendingTargetPlanDetail.ERDQuality = (float)pendingtargets.Where(x => x.value.Contains("ERD1") || x.value.Contains("ERD2")).Count() / 2;
                                pendingTargetPlanDetail.PRDQuality = (float)pendingtargets.Where(x => x.value.Contains("PRD1") || x.value.Contains("PRD2")).Count() / 2;
                                pendingTargetPlanDetail.DOFLQuality = (float)pendingtargets.Where(x => x.value.Contains("DOH1") || x.value.Contains("DOH2")).Count() / 2;
                                pendingTargetPlanDetail.JsonData = JsonConvert.SerializeObject(pendingtargets);
                            }
                        }
                        await uow.CommitAsync();
                    }

                }
            }

        }
    }
}
