using JobSendMail1STResignationsV2.src.Entity;
using JobSendMail1STResignationsV2.src.Enums;
using JobSendMail1STResignationsV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignationsV2.src.Services
{
    public class EmailHistoryService
    {
        private SQLQuery<EmailHistoryEntity> emailHistoryQuery;

        public EmailHistoryService()
        {
            emailHistoryQuery = new SQLQuery<EmailHistoryEntity>();
        }

        public List<EmailHistoryEntity> GetEmailHistoriesByReferenceNumbers(List<string> referenceNumbers)
        {
            List<EmailHistoryEntity> r_list = new List<EmailHistoryEntity>();
            try
            {
                string referenceNumberParams = string.Join("','", referenceNumbers);
                string selectData = string.Format(@"
                SELECT 
                     [Id]
                    ,[ItemId]
                    ,[ReferenceNumber]
                    ,[ItemType]
                    ,[DepartmentId]
                    ,[DepartmentType]
                    ,[UserSent]
                    ,[IsSent]
                    ,[Created]
                    ,[CreatedBy]
                    ,[Modified]
                    ,[ModifiedBy]
                    ,[IsDeleted]
                FROM [dbo].[EmailHistories]
                WHERE [IsDeleted] = 0 
                AND [ReferenceNumber] IN ('{0}')
                ORDER BY [Created] DESC
                ", referenceNumberParams);

                r_list = emailHistoryQuery.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetEmailHistoriesByReferenceNumbers:" + ex);
            }
            return r_list;
        }

        public async Task<bool> UpdateEmailHistory(EmailHistoryEntity emailHistories)
        {
            try
            {
                string updateQuery = string.Format(@"
                    UPDATE [dbo].[EmailHistories]
                    SET [UserSent] = N'{0}',
                        [Modified] = GETDATE()
                    WHERE [Id] = '{1}'",
                    emailHistories.UserSent?.Replace("'", "''") ?? "",
                    emailHistories.ID);
                await Task.Run(() => emailHistoryQuery.ExecuteRunQuery(updateQuery));
                return true;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"UpdateEmailHistory failed: {ex.Message}");
                return false;
            }
        }
    }
}
