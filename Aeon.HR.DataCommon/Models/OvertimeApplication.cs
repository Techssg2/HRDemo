using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class OvertimeApplication: WorkflowEntity, ICBEntity
    {
        public OvertimeApplication()
        {
            OvertimeItems = new HashSet<OvertimeApplicationDetail>();
        }        //--START-BOSUNG Workflow và status        
        public string UserSAPCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string ContentOfOtherReason { get; set; }
        public OverTimeType Type { get; set; }
        public DateTimeOffset? ApplyDate { get; set; }
        public Guid DivisionId { get; set; }
        public bool IsFollowPlan { get; set; }
        public string TimeInRound { get; set; }
        public string TimeOutRound { get; set; }
        //--END-BOSUNG
        public virtual ICollection<OvertimeApplicationDetail> OvertimeItems { get; set; }
        //--Phía dưới là danh sách khóa ngoại
        public string OldStatus { get; set; }
        public DateTimeOffset? UpdateStatusDate  { get; set; }

        public Guid? DeptId  { get; set; }
    }
}