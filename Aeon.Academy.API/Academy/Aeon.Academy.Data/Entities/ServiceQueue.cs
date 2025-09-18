using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.Data.Entities
{
    public class ServiceQueue : BaseEntity
    {
        [StringLength(255)]
        [Required]
        public string InstanceType { get; set; }
        public string InstanceData { get; set; }
        public string ErrorMessage { get; set; }
        public int NumberOfCall { get; set; }
        public int Disabled { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public string SapCode { get; set; }
        public Guid? TrainingInvitationId { get; set; }
    }
}
