using System;
using System.Collections.Generic;
using Aeon.Academy.Common.Utils;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingRequestManagement
    {
        public int Count { get; set; }
        public int TotalPending { get; set; }
        public int TotalApproved { get; set; }
        public decimal TotalAmountPendingApproval { get; set; }
        public decimal TotalAmountApproved { get; set; }
        public List<TrainingRequest> Data { get; set; }
    }
}
