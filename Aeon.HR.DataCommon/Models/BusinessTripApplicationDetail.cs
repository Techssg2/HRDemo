using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class BusinessTripApplicationDetail : IEntity
    {
        public BusinessTripApplicationDetail()
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
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public DateTimeOffset? CheckInHotelDate { get; set; }
        public DateTimeOffset? CheckOutHotelDate { get; set; }
        public string FlightNumberCode { get; set; }
        public string FlightNumberName { get; set; }
        public string ComebackFlightNumberCode { get; set; }
        public string ComebackFlightNumberName { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string ComebackAirlineCode { get; set; }
        public string ComebackAirlineName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? BusinessTripApplicationId { get; set; }
        public Guid? UserGradeId { get; set; }
        public int UserGradeValue { get; set; }
        public int OldUserGradeValue { get; set; }
        public string UserGradeTitle { get; set; }
        public virtual BusinessTripApplication BusinessTripApplication { get; set; }
        public virtual Department Department { get; set; }
        public virtual JobGrade UserGrade { get; set; }
        public string IDCard { get; set; }
        public string Passport { get; set; }
        public int TripGroup { get; set; }
        public bool IsOverBudget { get; set; }
        public bool StayHotel { get; set; }
        public decimal MaxBudgetAmount { get; set; }
        public virtual User User { get; set; }
        public string Comments { get; set; }
        public bool RememberInformation { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public DateTimeOffset? PassportDateOfIssue { get; set; }
        public DateTimeOffset? PassportExpiryDate { get; set; }
        public Guid? PartitionId { get; set; }
        public string PartitionCode { get; set; }
        public string PartitionName { get; set; }
        public string PartitionInfo { get; set; }
        public string ReferenceNumberOverBudget { get; set; }
        public Guid? BusinessTripOverBudgetId { get; set; }
        public bool CheckBookingCompleted { get; set; } //lamnl check booking completed
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CountryInfo { get; set; }
        public string Memberships { get; set; }
        public bool IsBookingContact { get; set; }
        public bool IsCommitBooking { get; set; }
        public string BookingNumber { get; set; }
        public string BookingCode { get; set; }
        public bool? HasBudget { get; set; }
    }
}
