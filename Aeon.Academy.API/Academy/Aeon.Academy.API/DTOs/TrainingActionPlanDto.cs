using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aeon.Academy.Common.Consts;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingActionPlanDto
    {
        public Guid Id { get; set; }

        public Guid TrainingReportId { get; set; }

        [Required]
        public string ActionPlanCode { get; set; }

        public bool Quarter1 { get; set; }
        public bool Quarter2 { get; set; }
        public bool Quarter3 { get; set; }
        public bool Quarter4 { get; set; }
    }
}