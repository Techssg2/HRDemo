using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class ShiftCode : SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string TypeOfHaftDay { get; set; }
        public ShiftLine Line { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsActive { get; set; }
    }
    public enum ShiftLine
    {
        FULLDAY = 1,
        HAFTDAY = 2,
        ACTUAL1 = 3,
        ACTUAL2 = 4
    }
}
