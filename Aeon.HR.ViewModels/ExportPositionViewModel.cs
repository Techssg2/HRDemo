using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ExportPosition
    {
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string Position { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public int Applicants { get; set; }
        public int Hired { get; set; }
        public int Required { get; set; }
        public string Assignee { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string RequestToHireNumber { get; set; }
    }

    public class PositionExportViewModel
    {
        public string PositionName { get; set; }
        public Guid DeptDivisionId { get; set; }
        public string DeptDivisionName { get; set; }
        public string DeptDivisionCode { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public int ExpiredDay { get; set; }
        public bool HasBudget { get; set; }
        public int Quantity { get; set; }
        public Guid AssignToId { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid? ApplicantId { get; set; }
        public PositionStatus Status { get; set; }
        public int ApplicantsCount { get; set; }
        public int HiredApplicantsCount { get; set; }
        public string AssignToFullName { get; set; }
        public string DeptDivisionJobGradeCaption { get; set; }
        public Guid DeptDivisionJobGradeId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public string StatusName { get; set; }
        public string RequestToHireNumber { get; set; }
    }
}
