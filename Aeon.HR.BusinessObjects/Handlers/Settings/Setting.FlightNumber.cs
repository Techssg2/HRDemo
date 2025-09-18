using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        public async Task<ResultDTO> CheckValidateFlightNumberCode(FlightNumberArg arg)
        {
            var result = new ResultDTO();
            var flightNumbers = await _uow.GetRepository<FlightNumber>().FindByAsync(x => x.Code.ToLower() == arg.Code.ToLower());
            result.Object = new ArrayResultDTO { Data = flightNumbers, Count = flightNumbers.Count() };
            return result;
        }

        public async Task<ResultDTO> DeleteFlightNumber(Guid id)
        {
            var result = new ResultDTO();
            var flightNumberExist = await _uow.GetRepository<FlightNumber>().GetSingleAsync(x => x.Id == id);
            if (flightNumberExist != null)
            {
                _uow.GetRepository<FlightNumber>().Delete(flightNumberExist);
                await _uow.CommitAsync();
            }
            return result;
        }

        public async Task<ResultDTO> GetFlightNumberById(Guid id)
        {
            var result = new ResultDTO();
            var flightNumber = await _uow.GetRepository<FlightNumber>().GetSingleAsync<FlightNumberViewModel>(x => x.Id == id);
            result.Object = new ArrayResultDTO { Data = flightNumber };
            return result;
        }

        public async Task<ResultDTO> GetFlightNumbers(QueryArgs arg)
        {
            var airlines = await _uow.GetRepository<FlightNumber>().FindByAsync<FlightNumberViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var total = await _uow.GetRepository<FlightNumber>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = airlines, Count = total } };
        }

        public async Task<ResultDTO> SaveFlightNumber(FlightNumberArg arg)
        {
            var result = new ResultDTO();
            var flightNumberExist = await _uow.GetRepository<FlightNumber>().GetSingleAsync(x => x.Id == arg.Id);
            if (flightNumberExist == null)
            {
                var flightNumber = Mapper.Map<FlightNumber>(arg);
                _uow.GetRepository<FlightNumber>().Add(flightNumber);
            }
            else
            {
                var flightNumber = Mapper.Map(arg, flightNumberExist);
                _uow.GetRepository<FlightNumber>().Update(flightNumber);
            }
            await _uow.CommitAsync();
            return result;
        }
    }
}
