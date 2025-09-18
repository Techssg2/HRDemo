using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
	public class BookingContract : SoftDeleteEntity 
	{
		public string FullName { get; set; }
		public string EmailBookingContract { get; set; }
		public string PhoneNumber { get; set; }
		public string FirstName { get; set; }
		public string SurName { get; set; }
	}
}
