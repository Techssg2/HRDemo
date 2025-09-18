using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.Data.Models
{
    public class User : SoftDeleteEntity
    {
        public User()
        {
            PromoteAndTransferOfUsers = new HashSet<PromoteAndTransfer>();
            PromoteAndTransferOfReportTo = new HashSet<PromoteAndTransfer>();
            ActingOfUsers = new HashSet<Acting>();
            ActingOfFirstAppraisers = new HashSet<Acting>();
            ActingOfSecondAppraisers = new HashSet<Acting>();
            LeaveApplications = new HashSet<LeaveApplication>();
            OvertimeApplications = new HashSet<OvertimeApplication>();
            UserDepartmentMappings = new HashSet<UserDepartmentMapping>();
            ShiftExchangeApplicationDetails = new HashSet<ShiftExchangeApplicationDetail>();
            MissingTimeClocks = new HashSet<MissingTimeClock>();
            ResignationApplications = new HashSet<ResignationApplication>();
        }
        public string FullName { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string Email { get; set; }
        //public byte[] ProfilePicture { get; set; }    
        public Guid? ProfilePictureId { get; set; }
        public bool IsActivated { get; set; }
        public LoginType Type { get; set; }
        public UserRole Role { get; set; }
        public Gender Gender { get; set; }
        public bool CheckAuthorizationUSB { get; set; }
        public bool IsTargetPlan { get; set; }
        public bool IsNotTargetPlan { get; set; }
        // Ngày nhân viên bắt đầu vào làm AEON 
        public DateTime? StartDate { get; set; }
        // Extra Rest Day (Nghỉ bù thứ bảy) còn lại trong năm
        //public double ErdRemain { get; set; }
        //public double AlRemain { get; set; }
        //public double DoflRemain { get; set; }
        #region Phần khóa ngoại do Huy thêm vào
        public virtual ICollection<MissingTimeClock> MissingTimeClocks { get; set; }
        public virtual ICollection<ResignationApplication> ResignationApplications { get; set; }

        //Khóa ngoại của handover
        public virtual ICollection<Handover> HandoverOfUsers { get; set; } // Nhân viên được chọn trong dropdown "SAP Code" Trong handover form

        // Khóa ngoại của promote and transfer
        [InverseProperty("User")] // cái InverseProperty dùng để phân biệt là đang khóa ngoại với thuộc tính nào bên bảng bên kia nếu một bảng có nhiều khóa ngoại để một bảng khác
        public virtual ICollection<PromoteAndTransfer> PromoteAndTransferOfUsers { get; set; } // Nhân viên được chọn trong dropdown SAP  Trong promote and transfer
        [InverseProperty("ReportTo")]
        public virtual ICollection<PromoteAndTransfer> PromoteAndTransferOfReportTo { get; set; } // Danh sách các prômte và transfer mà nhân viên này được được chọn trong mục report to

        // Khóa ngoại của acting
        [InverseProperty("User")]
        public virtual ICollection<Acting> ActingOfUsers { get; set; } // Nhân viên được chọn trong dropdown acting
        [InverseProperty("FirstAppraiser")]
        public virtual ICollection<Acting> ActingOfFirstAppraisers { get; set; } // Danh sách các acting mà nhân viên này được chọn là aprraiser 1
        [InverseProperty("SecondAppraiser")]
        public virtual ICollection<Acting> ActingOfSecondAppraisers { get; set; } //  Danh sách các acting mà nhân viên này được chọn là aprraiser 2

        // Khóa ngoại của leave application
        public virtual ICollection<LeaveApplication> LeaveApplications { get; set; } // Nhân viên tạo ra request leave application

        // Khóa ngoại của overtime application 
        public virtual ICollection<OvertimeApplication> OvertimeApplications { get; set; } // Nhân viên tạo ra request overtime application

        // Khóa ngoại của UserDepartmentMapping 
        public virtual ICollection<UserDepartmentMapping> UserDepartmentMappings { get; set; } // Danh sách phòng ban mà nhân viên này trực thuộc
        // Khóa ngoại cho shift exchange application detail
        public virtual ICollection<ShiftExchangeApplicationDetail> ShiftExchangeApplicationDetails { get; set; } // Những shift excchange chi tiết mà nhân viên này sỡ hữu // là những cái mà nhân viên này được thêm vào bên trong table
        //public virtual ICollection<ShiftExchangeApplication> ShiftExchangeApplications { get; set; } // Những shift exchange mà nhân viên này là người tạo đơn
        #endregion
        public string QuotaDataJson { get; set; }
        public string RedundantPRD { get; set; } // PRD còn dư của tháng trước tháng hiện tại
        public virtual AttachmentFile ProfilePicture { get; set; }
        public virtual ICollection<BusinessTripApplicationDetail> BusinessTripApplicationDetails { get; set; }
        public bool? HasTrackingLog { get; set; }
        public bool IsFromIT { get; set; }
        public virtual ICollection<UserPasswordHistories> UserPasswordHistory { get; set; }
        public int CountLogin { get; set; }
        public bool IsEdoc { get; set; }
    }
}