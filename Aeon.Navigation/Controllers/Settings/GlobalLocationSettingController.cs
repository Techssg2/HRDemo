using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.DTOs;

namespace SSG2.API.Controllers.Settings
{
    public class GlobalLocationSettingController : SettingController
    {
        public GlobalLocationSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        [HttpPost]
        public async Task<ResultDTO> GetListGlobalLocations(QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetListGlobalLocations(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListGlobalLocation", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateGlobalLocation(GlobalLocationArgs data)
        {
            try
            {
                var res = _settingBO.CreateGlobalLocation(data);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateGlobalLocation", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteGlobalLocation(GlobalLocationArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteGlobalLocation(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteGlobalLocation", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateGlobalLocation(GlobalLocationArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateGlobalLocation(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateGlobalLocation", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetGlobalLocationByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetGlobalLocationByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetHotelByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetGlobalLocationByArrivalPartition(QueryArgs arg)
        {
            try
            {
                var res = await _settingBO.GetGlobalLocationByArrivalPartition(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetGlobalLocationByArrivalPartition", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}
