using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.HR.ViewModels
{
    public class ReasonViewModelForCABSetting
    {
        [Required]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // key của type // sau khi gửi lên sẽ tìm ra được value của key này
        public Guid JobGradeId { get; set; }
        public string JobGradeCaption { get; set; } 
        public int? JobGradeGrade { get; set; }
        public string JobGradeTitle { get; set; }
    }
}