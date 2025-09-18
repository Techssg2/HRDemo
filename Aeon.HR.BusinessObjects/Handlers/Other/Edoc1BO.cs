using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.CustomSection;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.Other
{
    public class Edoc1BO : IEdoc01BO
    {
        private readonly SAPSettingsSection _edoc1Setting;
        private readonly SAPSettingsSection _edoc1SettingV2;
        private readonly IUnitOfWork _uow;
        private string   RootUrlEdoc1 = ConfigurationManager.AppSettings["RootUrlEdoc1"];
        private string   SubDomainLiquor = ConfigurationManager.AppSettings["SubDomainLiquor"];
        private readonly TradeContractSettingsSection _tradeContractSetting;
        private readonly string API_DEFAULT_SAF = "/default.aspx#/detail/";
        private readonly string API_DEFAULT_DS = "/default.aspx#/trade-contract/document-set-detail/";
        public Edoc1BO(IUnitOfWork uow)
        {
            _uow = uow;
            _tradeContractSetting = (TradeContractSettingsSection)ConfigurationManager.GetSection("aeonTradeContractSettings");
            _edoc1Setting = (SAPSettingsSection)ConfigurationManager.GetSection("aeonPhase1Settings");
            _edoc1SettingV2 = (SAPSettingsSection)ConfigurationManager.GetSection("aeonPhase1SettingsV2");
        }
        protected HttpClient ConfigAPI()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(_edoc1Setting.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_edoc1Setting.Header.AcceptType));
            client.DefaultRequestHeaders.Add("X-Requested-With", _edoc1Setting.Header.XRequestWith);
            client.DefaultRequestHeaders.Add("Token", _edoc1Setting.Header.Token);
            string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_edoc1Setting.Authentication.UserName}:{_edoc1Setting.Authentication.Password}")); //("Username:Password")  
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            return client;

        }

        protected HttpClient ConfigAPIV2()
        {
            /*var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(_edoc1SettingV2.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_edoc1SettingV2.Header.AcceptType));
            client.DefaultRequestHeaders.Add("X-Requested-With", _edoc1SettingV2.Header.XRequestWith);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _edoc1SettingV2.Header.Token);*/
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(4000);
            client.BaseAddress = new Uri(_edoc1SettingV2.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _edoc1SettingV2.Header.Token);
            return client;
        }

        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }

        public async Task<List<WorkflowTaskViewModel>> GetTasksV2(Edoc1ArgV2 arg)
        {
            var result = new List<WorkflowTaskViewModel>();
            var client = ConfigAPIV2();
            HttpResponseMessage response = null;
            try
            {
                var url = "/it/api/Partner/GetTasksByLoginName";
                var payload = JsonConvert.SerializeObject(arg);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    /*Edoc1TaskViewModelV2*/
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    ResultDTO res = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (res != null)
                    {
                        var detailTask = Mapper.Map<List<Edoc1TaskViewModelV2>>(JsonConvert.DeserializeObject<List<Edoc1TaskViewModelV2>>(res.Object.ToString()));
                        if (detailTask != null && detailTask.Any())
                        {
                            foreach (var task in detailTask)
                            {
                                var crTask = new WorkflowTaskViewModel
                                {
                                    Created = task.Created,
                                    DueDate = task.DueDate,
                                    RequestedDepartmentName = task.RequestedDepartmentName,
                                    ReferenceNumber = task.ReferenceNumber,
                                    Status = task.Status,
                                    RequestorUserName = task.RequestorUserName,
                                    RequestorFullName = task.RequestorFullName,
                                    ItemId = task.ItemId,
                                    Module = task.Module,
                                    ItemType = task.ItemType,
                                    Link = task.Link,
                                    Title = task.Title
                                };
                                this.GetTitleLinkEdoc1(task, crTask);
                                if (!string.IsNullOrEmpty(crTask.Link) && !crTask.Module.Equals("TradeContract"))
                                {
                                    var findType = GetItemTypeByLink(crTask.Link);
                                    if (!string.IsNullOrEmpty(findType))
                                    {
                                        crTask.ItemType = findType;
                                    }
                                }

                                if (task.IsConfidentialContract != null && task.IsConfidentialContract == true)
                                {
                                    crTask.ReferenceNumber = crTask.ReferenceNumber + "-CONF";
                                }

                                result.Add(crTask);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        private void GetTitleLinkEdoc1(Edoc1TaskViewModelV2 taskEdocIT, WorkflowTaskViewModel taskEdocEdoc1)
        {
            if (taskEdocIT != null)
            {
                if (!string.IsNullOrEmpty(taskEdocIT.ReferenceNumber))
                {
                    if (taskEdocIT.ReferenceNumber.StartsWith("F2-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("F3-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("F4-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("CN-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("RP-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("CA-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("RE-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("PR-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("TF-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("RF-"))
                    {
                        taskEdocEdoc1.Link = RootUrlEdoc1;
                        string refNumber = taskEdocIT.ReferenceNumber ?? string.Empty;
                        string lowerItemId = taskEdocIT.ItemId.ToString().ToLower().Replace("-", "");

                        // Mapping: prefix => (Title, ItemType, FormPath)
                        var formMapping = new Dictionary<string, (string Title, string ItemType, string FormPath)>
                        {
                            { "F2-", ("Purchasing", "Purchase", null) }, // Special case
                            { "F3-", ("Contract", "Contract", null) },   // Special case
                            { "F4-", ("Non-Expense Contract", "Non-Expense Contract", "/form-non-expense-contract/") },
                            { "CN-", ("Credit Note", "Credit Note", "/form-credit-note/") },
                            { "RP-", ("Reimbursement Payment", "Reimbursement Payment", "/form-reimbursement-payment/") },
                            { "PR-", ("Payment", "Payment", "/form-payment/") },
                            { "CA-", ("Advance", "Advance", "/form-advance/") },
                            { "RE-", ("Reimbursement", "Reimbursement", "/form-reimbursement/") },
                            { "TF-", ("Transfer Cash", "Transfer Cash", "/form-transfer-cash/") },
                            { "RF-", ("Refund Card", "Refund Card", "/form-refund-card/") }
                        };

                        // Tìm prefix phù hợp
                        foreach (var kv in formMapping)
                        {
                            if (refNumber.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                taskEdocEdoc1.Title = kv.Value.Title;
                                taskEdocEdoc1.ItemType = kv.Value.ItemType;

                                if (!string.IsNullOrEmpty(taskEdocIT.Link))
                                {
                                    if (!string.IsNullOrEmpty(taskEdocIT.ItemType))
                                    {
                                        taskEdocEdoc1.ItemType = taskEdocIT.ItemType;
                                    }
                                    taskEdocEdoc1.Link = $"{RootUrlEdoc1}{taskEdocIT.Link}";
                                    return;
                                }

                                // Xử lý đặc biệt F2- và F3-
                                if (kv.Key == "F2-")
                                {
                                    taskEdocEdoc1.Link += taskEdocIT.IsMultibudget
                                        ? "/form-purchase-multiItem/"
                                        : "/form-purchase/";
                                }
                                else if (kv.Key == "F3-")
                                {
                                    taskEdocEdoc1.Link += taskEdocIT.IsManual
                                        ? "/form-contract-custom/"
                                        : "/form-contract-multiF2/";
                                }
                                else
                                {
                                    taskEdocEdoc1.Link += kv.Value.FormPath;
                                }

                                taskEdocEdoc1.Link += lowerItemId;
                                return;
                            }
                        }
                    }
                    else if (taskEdocIT.ReferenceNumber.StartsWith("DOC-") ||
                            taskEdocIT.ReferenceNumber.StartsWith("PJ-") ||
                            taskEdocIT.ReferenceNumber.StartsWith("T-") ||
                            taskEdocIT.ReferenceNumber.StartsWith("RL-"))
                    {
                        taskEdocEdoc1.Link = SubDomainLiquor;
                        taskEdocEdoc1.Module = "LiquorLicense";

                        string refNumber = taskEdocIT.ReferenceNumber ?? string.Empty;
                        string itemId = taskEdocIT.ItemId.ToString();

                        // Mapping prefix → (Title, Path)
                        var liquorMap = new Dictionary<string, (string Title, string Path)>
                        {
                            { "DOC-", ("Document", "/Document/Document?id=") },
                            { "PJ-",  ("Project", "/Project/Project?id=") },
                            { "T-",   ("Tasks", "/Tasks/TaskDetail?id=") },
                            { "RL-",  ("RetailLicense", "/RetailLicense/RetailLicense?id=") }
                        };

                        // Duyệt qua mapping để gán giá trị phù hợp
                        foreach (var kv in liquorMap)
                        {
                            if (refNumber.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                taskEdocEdoc1.Title = kv.Value.Title;
                                taskEdocEdoc1.Link += kv.Value.Path + itemId;
                                break;
                            }
                        }
                    }
                    else if (taskEdocIT.ReferenceNumber.StartsWith("SA-") || taskEdocIT.ReferenceNumber.StartsWith("SR-"))
                    {
                        taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_SAF + taskEdocEdoc1.ReferenceNumber);
                        taskEdocEdoc1.Module = "TradeContract";
                        if (taskEdocIT.ReferenceNumber.StartsWith("SA-"))
                        {
                            taskEdocEdoc1.Title = "Supplier Application Form";
                            taskEdocEdoc1.ItemType = "Supplier Application Form";
                            taskEdocEdoc1.Module = "TradeContract";
                        }
                        else
                        {
                            taskEdocEdoc1.Title = "Supplier Audit Form";
                            taskEdocEdoc1.ItemType = "Supplier Audit Form";
                            taskEdocEdoc1.Module = "TradeContract";
                        }
                    }
                    else
                    {
                        if (taskEdocEdoc1.Module.Equals("TradeContract"))
                        {
                            taskEdocEdoc1.Title = "Document Set";
                            taskEdocEdoc1.ItemType = "Document Set";
                            taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_DS + taskEdocIT.ItemId);
                            taskEdocEdoc1.Module = "TradeContract";
                            if (taskEdocIT.DocumentSetPurpose != null)
                            {
                                if (taskEdocIT.DocumentSetPurpose == 1)
                                {
                                    taskEdocEdoc1.ItemType = "Freeze Contract Code";
                                    taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_SAF + taskEdocIT.ReferenceNumber);
                                }
                                else if (taskEdocIT.DocumentSetPurpose == 2)
                                {
                                    taskEdocEdoc1.ItemType = "Extend Trading Term";
                                    taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_SAF + taskEdocIT.ReferenceNumber);
                                }
                                else if (taskEdocIT.DocumentSetPurpose == 0)
                                {
                                    taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_DS + taskEdocIT.ItemId);
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public async Task<List<WorkflowTaskViewModel>> GetTasks(Edoc1Arg arg)
        {
            var result = new List<WorkflowTaskViewModel>();
            var client = ConfigAPI();
            HttpResponseMessage response = null;
            try
            {
                var url = "GetTasks";
                var payload = JsonConvert.SerializeObject(arg);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    Edoc1Result<Edoc1TaskViewModel> res = JsonConvert.DeserializeObject<Edoc1Result<Edoc1TaskViewModel>>(httpResponseResult);

                    if (res != null && res.Items.Any())
                    {
                        res.Items.ForEach(x =>
                        {
                            result.Add(new WorkflowTaskViewModel
                            {
                                Created = ConvertDateFromEdoc1Data(x.CreatedDate),
                                DueDate = ConvertDateFromEdoc1Data(x.DueDate),
                                RequestedDepartmentName = x.RequestedDepartmentName,
                                ReferenceNumber = x.ReferenceNumber,
                                Status = x.Status,
                                RequestorUserName = x.Requestor,
                                RequestorFullName = x.Requestor,
                                Title = x.ReferenceNumber,
                                Link = x.Link,
                                ItemType = GetItemTypeByLink(x.Link),
                                Module = "Edoc1"
                            });
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }
        private DateTime ConvertDateFromEdoc1Data(string date)
        {
            //"/Date(1530440534143-0000)/";
            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    var splitStringDate = date.Split('(');
                    var keys = splitStringDate[1].Split('-');
                    DateTime datetime = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(keys[0]));  
                    return datetime.ToLocalTime();
                }
                catch (Exception ex)
                {
                    return DateTime.MinValue;
                }
            }
            return DateTime.MinValue;

        }
        private string GetItemTypeByLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                if (link.Contains("form-contract") || link.Contains("form-contract-custom") || link.Contains("form-contract-multiF2"))
                {
                    return "Contract";
                }
                else if (link.Contains("form-non-expense-contract"))
                {
                    return "NonExpenseContract";
                }
                else if (link.Contains("form-payment"))
                {
                    return "Payment";
                }
                else if (link.Contains("form-advance"))
                {
                    return "Advance";
                }
                else if (link.Contains("form-reimbursement-payment"))
                {
                    return "ReimbursementPayment";
                }
                else if (link.Contains("form-reimbursement") || link.Contains("form-reimursement"))
                {
                    return "Reimbursement";
                }
                else if (link.Contains("form-purchase-multiItem") || link.Contains("form-purchase"))
                {
                    return "Purchase";

                }
                else if (link.Contains("form-transfer-cash"))
                {
                    return "TransferCash";
                }
                else if (link.Contains("form-refund-card"))
                {
                    return "RefundCard";
                }
                else if (link.Contains("form-credit-note"))
                {
                    return "CreditNote";
                }
                else if (link.Contains("/Tasks/TaskDetail"))
                {
                    return "Liquor/Task";
                }
                else if (link.Contains("/Project/Project"))
                {
                    return "Liquor/Project";
                }
                else if (link.Contains("/Document/Document"))
                {
                    return "Liquor/Document";
                }
                else if (link.Contains("/RetailLicense/RetailLicense"))
                {
                    return "Liquor/RetailLicense";
                }
            }
            return "";
        }
        public async Task<ResultDTO> CreateF2Form()
        {
            var result = new ResultDTO();
            HttpResponseMessage response = null;
            try
            {
                var client = ConfigAPI();
                var arg = new { LoginName = _uow.UserContext.CurrentUserName };
                //var arg = new { LoginName = "edoc04"};
                var url = "CreateF2Form";
                var payload = JsonConvert.SerializeObject(arg);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    SimpleEdoc1Result res = JsonConvert.DeserializeObject<SimpleEdoc1Result>(httpResponseResult);
                    result.Object = res;
                }

            }
            catch (Exception ex)
            {
                result.Messages.Add(ex.Message);
                result.ErrorCodes.Add(1004);
            }
            return result;
        }
    }
}
