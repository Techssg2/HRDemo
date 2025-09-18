using JobSendMailApproverNotificationsV2.src.Entity;
using JobSendMailApproverNotificationsV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Services
{
    public class UserDepartmentMappingService
    {
        private SQLQuery<UserDepartmentMappingEntity> userDepartmentMappingQuery;

        public UserDepartmentMappingService()
        {
            userDepartmentMappingQuery = new SQLQuery<UserDepartmentMappingEntity>();
        }
        public List<UserDepartmentMappingEntity> GetUserMappingsForAssistanceNodes(List<DepartmentEntity> assistanceNodes)
        {
            List<UserDepartmentMappingEntity> r_list = new List<UserDepartmentMappingEntity>();
            try
            {
                if (assistanceNodes == null || !assistanceNodes.Any())
                    return r_list;
                string departmentIds = string.Join(",", assistanceNodes.Select(d => "'" + d.ID + "'"));
                string selectData = string.Format(@"
                SELECT 
                    udm.[Id] AS ID
                    ,udm.[UserId]
                    ,udm.[DepartmentId]
                    ,udm.[Role]
                    ,udm.[IsHeadCount]
                    ,udm.[Created]
                    ,udm.[CreatedBy]
                    ,udm.[Modified]
                    ,udm.[ModifiedBy]
                    ,u.[FullName] AS UserFullName
                    ,u.[Email] AS UserEmail
                FROM [dbo].[UserDepartmentMappings] udm
                INNER JOIN [dbo].[Users] u ON udm.[UserId] = u.[Id]
                WHERE udm.[DepartmentId] IN ({0})
                ", departmentIds);
                r_list = userDepartmentMappingQuery.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetUserMappingsForAssistanceNodes:" + ex);
            }
            return r_list;
        }
    }
}
