using Aeon.HR.BusinessObjects.Interfaces;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using Aeon.HR.ViewModels.DTOs;
using System.Web.Http;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.Args;
using System;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.API.Controllers.Navigation
{
    public class NavigationController : BaseController
    {
        protected readonly INavigationBO _navigationBO;
        private readonly ISSGExBO _bo;
        public NavigationController(ILogger logger, INavigationBO navigationBO, ISSGExBO bo) : base(logger)
        {
            _navigationBO = navigationBO;
            _bo = bo;
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListNavigation([FromBody] QueryArgs args)
        {
            try
            {
                var res = await _navigationBO.GetListNavigation(args);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListUsers", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var res = await _navigationBO.GetAll();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListNav", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetListNavigationByUserIdAndDepartmentId()
        {
            try
            {
                var res = await _navigationBO.GetListNavigationByUserIdAndDepartmentId();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListNav", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListNavigationByUserIdAndDepartmentIdV2(NavigationArgs.GetListArgs args)
        {
            try
            {
                var res = await _navigationBO.GetListNavigationByUserIdAndDepartmentIdV2(args);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListNav", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateNavigation([FromBody] NavigationDataForCreatingArgs model)
        {
            try
            {
                var res = await _navigationBO.CreateNavigation(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateUser", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateNavigationById([FromBody] NavigationDataForCreatingArgs model)
        {
            try
            {
                var res = await _navigationBO.UpdateNavigation(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Update User", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> DeleteNavigationById(Guid Id)
        {
            try
            {
                var res = await _navigationBO.DeleteNavigationById(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Update User", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<ResultDTO> UpdateImageNavigation(NavigationDataForCreatingArgs data)
        {
            try
            {
                var res = await _navigationBO.UpdateImageNavigation(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateImageNav", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetChildNavigationByParentId(Guid parentId)
        {
            try
            {
                var res = await _navigationBO.GetChildNavigationByParentId(parentId);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListNav", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetChildNavigationByType(NavigationType type)
        {
            try
            {
                var res = await _navigationBO.GetChildNavigationByType(type);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetChildNavigationByType", ex.Message);
                return BadRequest("Error System");
            }
        }
    }
}