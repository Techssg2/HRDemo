using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class UserSelectViewModel
    {
        public Guid Id { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public int? JobGradeGrade { get; set; }
        public string JobGradeCaption { get; set; }
    }
}