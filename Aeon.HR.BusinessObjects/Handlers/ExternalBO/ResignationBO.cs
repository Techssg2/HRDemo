using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.ExternalHelper.SAP;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    class ResignationBO : ExternalExcution
    {


        //: base(log, uow, "resignationDataInfo.json") { }

        public ResignationBO(ILogger log, IUnitOfWork uow, ResignationApplication resignation, ITrackingBO trackingBO) : base(log, uow, "resignationDataInfo.json", resignation, trackingBO)
        {

        }
        public override void ConvertToPayload()
        {
            var model = (ResignationApplication)_integrationEntity;
            if (model != null)
            {
                ItemId = model.Id;
                ResignationDataInfo data = Mapper.Map<ResignationDataInfo>(model);
                /*if (model.SuggestionForLastWorkingDay.HasValue)
                {
                    data.OfficialResignationDate = model.SuggestionForLastWorkingDay.Value.AddDays(1).LocalDateTime.ToSAPFormat();
                } else
                {
                    data.OfficialResignationDate = model.OfficialResignationDate.LocalDateTime.ToSAPFormat();
                }*/
                // HR-993
                if (model.SubmitDate.HasValue)
                {
                    data.SubmitDate = model.SubmitDate.Value.LocalDateTime.ToSAPFormat();
                }
                // HR-964
                data.OfficialResignationDate = model.OfficialResignationDate.LocalDateTime.ToSAPFormat();
                SetSAPEntity(data);
            }
        }

        public override async Task<object> GetData(string predicate, string[] predicateParameter)
        {
            HttpResponseMessage response = await base.GetDataExcution(predicate, predicateParameter);
            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                string httpResponseResult = await response.Content.ReadAsStringAsync();
                var responseResult = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<SAPAPIResultForSingleItem>(httpResponseResult).D);
                var jsonData = JsonHelper.GetJsonContentFromFile("Mappings", _jsonFile);
                var mappingFiles = GenericExtension<List<FieldMappingDTO>>.DeserializeObject(jsonData);
                var jsonResponce = ProcessingFields(mappingFiles, responseResult, TypeProcessingField.Get);
                return JsonConvert.DeserializeObject<ResignationApplicationResponceSAPViewModel>(jsonResponce);
            }
            return null;
        }

        public override async Task SubmitData(bool allowSendToSAP) => await base.SubmitAPI(allowSendToSAP);

    }

    public class ResignationApplicationResponceSAPViewModel
    {
        public string EmployeeCode { get; set; }
        public string OfficialResignationDate { get; set; }
        public string ActionType { get; set; }
        public string Reason { get; set; }
    }
}