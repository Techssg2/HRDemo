using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Recruitments
{
    public class PositionController : RecruitmentController
    {
        private IMassBO massBo;
        public PositionController(ILogger logger, IRecruitmentBO setting, IMassBO _massBO) : base(logger, setting)
        {
            massBo = _massBO;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllListPositionForFilter()
        {
            try
            {
                var res = await _recruitmentBO.GetAllListPositionForFilter();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListPosition", ex.Message);
                return BadRequest("Error System");
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> GetOpenPositions()
        {
            try
            {
                var res = await _recruitmentBO.GetOpenPositions();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetOpenPosition", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListPosition([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetListPosition(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListPosition", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListPositionDetail([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetListPositionDetail(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListPositionDetail", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateNewPosition([FromBody] PositionForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.CreateNewPosition(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateNewPosition", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdatePosition([FromBody] PositionForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.UpdatePosition(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdatePosition", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ReAssigneeInPosition([FromBody] ReAssigneeInPositionArgs model)
        {
            try
            {
                var res = await _recruitmentBO.ReAssigneeAsync(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at method: {nameof(ReAssigneeInPosition)}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SendEmailToAssignee(Guid? assigneeId, Guid positionId)
        {
            try
            {
                var res = await _recruitmentBO.SendEmailToAssignee(_logger, assigneeId, positionId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at method: {nameof(SendEmailToAssignee)}", ex.Message);
                return BadRequest("Error");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> DeletePosition(Guid id)
        {
            try
            {
                var res = await _recruitmentBO.DeletePosition(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeletePosition", ex.Message);
                return BadRequest("System Error");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ChangeStatus([FromBody] PositionStatusArgs model)
        {
            try
            {
                var res = await _recruitmentBO.ChangeStatus(model);
                if (res.IsSuccess)
                {
                    await massBo.ChangeStatusPositionToMass(new Aeon.HR.ViewModels.PositionChangingStatus { Id = model.PositionId, InActive = model.Status == Aeon.HR.Infrastructure.Enums.PositionStatus.Closed ? false : true });
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdatePosition", ex.Message);
                return BadRequest("Error System");
            }
        }

        //[HttpPost]
        //public async Task<IHttpActionResult> GetAllNewStaffOnboard(StatusApplicantArgs arg)
        //{
        //    try
        //    {
        //        var res = await _recruitmentBO.GetAllNewStaffOnboard(arg);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error at method: DeletePosition", ex.Message);
        //        return BadRequest("System Error");
        //    }
        //}

        [HttpPost]
        public async Task<IHttpActionResult> GetPositionMappingApplicant([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetPositionMappingApplicant(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPositionMappingApplicant", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetPositionForActing([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetPositionForActing(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPositionMappingApplicant", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<ResultDTO> GetPositionById(Guid id)
        {
            try
            {
                var res = await _recruitmentBO.GetPositionById(id);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPositionMappingApplicant", ex.Message);
                return new ResultDTO { Messages = { "Not Found" }, ErrorCodes = { 404 } };
            }
        }
        [HttpGet]
        public async Task<ResultDTO> GetPositionByDepartmentId(Guid deptId)
        {
            try
            {
                var res = await _recruitmentBO.GetPositionByDepartmentId(deptId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPositionByDepartmentId", ex.Message);
                return new ResultDTO { Messages = { "Not Found" }, ErrorCodes = { 404 } };
            }
        }
    }
}