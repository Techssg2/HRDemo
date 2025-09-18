using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Aeon.HR.API.Filters
{
    public class SharePointApiControllerContextFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            Uri redirectUrl;
            switch (SharePointApiControllerContextProvider.CheckContextStatus(actionContext.ControllerContext, out redirectUrl))
            {
                case ContextStatus.Ok:
                    return;
                case ContextStatus.NotOk:
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Context couldn't be created: access denied");
                    break;
            }
        }
    }
}