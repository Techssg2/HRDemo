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
using Aeon.HR.BusinessObjects.Handlers.Test;
using Aeon.HR.Infrastructure.Constants;

namespace Aeon.HR.BusinessObjects.Handlers.Other
{
    public class SKUBO : ISKUBO
    {
        private readonly SkuSettingsSection _skuSettingsSection;
        private readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        public SKUBO(IUnitOfWork uow, ILogger logger, IEmailNotification emailNotification)
        {
            _uow = uow;
            _skuSettingsSection = (SkuSettingsSection) ConfigurationManager.GetSection("skuSettings");
            _logger = logger;
        }
        protected HttpClient ConfigAPI()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(_skuSettingsSection.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            /*string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_skuSettingsSection.Authentication.UserName}:{_skuSettingsSection.Authentication.Password}")); 
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);*/
            return client;
        }

        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }

        public async Task<List<WorkflowTaskViewModel>> GetTasks(SKUArgs arg)
        {
            var result = new List<WorkflowTaskViewModel>();
            var client = ConfigAPI();
            HttpResponseMessage response = null;
            try
            {
                var url = UrlConstants.Integrations.SKU;
                var payload = JsonConvert.SerializeObject(arg);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var res = Mapper.Map<SkuViewModel>(JsonConvert.DeserializeObject<SkuViewModel>(httpResponseResult));
                    if (res != null)
                    {
                        if (res.Data != null && res.Data.Result.Any())
                        {
                            foreach (var x in res.Data.Result)
                            {
                                var deptCode = await GetDepartmentCode(x.DepartmentId);
                                WorkflowTaskViewModel model = new WorkflowTaskViewModel
                                {
                                    RequestedDepartmentName = x.DepartmentName,
                                    ReferenceNumber = x.ReferenceNumber,
                                    Status = x.Status,
                                    RequestorUserName = string.IsNullOrEmpty(x.Created) ? "" : x.CreatedBy.Substring(6),
                                    RequestedDepartmentCode = deptCode,
                                    RequestorFullName = x.CreatedByFullName,
                                    Link = x.Link,
                                    ItemType = "SKU",
                                    RegionName = "",
                                    DueDate = null,
                                    Module = ModuleIntegrationsConstants.SKU
                                };

                                if (x.Created != null && !x.Created.Equals(""))
                                {
                                    x.Created = x.Created.Substring(6, (x.Created.Length) - 8);
                                    long createdDate = long.Parse(x.Created);
                                    model.Created = DateTimeOffset.FromUnixTimeMilliseconds(createdDate).DateTime;
                                }
                                result.Add(model);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Get Todo List TradeContract: " + ex + " | Time: " + DateTimeOffset.Now);
                return result;
            }
            return result;
        }

        public async Task<string> GetDepartmentCode(string departmentId)
        {
            string departmentCode = "";
            if (!string.IsNullOrEmpty(departmentId))
            {
                Guid deptId = new Guid(departmentId);
                var dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == deptId);
                if (dept != null) departmentCode = dept.Code;
            }
            return departmentCode;
        }
    }
}