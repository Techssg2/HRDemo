using Aeon.Academy.API.Core;
using Aeon.Academy.API.DTOs;
using Aeon.Academy.API.Filters;
using Aeon.Academy.API.Mappers;
using Aeon.Academy.API.Utils;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aeon.Academy.API.Controllers
{
    public class TrainingReportController : BaseAuthApiController
    {
        private readonly ITrainingReportService trainingReportService;
        private readonly IWorkflowService<TrainingReport> workflowService;
        private readonly SharepointFile sharepointFile;

        public TrainingReportController(ITrainingReportService trainingReportService, IWorkflowService<TrainingReport> workflowService)
        {
            this.trainingReportService = trainingReportService;
            this.workflowService = workflowService;
            this.sharepointFile = new SharepointFile(DocumentLibraryName.TrainingReport);
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            var request = trainingReportService.Get(id);
            if (request == null) return NotFound();

            var formActions = workflowService.GetWorkflowActions(CurrentUser, request);
            var dto = request.ToDto();
            dto.EnabledActions = formActions;
            var doc = sharepointFile.GetCourseDocument(TrainingReportDocumentType.Certificate + "/" + id.ToString());
            if (doc != null) dto.TrainingReportAttachments.Certificate = doc;
            doc = sharepointFile.GetCourseDocument(TrainingReportDocumentType.Material + "/" + id.ToString());
            if (doc != null) dto.TrainingReportAttachments.Material = doc;

            return Ok(dto);
        }
         public IHttpActionResult GetByInvitation(Guid id)
        {
            var request = trainingReportService.GetByInvitation(id, CurrentUser.Id);
            if (request == null) return Ok();

            var formActions = workflowService.GetWorkflowActions(CurrentUser, request);
            var dto = request.ToDto();
            dto.EnabledActions = formActions;
            var doc = sharepointFile.GetCourseDocument(TrainingReportDocumentType.Certificate + "/" + request.Id.ToString());
            if (doc != null) dto.TrainingReportAttachments.Certificate = doc;
            doc = sharepointFile.GetCourseDocument(TrainingReportDocumentType.Material + "/" + request.Id.ToString());
            if (doc != null) dto.TrainingReportAttachments.Material = doc;

            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult MyItems()
        {
            var myItems = trainingReportService.ListByUser(CurrentUser.Id);

            var dtos = myItems.ToMyItemDtos();

            return Ok(dtos);
        }

        [HttpPost]
        [ValidateModel]
        public IHttpActionResult Save(TrainingReportDto dto)
        {
            var request = trainingReportService.GetByInvitation(dto.TrainingInvitationId, CurrentUser.Id);
            request = dto.ToEntity(request, CurrentUser);

            if (!IsAuthorized(request, WorkflowAction.Save)) return Unauthorized();

            var id = trainingReportService.Save(request);
            var result = string.Empty;
            if (dto.TrainingReportAttachments.Certificate != null)
            {
                result = sharepointFile.UploadFiles(TrainingReportDocumentType.Certificate + "/" + id.ToString(), dto.TrainingReportAttachments.Certificate);
            }
            if (dto.TrainingReportAttachments.Material != null)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    sharepointFile.DeleteFolder(TrainingReportDocumentType.Certificate + "/" + id.ToString());
                }
                result = sharepointFile.UploadFiles(TrainingReportDocumentType.Material + "/" + id.ToString(), dto.TrainingReportAttachments.Material);
            }
            if (!string.IsNullOrEmpty(result))
            {
                //call delete report
                if (dto.Id == Guid.Empty)
                {
                    trainingReportService.Delete(id);
                }
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent(result)
                };
                return ResponseMessage(response);
            }

            return Ok(new { Id = id });
        }

        [HttpPost]
        public IHttpActionResult Submit(WorkflowActionDto dto)
        {
            var request = trainingReportService.Get(dto.RequestId);
            if (request == null) return NotFound();

            if (!IsAuthorized(request, WorkflowAction.Submit)) return Unauthorized();

            request.WorkflowData = workflowService.GetWorkflowTemplate(CommonKeys.WorkflowItemType, "report");

            workflowService.Submit(request, dto.Comment, CurrentUser);

            trainingReportService.UpdateParticipantStatus(request.TrainingInvitationId, CurrentUser.Id, ReportStatus.Submitted);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Approve(WorkflowActionDto dto)
        {
            var request = trainingReportService.Get(dto.RequestId);

            if (request == null) return NotFound();

            if (!IsAuthorized(request, WorkflowAction.Approve)) return Unauthorized();

            workflowService.Approve(request, dto.Comment, CurrentUser);
            trainingReportService.UpdateInvitation(request);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Reject(WorkflowActionDto dto)
        {
            var request = trainingReportService.Get(dto.RequestId);
            if (request == null) return NotFound();

            if (!IsAuthorized(request, WorkflowAction.Reject)) return Unauthorized();

            workflowService.Reject(request, dto.Comment, CurrentUser);
            trainingReportService.UpdateInvitation(request);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult RequestToChange(WorkflowActionDto dto)
        {
            var request = trainingReportService.Get(dto.RequestId);
            if (request == null) return NotFound();

            if (!IsAuthorized(request, WorkflowAction.RequestToChange)) return Unauthorized();

            workflowService.RequestToChange(request, dto.Comment, CurrentUser);

            trainingReportService.UpdateParticipantStatus(request.TrainingInvitationId, request.CreatedById, ReportStatus.NotSubmitted);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult ProgressingStage(Guid id)
        {
            var report = trainingReportService.ProgressingStage(id);
            if (report == null) return NotFound();
            return Ok(new
            {
                Histories = report.Histories.ToDto(),
                report.WorkflowData
            });
        }
        private bool IsAuthorized(TrainingReport request, string action)
        {
            var formActions = workflowService.GetWorkflowActions(CurrentUser, request);
            if (formActions == null || !formActions.Contains(action))
            {
                return false;
            }

            return true;
        }

        [HttpPost]
        public IHttpActionResult ListAllReport(TrainingReportFilter filter)
        {
            var request = trainingReportService.GetAllItem(CurrentUser, filter);
            if (request == null) return NotFound();
            return Ok(new
            {
                Count = request.TotalCount,
                Totalpages = request.TotalPages,
                Data = request.ToManagementDtos()
            });
        }
        [HttpPost]
        public IHttpActionResult ListMyReport(TrainingReportFilter filter)
        {
            filter.UserId = CurrentUser.Id;
            var request = trainingReportService.ListMyItem(filter);
            if (request == null) return NotFound();
            return Ok(new
            {
                Count = request.TotalCount,
                Totalpages = request.TotalPages,
                Data = request.ToManagementDtos()
            });
        }
    }
}