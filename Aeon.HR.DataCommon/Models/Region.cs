using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
	public class Region: SoftDeleteEntity 
	{
		public Guid Id { get; set; }
		public string RegionName { get; set; }
		public virtual ICollection<Department> Departments { get; set; }
	}
}
