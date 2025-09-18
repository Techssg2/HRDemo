using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class BTAViewModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid? DeptLineId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public Guid? DeptDivisionId { get; set; }
        public string DeptDivisionCode { get; set; }
        public string DeptDivisionName { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset SignedDate { get; set; }
        public string SignedBy { get; set; }
        public string UserSAPCode { get; set; }
        public string UserFullName { get; set; }
        public DateTimeOffset Created { get; set; }
        public string MaxGrade { get; set; }
        public bool IsStore { get; set; }
        public bool IsRoundTrip { get; set; }
        public bool IsOverBudget { get; set; }
        public bool StayHotel { get; set; }
        public bool CheckRoomNextStep { get; set; } // check neu chua co chon phong khach san thi day len tren buoc tiep theo
        public string EmployeeCode { get; set; }
        public BTAType Type { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public Guid? MaxDepartmentId { get; set; }
        public string DocumentDetails { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string DocumentChanges { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string RequestorNote { get; set; }
        public string RequestorNoteDetail { get; set; }
        public string CarRental { get; set; }
        public bool? IsRequestToChange { get; set; }
        public DateTimeOffset? BusinessTripFrom { get; set; }
        public DateTimeOffset? BusinessTripTo { get; set; }
        //tudm
        public Guid? CreatedById { get; set; }
        public bool IsCheckRe { get; set; }
        public string ReferenceNumberRE { get; set; }
        public string Url_RE { get; set; }
        public DateTimeOffset? ModifiedRE { get; set; }
        public string BookingContact { get; set; }
        public string CarRentalAttachmentDetails { get; set; }
        public string VisaAttachmentDetails { get; set; }
    }

    public class BTAOverBudgetViewModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid? DeptLineId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public Guid? DeptDivisionId { get; set; }
        public string DeptDivisionCode { get; set; }
        public string DeptDivisionName { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset SignedDate { get; set; }
        public string SignedBy { get; set; }
        public string UserSAPCode { get; set; }
        public string UserFullName { get; set; }
        public DateTimeOffset Created { get; set; }
        public string MaxGrade { get; set; }
        public bool IsStore { get; set; }
        public bool IsRoundTrip { get; set; }
        public bool IsOverBudget { get; set; }
        public bool StayHotel { get; set; }
        public string EmployeeCode { get; set; }
        public BTAType Type { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public Guid? MaxDepartmentId { get; set; }
        public string DocumentDetails { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string DocumentChanges { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string RequestorNote { get; set; }
        public string CarRental { get; set; }
        public bool? IsRequestToChange { get; set; }
        public DateTimeOffset? BusinessTripFrom { get; set; }
        public DateTimeOffset? BusinessTripTo { get; set; }
        public string BTAReferenceNumber { get; set; }
        public Guid? BusinessTripApplicationId { get; set; }
        public string Comment { get; set; }
    }
    public class ExportBusinessTripApplicantionViewModel
    {
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string SubmittedSAPCode { get; set; }
        public string SubmittedFullName { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionName { get; set; }
        public string DepartFlight { get; set; }
        public string ReturnFlight { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool IsForeigner { get; set; }
        public string Gender { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public string HotelName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CheckInHotelDate { get; set; }
        public string CheckOutHotelDate { get; set; }
        public string CreateDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IDCard { get; set; }
        public string Passport { get; set; }
        public bool IsCancelled { get; set; }
    }
}
