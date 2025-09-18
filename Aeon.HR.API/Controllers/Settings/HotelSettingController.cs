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
    public class HotelSettingController : SettingController
    {
        public HotelSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        #region Setting - Hotel
        [HttpPost]
        public async Task<ResultDTO> GetListHotels([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetListHotels(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListHotels " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateHotel(HotelArgs data)
        {
            try
            {
                var res = await _settingBO.CreateHotel(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateHotel " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteHotel(HotelArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteHotel(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteHotel " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateHotel(HotelArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateHotel(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateHotel " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetHotelByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetHotelByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetHotelByCode " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion
    }
}