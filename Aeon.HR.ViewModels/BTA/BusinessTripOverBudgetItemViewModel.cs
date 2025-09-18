using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class BusinessTripOverBudgetItemViewModel
    {
        public Guid Id { get; set; }
        public Guid? DeptLineId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public Guid? DeptDivisionId { get; set; }
        public string DeptDivisionCode { get; set; }
        public string DeptDivisionName { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public DateTimeOffset SignedDate { get; set; }
        public string SignedBy { get; set; }
        public string UserSAPCode { get; set; }
        public string UserFullName { get; set; }
        public BTAType Type { get; set; }
        public DateTimeOffset Created { get; set; }
        public IEnumerable<BusinessOverBudgetDetailDTO> BusinessTripOverBudgetDetails { get; set; }

        public string EmployeeCode { get; set; }
        public string DocumentDetails { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string DocumentChanges { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string RequestorNote { get; set; }
        public string CarRental { get; set; }
        public bool? IsRequestToChange { get; set; }
        public bool IsOverBudget { get; set; }
        public bool IsRoundTrip { get; set; }
        public bool StayHotel { get; set; }
        public string BTAReferenceNumber { get; set; }
        public Guid? BusinessTripApplicationId { get; set; }
        public DateTimeOffset? BusinessTripFrom { get; set; }
        public DateTimeOffset? BusinessTripTo { get; set; }
        public Guid? MaxDepartmentId { get; set; }
        public string MaxGrade { get; set; }
        public bool IsStore { get; set; }
        public string Comment { get; set; }
        public bool? LargerThanOrEqual10Day { get; set; }
        public bool? LessThan10Day { get; set; }
    }
}
