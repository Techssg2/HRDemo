using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class RecruitmentCategory : SoftDeleteEntity
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public Guid? ParentId { get; set; }
        public virtual RecruitmentCategory Parent { get; set; }
        public virtual ICollection<RecruitmentCategory> ChildCategories { get; set; }
        public virtual ICollection<RequestToHire> RequestToHires { get; set; }
    }
}
