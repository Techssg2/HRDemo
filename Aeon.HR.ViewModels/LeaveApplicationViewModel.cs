using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.Args;
using System;
using System.Collections.Generic;

namespace Aeon.HR.ViewModels
{
    public class LeaveApplicationViewModel: CBUserInfoViewModel
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }       
        public string Status { get; set; }
        public DateTimeOffset Created { get; set; }
        public string EmployeeCode { get; set; }
        //public Guid? UserId { get; set; }
        public Guid CreatedById { get; set; }
        public string UserNameCreated { get; set; }
        public bool Is2ndApproval { get; set; }

        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
    }
}