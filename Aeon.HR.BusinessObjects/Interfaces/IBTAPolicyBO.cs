using Aeon.HR.Data.Models;
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
	public interface IBTAPolicyBO
	{
		//Define Interface Functions - API controller call to this
		#region BTA Policy
		Task<ResultDTO> GetBTAPolicyByDepartment(string typeDepartment);
		Task<ResultDTO> GetBTAPolicyList(QueryArgs arg);
		Task<ResultDTO> CreateBTAPolicy(BTAPolicyArgs item);
		Task<ResultDTO> UpdateBTAPolicy(BTAPolicyArgs item);
		Task<ResultDTO> DeleteBTAPolicy(BTAPolicyArgs item);
		Task<ResultDTO> GetBTAPolicyByJobGradePartition(QueryArgs arg);
		#endregion
		#region BTA Policy Special Cases
		Task<ResultDTO> GetListBTAPolicySpecialCases(QueryArgs arg);
		Task<ResultDTO> GetBTAPolicySpecialCasesByUserSAPCode(string userSapCode, Guid partitionId);
		Task<ResultDTO> CreateBTAPolicySpecialCases(BTAPolicySpecialArgs arg);
		Task<ResultDTO> UpdateBTAPolicySpecialCases(BTAPolicySpecialArgs item);
		Task<ResultDTO> DeleteBTAPolicySpecialCases(BTAPolicySpecialArgs item);
		#endregion

	}
}
