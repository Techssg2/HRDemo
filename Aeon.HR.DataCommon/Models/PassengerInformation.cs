using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class PassengerInformation : IEntity
    {
        public PassengerInformation()
        {

        }

        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SAPCode { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string IDCard { get; set; }
        public string Passport { get; set; }
        public Gender Gender { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public DateTimeOffset? PassportDateOfIssue { get; set; }
        public DateTimeOffset? PassportExpiryDate { get; set; }
        public string CountryCode { get; set; }
        public string Memberships { get; set; }
        public bool? HasBudget { get; set; }

    }
}