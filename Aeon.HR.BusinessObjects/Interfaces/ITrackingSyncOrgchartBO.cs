using Aeon.HR.Infrastructure;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ITrackingSyncOrgchartBO
    {
        Task<ResultDTO> GetTrackingUserDepartmentsRequest(QueryArgs args);
        Task<ResultDTO> GetTrackingDepartmentsLogRequest(QueryArgs args);
        Task<ResultDTO> GetTrackingUsersLogRequest(QueryArgs args);
    }
}
