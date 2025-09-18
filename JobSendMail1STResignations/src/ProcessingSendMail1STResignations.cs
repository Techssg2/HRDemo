using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignations.src
{
    public class ProcessingSendMail1STResignations
    {
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
            Utilities.WriteLogError("----------------" + DateTime.Now.ToString("g") + "-------------------");
            Utilities.WriteLogError("---------------- Start ProcessingSendMail1STResignations.ProcessingAPI -------------------");
            var url = @ConfigurationManager.AppSettings["Url"];
            try
            {
                HttpResponseMessage response = null;
                var client = ConfigAPI(url);
                response = await client.GetAsync(url);
                string responseContent = await response.Content.ReadAsStringAsync();
                Utilities.WriteLogError("ProcessingAPI.responseContent: " + response);
                Utilities.WriteLogError("---------------- End -------------------");
            }
            catch (Exception e)
            {
                Utilities.WriteLogError("Exception.Message: " + e.Message);
                Utilities.WriteLogError("Exception.StackTrace: " + e.StackTrace);
            }
        }
    }
}
