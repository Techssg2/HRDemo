using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IMassBO
    {
        Task<ResultDTO> UpdateCategory(RecruitmentCategoryArgs data);
        Task<ResultDTO> DeleteCategory(Guid id);
        Task<ResultDTO> GetMassLocations();
        Task<MassResponseAPIViewModel> GetMassLocationsPRD();
        Task<bool> PushPositionToMass(PositionMassViewModel data);
        Task<ResultDTO> GetMassPositions(QueryArgs queryArg);
        Task<bool> ChangeStatusPositionToMass(PositionChangingStatus data);
    }
}
