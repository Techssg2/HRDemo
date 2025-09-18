using Aeon.HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ShiftCodeAgrs
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string TypeOfHaftDay { get; set; }
        public ShiftLine Line { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsActive { get; set; }
        public string FileName { get; set; }
        public string Module { get; set; }
    }
}
