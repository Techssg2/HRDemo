using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class WorkingTimeRecruitment: SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }       
        //Link
    }
}
