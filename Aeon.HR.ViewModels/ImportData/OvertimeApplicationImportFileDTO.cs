using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.ImportData
{
    public class OvertimeApplicationImportFileDTO
    {
        public OvertimeApplicationImportFileDTO()
        {
            Data = new List<OvertimeApplicationImportDetailDTO>();
        }
        public List<OvertimeApplicationImportDetailDTO> Data;
    }
    public class OvertimeApplicationImportDetailDTO
    {
        public string LineOfExcel { get; set; }
        public string SAPCode { get; set; }
        public DateTimeOffset? DateOfOT { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        //public bool DateOfInLieu { get; set; }
        public string DateOfInLieu { get; set; }
        public string IsNoOT { get; set; }
    }

    public class OvertimeApplicationImportActualFileDTO
    {
        public OvertimeApplicationImportActualFileDTO()
        {
            Data = new List<OvertimeApplicationImportActualDetailDTO>();
        }
        public List<OvertimeApplicationImportActualDetailDTO> Data;
    }
    public class OvertimeApplicationImportActualDetailDTO
    {
        public string LineOfExcel { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public DateTimeOffset? DateOfOT { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ActualFrom { get; set; }
        public string ActualTo { get; set; }
        public string DateOfInLieu { get; set; }
        public string IsNoOT { get; set; }
    }
}
