using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class JobGradeItemRecruitmentMapping: SoftDeleteEntity
    {
        public Guid JobGradeId { get; set; }
        public Guid ItemListRecruitmentId { get; set; }

        public virtual JobGrade JobGrade { get; set; }
        public virtual ItemListRecruitment ItemListRecruitment { get; set; }
    }
}
