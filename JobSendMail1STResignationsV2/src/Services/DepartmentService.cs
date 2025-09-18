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
    public class DepartmentService
    {
        private SQLQuery<DepartmentEntity> departmentQuery;

        public DepartmentService()
        {
            departmentQuery = new SQLQuery<DepartmentEntity>();
        }

        public List<DepartmentEntity> GetAssistanceNodes()
        {
            List<DepartmentEntity> r_list = new List<DepartmentEntity>();
            try
            {
                string selectData = string.Format(@"
                SELECT 
                    d.[Id] AS ID
                    ,d.[Code]
                    ,d.[Name]
                    ,d.[JobGradeId]
                    ,d.[ParentId]
                    ,d.[IsStore]
                    ,d.[Created]
                    ,d.[CreatedBy]
                    ,d.[Modified]
                    ,d.[ModifiedBy]
                FROM [dbo].[Departments] d
                INNER JOIN [dbo].[UserDepartmentMappings] udm ON d.[Id] = udm.[DepartmentId]
                WHERE udm.[IsHeadCount] = 1
                AND udm.[Role] = {0}
                ", (int)Group.Assistance);
                r_list = departmentQuery.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("[ERROR] - GetAssistanceNodes:" + ex);
            }
            return r_list;
        }
    }
}
