using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingRequestParticipant : BaseEntity
    {
        [Required]
        public Guid TrainingRequestId { get; set; }

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

        [JsonIgnore]
        public virtual TrainingRequest TrainingRequest { get; set; }

    }
}
