using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
	public partial class SettingBO
	{
		public async Task<ResultDTO> GetLogHistory(QueryArgs args)
		{
			var result = new ResultDTO();
			var items = await _uow.GetRepository<LogHistory>().FindByAsync<LogHistoryViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
			var total = items.Count();
			if (total > 0)
			{
				foreach (var item in items)
				{
					//item.ActionDescription = Enums.GetDescription(item.Action);
				}

				total = await _uow.GetRepository<LogHistory>().CountAsync(args.Predicate, args.PredicateParameters);
			}

			result.Object = new ArrayResultDTO { Data = items, Count = total };
			return result;
		}
		public async Task<ResultDTO> SaveLogHistory(LogHistoryViewModel args)
		{
			var result = new ResultDTO();
			//var items = await _uow.GetRepository<LogHistory>().GetSingleAsync(x => x.Id == args.Id);

			//if (items != null)
			//{
			if (args != null)
			{

				var adminHis = new LogHistory()
				{
					TypeAction = args.TypeAction,
					Comment = args.Comment,
					IdItem = args.IdItem,
					Document = args.Document,
					ModuleName = args.ModuleName,
					FullName = _uow.UserContext.CurrentUserFullName,
					UserName = _uow.UserContext.CurrentUserName,
					CreatedById = _uow.UserContext.CurrentUserId,
					ModifiedById = _uow.UserContext.CurrentUserId,
					OldData = args.OldData,
					NewData = args.NewData
				};
				_uow.GetRepository<LogHistory>().Add(adminHis);
				await _uow.CommitAsync();
			}

			return result;
		}
	}
}
