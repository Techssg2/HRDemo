using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UpdateUserInfoFromSapJobV2.src.ModelEntity;
using System.Configuration;
using Aeon.UpdateUserInfoFromSapUpdate.Utilities;
using static Aeon.UpdateUserInfoFromSapUpdate.Utilities.Utilities;
using System.Globalization;
using Newtonsoft.Json;
using UpdateUserInfoFromSapJobV2.src.SAP;
using UpdateUserInfoFromSapJobV2.src.Interfaces;
using UpdateUserInfoFromSapJobV2.src.ViewModel;
using System.IO;



namespace UpdateUserInfoFromSapUpdate.src
{
    public class SAPBO
    {
        private readonly SAPSettingsSection _sapSetting;
        private readonly IIntegrationExternalServiceBO _externalServiceBO;
        public SAPBO() {
            _sapSetting = (SAPSettingsSection)ConfigurationManager.GetSection("sapSettings");
         
        }

        private string BuildUrl(string name, string type)
        {
            var result = $"{type}/{name}";
            return result;
        }

        public async Task<DateTime?> GetJoiningDateOfEmployee(string SAPCode)
        {
            DateTime? result = null;
            try
            {
                var res = await SearchEmployee(SAPCode);
                if (res.IsSuccess)
                {
                    var dataInfo = JsonConvert.DeserializeObject<EmployeeResponsSearchingViewModel>(JsonConvert.SerializeObject(res.Object));
                    if (dataInfo != null)
                    {
                        result = DateTime.ParseExact(dataInfo.JoiningDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"Error at GetJoiningDateOfEmployee: {ex.Message}");
            }
            return result;
        }

        public async Task<ResultDTO> SearchEmployee(string SAPCode)
        {
            var data = new List<object>();

            try
            {
                if (string.IsNullOrEmpty(SAPCode))
                {
                    Utilities.WriteLogError("SearchEmployee: SAPCode is null or empty.");
                    return new ResultDTO
                    {
                        Object = null,
                        ErrorCodes = new List<int> { 1004 },
                        Messages = new List<string> { "SAPCode is required" }
                    };
                }

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                    client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                    client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);

                    string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);

                    string url = BuildUrl("Search_empSet", _sapSetting.SAPGroupDataCollection["Employee"].Value);
                    url = string.Format("{0}?$filter=Pernr eq '{1}'&sap-client={2}&$format=json", url, SAPCode, _sapSetting.Header.SapClient);

                    HttpResponseMessage response;
                    try
                    {
                        response = await client.GetAsync(url);
                    }
                    catch (Exception exHttp)
                    {
                        Utilities.WriteLogError($"SearchEmployee: Exception when calling SAP API: {exHttp.Message}");
                        return new ResultDTO
                        {
                            Object = null,
                            ErrorCodes = new List<int> { 1001 },
                            Messages = new List<string> { "Lỗi gọi SAP API", exHttp.Message }
                        };
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        string httpResponseResult = await response.Content.ReadAsStringAsync();

                        try
                        {

                            var syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<EmployeeResponsSearchingViewModel>("EmployeeResponseSearching", httpResponseResult, "Mappings", "employee-for-searching-mapping.json");
                            foreach (var item in syncContact)
                            {
                                var properties = item.GetType().GetProperties();
                                dynamic _data = new ExpandoObject();
                                foreach (var property in properties)
                                {
                                    Utilities.AddProperty(_data, property.Name, property.GetValue(item));
                                }
                                data.Add(_data);
                            }

                            var currentData = data.FirstOrDefault();
                            return new ResultDTO { Object = currentData };
                        }
                        catch (Exception exParse)
                        {
                            Utilities.WriteLogError($"SearchEmployee: Error parsing SAP response:{exParse.Message}");
                            return new ResultDTO
                            {
                                Object = null,
                                ErrorCodes = new List<int> { 1002 },
                                Messages = new List<string> { "Lỗi phân tích dữ liệu từ SAP", exParse.Message }
                            };
                        }
                    }
                    else
                    {
                        Utilities.WriteLogError($"SearchEmployee: SAP API response failed - StatusCode: {response.StatusCode}");
                        return new ResultDTO
                        {
                            Object = null,
                            ErrorCodes = new List<int> { (int)response.StatusCode },
                            Messages = new List<string> { $"SAP API trả về lỗi: {response.StatusCode}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"SearchEmployee: Unexpected error {ex.Message}");
                return new ResultDTO
                {
                    Object = null,
                    ErrorCodes = new List<int> { 9999 },
                    Messages = new List<string> { "Lỗi không xác định", ex.Message }
                };
            }
        }


        public async Task<ResultDTO> GetLeaveBalanceSet(string filterUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                    client.BaseAddress = new Uri($"{_sapSetting.BaseUrl}");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_sapSetting.Header.ContentType));
                    client.DefaultRequestHeaders.Add("X-Requested-With", _sapSetting.Header.XRequestWith);

                    string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_sapSetting.Authentication.UserName}:{_sapSetting.Authentication.Password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);

                    var url = BuildUrl("GetLeaveBalanceSet", _sapSetting.SAPGroupDataCollection["Employee"].Value);
                    url = string.Format("{0}?{1}&sap-client={2}&$format=json", url, filterUrl, _sapSetting.Header.SapClient);

                    HttpResponseMessage response;
                    try
                    {
                        response = await client.GetAsync(url);
                    }
                    catch (Exception exHttp)
                    {
                        Utilities.WriteLogError($"GetLeaveBalanceSet: Exception during HTTP request - {exHttp.Message}");
                        return new ResultDTO
                        {
                            Object = null,
                            ErrorCodes = new List<int> { 1001 },
                            Messages = new List<string> { "Lỗi khi gọi SAP API", exHttp.Message }
                        };
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        string httpResponseResult = await response.Content.ReadAsStringAsync();

                        try
                        {

                            var syncContact = SAPJsonConverterHelper.ConvertFromObjectToMasterData<LeaveBalanceResponceSAPViewModel>("LeaveBalanceSet", httpResponseResult, "Mappings", "leaveBalanceInfo.json");

                            return new ResultDTO { Object = syncContact };
                        }
                        catch (Exception exParse)
                        {
                            Utilities.WriteLogError($"GetLeaveBalanceSet: Error parsing response - {exParse.Message}");
                            return new ResultDTO
                            {
                                Object = null,
                                ErrorCodes = new List<int> { 1002 },
                                Messages = new List<string> { "Lỗi xử lý dữ liệu từ SAP", exParse.Message }
                            };
                        }
                    }
                    else
                    {
                        Utilities.WriteLogError($"GetLeaveBalanceSet: SAP API returned error - StatusCode: {response.StatusCode}");
                        return new ResultDTO
                        {
                            Object = null,
                            ErrorCodes = new List<int> { (int)response.StatusCode },
                            Messages = new List<string> { $"Lỗi từ SAP API: {response.StatusCode}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"GetLeaveBalanceSet: Unexpected error - {ex.Message}");
                return new ResultDTO
                {
                    Object = null,
                    ErrorCodes = new List<int> { 9999 },
                    Messages = new List<string> { "Lỗi không xác định", ex.Message }
                };
            }
        }

    }
}
