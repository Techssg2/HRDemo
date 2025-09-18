using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IFileProcessing
    {
        Task<ResultDTO> ImportAsync(FileStream stream);
        Task<ResultDTO> ExportAsync(QueryArgs parameters);
    }
}
