using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.DTOs
{
    public class ReasonDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Value { get; set; }
    }
}