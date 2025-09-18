using Aeon.HR.BusinessObjects.Handlers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static Aeon.HR.ViewModels.Args.CommonArgs.User;

namespace Aeon.HR.API.Controllers.Others
{
    public class APIEdoc2Controller : BaseController
    {
        protected readonly IAPIEdoc2BO bo;
        protected readonly ITargetPlanBO _targetPlanBO;
        public APIEdoc2Controller(ILogger _logger, IAPIEdoc2BO _bo, ITargetPlanBO targetPlanBO) : base(_logger)
        {
            bo = _bo;
            _targetPlanBO = targetPlanBO;
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetItemByReferenceNumber(CommonDTO arg)
        {
            try
            {
                var res = await bo.GetItemByReferenceNumber(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBTA_API", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllBTA_API(string ReferenceNumber, string Limit)
        {
            try
            {
                var res = await bo.GetAllBTA_API(ReferenceNumber, Limit);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBTA_API", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateStatusBTA(CommonDTO agrs)
        {
            try
            {
                var res = await bo.UpdateStatusBTA(agrs);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Update Status BTA-RE", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetDepartmentTree_API()
        {
            try
            {
                var res = await bo.GetDepartmentTree_API();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTree", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllUser_API()
        {
            try
            {
                var res = await bo.GetAllUser_API();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsers", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetAllUser_APIV2(IntergationAPI args)
        {
            try
            {
                var res = await bo.GetAllUser_APIV2(args);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllUser_APIV2", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetSpecificShiftPlan_API(ShiftPlanAPIArgs args)
        {
            try
            {
                var res = await bo.GetSpecificShiftPlan_API(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetActualShiftPlan_API", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetActualShiftPlan_API(IntergationAPI args)
        {
            try
            {
                var res = await bo.GetActualShiftPlan_API(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetActualShiftPlan_API", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetActualShiftPlanForDWS_API(IntergationAPI args)
        {
            try
            {
                var res = await bo.GetActualShiftPlanForDWS_API(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetActualShiftPlanForDWS_API", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> AccountVerification_API(string username, string password)
        {
            try
            {
                var res = await bo.AccountVerification_API(username, password);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: AccountVerification_API", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateTargetPlan_API(CreateTargetPlan_APIArgs agrs)
        {
            ResultDTO result = new ResultDTO() { };
            try
            {
                result = await _targetPlanBO.CreateTargetPlan_API(agrs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Messages = new List<string>() { $"Error at method: CreateTargetPlan_API {ex.Message}" };
                return Ok(result);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ValidateForEditDWS_API(CreateTargetPlan_APIArgs agrs)
        {
            ResultDTO result = new ResultDTO() { };
            try
            {
                result = await _targetPlanBO.ValidateForEditDWS_API(agrs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Messages = new List<string>() { $"Error at method: ValidateForEditDWS_API {ex.Message}" };
                return Ok(result);
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetUsersForTargetPlanByDeptIdDWS([FromBody] UserForDWSArg arg)
        {
            try
            {
                var res = await bo.GetUsersForTargetPlanByDeptIdDWS(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersForTargetPlanByDeptIdDWS", ex.Message);
                return BadRequest("Error System");
            }
        }
    }
}