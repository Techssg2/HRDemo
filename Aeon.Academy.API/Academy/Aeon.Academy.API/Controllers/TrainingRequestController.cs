using Aeon.Academy.API.Core;
using Aeon.Academy.API.DTOs;
using Aeon.Academy.API.Filters;
using Aeon.Academy.API.Mappers;
using Aeon.Academy.API.Utils;
using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.IntegrationServices;
using Aeon.Academy.Services;
using Aeon.HR.ViewModels.Args;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Logger = Aeon.Academy.Services.Logger;

namespace Aeon.Academy.API.Controllers
{
    public class TrainingRequestController : BaseAuthApiController
    {
        private readonly ITrainingRequestService trainingRequestService;
        private readonly IWorkflowService<TrainingRequest> workflowService;
        private readonly SharepointFile sharepointFile;
        private readonly IEdoc1Service _edoc1Service;
        private readonly IReasonOfTrainingRequestService reasonService;

        public TrainingRequestController(ITrainingRequestService requestService, IWorkflowService<TrainingRequest> workflowService, IEdoc1Service edoc1Service, IReasonOfTrainingRequestService reasonService)
        {
            this.trainingRequestService = requestService;
            this.workflowService = workflowService;
            this.sharepointFile = new SharepointFile(DocumentLibraryName.TrainingRequest);
            this._edoc1Service = edoc1Service;
            this.reasonService = reasonService;
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            var request = trainingRequestService.Get(id);
            if (request == null) return NotFound();

            bool currentUserIdIncludeParticipant = false;
            if (request.TrainingRequestParticipants.Any() && request.CreatedById != CurrentUser.Id)
            {
                currentUserIdIncludeParticipant = request.TrainingRequestParticipants.Any(x => x.SapCode == CurrentUser.SapCode);
            }

            var formActions = new List<string>();
            if (!currentUserIdIncludeParticipant)
            {
                formActions = workflowService.GetWorkflowActions(CurrentUser, request);
            }
            var dto = request.ToDto();
            dto.EnabledActions = formActions;

            var attachments = sharepointFile.GetCourseDocument(request.Id.ToString());
            dto.TrainingDetails.Attachments = attachments;
            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult List(int pageNumber, int pageSize)
        {
            var request = trainingRequestService.ListAll(pageNumber, pageSize);
            if (request == null) return NotFound();
            return Ok(new
            {
                Count = request.TotalCount,
                Totalpages = request.TotalPages,
                Data = request.ToMyTaskDtos()
            });
        }

        [HttpGet]
        public IHttpActionResult GetByDepartment(Guid departmentId)
        {
            var request = trainingRequestService.GetByDepartment(departmentId);
            if (request == null) return NotFound();
            return Ok(request.ToDto());
        }
        [HttpGet]
        public IHttpActionResult ListByDepartment(Guid? departmentId, int? pageNumber, int? pageSize)
        {
            var request = trainingRequestService.ListByDepartment(departmentId, pageNumber == null ? 1 : pageNumber.Value, pageSize == null ? 20 : pageSize.Value);
            if (request == null) return NotFound();
            return Ok(new
            {
                totalPending = request.TotalPending,
                totalApproved = request.TotalApproved,
                totalAmountPendingApproval = request.TotalAmountPendingApproval,
                totalAmountApproved = request.TotalAmountApproved,
                count = request.Count,
                data = request.Data.ToManagementDtos()
            });
        }

        [HttpGet]
        public IHttpActionResult MyItems()
        {
            var myItems = trainingRequestService.ListByUserId(CurrentUser.Id);

            var dtos = myItems.ToMyItemDtos();

            return Ok(dtos);
        }

        [HttpPost]
        [AuthFilterByUserType]
        [ValidateModel]
        public IHttpActionResult Save(TrainingRequestDto dto)
        {
            var logger = new Logger(); // Bạn nên dùng ILogger<ControllerName> thông qua DI nếu có thể

            try
            {
                // Log model đầu vào
                logger.LogError("Model: " + Newtonsoft.Json.JsonConvert.SerializeObject(dto));

                var request = trainingRequestService.Get(dto.Id);
                request = dto.ToEntity(request, CurrentUser);

                if (!IsAuthorized(request, WorkflowAction.Save))
                    return Unauthorized();

                var id = trainingRequestService.Save(request);

                var result = sharepointFile.UploadFiles(id.ToString(), dto.TrainingDetails.Attachments);

                if (!string.IsNullOrEmpty(result))
                {
                    // Log lỗi upload
                    logger.LogError("Create F2 " + request.ReferenceNumber + " - Response: " + result);

                    // Nếu là tạo mới thì rollback
                    if (dto.Id == Guid.Empty)
                    {
                        try
                        {
                            trainingRequestService.Delete(id);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError("Rollback failed after SharePoint upload failure:" + ex);
                        }
                    }

                    var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                    {
                        Content = new StringContent(result)
                    };
                    return ResponseMessage(response);
                }

                return Ok(new { Id = id });
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected error in Save() - ReferenceNumber: " + ex);
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        public IHttpActionResult Submit(WorkflowActionDto dto)
        {
            var request = trainingRequestService.Get(dto.RequestId);
            if (request == null) return NotFound();

            if (!IsAuthorized(request, WorkflowAction.Submit)) return Unauthorized();
            if (request.TypeOfTraining.Equals(TrainingType.External, StringComparison.OrdinalIgnoreCase))
            {
                //var save = SaveCostCenter(request.Id);
                var requestnew = trainingRequestService.Get(dto.RequestId);
                var createF2 = CreateF2(requestnew);
                if (createF2 == null)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Create purchase request failed.")
                    };
                    return ResponseMessage(response);
                }
                if (createF2.Errors != null)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Create purchase request failed. Error: " + createF2.Errors)
                    };
                    return ResponseMessage(response);
                }
                request.F2ReferenceNumber = createF2.Data.ReferenceNumber;
                request.F2URL = createF2.Data.URL;
            }
            var message = trainingRequestService.GetWorkflowName(request, CurrentUser.Id);
            request.WorkflowData = workflowService.GetWorkflowTemplate(CommonKeys.WorkflowItemType, message);

            workflowService.Submit(request, dto.Comment, CurrentUser);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Approve(WorkflowActionDto dto)
        {
            var logger = new Logger();

            logger.LogInfo($"Approve started - RequestId: {dto.RequestId}, User: {CurrentUser?.SapCode}, Comment: {dto.Comment}");

            var request = trainingRequestService.Get(dto.RequestId);

            if (request == null)
            {
                logger.LogError($"Approve failed - Request not found for RequestId: {dto.RequestId}");
                return NotFound();
            }

            if (!IsAuthorized(request, WorkflowAction.Approve))
            {
                logger.LogError($"Approve failed - Unauthorized access - RequestId: {dto.RequestId}, User: {CurrentUser?.SapCode}");
                return Unauthorized();
            }

            logger.LogInfo($"Request authorized - Proceeding to approve - RequestId: {dto.RequestId}, Reference: {request.ReferenceNumber}");

            workflowService.Approve(request, dto.Comment, CurrentUser);

            logger.LogInfo($"Approve completed successfully - RequestId: {dto.RequestId}, Approved by: {CurrentUser?.SapCode}");

            return Ok();
        }


        [HttpPost]
        public IHttpActionResult Reject(WorkflowActionDto dto)
        {
            var request = trainingRequestService.Get(dto.RequestId);
            if (request == null) return NotFound();

            if (!IsAuthorized(request, WorkflowAction.Reject)) return Unauthorized();

            workflowService.Reject(request, dto.Comment, CurrentUser);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult RequestToChange(WorkflowActionDto dto)
        {
            var request = trainingRequestService.Get(dto.RequestId);
            if (request == null) return NotFound();

            if (!IsAuthorized(request, WorkflowAction.RequestToChange)) return Unauthorized();

            workflowService.RequestToChange(request, dto.Comment, CurrentUser);

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult ProgressingStage(Guid id)
        {
            var request = trainingRequestService.ProgressingStage(id);
            if (request == null) return NotFound();
            return Ok(new
            {
                Histories = request.Histories.ToDto(),
                request.WorkflowData
            });
        }

        private bool IsAuthorized(TrainingRequest request, string action)
        {
            var formActions = workflowService.GetWorkflowActions(CurrentUser, request);
            if (formActions == null || !formActions.Contains(action))
            {
                return false;
            }

            return true;
        }
        private CreateF2MBResponse CreateF2(TrainingRequest request)
        {
            var url = ApplicationSettings.AcademyTrainingRequestUrl;
            url = url.Replace(":id", request.Id.ToString());
            //var reason = reasonService.Get(new Guid(request.ReasonOfTrainingRequest));
            var model = new CreateF2MBRequest
            {
                Amount = 0,
                BudgetInformations = new List<BudgetInformationModel>(),
                ContractorCode = request.SupplierCode ?? string.Empty,
                EmployeeCode = request.SapNo,
                GoodService = request.CourseName,
                PurposeReason = request.ReasonOfTrainingRequest != null ? request.ReasonOfTrainingRequest : string.Empty,
                ContractorName = request.SupplierName,
                CurrencyJson = request.CurrencyJson,
                RequestedDepartmentId = request.RequestedDepartmentId,
                DepartmentInCharge = new DepartmentInChargeModel
                {
                    id = request.DepartmentInChargeId.HasValue ? request.DepartmentInChargeId.Value : Guid.Empty,
                    name = request.DepartmentInCharge,
                    dicCode = request.DepartmentInChargeCode,
                    code = request.DicDepartmentCode
                },
                MethodOfChoosingContractor = request.MethodOfChoosingContractor,
                OrderingDepartment = new DepartmentInChargeModel
                {
                    name = "AEON VIET NAM ACADEMY (G5)",
                    code = "50022367"
                },
                Year = request.Year.HasValue ? request.Year.Value : 1975,
                TheProposalFor = request.TheProposalFor,
                AcademyURL = url,
                ATRReferenceNumber = request.ReferenceNumber,
                Reference = request.Reference ?? string.Empty,
            };
            model.FromDate = request.From.HasValue ? request.From.Value.ToString("yyyyMM") : "";
            model.ToDate = request.To.HasValue ? request.To.Value.ToString("yyyyMM") : "";
            if (request.RequestedDepartmentCode != null)
            {
                model.RequestedDepartment = new
                {
                    name = request.RequestedDepartment,
                    code = request.RequestedDepartmentCode,
                    id = request.RequestedDepartmentId.HasValue ? request.RequestedDepartmentId.Value : Guid.Empty
                };
            }
            else
            {
                return new CreateF2MBResponse
                {
                    Errors = "Requested Department code is null!"
                };
            }

            if (request.CostCenters != null)
            {
                decimal amount = 0;
                foreach (var item in request.CostCenters)
                {
                    var itemAmount = item.Amount.HasValue ? item.Amount.Value : 0;
                    model.BudgetInformations.Add(new BudgetInformationModel
                    {
                        Amount = itemAmount,
                        BudgetCode = item.BudgetCode,
                        BudgetPlan = item.BudgetPlan,
                        CostCenterCode = item.CostCenterCode,
                        CurrentBudget = item.BudgetBalanced.HasValue ? item.BudgetBalanced.Value : 0,
                        TotalBudget = item.TotalBudget.HasValue ? item.TotalBudget.Value : 0,
                        VATPercent = item.VATPercentage.HasValue ? item.VATPercentage.Value : 0
                    });
                    amount = amount + itemAmount;
                }
                model.Amount = amount;
            }
            var response = _edoc1Service.CreateF2MB(model);
            var httpResponseResult = response.Content.ReadAsStringAsync().Result;
            var logger = new Logger();
            if (response.IsSuccessStatusCode)
            {
                var result = Common.Utils.CommonUtil.DeserializeObject<CreateF2MBResponse>(httpResponseResult);
                if (result.Errors != null)
                {
                    logger.LogError("Create F2 " + request.ReferenceNumber + "- Response: " + httpResponseResult);
                    logger.LogError("Model: " + Common.Utils.CommonUtil.SerializeObject(model));
                }
                return result;
            }
            else
            {
                logger.LogError(response.ReasonPhrase + ". " + response.StatusCode + ". " + httpResponseResult);
                logger.LogError("Model: " + Common.Utils.CommonUtil.SerializeObject(model));
            }
            return new CreateF2MBResponse
            {
                Errors = httpResponseResult
            };
        }
        [HttpPost]
        public IHttpActionResult GetAllRequest(TrainingRequestFilter filter)
        {
            var request = trainingRequestService.GetAllItem(CurrentUser, filter);
            if (request == null) return NotFound();
            return Ok(new
            {
                Count = request.TotalCount,
                Totalpages = request.TotalPages,
                Data = request.ToViewItemDtos()
            });
        }
        [HttpPost]
        public IHttpActionResult GetMyRequest(TrainingRequestFilter filter)
        {
            filter.UserId = CurrentUser.Id;
            var request = trainingRequestService.GetMyItem(filter);
            if (request == null) return NotFound();
            return Ok(new
            {
                Count = request.TotalCount,
                Totalpages = request.TotalPages,
                Data = request.ToViewItemDtos()
            });
        }
        [HttpPost]
        public IHttpActionResult Cancel(WorkflowActionDto dto)
        {
            var request = trainingRequestService.Get(dto.RequestId);

            if (request == null) return NotFound();

            if (!IsAuthorized(request, WorkflowAction.Cancel)) return Unauthorized();

            if (request.Status == WorkflowStatus.Draft)
            {
                request.Status = WorkflowStatus.Cancelled;
                trainingRequestService.Save(request);
                return Ok();
            }
            workflowService.Cancel(request, dto.Comment, CurrentUser);

            return Ok();
        }
        [HttpGet]
        public IHttpActionResult SaveCostCenter(Guid id)
        {
            var result = new TrainingRequestDto();
            try
            {
                var request = trainingRequestService.Get(id);
                if (request != null)
                {
                    result = request.ToDto();
                    if (result.CostCenters!=null)
                    {
                        foreach (var item in result.CostCenters)
                        {
                            var budgetItem = new BudgetPlanResponse();
                            var response = _edoc1Service.GetBudgetItem(request.Year.Value, request.DepartmentInChargeCode, item.CostCenterCode, item.BudgetCode, item.BudgetPlan);
                            if (response.IsSuccessStatusCode)
                            {
                                var resultx = response.Content.ReadAsStringAsync().Result;
                                var results = CommonUtil.DeserializeObject<BudgetPlanResponse>(resultx);
                                if (results != null)
                                {
                                    budgetItem.Refund = results.Refund;
                                    budgetItem.TransferOut = results.TransferOut;
                                    budgetItem.TransferIn = results.TransferIn;
                                    budgetItem.CurrentBudget = results.CurrentBudget;
                                    budgetItem.TotalBudget = results.TotalBudget;
                                    budgetItem.CostCenterCode = results.CostCenterCode;
                                    budgetItem.BudgetCode = results.BudgetCode;
                                    budgetItem.BudgetName = results.BudgetName;
                                    budgetItem.BudgetPlan = results.BudgetPlan;
                                }
                            }
                            //calculate BudgetBallance
                            if (budgetItem.BudgetCode != null)
                            {
                                if (budgetItem.TotalBudget > 0)
                                {
                                    var ballance = budgetItem.TotalBudget - budgetItem.CurrentBudget  - (budgetItem.TransferOut.HasValue ? (decimal)budgetItem.TransferOut : decimal.Zero)
                                        + (budgetItem.TransferIn.HasValue ? (decimal)budgetItem.TransferIn : decimal.Zero) + (budgetItem.Refund.HasValue ? (decimal)budgetItem.Refund : decimal.Zero);
                                    decimal exchangeRate = decimal.One;
                                    var currency = JsonConvert.DeserializeObject<CurrencyDto>(request.CurrencyJson);
                                    if (currency != null)
                                    {
                                        exchangeRate = (decimal)currency.AmountInVND;
                                    }
                                    var ballanced = ballance - item.Amount * exchangeRate;
                                    item.BudgetBalance = ballanced;
                                }
                                else
                                {
                                    var ballance = (budgetItem.TransferIn.HasValue ? (decimal)budgetItem.TransferIn : decimal.Zero) - budgetItem.CurrentBudget
                                        - (budgetItem.TransferOut.HasValue ? (decimal)budgetItem.TransferOut : decimal.Zero)
                                        + (budgetItem.Refund.HasValue ? (decimal)budgetItem.Refund : decimal.Zero);
                                    decimal exchangeRate = decimal.One;
                                    var currency = JsonConvert.DeserializeObject<CurrencyDto>(request.CurrencyJson);
                                    if (currency != null)
                                    {
                                        exchangeRate = (decimal)currency.AmountInVND;
                                    }
                                    var ballanced = ballance - item.Amount * exchangeRate;
                                    item.BudgetBalance = ballanced;
                                }
                                item.RemainingBalance = budgetItem.CurrentBudget;
                                item.TransferOut = budgetItem.TransferOut.HasValue ? (decimal)budgetItem.TransferOut : decimal.Zero;
                                item.TransferIn = budgetItem.TransferIn.HasValue ? (decimal)budgetItem.TransferIn : decimal.Zero;
                                item.Refund = budgetItem.Refund.HasValue ? (decimal)budgetItem.Refund : decimal.Zero;
                            }

                        }
                    }
                    request = result.ToEntity(request, CurrentUser);

                    if (!IsAuthorized(request, WorkflowAction.Save)) return Unauthorized();
                    var ids = trainingRequestService.Save(request);
                    var srequest = trainingRequestService.Get(id);
                }
                else { return Ok(); }
            }
            catch (Exception ex)
            {
                return Ok();
            }
            return Ok();
        }
    }
}