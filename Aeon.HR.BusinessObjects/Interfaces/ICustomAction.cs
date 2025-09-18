using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{

    public interface ICustomAction
    {
        Task Execute(IUnitOfWork uow, Guid itemId, string lastOutcome, IDashboardBO dashboardBO, IWorkflowBO workflowBO);
    }
}
