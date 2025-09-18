using Aeon.HR.Infrastructure.Enums;
using System;

namespace Aeon.HR.ViewModels.Args
{
    public class LeaveApplicationForCreatingArgs
    {
        public LeaveApplicationForCreatingArgs()
        {
        }
        public Guid Id { get; set; }
        public string UserSAPCode { get; set; }
        public string ReferenceNumber { get; set; }      
        public string Status { get; set; }
        public string LeaveApplicantDetails { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public Guid? CreatedById { get; set; }
        //public Guid? UserId { get; set; }
        public DateTimeOffset? StartingDate { get; set; }

        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy

    }
}