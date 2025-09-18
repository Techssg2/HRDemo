using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingDurationDto
    {
        public TrainingDurationDto()
        {
            TrainingDurationItems = new List<TrainingDurationItemDto>();
        }
        [Required]
        public string TrainingMethod { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int EstimatedTrainingDays { get; set; }
        public int? EstimatedTrainingHours { get; set; }
        public List<TrainingDurationItemDto> TrainingDurationItems { get; set; }
    }
}