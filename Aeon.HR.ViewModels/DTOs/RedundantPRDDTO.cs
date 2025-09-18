using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class RedundantPRDDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double PRDRemain { get; set; }
    }
    public class RedundantPRDDataDTO
    {
        public RedundantPRDDataDTO()
        {
            JsonData = new List<RedundantPRDDTO>();
        }
        public List<RedundantPRDDTO> JsonData { get; set; }
    }
}
