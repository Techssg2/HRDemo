using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public class ReasonOfTrainingRequest : BaseEntity
    {
        [Required]
        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
