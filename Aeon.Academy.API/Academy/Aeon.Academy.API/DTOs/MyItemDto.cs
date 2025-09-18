using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.DTOs
{
    public class MyItemDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string ItemType { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByFullName { get; set; }
    }
}