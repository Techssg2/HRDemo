using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class MissingTimeClockDetailViewModel
    {
        public Guid? Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string TypeActualTime { get; set; }
        public string ShiftCode { get; set; }
        public double ActualTime { get; set; }
        public string ReasonCode { get; set; }
        public string Others { get; set; }
        public string Previous { get; set; }
        public DateTimeOffset Created { get; set; } = DateTime.Now;
        public DateTimeOffset Modified { get; set; } = DateTime.Now;
        public Guid? MissingTimeClockId { get; set; }
    }
}
