using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public class Category : BaseTrackingEntity
    {
        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
        public bool IsActivated { get; set; }

        public virtual ICollection<Course> Courses { get; set; }

    }
}
