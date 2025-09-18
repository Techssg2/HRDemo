using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface ITrainingRequestService
    {
        TrainingRequest Get(Guid id);
        PagedList<TrainingRequest> ListAll(int pageNumber, int pageSize);
        IList<TrainingRequest> GetByDepartment(Guid departmentId);
        Guid Save(TrainingRequest academyTrainingRequest);
        IList<TrainingRequest> ListByUserId(Guid userId);
        IList<TrainingRequest> ListByPending();
        TrainingRequestManagement ListByDepartment(Guid? departmentId, int pageNumber, int pageSize);
        PagedList<TrainingRequest> ListByDepartmentId(Guid userId, Guid departmentId, int pageNumber, int pageSize);
        PagedList<TrainingRequest> ListByDepartmentIds(Guid userId, List<Guid> departmentIds, int pageNumber, int pageSize);
        ProgressingStage<TrainingRequestHistory> ProgressingStage(Guid id);
        List<EdocTask<TrainingRequest>> ListTasks();
        void Delete(Guid id);
        string GetWorkflowName(TrainingRequest request, Guid userId);
        PagedList<TrainingRequest> GetMyItem(TrainingRequestFilter filter);
        PagedList<TrainingRequest> GetAllItem(User user, TrainingRequestFilter filter);
        IList<TrainingRequest> ListByExternalCompleted(DateTime date);
    }
}
