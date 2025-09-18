using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Common.Workflow;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Academy.Services
{
    public class WorkflowService<T> : IWorkflowService<T> where T : BaseWorkflowEntity
    {
        private readonly IUnitOfWork<EDocDbContext> eDocUnitOfWork = null;
        private readonly IUnitOfWork<AppDbContext> appUnitOfWork = null;
        private readonly IUserService userService = null;

        public WorkflowService(IUnitOfWork<EDocDbContext> eDocUnitOfWork, IUnitOfWork<AppDbContext> appUnitOfWork, IUserService userService)
        {
            this.eDocUnitOfWork = eDocUnitOfWork;
            this.appUnitOfWork = appUnitOfWork;
            this.userService = userService;
        }

        public string GetWorkflowTemplate(string itemType, string name = "")
        {
            IGenericRepository<WorkflowTemplate> workflowTemplateRepository = eDocUnitOfWork.GetRepository<WorkflowTemplate>();

            var workflowTemplate = workflowTemplateRepository.Query(t => t.ItemType == itemType && t.WorkflowName.ToLower().Contains(name)).FirstOrDefault();
            if (workflowTemplate == null || string.IsNullOrEmpty(name)) throw new NotImplementedException($"Cannot find any workflow for {CommonKeys.WorkflowItemType}.");

            return workflowTemplate.WorkflowDataStr;
        }

        public List<string> GetWorkflowActions(User currentUser, T request)
        {
            if (request.Status == WorkflowStatus.Draft)
            {
                if (currentUser.Id == request.CreatedById)
                {
                    return new List<string> { WorkflowAction.Save, WorkflowAction.Submit, WorkflowAction.Cancel };
                }
                throw new UnauthorizedAccessException();
            }

            if (request.Status == WorkflowStatus.Rejected || request.Status == WorkflowStatus.Cancelled) return new List<string>();

            if (request.Status == WorkflowStatus.RequestedToChange)
            {
                return currentUser.Id == request.CreatedById ? new List<string> { WorkflowAction.Save, WorkflowAction.Submit, WorkflowAction.Cancel } : new List<string>();
            }

            if (request.Status == WorkflowStatus.Pending)
            {
                var arrActions = new List<string> {
                    WorkflowAction.Approve,
                    WorkflowAction.Reject,
                    WorkflowAction.RequestToChange
                };
                if (request is TrainingReport)
                {
                    arrActions.Remove(WorkflowAction.Reject);
                }
                if (request is TrainingRequest && currentUser.Id == request.CreatedById)
                {
                    var req = request as TrainingRequest;
                    if (req.TypeOfTraining.Equals(TrainingType.Internal, StringComparison.OrdinalIgnoreCase))
                    {
                        return new List<string>() { WorkflowAction.Cancel };
                    }
                }
                if (request.AssignedUserId == null && request.AssignedDepartmentId == null)
                    throw new ApplicationException("Cannot find assigned user or department for this request");

                if (request.AssignedUserId != null && request.AssignedUserId.Contains(currentUser.Id.ToString()))
                    return arrActions;

                var departmentMapRepo = eDocUnitOfWork.GetRepository<UserDepartmentMapping>();
                var currentUserDepartmentIds = departmentMapRepo.Query(m => m.UserId == currentUser.Id)
                    .Select(m => m.DepartmentId).Distinct().ToList();

                if (request.AssignedUserId == null &&
                    request.AssignedDepartmentId != null && currentUserDepartmentIds.Contains(request.AssignedDepartmentId.Value))
                    return arrActions;
            }
            return new List<string>();
        }

        public void Submit(T request, string comment = "", User user = null)
        {
            var workflow = ValidateWorkflow(request);

            if (!CanStartWorkflow(request, workflow))
            {
                throw new ApplicationException($"Cannot start workflow of request {request.Id}.");
            }

            MoveWorkflowToNextStep(request, user, WorkflowStatus.Submitted, comment);
        }

        public void Approve(T request, string comment = "", User user = null)
        {
            MoveWorkflowToNextStep(request, user, WorkflowStatus.Approved, comment);
        }

        public void RequestToChange(T request, string comment = "", User user = null)
        {
            int? step = request.WorkflowStep;
            request.WorkflowStep = null;
            request.AssignedDepartmentId = null;
            request.AssignedUserId = null;
            request.Status = WorkflowStatus.RequestedToChange;
            var deptName = request.AssignedDepartmentName;
            request.AssignedDepartmentName = null;
            UpdateRequestAndHistory(request, request.Status, comment, user, step, deptName);
        }

        public void Reject(T request, string comment = "", User user = null)
        {
            request.Status = WorkflowStatus.Rejected;

            UpdateRequestAndHistory(request, request.Status, comment, user, request.WorkflowStep, request.AssignedDepartmentName);
        }

        public void Cancel(T request, string comment = "", User user = null)
        {
            if(request.Status == WorkflowStatus.RequestedToChange)
            {
                request.WorkflowStep = 0;
            }
            request.Status = WorkflowStatus.Cancelled;

            UpdateRequestAndHistory(request, request.Status, comment, user, request.WorkflowStep, request.AssignedDepartmentName);
        }
        private void MoveWorkflowToNextStep(T request, User user, string status = "", string comment = "")
        {
            var logger = new Logger();
            logger.LogInfo($"MoveWorkflowToNextStep started - RequestId: {request?.Id}, UserId: {user?.Id}, CurrentStatus: {status}");

            try
            {
                if (string.IsNullOrEmpty(status)) status = WorkflowStatus.Submitted;

                var workflow = ValidateWorkflow(request);
                var trainingType = string.Empty;
                var deptNameToHistory = string.Empty;

                if (request is TrainingRequest)
                {
                    var step0 = workflow.Steps.FirstOrDefault(x => x.StepNumber == 0);
                    if (step0 != null)
                        workflow.Steps.Remove(step0);
                    var req = request as TrainingRequest;
                    trainingType = req.TypeOfTraining;
                    deptNameToHistory = string.IsNullOrEmpty(req.AssignedDepartmentName) ? req.DepartmentName : req.AssignedDepartmentName;
                }

                if (request is TrainingReport)
                {
                    var req = request as TrainingReport;
                    deptNameToHistory = string.IsNullOrEmpty(req.AssignedDepartmentName) ? req.DepartmentName : req.AssignedDepartmentName;
                }

                var currentStepIndex = request.WorkflowStep.HasValue ? request.WorkflowStep.Value : 0;
                logger.LogInfo($"CurrentStepIndex: {currentStepIndex}");

                if (currentStepIndex <= workflow.Steps.Count)
                {
                    currentStepIndex++;
                    request.WorkflowStep = currentStepIndex;
                    var skipNextStep = false;
                    var nextStep = workflow.Steps.FirstOrDefault(s => s.StepNumber == currentStepIndex);
                    logger.LogInfo($"NextStep found: {nextStep?.StepName}");

                    if (nextStep != null)
                    {
                        request.Status = WorkflowStatus.Pending;
                        request.DueDate = DateTimeOffset.Now.AddDays(nextStep.DueDateNumber);

                        if (nextStep.ParticipantType == CommonKeys.CurrentDepartment)
                        {
                            Guid departmentId = Guid.Empty;
                            string userId = string.Empty;

                            var trainingReport = request as TrainingReport;
                            if (trainingReport != null)
                                departmentId = trainingReport.DepartmentId.Value;

                            var departmentRepo = eDocUnitOfWork.GetRepository<Department>();
                            var department = departmentRepo.Get(departmentId);
                            request.AssignedDepartmentId = department != null ? department.Id : Guid.Empty;
                            request.AssignedDepartmentName = department != null ? department.Name : string.Empty;
                            request.AssignedDepartmentPosition = department != null ? department.PositionName : string.Empty;
                            request.AssignedDepartmentGroup = (int)nextStep.DepartmentType;
                            userId = FindAssignedUserIdInDepartment(request.AssignedDepartmentId.Value, nextStep.DepartmentType);
                            request.AssignedUserId = userId;
                            logger.LogInfo($"Assigned to current department: {department?.Name}, AssignedUserId: {userId}");

                            if (userId == user.Id.ToString())
                            {
                                skipNextStep = true;
                                logger.LogInfo("SkipNextStep: current user is assigned user.");
                            }
                        }
                        else
                        {
                            if (trainingType.Equals(TrainingType.External, StringComparison.OrdinalIgnoreCase) && request is TrainingRequest)
                            {
                                request.AssignedDepartmentId = null;
                                request.AssignedDepartmentName = string.Empty;
                                request.AssignedUserId = string.Empty;
                                request.AssignedDepartmentPosition = "F2";
                                request.AssignedDepartmentGroup = null;
                                logger.LogInfo("Assigned to F2 (external training).");
                            }
                            else
                            {
                                string assignedUserId = string.Empty;
                                Department department = null;

                                if (request is TrainingRequest && nextStep.ParticipantType == CommonKeys.UpperDepartment)
                                {
                                    department = FindUpperDepartmentFromParticipant(request as TrainingRequest, workflow, nextStep, out assignedUserId);
                                }
                                else if (request is TrainingReport && nextStep.ParticipantType == CommonKeys.UpperDepartment)
                                {
                                    department = FindManagerTrainingReport(request as TrainingReport, out assignedUserId);
                                }
                                else
                                {
                                    department = FindADepartmentDiffFromCurrent(workflow, request.WorkflowStep.Value, nextStep, out assignedUserId);
                                }

                                request.AssignedDepartmentId = department.Id;
                                request.AssignedDepartmentName = department.Name;
                                request.AssignedUserId = assignedUserId;
                                request.AssignedDepartmentPosition = department.PositionName;
                                request.AssignedDepartmentGroup = (int)nextStep.DepartmentType;
                                logger.LogInfo($"Assigned to Dept: {department.Name}, AssignedUserId: {assignedUserId}");

                                if (assignedUserId.Contains(user.Id.ToString().ToLower()))
                                {
                                    skipNextStep = true;
                                    logger.LogInfo("SkipNextStep: current user is assigned user.");
                                }
                            }
                        }
                    }
                    else
                    {
                        request.Status = WorkflowStatus.Completed;
                        status = WorkflowStatus.Completed;
                        logger.LogInfo("Workflow completed. Assigning to Checker.");
                        AssignToChecker(request);
                    }

                    UpdateRequestAndHistory(request, status, comment, user, currentStepIndex - 1, deptNameToHistory);
                    logger.LogInfo("Request and history updated.");

                    if (skipNextStep)
                    {
                        var reponsitory = appUnitOfWork.GetRepository<T>();
                        var req = reponsitory.Get(request.Id);
                        logger.LogInfo("Skipping next step and calling recursively with Approved status.");
                        MoveWorkflowToNextStep(req, user, WorkflowStatus.Approved, comment);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in MoveWorkflowToNextStep - RequestId: {request?.Id}, Exception: {ex}");
                throw;
            }
        }


        private bool CanStartWorkflow(T request, WorkflowData workflow)
        {
            var canStart = true;

            var condition = workflow.StartWorkflowConditions.FirstOrDefault(c => c.FieldName == "Status");
            if (condition == null || condition.FieldValues == null || condition.FieldValues.Count == 0)
                return canStart;

            canStart = condition.FieldValues.Any(v => string.Equals(v, request.Status, StringComparison.OrdinalIgnoreCase));

            //TODO: check other conditions

            return canStart;
        }

        private WorkflowData ValidateWorkflow(T request)
        {
            var logger = new Logger();

            logger.LogInfo($"ValidateWorkflow started - RequestId: {request?.Id}");

            if (request == null)
            {
                logger.LogError("ValidateWorkflow failed - Request is null");
                throw new ArgumentNullException(nameof(request));
            }

            var workflow = CommonUtil.DeserializeWorkflow(request.WorkflowData);

            if (workflow == null)
            {
                logger.LogError($"ValidateWorkflow failed - Cannot deserialize workflow for RequestId: {request.Id}");
                throw new ApplicationException($"Cannot read workflow of request {request.Id}.");
            }

            logger.LogInfo($"ValidateWorkflow succeeded - RequestId: {request.Id}");

            return workflow;
        }


        private Department FindADepartmentDiffFromCurrent(WorkflowData workflow, int nextStepIndex, WorkflowStep nextStep, out string assignedUserId)
        {
            var logger = new Logger();
            logger.LogInfo($"FindADepartmentDiffFromCurrent started - NextStepIndex: {nextStepIndex}, StepName: {nextStep?.StepName}");

            assignedUserId = string.Empty;

            try
            {
                Department department = null;

                if (workflow.StartWorkflowConditions != null)
                {
                    var stepCondition = workflow.StartWorkflowConditions.FirstOrDefault(c => string.Equals(c.FieldName, $"{nextStepIndex}-Department"));
                    logger.LogInfo($"StepCondition for '{nextStepIndex}-Department' {(stepCondition != null ? "found" : "not found")}");

                    if (stepCondition != null)
                    {
                        var tokens = stepCondition.FieldValues.First().Split(new char[] { '-' });
                        var departmentCode = tokens.Length > 0 ? tokens[tokens.Length - 1] : string.Empty;

                        logger.LogInfo($"Parsed DepartmentCode: {departmentCode}");

                        var departmentRepository = eDocUnitOfWork.GetRepository<Department>();
                        department = departmentRepository.Query(d => d.Code.Equals(departmentCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                        logger.LogInfo(department != null
                            ? $"Department found: {department.Name} (ID: {department.Id})"
                            : "Department not found by code.");
                    }
                }

                if (department == null)
                {
                    logger.LogError($"Cannot find department to assign task for step {nextStepIndex}");
                    throw new ApplicationException($"Cannot find department to assign task for step {nextStepIndex}.");
                }

                assignedUserId = FindAssignedUserIdInDepartment(department.Id, nextStep.DepartmentType);
                logger.LogInfo($"AssignedUserId: {assignedUserId}");

                return department;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in FindADepartmentDiffFromCurrent - StepIndex: {nextStepIndex}, Exception: {ex}");
                throw;
            }
        }


        private Department FindUpperDepartmentFromParticipant(TrainingRequest request, WorkflowData workflow, WorkflowStep nextStep, out string assignedUserId)
        {
            var logger = new Logger();
            logger.LogInfo($"FindUpperDepartmentFromParticipant started - RequestId: {request.Id}, WorkflowStep: {request.WorkflowStep.Value}, NextStep: {nextStep?.StepName}");

            assignedUserId = string.Empty;

            Department department = null;

            if (workflow.StartWorkflowConditions != null)
            {
                department = GetDepartmentbyMaxJobGrade(request, nextStep, out assignedUserId);

                if (department == null)
                {
                    logger.LogError($"Cannot find department to assign task for step {request.WorkflowStep.Value}");
                    throw new ApplicationException($"Cannot find department to assign task for step {request.WorkflowStep.Value}.");
                }

                logger.LogInfo($"Department found - DepartmentId: {department.Id}, AssignedUserId: {assignedUserId}");
                return department;
            }

            if (department == null)
            {
                logger.LogError($"Cannot find department to assign task for step {request.WorkflowStep.Value}");
                throw new ApplicationException($"Cannot find department to assign task for step {request.WorkflowStep.Value}.");
            }

            assignedUserId = FindAssignedUserIdInDepartment(department.Id, Group.All);
            logger.LogInfo($"AssignedUserId: {assignedUserId}");

            return department;
        }

        private Department GetDepartmentbyMaxJobGrade(TrainingRequest request, WorkflowStep nextStep, out string assignedUserId)
        {
            assignedUserId = string.Empty;
            Department department = null;
            var participants = request.TrainingRequestParticipants;
            var maxJobgradePosition = participants.Select(p => p.Position).FirstOrDefault();
            int maxJobgrade = int.Parse(maxJobgradePosition.Replace("G", ""));
            foreach (var p in participants)
            {
                if (!string.IsNullOrEmpty(p.Position))
                {
                    int gradePosition = int.Parse(p.Position.Replace("G", ""));
                    if (maxJobgrade < gradePosition)
                    {
                        maxJobgrade = gradePosition;
                    }
                }
            }
            var list = participants.Where(p => !string.IsNullOrEmpty(p.Position) && int.Parse(p.Position.Replace("G","")) == maxJobgrade).ToList();

            department = GetDepartmentToWorkflow(request, list, maxJobgrade, nextStep);
            if (department != null)
            {
                assignedUserId = FindAssignedUserIdInDepartment(department.Id, Group.All);
                if (!string.IsNullOrEmpty(assignedUserId))
                    return department;

                if (string.IsNullOrEmpty(assignedUserId))
                {
                    department = GetDepartmentHasAssignedUser(department, out assignedUserId);
                    return department;
                }
            }

            return null;
        }
        private Department GetDepartmentToWorkflow(TrainingRequest request, List<TrainingRequestParticipant> participantsWithMaxGrade, int maxJobgrade, WorkflowStep nextStep)
        {
            var departmentRepo = eDocUnitOfWork.GetRepository<Department>();
            var jobGradeRepo = eDocUnitOfWork.GetRepository<JobGrade>();
            Department departmentNote = null;
            foreach (var participant in participantsWithMaxGrade)
            {
                var participantJobGrade = jobGradeRepo.Query(j => j.Title.Equals(participant.Position, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                departmentNote = departmentRepo.Query(x => x.Name.Equals(participant.Department, StringComparison.OrdinalIgnoreCase) && x.JobGradeId == participantJobGrade.Id).FirstOrDefault();
                if (departmentNote != null)
                    break;
            }
            if (departmentNote == null)
            {
                foreach (var participant in request.TrainingRequestParticipants)
                {
                    var participantJobGrade = jobGradeRepo.Query(j => j.Title.Equals(participant.Position, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    departmentNote = departmentRepo.Query(x => x.Name.Equals(participant.Department, StringComparison.OrdinalIgnoreCase) && x.JobGradeId == participantJobGrade.Id).FirstOrDefault();
                    if (departmentNote != null && departmentNote.Id != null && departmentNote.Id != Guid.Empty)
                        break;
                }
            }
            if (departmentNote != null)
            {
                if (maxJobgrade < nextStep.JobGrade.Value)
                {
                    maxJobgrade = nextStep.JobGrade.Value;
                }
                else
                {
                    int grade = maxJobgrade;
                    /*int.TryParse(maxJobgrade.Substring(1), out grade);*/
                    grade++;
                    /*maxJobgrade = "g" + grade;*/
                    maxJobgrade = grade;
                }
                return GetDepartmentBaseOnNote(departmentNote, maxJobgrade);
            }
            return null;
        }
        public Department GetDepartmentBaseOnNote(Department department, int maxJobgrade)
        {
            var jobGradeRepo = eDocUnitOfWork.GetRepository<JobGrade>();
            if (department != null)
            {
                var skip = false;
                var dept = department;
                var deptJobgrade = jobGradeRepo.Get(dept.JobGradeId);
                if (maxJobgrade <= deptJobgrade.Grade)
                {
                    return dept;
                }
                var departmentIdx = department.ParentId;
                while (!skip)
                {
                    if (departmentIdx.HasValue)
                    {
                        dept = eDocUnitOfWork.GetRepository<Department>().Get(departmentIdx.Value);
                        deptJobgrade = jobGradeRepo.Get(dept.JobGradeId);
                        if (deptJobgrade.Grade >= maxJobgrade)
                        {
                            return dept;
                        }
                        else
                            departmentIdx = dept.ParentId;
                    }
                    else
                    {
                        return dept;
                    }
                }
            }
            return null;
        }
        private Department GetDepartmentHasAssignedUser(Department department, out string assignedUserId)
        {
            assignedUserId = string.Empty;
            if (department != null)
            {
                var skip = false;
                var dept = department;
                var departmentIdx = department.ParentId;
                while (!skip)
                {
                    if (departmentIdx.HasValue)
                    {
                        dept = eDocUnitOfWork.GetRepository<Department>().Get(departmentIdx.Value);
                        assignedUserId = FindAssignedUserIdInDepartment(dept.Id, Group.All);
                        if (!string.IsNullOrEmpty(assignedUserId))
                        {
                            return dept;
                        }
                        else
                        {
                            departmentIdx = dept.ParentId;
                        }
                    }
                    else
                    {
                        return dept;
                    }
                }
            }
            return null;
        }

        private string FindAssignedUserIdInDepartment(Guid departmentId, Group userType)
        {
            var logger = new Logger();
            logger.LogInfo($"FindAssignedUserIdInDepartment started - DepartmentId: {departmentId}, UserType: {userType}");

            try
            {
                var userDepartmentRepository = eDocUnitOfWork.GetRepository<UserDepartmentMapping>();
                var assignedUser = string.Empty;
                List<UserDepartmentMapping> mapping = new List<UserDepartmentMapping>();

                if (userType != Group.All)
                {
                    logger.LogInfo("Filtering by specific group role: " + userType);
                    mapping = userDepartmentRepository.Query(m => m.DepartmentId == departmentId && m.Role == (int)userType).ToList();
                }
                else
                {
                    logger.LogInfo("Filtering by Group.All (Checker, HOD, Member)");
                    mapping = userDepartmentRepository.Query(m =>
                        m.DepartmentId == departmentId &&
                        (m.Role == (int)Group.Checker || m.Role == (int)Group.HOD || m.Role == (int)Group.Member)).ToList();
                }

                foreach (var item in mapping)
                {
                    assignedUser = assignedUser + item.UserId.ToString() + ",";
                }

                logger.LogInfo($"FindAssignedUserIdInDepartment result: {assignedUser}");
                return assignedUser.ToLower();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in FindAssignedUserIdInDepartment - DepartmentId: {departmentId}, UserType: {userType}, Exception: {ex}");
                throw;
            }
        }


        private void UpdateRequestAndHistory(T request, string action, string comment, User user, int? stepNumber, string departmentName = null)
        {
            var logger = new Logger();
            logger.LogInfo($"UpdateRequestAndHistory started - RequestId: {request?.Id}, Action: {action}, StepNumber: {stepNumber}");

            try
            {
                var requestRepository = appUnitOfWork.GetRepository<T>();

                if (request is TrainingReport && stepNumber == 1)
                {
                    var trainingReport = request as TrainingReport;
                    trainingReport.Remark = comment;
                    request = trainingReport as T;
                    logger.LogInfo("Request is TrainingReport at step 1 - updated remark.");
                }

                requestRepository.Update(request);
                logger.LogInfo("Request updated in repository.");

                var startDate = DateTimeOffset.Now;
                int? roundNumber = 1;
                if (string.IsNullOrEmpty(departmentName))
                    departmentName = string.Empty;

                if (request is TrainingRequest)
                {
                    logger.LogInfo("Processing as TrainingRequest...");

                    var requestHistoryRepository = appUnitOfWork.GetRepository<TrainingRequestHistory>();
                    var histories = requestHistoryRepository.Query(x => x.TrainingRequestId == request.Id).OrderByDescending(o => o.Created);
                    var latestHistory = histories.FirstOrDefault();

                    if (latestHistory != null)
                        startDate = latestHistory.Created;

                    var requestToChanges = histories.Where(r => r.Action == WorkflowStatus.RequestedToChange).OrderByDescending(o => o.RoundNumber);
                    if (requestToChanges.Any())
                    {
                        roundNumber = requestToChanges.FirstOrDefault().RoundNumber;
                        roundNumber = roundNumber != null ? roundNumber + 1 : 1;
                    }

                    var workflow = ValidateWorkflow(request);
                    var currentStep = workflow.Steps.FirstOrDefault(s => s.StepNumber == stepNumber);
                    var dueDate = DateTimeOffset.Now;

                    if (currentStep != null)
                        dueDate = startDate.AddDays(currentStep.DueDateNumber);

                    var historyItem = new TrainingRequestHistory
                    {
                        CreatebBy = user != null ? user.LoginName : request.CreatedBy,
                        Created = DateTimeOffset.Now,
                        CreatedByFullName = user != null ? user.FullName : request.CreatedByFullName,
                        CreatedById = user != null ? user.Id : request.CreatedById.Value,
                        ReferenceNumber = request.ReferenceNumber,
                        TrainingRequestId = request.Id,
                        Action = action,
                        Comment = comment,
                        StepNumber = stepNumber,
                        StartDate = startDate,
                        DueDate = dueDate,
                        AssignedToDepartmentName = departmentName,
                        RoundNumber = roundNumber
                    };
                    requestHistoryRepository.Add(historyItem);
                    logger.LogInfo("TrainingRequestHistory added.");
                }
                else
                {
                    logger.LogInfo("Processing as TrainingReport...");

                    var reportHistoryRepository = appUnitOfWork.GetRepository<TrainingReportHistory>();
                    var histories = reportHistoryRepository.Query(x => x.TrainingReportId == request.Id).OrderByDescending(o => o.Created);
                    var latestHistory = histories.FirstOrDefault();

                    if (latestHistory != null)
                        startDate = latestHistory.Created;

                    var requestToChanges = histories.Where(r => r.Action == WorkflowStatus.RequestedToChange).OrderByDescending(o => o.RoundNumber);
                    if (requestToChanges.Any())
                    {
                        roundNumber = requestToChanges.FirstOrDefault().RoundNumber;
                        roundNumber = roundNumber != null ? roundNumber + 1 : 1;
                    }

                    var workflow = ValidateWorkflow(request);
                    var currentStep = workflow.Steps.FirstOrDefault(s => s.StepNumber == stepNumber);
                    var dueDate = DateTimeOffset.Now;

                    if (currentStep != null)
                        dueDate = startDate.AddDays(currentStep.DueDateNumber);

                    var historyItem = new TrainingReportHistory
                    {
                        CreatebBy = user != null ? user.LoginName : request.CreatedBy,
                        Created = DateTimeOffset.Now,
                        CreatedByFullName = user != null ? user.FullName : request.CreatedByFullName,
                        CreatedById = user != null ? user.Id : request.CreatedById.Value,
                        ReferenceNumber = request.ReferenceNumber,
                        TrainingReportId = request.Id,
                        Action = action,
                        Comment = comment,
                        StepNumber = stepNumber,
                        StartDate = startDate,
                        DueDate = dueDate,
                        AssignedToDepartmentName = departmentName,
                        RoundNumber = roundNumber
                    };
                    reportHistoryRepository.Add(historyItem);
                    logger.LogInfo("TrainingReportHistory added.");
                }

                appUnitOfWork.Complete();
                logger.LogInfo("UnitOfWork completed.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateRequestAndHistory - RequestId: {request?.Id}, Action: {action}, Exception: {ex}");
                throw;
            }
        }

        public void AssignToChecker(T request)
        {
            var logger = new Logger();
            logger.LogInfo($"AssignToChecker started - RequestId: {request?.Id}");

            try
            {
                if (request is TrainingRequest)
                {
                    var deptCode = ApplicationSettings.DeptAcademyCode;
                    logger.LogInfo($"DeptAcademyCode: {deptCode}");

                    var department = eDocUnitOfWork.GetRepository<Department>().Query(d => d.SAPCode == deptCode).FirstOrDefault();

                    if (department == null)
                    {
                        logger.LogError($"Department with SAPCode '{deptCode}' not found.");
                    }
                    else
                    {
                        logger.LogInfo($"Department found - Id: {department.Id}, Name: {department.Name}");

                        request.AssignedDepartmentId = department.Id;
                        request.AssignedDepartmentName = department.Name;
                        request.AssignedUserId = FindAssignedUserIdInDepartment(department.Id, Group.Checker);
                        request.AssignedDepartmentPosition = department.PositionName;
                        request.AssignedDepartmentGroup = (int)Group.Checker;

                        logger.LogInfo($"AssignedUserId: {request.AssignedUserId}, Group: Checker");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AssignToChecker - RequestId: {request?.Id}, Exception: {ex}");
                throw;
            }
        }

        private Department FindManagerTrainingReport(TrainingReport report, out string assignedUserId)
        {
            var logger = new Logger();
            logger.LogInfo($"FindManagerTrainingReport started - ReportId: {report?.Id}, DepartmentId: {report?.DepartmentId}");

            assignedUserId = string.Empty;

            try
            {
                var departmentRepo = eDocUnitOfWork.GetRepository<Department>();
                var jobGradeRepo = eDocUnitOfWork.GetRepository<JobGrade>();

                var departmentNote = departmentRepo.Query(d => d.Id == report.DepartmentId).FirstOrDefault();
                var employeeJobGrade = jobGradeRepo.Get(departmentNote.JobGradeId);

                int minGradeManager = 4;

                if (departmentNote != null && employeeJobGrade != null)
                {
                    logger.LogInfo($"Department: {departmentNote.Name}, Grade: {employeeJobGrade.Grade}, IsStore: {departmentNote.IsStore}");

                    if (employeeJobGrade.Grade == 9)
                    {
                        assignedUserId = FindAssignedUserIdInDepartment(departmentNote.Id, Group.All);
                        logger.LogInfo($"Grade = 9 → AssignedUserId: {assignedUserId}");
                        return departmentNote;
                    }

                    if (departmentNote.IsStore && employeeJobGrade.Grade >= 4)
                    {
                        minGradeManager = employeeJobGrade.Grade + 1;
                    }

                    if (!departmentNote.IsStore && employeeJobGrade.Grade < 5)
                    {
                        minGradeManager = 5;
                    }

                    if (!departmentNote.IsStore && employeeJobGrade.Grade >= 5)
                    {
                        minGradeManager = employeeJobGrade.Grade + 1;
                    }

                    var minJobGrade = minGradeManager;
                    var managerDept = GetDepartmentBaseOnNote(departmentNote, minJobGrade);

                    while (true)
                    {
                        if (managerDept != null)
                        {
                            assignedUserId = FindAssignedUserIdInDepartment(managerDept.Id, Group.All);
                            logger.LogInfo($"Trying with DeptId: {managerDept.Id}, AssignedUserId: {assignedUserId}");

                            if (!string.IsNullOrEmpty(assignedUserId))
                                return managerDept;

                            minGradeManager = jobGradeRepo.Query(j => j.Id == managerDept.JobGradeId).Select(j => j.Grade).FirstOrDefault();
                            minGradeManager++;

                            if (minGradeManager == 10)
                            {
                                if (managerDept == null)
                                {
                                    logger.LogError("ManagerDept is null at grade 10 - throwing exception.");
                                    throw new ApplicationException($"Cannot find department to assign task for next step.");
                                }

                                return null;
                            }

                            minJobGrade = minGradeManager;
                            managerDept = GetDepartmentBaseOnNote(managerDept, minJobGrade);
                        }
                        else
                        {
                            if (managerDept == null)
                            {
                                logger.LogError("ManagerDept is null - throwing exception.");
                                throw new ApplicationException($"Cannot find department to assign task for next step.");
                            }

                            return null;
                        }
                    }
                }

                logger.LogInfo("Department or JobGrade not found → returning null.");
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in FindManagerTrainingReport - ReportId: {report?.Id}, Exception: {ex}");
                throw;
            }
        }

    }
}
