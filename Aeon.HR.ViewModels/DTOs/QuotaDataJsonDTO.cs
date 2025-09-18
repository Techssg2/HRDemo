using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class QuotaDataJsonDTO
    {
        public QuotaDataJsonDTO()
        {
            JsonData = new List<QuotaDataJsonDetailDTO>();
        }
        public List<QuotaDataJsonDetailDTO> JsonData { get; set; }
    }
    public class QuotaDataJsonDetailDTO
    {
        public int Year { get; set; }
        public double ALRemain { get; set; }
        public double ERDRemain { get; set; }
        public double DOFLRemain { get; set; }

    }
}
