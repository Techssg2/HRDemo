using Aeon.HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TargetPlanTesting.ImportData
{
    public class ShiftCodeImportFileDTO
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string TypeOfHaftDay { get; set; }
        public string Line { get; set; }
        public string IsHoliday { get; set; }
        public string IsActive { get; set; }
    }
    public class ImportShiftCodeArg
    {
        public string Module { get; set; }
        public string FileName { get; set; }
    }
        public class ImportShiftCodeError
    {
        public int RowNum { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string TypeOfHaftDay { get; set; }
        public ShiftLine Line { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string ErrorCode { get; set; }
        public List<string> ErrorMessage { get; set; }
    }
    
}
