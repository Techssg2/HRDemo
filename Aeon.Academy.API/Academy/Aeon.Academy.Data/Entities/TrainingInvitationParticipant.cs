using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingInvitationParticipant : BaseEntity
    {
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
        public int? SapStatusCode { get; set; }

        public virtual TrainingInvitation TrainingInvitation { get; set; }

    }
}
