using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.Academy.Data.Entities
{
    public class ReferenceNumber : BaseTrackingEntity
    {
        [Index]
        [StringLength(255)]
        public string ModuleType { get; set; }
        public int CurrentNumber { get; set; }
        public bool IsNewYearReset { get; set; }    //nếu = true thì field CurrentNumber sẽ trả về 1 
        public string Formula { get; set; }
        public int CurrentYear { get; set; }
    }
}
