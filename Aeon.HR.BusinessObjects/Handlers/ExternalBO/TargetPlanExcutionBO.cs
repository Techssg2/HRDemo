using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;



namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    class TargetPlanExcutionBO : ExternalExcution
    {
        public TargetPlanExcutionBO(ILogger log, IUnitOfWork uow, TargetPlan targetPlan, ITrackingBO trackingBO) : base(log, uow, "targetPlanDataInfo.json", targetPlan, trackingBO)
        {

        }
        public override async void ConvertToPayload()
        {

        }

        public override Task<object> GetData(string predicate, string[] param)
        {
            throw new NotImplementedException();
        }

        public override async Task SubmitData(bool allowSendToSAP)
        {
            var model = (TargetPlan)_integrationEntity;

            List<TargetPlanInfo> listSubmit = new List<TargetPlanInfo>();
            var items = new List<ISAPEntity>();

            //build list of submit to sap
            var targetPlan = await _uow.GetRepository<TargetPlan>(true).FindByIdAsync(model.Id);
            if (targetPlan != null)
            {
                var targetDetails = await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.TargetPlanId == targetPlan.Id);
                foreach (var item in targetDetails)
                {
                    bool isNew = false;
                    TargetPlanInfo sub = listSubmit.FirstOrDefault(x => x.SapCode == item.SAPCode);
                    if (sub == null)
                    {
                        sub = new TargetPlanInfo()
                        {

                            ReferenceNumber = item.TargetPlan.ReferenceNumber,
                            SapCode = item.SAPCode,
                            PeriodMonth = item.TargetPlan.PeriodToDate.Month.ToString(),
                            PeriodYear = item.TargetPlan.PeriodToDate.Year.ToString(),
                            RequestUser = item.TargetPlan.UserSAPCode
                        };
                        isNew = true;
                    }
                    if (item.Type == TypeTargetPlan.Target1)
                    {
                        sub.Target1 = Mapper.Map<List<DateValueItem>>(JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData));
                    }

                    else if (item.Type == TypeTargetPlan.Target2)
                    {
                        sub.Target2 = Mapper.Map<List<DateValueItem>>(JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData));
                    }
                    if (isNew)
                        listSubmit.Add(sub);
                }
            }
            items.AddRange(listSubmit);
            var trackingRequests = await AddTrackingRequests(items, "TargetPlan");
            foreach (var item in trackingRequests)
            {
                await base.SubmitAPIWithTracking(item, allowSendToSAP);
            }
        }

    }
}
