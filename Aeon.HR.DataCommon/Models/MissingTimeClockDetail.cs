using Aeon.HR.Infrastructure.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class MissingTimeClockDetail: IEntity
    {
        public DateTimeOffset Date { get; set; }       
        public string TypeActualTime { get; set; }
        public string ShiftCode { get; set; }
        public DateTimeOffset ActualTime { get; set; }
        public string Reason { get; set; }
        public string ReasonName { get; set; }
        public string Others { get; set; }
        public string Previous { get; set; }
        [JsonIgnore]
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; } = DateTime.Now;
        public DateTimeOffset Modified { get; set; } = DateTime.Now;
        public Guid? MissingTimeClockId { get; set; }
        public virtual MissingTimeClock MissingTimeClock { get; set; }
    }
}
