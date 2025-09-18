using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class BusinessTripItemViewModel
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
        public IEnumerable<BusinessTripDetailDTO> BusinessTripDetails { get; set; }
        public IEnumerable<BusinessOverBudgetDTO> BusinessOverBudgets { get; set; }
        public IEnumerable<ChangeCancelBusinessTripDTO> ChangeCancelBusinessTripDetails { get; set; }
        public IEnumerable<RoomOrganizationDTO> RoomOrganizations { get; set; }
        public IEnumerable<RoomOrganizationDTO> ChangedRoomOrganizations { get; set; }
        public string EmployeeCode { get; set; }
        public string DocumentDetails { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string DocumentChanges { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string RequestorNote { get; set; }
        public string RequestorNoteDetail { get; set; }
        public string CarRental { get; set; }
        public bool? IsRequestToChange { get; set; }
        public bool IsOverBudget { get; set; }
        public bool IsRoundTrip { get; set; }
        public bool StayHotel { get; set; }
        public bool IsCheckRe { get; set; }
        public string ReferenceNumberRE { get; set; }
        public string Url_RE { get; set; }
        public DateTimeOffset? ModifiedRE { get; set; }

        //tudm
        public Guid? CreatedById { get; set; }
        public bool CheckRoomNextStep { get; set; } // check neu chua co chon phong khach san thi day len tren buoc tiep theo
        public string BookingContact { get; set; }
        public string CarRentalAttachmentDetails { get; set; }
        public string VisaAttachmentDetails { get; set; }
    }
}
