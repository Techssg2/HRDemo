using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using SSG2.API.Controllers.Others;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;

namespace SSG2.API.Controllers.Recruitments
{
    public class TrackingLogController: RecruitmentController
    {
        public TrackingLogController(ILogger logger, IRecruitmentBO recruitment) : base(logger, recruitment)
        {
        }
        #region Tracking Log
        [HttpPost]
        public async Task<IHttpActionResult> SaveTrackingLog([FromBody] TrackingLogArgs model)
        {
            try
            {
                var res = await _recruitmentBO.SaveTrackingLog(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveTrackingLog", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListTrackingLog([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetListTrackingLog(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListTrackingLog", ex.Message);
                return BadRequest("Error System");
            }
        }
        #endregion
    }
}