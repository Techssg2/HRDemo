using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;

namespace Aeon.UpdateUserInfoFromSapJob.src
{
    public class ProcessingUpdateUserInfoFromSapJob
    {
        public ProcessingUpdateUserInfoFromSapJob() { }
        protected static HttpClient ConfigAPI(string uri)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(4000);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ConfigurationManager.AppSettings["Token"]);
            return client;
        }
        public static async Task ProcessingAPI()
        {
            Utilities.WriteLogError("----------------"+ DateTime.Now.ToString("g") + "-------------------");
            Utilities.WriteLogError("---------------- UpdateUserInfoFromSapJob.ProcessingAPI -------------------");
            var url = @ConfigurationManager.AppSettings["Url"];
            try
            {
                url = string.Format("{0}?year={1}", url, DateTime.Now.Year);
                HttpResponseMessage response = null;
                var client = ConfigAPI(url);
                response = await client.GetAsync(url);
                string responseContent = await response.Content.ReadAsStringAsync();
                Utilities.WriteLogError("ProcessingAPI.responseContent: ");
            }
            catch (Exception e)
            {
                Utilities.WriteLogError("Exception.Message: " + e.Message);
                Utilities.WriteLogError("Exception.StackTrace: " + e.StackTrace);
            }
        }
    }
}