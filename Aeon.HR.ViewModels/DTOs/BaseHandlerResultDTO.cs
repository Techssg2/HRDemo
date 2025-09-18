using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.DTOs
{

	public class BaseHandlerResultDTO
	{
		public BaseHandlerResultDTO()
		{

		}

		#region Properties
		public bool success
		{
			get;
			set;
		}

		public string message
		{
			get;
			set;
		}

		public object data
		{
			get;
			set;
		}
		#endregion
	}
}