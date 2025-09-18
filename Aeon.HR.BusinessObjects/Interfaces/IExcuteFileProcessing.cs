using Aeon.HR.Infrastructure.Enums;
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
    public interface IExcuteFileProcessing
    {
        IFileProcessing CreatedFileProcessing(FileProcessingType type);
        Task<ResultDTO> ImportAsync(FileProcessingType type, FileStream fileStream);
        Task<ResultDTO> ExportAsync(FileProcessingType type, QueryArgs parameters);
    }
}
