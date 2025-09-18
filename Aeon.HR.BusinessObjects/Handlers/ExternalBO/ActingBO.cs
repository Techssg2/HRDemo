using Aeon.HR.API.ExternalItem;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.ExternalHelper.SAP;
using Aeon.HR.ViewModels;
using AutoMapper;
using Aeon.HR.ViewModels.Args;

namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    public class ActingInfoBO : ExternalExcution
    {
        private readonly Acting model;
        private IMasterDataB0 _masterbo;
        public ActingInfoBO(ILogger log, IUnitOfWork uow, Acting acting, ITrackingBO _trackingBO, IMasterDataB0 masterbo) : base(log, uow, "actingInfo.json", acting, _trackingBO)
        {
            model = acting;
            _masterbo = masterbo;
        }
        public override async Task SubmitData(bool allowSentoSAP)
        {
            var model = (Acting)_integrationEntity;
            if (model != null && !model.Status.Equals("Completed"))
            {
                if (model != null)
                {
                    Func<Acting, DateTimeOffset?> GetActingEndDate = (Acting actingData) =>
                   {
                       DateTimeOffset? returnValue = null;
                       try
                       {
                           List<DateTimeOffset?> dates = new List<DateTimeOffset?>() { model.Period1To, model.Period2To, model.Period3To, model.Period4To };
                           returnValue = dates.Where(x => x.HasValue).OrderByDescending(x => x.Value).FirstOrDefault();
                       }
                       catch (Exception ex)
                       {
                           _log.LogError(ex.Message, ex.StackTrace);
                       }
                       return returnValue;
                   };

                    Func<Guid, Department> GetDepartmentSAP = (Guid depId) =>
                    {
                        Department returnDepartment = null;
                        try
                        {
                            returnDepartment = _uow.GetRepository<Department>().GetSingle(x => x.Id == depId);
                        }
                        catch
                        {

                        }
                        return returnDepartment;
                    };

                    var actionInfo = new ActingInfo();
                    Guid createdByUserId = model.CreatedById.Value;
                    User createdByUser = _uow.GetRepository<User>(true).FindBy(x => x.Id == createdByUserId).FirstOrDefault();

                    actionInfo.RequestFrom = createdByUser?.SAPCode;
                    actionInfo.EmployeeCode = model?.User?.SAPCode;
                    if(model.Period1From.HasValue)
                    {
                        actionInfo.EffectiveDate = model.Period1From.Value.ToLocalTime().Date.ToSAPFormat();
                    }
                    actionInfo.Reason = "87";
                    actionInfo.Type = "Y6";
                    if(model.DepartmentSAPId.HasValue)
                    {
                        Department DepSAP = GetDepartmentSAP(model.DepartmentSAPId.Value);
                        if(DepSAP != null)
                        {
                            actionInfo.Position = DepSAP.SAPCode;
                        }
                    }
                    actionInfo.PersonelArea = model.NewPersonnelArea;
                    actionInfo.PersonelSubarea = model.NewWorkLocationCode;
                    actionInfo.EmployeeGroupCode = model.EmployeeGroup;
                    actionInfo.EmployeeSubGroupCode = model.EmployeeSubgroup;

                    var trackingRequests = await AddTrackingRequests(actionInfo, "Employee");
                    await base.SubmitAPIWithTracking(trackingRequests, allowSentoSAP);
                }
            }
        }
        public override void ConvertToPayload()
        {



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
                return JsonConvert.DeserializeObject<ActingResponceSAPViewModel>(jsonResponce);
            }
            return null;
        }
        private async Task<MasterDataViewModel> GetMasterNameDataByCode(string type, string code)
        {
            MasterDataViewModel result = null;
            if (!string.IsNullOrEmpty(code))
            {
                var masterData = await _masterbo.GetMasterDataValues(new MasterDataArgs { Name = type, ParentCode = code });
                if (masterData.IsSuccess)
                {
                    var arrayData = JsonConvert.DeserializeObject<ArrayResultDTO>(JsonConvert.SerializeObject(masterData.Object)).Data;
                    List<MasterDataViewModel> masterDataValues = Mapper.Map<List<MasterDataViewModel>>(arrayData);
                    result = masterDataValues.FirstOrDefault();
                }
            }
            return result;

        }
    }

    public class ActingResponceSAPViewModel
    {
        public string EmployeeCode { get; set; }
        public string TitleInActingPeriod { get; set; }
        public string ActingFrom { get; set; }
        public string ActionType { get; set; }
        public string ReasonforActing { get; set; }
        public string Position { get; set; }
        public string EmployeeGroupCode { get; set; }
        public string EmployeeSubGroupCode { get; set; }
        public string PersonelSubarea { get; set; }
        public string PersonelArea { get; set; }
    }
}

