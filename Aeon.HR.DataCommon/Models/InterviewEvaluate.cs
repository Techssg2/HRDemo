using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class InterviewEvaluate : Entity
    {
        public string Interviewer { get; set; }
        public float? Score { get; set; }
        public string Notes { get; set; }
        public float? Criteria1 { get; set; }
        public float? Criteria2 { get; set; }
        public float? Criteria3 { get; set; }
        public float? Criteria4 { get; set; }
        public Guid ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }
    }
}
