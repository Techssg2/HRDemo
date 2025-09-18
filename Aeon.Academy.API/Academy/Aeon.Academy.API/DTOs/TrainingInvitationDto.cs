using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingInvitationDto
    {
        public TrainingInvitationDto()
        {
            Participants = new List<TrainingInvitationParticipantDto>();
        }
        public Guid Id { get; set; }

        public string Status { get; set; }

        [Required]
        public Guid TrainingRequestId { get; set; }

        [StringLength(255)]
        [Required]
        public string ReferenceNumber { get; set; }

        public Guid? CategoryId { get; set; }

        public string CategoryName { get; set; }
       
        public Guid? CourseId { get; set; }

        [Required]
        [StringLength(255)]
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

        public int NumberOfParticipant { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        [StringLength(50)]
        public string CreatedBySapCode { get; set; }
        public object Attachments { get; set; }

        public List<TrainingInvitationParticipantDto> Participants { get; set; }
    }
}