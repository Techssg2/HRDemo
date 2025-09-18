using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TargetPlanResultDTO
    {
        public TargetPlanResultDTO()
        {
            Data = new object { };
            AllData = new object { };
            Count = 0;
        }
        public object Data { get; set; }
        public object AllData { get; set; }
        public int Count { get; set; }

    }
}
