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
using Aeon.HR.ViewModels;

namespace SSG2.API.Controllers.Recruitments
{
    public class ApplicantController : RecruitmentController
    {
        public ApplicantController(ILogger logger, IRecruitmentBO setting) : base(logger, setting)
        {
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetApplicantList([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetApplicantList(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetApplicantList", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetSimpleApplicantList([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetSimpleApplicantList(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetSimpleApplicantList", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SearchApplicantList(ApplicantSearchArgs query)
        {
            try
            {
                var res = await _recruitmentBO.SearchApplicantList(query);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchApplicantList", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CreateApplicant([FromBody]ApplicantArgs data)
        {
            try
            {
                var res = await _recruitmentBO.CreateApplicant(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateApplicant", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateApplicant(ApplicantArgs data)
        {
            try
            {
                var res = await _recruitmentBO.UpdateApplicant(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateApplicant", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> DeleteApplicant(Guid id)
        {
            try
            {
                var res = await _recruitmentBO.DeleteApplicant(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteApplicant", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdatePositionDetail(PositionDetailItemArgs data)
        {
            try
            {
                var res = await _recruitmentBO.UpdatePositionDetail(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdatePositionDetail", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<ResultDTO> SubmitApplicant(Guid ItemId)
        {
            try
            {
                var res = await _recruitmentBO.SubmitApplicant(ItemId);
                return new ResultDTO { Object = res };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SubmitApplicant", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> SearchApplicant(QueryArgs args)
        {
            try
            {
                var res = await _recruitmentBO.SearchApplicant(args);
                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchApplicant", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetApplicantDetail(Guid Id)
        {
            try
            {
                var res = await _recruitmentBO.GetDetailApplicant(Id);
                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetApplicantDetail", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

    }
}
