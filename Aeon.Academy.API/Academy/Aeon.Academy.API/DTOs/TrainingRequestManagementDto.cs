using System;
using System.Collections.Generic;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingRequestManagementDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string RequesterName { get; set; }
        public string DepartmentName { get; set; }
        public string TypeOfTraining { get; set; }        
        public decimal? TrainingFee { get; set; }
    }
}