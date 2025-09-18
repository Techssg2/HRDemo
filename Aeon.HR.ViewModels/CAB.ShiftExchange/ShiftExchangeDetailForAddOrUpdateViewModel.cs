using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ShiftExchangeDetailForAddOrUpdateViewModel
    {
        public Guid Id { get; set; }
        public Guid? ShiftExchangeApplicationId { get; set; }
        public Guid UserId { get; set; }           // ko đc null 
        public string UserFullName { get; set; }
        public string UserSAPCode { get; set; }
        public string EmployeeCode { get; set; }
        public DateTimeOffset ShiftExchangeDate { get; set; }
        public string CurrentShiftCode { get; set; }  // master data
        public string CurrentShiftName { get; set; }  // master data
        public string NewShiftCode { get; set; } // master data
        public string NewShiftName { get; set; } // master data
        public string ReasonCode { get; set; }  // master data
        public string ReasonName { get; set; } // master data
        public string OtherReason { get; set; }
        public bool IsERD { get; set; }
    }
    public class ExportShiftExchangeDetaiViewModel
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }
        public string UserFullName { get; set; }
        public string UserSAPCode { get; set; }
        public string EmployeeCode { get; set; }
        public DateTimeOffset ShiftExchangeDate { get; set; }
        public string CurrentShiftCode { get; set; }  // master data
        public string CurrentShiftName { get; set; }  // master data
        public string NewShiftCode { get; set; } // master data
        public string NewShiftName { get; set; } // master data
        public string ReasonCode { get; set; }  // master data
        public string ReasonName { get; set; } // master data
        public string OtherReason { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public double QuotaErd { get; set; }
        public bool IsERD { get; set; }
        public DateTimeOffset? CompletedDate { get; set; }
    }
    public class ShiftExchangeDetailSimpleViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }
        public string EmployeeCode { get; set; }
        public DateTimeOffset ShiftExchangeDate { get; set; }
        
    }
}
