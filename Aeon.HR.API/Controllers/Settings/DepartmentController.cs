using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Infrastructure.Enums;

namespace SSG2.API.Controllers.Settings
{
    public class DepartmentController : SettingController
    {
        public DepartmentController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        #region Department management
        [HttpPost]
        public async Task<IHttpActionResult> CreateDepartment([FromBody] DepartmentArgs model)
        {
            try
            {
                var res = await _settingBO.CreateDepartment(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateDepartment", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateDepartment([FromBody] DepartmentArgs model)
        {
            try
            {
                var res = await _settingBO.UpdateDepartment(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateDepartment" + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetDepartments(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetDepartments(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartments", ex.Message);
                return BadRequest("System Error");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetDetailDepartment([FromBody] DepartmentArgs model)
        {
            try
            {
                var res = await _settingBO.GetDetailDepartment(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDetailDepartment", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> DeleteDepartment(Guid Id)
        {
            try
            {
                var res = await _settingBO.DeleteDepartment(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartments", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetDepartmentTree()
        {
            try
            {
                var res = await _settingBO.GetDepartmentTree();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTree", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetDepartmentByFilter(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetDepartmentByFilter(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTree", ex.Message);
                return BadRequest("System Error");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetDepartmentByFilterV2(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetDepartmentByFilterV2(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTree", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetDepartmentTreeByGrade(Guid jobGradeId)
        {
            try
            {
                var res = await _settingBO.GetDepartmentTreeByGrade(jobGradeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTreeByGrade", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllDeptLineByGrade(Guid jobGradeId)
        {
            try
            {
                var res = await _settingBO.GetAllDeptLineByGrade(jobGradeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllDeptLineByGrade", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetOnlyDivisionTree(Guid departmentId)
        {
            try
            {
                var res = await _settingBO.GetDepartmentTreeByType(departmentId, DepartmentFilterEnum.OnlyDivison);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTreeByType", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetOnlyDeptLineTree(Guid departmentId)
        {
            try
            {
                var res = await _settingBO.GetDepartmentTreeByType(departmentId, DepartmentFilterEnum.OnlyDepartment);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTreeByType", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllChildTree(Guid departmentId)
        {
            try
            {
                var res = await _settingBO.GetDepartmentTreeByType(departmentId, DepartmentFilterEnum.AllChild);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTreeByType", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetDepartmentByReferenceNumber(string prefix, Guid itemId)
        {
            try
            {
                var res = await _settingBO.GetDepartmentByReferenceNumber(prefix, itemId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTreeByType", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetDepartmentByCode(string deptCode)
        {
            try
            {
                var res = await _settingBO.GetDepartmentByCode(deptCode);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateDepartment", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> getAllListDepartments()
        {
            try
            {
                var res = await _settingBO.GetAllListDepartments();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: getAllDepartments", ex.Message);
                return BadRequest("System Error");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetDepartmentsByUserKeyword(CommonArgs.Member.User.GetAllUserByKeyword args)
        {
            try
            {
                var res = await _settingBO.GetDepartmentsByUserKeyword(args);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentsByUser", ex.Message);
                return BadRequest("System Error");
            }
        }
        #endregion
        #region User in Department
        [HttpGet]
        public async Task<IHttpActionResult> GetEmployeeCodes()
        {
            try
            {
                var res = await _settingBO.GetEmployeeCodes();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetEmployeeCodes", ex.Message);
                return BadRequest("System Error");
            }
        }
        public async Task<IHttpActionResult> GetUserInDepartment(Guid Id)
        {
            try
            {
                var res = await _settingBO.GetUserInDepartment(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUserInDepartment", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> AddUserToDepartment([FromBody] UserInDepartmentArgs model)
        {
            try
            {
                var res = await _settingBO.AddUserToDepartment(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: AddUserToDepartment", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserInDepartment([FromBody] UpdateUserDepartmentMappingArgs model)
        {
            try
            {
                var res = await _settingBO.UpdateUserInDepartment(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateUserInDepartment", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> RemoveUserFromDepartment(Guid Id)
        {
            try
            {
                var res = await _settingBO.RemoveUserFromDepartment(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: RemoveUserFromDepartment", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> MoveHeadCountAdd([FromBody] UserInDepartmentArgs model)
        {
            try
            {
                var res = await _settingBO.MoveHeadCountAdd(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: MoveHeadCountAdd", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> MoveHeadCountUpdate([FromBody] UpdateUserDepartmentMappingArgs model)
        {
            try
            {
                var res = await _settingBO.MoveHeadCountUpdate(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: MoveHeadCountUpdate", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetDepartmentById(Guid Id)
        {
            try
            {
                var res = await _settingBO.GetDepartmentById(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentById", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetDepartmentsByUserId (Guid id)
        {
            try
            {
                var res = await _settingBO.GetDepartmentsByUserId(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentsByUserId", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetDepartmentByArg(QueryArgs arg)
        {
            try
            {
                var res = await _settingBO.GetDepartmentByArg(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentByArg", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetDepartmentUpToG4ByUserId(Guid id)
        {
            try
            {
                var res = await _settingBO.GetDepartmentUpToG4ByUserId(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentByArg", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllDepartmentsByPositonName(string posiontionName)
        {
            try
            {
               var res = await _settingBO.GetAllDepartmentsByPositonName(posiontionName);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: getAllDepartmentsByPositonName", ex.Message);
                return BadRequest("System Error");
            }
        }
        #endregion
        #region Region management
        [HttpPost]
        public async Task<IHttpActionResult> GetRegionList(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetRegionList(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetRegionList", ex.Message);
                return BadRequest("System Error");
            }
        }
        #endregion
    }
}
