using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class ItemWorkflowLog : Entity
    {
        public Guid ItemId { get; set; }
        [StringLength(100)]
        public string Status { get; set; }
        [StringLength(255)]
        public string Message { get; set; }
    }
}
