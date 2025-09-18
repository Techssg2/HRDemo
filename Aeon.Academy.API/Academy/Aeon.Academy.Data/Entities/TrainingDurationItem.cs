using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingDurationItem : BaseEntity
    {
        [Required]
        public Guid TrainingRequestId { get; set; }
        [Required]
        public string TrainingMethod { get; set; }
        public int Duration { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? From { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? To { get; set; }
        [StringLength(255)]
        public string TrainingLocation { get; set; }

    }
}
