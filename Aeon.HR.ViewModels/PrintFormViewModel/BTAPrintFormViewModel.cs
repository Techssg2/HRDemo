using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.PrintFormViewModel
{
    public class BTAPrintFormViewModel
    {
        public BTAPrintFormViewModel()
        {
            Details = new List<BTADetailViewModel>();
            ChangeDetails = new List<ChangeCancelBTADetailViewModel>();
        }
        public List<BTADetailViewModel> Details { get; set; }
        public BTAHeadViewModel Head { get; set; }
        public List<ChangeCancelBTADetailViewModel> ChangeDetails { get; set; }
    }
    public class BTADetailViewModel
    {
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public Gender Gender { get; set; }
        public string DepartureName { get; set; }
        public string ArrivalName { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string HotelName { get; set; }
        public string FlightNumberName { get; set; }
        public string Status { get; set; }
        public string ComebackFlightNumberName { get; set; }
        public string PartitionName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CountryInfo { get; set; }

    }
    public class BTAHeadViewModel
    {
        public string AdminDepartmentRemark { get; set; }
        public string Applicant { get; set; }
        public string GeneralManager { get; set; }
        public string SGeneralManager { get; set; }
        public string GeneralDirector { get; set; }
        public DateTimeOffset? GeneralDirectorSignedDate { get; set; }
        public DateTimeOffset? AdminDepartmentRemarkSignedDate { get; set; }
        public DateTimeOffset? ApplicantSignedDate { get; set; }
        public DateTimeOffset? GeneralManagerSignedDate { get; set; }
        public DateTimeOffset? SGeneralManagerSignedDate { get; set; }
        public string RequestorNote { get; set; }
        public string DeptLine { get; set; }
        public string ReferenceNumber { get; set; }
    }
    public class ChangeCancelBTADetailViewModel
    {
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string DestinationName { get; set; }
        public string Reason { get; set; }
        public string GeneralManager { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public DateTimeOffset? NewFromDate { get; set; }
        public DateTimeOffset? NewToDate { get; set; }
    }

    public class CommonBTAViewModel
    {
        public string RequestorNote { get; set; }
        public string DeptLine { get; set; }
        public string AdminDepartmentRemarkSignedDate { get; set; }
        public string ApplicantSignedDate { get; set; }
        public string SGeneralManagerSignedDate { get; set; }
        public string ReferenceNumber { get; set; }
    }
    //lamnl
    public class BTAOverBudgetDetailViewModel
    {
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public Gender Gender { get; set; }
        public string DepartureName { get; set; }
        public string ArrivalName { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string Status { get; set; }

    }

}
