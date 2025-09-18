using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ICommonBO
    {
        Task<ResultDTO> DeleteItemById(Guid id);
        Task<ResultDTO> UpdateStatusByReferenceNumber(UpdateStatusArgs args);
        Task<ResultDTO> GetItemById(Guid id);
    }
}
