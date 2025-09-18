using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
	public interface IFacilityBO
	{
		Task<List<WorkflowTaskViewModel>> GetTasks(QueryArgs arg, bool isSuperAdmin);
	}
}
