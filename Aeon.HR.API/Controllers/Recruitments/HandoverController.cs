using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Recruitments
{
    public class HandoverController : RecruitmentController
    {
        public HandoverController(ILogger logger, IRecruitmentBO recruitment) : base(logger, recruitment) { }
        #region Handover
        [HttpPost]
        public async Task<IHttpActionResult> CreateHandover([FromBody] HandoverDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.CreateHandover(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateHandover " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListHandovers([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetListHandovers(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListHandovers", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListDetailHandover([FromBody] HandoverDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.GetListDetailHandover(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListDetailHandover", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> SearchListHandover([FromBody] HandoverDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.SearchListHandover(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchListHandover", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteHandover([FromBody] HandoverDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.DeleteHandover(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteHandover", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateHandover([FromBody] HandoverDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.UpdateHandover(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateHandover", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetHandovers([FromBody] QueryArgs model)
        {
            try
            {
                var res = await _recruitmentBO.GetHandovers(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetHandovers", ex.Message);
                return BadRequest("Error System");
            }
        }
        #endregion
    }
}