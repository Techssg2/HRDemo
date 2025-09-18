using Aeon.HR.Infrastructure.Abstracts;
using System;

namespace Aeon.HR.Data.Models
{
    public class HeadCount : SoftDeleteEntity
    {
        public Guid DepartmentId { get; set; }
        public Guid JobGradeForHeadCountId { get; set; }
        public int Quantity { get; set; }
        //Link
        //+ Department trong setting
        public virtual Department Department { get; set; }
        public virtual JobGrade JobGradeForHeadCount { get; set; }
    }
}