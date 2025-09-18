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
        public async Task<ResultDTO> DeleteAirline(Guid id)
        {
            var result = new ResultDTO();
            var airlineExist = await _uow.GetRepository<Airline>().GetSingleAsync(x => x.Id == id);
            if(airlineExist != null)
            {
                _uow.GetRepository<Airline>().Delete(airlineExist);
                await _uow.CommitAsync();
            }
            return result;
        }

        public async Task<ResultDTO> GetAirlineById(Guid id)
        {
            var result = new ResultDTO();
            var airline = await _uow.GetRepository<Airline>().GetSingleAsync<AirlineViewModel>(x => x.Id == id);
            result.Object = new ArrayResultDTO { Data = airline } ;
            return result;
        }

        public async Task<ResultDTO> GetAirlines(QueryArgs arg)
        {
            var airlines = await _uow.GetRepository<Airline>().FindByAsync<AirlineViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var total= await _uow.GetRepository<Airline>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = airlines, Count = total } };
        }

        public async Task<ResultDTO> SaveAirline(AirlineArg arg)
        {
            var result = new ResultDTO();
            var airlineExist = await _uow.GetRepository<Airline>().GetSingleAsync(x => x.Id == arg.Id);
            if(airlineExist == null)
            {
                var airline = Mapper.Map<Airline>(arg);
                _uow.GetRepository<Airline>().Add(airline);
            }
            else
            {
                var airline = Mapper.Map(arg, airlineExist);
                _uow.GetRepository<Airline>().Update(airline);
            }
            await _uow.CommitAsync();
            return result;
        }

        public async Task<ResultDTO> CheckValidateCode(AirlineArg arg)
        {
            var result = new ResultDTO();
            var airlines = await _uow.GetRepository<Airline>().FindByAsync(x => x.Code.ToLower() == arg.Code.ToLower());
            result.Object = new ArrayResultDTO { Data = airlines, Count = airlines.Count() };
            return result;
        }

        public async Task<ResultDTO> ValidateWhenFlightNumberUsed(Guid airlineId)
        {
            var result = new ResultDTO();
            var airlineIdIsExistFlightNumbers = await _uow.GetRepository<FlightNumber>().FindByAsync(x => x.AirlineId == airlineId);
            result.Object = new ArrayResultDTO { Data = airlineIdIsExistFlightNumbers, Count = airlineIdIsExistFlightNumbers.Count() };
            return result;
        }
    }
}
