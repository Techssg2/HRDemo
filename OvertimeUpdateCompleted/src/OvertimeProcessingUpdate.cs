using Aeon.OvertimeUpdateCompleted.Utilities;
using OvertimeUpdateCompleted.src.ModelEntity;
using OvertimeUpdateCompleted.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OvertimeUpdateCompleted.src
{
    public class OvertimeProcessingUpdate : SQLQuery<OvertimeApplicationEntity>
    {
        public OvertimeProcessingUpdate() {}
        public List<string> ExcuteUpdate()
        {
            Utilities.WriteLogError("Function ExcuteUpdate() run at: " + DateTimeOffset.Now);
            List<OvertimeApplicationEntity> refererNumbers = this.GetListOvertimeWrongStatus();
            if (refererNumbers.Any())
            {
                foreach (var item in refererNumbers)
                {
                    if (item.ReferenceNumber.StartsWith("OVE-"))
                    {
                        this.ExecuteRunQuery(string.Format(@"Update OvertimeApplications set Status='Completed', OldStatus='{1}', UpdateStatusDate='{2}' where ID='{0}'", item.ID, item.Status, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                    else if(item.ReferenceNumber.StartsWith("LEA-"))
                    {
                        this.ExecuteRunQuery(string.Format(@"Update LeaveApplications set Status='Completed' where ID='{0}'", item.ID));
                    }
                    else if (item.ReferenceNumber.StartsWith("SHI-"))
                    {
                        this.ExecuteRunQuery(string.Format(@"Update ShiftExchangeApplications set Status='Completed' where ID='{0}'", item.ID));
                    }
                }
                refererNumbers.ForEach(x => Utilities.WriteLogError(x.ReferenceNumber));
            }
            return refererNumbers.Any() ? refererNumbers.Select(x => x.ReferenceNumber).ToList() : new List<string>();
        }

        public List<OvertimeApplicationEntity> GetListOvertimeWrongStatus()
        {
            List<OvertimeApplicationEntity> r_list = new List<OvertimeApplicationEntity>();
            try
            {
                string selectData = string.Format(@"
                    select a.id, a.ReferenceNumber, a.Status from OvertimeApplications a
                        where 
                            (select IsCompleted from WorkflowTasks where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 1
                            and (select Vote from WorkflowTasks where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) <> 4
                        and (select IsCompleted from WorkflowInstances where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 1
                        and a.Status not in
                        (
                            'Cancelled',
                            'Completed',
                            'Draft',
                            'Pending',
                            'Reject',
                            'Rejected',
                            'Requested To Change',
                            'Out Of Period',
                            'Approved'
                        )
                    union
                    select a.id, a.ReferenceNumber, a.Status from ShiftExchangeApplications a
                        where 
                            (select IsCompleted from WorkflowTasks where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 1
                            and (select Vote from WorkflowTasks where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) <> 4
                        and (select IsCompleted from WorkflowInstances where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 1
                        and a.Status not in
                        (
                            'Cancelled',
                            'Completed',
                            'Draft',
                            'Pending',
                            'Reject',
                            'Rejected',
                            'Requested To Change',
                            'Out Of Period',
                            'Approved'
                        )
                    union
                    select a.id, a.ReferenceNumber, a.Status from LeaveApplications a
                        where 
                            (select IsCompleted from WorkflowTasks where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 1
                            and (select Vote from WorkflowTasks where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) <> 4
                        and (select IsCompleted from WorkflowInstances where ItemId = a.id order by created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 1
                        and a.Status not in
                        (
                            'Cancelled',
                            'Completed',
                            'Draft',
                            'Pending',
                            'Reject',
                            'Rejected',
                            'Requested To Change',
                            'Out Of Period',
                            'Approved'
                        )
                ");
                r_list = this.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetListOvertimeWrongStatus:" + ex);
            }
            return r_list;
        }

    }
}
