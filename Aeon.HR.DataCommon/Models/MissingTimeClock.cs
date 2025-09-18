using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class MissingTimeClock: WorkflowEntity, ICBEntity
    {
        public MissingTimeClock()
        {
            MissingTimeClockDetails = new HashSet<MissingTimeClockDetail>();
        }       
        public string ListReason { get; set; }
        public string UserSAPCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public Guid DeptId { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public virtual ICollection<MissingTimeClockDetail> MissingTimeClockDetails { get; set; }
    }
}