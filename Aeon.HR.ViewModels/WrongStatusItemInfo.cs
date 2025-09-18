using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
	public class MaintenantItemInfo : StatusItemInfo
	{
		public string ItemReferenceNumber { get; set; }

	}
	public class StatusItemInfo
	{
		public Guid ItemId { get; set; }
		public string Status { get; set; }

	}
}
