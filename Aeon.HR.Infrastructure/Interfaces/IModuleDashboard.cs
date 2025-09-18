using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Interfaces
{
    public interface IModuleDashboard
    {
        IList<object> GetMyItems(Guid userId);
        IList<object> GetMyTasks(Guid userId, QueryArgs args);
        IList<object> GetJobTasks();
    }
}
