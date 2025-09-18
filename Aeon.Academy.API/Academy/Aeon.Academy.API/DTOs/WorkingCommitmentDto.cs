using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class WorkingCommitmentDto
    {
        [Required]
        public bool WorkingCommitment { get; set; }        
        [DataType(DataType.DateTime)]
        public DateTime? From { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? To { get; set; }
        [StringLength(255)]
        public string SponsorshipContractNumber { get; set; }
        public decimal CompensateAmount { get; set; }
    }
}