using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class BookingContractViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string EmailBookingContract { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
    }
}
