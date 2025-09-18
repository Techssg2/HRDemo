using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingActionPlan : BaseEntity
    {
        public Guid TrainingReportId { get; set; }
        
        [Required]
        public string ActionPlanCode { get; set; }
        public bool Quarter1 { get; set; }
        public bool Quarter2 { get; set; }
        public bool Quarter3 { get; set; }
        public bool Quarter4 { get; set; }

        public virtual TrainingReport TrainingReport { get; set; }
    }
}
