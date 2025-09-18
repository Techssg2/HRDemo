using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class OpenPositionViewModel
    {
        public Guid Id { get; set; }
        public string PositionName { get; set; }
        public string DeptDivisionName { get; set; }
        public string DeptDivisionCode { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string DeptDivisionJobGradeTitle { get; set; } //===== CR11.2 =====
        public string DeptDivisionJobGradeCaption { get; set; }
        public Guid DeptDivisionJobGradeId { get; set; }
        public Guid DeptDivisionId { get; set; }
        public int DeptDivisionJobGradeGrade { get; set; }
        public int PositionGrade { get; set; }
    }
}
