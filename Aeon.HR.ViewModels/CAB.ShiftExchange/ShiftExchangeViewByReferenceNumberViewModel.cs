using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aeon.HR.ViewModels
{
    public class ShiftExchangeViewByReferenceNumberViewModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset ApplyDate { get; set; }
        public Guid? DeptLineId { get; set; }
        public Guid? DeptDivisionId { get; set; }
        public Guid CreatedById { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }
        public ICollection<ShiftExchangeDetailForAddOrUpdateViewModel> ExchangingShiftItems { get; set; }
        public DateTimeOffset Created { get; set; }
        public string DeptLineCode { get; set; }
        public string DeptLineName { get; set; }
        public string DeptDivisionCode { get; set; }
        public string DeptDivisionName { get; set; }
    }
}
