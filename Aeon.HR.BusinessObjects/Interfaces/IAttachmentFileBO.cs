using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IAttachmentFileBO
    {
        Task<ResultDTO> Save(AttachmentFileViewModel model);
        Task<AttachmentFileViewModel> Get(Guid id);
        Task<ResultDTO> Delete(Guid id);
        Task<ResultDTO> DeleteMultiFile(Guid[] ids);
    }
}
