using Aeon.Academy.API.Utils;
using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Unity;

namespace Aeon.Academy.API.Filters
{
    public class AuthFilterByRoleAttribute : ActionFilterAttribute
    {
        [Dependency]
        public IUserService AccountService { get; set; }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers;
            headers.TryGetLoginName(out string loginName);
            if (!string.IsNullOrEmpty(loginName))
            {
                var role = ApplicationSettings.AdminRole;
                if (role > 0)
                {
                    var user = AccountService.GetUser(loginName);
                    if (user != null && user.Role != role)
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }                
            }

            base.OnActionExecuting(actionContext);
        }
    }
}