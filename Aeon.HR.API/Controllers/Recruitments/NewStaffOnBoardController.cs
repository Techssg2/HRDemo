using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.ViewModels.Args;

namespace SSG2.API.Controllers.Recruitments
{
    public class NewStaffOnBoardController : RecruitmentController
    {
        public NewStaffOnBoardController(ILogger logger, IRecruitmentBO setting) : base(logger, setting)
        {
        }

        [HttpPost]
        public async Task<ResultDTO> UpdateStatusNewStaffOnBoard([FromBody] UpdateNewStaffOnBoardArgs data)
        {
            try
            {
                var res = await _recruitmentBO.UpdateStatusNewStaffOnBoard(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateStatusNewStaffOnBoard", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetAllNewStaffOnboard([FromBody] QueryArgs data)
        {
            try
            {
                var res = await _recruitmentBO.GetAllNewStaffOnboard(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllNewStaffOnboard", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}
