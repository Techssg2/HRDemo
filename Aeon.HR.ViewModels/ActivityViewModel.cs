using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ActivityViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }      
        public string Responsibilty { get; set; }
        public int ApproximateDays { get; set; }
        public Guid? ApplicantId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
