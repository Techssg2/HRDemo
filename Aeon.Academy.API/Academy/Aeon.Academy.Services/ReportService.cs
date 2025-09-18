using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Aeon.Academy.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IGenericRepository<TrainingReport> reportRepository = null;
        private readonly IGenericRepository<TrainingInvitation> invitationRepository = null;
        private readonly IGenericRepository<TrainingInvitationParticipant> invitationParticipantRep = null;
        private readonly IGenericRepository<TrainingRequest> requestRepository = null;
        private readonly IUnitOfWork<EDocDbContext> eDocUnitOfWork = null;

        public ReportService(IUnitOfWork<AppDbContext> unitOfWork, IUnitOfWork<EDocDbContext> eDocUnitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.reportRepository = unitOfWork.GetRepository<TrainingReport>();
            this.invitationRepository = unitOfWork.GetRepository<TrainingInvitation>();
            this.invitationParticipantRep = unitOfWork.GetRepository<TrainingInvitationParticipant>();
            this.requestRepository = unitOfWork.GetRepository<TrainingRequest>();
            this.eDocUnitOfWork = eDocUnitOfWork;
        }

        public List<IndividualReport> GetIndividualReports(Guid userId)
        {
            var individualReports = new List<IndividualReport>();
            var trainings = invitationParticipantRep.Query(c => c.ParticipantId == userId);
            var trainingReports = reportRepository.Query(q => q.CreatedById == userId);
            if (trainings != null)
            {
                foreach (var item in trainings)
                {
                    var rp = new IndividualReport();
                    if (item.TrainingInvitation != null)
                    {
                        rp.CourseName = item.TrainingInvitation.CourseName;
                        rp.StartingDate = item.TrainingInvitation.StartDate;
                        rp.EndDate = item.TrainingInvitation.EndDate;
                        if (item.TrainingInvitation.TrainingRequest != null)
                        {
                            rp.SupplierName = item.TrainingInvitation.TrainingRequest.SupplierName;
                            rp.TypeofTraining = item.TrainingInvitation.TrainingRequest.TypeOfTraining;
                        }
                    }
                    if (trainingReports != null)
                    {
                        var trainingReport = trainingReports.Where(x => x.TrainingInvitationId == item.TrainingInvitationId).FirstOrDefault();
                        rp.CourseAssessment = trainingReport != null ? trainingReport.Remark : string.Empty;
                    }
                    individualReports.Add(rp);
                }
            }
            return individualReports;
        }
        public PagedList<TrainingTrackerReport> GetTrainingTrackerReports(TrainingTrackerReportFilter filter)
        {
            var appDb = new AppDbContext();
            var participants = appDb.TrainingInvitationParticipants.Where(x => x.Response == ResponseType.Accept);
            var query = from participant in participants
                        join invitation in appDb.TrainingInvitations on participant.TrainingInvitationId equals invitation.Id
                        join report in appDb.TrainingReports on invitation.Id equals report.TrainingInvitationId into gj
                        from o in (from f in gj where f.CreatedById == participant.ParticipantId && f.Status == WorkflowStatus.Completed select f).DefaultIfEmpty()
                        join request in appDb.TrainingRequests on invitation.TrainingRequestId equals request.Id
                        orderby invitation.StartDate descending
                        select new
                        {
                            participant.ParticipantId,
                            participant.SapCode,
                            JobGrade = participant.Position,
                            Division = participant.Department,
                            FullName = participant.Name,
                            invitation.StartDate,
                            invitation.EndDate,
                            participant.TrainingInvitationId,
                            invitation.TrainingRequestId,
                            ActualAttendingDate = o != null ? o.ActualAttendingDate : null,
                            request.CourseName,
                            request.SupplierName,
                            request.TypeOfTraining,
                            request.TrainingFee,
                            invitation.TotalOfflineTrainingHours,
                            invitation.TotalOnlineTrainingHours,
                        };

            var trackerReports = new List<TrainingTrackerReport>();
            foreach (var item in query)
            {
                var newReport = new TrainingTrackerReport
                {
                    UserId = item.ParticipantId,
                    SapCode = item.SapCode,
                    FullName = item.FullName,
                    Division = item.Division,
                    JobGrade = item.JobGrade,
                    CourseName = item.CourseName,
                    TrainingFee = item.TrainingFee,
                    TypeOfTraining = item.TypeOfTraining,
                    SupplierName = item.SupplierName,
                    StartingDate = item.StartDate,
                    EndingDate = item.EndDate,
                    ActualAttendingDate = item.ActualAttendingDate,
                    TotalOfflineTrainingHours = item.TotalOfflineTrainingHours,
                    TotalOnlineTrainingHours = item.TotalOnlineTrainingHours,
                };

                trackerReports.Add(newReport);
            }
            if (filter == null) filter = new TrainingTrackerReportFilter();
            trackerReports = trackerReports.Where(q => q.SapCode.IndexOf(filter.SapCode, StringComparison.OrdinalIgnoreCase) >= 0 &&
                q.FullName.IndexOf(filter.FullName, StringComparison.OrdinalIgnoreCase) >= 0 &&
                q.CourseName != null && q.CourseName.IndexOf(filter.CourseName, StringComparison.OrdinalIgnoreCase) >= 0 &&
                q.SupplierName != null && q.SupplierName.IndexOf(filter.SupplierName, StringComparison.OrdinalIgnoreCase) >= 0 &&
                (!filter.TypeofTraining.Any() || filter.TypeofTraining.Any(r => r.ToUpper().Contains(q.TypeOfTraining.ToUpper())))
            ).ToList();

            if (filter.StartingDateFrom != null || filter.StartingDateTo != null)
            {
                trackerReports = trackerReports.Where(q =>
                    (filter.StartingDateFrom != null && filter.StartingDateTo != null && q.StartingDate >= filter.StartingDateFrom && q.StartingDate < filter.StartingDateTo) ||
                    (filter.StartingDateFrom != null && filter.StartingDateTo == null && q.StartingDate >= filter.StartingDateFrom) ||
                    (filter.StartingDateTo != null && filter.StartingDateFrom == null && q.StartingDate < filter.StartingDateTo)
                ).ToList();
            }
            if (filter.EndingDateFrom != null || filter.EndingDateTo != null)
            {
                trackerReports = trackerReports.Where(q =>
                    (filter.EndingDateFrom != null && filter.EndingDateTo != null && q.EndingDate >= filter.EndingDateFrom && q.EndingDate < filter.EndingDateTo) ||
                    (filter.EndingDateFrom != null && filter.EndingDateTo == null && q.EndingDate >= filter.EndingDateFrom) ||
                    (filter.EndingDateTo != null && filter.EndingDateFrom == null && q.EndingDate < filter.EndingDateTo)
               ).ToList();
            }
            var result = PagedList<TrainingTrackerReport>.ToPagedList(trackerReports.AsQueryable(), filter.PageNumber.Value, filter.PageSize.Value);
            UpdateDeptLine(result);
            return result;
        }
        private void UpdateDeptLine(PagedList<TrainingTrackerReport> reports)
        {
            if (reports != null && reports.Any())
            {
                var invitationService = new TrainingInvitationService(unitOfWork, eDocUnitOfWork);
                foreach (var rp in reports)
                {
                    var department = invitationService.GetDeptByUserId(rp.UserId);
                    if (department != null && department.Type == 2)
                    {
                        rp.DeptLine = department.Name;
                        rp.Division = string.Empty;
                    }
                    else
                    {
                        var deptLine = GetDepartmentLine(department);
                        rp.DeptLine = deptLine;
                    }
                }
            }
            return;
        }
        private string GetDepartmentLine(Department department)
        {
            if (department != null)
            {
                var skip = false;
                var departmentIdx = department.ParentId;
                while (!skip)
                {
                    if (departmentIdx.HasValue)
                    {
                        var dept = eDocUnitOfWork.GetRepository<Department>().Get(departmentIdx.Value);
                        if (dept.Type == 2)
                        {
                            return dept.Name;
                        }
                        else
                        {
                            departmentIdx = dept.ParentId;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            return string.Empty;
        }
        public PagedList<TrainingReport> GetTrainingSurveyReport(TrainingSurveyReportFilter filter)
        {
            var trainingReports = reportRepository.Query(x => x.Status == WorkflowStatus.Completed && x.SapCode.ToUpper().Contains(filter.SapCode) && x.FullName.ToUpper().Contains(filter.FullName) &&
                                x.TrainingInvitation.CourseName.ToUpper().Contains(filter.CourseName) && x.TrainerName.ToUpper().Contains(filter.TrainerName) &&
                                x.TrainingInvitation.StartDate >= filter.StartDateFrom && x.TrainingInvitation.StartDate <= filter.StartDateTo &&
                                x.TrainingInvitation.EndDate >= filter.EndDateFrom && x.TrainingInvitation.EndDate <= filter.EndDateTo).OrderByDescending(x => x.Created).AsQueryable();

            return PagedList<TrainingReport>.ToPagedList(trainingReports, filter.PageNumber, filter.PageSize);
        }
        public PagedList<TrainerContributionReport> GetTrainerContributionReport(TrainingSurveyReportFilter filter)
        {
            var userRepository = eDocUnitOfWork.GetRepository<User>();
            var invitationService = new TrainingInvitationService(unitOfWork, eDocUnitOfWork);
            var trainerContributionReports = new List<TrainerContributionReport>();
            var invitations = invitationRepository.Query(x => x.Status == TrainingInvitationStatus.Completed && x.TrainerName != null && !x.TrainerName.Equals(string.Empty)).OrderByDescending(x => x.Created).ToList();
            foreach (var invitation in invitations)
            {
                var department = GetDepartmentLineByUserId(invitation.TrainerId);
                var newReport = new TrainerContributionReport()
                {
                    SapCode = userRepository.Query(u => u.Id == invitation.TrainerId).Select(u => u.SapCode).FirstOrDefault(),
                    FullName = invitation.TrainerName,
                    Department = department != null ? department.Name : string.Empty,
                    CourseName = invitation.CourseName,
                    CourseDuration = invitation.HoursPerDay,
                    TotalCourseAttended = invitation.NumberOfDays,
                    TotalTimeAttended = invitation.TotalHours,
                    StartDate = invitation.StartDate,
                    EndDate = invitation.EndDate
                };
                trainerContributionReports.Add(newReport);
            }
            if (filter == null) filter = new TrainingSurveyReportFilter();
            trainerContributionReports = trainerContributionReports.Where(q => q.SapCode.ToUpper().Contains(filter.SapCode) &&
                q.FullName.ToUpper().Contains(filter.TrainerName) && q.CourseName != null && q.CourseName.ToUpper().Contains(filter.CourseName)).ToList();

            var result = PagedList<TrainerContributionReport>.ToPagedList(trainerContributionReports.AsQueryable(), filter.PageNumber, filter.PageSize);
            return result;
        }
        public PagedList<TrainingBudgetBalanceReport> GetTrainingBudgetBalanceReport(TrainingBudgetBalanceReportFilter filter)
        {
            var trainingBudgetBalanceReports = new List<TrainingBudgetBalanceReport>();
            var requests = requestRepository.Query(r => r.TypeOfTraining == TrainingType.External && (r.Status == WorkflowStatus.Pending || r.Status == WorkflowStatus.Completed)).ToList();
            foreach (var request in requests)
            {
                var requesterDepartment = GetDepartmentLineByUserId(request.CreatedById);
                var newReport = new TrainingBudgetBalanceReport()
                {
                    RequestFor = requesterDepartment != null ? requesterDepartment.Name : string.Empty,
                    RequestForDeptId = requesterDepartment != null ? requesterDepartment.Id : Guid.Empty,
                    CourseName = request.CourseName, 
                    BudgetGroup = request.CostCenters.FirstOrDefault()?.BudgetPlan,
                    PlannedBudget = request.TrainingFee,
                    SupplierName = request.SupplierName,
                    RequestedDate = request.DateOfSubmission,
                };
                if (request.ActualTuitionReimbursementAmount != null)
                {
                    newReport.ActualUsedBudget = RoundBudget(request.TrainingFee - request.ActualTuitionReimbursementAmount.Value);
                }
                if (request.SponsorshipPercentage != 0)
                {
                    newReport.ActualUsedBudget = RoundBudget(request.TrainingFee * (100 - request.SponsorshipPercentage)/100);
                }
                if (request.TrainingRequestParticipants != null && request.TrainingRequestParticipants.Any())
                {
                    var listRequested = new List<TrainingRequestParticipant>();
                    foreach (var participant in request.TrainingRequestParticipants)
                    {
                        var participantInfo = participant;
                        var requestedDept = GetDepartmentLineByUserId(participant.ParticipantId);
                        participantInfo.Department = requestedDept != null ? requestedDept.Name : string.Empty;
                        participantInfo.Id = requestedDept != null ? requestedDept.Id : Guid.Empty;
                        listRequested.Add(participantInfo);
                    }
                    var listRequestbyDeprtId = listRequested.GroupBy(x => x.Id);
                    var requestedDepartments = new List<RequestedDepartment>();
                    foreach (var group in listRequestbyDeprtId)
                    {
                        var participantByGroup = listRequested.Where(x => x.Id == group.Key).ToList();
                        var requestedDepartment = new RequestedDepartment()
                        {
                            DepartmentName = participantByGroup.FirstOrDefault().Department,
                            DepartmentId = participantByGroup.FirstOrDefault().Id,
                        };
                        var budget = newReport.ActualUsedBudget * participantByGroup.Count / request.TrainingRequestParticipants.Count;
                        requestedDepartment.ActualUsedBudgetByDepartment = Math.Ceiling(budget);
                        var trainingBudgetParticipants = new List<TrainingBudgetParticipant>();
                        var listGroupByJobGrade = participantByGroup.GroupBy(x => x.Position);
                        foreach (var jobGrade in listGroupByJobGrade)
                        {
                            var participantByJobGrade = participantByGroup.Where(x => x.Position == jobGrade.Key).ToList();
                            var trainingBudgetParticipant = new TrainingBudgetParticipant()
                            {
                                Jobgrade = participantByJobGrade.FirstOrDefault().Position,
                                NoParticipant = participantByJobGrade.Count()
                            };
                            trainingBudgetParticipants.Add(trainingBudgetParticipant);
                        }
                        requestedDepartment.Participants = trainingBudgetParticipants;
                        requestedDepartments.Add(requestedDepartment);
                    }
                    newReport.RequestedDepartments = requestedDepartments;
                }
                trainingBudgetBalanceReports.Add(newReport);
            }
            if (filter == null) filter = new TrainingBudgetBalanceReportFilter();
            if (filter.RequestForDeptId != null && filter.RequestForDeptId != Guid.Empty)
            {
                var deptRoot = DepartmentTree(filter.RequestForDeptId);
                trainingBudgetBalanceReports = trainingBudgetBalanceReports.Where(x => deptRoot.Any(u => u == x.RequestForDeptId)).ToList();
            }
            if (filter.RequestedDeptId != null && filter.RequestedDeptId != Guid.Empty)
            {
                var deptRoot = DepartmentTree(filter.RequestedDeptId);
                trainingBudgetBalanceReports = trainingBudgetBalanceReports.Where(x => deptRoot.Any(u => x.RequestedDepartments.Any(v => v.DepartmentId == u))).ToList();
            }
            trainingBudgetBalanceReports = trainingBudgetBalanceReports.Where(q =>q.SupplierName != null && q.SupplierName.ToUpper().Contains(filter.SupplierName) &&
                                           q.CourseName.ToUpper().Contains(filter.CourseName) && q.RequestedDate >= filter.RequestedDateFrom && q.RequestedDate <= filter.RequestedDateTo).ToList();

            return PagedList<TrainingBudgetBalanceReport>.ToPagedList(trainingBudgetBalanceReports.AsQueryable(), filter.PageNumber, filter.PageSize);
        }
        public Department GetDepartmentLineByUserId(Guid? id)
        {
            var invitationService = new TrainingInvitationService(unitOfWork, eDocUnitOfWork);
            var department = invitationService.GetDeptByUserId(id);
            if (department != null)
            {
                if (department.Type == 2)
                {
                    return department;
                }
                else
                {
                    var skip = false;
                    var departmentIdx = department.ParentId;
                    while (!skip)
                    {
                        if (departmentIdx.HasValue)
                        {
                            var dept = eDocUnitOfWork.GetRepository<Department>().Get(departmentIdx.Value);
                            if (dept.Type == 2)
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
                            return department;
                        }
                    }
                }
            }
            return null;
        }
        private decimal RoundBudget(decimal budget)
        {
            decimal result = budget % 1000 > 0 ? budget + 1000 - budget % 1000 : budget;
            return result;
        }
        private List<Guid> DepartmentTree(Guid departmentId)
        {
            var repoDepartment = eDocUnitOfWork.GetRepository<Department>();
            var departments = repoDepartment.GetAll().ToList();
            var list = new List<Guid>() {
                departmentId
            };
            ListDepartmentTree(departmentId, departments, list);
            return list;
        }
        private void ListDepartmentTree(Guid id, List<Department> departments, List<Guid> list)
        {
            var childIds = departments.Where(x => x.ParentId == id).Select(x => x.Id);

            if (childIds != null && childIds.Any())
            {
                list.AddRange(childIds);
                foreach (var childId in childIds)
                {
                    ListDepartmentTree(childId, departments, list);
                }
            }
        }

    }
}
