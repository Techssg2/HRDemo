using Aeon.HR.Data.Models;
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
	public class BTAUserFlightTicketViewModel
	{
		public string SAPCode { get; set; }
		public Guid BusinessTripApplicationDetailId { get; set; }
		public BusinessTripApplication BusinessTripApplication { get; set; }
		public List<FlightDetailViewModel> FlightTicketInfo { get; set; }
		public ChangeCancelBusinessTripDetailViewModel ChangeCancelInfo { get; set; } 
	}
}
