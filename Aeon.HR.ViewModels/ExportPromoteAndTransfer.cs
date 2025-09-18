using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ExportPromoteAndTransfer
    {
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string Type { get; set; }
        public string RequestFrom { get; set; }
        public string FullName { get; set; }
        public string SapCode { get; set; }
        public string CurrentPosittion { get; set; }
        public string CurrentJobGrade { get; set; }
        public string CurrentDepartment { get; set; }
        public string CurrentWorkLocation { get; set; }
        //CR222================================================
        public string PersonnelArea { get; set; }
        public string PersonnelAreaText { get; set; }
        public string EmployeeGroup { get; set; }
        public string EmployeeGroupDescription { get; set; }
        public string EmployeeSubgroup { get; set; }
        public string EmployeeSubgroupDescription { get; set; }
        public string PayScaleArea { get; set; }
        //====================================================
        public DateTimeOffset EffectiveDate { get; set; }
        public Double NewSalaryBenefits { get; set; }
        public string NewPosition { get; set; }
        public string NewDeptLine { get; set; }
        public string NewJobGrade { get; set; }
        public string NewWorkLocation { get; set; }
        public string ReportTo { get; set; }
        public string ReasonOfPromotionTransfer { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedByFullName { get; set; }
    }
}
