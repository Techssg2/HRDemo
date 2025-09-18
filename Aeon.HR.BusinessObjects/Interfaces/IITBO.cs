
using Aeon.HR.Infrastructure.Abstracts;
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
    public interface IITBO
    {
        Task<ResultDTO> SaveResignation(ITSaveResignationArgs args);
        Task<ResultDTO> SaveRequestToHire(ITSaveActingArgs args);
        Task<ResultDTO> SaveShiftExchange(ITSaveShiftExchangeArgs args);
        Task<ResultDTO> SaveTargetPlan(ITSaveTargetPlanArgs args);
        Task<ResultDTO> SavePromoteAndTransfer(ITSavePromoteAndTransferArgs args);
        Task<ResultDTO> SaveBTA(ITSaveBTAArgs args);
    }
}
