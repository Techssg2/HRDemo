using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public class JobGrade : BaseEntity
    {
        [Required]
        public int Grade { get; set; }
        public string Caption { get; set; }
        public string Title { get; set; }
    }
}
