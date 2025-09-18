using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class ParticipantDto
    {
        public Guid Id { get; set; }

        public Guid TrainingRequestId { get; set; }

        [Required]
        public Guid UserId { get; set; }

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
    }
}