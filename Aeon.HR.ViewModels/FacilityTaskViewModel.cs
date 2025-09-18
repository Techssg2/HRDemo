using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
	public class FacilityTaskViewModel
	{
		public string Title { get; set; }
		public Guid ItemId { get; set; }
		public string ItemType { get; set; }
		public int RequestType { get; set; }
		public string ReferenceNumber { get; set; }
		public DateTimeOffset DueDate { get; set; }
		public string Status { get; set; }
		public VoteType Vote { get; set; }
		public Guid? RequestedDepartmentId { get; set; }
		public string RequestedDepartmentCode { get; set; }
		public string RequestedDepartmentName { get; set; }
		public Guid? RequestorId { get; set; }
		public string RequestorUserName { get; set; }
		public string RequestorFullName { get; set; }
		public bool IsCompleted { get; set; }
		public Guid WorkflowInstanceId { get; set; }
		public DateTimeOffset Created { get; set; }
		public string Link { get; set; }
		public Guid? RegionId { get; set; }
		public string RegionName { get; set; }
	}
	public class FacilityObjectViewModel
	{
		public int Count { get; set; }
		public object Items { get; set; }
	}
}
