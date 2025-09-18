using Aeon.CreatePayloadCompleted.Utilities;
using CreatePayloadCompleted.src.ModelEntity;
using CreatePayloadCompleted.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace CreatePayloadCompleted.src
{
    public class CreatePayloadCompletedUpdate : SQLQuery<CreatePayloadCompletedEntity>
    {
        public CreatePayloadCompletedUpdate() {}

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

        public List<string> ExcuteUpdate()
        {
            Utilities.WriteLogError("Function ExcuteUpdate() run at: " + DateTimeOffset.Now);
            List<CreatePayloadCompletedEntity> refererNumbers = this.GetListCompletedTicketWithOutPayload();
            if (refererNumbers.Any())
            {
                CreatePayloadArgs args = new CreatePayloadArgs();
                args.ReferenceNumbers = refererNumbers.Select(x => x.ReferenceNumber).ToList();

                var payload = JsonConvert.SerializeObject(args);


                refererNumbers.ForEach(x => Utilities.WriteLogError(x.ReferenceNumber));
            }
            return refererNumbers.Any() ? refererNumbers.Select(x => x.ReferenceNumber).ToList() : new List<string>();
        }

        public async Task ProcessingAPI()
        {
            Utilities.WriteLogError("----------------" + DateTime.Now.ToString("g") + "-------------------");
            List<CreatePayloadCompletedEntity> refererNumbers = this.GetListCompletedTicketWithOutPayload();
            if (refererNumbers.Any()) {
                var url = @ConfigurationManager.AppSettings["Url"];
                try
                {
                    HttpResponseMessage response = null;
                    var client = ConfigAPI(url);
                    CreatePayloadArgs args = new CreatePayloadArgs();
                    args.ReferenceNumbers = refererNumbers.Select(x => x.ReferenceNumber).ToList();

                    var payload = JsonConvert.SerializeObject(args);
                    Utilities.WriteLogError("Payload" + payload);
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

        public List<CreatePayloadCompletedEntity> GetListCompletedTicketWithOutPayload()
        {
            List<CreatePayloadCompletedEntity> r_list = new List<CreatePayloadCompletedEntity>();
            try
            {
                string selectData = string.Format(@"
                    select a.id, a.ReferenceNumber, a.Status from ShiftExchangeApplications a
                    where a.Created >= '2025-1-1'
                    and a.Status = 'Completed'
                    and (select count(*) from WorkflowInstances where ItemId = a.Id and IsITUpdate = 1) = 0
                    and (select IsCompleted from WorkflowInstances where ItemId = a.Id order by created desc
                        OFFSET 0 ROWS
                        FETCH NEXT 1 ROWS ONLY) = 1
                    and (select count(*) from TrackingLogInitDatas where ReferenceNumber = a.ReferenceNumber) = 0
                    and (select h.Modified from WorkflowInstances i
	                    join WorkflowHistories h on (i.Id = h.InstanceId)
	                    where i.ItemId = a.Id
	                    order by h.Modified desc
                        OFFSET 0 ROWS
                        FETCH NEXT 1 ROWS ONLY) < DATEADD(MINUTE, -10, GETDATE())
                    union all
                    select a.id, a.ReferenceNumber, a.Status from LeaveApplications a
                    where a.Created >= '2025-1-1'
                    and a.Status = 'Completed'
                    and (select count(*) from WorkflowInstances where ItemId = a.Id and IsITUpdate = 1) = 0
                    and (select IsCompleted from WorkflowInstances where ItemId = a.Id order by created desc
                        OFFSET 0 ROWS
                        FETCH NEXT 1 ROWS ONLY) = 1
                    and (select count(*) from TrackingLogInitDatas where ReferenceNumber = a.ReferenceNumber) = 0
                    and (select h.Modified from WorkflowInstances i
	                    join WorkflowHistories h on (i.Id = h.InstanceId)
	                    where i.ItemId = a.Id
	                    order by h.Modified desc
                        OFFSET 0 ROWS
                        FETCH NEXT 1 ROWS ONLY) < DATEADD(MINUTE, -10, GETDATE())
                    union all
                    select a.id, a.ReferenceNumber, a.Status from TargetPlans a
                    where a.Created >= '2025-1-1'
                    and a.Status = 'Completed'
                    and (select count(*) from WorkflowInstances where ItemId = a.Id and IsITUpdate = 1) = 0
                    and (select IsCompleted from WorkflowInstances where ItemId = a.Id order by created desc
                        OFFSET 0 ROWS
                        FETCH NEXT 1 ROWS ONLY) = 1
                    and (select count(*) from TrackingLogInitDatas where ReferenceNumber = a.ReferenceNumber) = 0
                    and (select h.Modified from WorkflowInstances i
	                    join WorkflowHistories h on (i.Id = h.InstanceId)
	                    where i.ItemId = a.Id
	                    order by h.Modified desc
                        OFFSET 0 ROWS
                        FETCH NEXT 1 ROWS ONLY) < DATEADD(MINUTE, -10, GETDATE())
                ");
                r_list = this.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetListCompletedTicketWithOutPayload:" + ex);
            }
            return r_list;
        }

    }
}
