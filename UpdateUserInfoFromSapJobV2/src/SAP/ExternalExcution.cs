
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UpdateUserInfoFromSapJobV2.src.ModelEntity;
using UpdateUserInfoFromSapJobV2.src.Interfaces;
using UpdateUserInfoFromSapJobV2.src.Helpers;


namespace UpdateUserInfoFromSapJobV2.src.SAP
{
    public abstract class ExternalExcution
    {
        public string APIName { get; set; }
        protected string _jsonFile = "";
        private ISAPEntity _sapEntityInfo;
        protected ILogger _log;
        protected IUnitOfWork _uow;
        protected IIntegrationEntity _integrationEntity;
        protected readonly ITrackingBO _trackingBO;
        protected readonly SAPSettingsSection _sapSetting;
        protected List<TrackingRequest> trackingRequests;
        protected string sap_client;
        protected string format;
        protected Guid? ItemId { get; set; }
        public AdditionalItem AdditionalItem { get; set; }
        public ExternalExcution(ILogger logger, IUnitOfWork uow, string jsonFile, IIntegrationEntity integrationEntity, ITrackingBO trackingBO)
        {
            _log = logger;
            _jsonFile = jsonFile;
            _uow = uow;
            _integrationEntity = integrationEntity;
            _trackingBO = trackingBO;
            _sapSetting = (SAPSettingsSection)ConfigurationManager.GetSection("sapSettings");
            sap_client = $"sap-client={_sapSetting.Header.SapClient}";
            format = $"$format={_sapSetting.Header.Format}";
            trackingRequests = new List<TrackingRequest>();
        }
        protected HttpClient ConfigAPI()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(_sapSetting.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.AcceptType));
            client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
            string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            return client;

        }

        protected void SetSAPEntity(ISAPEntity sapEntityInfo)
        {
            _sapEntityInfo = sapEntityInfo;
        }
        protected string BuildUrlInfo(string Predicate, string[] PredicateParameters)
        {
            string param = "";
            string returnValue = string.Empty;
            switch (APIName)
            {
                case "GetLeaveBalanceSet":
                    {
                        param = string.Format(Predicate, PredicateParameters);
                        returnValue = String.Format("{0}/{1}?{2}&{3}&{4}", _sapSetting.SAPGroupDataCollection["Employee"].Value, APIName, param, sap_client, format);
                        break;
                    }
                case "GetCurrentShiftSet":
                    {
                        param = string.Format(Predicate, PredicateParameters);
                        returnValue = String.Format("{0}/{1}?{2}&{3}", _sapSetting.SAPGroupDataCollection["Employee"].Value, APIName, param, format);
                        break;
                    }
                default:
                    if (!string.IsNullOrEmpty(Predicate) && PredicateParameters != null)
                    {
                        param = Predicate;
                        for (int i = 0; i < PredicateParameters.Length; i++)
                        {
                            if (i == PredicateParameters.Length - 1)
                            {
                                param = String.Format("({0})", param.Replace($"@{i}", PredicateParameters[i]));
                            }
                            else
                            {
                                param = String.Format("{0}", param.Replace($"@{i}", PredicateParameters[i]));
                            }
                        }
                    }
                    returnValue = String.Format("{0}/{1}{2}?{3}&{4}", _sapSetting.SAPGroupDataCollection["Employee"].Value, APIName, param, sap_client, format);
                    break;
            }
            return returnValue;
        }
        private string BuildJsonFromObject(ISAPEntity data)
        {
            var jsonData = JsonHelper.GetJsonContentFromFile("Mappings", _jsonFile);
            var mappingFiles = GenericExtension<List<FieldMappingDTO>>.DeserializeObject(jsonData);
            return ProcessingFields(mappingFiles, data, TypeProcessingField.Push);
        }
        public abstract Task SubmitData(bool allowSentoSAP);
        public abstract void ConvertToPayload();
        protected string ProcessingFields(List<FieldMappingDTO> fields, object data, TypeProcessingField type)
        {
            var result = "";
            if (fields.Count > 0)
            {
                if (type == TypeProcessingField.Get)
                {
                    result = data.ToString();
                    foreach (var item in fields)
                    {
                        result = result.SafeReplace(item.TargetField, item.SourceField, true);
                    }
                }
                else
                {
                    result = JsonConvert.SerializeObject(data);
                    foreach (var item in fields)
                    {
                        result = result.SafeReplace(item.SourceField, item.TargetField, true);
                    }
                }

            }
            return result;
        }

        public async Task SubmitAPI(bool allowSentoSAP)
        {
            var client = ConfigAPI();
            HttpResponseMessage response = null;
            ResponseExternalDataMappingDTO mappingResponse = null;
            TrackingRequest instance = null;
            var apiName = GenericExtension<MyClassAttribute>.GetCustomAttributeInfo(_sapEntityInfo).APIName;
            try
            {
                var url = String.Format("{0}/{1}", _sapSetting.SAPGroupDataCollection["Employee"].Value, apiName);
                var payload = BuildJsonFromObject(_sapEntityInfo);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                mappingResponse = new ResponseExternalDataMappingDTO
                {
                    Url = url,
                    Payload = payload,
                    ItemId = ItemId.HasValue ? ItemId.Value : Guid.Empty,
                    AdditionalItem = AdditionalItem
                };
                instance = await _trackingBO.AddNewTrackingRequest(mappingResponse);
                if (allowSentoSAP)
                {
                    response = await ProcessPushToSAP(url, content, client);
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error at: {apiName}. Ex: {{0}}", ex.Message);
                instance.Status = String.Format("Exception: {0}", ex.Message);
                instance.Response = ex.Message;
            }
            finally
            {
                await _trackingBO.UpdateTrackingRequestByInstance(response, instance);
            }
        }
        private async Task<HttpResponseMessage> ProcessPushToSAP(string url, StringContent content, HttpClient client)
        {
            var allowPushToSAP = ConfigurationManager.AppSettings["allowPushToSAP"];
            if (!string.IsNullOrEmpty(allowPushToSAP) && bool.Parse(allowPushToSAP))
            {
                return await client.PostAsync(string.Format("{0}?{1}", url, sap_client), content);
            }
            return null;
        }

        public async Task<TrackingRequest> AddTrackingRequests(ISAPEntity sapEntityInfo, string group)
        {
            var result = new TrackingRequest();
            if (null != sapEntityInfo)
            {
                var attributeInfo = GenericExtension<MyClassAttribute>.GetCustomAttributeInfo(sapEntityInfo);
                if (attributeInfo != null)
                {
                    var apiName = attributeInfo.APIName;
                    var url = String.Format("{0}/{1}", _sapSetting.SAPGroupDataCollection[group].Value, apiName);
                    var payload = BuildJsonFromObject(sapEntityInfo);
                    payload = ReplaceNull(payload);
                    ResponseExternalDataMappingDTO mappingResponse = new ResponseExternalDataMappingDTO
                    {
                        Url = url,
                        Payload = payload,
                        ItemId = ItemId.HasValue ? ItemId.Value : Guid.Empty,
                        AdditionalItem = AdditionalItem,
                        Status = "Fail"
                    };
                    var instance = await _trackingBO.AddNewTrackingRequest(mappingResponse);
                    result = instance;
                }
            }
            return result;
        }

        public async Task<List<TrackingRequest>> AddTrackingRequests(List<ISAPEntity> sapEntityInfos, string group)
        {
            var result = new List<TrackingRequest>();
            foreach (var entity in sapEntityInfos)
            {
                var instance = await AddTrackingRequests(entity, group);
                result.Add(instance);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public async Task SubmitAPIWithTracking(TrackingRequest instance, bool allowPushToSAP)
        {
            var client = ConfigAPI();
            var content = Utilities.StringContentObjectFromJson(instance.Payload);
            HttpResponseMessage response = null;
            try
            {
                if (allowPushToSAP)
                {
                    response = await ProcessPushToSAP(instance.Url, content, client);
                }

            }
            catch (Exception ex)
            {
                _log.LogError($"Error at: {instance.Url}. Payload: {instance.Payload}. Ex: {{0}}", ex.Message);
                instance.Status = String.Format("Exception: {0}", ex.Message);
                instance.Response = ex.Message;
            }
            finally
            {
                await _trackingBO.UpdateTrackingRequestByInstance(response, instance);
            }
        }
        protected async Task<HttpResponseMessage> GetDataExcution(string predicate, string[] predicateParams)
        {
            var client = ConfigAPI();
            HttpResponseMessage response = null;
            var url = BuildUrlInfo(predicate, predicateParams);
            try
            {
                response = await client.GetAsync(url);
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError($"Error at: {url}", ex.Message);
                return null;
            }
        }
        public abstract Task<object> GetData(string predicate, string[] param);
        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }
        public void UpDateAdditionalItem()
        {

        }

    }
}

public enum TypeProcessingField
{
    Get = 1,
    Push = 2
}