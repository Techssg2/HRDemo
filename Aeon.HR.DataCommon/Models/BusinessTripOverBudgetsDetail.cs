using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class BusinessTripOverBudgetsDetail : IEntity
    {
        public BusinessTripOverBudgetsDetail()
        {

        }
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
        public Guid? BtaDetailId { get; set; }
        public int UserGradeValue { get; set; }
        public int OldUserGradeValue { get; set; }
        public Guid? UserGradeId { get; set; }
        public virtual BusinessTripOverBudget BusinessTripOverBudget { get; set; }
        public virtual Department Department { get; set; }
        public int TripGroup { get; set; }
        public bool IsOverBudget { get; set; }
        public decimal MaxBudgetAmount { get; set; }
        public virtual User User { get; set; }
        public string Comments { get; set; }
        public decimal ExtraBudget { get; set; }
        public Guid? PartitionId { get; set; }
        public string PartitionCode { get; set; }
        public string PartitionName { get; set; }
        public string PartitionInfo { get; set; }
        //lamnl add 6/7/2022
        public string FlightDetails { get; set; }
        // luu thon tin dieu kien ve may bay
        public string DepartureSearchId { get; set; }
        public string ReturnSearchId { get; set; }
        // Departure
        public string TitleDepartureFareRule { get; set; }
        public string DetailDepartureFareRule { get; set; }
        // Return
        public string TitleReturnFareRule { get; set; }
        public string DetailReturnFareRule { get; set; }
        public JobGrade UserGrade { get; set; }
    }
}
