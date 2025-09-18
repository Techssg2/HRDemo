using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Recruitments
{
    public class PromoteAndTransferController : RecruitmentController
    {
        public PromoteAndTransferController(ILogger logger, IRecruitmentBO recruitment) : base(logger, recruitment) { }
        #region Promote And Transfer
        [HttpPost]
        public async Task<IHttpActionResult> CreatePromoteAndTransfer([FromBody] PromoteAndTransferDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.CreatePromoteAndTransfer(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreatePromoteAndTransfer", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListPromoteAndTransfers([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetListPromoteAndTransfers(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListPromoteAndTransfers", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetPromoteAndTransferById([FromBody] PromoteAndTransferDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.GetPromoteAndTransferById(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPromoteAndTransferById", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> SearchListPromoteAndTransfers([FromBody] PromoteAndTransferDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.SearchListPromoteAndTransfers(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchListPromoteAndTransfers", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeletePromoteAndTransfer([FromBody] PromoteAndTransferDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.DeletePromoteAndTransfer(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeletePromoteAndTransfer", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdatePromoteAndTransfer([FromBody] PromoteAndTransferDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.UpdatePromoteAndTransfer(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdatePromoteAndTransfer", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> PrintForm(Guid id){
            try
            {

                var arrayContent = await _recruitmentBO.PrintForm(id);
                if (arrayContent == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent,
                    FileName = string.Format("Record-{0}.pdf", id),
                    Type = "Application/pdf"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: PrintFormPromoteTransfer", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }
        #endregion
    }
}