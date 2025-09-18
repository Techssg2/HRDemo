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
    public class AirlineController : SettingController
    {
        public AirlineController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<ResultDTO> GetAirlines([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetAirlines(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAirlines " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetAirlineById([FromBody] AirlineArg arg)
        {
            try
            {
                var res = _settingBO.GetAirlineById(arg.Id);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAirlineById " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> SaveAirline([FromBody] AirlineArg arg)
        {
            try
            {
                var res = _settingBO.SaveAirline(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveAirline " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> DeleteAirline([FromBody] AirlineArg arg)
        {
            try
            {
                var res = _settingBO.DeleteAirline(arg.Id);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteAirline " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> CheckValidateCode([FromBody] AirlineArg arg)
        {
            try
            {
                var res = _settingBO.CheckValidateCode(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteAirline " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> ValidateWhenFlightNumberUsed([FromBody] AirlineArg arg)
        {
            try
            {
                var res = _settingBO.ValidateWhenFlightNumberUsed(arg.Id);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ValidateWhenFlightNumberUsed " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}