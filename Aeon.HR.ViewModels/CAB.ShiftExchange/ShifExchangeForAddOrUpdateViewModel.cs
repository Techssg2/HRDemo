using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ShifExchangeForAddOrUpdateViewModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset ApplyDate { get; set; }
        public Guid? DeptLineId { get; set; }
        public Guid? DeptDivisionId { get; set; }
        public Guid currentUserId { get; set; }       
        public virtual ICollection<ShiftExchangeDetailForAddOrUpdateViewModel> ExchangingShiftItems { get; set; }
        public bool IsVNMessage { get; set; }
    }
}
