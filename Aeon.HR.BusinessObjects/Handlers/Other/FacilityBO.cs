using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.CustomSection;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
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
	public class FacilityBO : IFacilityBO
    {
        private readonly FacilitySettingsSection _facilitySetting;
        private readonly IUnitOfWork _uow;
        public FacilityBO(IUnitOfWork uow)
        {
            _uow = uow;
            _facilitySetting = (FacilitySettingsSection)ConfigurationManager.GetSection("facilitySettings");
        }
        protected HttpClient ConfigAPI()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(_facilitySetting.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_facilitySetting.Header.AcceptType));
            client.DefaultRequestHeaders.Add("X-Requested-With", _facilitySetting.Header.XRequestWith);
            client.DefaultRequestHeaders.Add("secret", "DI9s9JTL4GSTigChfakC1f6vV3Mr2b+BVUuMu0Eyw1Q=");
            client.DefaultRequestHeaders.Add("uxr", UserBO_Helper.GetUrxUser());
            //client.DefaultRequestHeaders.Add("uxr", "cNX+DlF5qbrn4KrRAiG7r+ZaniQt2AvpqQuFw96TStHJjr+tkTJLMEfS22N3/eu9/wGq2p2/Jj48/H4ZtMWdN3wksvXTBbJzUDrvWMUl9o8T5F1Y6rHtdth7oSNBfg+C");
            return client;

        }
        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }
        public async Task<List<WorkflowTaskViewModel>> GetTasks(QueryArgs arg, bool isSuperAdmin)
        {
            var result = new List<WorkflowTaskViewModel>();
            var client = ConfigAPI();
            HttpResponseMessage response = null;
            try
            {
                var url = "Workflow/GetTasks?isSuperAdmin=" + isSuperAdmin;
                var payload = JsonConvert.SerializeObject(arg);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    //FacilityTaskViewModel res = JsonConvert.DeserializeObject<FacilityTaskViewModel>(httpResponseResult);

                    ResultDTO res = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (res.IsSuccess)
                    {
                        var jsonObject = JsonConvert.SerializeObject(res.Object);
                        var task = JsonConvert.DeserializeObject<FacilityObjectViewModel>(jsonObject);
                        if (task.Count > 0)
                        {
                            var itemsObject = JsonConvert.SerializeObject(task.Items);
                            var items = JsonConvert.DeserializeObject<List<FacilityTaskViewModel>>(itemsObject);
                            if (items != null && items.Count > 0)
                            {
                                items.ForEach(x => {
                                    result.Add(new WorkflowTaskViewModel
                                    {
                                        RegionId = x.RegionId,
                                        RegionName = x.RegionName,
                                        Created = x.Created,
                                        DueDate = x.DueDate,
                                        RequestedDepartmentName = x.RequestedDepartmentName,
                                        ReferenceNumber = x.ReferenceNumber,
                                        Status = x.Status,
                                        RequestorUserName = x.RequestorUserName,
                                        RequestorFullName = x.RequestorFullName,
                                        Title = x.ReferenceNumber,
                                        Link = x.Link,
                                        ItemType = x.RequestType == 0 ? "Stationery" : "Material",
                                        Module = "Facility"
                                    });
                                });
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
        private DateTime ConvertDateFromData(string date)
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
                else if (link.Contains("form-reimbursement"))
                {
                    return "Reimbursement";
                }
                else if (link.Contains("form-reimbursement-payment"))
                {
                    return "ReimbursementPayment";
                }
                else if (link.Contains("form-purchase-multiItem") || link.Contains("form-purchase"))
                {
                    return "Purchase";

                }
                else if (link.Contains("form-credit-note"))
                {
                    return "CreditNote";
                }
            }
            return "";
        }
	}
}
