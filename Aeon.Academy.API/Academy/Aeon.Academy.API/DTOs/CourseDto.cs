using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [StringLength(255)]
        public string CategoryName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Type { get; set; }

        public string ServiceProvider { get; set; }
        public string ServiceProviderCode { get; set; }

        public string Description { get; set; }

        [Required]
        public int Duration { get; set; }

        public string ImageName { get; set; }

        public string Image { get; set; }

        [Required]
        public bool IsActivated { get; set; }

        public object Documents { get; set; }
    }
}