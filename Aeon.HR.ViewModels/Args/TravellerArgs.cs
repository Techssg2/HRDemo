using Aeon.HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
	public class TravellerArgs
	{
		public Guid Id { get; set; }
		public string bookingNumber { get; set; }

		public List<BusinessTripApplicationDetail> btaDetailItem { get; set; }
		public string airlines { get; set; }
	}
}
