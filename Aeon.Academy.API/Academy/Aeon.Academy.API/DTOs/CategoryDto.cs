using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
        public bool? IsActivated { get; set; }
    }
}