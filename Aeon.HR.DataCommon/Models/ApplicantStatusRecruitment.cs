using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class ApplicantStatusRecruitment: SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Arrangement { get; set; }        
    }
}