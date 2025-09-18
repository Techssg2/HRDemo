using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class BTAPolicyArgs
    {
        public Guid Id { get; set; }
        public Guid? JobGradeId { get; set; }
        public decimal BudgetFrom { get; set; }
        public decimal BudgetTo { get; set; }
        public bool IsStore { get; set; }
        public Guid? PartitionId { get; set; }
        public string PartitionName { get; set; }
        public string PartitionCode { get; set; }
    }
}