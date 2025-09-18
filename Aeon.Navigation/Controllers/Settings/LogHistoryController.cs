using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace Aeon.HR.API.Controllers.Settings
{
    public class LogHistoryController : SettingController
    {
        public LogHistoryController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<IHttpActionResult> GetLogHistory(QueryArgs args)
        {
            try
            {
                var result = await _settingBO.GetLogHistory(args);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetReasons", ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveLogHistory(LogHistoryViewModel args)
        {
            try
            {
                var result = await _settingBO.SaveLogHistory(args);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetReasons", ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }
    }
}