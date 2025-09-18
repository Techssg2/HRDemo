using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TargetPlanViewModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid? DeptId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public Guid? DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserSAPCode { get; set; }
        public string UserFullName { get; set; }
        public Guid PeriodId { get; set; }
        public string PeriodName { get; set; }
        public DateTimeOffset PeriodFromDate { get; set; }
        public DateTimeOffset PeriodToDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public IEnumerable<TargetPlanDetailViewModel> TargetPlanDetails { get; set; }

        //Upgrade
        public string CreatedByFullName { get; set; }
    }
}
