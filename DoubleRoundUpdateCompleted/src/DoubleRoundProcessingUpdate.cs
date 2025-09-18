using Aeon.OvertimeUpdateCompleted.Utilities;
using OvertimeUpdateCompleted.src.ModelEntity;
using OvertimeUpdateCompleted.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OvertimeUpdateCompleted.src
{
    public class DoubleRoundProcessingUpdate : SQLQuery<DoubleRoundEntity>
    {
        public DoubleRoundProcessingUpdate() {}
        public List<string> ExcuteUpdate()
        {
            Utilities.WriteLogError("Function ExcuteUpdate() run at: " + DateTimeOffset.Now);
            List<DoubleRoundEntity> refererNumbers = this.GetListCompletedDoubleRound();
            if (refererNumbers.Any())
            {
                foreach (var item in refererNumbers)
                {
                    this.ExecuteRunQuery(string.Format(@"Delete from WorkflowHistories where InstanceId='{0}'", item.InstanceId));
                    this.ExecuteRunQuery(string.Format(@"Delete from WorkflowTasks where WorkflowInstanceId='{0}'", item.InstanceId));
                    this.ExecuteRunQuery(string.Format(@"Delete from WorkflowInstances where Id='{0}'", item.InstanceId));
                }
                refererNumbers.ForEach(x => Utilities.WriteLogError(x.ReferenceNumber));
            }
            return refererNumbers.Any() ? refererNumbers.Select(x => x.ReferenceNumber).ToList() : new List<string>();
        }

        public List<DoubleRoundEntity> GetListCompletedDoubleRound()
        {
            List<DoubleRoundEntity> r_list = new List<DoubleRoundEntity>();
            try
            {
                string selectData = string.Format(@"
                    select a.id, a.ReferenceNumber, a.Status, w.id InstanceId from OvertimeApplications a
                    join WorkflowInstances w on (a.id = w.ItemId)
                        where a.created >= '2024-1-1' and w.Id in
                            (select b.id from WorkflowInstances b where b.ItemId = a.id and b.IsCompleted = 0 and b.IsITUpdate = 0
                            and ((select IsCompleted from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select vote from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select IsStepCompleted from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select VoteType from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0)
		                    or ((select count(1) from WorkflowTasks c where c.WorkflowInstanceId = b.id) = 0
		                    and (select count(1) from WorkflowHistories c where c.InstanceId = b.id) = 0))
                            and a.Status = 'Completed'
                    union all
                    select a.id, a.ReferenceNumber, a.Status, w.id InstanceId from ShiftExchangeApplications a
                    join WorkflowInstances w on (a.id = w.ItemId)
                        where a.created >= '2024-1-1' and w.Id in
                            (select b.id from WorkflowInstances b where b.ItemId = a.id and b.IsCompleted = 0 and b.IsITUpdate = 0
                            and ((select IsCompleted from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select vote from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select IsStepCompleted from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select VoteType from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0)
		                    or ((select count(1) from WorkflowTasks c where c.WorkflowInstanceId = b.id) = 0
		                    and (select count(1) from WorkflowHistories c where c.InstanceId = b.id) = 0))
                            and a.Status = 'Completed'
                    union all
                    select a.id, a.ReferenceNumber, a.Status, w.id InstanceId from LeaveApplications a
                    join WorkflowInstances w on (a.id = w.ItemId)
                        where a.created >= '2024-1-1' and w.Id in
                            (select b.id from WorkflowInstances b where b.ItemId = a.id and b.IsCompleted = 0 and b.IsITUpdate = 0
                            and ((select IsCompleted from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select vote from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select IsStepCompleted from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select VoteType from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0)
		                    or ((select count(1) from WorkflowTasks c where c.WorkflowInstanceId = b.id) = 0
		                    and (select count(1) from WorkflowHistories c where c.InstanceId = b.id) = 0))
                            and a.Status = 'Completed'
                    union all
                    select a.id, a.ReferenceNumber, a.Status, w.id InstanceId from TargetPlans a
                    join WorkflowInstances w on (a.id = w.ItemId)
                        where a.created >= '2024-1-1' and w.Id in
                            (select b.id from WorkflowInstances b where b.ItemId = a.id and b.IsCompleted = 0 and b.IsITUpdate = 0
                            and ((select IsCompleted from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select vote from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select IsStepCompleted from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select VoteType from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0)
		                    or ((select count(1) from WorkflowTasks c where c.WorkflowInstanceId = b.id) = 0
		                    and (select count(1) from WorkflowHistories c where c.InstanceId = b.id) = 0))
                            and a.Status = 'Completed'
                    union all
                    select a.id, a.ReferenceNumber, a.Status, w.id InstanceId from MissingTimeClocks a
                    join WorkflowInstances w on (a.id = w.ItemId)
                        where a.created >= '2024-1-1' and w.Id in
                            (select b.id from WorkflowInstances b where b.ItemId = a.id and b.IsCompleted = 0 and b.IsITUpdate = 0
                            and ((select IsCompleted from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select vote from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select IsStepCompleted from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select VoteType from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0)
		                    or ((select count(1) from WorkflowTasks c where c.WorkflowInstanceId = b.id) = 0
		                    and (select count(1) from WorkflowHistories c where c.InstanceId = b.id) = 0))
                            and a.Status = 'Completed'
                    union all
                    select a.id, a.ReferenceNumber, a.Status, w.id InstanceId from ResignationApplications a
                    join WorkflowInstances w on (a.id = w.ItemId)
                        where a.created >= '2024-1-1' and w.Id in
                            (select b.id from WorkflowInstances b where b.ItemId = a.id and b.IsCompleted = 0 and b.IsITUpdate = 0
                            and ((select IsCompleted from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select vote from WorkflowTasks c where c.WorkflowInstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select IsStepCompleted from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0
                            and (select VoteType from WorkflowHistories c where c.InstanceId = b.id order by c.created desc
                            OFFSET 0 ROWS
                            FETCH NEXT 1 ROWS ONLY) = 0)
		                    or ((select count(1) from WorkflowTasks c where c.WorkflowInstanceId = b.id) = 0
		                    and (select count(1) from WorkflowHistories c where c.InstanceId = b.id) = 0))
                            and a.Status = 'Completed'
                ");
                r_list = this.GetItemsByQuery(selectData);
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetListCompletedDoubleRound:" + ex);
            }
            return r_list;
        }

    }
}
