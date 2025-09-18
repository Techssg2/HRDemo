using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UpdateUserInfoFromSapJobV2.src.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using UpdateUserInfoFromSapJobV2.src.ModelEntity;
using UpdateUserInfoFromSapJobV2.src.ViewModel;

namespace UpdateUserInfoFromSapJobV2.src.SAP
{
    public class TrackingBO : ITrackingBO
    {
        protected readonly IUnitOfWork _uow;
        protected readonly ILogger _logger;
        protected readonly SAPSettingsSection _sapSetting;
        protected string sap_client;
        protected string format;
        public TrackingBO(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _sapSetting = (SAPSettingsSection)ConfigurationManager.GetSection("sapSettings");
            sap_client = $"sap-client={_sapSetting.Header.SapClient}";
            format = $"$format={_sapSetting.Header.Format}";
            _logger = logger;
        }
        public async Task<TrackingRequest> AddNewTrackingRequest(ResponseExternalDataMappingDTO mappingResponse)
        {
            TrackingRequest trackingRequest = null;
            if (mappingResponse != null)
            {
                trackingRequest = Mapper.Map<TrackingRequest>(mappingResponse);
                if (mappingResponse.AdditionalItem != null)
                {

                }
                if (mappingResponse.AdditionalItem != null)
                {
                    trackingRequest.DeptCode = mappingResponse.AdditionalItem.DeptCode;
                    trackingRequest.DeptName = mappingResponse.AdditionalItem.DeptName;
                    trackingRequest.DivisionCode = mappingResponse.AdditionalItem.DivisionCode;
                    trackingRequest.DivisionName = mappingResponse.AdditionalItem.DivisionName;
                    trackingRequest.ReferenceNumber = mappingResponse.AdditionalItem.ReferenceNumber;
                    trackingRequest.UserName = mappingResponse.AdditionalItem.UserName;
                }
                _uow.GetRepository<TrackingRequest>().Add(trackingRequest);
                try
                {
                    AddTrackingInitData(trackingRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError("AddTrackingInitData: {0} error message: {1}", trackingRequest.ReferenceNumber, ex.Message);
                }

                await _uow.CommitAsync();
            }
            return trackingRequest;
        }

        public async Task<HttpResponseMessage> RetryRequest(string url, string payload)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(_sapSetting.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.AcceptType));
            client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
            string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            var content = StringContentObjectFromJson(payload);
            var response = await client.PostAsync(string.Format("{0}?{1}", url, sap_client), content);
            return response;
        }
        public Task<HttpResponseMessage> RetryRequestNoAwait(string url, string payload)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(_sapSetting.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.AcceptType));
            client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
            string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            var content =StringContentObjectFromJson(payload);
            var response = client.PostAsync(url, content);
            return response;
        }

        public async Task UpdateTrackingRequestByInstance(HttpResponseMessage response, TrackingRequest instance)
        {
            try
            {
                if (response != null)
                {
                    if (response.Content != null)
                    {
                        try
                        {
                            string httpResponseResult = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode) // Success Submit Data
                            {
                                var responseResult = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<SAPAPIResultForSingleItem>(httpResponseResult).D);
                                if (!string.IsNullOrEmpty(responseResult))
                                {
                                    JObject _data = JObject.Parse(responseResult);
                                    instance.Response = httpResponseResult;
                                    instance.Status = _data.GetValue("Status").ToString();
                                    instance.HttpStatusCode = response.StatusCode.ToString();

                                }
                            }
                            else // Fail Submit Data
                            {
                                instance.HttpStatusCode = response.StatusCode.ToString();
                                if (response.StatusCode == HttpStatusCode.Unauthorized)
                                {
                                    instance.Response = "Unauthorized";
                                }
                                else
                                {
                                    instance.Response = httpResponseResult;
                                }
                                instance.Status = Enum.GetName(typeof(HttpStatusCode), response.StatusCode);
                            }

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error at: UpdateTrackingRequestByInstance. Payload: {instance.Payload}. Ex:" + ex.Message);
                        }

                    }
                }
                _uow.GetRepository<TrackingRequest>().Update(instance);
                await _uow.CommitAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddTrackingInitData(TrackingRequest tracking)
        {
            dynamic payload = JsonConvert.DeserializeObject<dynamic>(tracking.Payload);
            if (payload != null)
            {
                TrackingLogInitData LogInitData = new TrackingLogInitData();
                string fromDate = string.Empty;
                string toDate = string.Empty;
                LogInitData.Id = Guid.NewGuid();
                LogInitData.TrackingLogId = tracking.Id;
                LogInitData.SAPCode = payload["Pernr"];
                LogInitData.CreatedByFullName = tracking.UserName;
                LogInitData.Created = tracking.Created;
                LogInitData.Modified = tracking.Modified;
                LogInitData.ReferenceNumber = tracking.ReferenceNumber;
                try
                {
                    if (tracking.Url.Contains("ShiftExchange"))
                    {
                        fromDate = payload["Date"].ToString();
                        toDate = payload["Date"].ToString();
                        LogInitData.Code = payload["Tprog"];
                        LogInitData.FunctionType = "ShiftExchange";

                    }
                    else if (tracking.Url.Contains("OverTime"))
                    {
                        fromDate = payload["Date"].ToString();
                        toDate = payload["Date"].ToString();
                        LogInitData.FunctionType = "OverTime";
                    }
                    else if (tracking.Url.Contains("Resignation"))
                    {
                        fromDate = payload["Begda"].ToString();
                        toDate = payload["Begda"].ToString();
                        LogInitData.FunctionType = "Resignation";
                    }
                    else if (tracking.Url.Contains("MissingTimeclock"))
                    {
                        fromDate = payload["Date"].ToString();
                        toDate = payload["Date"].ToString();
                        LogInitData.FunctionType = "OverTime";
                    }
                    else if (tracking.Url.Contains("LeaveBalance"))
                    {
                        fromDate = payload["Begda"].ToString();
                        toDate = payload["Endda"].ToString();
                        LogInitData.Code = payload["Awart"];
                        LogInitData.FunctionType = "LeaveBalance";
                    }
                    else if (tracking.Url.Contains("Add_EmployeeSet"))
                    {
                        fromDate = payload["Begda"].ToString();
                        toDate = payload["Begda"].ToString();
                        LogInitData.FunctionType = "Employee";
                    }
                    else if (tracking.Url.Contains("TargetPlan"))
                    {
                        //fix - 484
                        var period = payload["Period"].ToString();
                        var year = payload["Zyear"].ToString();
                        if (period != "1")
                        {
                            var previousPeriod = int.Parse(period) - 1;
                            var periodInt = int.Parse(period);
                            if (periodInt > 10)
                            {
                                fromDate = string.Format("{0}{1}{2}", year, previousPeriod, "26");
                                toDate = string.Format("{0}{1}{2}", year, periodInt, "25");
                            }
                            else
                            {
                                fromDate = string.Format("{0}0{1}{2}", year, previousPeriod, "26");
                                if (periodInt == 10)
                                {
                                    toDate = string.Format("{0}{1}{2}", year, periodInt, "25");
                                }
                                else
                                {
                                    toDate = string.Format("{0}0{1}{2}", year, periodInt, "25");
                                }

                            }

                        }
                        else
                        {
                            var previousYear = int.Parse(year) - 1;
                            fromDate = string.Format("{0}{1}{2}", previousYear, "12", "26");
                            toDate = string.Format("{0}{1}{2}", year, "01", "25");
                        }
                        LogInitData.FunctionType = "TargetPlan";
                    }

                    if (!String.IsNullOrEmpty(fromDate))
                    {
                        LogInitData.FromDate = DateTime.ParseExact(fromDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    if (!String.IsNullOrEmpty(toDate))
                    {
                        LogInitData.ToDate = DateTime.ParseExact(toDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    _uow.GetRepository<TrackingLogInitData>().Add(LogInitData);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public static StringContent StringContentObjectFromJson(string jsonContent)
        {
            StringContent result = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");
            return result;
        }
    }



}
