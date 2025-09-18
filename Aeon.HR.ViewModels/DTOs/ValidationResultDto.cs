using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.HR.ViewModels.DTOs
{
    public class ValidationResultDto
    {
        public List<ValidationResult> ValidationResults { get; set; }
        public bool IsValid { get; set; }
    }
}