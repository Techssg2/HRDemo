using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ApplicantFromMassViewModel
    {
        public ApplicantFromMassViewModel()
        {
            Items = new List<ApplicantFromMassDetailViewModel>();
        }
        public int Count { get; set; }
        public string ErrorMessage { get; set; }
        public List<ApplicantFromMassDetailViewModel> Items { get; set; }
    }
    public class ApplicantFromMassDetailViewModel
    {
        public Guid Id { get; set; }
        public string ApplicantNumber { get; set; }
        public string ApplicantStatus { get; set; }
        public string ApplicantType { get; set; }
        public DateTimeOffset? Created { get; set; }
        public string CreatedBy { get; set; }
        public string Criteria1 { get; set; }
        public string Criteria2 { get; set; }
        public string Criteria3 { get; set; }
        public string Criteria4 { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ExamScore { get; set; }
        public string Gender { get; set; }
        public string GenderTitle { get; set; }
        public string IdCard12Number { get; set; }
        public string IdCardNumber { get; set; }
        public string InterviewNotes { get; set; }
        public string InterviewResult { get; set; }
        public string InterviewScore { get; set; }
        public string Interviewer { get; set; }
        public bool IsOnPension { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Position1CategoryName { get; set; }
        public string Position1Name { get; set; }
        public string Position2CategoryName { get; set; }
        public string Position2Name { get; set; }
        public string Position3CategoryName { get; set; }
        public string Position3Name { get; set; }
        public float? ProfileScore { get; set; }
        public DateTimeOffset? ReceivedRunningNumberTime { get; set; }
        public int? RunningNumber { get; set; }
        public string Store { get; set; }
    }
}
