using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ConfigurationViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
