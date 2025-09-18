using System;
using System.Collections.Generic;
using System.Text;

namespace TargetPlanTesting.ImportData
{
    public class TargetPlanFromImportFileDTO
    {
        public TargetPlanFromImportFileDTO()
        {
            Data = new List<TargetPlanFromImportDetailDTO>();
        }
        public DateTimeOffset? PeriodFromDate { get; set; }
        public DateTimeOffset? PeriodToDate { get; set; }
        public List<TargetPlanFromImportDetailDTO> Data { get; set; }

    }
    public class TargetPlanFromImportDetailDTO
    {
        public TargetPlanFromImportDetailDTO()
        {
            Targets = new List<TargetPlanFromImportDetailItemDTO>();
        }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Grade { get; set; }
        public string Target { get; set; }
        public List<TargetPlanFromImportDetailItemDTO> Targets { get; set; }
    }
    public class TargetPlanFromImportDetailItemDTO
    {
        public string date { get; set; }
        public string value { get; set; }
    }
}
