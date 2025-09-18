using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        public async Task<ResultDTO> GetTrackingRequest(QueryArgs args)
        {
            var result = new ResultDTO();
            var items = await _uow.GetRepository<TrackingRequest>().FindByAsync<TrackingRequestForGetListViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var total = items.Count();
            if (total > 0)
            {
                foreach (var item in items)
                {
                    item.ActionDescription = Enums.GetDescription(item.Action);
                }

                total = await _uow.GetRepository<TrackingRequest>().CountAsync(args.Predicate, args.PredicateParameters);
            }

            result.Object = new ArrayResultDTO { Data = items, Count = total };
            return result;
        }

        public async Task<ResultDTO> UpdatePayloadById(EditPayloadArgs args)
        {
            var result = new ResultDTO();
            try
            {
                if (string.IsNullOrEmpty(args.Payload))
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Payload is not null" };
                    goto Finish;
                }
                var trackingRequest = await _uow.GetRepository<TrackingRequest>().GetSingleAsync(x => x.Id == args.Id);
                if (trackingRequest is null)
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "TrackingRequest is null!" };
                    goto Finish;
                }

                trackingRequest.Payload = args.Payload;
                trackingRequest.HasTrackingLog = true;
                await _uow.CommitAsync();
                result.Object = new ArrayResultDTO { Data = trackingRequest, Count = 1 };
            }
            catch (Exception e)
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { e.Message };
                goto Finish;
            }
            Finish:
            return result;
        }
    }
}
