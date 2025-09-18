using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class ItemListRecruitment: SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }

        // Foreign Key
        //public Guid? HandoverId { get; set; }
        // public Guid? JobGradeId { get; set; }
        //Link 
        //+ Job Grade ( Add Items ) trong Setting
        //+ Hangover ( Add A Handover) trong Recruitment
        // public virtual JobGrade JobGrade { get; set; }
        //public virtual Handover Handover { get; set; }
        public virtual ICollection<JobGradeItemRecruitmentMapping>  JobGradeItemRecruitmentMappings { get; set; }
    }
}