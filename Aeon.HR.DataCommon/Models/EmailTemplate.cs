using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class EmailTemplate : Entity
    {
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string TemplatCode { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(10000)]
        public string Body { get; set; }
        [StringLength(200)]
        public string Subject { get; set; }
    }
}
