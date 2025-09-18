using Aeon.HR.Infrastructure;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IOverBudgetBO
    {
        Task<ResultDTO> GetItemOverBudgetById(Guid id);
        Task<ResultDTO> GetListOverBudget(QueryArgs arg);
        Task<ResultDTO> SaveRequestOverBudget(RequestOverBudgetDTO data);
        Task<ResultDTO> SaveBudget(BusinessOverBudgetDTO data);
        ResultDTO GetTripOverBudgetGroups(Guid Id);
    }
}
