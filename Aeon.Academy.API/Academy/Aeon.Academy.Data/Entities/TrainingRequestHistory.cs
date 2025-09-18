using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingRequestHistory : BaseEntity
    {
        public Guid TrainingRequestId { get; set; }

        public DateTimeOffset Created { get; set; }

        public Guid CreatedById { get; set; }

        [StringLength(255)]
        [Required]
        public string CreatebBy { get; set; }

        [StringLength(255)]
        [Required]
        public string CreatedByFullName { get; set; }

        [StringLength(50)]
        [Required]
        public string ReferenceNumber { get; set; }

        public string Comment { get; set; }

        [StringLength(50)]
        [Required]
        public string Action { get; set; }
        [Required]
        public int? StepNumber { get; set; }

        [StringLength(150)]
        public string AssignedToDepartmentName { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public int? RoundNumber { get; set; }

        public virtual TrainingRequest TrainingRequest { get; set; }
    }
}
