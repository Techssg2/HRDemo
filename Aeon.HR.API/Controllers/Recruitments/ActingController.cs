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
    public class ActingController : RecruitmentController
    {
        public ActingController(ILogger logger, IRecruitmentBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<IHttpActionResult> CreateActing([FromBody]MasterActingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.CreateActing(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateActing " + ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetActings(QueryArgs data)
        {
            try
            {
                var res = await _recruitmentBO.GetActings(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetActings " + ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetActingByReferenceNumber(ActingRequestArgs data)
        {
            try
            {
                var res = await _recruitmentBO.GetActingByReferenceNumber(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetActingByReferenceNumber " + ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> PrintForm(Guid id)
        {
            try
            {

                var arrayContent = await _recruitmentBO.PrintFormActing(id);
                if (arrayContent == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent,
                    FileName = string.Format("Record-{0}.xlsx", id),
                    Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: PrintFormPromoteTransfer " + ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }
    }
}
