using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.ViewModels.Args;
using AutoMapper;
using Aeon.HR.ViewModels;
using Aeon.HR.BusinessObjects.Jobs;

namespace SSG2.API.Controllers.CBs
{
    public class LeaveManagementController : CandBController
    {
        protected readonly IEmployeeBO employeeBo;
        private readonly SendMail1STResignations _send;
        public LeaveManagementController(ILogger logger, ICBBO cbBO, IEmployeeBO employee, SendMail1STResignations send) : base(logger, cbBO)
        {
            employeeBo = employee;
            _send = send;
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListLeaveApplication([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _cbBO.GetListLeaveApplication(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListLeave", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateNewLeaveApplication([FromBody] LeaveApplicationForCreatingArgs model)
        {
            try
            {
                var res = await _cbBO.CreateNewLeaveApplication(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateNewLeave", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateLeaveApplication([FromBody] LeaveApplicationForCreatingArgs model)
        {
            try
            {
                var res = await _cbBO.UpdateLeaveApplication(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateLeaveApplication", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> DeleteLeaveApplication(Guid id)
        {
            try
            {
                var res = await _cbBO.DeleteLeaveApplication(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteLeaveApplication", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckValidLeaveKind(ObjectToCheckValidLeaveManagemetDTO objectToCheck)
        {
            try
            {
                //Will Replace Function FinalCheckValidLeaveKind
                var res = await _cbBO.FinalCheckValidLeaveKind(objectToCheck);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CheckValidLeaveKind", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetLeaveApplicantDetail(Guid Id)
        {
            try
            {
                var res = await _cbBO.GetLeaveApplicantDetail(Id);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetLeaveApplicantDetail", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1004 }, Messages = { "Error System" } });
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetLeaveApplicationFromUserId(Guid userId)
        {
            try
            {
                var res = await _cbBO.GetLeaveApplicantDetailFromUserId(userId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteLeaveApplication", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetLeaveApplicationById(Guid Id)
        {
            try
            {
                var res = await _cbBO.GetLeaveApplicationById(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetLeaveApplicationById", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1004 }, Messages = { "Error System" } });
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllLeaveManagementByUserId(Guid id)
        {
            try
            {
                var res = await _cbBO.GetAllLeaveManagementByUserId(id);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetLeaveApplicantDetail", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1004 }, Messages = { "Error System" } });
            }
        }
    }

}
