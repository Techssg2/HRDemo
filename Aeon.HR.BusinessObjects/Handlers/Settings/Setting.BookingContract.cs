using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        public async Task<ResultDTO> GetBookingContract(QueryArgs args)
        {
            var bookingContracts = await _uow.GetRepository<BookingContract>().FindByAsync<BookingContractViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<BookingContract>().CountAsync(args.Predicate, args.PredicateParameters);

            return new ResultDTO
            {
                Object = new ArrayResultDTO { Data = bookingContracts, Count = count }
            };

        }
        public async Task<ResultDTO> UpdateBookingContract(BookingContractArgs args)
        {
            var item = await _uow.GetRepository<BookingContract>().GetSingleAsync(x => x.Id == args.Id);
            if (item == null)
            {
                return new ResultDTO { Object = args };
            }
            else
            {
                item.FullName = args.FullName;
                item.EmailBookingContract = args.EmailBookingContract;
                item.PhoneNumber = args.PhoneNumber;
                _uow.GetRepository<BookingContract>().Update(item);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = item };
        }
    }
}