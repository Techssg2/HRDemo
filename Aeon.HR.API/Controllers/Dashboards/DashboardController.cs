using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Dashboards
{
    public class DashboardController : BaseController
    {
        protected readonly IDashboardBO _dashboardBO;
        private readonly ISSGExBO _bo;
        public DashboardController(ILogger logger, IDashboardBO dashboardBO, ISSGExBO bo) : base(logger)
        {
            _dashboardBO = dashboardBO;
            _bo = bo;
        }
        [HttpGet]
        public async Task<ResultDTO> GetMyItems()
        {
            return await _dashboardBO.GetMyItems();
        }

        [HttpGet]
        public ResultDTO GetEmployeeNodesByDepartment(Guid departmentId, int maxLvl = 3)
        {
            return _dashboardBO.GetEmployeeNodesByDepartment(departmentId, maxLvl);
        }
        [HttpGet]
        public ResultDTO RefreshDepartmentNodes()
        {
            return _dashboardBO.ClearNode();
        }
        [HttpGet]
        public async Task<ResultDTO> InsertDepartment(int number)
        {
            try
            {
                return await _bo.InserDepartment(number);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at ReTry: " + ex.Message);
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }
    }
}
