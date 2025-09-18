using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class RequestOverBudgetDTO
    {
        public Guid? Id { get; set; }
        public string OverBudgetInfos { get; set; } // json  
        public string Comment { get; set; }
    }
}
