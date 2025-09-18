using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ItemListApplicantViewModel
    {
        public string Id { get; set; }
        public string ApplicantStatusName { get; set; }
        public string ReferenceNumber { get; set; }
        public string FullName { get; set; }
        public string PositionName { get; set; }
        public string IDCard9Number { get; set; }
        public string IDCard12Number { get; set; }
        public string DeptDivisionName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string GenderName { get; set; }
        public string NativeName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}