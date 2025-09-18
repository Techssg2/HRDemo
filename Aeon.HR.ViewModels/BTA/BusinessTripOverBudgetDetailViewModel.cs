using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class BusinessTripOverBudgetDetailViewModel
    {
        public Guid Id { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public bool IsForeigner { get; set; }
        public Gender Gender { get; set; }
        public string DepartureCode { get; set; }
        public string DepartureName { get; set; }
        public string DepartureInfo { get; set; }
        public string ArrivalCode { get; set; }
        public string ArrivalName { get; set; }
        public string ArrivalInfo { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? BusinessTripOverBudgetId { get; set; }
        public int UserGradeValue { get; set; }

        public string ReferenceNumber { get; set; }
        public string StatusItem { get; set; }
        public Guid? CreatedById { get; set; }
        public string CreatedByFullName { get; set; }
        public string CreatedBySapCode { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }

        public int TripGroup { get; set; }
        public bool IsOverBudget { get; set; }
        public decimal MaxBudgetAmount { get; set; }
        public decimal ExtraBudget { get; set; }
        public string Comments { get; set; }

        public int GroupMemberCount { get; set; }
        public DepartmentViewModel Department { get; set; }
        public List<FlightDetailViewModel> FlightDetails { get; set; }
        public int JobGrade
        {
            get
            {
                return Department.JobGradeGrade;
            }
        }
        public UserListViewModel User { get; set; }
        public Guid? PartitionId { get; set; }
        public string PartitionCode { get; set; }
        public string PartitionName { get; set; }
        public string PartitionInfo { get; set; }
    }
}
