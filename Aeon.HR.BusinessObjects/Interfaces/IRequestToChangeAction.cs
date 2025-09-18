using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IRequestToChangeAction
    {
        Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger, string stepName);
    }
}
