using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class AppreciationListRecruitment: SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}