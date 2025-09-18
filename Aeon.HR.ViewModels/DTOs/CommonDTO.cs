using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class CommonDTO
    {
        public CommonDTO()
        {
            Limit = -1;
        }
        public Guid Id { get; set; }
        public string BtaDetails { get; set; }
        public string ReferenceNumber { get; set; }
        public int Status { get; set; }
        public string Url { get; set; }
        public int Limit { get; set; }
        public string ReferenceNumberRE { get; set; }

    }
}
