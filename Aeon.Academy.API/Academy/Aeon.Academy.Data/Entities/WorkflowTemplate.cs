using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public class WorkflowTemplate : BaseEntity
    {
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        [StringLength(255)]
        public string WorkflowName { get; set; }
        [StringLength(255)]
        public string ItemType { get; set; }
        public int Order { get; set; }

        public bool IsActivated { get; set; }
        public bool AllowMultipleFlow { get; set; }
        public string StartWorkflowButton { get; set; }
        public string WorkflowDataStr { get; set; }
    }
}
