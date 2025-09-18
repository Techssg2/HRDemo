using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface INavigationBO
    {
        Task<ArrayResultDTO> GetListNavigation(QueryArgs args);
        Task<ArrayResultDTO> GetAll();
        Task<ArrayResultDTO> GetListNavigationByUserIdAndDepartmentId();
        Task<ArrayResultDTO> GetListNavigationByUserIdAndDepartmentIdV2(NavigationArgs.GetListArgs args);
        Task<ArrayResultDTO> GetListNavigationByUserIsNotEdocHR();
        Task<ArrayResultDTO> GetChildNavigationByParentId(Guid parentId);
        Task<ArrayResultDTO> GetChildNavigationByType(NavigationType type);
        Task<ResultDTO> CreateNavigation(NavigationDataForCreatingArgs args);
        Task<ResultDTO> UpdateNavigation(NavigationDataForCreatingArgs args);
        Task<ResultDTO> DeleteNavigationById(Guid Id);
        Task<ResultDTO> UpdateImageNavigation(NavigationDataForCreatingArgs data);
    }
}
