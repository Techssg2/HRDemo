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
    public class PromoteAndTransferBO : ExternalExcution
    {
        private IMasterDataB0 _masterbo;
        public PromoteAndTransferBO(ILogger log, IUnitOfWork uow, PromoteAndTransfer promote, ITrackingBO trackingBO, IMasterDataB0 masterbo) : base(log, uow, "promoteAndTransferInfo.json", promote, trackingBO)
        {
            _masterbo = masterbo;
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
                return JsonConvert.DeserializeObject<PromoteAndTransferResponceSAPViewModel>(jsonResponce);
            }
            return null;
        }

        public override async Task SubmitData(bool allowSentoSAP)
        {
            try
            {
                var model = (PromoteAndTransfer)_integrationEntity;
                if (model != null)
                {
                    var promoteAndTransfer = new PromoteAndTransferInfo();
                    Guid createdByUserId = model.CreatedById.Value;
                    User createdByUser = _uow.GetRepository<User>(true).FindBy(x => x.Id == createdByUserId).FirstOrDefault();
                    promoteAndTransfer.RequestFrom = createdByUser?.SAPCode;
                    promoteAndTransfer.EmployeeCode = model?.User?.SAPCode;
                    promoteAndTransfer.EffectiveDate = model.EffectiveDate.ToLocalTime().Date.ToSAPFormat();
                    promoteAndTransfer.Reason = GetReason(model);
                    promoteAndTransfer.Type = GetType(model);
                    promoteAndTransfer.NewTitle = model?.NewTitleCode;
                    promoteAndTransfer.Position = model?.NewDeptOrLine.SAPCode;
                    promoteAndTransfer.PersonelArea = model.PersonnelArea;
                    promoteAndTransfer.PersonelSubarea = model.NewWorkLocationCode;
                    promoteAndTransfer.EmployeeGroupCode = model.EmployeeGroup;
                    promoteAndTransfer.EmployeeSubGroupCode = model.EmployeeSubgroup;

                    var trackingRequests = await AddTrackingRequests(promoteAndTransfer, "Employee");
                    await base.SubmitAPIWithTracking(trackingRequests, allowSentoSAP);

                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message + ". " + ex.StackTrace);
            }

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

        private string GetReason(PromoteAndTransfer model)
        {
            string returnValue = string.Empty;
            try
            {
                switch (model.TypeCode)
                {
                    case "Tran":
                        {
                            if (model.RequestFrom.Equals("EMP", StringComparison.OrdinalIgnoreCase))
                            {
                                returnValue = Reason.EmployeeRequest;// "Employee request"; Code 7
                            }
                            else
                            {
                                returnValue = Reason.AppointedByCompany;// "Appointed by company"; Code 8
                            }
                            break;
                        }
                    case "Pro":
                        {
                            returnValue = Reason.FillUpVacantPosition;// "Fill up vacant position"; Code 17
                            break;
                        }
                    case "ProAndTran":
                        {
                            Guid? requestToHireGuid = model?.NewDeptOrLine?.RequestToHireId;
                            if(requestToHireGuid.HasValue)
                            {
                                RequestToHire requestToHire = _uow.GetRepository<RequestToHire>(true).GetSingle(x => x.Id == requestToHireGuid.Value);
                                if(requestToHire  != null)
                                {
                                    if(requestToHire.ReplacementFor == Infrastructure.Enums.TypeOfNeed.NewPosition)
                                    {
                                        returnValue = Reason.FillUpVacantPosition;// "Fill Up vacant possition"; Code 17
                                    }
                                    else
                                    {
                                        if (model.RequestFrom.Equals("EMP", StringComparison.OrdinalIgnoreCase))
                                        {
                                            returnValue = Reason.PromotionTransfer;// "Promotion transfer"; Code 18
                                        }
                                        else
                                        {
                                            returnValue = Reason.PromotionTransfer_OwnReq;// "Promotion transfer (Ow req.)"; Code 19
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    default:
                        returnValue = string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                returnValue = string.Empty;
                _log.LogError(ex.Message + ". " + ex.StackTrace);
            }
            return returnValue;
        }

        private string GetType(PromoteAndTransfer model)
        {
            string returnValue = string.Empty;
            try
            {
                switch (model.TypeCode)
                {
                    case "Tran":
                        {
                            returnValue = ActionType.Transfer;
                            break;
                        }
                    case "Pro":
                        {
                            returnValue = ActionType.Promote;
                            break;
                        }
                    case "ProAndTran":
                        {
                            returnValue = ActionType.Promote;
                            break;
                        }
                    default:
                        returnValue = string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                returnValue = string.Empty;
                _log.LogError(ex.Message + ". " + ex.StackTrace);
            }
            return returnValue;
        }
    }

    internal struct Reason
    {
        internal const string EmployeeRequest = "7";
        internal const string AppointedByCompany = "8";
        internal const string EndOfOnJobTraining = "9";
        internal const string BusinessClosure = "10";
        internal const string ChangeContractType = "11";
        internal const string FillUpVacantPosition = "17";
        internal const string PromotionTransfer = "18";
        internal const string PromotionTransfer_OwnReq = "19";
        internal const string Acting = "87";
    }

    internal struct ActionType
    {
        internal const string Promote = "Z6";
        internal const string Transfer = "Z3";
    }

    public class PromoteAndTransferResponceSAPViewModel
    {
        public string EmployeeCode { get; set; }
        public string EffectiveDate { get; set; }
        public string NewTitle { get; set; }
        public string Type { get; set; }
        public string ReasonOfPromotion { get; set; }
        public string Position { get; set; }
        public string EmployeeGroupCode { get; set; }
        public string EmployeeSubGroupCode { get; set; }
        public string PersonelSubarea { get; set; }
        public string PersonelArea { get; set; }
    }
}

