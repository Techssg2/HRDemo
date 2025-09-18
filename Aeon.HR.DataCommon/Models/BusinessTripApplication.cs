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
    public class BusinessTripApplication : WorkflowEntity, ICBEntity
    {
        public BusinessTripApplication()
        {
            BusinessTripApplicationDetails = new HashSet<BusinessTripApplicationDetail>();
            ChangeCancelBusinessTripDetails = new HashSet<ChangeCancelBusinessTripDetail>();
            RoomOrganizations = new HashSet<RoomOrganization>();
            BusinessTripOverBudgets = new HashSet<BusinessTripOverBudget>();
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
        public string OldMaxGrade { get; set; }
        public string MaxGrade { get; set; }
        public bool IsStore { get; set; }
        public BTAType Type { get; set; }
        public string RequestorNote { get; set; }
        public string RequestorNoteDetail { get; set; }
        public string CarRental { get; set; }
        public bool? IsRequestToChange { get; set; } // dành cho Admin G5 Request to Change về Admin Checker
        public bool IsOverBudget { get; set; }
        public bool IsRoundTrip { get; set; }
        public bool StayHotel { get; set; }
        public virtual Department DeptLine { get; set; }
        public virtual Department DeptDivision { get; set; }
        public virtual Department MaxDepartment { get; set; }
        public virtual ICollection<BusinessTripApplicationDetail> BusinessTripApplicationDetails { get; set; }
        [NotMapped]
        public virtual ICollection<ChangeCancelBusinessTripDetail> ChangeCancelBusinessTripDetails { get; set; }
        [NotMapped]
        public virtual ICollection<RoomOrganization> RoomOrganizations { get; set; }
        [NotMapped]
        public virtual ICollection<BusinessTripOverBudget> BusinessTripOverBudgets { get; set; }
        public string DocumentDetails { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string DocumentChanges { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public DateTimeOffset? BusinessTripFrom { get; set; }
        public DateTimeOffset? BusinessTripTo { get; set; }
        public bool CheckRoomNextStep { get; set; } // check neu chua co chon phong khach san thi day len tren buoc tiep theo
        //tudm add API_RE
        public bool IsCheckRe { get; set; }
        public string Url_RE { get; set; }
        public string ReferenceNumberRE { get; set; }
        public DateTimeOffset? ModifiedRE { get; set; }
        public string BookingContact { get; set; }
        public string CarRentalAttachmentDetails { get; set; } // dành cho button Car Rental attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string VisaAttachmentDetails { get; set; } // dành cho button Visa attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
    }
}