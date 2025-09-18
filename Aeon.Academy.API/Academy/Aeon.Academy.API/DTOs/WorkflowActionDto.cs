using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.DTOs
{
    public class WorkflowActionDto
    {
        public string Comment { get; set; }
        public Guid RequestId { get; set; }
    }
}