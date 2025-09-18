using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Aeon.HR.API.Controllers.Settings
{
    public class TimeController : SettingController
    {
        public TimeController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> GetTimeConfigurations([FromBody] MasterdataArgs args)
        {
            try
            {
                var result = await _settingBO.GetTimeSettings(args);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTimeSettings" + ex.Message);
                return BadRequest("Error System");
            }
        }
        public async Task<IHttpActionResult> UpdateConfiguration([FromBody] ConfigurationViewModel args)
        {
            try
            {
                var result = await _settingBO.UpdateConfiguration(args);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateConfiguration" + ex.Message);
                return BadRequest("Error System");
            }
        }
    }
}