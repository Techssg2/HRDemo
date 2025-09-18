using System;
using System.Collections.Generic;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;

namespace Aeon.HR.Data.Models
{
    public class LeaveApplication : WorkflowEntity, ICBEntity
    {
        public LeaveApplication()
        {
            LeaveApplicationDetails = new HashSet<LeaveApplicationDetail>();
        }
        //--START-BOSUNG Workflow và status

        //--END-BOSUNG              
        public Guid? DeptId { get; set; }
        public string UserSAPCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        //public Guid? UserId { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
        public bool Is2ndApproval { get; set; }
        public virtual ICollection<LeaveApplicationDetail> LeaveApplicationDetails { get; set; }
        public virtual ICollection<AttachmentFile> AttachmentFiles { get; set; }
        //--Phía dưới là các thông tin sync từ bên sap bên kia qua
        // Có cái field starting date thì call bên api qua api "Search Employee"
        // các field phía dưới thì call lên api "Get Leave Balance"
        // Balance [Năm]
        // Balance [Ngày cuối năm]
        // 5th AL BONUS (if round 5 year)

        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
    }
}