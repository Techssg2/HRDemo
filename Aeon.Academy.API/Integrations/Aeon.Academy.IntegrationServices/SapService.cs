using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Entities;
using Aeon.HR.ViewModels.CustomSection;
using Refit;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Activation;
using System.Text;

namespace Aeon.Academy.IntegrationServices
{
    public class SapService
    {
        private readonly ISapApi sapApi;
        private HttpClient httpClient;
        private readonly SAPSettingsSection sapSetting;
        public SapService()
        {
            var fakeUrl = ApplicationSettings.FakeUrl;
            sapSetting = ApplicationSettings.SapSettings;
            httpClient = new HttpClient(new AuthenticatedHttpClientHandler());
            if (!string.IsNullOrEmpty(fakeUrl))
            {
                httpClient.BaseAddress = new Uri(fakeUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            else
            {
                httpClient.BaseAddress = new Uri($"{sapSetting.BaseUrl}");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(sapSetting.Header.ContentType));
                httpClient.DefaultRequestHeaders.Add("X-Requested-With", sapSetting.Header.XRequestWith);
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sapSetting.Authentication.UserName}:{sapSetting.Authentication.Password}")); //("Username:Password")  
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            }

            sapApi = RestService.For<ISapApi>(httpClient, new RefitSettings()
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer()
            });
        }
        public HttpResponseMessage SyncData(AcademyTrainingRequest model)
        {
            var api = ApplicationSettings.SapApi;
            var url = httpClient.BaseAddress.ToString() + api;
            httpClient.BaseAddress = new Uri(url);
            var sap = RestService.For<ISapApi>(httpClient, new RefitSettings());
            return sap.SyncData(model).Result;
        }
    }
}
