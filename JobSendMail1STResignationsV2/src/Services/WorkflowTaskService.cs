using JobSendMail1STResignationsV2.src.Entity;
using JobSendMail1STResignationsV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignationsV2.src.Services
{
    public class WorkflowTaskService
    {
        private SQLQuery<WorkflowTaskEntity> workflowTaskQuery;

        public WorkflowTaskService()
        {
            workflowTaskQuery = new SQLQuery<WorkflowTaskEntity>();
        }

        public List<WorkflowTaskEntity> GetTask()
        {
            List<WorkflowTaskEntity> r_list = new List<WorkflowTaskEntity>();
            try
            {
                string selectData = string.Format(@"
                SELECT 
                     [Id]
                    ,[Title]
                    ,[WorkflowInstanceId]
                    ,[AssignedToId]
                    ,[AssignedToDepartmentId]
                    ,[AssignedToDepartmentGroup]
                    ,[ReferenceNumber]
                    ,[ItemId]
                    ,[ItemType]
                    ,[IsCompleted]
                    ,[IsTurnedOffSendNotification]
                    ,[Created]
                    ,[CreatedBy]
                    ,[Modified]
                    ,[ModifiedBy]
                    ,[Vote]
                    ,[RequestorId]
                    ,[RequestorUserName]
                    ,[RequestorFullName]
                    ,[Status]
                    ,[RequestedDepartmentId]
                    ,[RequestedDepartmentCode]
                    ,[RequestedDepartmentName]
                    ,[IsAttachmentFile]
                    ,[DueDate]
                FROM [dbo].[WorkflowTasks]
                WHERE [IsCompleted] = 0 
                AND [IsTurnedOffSendNotification] = 0
                ORDER BY [Created] DESC
                ");
                r_list = workflowTaskQuery.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("[ERROR] - GetTask:" + ex);
            }
            return r_list;
        }

    }
}
