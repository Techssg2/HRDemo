using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class RequesterDto
    {
        public Guid RequesterId { get; set; }
        [Required]
        public string SapNumber { get; set; }
        [Required]
        public string RequesterName { get; set; }
        public string Affiliate { get; set; }
        [Required]
        public Guid? DepartmentId { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        public Guid? RegionId { get; set; }
        [StringLength(255)]
        public string RegionName { get; set; }
        public string Position { get; set; }   
        [DataType(DataType.DateTime)]
        public DateTime DateOfSubmission { get; set; }
    }
}