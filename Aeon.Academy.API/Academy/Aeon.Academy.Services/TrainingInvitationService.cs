using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Common.Workflow;
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
    public class TrainingInvitationService : ITrainingInvitationService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IUnitOfWork<EDocDbContext> eDocUnitOfWork = null;
        private readonly IGenericRepository<TrainingInvitation> repository = null;
        private readonly IGenericRepository<TrainingInvitationParticipant> participantRepository = null;

        public TrainingInvitationService(IUnitOfWork<AppDbContext> unitOfWork, IUnitOfWork<EDocDbContext> eDocUnitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.eDocUnitOfWork = eDocUnitOfWork;
            this.repository = unitOfWork.GetRepository<TrainingInvitation>();
            this.participantRepository = unitOfWork.GetRepository<TrainingInvitationParticipant>();
        }

        public TrainingInvitation Get(Guid id)
        {
            return repository.Get(id);
        }

        public PagedList<TrainingInvitation> GetAll(User currentUser, TrainingInvitationFilter filter)
        {
            // Condition with get all data
            bool isSadmin = currentUser.LoginName.ToLower().Equals("sadmin");
            bool isHeadCountOfAcademy = false;

            var departmentMappingbyUser = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(d => d.UserId == filter.UserId).AsNoTracking().ToList();
            var userList = new List<Guid?>();
            foreach (var userDepartmentMapping in departmentMappingbyUser)
            {
                var userMappingbyDept = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(d => d.DepartmentId == userDepartmentMapping.DepartmentId).ToList();
                userMappingbyDept.ForEach(x => userList.Add(x.UserId));
            }

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
                request = repository.Query(i => (filter.Status.Any(x => (i.Status.ToUpper().Contains(x.ToUpper())) || !filter.Status.Any()) && i.ReferenceNumber.ToUpper().Contains(filter.ReferenceNumber) && i.CreatedBySapCode.Contains(filter.SapCode) &&
                        i.TrainingRequest.TypeOfTraining.ToUpper().Contains(filter.CourseType) && i.Created >= filter.CreateDateFrom && i.Created <= filter.CreateDateTo
                        )).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
            }
            else
            {
                var listDepartmentOfCurrentUser = userDepartmentMappings.Select(x => x.DepartmentId).ToList();
                if (listDepartmentOfCurrentUser.Any())
                {
                    List<Guid> deptList = new List<Guid>();
                    foreach (var departmentId in listDepartmentOfCurrentUser)
                    {
                        deptList.AddRange(GetDepartmentTree(departmentId.Value));
                    }
                    request = repository.Query(i => (filter.Status.Any(x => (i.Status.ToUpper().Contains(x.ToUpper())) || !filter.Status.Any()) && i.ReferenceNumber.ToUpper().Contains(filter.ReferenceNumber) && i.CreatedBySapCode.Contains(filter.SapCode) &&
                        i.TrainingRequest.TypeOfTraining.ToUpper().Contains(filter.CourseType) && i.Created >= filter.CreateDateFrom && i.Created <= filter.CreateDateTo
                        && deptList.Contains(i.DepartmentId.Value)
                        )).OrderByDescending(r => r.Created).AsQueryable().AsNoTracking();
                }
            }
            return PagedList<TrainingInvitation>.ToPagedList(request, filter.PageNumber, filter.PageSize);
        }

        public TrainingInvitation GetByRequest(Guid requestId)
        {
            return repository.Query(r => r.TrainingRequestId == requestId && r.Status != TrainingInvitationStatus.Cancelled).FirstOrDefault();
        }

        public TrainingInvitationParticipant GetParticipant(Guid id)
        {
            return participantRepository.Get(id);
        }

        public PagedList<TrainingInvitationParticipant> GetAllInvitations(User currentUser, TrainingInvitationFilter filter)
        {
            // Condition with get all data
            bool isSadmin = currentUser.LoginName.ToLower().Equals("sadmin");
            bool isHeadCountOfAcademy = false;

            var departmentMappingbyUser = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(d => d.UserId == filter.UserId).AsNoTracking().ToList();
            var userList = new List<Guid?>();
            foreach (var userDepartmentMapping in departmentMappingbyUser)
            {
                var userMappingbyDept = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(d => d.DepartmentId == userDepartmentMapping.DepartmentId).ToList();
                userMappingbyDept.ForEach(x => userList.Add(x.UserId));
            }

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
            var invitations = (dynamic)null;
            if (isHeadCountOfAcademy || isSadmin)
            {
                invitations = participantRepository.Query(r => userList.Any(u => u == r.ParticipantId) && r.TrainingInvitation.Status != TrainingInvitationStatus.Save && r.TrainingInvitation.Status != TrainingInvitationStatus.Cancelled &&
                            (filter.Status.Any(x => r.Response.ToUpper().Contains(x.ToUpper())) || !filter.Status.Any()) && r.TrainingInvitation.ReferenceNumber.ToUpper().Contains(filter.ReferenceNumber) &&
                            r.SapCode.ToUpper().Contains(filter.SapCode) && r.TrainingInvitation.TrainingRequest.TypeOfTraining.ToUpper().Contains(filter.CourseType) &&
                            r.TrainingInvitation.Created >= filter.CreateDateFrom && r.TrainingInvitation.Created <= filter.CreateDateTo
                            ).OrderByDescending(r => r.TrainingInvitation.Created).AsQueryable();
            }
            else
            {
                var listDepartmentOfCurrentUser = userDepartmentMappings.Select(x => x.DepartmentId).ToList();
                if (listDepartmentOfCurrentUser.Any())
                {
                    List<Guid> deptList = new List<Guid>();
                    foreach (var departmentId in listDepartmentOfCurrentUser)
                    {
                        deptList.AddRange(GetDepartmentTree(departmentId.Value));
                    }
                    invitations = participantRepository.Query(r => userList.Any(u => u == r.ParticipantId) && r.TrainingInvitation.Status != TrainingInvitationStatus.Save && r.TrainingInvitation.Status != TrainingInvitationStatus.Cancelled &&
                            (filter.Status.Any(x => r.Response.ToUpper().Contains(x.ToUpper())) || !filter.Status.Any()) && r.TrainingInvitation.ReferenceNumber.ToUpper().Contains(filter.ReferenceNumber) &&
                            r.SapCode.ToUpper().Contains(filter.SapCode) && r.TrainingInvitation.TrainingRequest.TypeOfTraining.ToUpper().Contains(filter.CourseType) &&
                            r.TrainingInvitation.Created >= filter.CreateDateFrom && r.TrainingInvitation.Created <= filter.CreateDateTo
                            & deptList.Contains(r.TrainingInvitation.DepartmentId.Value)).OrderByDescending(r => r.TrainingInvitation.Created).AsQueryable();
                }
            }
            return PagedList<TrainingInvitationParticipant>.ToPagedList(invitations, filter.PageNumber, filter.PageSize);
        }

        public PagedList<TrainingInvitationParticipant> ListMyInvitations(TrainingInvitationFilter filter)
        {
            var invitations = participantRepository.Query(r => r.ParticipantId == filter.UserId && r.TrainingInvitation.Status != TrainingInvitationStatus.Save && r.TrainingInvitation.Status != TrainingInvitationStatus.Cancelled &&
                            (filter.Status.Any(x => r.Response.ToUpper().Contains(x.ToUpper())) || !filter.Status.Any()) && r.TrainingInvitation.ReferenceNumber.ToUpper().Contains(filter.ReferenceNumber) &&
                            r.SapCode.ToUpper().Contains(filter.SapCode) && r.TrainingInvitation.TrainingRequest.TypeOfTraining.ToUpper().Contains(filter.CourseType) &&
                            r.TrainingInvitation.Created >= filter.CreateDateFrom && r.TrainingInvitation.Created <= filter.CreateDateTo).OrderByDescending(r => r.TrainingInvitation.Created).AsQueryable();
            return PagedList<TrainingInvitationParticipant>.ToPagedList(invitations, filter.PageNumber, filter.PageSize);
        }

        public PagedList<TrainingInvitation> ListPendingByUserId(Guid userId, int pageNumber, int pageSize)
        {
            var invitationIds = participantRepository.Query(p => p.ParticipantId == userId && p.Response == ResponseType.Pending && p.TrainingInvitation.Status == TrainingInvitationStatus.SendInvitation).Select(p => p.TrainingInvitationId);
            var query = repository.Query(r => invitationIds.Contains(r.Id)).OrderByDescending(r => r.Modified).AsNoTracking();

            return PagedList<TrainingInvitation>.ToPagedList(query, pageNumber, pageSize);
        }

        public PagedList<TrainingInvitation> ListAfterReportByUserId(Guid userId, int pageNumber, int pageSize)
        {
            var invitationIds = participantRepository.Query(p => p.ParticipantId == userId && p.StatusOfReport == ReportStatus.NotSubmitted && p.TrainingInvitation.EndDate < DateTime.Now && p.TrainingInvitation.AfterTrainingReportDeadline >= DateTime.Now).Select(p => p.TrainingInvitationId);
            var query = repository.Query(r => invitationIds.Contains(r.Id))
               .OrderByDescending(r => r.Modified).AsNoTracking();

            return PagedList<TrainingInvitation>.ToPagedList(query, pageNumber, pageSize);
        }

        public Guid Save(TrainingInvitation trainingInvitation)
        {
            var request = unitOfWork.GetRepository<TrainingRequest>().Get(trainingInvitation.TrainingRequestId);
            UpdateTrainingRequest(request, trainingInvitation.Status);
            if (trainingInvitation.Id == Guid.Empty)
            {
                if (trainingInvitation.StartDate == DateTime.MinValue)
                    trainingInvitation.StartDate = DateTime.Now;
                if (trainingInvitation.EndDate == DateTime.MinValue)
                    trainingInvitation.EndDate = DateTime.Now;
                if (trainingInvitation.AfterTrainingReportDeadline == DateTime.MinValue)
                    trainingInvitation.AfterTrainingReportDeadline = DateTime.Now;
                var department = GetDeptByUserId(trainingInvitation.CreatedById);
                trainingInvitation.DepartmentId = department != null ? department.Id : Guid.Empty;
                trainingInvitation.DepartmentName = department != null ? department.Name : string.Empty;

                var trainingRequestStatus = request != null ? request.Status : string.Empty;
                if (trainingRequestStatus == WorkflowStatus.Completed)
                {
                    repository.Add(trainingInvitation);
                }
            }
            else
            {
                var participantItems = participantRepository.Query(a => a.TrainingInvitationId == trainingInvitation.Id).ToList();
                if (participantItems != null)
                {
                    participantItems.ForEach(a => participantRepository.Delete(a));
                }
                var department = GetDeptByUserId(trainingInvitation.CreatedById);
                trainingInvitation.DepartmentId = department != null ? department.Id : Guid.Empty;
                trainingInvitation.DepartmentName = department != null ? department.Name : string.Empty;
                trainingInvitation.TrainingInvitationParticipants.ToList().ForEach(a => participantRepository.Add(a));
                repository.Update(trainingInvitation);
            }

            unitOfWork.Complete();

            return trainingInvitation.Id;
        }

        public bool Delete(TrainingInvitation trainingInvitation)
        {
            if (trainingInvitation == null || trainingInvitation.Id == Guid.Empty || trainingInvitation.Status == TrainingInvitationStatus.SendInvitation) return false;

            var existing = repository.Get(trainingInvitation.Id);
            if (existing == null) return false;

            var participantItems = participantRepository.Query(a => a.TrainingInvitationId == trainingInvitation.Id).ToList();
            if (participantItems != null)
            {
                participantItems.ForEach(x => participantRepository.Delete(x));
            }
            repository.Delete(trainingInvitation);
            unitOfWork.Complete();
            return true;
        }

        public Guid Accept(TrainingInvitationParticipant participant)
        {
            participant.Response = ResponseType.Accept;
            if (participant.TrainingInvitation.AfterTrainingReportNotRequired == true)
            {
                participant.StatusOfReport = ReportStatus.NotRequired;
            }
            else
            {
                participant.StatusOfReport = ReportStatus.NotSubmitted;
            }
            participantRepository.Update(participant);
            unitOfWork.Complete();
            WaitingForAfterReportInvitation(participant.TrainingInvitationId);
            return participant.Id;
        }


        public Guid Decline(TrainingInvitationParticipant participant)
        {
            participant.Response = ResponseType.Decline;
            participant.StatusOfReport = ReportStatus.Declined;
            participantRepository.Update(participant);
            unitOfWork.Complete();
            WaitingForAfterReportInvitation(participant.TrainingInvitationId);

            return participant.Id;
        }

        public bool CheckAcademyUser(Guid UserId)
        {
            var departmentCode = ApplicationSettings.DeptAcademyCode;
            var departmentRepository = eDocUnitOfWork.GetRepository<Department>();
            var department = departmentRepository.Query(d => d.Code == departmentCode).FirstOrDefault();
            if (department != null)
            {
                var isChecker = eDocUnitOfWork.GetRepository<UserDepartmentMapping>().Query(r => r.DepartmentId == department.Id && r.UserId == UserId && (Group)r.Role == Group.Checker).Any();
                return isChecker;
            }
            return false;
        }

        public List<string> GetEnabledActions(TrainingInvitationParticipant participant, Guid UserId)
        {
            if (participant.ParticipantId == UserId && participant.Response == ResponseType.Pending && participant.TrainingInvitation.Status != TrainingInvitationStatus.Cancelled)
            {
                return new List<string> { ResponseType.Accept, ResponseType.Decline };
            }

            return new List<string>();
        }

        public Department GetDeptByUserId(Guid? id)
        {
            var repoDepartment = eDocUnitOfWork.GetRepository<Department>();
            var repoUserDepartmentMapping = eDocUnitOfWork.GetRepository<UserDepartmentMapping>();

            var departmentId = repoUserDepartmentMapping.Query(x => x.UserId == id && x.IsHeadCount).Select(x => x.DepartmentId).FirstOrDefault();
            if (departmentId != null)
            {
                return repoDepartment.Query(x => x.Id == departmentId).FirstOrDefault();
            }
            return null;
        }

        private void UpdateTrainingRequest(TrainingRequest request, string status)
        {
            if (request == null) return;
            if (request.Status == WorkflowStatus.Completed)
            {
                if (status == TrainingInvitationStatus.SendInvitation)
                {
                    request.AssignedUserId = null;
                    unitOfWork.GetRepository<TrainingRequest>().Update(request);
                }
                else if (status == TrainingInvitationStatus.Cancelled)
                {
                    var wfService = new WorkflowService<TrainingRequest>(eDocUnitOfWork, unitOfWork, null);
                    wfService.AssignToChecker(request);
                    unitOfWork.GetRepository<TrainingRequest>().Update(request);
                }
            }
        }

        private void WaitingForAfterReportInvitation(Guid id)
        {
            var trainingInvitation = repository.Get(id);
            var request = unitOfWork.GetRepository<TrainingRequest>().Get(trainingInvitation.TrainingRequestId);
            var noParticipantDecline = trainingInvitation.TrainingInvitationParticipants.Where(x => x.Response == ResponseType.Decline).ToList();
            if (!trainingInvitation.TrainingInvitationParticipants.Any(x => x.Response == ResponseType.Pending))
            {
                if (trainingInvitation.AfterTrainingReportNotRequired == true)
                {
                    if (trainingInvitation.TrainingInvitationParticipants.Count == noParticipantDecline.Count)
                    {
                        trainingInvitation.Status = TrainingInvitationStatus.Cancelled;
                        UpdateTrainingRequest(request, trainingInvitation.Status);
                    }
                    else
                        trainingInvitation.Status = TrainingInvitationStatus.Completed;
                }
                else
                {
                    if (trainingInvitation.TrainingInvitationParticipants.Count == noParticipantDecline.Count)
                    {
                        trainingInvitation.Status = TrainingInvitationStatus.Cancelled;
                        UpdateTrainingRequest(request, trainingInvitation.Status);
                    }
                    else
                        trainingInvitation.Status = TrainingInvitationStatus.WaitingForAfterReport;
                }
                repository.Update(trainingInvitation);
                unitOfWork.Complete();
            }
        }

        public User FindEmployeeManager(TrainingInvitationParticipant participant)
        {
            var userRepository = eDocUnitOfWork.GetRepository<User>();
            var userDepartmentRepository = eDocUnitOfWork.GetRepository<UserDepartmentMapping>();
            var departmentRepo = eDocUnitOfWork.GetRepository<Department>();
            var jobGradeRepo = eDocUnitOfWork.GetRepository<JobGrade>();
            var wfService = new WorkflowService<TrainingRequest>(eDocUnitOfWork, unitOfWork, null);
            User manager = null;
            var participantJobGrade = jobGradeRepo.Query(j => j.Title.Equals(participant.Position, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var departmentNote = departmentRepo.Query(x => x.Name.Equals(participant.Department, StringComparison.OrdinalIgnoreCase) && x.JobGradeId == participantJobGrade.Id).FirstOrDefault();
            var minGradeManager = 4;

            if (departmentNote != null && participantJobGrade != null)
            {
                if (participantJobGrade.Grade == 9)
                    return null;
                if (departmentNote.IsStore && participantJobGrade.Grade >= 4)
                {
                    minGradeManager = participantJobGrade.Grade + 1;
                }
                if (!departmentNote.IsStore && participantJobGrade.Grade < 5)
                {
                    minGradeManager = 5;
                }
                if (!departmentNote.IsStore && participantJobGrade.Grade >= 5)
                {
                    minGradeManager = participantJobGrade.Grade + 1;
                }
                /*var minJobGrade = "G" + minGradeManager;*/
                var minJobGrade = minGradeManager;
                var managerDept = wfService.GetDepartmentBaseOnNote(departmentNote, minJobGrade);
                while (true)
                {
                    if (managerDept != null)
                    {
                        minGradeManager = jobGradeRepo.Query(j => j.Id == managerDept.JobGradeId).Select(j => j.Grade).FirstOrDefault();
                        var mapping = userDepartmentRepository.Query(m => m.DepartmentId == managerDept.Id && m.Role == (int)Group.Member && m.IsHeadCount).FirstOrDefault();
                        if (mapping != null)
                        {
                            manager = userRepository.Query(u => u.Id == mapping.UserId).FirstOrDefault();
                            if (manager != null)
                                return manager;
                        }
                        minGradeManager++;
                        if (minGradeManager == 10)
                            return null;
                        /*minJobGrade = "G" + minGradeManager;*/
                        minJobGrade = minGradeManager;
                        managerDept = wfService.GetDepartmentBaseOnNote(managerDept, minJobGrade);
                    }
                    else
                        return null;
                }
            }

            return null;
        }
        public TrainingInvitationParticipant GetParticipantBySapCode(string sapCode, Guid? invitationId)
        {
            return participantRepository.Query(x => x.SapCode == sapCode && x.TrainingInvitationId == invitationId).FirstOrDefault();
        }
        public List<TrainingInvitationParticipant> GetParticipantAccepted(Guid invitationId)
        {
            return participantRepository.Query(x => x.TrainingInvitationId == invitationId && x.Response == ResponseType.Accept).ToList();
        }
        public List<TrainingInvitationParticipant> GetParticipantsWithNotSubmitted()
        {
            return participantRepository.Query(x => x.TrainingInvitation.Status == TrainingInvitationStatus.Completed && x.TrainingInvitation.EndDate < DateTime.Today
            && x.Response == ResponseType.Accept && x.SapStatusCode == (int)SAPStatusCode.NotSubmit).ToList();
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
