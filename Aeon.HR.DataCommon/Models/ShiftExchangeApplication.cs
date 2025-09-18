using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class ShiftExchangeApplication: WorkflowEntity, ICBEntity
    {
        public ShiftExchangeApplication()
        {
            ExchangingShiftItems = new HashSet<ShiftExchangeApplicationDetail>();
        }

        public DateTimeOffset ApplyDate { get; set; }
        public Guid? DeptLineId { get; set; }
        public Guid? DeptDivisionId { get; set; }

        // Phía dưới là danh sách khóa ngoại
        public virtual ICollection<ShiftExchangeApplicationDetail> ExchangingShiftItems { get; set; }
        public virtual Department DeptLine { get; set; }
        public virtual Department DeptDivision { get; set; }
        [ForeignKey("CreatedById")]
        public virtual User UserCreatedBy { get; set; }
        //Ngan
        public string UserSAPCode { get; set; }
        //end
    }
}