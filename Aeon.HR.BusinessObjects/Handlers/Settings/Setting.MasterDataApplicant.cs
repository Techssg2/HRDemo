using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
	public partial class SettingBO
	{
		public async Task<ResultDTO> GetMasterDataApplicantList(QueryArgs args)
		{
			var masterDatas = await _uow.GetRepository<MasterData>().FindByAsync(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
			var vm = Mapper.Map<IEnumerable<MasterData>, IEnumerable<MasterDataViewModel>>(masterDatas);
			var count = await _uow.GetRepository<MasterData>().CountAsync(args.Predicate, args.PredicateParameters);

			return new ResultDTO
			{
				Object = new ArrayResultDTO { Data = vm, Count = count }
			};
		}
	}
}