using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ObjectToCheckValidLeaveManagemetDTO
    {
        public ObjectToCheckValidLeaveManagemetDTO()
        {
            MyLeaveBalances = new List<LeaveBalanceResponceSAPViewModel>();
            LeaveDetails = new List<LeaveApplicantDetailDTO>();
        }
        public Guid? Id { get; set; }
        public string UserSapCode { get; set; }
        public List<LeaveApplicantDetailDTO> LeaveDetails { get; set; }
        public List<LeaveBalanceResponceSAPViewModel> MyLeaveBalances { get; set; }
    }
    public class LeaveApplicantDetailDTO
    {
        
        public Guid Id { get; set; }
        public bool IsApproved { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveName { get; set; }
        public double Quantity { get; set; }
        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ToDate { get; set; }
        public string Reason { get; set; }
        public string LeaveApplicationReferenceNumber { get; set; }
        public string LeaveApplicationStatus { get; set; }
        public string SAPCode { get; set; }
    }

    public class ExportLeaveApplicationViewModel
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveName { get; set; }
        public double Quantity { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ToDate { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public DateTimeOffset CreateDate { get; set; }

    }
    public class SimpleLeaveApplicantDetail
    {
        public DateTimeOffset Date { get; set; }
        public double Quantity { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveApplicationReferenceNumber { get; set; }
    }

}
