using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.PrintFormViewModel
{
    public class TargetPlanPrintFormViewModel
    {
        public string PeriodFromDate { get; set; }
        public string DeptName { get; set; }
        public string DivisionName { get; set; }

    }
    public class TargetPlanDetailPrintFormViewModel
    {
        public TypeTargetPlan Type { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string JsonData { get; set; }
        public string DepartmentName { get; set; }
        public Guid TargetPlanId { get; set; }
        public string GradeCaption { get; set; }
    }

    public class ActualShiftPlanPrintFormViewModel
    {
        public string SAPCode { get; set; }
        public string Actual1 { get; set; }
        public string Actual2 { get; set; }
    }

    public class ShiftCodePrintFormViewModel
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string TypeOfHaftDay { get; set; }
        public ShiftLine Line { get; set; }
        public bool IsHoliday { get; set; }
    }
}
