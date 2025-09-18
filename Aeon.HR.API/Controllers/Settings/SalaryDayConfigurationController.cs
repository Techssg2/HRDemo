using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Infrastructure.Enums;

namespace SSG2.API.Controllers.Settings
{
    public class SalaryDayConfigurationController : SettingController
    {
        public SalaryDayConfigurationController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<IHttpActionResult> GetSalaryDayConfigurations([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _settingBO.GetSalaryDayConfigurations(arg);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetSalaryDayConfigurations " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveSalaryDayConfiguration([FromBody] SalaryDayConfigurationArg arg)
        {
            try
            {
                var res = await _settingBO.SaveSalaryDayConfiguration(arg);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SalaryDayConfigurationArg " + ex.Message);
                return BadRequest("Error System");
            }
        }
    }
}
