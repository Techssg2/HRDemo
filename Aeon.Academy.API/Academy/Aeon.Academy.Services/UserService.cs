using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Common.Workflow;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork<EDocDbContext> unitOfWork = null;
        private readonly ITrainingRequestService trainingRequestService = null;

        public UserService(IUnitOfWork<EDocDbContext> unitOfWork, ITrainingRequestService trainingRequestService)
        {
            this.unitOfWork = unitOfWork;
            this.trainingRequestService = trainingRequestService;
        }
        public User GetUser(string loginName)
        {
            var repo = unitOfWork.GetRepository<User>();
            return repo.Query(e => e.LoginName.ToLower() == loginName.ToLower()).AsNoTracking().FirstOrDefault();
        }
        public IList<UserDepartmentMapping> GetUserDepartmentMappingsByUserId(Guid userId)
        {
            var repo = unitOfWork.GetRepository<UserDepartmentMapping>();
            return repo.Query(m => m.UserId == userId).AsNoTracking().Distinct().ToList();
        }

        public bool IsCheckerAcademy(Guid requestId, User user)
        {
            var request = trainingRequestService.Get(requestId);
            var workflow = request != null ? CommonUtil.DeserializeWorkflow(request.WorkflowData) : null;
            var stepCondition = workflow != null ? workflow.StartWorkflowConditions.FirstOrDefault(c => string.Equals(c.FieldName, "2-Department")) : null;
            return CheckRole(stepCondition, user.Id, Group.Checker);
        }

        public bool IsHODAcademy(Guid requestId, User user)
        {
            var request = trainingRequestService.Get(requestId);
            var workflow = request != null ? CommonUtil.DeserializeWorkflow(request.WorkflowData) : null;
            var stepCondition = workflow != null ? workflow.StartWorkflowConditions.FirstOrDefault(c => string.Equals(c.FieldName, "3-Department")) : null;
            return CheckRole(stepCondition, user.Id, Group.HOD);
        }
        public bool IsAcademy(Guid userId)
        {
            var academySapcode = ApplicationSettings.DeptAcademyCode;
            var departmentRepository = unitOfWork.GetRepository<Department>();
            var departmentId = departmentRepository.Query(d => d.Code == academySapcode).Select(d => d.Id).FirstOrDefault();
            var userDeptMapping = GetUserDepartmentMappingsByUserId(userId).Select(d => d.DepartmentId);
            return userDeptMapping.Contains(departmentId);
        }
        private bool CheckRole(WorkflowCondition stepCondition, Guid userId, Group roleType)
        {
            if (stepCondition != null)
            {
                var tokens = stepCondition.FieldValues.First().Split(new char[] { '-' });
                var departmentCode = tokens.Length > 0 ? tokens[tokens.Length - 1] : string.Empty;
                var departmentRepository = unitOfWork.GetRepository<Department>();
                var department = departmentRepository.Query(d => d.Code == departmentCode).FirstOrDefault();
                if (department != null)
                {
                    var isChecker = unitOfWork.GetRepository<UserDepartmentMapping>().Query(r => r.DepartmentId == department.Id && r.UserId == userId && (Group)r.Role == roleType).Any();
                    return isChecker;
                }
            }
            return false;
        }
    }
}
