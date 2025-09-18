using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class BusinessOverBudgetDTO
    {
        public Guid? Id { get; set; }
        public Guid? DeptLineId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public Guid? DeptDivisionId { get; set; }
        public string DeptDivisionCode { get; set; }
        public string DeptDivisionName { get; set; }
        public string BusinessTripOverBudgetDetails { get; set; } // json   
        public string Status { get; set; }
        public string UserSAPCode { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserFullName { get; set; }
        public string UserCreatedFullName { get; set; }
        public string MaxGrade { get; set; }
        public bool IsStore { get; set; }
        public BTAType Type { get; set; }
        public Guid? MaxDepartmentId { get; set; }
        public string DocumentDetails { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string DocumentChanges { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string RequestorNote { get; set; }
        public string CarRental { get; set; }
        public bool IsRoundTrip { get; set; }
        public bool StayHotel { get; set; }

        //tuhm
        public string RoundTrip { get; set; } //check IsRoundChip
        public string Comment { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
