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

namespace Aeon.Navigation.Controllers.Settings
{
    public class HeadCountSettingController : SettingController
    {
        public HeadCountSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        
        [HttpPost]
        public async Task<IHttpActionResult> GetJobGradeList(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetJobGradeList(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetJobGradeList " + ex.Message);
                return BadRequest("System Error");
            }
        }
    }
}
