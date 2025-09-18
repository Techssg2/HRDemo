using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingRequestViewItemDto
    {
        public Guid ItemId { get; set; }
        public string ItemType { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string Status { get; set; }
        public string RequestorFullName { get; set; }
        public string CreatedByFullName { get; set; }
        public string RequestedDepartmentName { get; set; }
        public string RequestedDepartmentCode { get; set; }
        public string RegionName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}