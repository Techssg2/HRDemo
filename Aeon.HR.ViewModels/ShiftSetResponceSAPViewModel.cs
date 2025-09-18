using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ShiftSetResponceSAPViewModel
    {
        public string EmployeeCode { get; set; }
        public string Date { get; set; }
        public string Year { get; set; }
        public string Period { get; set; }
        public string Shift1 { get; set; }
        public string Shift2 { get; set; }
        public string Shift3 { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string ExchangeDT { get
            {
                return Date;
            }
        }
    }
}
