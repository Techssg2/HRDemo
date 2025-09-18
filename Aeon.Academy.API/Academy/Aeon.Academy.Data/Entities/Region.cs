using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.Data.Entities
{
    public class Region : BaseEntity
    {
        public string RegionName { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
