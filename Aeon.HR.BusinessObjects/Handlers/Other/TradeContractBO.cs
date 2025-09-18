using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.CustomSection;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoMapper;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Jobs;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Data.Models;

namespace Aeon.HR.BusinessObjects.Handlers.Other
{
    public class TradeContractBO : ITradeContractBO
    {
        private readonly TradeContractSettingsSection _tradeContractSetting;
        private readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        private readonly IEmailNotification _emailNotification;
        private static readonly string TradeContractURL = (string) ConfigurationManager.GetSection("tradeUrl");
        private readonly string API_DEFAULT_SAF = "/default.aspx#/detail/";
        private readonly string API_DEFAULT_DS =  "/default.aspx#/trade-contract/document-set-detail/";
        public TradeContractBO(IUnitOfWork uow, ILogger logger, IEmailNotification emailNotification)
        {
            _uow = uow;
             _tradeContractSetting = (TradeContractSettingsSection) ConfigurationManager.GetSection("aeonTradeContractSettings");
            _logger = logger;
            _emailNotification = emailNotification;
        }
        protected HttpClient ConfigAPI()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(_tradeContractSetting.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_tradeContractSetting.Authentication.UserName}:{_tradeContractSetting.Authentication.Password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            return client;
        }

        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }

        public async Task<List<WorkflowTaskViewModel>> GetTasks(TradeContractArgs arg)
        {
            var result = new List<WorkflowTaskViewModel>();
            var client = ConfigAPI();
            HttpResponseMessage response = null;
            try
            {
                var url = "/_layouts/api/Workflow/GetNeedApprovalItems";
                var payload = JsonConvert.SerializeObject(arg);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    // replace $id do kh parse duoc json
                    httpResponseResult = httpResponseResult.Replace("$id", "DocumentSetId"); // replace for parse
                    var res = Mapper.Map<TradeContractViewModel>(JsonConvert.DeserializeObject<TradeContractViewModel>(httpResponseResult));
                    if (res != null)
                    {
                        res.Results.ForEach(x =>
                        {
                            WorkflowTaskViewModel model = new WorkflowTaskViewModel
                            {
                                RequestedDepartmentName = x.SupplierName,
                                ReferenceNumber = x.ReferenceNo,
                                Status = this.getStatusTradeContract(x.DocumentSet.ApprovalStatus),
                                RequestorUserName = x.SubmittedByLoginName,
                                RequestedDepartmentCode = x.ReferenceNo,
                                RequestorFullName = x.SubmittedByLoginName,
                                // Link = (x.ReferenceNo.StartsWith("SA") || x.ReferenceNo.StartsWith("SR")) ? (API_DEFAULT_SAF + x.ReferenceNo) : (API_DEFAULT_DS + x.DocumentSet.Id),
                                ItemType = (x.DocumentSet != null && !string.IsNullOrEmpty(x.DocumentSet.DocumentSetType)) ? this.getDocumentSetName(x.DocumentSet.DocumentSetType) : this.getDocumentSetPurposeName(x.DocumentSet.DocumentSetPurpose),
                                RegionName = "",
                                Module = "TradeContract",
                            };

                            if (x.ReferenceNo.StartsWith("SA") || x.ReferenceNo.StartsWith("SR"))
                            {
                                model.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_SAF + x.ReferenceNo);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(x.DocumentSet.DocumentSetPurpose))
                                {
                                    if (x.DocumentSet.DocumentSetPurpose.Equals("1") || x.DocumentSet.DocumentSetPurpose.Equals("2"))
                                    {
                                        model.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_SAF + x.ReferenceNo);
                                    }
                                    else if (x.DocumentSet.DocumentSetPurpose.Equals("0"))
                                    {
                                        model.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_DS + x.DocumentSet.Id);
                                    }
                                }
                            }

                            model.DueDate = null;
                            if (x.Duedate != null && !x.Duedate.Equals(""))
                            {
                                int dueDateLength = x.Duedate.Length;
                                x.Duedate = x.Duedate.Substring(6, dueDateLength - 13);
                                long dueDateDb = long.Parse(x.Duedate);
                                model.DueDate = DateTimeOffset.FromUnixTimeMilliseconds(dueDateDb).DateTime;
                            }
                            model.Created = null;
                            if (x.DocumentSet.Created != null)
                            {
                                model.Created = DateTimeOffset.Parse(x.DocumentSet.Created);
                            }
                            result.Add(model);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Get Todo List TradeContract: " + ex + " | Time: " + DateTimeOffset.Now);
            }
            return result;
        }

        private DateTime ConvertDateFromEdoc1Data(string date)
        {
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

        public string getDocumentSetName(string documentSetType)
        {
            if (documentSetType == null || documentSetType.Equals("")) return "";
            string rtDocumentSet = "Document Set";
            switch (documentSetType)
            {
                case "-2":
                    rtDocumentSet = "Supplier Application Form";
                    break;
                case "-1":
                    rtDocumentSet = "Supplier Audit";
                    break;
                case "0":
                case "1":
                case "2":
                case "3":
                    if (documentSetType == "0")
                    {
                        rtDocumentSet += " Agreement";
                    }
                    else if (documentSetType == "1")
                    {
                        rtDocumentSet += " Appendix";
                    }
                    else if (documentSetType == "2")
                    {
                        rtDocumentSet += " Liquidation";
                    }
                    else if (documentSetType == "3")
                    {
                        rtDocumentSet += " Announcement";
                    }
                    break;
            }
            return rtDocumentSet;
        }

        public string getDocumentSetPurposeName(string documentSetPurposeType)
        {
            string rtDocumentSetPurpose = "";
            if (!string.IsNullOrEmpty(documentSetPurposeType))
            {
                switch (documentSetPurposeType)
                {
                    case "1":
                        rtDocumentSetPurpose = "Freeze Contract Code";
                        break;
                    case "2":
                        rtDocumentSetPurpose = "Extend trading term";
                        break;
                }
            }
            return rtDocumentSetPurpose;
        }

        public string getStatusTradeContract(string status)
        {
            string rtStatus = "";
            if (!string.IsNullOrEmpty(status))
            {
                List<string> statusList = new List<string>()
                {
                "draft", "completed", "canceled", "rejected", "remove", "expired", "requested-to-change", "waiting-for-hard-copy-confirmation",
                "waiting-for-ap-checker-confirm-document", "waiting-for-sm-checker-accept", "waiting-for-auditor-confirm", "waiting-for-audit-and-report", "waiting-for-audit-update-report", "waiting-for-ap-checker-approve"
                };
                if (statusList.Contains(status))
                {
                    if (status.Contains("-"))
                    {
                        status = status.Replace("-", " ");
                    }
                    rtStatus = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(status.ToLower());
                }
                else
                {
                    switch (status)
                    {
                        case "skipped":
                            rtStatus = "Skip";
                            break;
                        case "waiting-for-cmd-approve":
                            rtStatus = "Waiting For CMD Approval";
                            break;
                        case "waiting-for-head-of-division-approve":
                            rtStatus = "Waiting For Head Of Division Approval";
                            break;
                        case "waiting-for-head-of-line-approve":
                            rtStatus = "Waiting For Head Of Line Approval";
                            break;
                        case "waiting-for-ed-approve":
                            rtStatus = "Waiting For Executive Director Approval";
                            break;
                        case "waiting-for-received-hardcopy":
                            rtStatus = "Waiting For Received Hard Copy";
                            break;
                        case "waiting-for-cmd-legal-ap-approve":
                            rtStatus = "Waiting For CMD Legal AP Approval";
                            break;
                        case "waiting-for-cmd-ap-approve":
                            rtStatus = "Waiting For CMD AP Approval";
                            break;
                        case "waiting-for-legal-ap-hod-approve":
                            rtStatus = "Waiting For Legal AP Head Of Division Approval";
                            break;
                        case "waiting-for-sm-checker-approve":
                            rtStatus = "Waiting For SM Checker Approve";
                            break;
                        case "waiting-for-sm-checker-accept":
                            rtStatus = "Waiting For SM Manager Approve";
                            break;
                        case "waiting-for-upload-hardcopy":
                            rtStatus = "Waiting For Hard Copy Uploaded";
                            break;
                        case "waiting-for-legal-checker-approve":
                            rtStatus = "Waiting For Legal Checker Approval";
                            break;
                        case "waiting-for-legal-manager-approve":
                            rtStatus = "Waiting For Legal Manager Approval";
                            break;
                        case "waiting-for-ap-manager-approve":
                            rtStatus = "Waiting For AP Manager Approval";
                            break;
                        case "waiting-for-confirm-hardcopy":
                            rtStatus = "Waiting For Hard Copy Uploaded";
                            break;
                        case "waiting-for-ap-hod-approve":
                            rtStatus = "Waiting For AP Head Of Division Approval";
                            break;
                        case "remove":
                            rtStatus = "Removed";
                            break;
                        default:
                            if (status.Contains("-"))
                            {
                                status = status.Replace("-", " ");
                            }
                            rtStatus = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(status.ToLower());
                            break;
                    }
                }
            }
            return rtStatus;
        }
    }
}