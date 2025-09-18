using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class BusinessModel : SoftDeleteEntity
    {
        public BusinessModel()
        {
            Departments = new HashSet<Department>();
            BusinessModelUnitMappings = new HashSet<BusinessModelUnitMapping>();
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<BusinessModelUnitMapping> BusinessModelUnitMappings { get; set; }
    }
}