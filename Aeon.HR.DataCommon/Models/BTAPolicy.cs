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
	public class BTAPolicy : SoftDeleteEntity
	{
		public Guid? JobGradeId { get; set; }
		public decimal BudgetFrom { get; set; }
		public decimal BudgetTo { get; set; }
		public bool IsStore { get; set; }
		public virtual JobGrade JobGrade { get; set; }
		public Guid? PartitionId { get; set; }
		public string PartitionCode { get; set; }
		public string PartitionName { get; set; }
		public virtual Partition Partition { get; set; }
	}
}
