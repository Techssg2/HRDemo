using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
	public class BTAPolicySpecial : SoftDeleteEntity
	{
		public string SapCode { get; set; }
		public string FullName { get; set; }
		public Guid? UserId { get; set; }
		public Guid? DepartmentId { get; set; }
		public string PositionName { get; set; }
		public Guid? JobGradeId { get; set; }
		public decimal BudgetFrom { get; set; }
		public decimal BudgetTo { get; set; }
		public virtual Department Department { get; set; }
		public virtual JobGrade JobGrade { get; set; }
		public virtual User User { get; set; }
		public Guid? PartitionId { get; set; }
		public virtual Partition Partition { get; set; }
	}
}
