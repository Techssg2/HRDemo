using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
	public class MaintainPromoteAndTranferPrint : SoftDeleteEntity 
	{
		public string RemovingValue { get; set; }
	}
}
