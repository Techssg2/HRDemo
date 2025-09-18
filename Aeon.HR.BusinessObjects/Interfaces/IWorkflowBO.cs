
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IWorkflowBO
    {
        Task<ResultDTO> GetWorkflowTemplates(QueryArgs args);
        Task<ResultDTO> GetWorkflowTemplateById(Guid id);
        ResultDTO GetAllItemTypes();
        Task PushData(WorkflowEntity item);
        Task<ResultDTO> UpdateWorkflowTemplate(WorkflowTemplateViewModel args);
        Task<ResultDTO> ChangeStatusWorkflow(WorkflowTemplateViewModel args);
        Task<ResultDTO> CreateWorkflowTemplate(WorkflowTemplateViewModel args);
        Task<ResultDTO> StartWorkflow(Guid WorkflowId, Guid itemId, string comment = "");
        Task<ResultDTO> GetRunningWorkflow();
        Task<ResultDTO> GetWorkflowStatusByItemId(Guid itemId, bool force = false);
        Task<ResultDTO> TerminateWorkflowById(Guid workflowId);
        Task<ResultDTO> Vote(VoteArgs args);
        Task<ResultDTO> GetTasks(QueryArgs args, bool isSuperAdmin);
        Task<ResultDTO> GetTodoList(TodoListArgs args);
        Task<ResultDTO> SyncWorkflowById(Guid Id);
        Task<ResultDTO> UpdateAssignToByItemId(UpdateAssignToWorkflowArgs args);
        Task<ResultDTO> ChangeStepWorkflow(ChangeStepWorkflowArgs args);
        void SendEmailNotificationForApprover(EmailTemplateName type, EdocTaskViewModel task);
        List<string> IgnoreReferenceNumberActing();
        Task<ResultDTO> VoteAdminBTA(VoteArgs args);
        Task<ResultDTO> ReFindAssignToCurrentStep(Guid InstanceId);
        Task<ResultDTO> CheckReFindAssign(Guid InstanceId);
    }
}
