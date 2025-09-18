using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class UserInfoCABViewModel
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public Guid? JobGradeId { get; set; }
        public bool IsActivated { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public int JobGrade { get; set; }
    }
}