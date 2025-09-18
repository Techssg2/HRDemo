using Aeon.Academy.API.Core;
using Aeon.Academy.API.DTOs;
using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Aeon.Academy.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IUserService accountService;
        private readonly ITrainingInvitationService trainingInvitationService;
        public AccountController(IUserService accountService, ITrainingInvitationService trainingInvitationService)
        {
            this.accountService = accountService;
            this.trainingInvitationService = trainingInvitationService;
        }
        [HttpPost]
        public IHttpActionResult Login(LoginDto login)
        {
            var user = accountService.GetUser(login.LoginName);
            if (user == null) return NotFound();
            var token = StringCipher.Encrypt(user.LoginName, ApplicationSettings.ApiSecret);
            return Ok(new LoginResultDto { Token = token });
        }
        [HttpGet]
        public IHttpActionResult IsCheckerAcademy()
        {
            var isAuthorized = trainingInvitationService.CheckAcademyUser(CurrentUser.Id);
            return Ok(new { isAuthorized });
        }
        [HttpGet]
        public IHttpActionResult IsHODAcademy(Guid requestId)
        {
            var isAuthorized = accountService.IsHODAcademy(requestId, CurrentUser);
            return Ok(new { isAuthorized = isAuthorized });
        }
        [HttpGet]
        public IHttpActionResult Healthcheck()
        {
            return Ok();
        }
    }
}
