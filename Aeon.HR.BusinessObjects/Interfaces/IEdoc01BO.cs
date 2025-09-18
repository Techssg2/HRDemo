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
    public interface IEdoc01BO
    {
        Task<List<WorkflowTaskViewModel>> GetTasks(Edoc1Arg arg);
        Task<List<WorkflowTaskViewModel>> GetTasksV2(Edoc1ArgV2 arg);
        Task<ResultDTO> CreateF2Form();
    }
}
