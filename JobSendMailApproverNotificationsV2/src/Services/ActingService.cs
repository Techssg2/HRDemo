using JobSendMailApproverNotificationsV2.src.Entity;
using JobSendMailApproverNotificationsV2.src.Enums;
using JobSendMailApproverNotificationsV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Services
{
    public class ActingService
    {
        private SQLQuery<ActingEntity> actingQuery;

        public ActingService()
        {
            actingQuery = new SQLQuery<ActingEntity>();
        }

        public List<string> GetIgnoreReferenceNumbers()
        {
            List<string> referenceNumberActingValid = new List<string>();
            try
            {
                #region Get Ignore Acting Items
                List<string> ignoreStatus = new List<string>() { "Completed", "Draft", "Cancelled", "Rejected", "Requested To Change" };
                string ignoreStatusString = "'" + string.Join("','", ignoreStatus) + "'";

                string selectData = string.Format(@"
                SELECT 
                    [Id] AS ID
                    ,[ReferenceNumber]
                    ,[Status]
                    ,[Period1To]
                    ,[Period2To]
                    ,[Period3To]
                    ,[Period4To]
                    ,[Created]
                    ,[CreatedBy]
                    ,[Modified]
                    ,[ModifiedBy]
                FROM [dbo].[Actings]
                WHERE [Status] NOT IN ({0})
            ", ignoreStatusString);

                var processingActingItems = actingQuery.GetItemsByQuery(selectData);

                if (processingActingItems != null && processingActingItems.Any())
                {
                    // Lấy thời gian hiện tại ở múi giờ UTC
                    DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

                    foreach (var act in processingActingItems)
                    {
                        if (act.Period4To.HasValue && (act.Status == "Waiting for Appraiser 1" || act.Status == "Waiting for Appraiser 2"))
                        {
                            if (DoesAllowShowTodoList(act.Period4To, nowUtc))
                                referenceNumberActingValid.Add(act.ReferenceNumber);
                        }
                        else if (act.Period3To.HasValue && (act.Status == "Waiting for Appraiser 1" || act.Status == "Waiting for Appraiser 2"))
                        {
                            if (DoesAllowShowTodoList(act.Period3To, nowUtc))
                                referenceNumberActingValid.Add(act.ReferenceNumber);
                        }
                        else if (act.Period2To.HasValue && (act.Status == "Waiting for Appraiser 1" || act.Status == "Waiting for Appraiser 2"))
                        {
                            if (DoesAllowShowTodoList(act.Period2To, nowUtc))
                                referenceNumberActingValid.Add(act.ReferenceNumber);
                        }
                        else if (act.Period1To.HasValue && (act.Status == "Waiting for Appraiser 1" || act.Status == "Waiting for Appraiser 2"))
                        {
                            if (DoesAllowShowTodoList(act.Period1To, nowUtc))
                                referenceNumberActingValid.Add(act.ReferenceNumber);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetIgnoreReferenceNumbers: " + ex.Message);
            }

            return referenceNumberActingValid;
        }

        private bool DoesAllowShowTodoList(DateTimeOffset? periodEnd, DateTimeOffset nowUtc)
        {
            bool result = false;
            try
            {
                if (periodEnd.HasValue)
                {
                    DateTimeOffset periodEndUtc = periodEnd.Value.ToUniversalTime();
                    DateTimeOffset fifteenDaysBeforeEnd = periodEndUtc.AddDays(-15);
                    DateTimeOffset oneDayAfterEnd = periodEndUtc.AddDays(1);

                    if (nowUtc < fifteenDaysBeforeEnd || nowUtc > oneDayAfterEnd)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
