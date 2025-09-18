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
    public class AuthFilterByUserTypeAttribute : ActionFilterAttribute
    {
        [Dependency]
        public IUserService AccountService { get; set; }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers;
            headers.TryGetLoginName(out string loginName);
            if (!string.IsNullOrEmpty(loginName))
            {
                var role = ApplicationSettings.UserType;
                if (role == 0 || role == 1)// 0 is AD, 1 is Member
                {
                    var user = AccountService.GetUser(loginName);
                    if (user != null && user.Type != role)
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                }                
            }

            base.OnActionExecuting(actionContext);
        }
    }
}