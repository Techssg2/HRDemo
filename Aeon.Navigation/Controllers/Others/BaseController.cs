using Aeon.Navigation.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aeon.Navigation.Controllers.Others
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
