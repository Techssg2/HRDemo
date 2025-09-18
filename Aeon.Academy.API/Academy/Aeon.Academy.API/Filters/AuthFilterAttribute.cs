using Aeon.Academy.API.Utils;
using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Aeon.Academy.API.Filters
{
    public class AuthFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers;
            var auth = headers.TryGetLoginName(out string loginName) && !string.IsNullOrEmpty(loginName);
            if (!auth)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            base.OnActionExecuting(actionContext);
        }
    }
}