using Aeon.HR.API.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SSG2.API.Controllers.Others
{
    [ApiAuthFilter]
    public class BaseController : ApiController
    { 
        protected readonly ILogger _logger;
        public BaseController(ILogger logger)
        {
            _logger = logger;
        }
    }
}
