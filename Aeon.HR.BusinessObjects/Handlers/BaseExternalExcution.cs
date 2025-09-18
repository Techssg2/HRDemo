using Aeon.HR.BusinessObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class BaseExternalExcution
    {
        protected readonly ITrackingBO _tracking;
        public BaseExternalExcution(ITrackingBO tracking)
        {
            _tracking = tracking;
        }
    }
}
