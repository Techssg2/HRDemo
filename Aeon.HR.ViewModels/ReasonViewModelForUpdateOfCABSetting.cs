using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.HR.ViewModels
{
    public class ReasonViewModelForUpdateOfCABSetting
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
    }
}