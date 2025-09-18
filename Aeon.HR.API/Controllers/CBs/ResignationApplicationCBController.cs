using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.ViewModels.Args;


namespace SSG2.API.Controllers.CBs
{
    public class ResignationApplicationCBController : CandBController
    {
        public ResignationApplicationCBController(ILogger logger, ICBBO cbBO) : base(logger, cbBO) { }

        [HttpPost]
        public async Task<ResultDTO> GetAllResignationApplicantion(QueryArgs args)
        {
            try
            {
                var res = await _cbBO.GetAllResignationApplicantion(args);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllResignationApplicantion", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpGet]
        public async Task<ResultDTO> CheckInProgressResignationApplicantion(string userSapCode)
        {
            try
            {
                var res = await _cbBO.CheckInProgressResignationApplicantion(userSapCode);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CheckInProgressResignationApplicantion", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        //Function Check moi
        [HttpGet]
        public async Task<ResultDTO> CheckInProgressResignationWithIsActive(string userSapCode)
        {
            try
            {
                var res = await _cbBO.CheckInProgressResignationWithIsActive(userSapCode);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CheckInProgressResignationWithIsActive", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetResignationApplicantionByReferenceNumber(MasterdataArgs args)
        {
            try
            {
                var res = await _cbBO.GetResignationApplicantionByReferenceNumber(args);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetResignationApplicantionByReferenceNumber", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpGet]
        public async Task<ResultDTO> GetResignationApplicantionById(Guid id)
        {
            try
            {
                var record = await _cbBO.GetResignationApplicantionById(id);
                if (record != null)
                {
                    return new ResultDTO { Object = record };
                }
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetResignationApplicantionByReferenceNumber", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> SaveResignationApplicantion(ResignationApplicationArgs data)
        {
            try
            {
                var res = await _cbBO.SaveResignationApplicantion(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveResignationApplicantion", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> SubmitResignationApplicantion(ResignationApplicationArgs data)
        {
            try
            {
                var res = await _cbBO.SubmitResignationApplicantion(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SubmitResignationApplicantion", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> PrintForm(Guid id)
        {
            try
            {

                var arrayContent = await _cbBO.PrintFormResignation(id);
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
                _logger.LogError($"Error at Print File: ResignationApplicantion", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetSubmitedFirstDate(Guid ItemId)
        {
            try
            {
                var res = await _cbBO.GetSubmittedFirstDate(ItemId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetSubmitedFirstDate", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CountExitInterview(Guid ItemId)
        {
            try
            {
                var res = await _cbBO.CountExitInterview(ItemId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CountExitInterview", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}
