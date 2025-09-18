using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingInvitationParticipantDto
    {
        public TrainingInvitationParticipantDto()
        {
            EnabledActions = new List<string>();
        }
        public Guid Id { get; set; }

        [Required]
        public Guid TrainingInvitationId { get; set; }

        [Required]
        public Guid ParticipantId { get; set; }

        [StringLength(50)]
        [Required]
        public string SapCode { get; set; }

        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [StringLength(255)]
        [Required]
        public string Email { get; set; }

        [StringLength(50)]
        [Required]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        [Required]
        public string Position { get; set; }

        public string Department { get; set; }

        public string EmailContent { get; set; }

        [StringLength(50)]
        public string Response { get; set; }

        public string ReasonOfDecline { get; set; }

        [StringLength(50)]
        public string StatusOfReport { get; set; }

        public string ReferenceNumber { get; set; }

        public string CategoryName { get; set; }

        public string CourseName { get; set; }

        public string ServiceProvider { get; set; }

        public string TrainerName { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        public bool AfterTrainingReportNotRequired { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? AfterTrainingReportDeadline { get; set; }

        public string TrainingLocation { get; set; }

        public List<string> EnabledActions { get; set; }

        public string CourseType { get; set; }

        public int NumberOfParticipant { get; set; }

        public string Requester { get; set; }

        public object Attachments { get; set; }
    }
}