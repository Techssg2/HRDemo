using Aeon.HR.BusinessObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Unity;

namespace Aeon.Navigation.Filters
{
    public class ApiAuthFilterAttribute : ActionFilterAttribute
    {
        [Dependency]
        public IAPIContext _ctx;
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var req = actionContext.Request;
            var isValid = _ctx.ValidateContext(req.Headers, req.RequestUri.LocalPath.ToString());
            if (!isValid)
            {
                actionContext.Response = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                };
            }
            base.OnActionExecuting(actionContext);

        }

    }
}