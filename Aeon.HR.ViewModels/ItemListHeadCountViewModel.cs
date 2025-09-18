using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ItemListHeadCountViewModel
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DepartmentJobGradeId { get; set; }
        public Guid JobGradeForHeadCountId { get; set; }
        public int JobGradeValue { get; set; }
        public string JobGradeCaption { get; set; }
        public string JobGradeTitle { get; set; }
        public string DepartmentJobGradeCaption { get; set; }
        public string DepartmentName { get; set; }
        public int Quantity { get; set; }
    }
}