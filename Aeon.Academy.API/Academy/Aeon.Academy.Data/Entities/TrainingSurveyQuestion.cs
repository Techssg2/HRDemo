using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingSurveyQuestion : BaseEntity
    {
        public Guid TrainingReportId { get; set; }
        
        [Required]
        public string SurveyQuestion { get; set; }
        public string ParentQuestion { get; set; }
        public string Value { get; set; }

        public virtual TrainingReport TrainingReport { get; set; }
    }
}
