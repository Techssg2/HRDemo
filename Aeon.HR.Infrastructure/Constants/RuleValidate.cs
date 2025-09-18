using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Constants
{
    public class RuleValidate
    {
        public static readonly string[] InProgess = { "Draft", "Cancelled", "Rejected", "Completed", "Requested To Change", "Completed Changing", "Pending" };
        public static readonly string Changing = "Changing/ Cancelling Business Trip";
    }
}
