using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
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

namespace Aeon.AutoImportRequestToHire.src
{
    public class ProcessingAutoImportRequestToHire
    {
        public ProcessingAutoImportRequestToHire() { }

        protected static HttpClient ConfigAPI(string uri)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(4000);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJTU0ciLCJuYW1lIjoiQUVPTiJ9.RumX95b8ljvb7t00CO6CX7kVy2rNK3ccs455Zoaj8SA");
            return client;
        }

        public static async Task ProcessingAPI()
        {
            Utilities.WriteLogError("----------------"+ DateTime.Now.ToString("g") + "-------------------");
            var url = @ConfigurationManager.AppSettings["Url"];
            var rootDirectory = @ConfigurationManager.AppSettings["RootDirectory"];
            var rootDirectoryReceive = @ConfigurationManager.AppSettings["RootDirectoryReceive"];
            var rootDirectoryLog = @ConfigurationManager.AppSettings["RootDirectoryLog"];
            try
            {
                HttpResponseMessage response = null;
                var client = ConfigAPI(url);
                var payload = JsonConvert.SerializeObject(new DataImportArgs()
                {
                    Url = url,
                    RootDirectory = rootDirectory,
                    RootDirectoryReceive = rootDirectoryReceive,
                    RootDirectoryLog = rootDirectoryLog
                });
                Utilities.WriteLogError("Payload" + payload);
                payload = payload.Replace("null", "\"\"");
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                Utilities.WriteLogError("ResponseStatusCode: " + response.StatusCode);
                Utilities.WriteLogError("ResponseContent: " + responseContent);
            }
            catch (Exception e)
            {
                Utilities.WriteLogError("Exception.Message: " + e.Message);
                Utilities.WriteLogError("Exception.StackTrace: " + e.StackTrace);
            }
        }
    }

    public class DataImportArgs
    {
        public string Url { get; set; }
        public string RootDirectory { get; set; }
        public string RootDirectoryReceive { get; set; }
        public string RootDirectoryLog { get; set; }
    }
}