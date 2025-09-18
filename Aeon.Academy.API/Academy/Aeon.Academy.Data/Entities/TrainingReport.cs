using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingReport : BaseWorkflowEntity, IDataEntity
    {
        [StringLength(255)]
        [Required]
        public string SapCode { get; set; }

        [StringLength(255)]
        [Required]
        public string FullName { get; set; }

        [StringLength(255)]
        public string Location { get; set; }

        [Required]
        public Guid? DepartmentId { get; set; }

        [StringLength(255)]
        [Required]
        public string DepartmentName { get; set; }

        public Guid? RegionId { get; set; }
        [StringLength(255)]
        public string RegionName { get; set; }

        [StringLength(255)]
        public string Position { get; set; }

        [Required]
        public Guid TrainingInvitationId { get; set; }

        [StringLength(255)]
        public string TrainerName { get; set; }
        public string OtherReasons { get; set; }
        public string OtherFeedback { get; set; }
        public string Remark { get; set; }
        public DateTime? ActualAttendingDate { get; set; }

        public virtual IList<TrainingActionPlan> TrainingActionPlans { get; set; }
        public virtual IList<TrainingSurveyQuestion> TrainingSurveyQuestions { get; set; }
        public virtual TrainingInvitation TrainingInvitation { get; set; }
    }
}