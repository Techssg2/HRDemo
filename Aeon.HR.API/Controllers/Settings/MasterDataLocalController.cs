using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Settings
{
    public class MasterDataLocalController : SettingController
    {
        public MasterDataLocalController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        [HttpPost]
        public async Task<IHttpActionResult> GetMasterDataApplicantList(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetMasterDataApplicantList(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetMasterDataApplicantList " + ex.Message);
                return BadRequest("System Error");
            }
        }
    }
}
