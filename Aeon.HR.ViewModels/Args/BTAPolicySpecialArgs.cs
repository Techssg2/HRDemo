using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class BTAPolicySpecialArgs
    {
		public Guid Id { get; set; }
		public Guid? UserId { get; set; }
		public string SapCode { get; set; }
		public string FullName { get; set; }
		public Guid? DepartmentId { get; set; }
		public string PositionName { get; set; }
		public Guid? JobGradeId { get; set; }
		public decimal BudgetFrom { get; set; }
		public decimal BudgetTo { get; set; }
		public Guid? PartitionId { get; set; }
		public string PartitionName { get; set; }
		public string PartitionCode { get; set; }
	}
}