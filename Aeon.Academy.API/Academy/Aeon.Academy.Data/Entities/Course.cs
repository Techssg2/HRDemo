using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public class Course : BaseTrackingEntity
    {
        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [StringLength(50)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        [Required]
        public string Type { get; set; }

        [Required]
        public virtual Guid CategoryId { get; set; }

        [StringLength(255)]
        public string CategoryName { get; set; }

        [StringLength(255)]
        public string ServiceProvider { get; set; }
        [StringLength(10)]
        public string ServiceProviderCode { get; set; }

        public string Description { get; set; }

        [Required]
        public int Duration { get; set; }

        [StringLength(255)]
        public string ImageName { get; set; }

        public string Image { get; set; }

        [Required]
        public bool IsActivated { get; set; }

        public virtual Category Category { get; set; }
    }
}
