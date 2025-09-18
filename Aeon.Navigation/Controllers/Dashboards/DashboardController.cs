using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.Navigation.Controllers.Others;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aeon.Navigation.Controllers.Dashboards
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
    }
}
