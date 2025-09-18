using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using Aeon.Academy.Services.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Aeon.Academy.Services
{
    public class TrainingReportService : ITrainingReportService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IGenericRepository<TrainingReport> repository = null;
        private readonly IUnitOfWork<EDocDbContext> eDocUnitOfWork = null;

        public TrainingReportService(IUnitOfWork<AppDbContext> unitOfWork, IUnitOfWork<EDocDbContext> eDocUnitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repository = unitOfWork.GetRepository<TrainingReport>();
            this.eDocUnitOfWork = eDocUnitOfWork;
        }

        public TrainingReport Get(Guid id)
        {
            return repository.Get(id);
        }
        public TrainingReport GetByInvitation(Guid invitationId, Guid? userId)
        {
            return repository.Query(q => q.TrainingInvitationId == invitationId && q.CreatedById == userId).FirstOrDefault();
        }

        public IList<TrainingReport> ListByUser(Guid userId)
        {
            return repository.Query(r => r.CreatedById.Value == userId).AsNoTracking().ToList();
        }
        public PagedList<TrainingReport> ListByDepartmentId(Guid userId, Guid departmentId, int pageNumber, int pageSize)
        {
            var query = repository.Query(r =>
                (r.AssignedUserId == null && r.AssignedDepartmentId.HasValue && r.AssignedDepartmentId == departmentId) ||
                (r.AssignedUserId != null && r.AssignedUserId.Contains(userId.ToString()))).OrderByDescending(r => r.Modified).AsNoTracking();
            return PagedList<TrainingReport>.ToPagedList(query, pageNumber, pageSize);
        }

        public PagedList<TrainingReport> ListByDepartmentIds(Guid userId, List<Guid> departmentIds, int pageNumber, int pageSize)
        {
            var query = repository.Query(r => r.Status == WorkflowStatus.Pending &&
                ((r.AssignedUserId == null && r.CreatedById != userId && r.AssignedDepartmentId.HasValue && departmentIds.Contains(r.AssignedDepartmentId.Value)) ||
                (r.AssignedUserId != null && r.AssignedUserId.Contains(userId.ToString()))))
                    .OrderByDescending(r => r.Modified).AsNoTracking();
            return PagedList<TrainingReport>.ToPagedList(query, pageNumber, pageSize);
        }
        public Guid Save(TrainingReport trainingReport)
        {
            var isAdd = false;
            if (trainingReport.Id == Guid.Empty)
            {
                isAdd = true;
                var referenceNumberSerivce = new ReferenceNumberService(eDocUnitOfWork);
                var referenceNumber = referenceNumberSerivce.GenerateReferenceNumber(CommonKeys.ReportReferenceNumber);
                trainingReport.ReferenceNumber = referenceNumber;
                repository.Add(trainingReport);
            }
            else
            {
                var repositorySurvey = unitOfWork.GetRepository<TrainingSurveyQuestion>();
                var repositoryActionPlan = unitOfWork.GetRepository<TrainingActionPlan>();
                var surveyQuestions = repositorySurvey.Query(a => a.TrainingReportId == trainingReport.Id).ToList();
                if (surveyQuestions != null)
                {
                    surveyQuestions.ForEach(x => repositorySurvey.Delete(x));
                }
                var actionPlans = repositoryActionPlan.Query(a => a.TrainingReportId == trainingReport.Id).ToList();
                if (actionPlans != null)
                {
                    actionPlans.ForEach(x => repositoryActionPlan.Delete(x));
                }
                trainingReport.TrainingSurveyQuestions.ToList().ForEach(x => repositorySurvey.Add(x));
                trainingReport.TrainingActionPlans.ToList().ForEach(x => repositoryActionPlan.Add(x));

                repository.Update(trainingReport);
            }

            unitOfWork.Complete();
            if (isAdd) eDocUnitOfWork.Complete();
            return trainingReport.Id;
        }
        public ProgressingStage<TrainingReportHistory> ProgressingStage(Guid id)
        {
            var progressingStage = new ProgressingStage<TrainingReportHistory>();
            var histories = unitOfWork.GetRepository<TrainingReportHistory>().Query(r => r.TrainingReportId == id)
                .OrderByDescending(r => r.Created).ToList();
            var request = Get(id);
            if (request == null) throw new ArgumentNullException(nameof(request));
            var workflow = CommonUtil.DeserializeWorkflow(request.WorkflowData);
            if (request.Status == WorkflowStatus.Pending)
            {
                var departmentRep = eDocUnitOfWork.GetRepository<Department>();
                var department = departmentRep.Query(d => d.Id == request.AssignedDepartmentId).FirstOrDefault();
                var latestHistory = histories.FirstOrDefault();
                var inProgress = new TrainingReportHistory
                {
                    TrainingReportId = request.Id,
                    Created = DateTimeOffset.MinValue,
                    ReferenceNumber = request.ReferenceNumber,
                    StepNumber = request.WorkflowStep,
                    AssignedToDepartmentName = department != null ? department.Name : string.Empty,
                    DueDate = request.DueDate,
                    RoundNumber = latestHistory.RoundNumber
                };
                DateTimeOffset? startDate = null;
                startDate = latestHistory != null ? latestHistory.Created : startDate;
                if (startDate == null && request.DueDate.HasValue)
                {
                    var currentStep = workflow.Steps.FirstOrDefault(s => s.StepNumber == request.WorkflowStep);
                    var dueDateNumber = currentStep != null ? currentStep.DueDateNumber : 7;
                    startDate = request.DueDate.Value.AddDays(-(dueDateNumber));
                }

                inProgress.StartDate = startDate ?? DateTimeOffset.Now;
                histories.Insert(0, inProgress);
            }

            progressingStage.Histories = histories;
            progressingStage.WorkflowData = workflow;
            return progressingStage;
        }
        public void UpdateParticipantStatus(Guid? invitationId, Guid? useId, string status)
        {
            if (invitationId == null || useId == null) return;
            var participantRep = unitOfWork.GetRepository<TrainingInvitationParticipant>();
            var participant = participantRep.Query(x => x.ParticipantId == useId && x.TrainingInvitationId == invitationId).FirstOrDefault();
            if (participant != null)
            {
                participant.StatusOfReport = status;
                participantRep.Update(participant);
                unitOfWork.Complete();
            }
            return;
        }
        public List<EdocTask<TrainingReport>> ListTasks()
        {
            var query = repository.Query(r => r.Status == WorkflowStatus.Pending && r.AssignedUserId != null && !r.AssignedUserId.Equals(string.Empty)).OrderByDescending(r => r.Modified).AsNoTracking().ToList();
            var repoUser = eDocUnitOfWork.GetRepository<User>();
            var listTask = new List<EdocTask<TrainingReport>>();
            foreach (var group in query.GroupBy(g => g.AssignedUserId))
            {
                var userIds = group.Key.Split(',');
                foreach (var userId in userIds)
                {
                    Guid guid;
                    if (Guid.TryParse(userId, out guid))
                    {
                        var user = repoUser.Query(u => u.Id == guid).FirstOrDefault();
                        var task = new EdocTask<TrainingReport>
                        {
                            User = new NotificationUser
                            {
                                UserId = user.Id,
                                UserFullName = user.FullName,
                                UserEmail = user.Email,
                                DepartmentId = (Guid)group.FirstOrDefault().DepartmentId,
                            },
                            Edoc2Tasks = ListTaskByUserId(user.Id),
                        };
                        listTask.Add(task);
                    }
                }
            }
            return listTask;
        }
        private List<TrainingReport> ListTaskByUserId(Guid userId)
        {
            return repository.Query(r => r.Status == WorkflowStatus.Pending && r.AssignedUserId != null && r.AssignedUserId.Contains(userId.ToString())).OrderByDescending(r => r.Modified).AsNoTracking().ToList() as List<TrainingReport>;
        }

        public void Delete(Guid id)
        {
            var repositorySurvey = unitOfWork.GetRepository<TrainingSurveyQuestion>();
            var repositoryActionPlan = unitOfWork.GetRepository<TrainingActionPlan>();
            var surveyQuestions = repositorySurvey.Query(a => a.TrainingReportId == id).ToList();
            if (surveyQuestions != null)
            {
                surveyQuestions.ForEach(x => repositorySurvey.Delete(x));
            }
            var actionPlans = repositoryActionPlan.Query(a => a.TrainingReportId == id).ToList();
            if (actionPlans != null)
            {
                actionPlans.ForEach(x => repositoryActionPlan.Delete(x));
            }
            var report = repository.Get(id);
            repository.Delete(report);
            unitOfWork.Complete();
            return;
        }
        public void UpdateInvitation(TrainingReport report)
        {
            var invitationService = new TrainingInvitationService(unitOfWork, eDocUnitOfWork);
            var participants = invitationService.GetParticipantAccepted(report.TrainingInvitationId);
            var totalReportResponse = repository.Query(x => x.TrainingInvitationId == report.TrainingInvitationId && (x.Status == WorkflowStatus.Completed || x.Status == WorkflowStatus.Rejected)).Count();
            if (participants.Count == totalReportResponse)
            {
                var invitation = invitationService.Get(report.TrainingInvitationId);
                if (invitation != null && invitation.Status == TrainingInvitationStatus.WaitingForAfterReport)
                {
                    invitation.Status = TrainingInvitationStatus.Completed;
                    unitOfWork.GetRepository<TrainingInvitation>().Update(invitation);
                    unitOfWork.Complete();
                }
            }
            return;
        }
        public PagedList<TrainingReport> ListByCurrentDepartment(TrainingReportFilter filter)
        {
            var departmentMappingbyUser = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(d => d.UserId == filter.UserId).AsNoTracking().ToList();
            var deptList = new List<Guid>();
            foreach (var department in departmentMappingbyUser)
            {
                deptList.AddRange(GetDepartmentTree(department.DepartmentId));
            }
            var userList = new List<Guid?>();
            foreach (var dept in deptList)
            {
                var userIds = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(d => d.DepartmentId == dept).Select(x => x.UserId).ToList();
                userList.AddRange(userIds);
            }
            var report = repository.Query(x => userList.Contains(x.CreatedById) && x.Status != WorkflowStatus.Draft && (!filter.Status.Any() || filter.Status.Any(r => r.ToUpper().Contains(x.Status.ToUpper()))) && x.CreatedByFullName.Contains(filter.CreatedBy)
                && x.SapCode.Contains(filter.SapCode) && x.TrainingInvitation.CourseName.Contains(filter.CourseName)).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
            return PagedList<TrainingReport>.ToPagedList(report, filter.PageNumber.Value, filter.PageSize.Value);
        }
        public PagedList<TrainingReport> ListMyItem(TrainingReportFilter filter)
        {
            var report = repository.Query(x => x.CreatedById == filter.UserId && (!filter.Status.Any() || filter.Status.Any(r => r.ToUpper().Contains(x.Status.ToUpper()))) && x.CreatedByFullName.Contains(filter.CreatedBy)
                && x.SapCode.Contains(filter.SapCode) && x.TrainingInvitation.CourseName.Contains(filter.CourseName)).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
            return PagedList<TrainingReport>.ToPagedList(report, filter.PageNumber.Value, filter.PageSize.Value);
        }

        public PagedList<TrainingReport> GetAllItem(User currentUser, TrainingReportFilter filter)
        {
            // Condition with get all data
            bool isSadmin = currentUser.LoginName.ToLower().Equals("sadmin");
            bool isHeadCountOfAcademy = false;

            // Query department
            var userDepartmentMappings = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(x => x.UserId == currentUser.Id).ToList();
            var deptHeadcount = userDepartmentMappings.Where(x => x.IsHeadCount).ToList().FirstOrDefault();
            if (deptHeadcount != null)
            {
                var unitDept = eDocUnitOfWork.GetRepository<Department>().Query(y => y.Id == deptHeadcount.DepartmentId).FirstOrDefault();
                if (unitDept != null && unitDept.IsAcademy.HasValue && unitDept.IsAcademy.Value) isHeadCountOfAcademy = true;
                // kiem tra xem department cha xem
                if (!isHeadCountOfAcademy)
                    isHeadCountOfAcademy = Utilities.HasParentDepartmentIsAcademy(eDocUnitOfWork, unitDept);
            }

            var report = (dynamic)null;
            if (isHeadCountOfAcademy || isSadmin)
            {
                report = repository.Query(x => /*userList.Contains(x.CreatedById) &&*/ x.Status != WorkflowStatus.Draft && (!filter.Status.Any() || filter.Status.Any(r => r.ToUpper().Contains(x.Status.ToUpper()))) && x.CreatedByFullName.Contains(filter.CreatedBy)
                && x.SapCode.Contains(filter.SapCode) && x.TrainingInvitation.CourseName.Contains(filter.CourseName)).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
            }
            else
            {
                if (userDepartmentMappings.Any())
                {
                    var userList = new List<Guid?>();
                    var deptList = new List<Guid>();
                    foreach (var departmentId in userDepartmentMappings)
                    {
                        deptList.AddRange(GetDepartmentTree(departmentId.DepartmentId.Value));
                    }
                    foreach (var dept in deptList)
                    {
                        var userIds = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(d => d.DepartmentId == dept).Select(x => x.UserId).ToList();
                        userList.AddRange(userIds);
                    }
                    report = repository.Query(x => userList.Contains(x.CreatedById) && x.Status != WorkflowStatus.Draft && (!filter.Status.Any() || filter.Status.Any(r => r.ToUpper().Contains(x.Status.ToUpper()))) && x.CreatedByFullName.Contains(filter.CreatedBy)
                    && x.SapCode.Contains(filter.SapCode) && x.TrainingInvitation.CourseName.Contains(filter.CourseName)).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
                }
            }

            return PagedList<TrainingReport>.ToPagedList(report, filter.PageNumber.Value, filter.PageSize.Value);
        }

        public List<Guid> GetDepartmentTree(Guid? id)
        {
            var list = new List<Guid>();
            if(id != null)
            {
                list.Add(id.Value);
                var deptList = eDocUnitOfWork.GetRepository<Department>().GetAll().ToList();
                ListTree(id.Value, deptList, list);
            }
            return list;
        }
        private void ListTree(Guid id, List<Department> categories, List<Guid> list)
        {
            var childIds = categories.Where(x => x.ParentId == id).Select(x => x.Id);

            if (childIds != null && childIds.Any())
            {
                list.AddRange(childIds);
                foreach (var childId in childIds)
                {
                    ListTree(childId, categories, list);
                }
            }
        }
    }
}
