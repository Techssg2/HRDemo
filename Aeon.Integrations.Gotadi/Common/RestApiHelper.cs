using Aeon.HR.ViewModels.CustomSection;
using AEON.Integrations.Gotadi.API;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.Common
{
    public static class RestApiHelper
    {

        public static T GetRestServiceApi<T>()
        {
            GotadiSettingsSection gotadiSettings = ConfigHelper.GetGotadiConfig();
            var client = new HttpClient(new HttpClientHandler())
            {
                BaseAddress = new Uri(gotadiSettings.BaseUrl),
                Timeout= TimeSpan.FromSeconds(int.Parse(gotadiSettings.Timeouts)) 
            };
            client.DefaultRequestHeaders.Add("apiKey", gotadiSettings.Header.APIKey);

            var service = RestService.For<T>(client, new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer()
            });

            return service;

        }
    }
}
