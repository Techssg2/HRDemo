using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class LanguageProficiencyEntryViewModel
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        
        public string LanguageName { get; set; }
       
        public string Spoken { get; set; }
      
        public string Writen { get; set; }
       
        public string Understand { get; set; }
        public Guid ApplicantId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
