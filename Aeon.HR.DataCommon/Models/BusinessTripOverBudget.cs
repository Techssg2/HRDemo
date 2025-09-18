using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class BusinessTripOverBudget : WorkflowEntity, ICBEntity
    {
        public BusinessTripOverBudget()
        {
            BusinessTripOverBudgetDetails = new HashSet<BusinessTripOverBudgetsDetail>();
        }
        public Guid? DeptLineId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public Guid? DeptDivisionId { get; set; }
        public Guid? MaxDepartmentId { get; set; }
        public string DeptDivisionCode { get; set; }
        public string DeptDivisionName { get; set; }
        public string UserSAPCode { get; set; }
        public string UserFullName { get; set; }
        public string MaxGrade { get; set; }
        public bool IsStore { get; set; }
        public BTAType Type { get; set; }
        public string RequestorNote { get; set; }
        public string CarRental { get; set; }
        public bool? IsRequestToChange { get; set; } // dành cho Admin G5 Request to Change về Admin Checker
        public bool IsOverBudget { get; set; }
        public bool IsRoundTrip { get; set; }
        public bool StayHotel { get; set; }
        public virtual Department DeptLine { get; set; }
        public virtual Department DeptDivision { get; set; }
        public virtual Department MaxDepartment { get; set; }
        public Guid? BusinessTripApplicationId { get; set; }
        public string BTAReferenceNumber { get; set; }
        public virtual BusinessTripApplication BusinessTripApplication { get; set; }
        public virtual ICollection<BusinessTripOverBudgetsDetail> BusinessTripOverBudgetDetails { get; set; }
        public string DocumentDetails { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string DocumentChanges { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public DateTimeOffset? BusinessTripFrom { get; set; }
        public DateTimeOffset? BusinessTripTo { get; set; }
        public string Comment { get; set; }
    }
}
