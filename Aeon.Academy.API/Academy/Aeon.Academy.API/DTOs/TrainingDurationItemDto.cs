using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingDurationItemDto
    {
        public Guid Id { get; set; }
        [Required]
        public string TrainingMethod { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Duration { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? From { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? To { get; set; }
        public string TrainingLocation { get; set; }
    }
}