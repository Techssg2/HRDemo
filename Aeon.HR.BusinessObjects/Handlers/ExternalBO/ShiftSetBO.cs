using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.ExternalHelper.SAP;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    class ShiftSetBO : ExternalExcution
    {


        //: base(log, uow, "shiftExchangeDataInfo.json") { }
        public ShiftSetBO(ILogger log, IUnitOfWork uow, ShiftExchangeApplication shiftExchange, ITrackingBO trackingBO) : base(log, uow, "shiftExchangeDataInfo.json", shiftExchange, trackingBO)
        {

        }

        public override void ConvertToPayload()
        {
            throw new NotImplementedException();
        }

        public override async Task<object> GetData(string predicate, string[] predicateParameter)
        {

            HttpResponseMessage response = await base.GetDataExcution(predicate, predicateParameter);
            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                string httpResponseResult = await response.Content.ReadAsStringAsync();
                var responseResult = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<SAPAPIResultForArray>(httpResponseResult).D.Results);
                var jsonData = JsonHelper.GetJsonContentFromFile("Mappings", _jsonFile);
                var mappingFiles = GenericExtension<List<FieldMappingDTO>>.DeserializeObject(jsonData);
                var jsonResponce = ProcessingFields(mappingFiles, responseResult, TypeProcessingField.Get);
                var res = JsonConvert.DeserializeObject<List<ShiftSetResponceSAPViewModel>>(jsonResponce);
                return res;
            }
            return null;
        }

        public override Task SubmitData(bool allowSendToSAP)
        {
            throw new NotImplementedException();
        }
    }
}
