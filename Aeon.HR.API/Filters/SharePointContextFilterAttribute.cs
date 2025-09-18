using System;
using System.Configuration;
using System.Web.Mvc;

namespace Aeon.HR.API
{
    /// <summary>
    /// SharePoint action filter attribute.
    /// </summary>
    public class SharePointContextFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(filterContext.HttpContext, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    filterContext.Result = new RedirectResult(redirectUrl.AbsoluteUri);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    var redirect = $"?SPHostUrl={ConfigurationManager.AppSettings["siteUrl"]}";
                    filterContext.Result = new RedirectResult(redirect);
                    //filterContext.Result = new ViewResult { ViewName = "Error" };
                    break;
            }
        }
    }
}
