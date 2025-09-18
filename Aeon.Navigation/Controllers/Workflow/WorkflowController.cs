using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.Navigation.Controllers.Others;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aeon.Navigation.Controllers.Workflow
{
    public class WorkflowController : BaseController
    {
        protected readonly IWorkflowBO _workflowBO;
        public WorkflowController(ILogger logger, IWorkflowBO workflowBO) : base(logger)
        {
            _workflowBO = workflowBO;
        }

        [HttpPost]
        public async Task<ResultDTO> GetTasks(QueryArgs args, bool isSuperAdmin = false)
        {
            return await _workflowBO.GetTasks(args, isSuperAdmin);
        }
    }
}