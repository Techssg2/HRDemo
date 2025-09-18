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
    public class BusinessTripLocationSettingController : SettingController
    {
        public BusinessTripLocationSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        #region Setting - Business Trip Location
        [HttpPost]
        public async Task<ResultDTO> GetListBusinessTripLocation([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetListBusinessTripLocation(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListBusinessTripLocation " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateBusinessTripLocation(BusinessTripLocationArgs data)
        {
            try
            {
                var res = await _settingBO.CreateBusinessTripLocation(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateBusinessTripLocation " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteBusinessTripLocation(BusinessTripLocationArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteBusinessTripLocation(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteBusinessTripLocation " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateBusinessTripLocation(BusinessTripLocationArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateBusinessTripLocation(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateBusinessTripLocation " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetBusinessTripLocationByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetBusinessTripLocationByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBusinessTripLocationByCode " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion
    }
}