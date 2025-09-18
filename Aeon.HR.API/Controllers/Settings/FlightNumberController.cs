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
    public class FlightNumberController : SettingController
    {
        public FlightNumberController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<ResultDTO> GetFlightNumbers([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetFlightNumbers(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetFlightNumbers " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> SaveFlightNumber([FromBody] FlightNumberArg arg)
        {
            try
            {
                var res = _settingBO.SaveFlightNumber(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveFlightNumber " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> DeleteFlightNumber([FromBody] FlightNumberArg arg)
        {
            try
            {
                var res = _settingBO.DeleteFlightNumber(arg.Id);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteFlightNumber " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetFlightNumberById([FromBody] FlightNumberArg arg)
        {
            try
            {
                var res = _settingBO.GetFlightNumberById(arg.Id);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetFlightNumberById " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> CheckValidateFlightNumberCode([FromBody] FlightNumberArg arg)
        {
            try
            {
                var res = _settingBO.CheckValidateFlightNumberCode(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CheckValidateFlightNumberCode " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}
