using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SyncOrgchartJob.Service
{
    public class IntergrationService
    {
        private static HttpClient ConfigAPI()
        {
            var client = new HttpClient();
            // 
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["HRApiUrl"]); // KHÔNG có dấu '/' ở cuối
            
            //var token = ConfigurationManager.AppSettings["HRApiToken"];
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token...");
            return client;
        }

        public async Task<bool> ClearCacheDepartment()
        {
            var client = ConfigAPI();
            var response = await client.GetAsync("api/ServiceAPI/ClearCacheDepartment"); // KHÔNG có dấu '/' đầu
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ClearCacheDepartmentIT()
        {
            var client = ConfigAPI();
            var response = await client.GetAsync("it/api/Partner/ClearCacheDepartment"); // KHÔNG có dấu '/' đầu
            return response.IsSuccessStatusCode;
        }
        
    }
}