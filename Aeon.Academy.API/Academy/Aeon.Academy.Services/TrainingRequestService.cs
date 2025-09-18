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
    public class TrainingRequestService : ITrainingRequestService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IUnitOfWork<EDocDbContext> eDocUnitOfWork = null;
        private readonly IGenericRepository<TrainingRequest> repository = null;

        public TrainingRequestService(IUnitOfWork<AppDbContext> unitOfWork, IUnitOfWork<EDocDbContext> eDocUnitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.eDocUnitOfWork = eDocUnitOfWork;
            this.repository = unitOfWork.GetRepository<TrainingRequest>();
        }

        public TrainingRequest Get(Guid id)
        {
            return repository.Get(id);
        }
        public PagedList<TrainingRequest> ListAll(int pageNumber, int pageSize)
        {
            var request = repository.GetAll().AsQueryable().AsNoTracking();
            return PagedList<TrainingRequest>.ToPagedList(request, pageNumber, pageSize);
        }

        public IList<TrainingRequest> GetByDepartment(Guid departmentId)
        {
            return repository.Query(r => r.DepartmentId != null && r.DepartmentId.HasValue && r.DepartmentId == departmentId)
                .OrderByDescending(r => r.Modified).AsNoTracking().ToList();
        }
        public TrainingRequestManagement ListByDepartment(Guid? departmentId, int pageNumber, int pageSize)
        {
            IQueryable<TrainingRequest> query = Enumerable.Empty<TrainingRequest>().AsQueryable();
            if (departmentId == null)
            {
                query = repository.Query(r => r.Status != WorkflowStatus.Draft && r.Status != WorkflowStatus.Rejected).OrderByDescending(r => r.Modified).AsNoTracking();
            }
            else
            {
                query = repository.Query(r => r.DepartmentId != null && r.DepartmentId.HasValue && r.DepartmentId == departmentId && r.Status != WorkflowStatus.Draft && r.Status != WorkflowStatus.Rejected)
                .OrderByDescending(r => r.Modified).AsNoTracking();
            }
            var count = query.Count();
            var allItems = query?.ToList();
            if (allItems == null)
            {
                return new TrainingRequestManagement();
            }
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var pendingItems = allItems.Where(x => x.Status == WorkflowStatus.Pending || x.Status == WorkflowStatus.RequestedToChange);
            var approvedItems = allItems.Where(x => x.Status == WorkflowStatus.Completed);
            var pendingAmount = pendingItems.Sum(s => s.TrainingFee);
            var approvedAmount = approvedItems.Sum(s => s.TrainingFee);
            return new TrainingRequestManagement
            {
                Count = count,
                Data = items,
                TotalPending = pendingItems.Count(),
                TotalApproved = approvedItems.Count(),
                TotalAmountApproved = approvedAmount,
                TotalAmountPendingApproval = pendingAmount
            };
        }
        public PagedList<TrainingRequest> ListByDepartmentId(Guid userId, Guid departmentId, int pageNumber, int pageSize)
        {
            var query = repository.Query(r =>
                (r.AssignedUserId == null && r.AssignedDepartmentId.HasValue && r.AssignedDepartmentId == departmentId) ||
                (r.AssignedUserId != null && r.AssignedUserId.Contains(userId.ToString()))).OrderByDescending(r => r.Modified).AsNoTracking();
            return PagedList<TrainingRequest>.ToPagedList(query, pageNumber, pageSize);
        }

        public PagedList<TrainingRequest> ListByDepartmentIds(Guid userId, List<Guid> departmentIds, int pageNumber, int pageSize)
        {
            var query = repository.Query(r => r.Status == WorkflowStatus.Pending &&
                ((r.AssignedUserId == null && r.CreatedById != userId && r.AssignedDepartmentId.HasValue && departmentIds.Contains(r.AssignedDepartmentId.Value)) ||
                (r.AssignedUserId != null && r.AssignedUserId.Contains(userId.ToString()))))
                    .OrderByDescending(r => r.Modified).AsNoTracking();
            var completedItems = repository.Query(r => r.Status == WorkflowStatus.Completed && r.AssignedUserId != null && r.AssignedUserId.Contains(userId.ToString()))
                    .OrderByDescending(r => r.Modified).AsNoTracking();
            var requestIds = completedItems.Select(x => x.Id);

            var invitationRepo = unitOfWork.GetRepository<TrainingInvitation>();
            var invitations = invitationRepo.Query(x => requestIds.Contains(x.TrainingRequestId) && x.Status == TrainingInvitationStatus.SendInvitation).AsNoTracking().Select(x => x.TrainingRequestId);

            var pendingCreateInvitations = invitations == null ? completedItems : completedItems.Where(x => !invitations.Contains(x.Id)).AsQueryable();
            query = query.Concat(pendingCreateInvitations).OrderByDescending(r => r.Modified).AsNoTracking();

            return PagedList<TrainingRequest>.ToPagedList(query, pageNumber, pageSize);
        }

        public IList<TrainingRequest> ListByUserId(Guid userId)
        {
            return repository.Query(r => r.CreatedById.HasValue && r.CreatedById.Value == userId).OrderByDescending(r => r.Modified).AsNoTracking().ToList();
        }
        public IList<TrainingRequest> ListByPending()
        {
            return repository.Query(r => !string.IsNullOrEmpty(r.F2ReferenceNumber) && r.Status == WorkflowStatus.Pending).OrderByDescending(r => r.Modified).ToList();
        }
        public Guid Save(TrainingRequest trainingRequest)
        {
            var isAdd = false;
            if (trainingRequest.Id == Guid.Empty)
            {
                isAdd = true;
                if (trainingRequest.DateOfSubmission == DateTime.MinValue)
                    trainingRequest.DateOfSubmission = DateTime.Now;
                if (trainingRequest.WorkingCommitmentFrom == DateTime.MinValue)
                    trainingRequest.WorkingCommitmentFrom = DateTime.Now;
                if (trainingRequest.WorkingCommitmentTo == DateTime.MinValue)
                    trainingRequest.WorkingCommitmentTo = DateTime.Now;
                var referenceNumberSerivce = new ReferenceNumberService(eDocUnitOfWork);
                var referenceNumber = referenceNumberSerivce.GenerateReferenceNumber(CommonKeys.ReferenceNumberType);
                trainingRequest.ReferenceNumber = referenceNumber;
                repository.Add(trainingRequest);
            }
            else
            {
                var repositoryDuration = unitOfWork.GetRepository<TrainingDurationItem>();
                var trainingDurationItems = repositoryDuration.Query(a => a.TrainingRequestId == trainingRequest.Id).ToList();
                if (trainingDurationItems != null)
                {
                    trainingDurationItems.ForEach(x => repositoryDuration.Delete(x));
                }
                var repositoryParticipant = unitOfWork.GetRepository<TrainingRequestParticipant>();
                var participantItems = repositoryParticipant.Query(a => a.TrainingRequestId == trainingRequest.Id).ToList();
                if (participantItems != null)
                {
                    participantItems.ForEach(x => repositoryParticipant.Delete(x));
                }
                var repositoryCostCenter = unitOfWork.GetRepository<TrainingRequestCostCenter>();
                var costCenterItems = repositoryCostCenter.Query(x => x.TrainingRequestId == trainingRequest.Id).ToList();
                if (costCenterItems != null)
                {
                    costCenterItems.ForEach(x => repositoryCostCenter.Delete(x));
                }
                trainingRequest.CostCenters.ToList().ForEach(x => repositoryCostCenter.Add(x));
                trainingRequest.TrainingDurationItems.ToList().ForEach(x => repositoryDuration.Add(x));
                trainingRequest.TrainingRequestParticipants.ToList().ForEach(x => repositoryParticipant.Add(x));
                repository.Update(trainingRequest);
            }

            unitOfWork.Complete();
            if (isAdd) eDocUnitOfWork.Complete();
            return trainingRequest.Id;
        }
        public ProgressingStage<TrainingRequestHistory> ProgressingStage(Guid id)
        {
            var progressingStage = new ProgressingStage<TrainingRequestHistory>();
            var histories = unitOfWork.GetRepository<TrainingRequestHistory>().Query(r => r.TrainingRequestId == id)
                .OrderByDescending(r => r.Created).ToList();
            var request = Get(id);
            if (request == null) throw new ArgumentNullException(nameof(request));
            var workflow = CommonUtil.DeserializeWorkflow(request.WorkflowData);
            if (request.Status == WorkflowStatus.Pending)
            {
                var departmentRep = eDocUnitOfWork.GetRepository<Department>();
                var department = departmentRep.Query(d => d.Id == request.AssignedDepartmentId).FirstOrDefault();
                var latestHistory = histories.FirstOrDefault();
                var inProgress = new TrainingRequestHistory
                {
                    TrainingRequestId = request.Id,
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
        public List<EdocTask<TrainingRequest>> ListTasks()
        {
            var query = repository.Query(r => r.Status == WorkflowStatus.Pending && r.AssignedUserId != null && !r.AssignedUserId.Equals(string.Empty)).OrderByDescending(r => r.Modified).AsNoTracking().ToList();
            var repoUser = eDocUnitOfWork.GetRepository<User>();
            var listTask = new List<EdocTask<TrainingRequest>>();
            foreach (var group in query.GroupBy(g => g.AssignedUserId))
            {
                var userIds = group.Key.Split(',');
                foreach(var userId in userIds)
                {
                    Guid guid;
                    if (Guid.TryParse(userId, out guid))
                    {
                        var user = repoUser.Query(u => u.Id == guid).FirstOrDefault();
                        var task = new EdocTask<TrainingRequest>
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
        private List<TrainingRequest> ListTaskByUserId(Guid userId)
        {
            return repository.Query(r => r.Status == WorkflowStatus.Pending && r.AssignedUserId != null && r.AssignedUserId.Contains(userId.ToString())).OrderByDescending(r => r.Modified).AsNoTracking().ToList() as List<TrainingRequest>;
        }

        public void Delete(Guid id)
        {
            var repositoryDuration = unitOfWork.GetRepository<TrainingDurationItem>();
            var trainingDurationItems = repositoryDuration.Query(a => a.TrainingRequestId == id).ToList();
            if (trainingDurationItems != null)
            {
                trainingDurationItems.ForEach(x => repositoryDuration.Delete(x));
            }
            var repositoryParticipant = unitOfWork.GetRepository<TrainingRequestParticipant>();
            var participantItems = repositoryParticipant.Query(a => a.TrainingRequestId == id).ToList();
            if (participantItems != null)
            {
                participantItems.ForEach(x => repositoryParticipant.Delete(x));
            }
            var request = repository.Get(id);
            repository.Delete(request);
            unitOfWork.Complete();
            return;
        }
        public string GetWorkflowName(TrainingRequest request, Guid userId)
        {
            if (request.TypeOfTraining.Equals(TrainingType.External, StringComparison.OrdinalIgnoreCase))
                {
                    return CommonKeys.ATRforExternal;
                }
                else
                {
                    var userService = new UserService(eDocUnitOfWork, null);
                    if (userService.IsAcademy(userId))
                        return CommonKeys.ATRforInternalAcaDemyDept;

                    var participants = request.TrainingRequestParticipants;
                    var maxJobgradePosition = participants.Select(p => p.Position).FirstOrDefault();
                    int maxJobgrade = int.Parse(maxJobgradePosition.Substring(1));
                    foreach (var p in participants)
                    {
                        if (!string.IsNullOrEmpty(p.Position))
                        {
                            int gradePosition = int.Parse(p.Position.Substring(1));
                            if (maxJobgrade < gradePosition)
                            {
                                maxJobgrade = gradePosition;
                            }
                        }
                    }
                    var participant = participants.Where(p => !string.IsNullOrEmpty(p.Position) && int.Parse(p.Position.Substring(1)) == maxJobgrade).FirstOrDefault();
                    var departmentRepo = eDocUnitOfWork.GetRepository<Department>();
                    var department = departmentRepo.Query(x => x.Name.Equals(participant.Department, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    int grade = maxJobgrade;
                    /*Int32.TryParse(maxJobgrade.Substring(1), out grade);*/
                    if (department.IsStore && grade < 4)
                        return CommonKeys.ATRforInternalG3DownStore;
                    if (department.IsStore && grade >= 4 && grade < 9)
                        return CommonKeys.ATRforInternalG4UpStore;
                    if (!department.IsStore && grade <= 4)
                        return CommonKeys.ATRforInternalG4DownHQ;
                    if (!department.IsStore && grade > 4 && grade < 9)
                        return CommonKeys.ATRforInternalG5UpHQ;
                    if (grade == 9)
                        return CommonKeys.ATRforInternalAcaDemyDept;
                }

            return null;
        }
        public PagedList<TrainingRequest> GetAllItem(User currentUser, TrainingRequestFilter filter)
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
                if (unitDept != null && unitDept.IsAcademy.HasValue && unitDept.IsAcademy.Value)
                    isHeadCountOfAcademy = true;
                // kiem tra xem department cha xem
                if (!isHeadCountOfAcademy)
                    isHeadCountOfAcademy = Utilities.HasParentDepartmentIsAcademy(eDocUnitOfWork, unitDept);
            }
            var request = (dynamic)null;
            if (isHeadCountOfAcademy || isSadmin)
            {
                request = repository.Query(x => ((!filter.Status.Any() || filter.Status.Any(r => r.ToUpper().Contains(x.Status.ToUpper()))) &&
                        (!filter.Type.Any() || filter.Type.Any(r => r.ToUpper().Contains(x.TypeOfTraining.ToUpper()))) &&
                        x.CourseName.ToLower().Contains(filter.CourseName) &&
                        (x.ReferenceNumber.ToLower().Contains(filter.Keyword) || x.DepartmentName.ToLower().Contains(filter.Keyword) ||
                        x.SapNo.ToLower().Contains(filter.Keyword) || x.CreatedByFullName.ToLower().Contains(filter.Keyword)) &&
                        x.Created >= filter.CreateDateFrom && x.Created <= filter.CreateDateTo)
                        ).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
            }
            else
            {
                var listDepartmentOfCurrentUser = userDepartmentMappings.Select(x => x.DepartmentId).ToList();
                if (listDepartmentOfCurrentUser.Any())
                {
                    var deptList = new List<Guid>();
                    foreach (var departmentId in listDepartmentOfCurrentUser)
                    {
                        deptList.AddRange(GetDepartmentTree(departmentId.Value));
                    }
                    request = repository.Query(x => ((!filter.Status.Any() || filter.Status.Any(r => r.ToUpper().Contains(x.Status.ToUpper()))) &&
                        (!filter.Type.Any() || filter.Type.Any(r => r.ToUpper().Contains(x.TypeOfTraining.ToUpper()))) &&
                        x.CourseName.ToLower().Contains(filter.CourseName) &&
                        (x.ReferenceNumber.ToLower().Contains(filter.Keyword) || x.DepartmentName.ToLower().Contains(filter.Keyword) ||
                        x.SapNo.ToLower().Contains(filter.Keyword) || x.CreatedByFullName.ToLower().Contains(filter.Keyword)) &&
                        x.Created >= filter.CreateDateFrom && x.Created <= filter.CreateDateTo)
                        && ((deptList.Contains(x.DepartmentId.Value)) || (x.TrainingRequestParticipants.Any(y => y.SapCode == currentUser.SapCode)))).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
                }
            }
            return PagedList<TrainingRequest>.ToPagedList(request, filter.PageNumber.Value, filter.PageSize.Value);
        }
        public PagedList<TrainingRequest> GetMyItem(TrainingRequestFilter filter)
        {
            var request = repository.Query(x => x.CreatedById == filter.UserId &&
                (!filter.Status.Any() || filter.Status.Any(r => r.ToUpper().Contains(x.Status.ToUpper()))) &&
                (!filter.Type.Any() || filter.Type.Any(r => r.ToUpper().Contains(x.TypeOfTraining.ToUpper()))) &&
                x.CourseName.ToLower().Contains(filter.CourseName) &&
                (x.ReferenceNumber.ToLower().Contains(filter.Keyword) || x.DepartmentName.ToLower().Contains(filter.Keyword) ||
                x.SapNo.ToLower().Contains(filter.Keyword) || x.CreatedByFullName.ToLower().Contains(filter.Keyword)) &&
                x.Created >= filter.CreateDateFrom && x.Created <= filter.CreateDateTo).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
            return PagedList<TrainingRequest>.ToPagedList(request, filter.PageNumber.Value, filter.PageSize.Value);
        }
        public IList<TrainingRequest> ListByExternalCompleted(DateTime date)
        {
            return repository.Query(r => !string.IsNullOrEmpty(r.F2ReferenceNumber) && r.Status == WorkflowStatus.Completed && 
                r.TypeOfTraining.Equals(TrainingType.External, StringComparison.OrdinalIgnoreCase) && r.Created > date).OrderByDescending(r => r.Modified).ToList();
        }
        public List<Guid> GetDepartmentTree(Guid? id)
        {
            var list = new List<Guid>();
            if (id != null)
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
