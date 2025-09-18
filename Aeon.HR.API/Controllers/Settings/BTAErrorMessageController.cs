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
    public class BTAErrorMessageController : SettingController
    {
        public BTAErrorMessageController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<ResultDTO> GetBTAErrorMessageList([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetBTAErrorMessageList(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateBTAErrorMessageList(BTAErrorMessageArgs data)
        {
            try
            {
                var res = await _settingBO.CreateBTAErrorMessageList(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteBTAErrorMessage(BTAErrorMessageArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteBTAErrorMessage(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateBTAErrorMessage(BTAErrorMessageArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateBTAErrorMessage(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}