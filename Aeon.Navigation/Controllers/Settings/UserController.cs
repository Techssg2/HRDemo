using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Infrastructure.Enums;
using System.Linq;
using System.Collections.Generic;
namespace Aeon.Navigation.Controllers.Settings
{
    public class UserController : SettingController
    {
        public UserController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllUsers()
        {
            try
            {
                var res = await _settingBO.GetAllUsers();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListUsers", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<ResultDTO> GetCurrentUserV2()
        {
            try
            {
                var res = await _settingBO.GetCurrentUserV2();
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetCurrentUser", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetUsers([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _settingBO.GetListUsers(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListUsers", ex.Message);
                return BadRequest("Error System");
            }
        }
    }
}
