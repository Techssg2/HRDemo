using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class OvertimeApplicationDetail: AuditableEntity
    {
        public Guid OvertimeApplicationId { get; set; }
        public DateTimeOffset Date { get; set; } // tương ứng với cột " Date Of Ot"
        public string ReasonCode { get; set; } // tương ứng với cột "reason of ot"
        public string ReasonName { get; set; } // tương ứng với cột "reason of ot"
        public string DetailReason { get; set; } // detail reason of ot
        public string ProposalHoursFrom { get; set; }
        public string ProposalHoursTo { get; set; }
        public string ActualHoursFrom { get; set; }
        public string ActualHoursTo { get; set; }
        public string CalculatedActualHoursFrom { get; set; }
        public string CalculatedActualHoursTo { get; set; }
        public bool DateOffInLieu { get; set; }
        public bool IsNoOT { get; set; }
        /// Manager Apply for Emloyee
        public Guid? UserId { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public bool? IsStore { get; set; }

        // Phía dưới là danh sách khóa ngoại
        public virtual User User { get; set; }
        public virtual OvertimeApplication OvertimeApplication { get; set; }

    }
}