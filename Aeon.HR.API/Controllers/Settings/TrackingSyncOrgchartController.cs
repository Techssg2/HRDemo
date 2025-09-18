using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using SSG2.API.Controllers.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.Settings
{
    public class TrackingSyncOrgchartController : BaseController
    {
        protected readonly ITrackingSyncOrgchartBO _trackingSyncOrgchart;
        public TrackingSyncOrgchartController(ILogger logger, ITrackingSyncOrgchartBO _trackingSyncOrgchart) : base(logger) {
            this._trackingSyncOrgchart = _trackingSyncOrgchart;
        }
        [HttpPost]
        public async Task<ResultDTO> GetTrackingUserDepartmentsRequest(QueryArgs args)
        {
            try
            {
                var result = await _trackingSyncOrgchart.GetTrackingUserDepartmentsRequest(args);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetReasons", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetTrackingUsersLogRequest(QueryArgs args)
        {
            try
            {
                var result = await _trackingSyncOrgchart.GetTrackingUsersLogRequest(args);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetReasons", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetTrackingDepartmentsLogRequest(QueryArgs args)
        {
            try
            {
                var result = await _trackingSyncOrgchart.GetTrackingDepartmentsLogRequest(args);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetReasons", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}