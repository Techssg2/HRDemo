using Aeon.Academy.API.Filters;
using Aeon.Academy.API.Utils;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unity;

namespace Aeon.Academy.API.Core
{
    [CustomExceptionFilter]
    public abstract class BaseApiController : ApiController
    {
        [Dependency]
        public IUserService AccountService { get; set; }
        private User currentUser = null;
        protected User CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    var headers = Request.Headers;
                    var auth = headers.TryGetLoginName(out string loginName) && !string.IsNullOrEmpty(loginName);
                    if (!auth) throw new UnauthorizedAccessException();
                    if (!string.IsNullOrEmpty(loginName) && loginName.Equals("daiso")) loginName = "huong.du";
                    if (!string.IsNullOrEmpty(loginName) && loginName.Equals("vy.nguyentranhoa")) loginName = "vy.nguyentranhoai";
                    currentUser = AccountService.GetUser(loginName);
                }

                return currentUser;
            }
        }
    }

    [AuthFilter]
    public abstract class BaseAuthApiController : BaseApiController
    {
    }
}