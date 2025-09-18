using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    class MissingTimeClockBO : ExternalExcution
    {
        //: base(log,uow, "missingTimeclockInfo.json")
        private IMasterDataB0 _masterbo;
        public MissingTimeClockBO(ILogger log, IUnitOfWork uow, MissingTimeClock missingTimeClock, ITrackingBO trackingBO, IMasterDataB0 masterbo) : base(log, uow, "missingTimeclockInfo.json", missingTimeClock, trackingBO)
        {
            _masterbo = masterbo;
        }
        public override void ConvertToPayload()
        {
        }

        public override Task<object> GetData(string predicate, string[] param)
        {
            throw new NotImplementedException();
        }

        public override async Task SubmitData(bool allowSentoSAP)
        {
            var model = (MissingTimeClock)_integrationEntity;
            if (model != null)
            {
                ItemId = model.Id;
                var items = new List<ISAPEntity>();
                var listMissingTimeClockDetails = JsonConvert.DeserializeObject<List<MissingTimeClockItemDetail>>(model.ListReason);

                //AEON-1740: Đối với Type = Out (Type)
                //Nếu Actual Time >= giờ From của mã ca => thì khi sinh Payload sẽ giữ nguyên ngày 
                ResultDTO ShiftCodeData = await _masterbo.GetMasterDataValues(new MasterDataArgs() { Name = "ShiftCode", ParentCode = string.Empty });
                ArrayResultDTO ShiftCodeArray = ShiftCodeData.Object as ArrayResultDTO;
                List<MasterExternalDataViewModel> ShiftCode_MasterData = ShiftCodeArray.Data as List<MasterExternalDataViewModel>;
                ShiftCodeDetailCollection shiftCodeCollection = new ShiftCodeDetailCollection(ShiftCode_MasterData);

                foreach (var item in listMissingTimeClockDetails)
                {
                    
                    var data = Mapper.Map<MissingTimeClockInfo>(model);
                    data.ActualTimeIn = String.Format("{0}{1}", DateTime.Parse(item.ActualTime).ToLocalTime().ToString("HHmm"), "00");

                    //AEON-1740: Đối với Type = Out (Type)
                    //Nếu Actual Time >= giờ From của mã ca => thì khi sinh Payload sẽ giữ nguyên ngày 
                    ShiftCodeDetail shiftCodeDetail = shiftCodeCollection.GetDetailsByCode(item.ShiftCode);
                    var shiftStartTime = new TimeSpan();
                    if (DateTime.TryParseExact(shiftCodeDetail.StartTime, "HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
                    {
                        shiftStartTime = parsedTime.TimeOfDay;
                    }

                    var actualTime = new TimeSpan();
                    if (DateTime.TryParseExact(data.ActualTimeIn, "HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime1))
                    {
                        actualTime = parsedTime1.TimeOfDay;
                    }

                    if (item.TypeActualTime == 2 && (item.Previous == "X" || item.Previous == "x") && actualTime < shiftStartTime) // 1: in 2: out
                    {
                        item.Date = item.Date.AddDays(1);
                    }
                    data.Date = item.Date.ToLocalTime().ToSAPFormat();
                    items.Add(data);
                    //await base.SubmitAPI();
                }
                var trackingRequests = await AddTrackingRequests(items, "Employee");
                foreach (var item in trackingRequests)
                {
                    await base.SubmitAPIWithTracking(item, allowSentoSAP);
                }
            }
        }
    }
}