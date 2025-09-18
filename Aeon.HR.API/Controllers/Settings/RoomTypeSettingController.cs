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
    public class RoomTypeSettingController : SettingController
    {
        public RoomTypeSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        #region Setting - Room Type
        [HttpPost]
        public async Task<ResultDTO> GetListRoomTypes([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetListRoomTypes(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListRoomTypes " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateRoomType(RoomTypeArgs data)
        {
            try
            {
                var res = await _settingBO.CreateRoomType(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateRoomType " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteRoomType(RoomTypeArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteRoomType(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteRoomType " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateRoomType(RoomTypeArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateRoomType(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateRoomType " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetRoomTypeByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetRoomTypeByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetRoomTypeByCode " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion
    }
}