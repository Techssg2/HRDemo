using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ItemListDepartmentForHeadCountViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid JobGradeForHeadCountId { get; set; }
        public string JobGradeCaption { get; set; }
        public int JobGradeValue { get; set; }
    }
}