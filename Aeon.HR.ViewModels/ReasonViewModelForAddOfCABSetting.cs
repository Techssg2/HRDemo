using System.ComponentModel.DataAnnotations;

namespace Aeon.HR.ViewModels
{
    public class ReasonViewModelForAddOfCABSetting
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; } // key của type // sau khi gửi lên sẽ tìm ra được value của key này
    }
}