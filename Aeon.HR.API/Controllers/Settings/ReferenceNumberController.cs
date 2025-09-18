using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Infrastructure.Enums;

namespace SSG2.API.Controllers.Settings
{
    public class ReferenceNumberController : SettingController
    {
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public ReferenceNumberController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        #region ReferencyNumber 
        [HttpGet]
        public async Task<ResultDTO> GetReferencyNumberRecruiments()
        {
            try
            {
                var res = await _settingBO.GetReferencyNumberRecruiments();
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetReferencyNumber " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        
        [HttpPost]
        public async Task<ResultDTO> UpdateReferencyNumberRecruitment([FromBody] ReferencyNumberArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateReferencyNumberRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateReferencyNumber " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion
    }
}
