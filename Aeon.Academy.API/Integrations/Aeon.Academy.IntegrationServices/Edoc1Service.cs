using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Entities;
using Aeon.HR.ViewModels.CustomSection;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Aeon.Academy.IntegrationServices
{
    public class Edoc1Service : IEdoc1Service
    {
        private readonly IEdoc1Api edoc1Api;
        private HttpClient httpClient;
        private readonly SAPSettingsSection edoc1Setting;
        public Edoc1Service()
        {
            var fakeUrl = ApplicationSettings.F2FakeUrl;
            edoc1Setting = ApplicationSettings.Edoc1Settings;
            httpClient = new HttpClient(new AuthenticatedHttpClientHandler());
            if (!string.IsNullOrEmpty(fakeUrl))
            {
                httpClient.BaseAddress = new Uri(fakeUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            else
            {
                httpClient.BaseAddress = new Uri($"{edoc1Setting.BaseUrl}");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(edoc1Setting.Header.ContentType));
                string authInfo = edoc1Setting.SAPGroupDataCollection["Authorization"].Value;
                httpClient.DefaultRequestHeaders.Add("Authorization", authInfo);
            }

            edoc1Api = RestService.For<IEdoc1Api>(httpClient, new RefitSettings()
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer()
            });
        }
        public HttpResponseMessage GetDepartmentInCharges()
        {
            return edoc1Api.GetDepartmentInCharges().Result;
        }
        public HttpResponseMessage GetBudgetInformations(int year, string dicCode)
        {
            return edoc1Api.GetBudgetInformations(new BudgetPlanRequest() { Year = year, DICCode = dicCode }).Result;
        }
        public HttpResponseMessage GetRequestedDepartments(string sapCode)
        {
            return edoc1Api.GetRequestedDepartments(new RequestedDepartmentRequest() { SAPCode = sapCode }).Result;
        }
        public HttpResponseMessage CreateF2MB(CreateF2MBRequest request)
        {
            return edoc1Api.CreateF2MB(request).Result;
        }
        public HttpResponseMessage GetStatusF2MB(string referenceNumber)
        {
            return edoc1Api.GetStatusF2MB(new F2MBStatusRequest() { ReferenceNumber = referenceNumber }).Result;
        }
        public HttpResponseMessage GetSuppliers()
        {
            return edoc1Api.GetSuppliers().Result;
        }
        public HttpResponseMessage GetYears()
        {
            return edoc1Api.GetYears().Result;
        }
        public HttpResponseMessage GetCurrency(string symbol)
        {
            return edoc1Api.GetCurrency(new CurrencyModel() { Symbol = symbol }).Result;
        }
        public HttpResponseMessage GetBudgetItem(int year, string departmentInChargeCode, string CFRCode, string budgetCodeCode, string budgetPlan)
        {
            return edoc1Api.GetBudgetItem(new BudgetBallanceModel()
            {
                Year = year,
                DepartmentInChargeCode= departmentInChargeCode,
                CFRCode = CFRCode,
                BudgetCodeCode = budgetCodeCode,
                BudgetPlan = budgetPlan
            }).Result;
        }
    }
}
