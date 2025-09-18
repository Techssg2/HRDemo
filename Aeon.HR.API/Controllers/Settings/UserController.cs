using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Infrastructure.Enums;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System.Linq;
using System.Collections.Generic;

namespace SSG2.API.Controllers.Settings
{
    public class UserController : SettingController
    {
        public UserController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        [HttpPost]
        public async Task<IHttpActionResult> SAPCreateUser([FromBody] SAPUserDataForCreatingArgs model)
        {
            try
            {
                var res = await _settingBO.SAPCreateUser(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SAPCreateUser", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateUser([FromBody] UserDataForCreatingArgs model)
        {
            try
            {
                var res = await _settingBO.CreateUser(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateUser", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ForgotPassword(string username, string email)
        {
            try
            {
                return Ok(await _settingBO.ForgotPassword(username, email));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ForgotPassword", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordArgs args)
        {
            try
            {
                return Ok(await _settingBO.ChangePassword(args));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ChangePassword", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ActiveAllUser(string password)
        {
            try
            {
                await _settingBO.ActiveAllUser(password);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateUser", ex.Message);
                return BadRequest(string.Format("Error System; {0}", ex.Message));
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> Update([FromBody] UserDataForCreatingArgs model)
        {
            try
            {
                var res = await _settingBO.Update(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Update User", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetUsers([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _settingBO.GetListUsers(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListUsers", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetUsersInCurrentDivision(Guid? departmentId, string currentSapCode)
        {
            try
            {
                var res = await _settingBO.GetUsersByDivision(departmentId, GetUserByDivisionEnum.InCurrentDepartment, currentSapCode);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersByDivision", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetChildUsers(Guid departmentId, int limit, int page, string searchText, bool isAll = false)
        {
            try
            {
                var res = await _settingBO.GetChildUsers(departmentId, limit, page, searchText, isAll);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetChildUsers", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetUsersForReportTargetPlan(Guid departmentId, Guid periodId, int limit, int page, string searchText, bool isMade = false)
        {
            try
            {
                var res = await _settingBO.GetUsersForReportTargetPlan(departmentId, periodId, limit, page, searchText, isMade);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersForReportTargetPlan", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetUsersForTargetPlanByDeptId([FromBody] UserForTargetArg arg)
        {
            try
            {
                var res = await _settingBO.GetUsersForTargetPlanByDeptId(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersForTargetPlanByDeptId", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetValidUsersForSubmitTargetPlan([FromBody] UserForTargetArg arg)
        {
            try
            {
                var res = await _settingBO.GetValidUsersForSubmitTargetPlan(arg);
                return Ok(new ResultDTO { Object = new ArrayResultDTO { Data = res, Count = res.Count } });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetValidUsersForSubmitTargetPlan", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetUserCheckedHeadCount(Guid departmentId, string textSearch)
        {
            try
            {
                var res = await _settingBO.GetUserCheckedHeadCount(departmentId, textSearch);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUserCheckedHeadCount", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetUsersByOnlyDeptLine(Guid depLineId, int limit, int page, string searchText, bool isAll = false)
        {
            try
            {
                var res = await _settingBO.GetUsersByOnlyDeptLine(depLineId, limit, page, searchText, isAll);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersByOnlyDeptLine", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetUsersByDeptLines([FromBody] CommonArgs.Department.GetUsersByDeptLines args, bool isAll = false)
        {
            try
            {
                var res = await _settingBO.GetUsersByDeptLines(args, isAll);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersByDeptLines", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetUsersInCurrentDivisionAndChild(Guid? departmentId, string currentSapCode)
        {
            try
            {
                var res = await _settingBO.GetUsersByDivision(departmentId, GetUserByDivisionEnum.AllChildDepartment, currentSapCode);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersByDivision", ex.Message);
                return BadRequest("Error System");
            }
        }
        public async Task<IHttpActionResult> GetUsersInAllDivision(string currentSapCode)
        {
            try
            {
                var res = await _settingBO.GetUsersByDivision(null, GetUserByDivisionEnum.AllChildDepartment, currentSapCode);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersByDivision", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ChangeStatus([FromBody] UserInfoCABArg arg)
        {
            try
            {
                var res = await _settingBO.ChangeStatus(arg.UserId, arg.IsActivated);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ChangeStatus", ex.Message);
                return BadRequest("Error System");
            }
        }
        //lamnl: 19/01/2022
        [HttpPost]
        public async Task<IHttpActionResult> LockUserMembership([FromBody] UserInfoCABArg arg)
        {
            try
            {
                var res = await _settingBO.LockUserMembership(arg.UserId, arg.IsActivated, arg.LockType);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Lock/ Unlock user", ex.Message);
                return BadRequest("Error System");
            }
        }//end
        //lamnl: 24/01/2022
        [HttpPost]
        public async Task<ResultDTO> CheckLockUser([FromBody] UserInfoCABArg arg)
        {
            try
            {
                var res = await _settingBO.CheckLockUser(arg.UserId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUserProfileDataById", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }//end

        [HttpPost]
        public async Task<IHttpActionResult> ResetPassword([FromBody] UserInfoCABArg arg)
        {
            try
            {
                var res = await _settingBO.ResetPassword(arg.UserId);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ResetPassword", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetUserById([FromBody] UserInfoCABArg arg)
        {
            try
            {
                var res = await _settingBO.GetUserById(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUserById", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpGet]
        public async Task<ResultDTO> GetCurrentUser()
        {
            try
            {
                var res = await _settingBO.GetCurrentUser();
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetCurrentUser", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpGet]
        public async Task<ResultDTO> GetCurrentUserV2()
        {
            try
            {
                var res = await _settingBO.GetCurrentUserV2();
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetCurrentUser", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpGet]
        public async Task<ResultDTO> GetLinkImageUserByLoginName(string loginName)
        {
            try
            {
                var res = await _settingBO.GetLinkImageUserByLoginName(loginName);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetLinkImageUserByLoginName", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> UpdateImageUser(UserDataForCreatingArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateImageUser(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateImageUser", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetImageUserById(UserInfoCABArg data)
        {
            try
            {
                var res = await _settingBO.GetImageUserById(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetImageUserById", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> FindUserForDataInvalid(UserForCheckDataArg data)
        {
            try
            {
                var res = await _settingBO.FindUserForDataInvalid(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: FindUserForDataInvalid", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetUserProfileDataById([FromBody] UserInfoCABArg arg)
        {
            try
            {
                var res = await _settingBO.GetUserProfileDataById(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUserProfileDataById", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetUserProfileCustomById([FromBody] UserInfoCABArg arg)
        {
            try
            {
                var res = await _settingBO.GetUserProfileCustomById(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUserProfileDataById", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CheckUserIsStore(string departmentCode)
        {
            try
            {
                var res = await _settingBO.CheckUserIsStore(departmentCode);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUserProfileDataById", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpGet]
        public async Task<ResultDTO> GetUsersByList(string sapCodes)
        {
            try
            {
                if (!string.IsNullOrEmpty(sapCodes))
                {
                    var res = await _settingBO.GetUsersByListSAPs(sapCodes);
                    return new ResultDTO { Object = res };
                }
                return new ResultDTO { Object = null };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUsersByList" + ex.Message + ": Stack Trace:" + ex.StackTrace);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> CheckUserBySAPCode(string sapCode)
        {
            try
            {
                var res = await _settingBO.CheckUserBySAPCode(sapCode);
                return new ResultDTO { Object = res };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CheckUserBySAPCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetAllUsersByKeyword([FromBody] CommonArgs.Member.User.GetAllUserByKeyword arg)
        {
            try
            {
                var res = await _settingBO.GetAllUsersByKeyword(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListUsers", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllUsers()
        {
            try
            {
                var res = await _settingBO.GetAllUsers();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListUsers", ex.Message);
                return BadRequest("Error System");
            }
        }
    }
}
