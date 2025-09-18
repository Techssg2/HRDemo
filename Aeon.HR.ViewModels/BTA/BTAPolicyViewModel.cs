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
    public class BTAPolicyViewModel
    {
        public Guid Id { get; set; }

        public decimal BudgetFrom { get; set; }
        public decimal BudgetTo { get; set; }
        public bool IsStore { get; set; }
        public JobGradeViewModel JobGrade { get; set; }
        public Guid? JobGradeId { get; set; }
        public int JobGradeGrade { get; set; }
        public string JobGradeCaption { get; set; }
        public string JobGradeTitle { get; set; }
        public Guid? PartitionId { get; set; }
        public string PartitionName { get; set; }
        public string PartitionCode { get; set; }
    }
}
