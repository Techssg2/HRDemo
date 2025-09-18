using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class BusinessTripDTO
    {
        public Guid? Id { get; set; }
        public Guid? DeptLineId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public Guid? DeptDivisionId { get; set; }
        public string DeptDivisionCode { get; set; }
        public string DeptDivisionName { get; set; }
        public string BusinessTripDetails { get; set; } // json      
        public string ChangeCancelBusinessTripDetails { get; set; }  // json       
        public string Status { get; set; }
        public string UserSAPCode { get; set; }
        public string UserCreatedFullName { get; set; }
        public string MaxGrade { get; set; }
        public bool IsStore { get; set; }
        public BTAType Type { get; set; }
        public Guid? MaxDepartmentId { get; set; }
        public string DocumentDetails { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string DocumentChanges { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public string RequestorNote { get; set; }
        public string RequestorNoteDetail { get; set; }
        public string CarRental { get; set; }
        public bool IsRoundTrip { get; set; }
        public bool StayHotel { get; set; }
        public bool CheckRoomNextStep { get; set; } // check neu chua co chon phong khach san thi day len tren buoc tiep theo
        public string BookingContact { get; set; }
        public string CarRentalAttachmentDetails { get; set; }
        public string VisaAttachmentDetails { get; set; }

    }
}
