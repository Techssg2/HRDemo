using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface ITrainingReportService
    {
        TrainingReport Get(Guid id);
        Guid Save(TrainingReport academyTrainingRequest);
        IList<TrainingReport> ListByUser(Guid userId);
        PagedList<TrainingReport> ListByDepartmentId(Guid userId, Guid departmentId, int pageNumber, int pageSize);
        PagedList<TrainingReport> ListByDepartmentIds(Guid userId, List<Guid> departmentIds, int pageNumber, int pageSize);
        ProgressingStage<TrainingReportHistory> ProgressingStage(Guid id);
        void UpdateParticipantStatus(Guid? invitationId, Guid? useId, string status);
        List<EdocTask<TrainingReport>> ListTasks();
        TrainingReport GetByInvitation(Guid invitationId, Guid? userId);
        void Delete(Guid id);
        void UpdateInvitation(TrainingReport report);
        PagedList<TrainingReport> ListByCurrentDepartment(TrainingReportFilter filter);
        PagedList<TrainingReport> ListMyItem(TrainingReportFilter filter);
        PagedList<TrainingReport> GetAllItem(User currentUser, TrainingReportFilter filter);
    }
}
