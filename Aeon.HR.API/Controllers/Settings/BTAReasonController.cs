using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Settings
{
    public class BTAReasonController : SettingController
    {
        public BTAReasonController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<ResultDTO> GetBTAReasons([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetBTAReasons(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBTAReasons " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> CheckValidateReasons([FromBody] ReasonArg arg)
        {
            try
            {
                var res = _settingBO.CheckValidateReasons(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteAirline " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> SaveBTAReason([FromBody] ReasonArg arg)
        {
            try
            {
                var res = _settingBO.SaveBTAReason(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveBTAReason " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> DeleteReason([FromBody] ReasonArg arg)
        {
            try
            {
                var res = _settingBO.DeleteReason(arg.Id);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteReason " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}