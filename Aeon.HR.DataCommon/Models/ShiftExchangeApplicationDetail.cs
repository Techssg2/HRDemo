using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class ShiftExchangeApplicationDetail: IEntity
    {

        public Guid? ShiftExchangeApplicationId { get; set; }
        public Guid UserId { get; set; }
        public string SAPCode { get; set; }
        public DateTimeOffset ShiftExchangeDate { get; set; }
        public string CurrentShiftCode { get; set; }  // master data
        public string CurrentShiftName { get; set; }  // master data
        public string NewShiftCode { get; set; } // master data
        public string NewShiftName { get; set; } // master data
        public string ReasonCode { get; set; }  // master data
        public string ReasonName { get; set; } // master data
        public string OtherReason { get; set; }
        //--Phía dưới là danh sách khóa ngoại

        public virtual ShiftExchangeApplication ShiftExchangeApplication { get; set; }
        public virtual User User { get; set; }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get ; set ; }
        public DateTimeOffset Created { get ; set ; }
        public DateTimeOffset Modified { get ; set ; }
        public bool IsERD { get; set; }
    }
}