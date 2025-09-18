using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.Workflow
{
    public class WorkflowController : BaseController
    {
        protected readonly IWorkflowBO _workflowBO;
        public WorkflowController(ILogger logger, IWorkflowBO workflowBO) : base(logger)
        {
            _workflowBO = workflowBO;
        }

        [HttpGet]
        public async Task<ResultDTO> GetRunningWorkflow()
        {
            return await _workflowBO.GetRunningWorkflow();
        }

        [HttpGet]
        public async Task<ResultDTO> GetWorkflowStatusByItemId(Guid itemId)
        {
            return await _workflowBO.GetWorkflowStatusByItemId(itemId);
        }

        [HttpGet]
        public async Task<ResultDTO> GetWorkflowTemplateById(Guid id)
        {
            return await _workflowBO.GetWorkflowTemplateById(id);
        }

        [HttpGet]
        public ResultDTO GetAllItemTypes()
        {
            return _workflowBO.GetAllItemTypes();
        }

        [HttpPost]
        public async Task<ResultDTO> GetWorkflowTemplates(QueryArgs args)
        {
            return await _workflowBO.GetWorkflowTemplates(args);
        }

        [HttpPost]
        public async Task<ResultDTO> StartWorkflow(Guid workflowId, Guid itemId, string comment = "")
        {
            return await _workflowBO.StartWorkflow(workflowId, itemId, comment);
        }

        [HttpPost]
        public async Task<ResultDTO> TerminateWorkflowById(Guid workflowId)
        {
            return await _workflowBO.TerminateWorkflowById(workflowId);
        }

        [HttpPost]
        public async Task<ResultDTO> UpdateWorkflowTemplate(WorkflowTemplateViewModel args)
        {
            return await _workflowBO.UpdateWorkflowTemplate(args);
        }

        [HttpPost]
        public async Task<ResultDTO> ChangeStatusWorkflow(WorkflowTemplateViewModel args)
        {
            return await _workflowBO.ChangeStatusWorkflow(args);
        }

        [HttpPost]
        public async Task<ResultDTO> CreateWorkflowTemplate(WorkflowTemplateViewModel args)
        {
            return await _workflowBO.CreateWorkflowTemplate(args);
        }

        [HttpPost]
        public async Task<ResultDTO> Vote(VoteArgs args)
        {
            return await _workflowBO.Vote(args);
        }
        [HttpPost]
        public async Task<ResultDTO> GetTasks(QueryArgs args, bool isSuperAdmin = false)
        {
            return await _workflowBO.GetTasks(args, isSuperAdmin);
        }
        [HttpGet]
        public async Task<ResultDTO> SyncWorkflowById(Guid Id)
        {
            return await _workflowBO.SyncWorkflowById(Id);
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateAssignToByItemId(UpdateAssignToWorkflowArgs args)
        {
            return await _workflowBO.UpdateAssignToByItemId(args);
        }

        [HttpPost]
        public async Task<ResultDTO> ChangeStepWorkflow(ChangeStepWorkflowArgs args)
        {
            return await _workflowBO.ChangeStepWorkflow(args);
        }
        [HttpPost]
        public async Task<ResultDTO> VoteAdminBTA(VoteArgs args)
        {
            return await _workflowBO.VoteAdminBTA(args);
        }
        [HttpGet]
        public async Task<ResultDTO> ReFindAssignToCurrentStep(Guid InstanceId)
        {
            return await _workflowBO.ReFindAssignToCurrentStep(InstanceId);
        }
        [HttpGet]
        public async Task<ResultDTO> CheckReFindAssign(Guid InstanceId)
        {
            return await _workflowBO.CheckReFindAssign(InstanceId);
        }
    }
}