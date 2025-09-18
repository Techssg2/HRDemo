using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ReferencyNumberViewModel
    {
        public Guid Id { get; set; }
        public string ModuleType { get; set; }
        public int CurrentNumber { get; set; }
        public bool IsNewYearReset { get; set; }    //nếu = true thì field CurrentNumber sẽ trả về 1 
        public string Formula { get; set; }
        public int CurrentYear { get; set; }
    }
}