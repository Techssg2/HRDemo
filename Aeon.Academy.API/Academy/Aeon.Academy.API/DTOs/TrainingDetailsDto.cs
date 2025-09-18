using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingDetailsDto
    {
        [StringLength(50)]
        [Required]
        public string TypeOfTraining { get; set; }
        [StringLength(50)]
        public string ExistingCourse { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid? CourseId { get; set; }
        public string CourseName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public decimal? TrainingFee { get; set; }
        public decimal? TrainingNotTotalFee { get; set; }
        public decimal? AccommodationIfAny { get; set; }
        public decimal? AirTicketFeeIfAny { get; set; }
        public string Currency { get; set; }
        public string CurrencyJson { get; set; }
        public object Attachments { get; set; }
        [Required]
        public string ReasonOfTrainingRequest { get; set; }
    }
}