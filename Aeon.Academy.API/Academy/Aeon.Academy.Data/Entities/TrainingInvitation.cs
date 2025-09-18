using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingInvitation : BaseTrackingEntity
    {
        [Required]
        public Guid TrainingRequestId { get; set; }
        [StringLength(255)]
        [Required]
        public string ReferenceNumber { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? CourseId { get; set; }

        [StringLength(255)]
        [Required]
        public string CourseName { get; set; }

        [StringLength(255)]
        public string ServiceProvider { get; set; }

        public Guid? TrainerId { get; set; }

        [StringLength(255)]
        public string TrainerName { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime EndDate { get; set; }


        [Required]
        public int TotalOnlineTrainingHours { get; set; }

        [Required]
        public int TotalOfflineTrainingHours { get; set; }
        

        [StringLength(255)]
        public string TrainingLocation { get; set; }

        [Required]
        public int HoursPerDay { get; set; }

        [Required]
        public int NumberOfDays { get; set; }

        [Required]
        public int TotalHours { get; set; }

        public string Note { get; set; }

        [Required]
        public bool AfterTrainingReportNotRequired { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime AfterTrainingReportDeadline { get; set; }

        public string Content { get; set; }

        public string Status { get; set; }

        public Guid? DepartmentId { get; set; }

        [StringLength(255)]
        public string DepartmentName { get; set; }

        [StringLength(50)]
        public string CreatedBySapCode { get; set; }

        public virtual TrainingRequest TrainingRequest { get; set; }

        public virtual IList<TrainingInvitationParticipant> TrainingInvitationParticipants { get; set; }
    }
}
