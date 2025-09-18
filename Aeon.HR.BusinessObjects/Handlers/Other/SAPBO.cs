using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.CustomSection;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.Other
{
    public class SAPBO : ISAPBO
    {
        private readonly SAPSettingsSection _sapSetting;
       
        public SAPBO()
        {
            _sapSetting = (SAPSettingsSection)ConfigurationManager.GetSection("sapSettings");
        }
        //private string BuildParameters(MasterDataArgs arg)
        //{
        //    var result = "";
        //    if (!string.IsNullOrEmpty(arg.Filter))
        //    {
        //        result += $"$filter={arg.Filter}";
        //    }
        //    if (!string.IsNullOrEmpty(arg.Select))
        //    {
        //        result += '&' + $"$select={arg.Select}";
        //    }
        //    if (arg.Top != default)
        //    {
        //        result += '&' + $"$top={arg.Top}";
        //    }
        //    if (arg.Skip != default)
        //    {
        //        result += '&' + $"$skip={arg.Skip}";
        //    }
        //    return result;
        //}

        private string BuildUrl(RemoteMasterDataDetailInformation arg, string type)
        {
            var result = $"{type}/{arg.ApiName}?$format={_sapSetting.Header.Format}&sap-client={_sapSetting.Header.SapClient}";
            if (!string.IsNullOrEmpty(arg.Filter))
            {
                result = string.Format(result + "&{0}", arg.Filter);
            }
            return result;
        }
        private string BuildUrl(string name, string type)
        {
            var result = $"{type}/{name}";
            return result;
        }
        /// <summary>
        /// Lấy data trực tiếp từ SAP
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ResultDTO> GetMasterData(RemoteMasterDataDetailInformation arg)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var data = new List<object>();
                var url = BuildUrl(arg, _sapSetting.SAPGroupDataCollection["MasterData"].Value);
                HttpResponseMessage response = null;
                try
                {
                    response = await client.GetAsync(url);
                }
                catch (Exception ex)
                {

                }
                if (response != null && response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    IEnumerable<object> syncContact = null;
                    syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<MasterDataViewModel>(arg.Name, httpResponseResult, "Mappings", "remote-master-data-mapping.json");
                    foreach (var item in syncContact)
                    {
                        var properties = item.GetType().GetProperties();
                        dynamic _data = new ExpandoObject();
                        foreach (var property in properties)
                        {
                            Utilities.AddProperty(_data, property.Name, item.GetType().GetProperty(property.Name).GetValue(item));

                        }
                        data.Add(_data);
                    }
                    return new ResultDTO { Object = data };
                }
                else
                {
                    return new ResultDTO { Object = null, ErrorCodes = new List<int> { 1004 }, Messages = new List<string> { "Không có kết nối" } };
                }

            }
        }

        public async Task<ResultDTO> SearchEmployee(string SAPCode)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var data = new List<object>();
                #region Consume GET method  
                var url = BuildUrl("Search_empSet", _sapSetting.SAPGroupDataCollection["Employee"].Value);
                if (!string.IsNullOrEmpty(SAPCode))
                {
                    url = string.Format("{0}?$filter=Pernr eq '*{1}*'&sap-client={2}", url, SAPCode, _sapSetting.Header.SapClient);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string httpResponseResult = await response.Content.ReadAsStringAsync();
                        var syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<EmployeeResponsSearchingViewModel>("EmployeeResponseSearching", httpResponseResult, "Mappings", "employee-for-searching-mapping.json");
                        foreach (var item in syncContact)
                        {
                            var properties = item.GetType().GetProperties();
                            dynamic _data = new ExpandoObject();
                            foreach (var property in properties)
                            {
                                Utilities.AddProperty(_data, property.Name, item.GetType().GetProperty(property.Name).GetValue(item));
                            }
                            data.Add(_data);
                        }
                        var currentData = data.FirstOrDefault();
                        return new ResultDTO { Object = currentData };
                    }
                    else
                    {
                        return new ResultDTO { Object = null, ErrorCodes = new List<int> { (int)response.StatusCode }, Messages = new List<string> { response.StatusCode.ToString() } };
                    }
                }
                else
                {
                    return new ResultDTO { Object = null, ErrorCodes = new List<int> { 1004 }, Messages = new List<string> { "Not Found Connection" } };
                }

                #endregion
            }
        }

        public async Task<string> CheckValidEmployeeFromSAP(string[] keys)
        {
            var result = "";
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var url = BuildUrl("Search_empSet", _sapSetting.SAPGroupDataCollection["Employee"].Value);
                if (keys.Length > 0)
                {
                    url = string.Format("{0}?$filter=Icnum eq '{1}'&sap-client={2}", url, keys[0], _sapSetting.Header.SapClient);
                    if (keys.Length > 1)
                    {
                        var mappingKeys = string.Join(",", keys);
                        url = string.Format("{0}?$filter=Icnum eq '{1}'&sap-client={2}", url, mappingKeys, _sapSetting.Header.SapClient);
                    }
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string httpResponseResult = await response.Content.ReadAsStringAsync();
                        var syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<EmployeeResponsSearchingViewModel>("EmployeeResponseSearching", httpResponseResult, "Mappings", "employee-for-searching-mapping.json");
                        if (syncContact.Any())
                        {
                            result = ((EmployeeResponsSearchingViewModel)syncContact.FirstOrDefault()).SAPCode;
                        }
                    }
                }
            }
            return result;
        }
        public async Task<ResultDTO> GetUsers(UserSAPArg arg)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var data = new List<object>();
                #region Consume GET method  
                var url = BuildUrl("Search_empSet", _sapSetting.SAPGroupDataCollection["Employee"].Value);
                if (string.IsNullOrEmpty(arg.Predicate))
                {
                    url = string.Format("{0}?$filter=Pernr eq '**'&$top={1}&sap-client={2}", url, arg.Limit, _sapSetting.Header.SapClient);
                }
                else
                {
                    url = string.Format("{0}?$filter=Vorna eq '*{1}*'and Nachn eq '*{2}*'and Pernr eq '*{3}*'&$top={4}&sap-client={5}", url, arg.Predicate, arg.Predicate, arg.Predicate, arg.Limit, _sapSetting.Header.SapClient);
                }
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<EmployeeResponsSearchingViewModel>("EmployeeResponseSearching", httpResponseResult, "Mappings", "employee-for-searching-mapping.json");
                    foreach (var item in syncContact)
                    {
                        var properties = item.GetType().GetProperties();
                        dynamic _data = new ExpandoObject();
                        foreach (var property in properties)
                        {
                            Utilities.AddProperty(_data, property.Name, item.GetType().GetProperty(property.Name).GetValue(item));
                        }
                        data.Add(_data);
                    }

                    return new ResultDTO { Object = new ArrayResultDTO { Data = data } };
                }
                else
                {
                    return new ResultDTO { Object = null, ErrorCodes = new List<int> { (int)response.StatusCode }, Messages = new List<string> { response.StatusCode.ToString() } };
                }

                #endregion
            }
        }

        public async Task<ResultDTO> GetMasterDataEmployeeList(string empSubGroup) {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var data = new List<object>();
                #region Consume GET method  
                var url = BuildUrl("GetEmployeeGroupCodeSet", _sapSetting.SAPGroupDataCollection["GetEmployeeGroupCodeSet"].Value);
                if (!string.IsNullOrEmpty(empSubGroup))
                {
                    url = string.Format("{0}?$filter=Persk eq '{1}'&sap-client={2}", url, empSubGroup, _sapSetting.Header.SapClient);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string httpResponseResult = await response.Content.ReadAsStringAsync();
                        var syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<EmployeeSubGroupViewModel>("EmployeeSubGroup", httpResponseResult, "Mappings", "employee-sub-group-mapping.json");
						foreach (var item in syncContact)
						{
							var properties = item.GetType().GetProperties();
							dynamic _data = new ExpandoObject();
							foreach (var property in properties)
							{
								Utilities.AddProperty(_data, property.Name, item.GetType().GetProperty(property.Name).GetValue(item));
							}
							data.Add(_data);
						}
                        return new ResultDTO { Object = new ArrayResultDTO { Data = data } };
                    }
                    else
                    {
                        return new ResultDTO { Object = null, ErrorCodes = new List<int> { (int)response.StatusCode }, Messages = new List<string> { response.StatusCode.ToString() } };
                    }
                }
                else
                {
                    return new ResultDTO { Object = null, ErrorCodes = new List<int> { 1004 }, Messages = new List<string> { "Not Found Connection" } };
                }

                #endregion
            }
        }

        public async Task<ResultDTO> GetNewWorkLocationList(string newWorkLocationText) 
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var data = new List<object>();
                #region Consume GET method  
                var url = BuildUrl("GetWorkLocationSet", _sapSetting.SAPGroupDataCollection["GetWorkLocationSet"].Value);
                if(string.IsNullOrEmpty(newWorkLocationText))
                {
                    url = string.Format("{0}?sap-client={1}", url, _sapSetting.Header.SapClient);
                }
                else
                {
                    url = string.Format("{0}?$filter=Btext eq '{2}'&sap-client={1}", url, _sapSetting.Header.SapClient, newWorkLocationText);
                }
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<NewWorkLocationViewModel>("NewWorkLocationGroup", httpResponseResult, "Mappings", "new-work-location-mapping.json");
                    foreach (var item in syncContact)
                    {
                        var properties = item.GetType().GetProperties();
                        dynamic _data = new ExpandoObject();
                        foreach (var property in properties)
                        {
                            Utilities.AddProperty(_data, property.Name, item.GetType().GetProperty(property.Name).GetValue(item));
                        }
                        data.Add(_data);
                    }

                    if (string.IsNullOrEmpty(newWorkLocationText))
                    {
                        return new ResultDTO { Object = data };
                    }
                    else
                    {
                        return new ResultDTO { Object = data.FirstOrDefault() };
                    }
                }
                else
                {
                    return new ResultDTO { Object = null, ErrorCodes = new List<int> { (int)response.StatusCode }, Messages = new List<string> { response.StatusCode.ToString() } };
                }

                #endregion
            }
        }

        public async Task<ResultDTO> GetNewWorkLocationListV2(string newWorkLocationCode)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var data = new List<object>();
                #region Consume GET method  
                var url = BuildUrl("GetWorkLocationSet", _sapSetting.SAPGroupDataCollection["GetWorkLocationSet"].Value);

                if (!string.IsNullOrEmpty(newWorkLocationCode))
                {
                    url = string.Format("{0}?$filter=(Btrtl eq '{2}')&sap-client={1}", url, _sapSetting.Header.SapClient, newWorkLocationCode);
                }
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<NewWorkLocationViewModel>("NewWorkLocationGroup", httpResponseResult, "Mappings", "new-work-location-mapping.json");
                    foreach (var item in syncContact)
                    {
                        var properties = item.GetType().GetProperties();
                        dynamic _data = new ExpandoObject();
                        foreach (var property in properties)
                        {
                            Utilities.AddProperty(_data, property.Name, item.GetType().GetProperty(property.Name).GetValue(item));
                        }
                        data.Add(_data);
                    }

                    return new ResultDTO { Object = data.FirstOrDefault() };
                }
                else
                {
                    return new ResultDTO { Object = null, ErrorCodes = new List<int> { (int)response.StatusCode }, Messages = new List<string> { response.StatusCode.ToString() } };
                }

                #endregion
            }
        }

    }
}