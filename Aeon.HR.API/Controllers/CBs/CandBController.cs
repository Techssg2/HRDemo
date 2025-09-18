using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Interfaces;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SSG2.API.Controllers.CBs
{
    //[Authorize(Roles = "Administrator")]
    public class CandBController : BaseController
    {
        protected readonly ICBBO _cbBO;
        public CandBController(ILogger logger, ICBBO cbBO) : base(logger)
        {
            _cbBO = cbBO;
        }
    }

}
