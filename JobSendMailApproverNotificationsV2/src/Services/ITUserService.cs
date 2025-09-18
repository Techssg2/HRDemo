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
    public class ITUserService
    {
        private SQLQuery<ITUserEntity> itUserQuery;

        public ITUserService()
        {
            itUserQuery = new SQLQuery<ITUserEntity>();
        }

        public string GetUserEmailEdoc1(UserEntity user)
        {
            string r_email = user.Email;
            try
            {
                string selectData = string.Format(@"
                SELECT
                    [Id]
                    ,[EmailEdoc1]
                FROM [dbo].[ITUsers]
                WHERE [Id] = '{0}'", user.ID);
                var itUsers = itUserQuery.GetItemsByQuery(selectData);

                if (itUsers != null && itUsers.Any() && !string.IsNullOrEmpty(itUsers[0].EmailEdoc1))
                {
                    r_email = itUsers[0].EmailEdoc1;
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetUserEmailEdoc1:" + ex);
            }
            return r_email;
        }
    }
}
