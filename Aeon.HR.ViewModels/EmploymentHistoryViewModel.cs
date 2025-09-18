using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class EmploymentHistoryViewModel
    {
       public Guid Id { get; set; }
        //public string Date { get; set; }
        //public DateTimeOffset? FromDate { get; set; }
        //public DateTimeOffset? ToDate { get; set; }      
        public DateTimeOffset? Date { get; set; }
        public string NameOfEmployer { get; set; }
      
        public string JobPosition { get; set; }
        public int GlossSalary { get; set; }
      
        public string ReasonsForLeaving { get; set; }
        public Guid? ApplicantId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
