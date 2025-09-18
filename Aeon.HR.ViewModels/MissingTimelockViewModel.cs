using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class MissingTimelockViewModel: CBUserInfoViewModel
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string ReferenceNumber { get; set; }
        public string ListReason { get; set; }
        public string Status { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
    }
    public class MissingTimeClockExportDetailViewModel
    {
        public DateTime Date { get; set; }
        public string ShiftCode { get; set; }
        //public string ActualTimeIn { get; set; }
        //public string ActualTimeOut { get; set; }
        public string ActualTime { get; set; }
        public string Reason { get; set; }
        public string ReasonName { get; set; }
        public string Others { get; set; }
        public TypeActualTime TypeActualTime { get; set; }
    }
    public class ExportMissingTimeClockViewModel
    {
        public string SapCode { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string ShiftCode { get; set; }
        public string ActualTime { get; set; }
        public string Reason { get; set; }
        public string ReasonName { get; set; }
        public string Other { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string FullName{ get; set; }
        public string TypeActualTime { get; set; }
        public string Documents { get; set; }
    }
}