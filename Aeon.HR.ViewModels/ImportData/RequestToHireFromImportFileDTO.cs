using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TargetPlanTesting.ImportData
{
    public class RequestToHireFromImportFileDTO
    {
        public RequestToHireFromImportFileDTO()
        {
            Data = new List<RequestToHireFromImportDetailDTO>();
        }
        public List<RequestToHireFromImportDetailDTO> Data { get; set; }

    }
    public class RequestToHireFromImportDetailDTO
    {
        public string ID { get; set; }
        public string PositionCode { get; set; }
        public string TypeOfNeed { get; set; }
        public string CheckBudget { get; set; }
        public string Quantity { get; set; }
        public string DepartmentCode { get; set; }
        public string ExpiredDay { get; set; }
        public string ReplacementForUser { get; set; }
        public string Reason { get; set; }
        public string CostCenterCode { get; set; }
        public string BusinessUnit { get; set; } // Personel Subarea
        public string HQ_Operation { get; set; }
        public string WorkingAddressCode { get; set; }
        public string StartingDate { get; set; }
        public string WorkingTime { get; set; }
        public string ContractType { get; set; }
        public string WorkingHourPerWeek { get; set; }
        public string WagePerHour { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AssignTo { get; set; }
        public string DepartmentName { get; set; } // For New Position
        public string Category { get; set; }
        public string WorkingLocation { get; set; } // (From Mass)
        public string Remark { get; set; }
        public string DepartmentSAPCode { get; set; } // Sapcode of Department
        public string Preventive1 { get; set; } // Du phong 1
        public string Preventive2 { get; set; } // Du phong 2
        public string Preventive3 { get; set; } // Du phong 3
        public string Preventive4 { get; set; } // Du phong 4
        public string Preventive5 { get; set; } // Du phong 5
    }

    public class ImportRequestToHireError
    {
        public int RowNum { get; set; }
        public string ReferenceId { get; set; }
        public Guid? RequestToHireId { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string ErrorCode { get; set; }
        public List<string> ErrorMessage { get; set; }
    }
}
