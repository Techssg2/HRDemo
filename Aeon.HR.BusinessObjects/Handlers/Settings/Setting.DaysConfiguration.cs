using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Newtonsoft.Json;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using Aeon.HR.Infrastructure.Constants;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        public async Task<ResultDTO> GetSalaryDayConfigurations(QueryArgs arg)
        {
            var daysConfigurations = (await _uow.GetRepository<DaysConfiguration>().FindByAsync(arg.Order, arg.Page, Int32.MaxValue, arg.Predicate, arg.PredicateParameters)).ToList();
            var list = daysConfigurations.Select(x => new { Name = x.Name, Value = x.Value }).ToList();
            var totalList = list.Count();
            return new ResultDTO { Object = new ArrayResultDTO { Data = list, Count = totalList } };
        }
        private async Task AddUpdateDayConfiguration(string name, int value)
        {
            var exist = await _uow.GetRepository<DaysConfiguration>().GetSingleAsync(x => x.Name == name);
            if (exist != null)
            {
                exist.Value = value;
            }
            else _uow.GetRepository<DaysConfiguration>().Add(new DaysConfiguration()
            {
                Name = name,
                Value = value
            });
            await _uow.CommitAsync();
        }
        public async Task<ResultDTO> SaveSalaryDayConfiguration(SalaryDayConfigurationArg arg)
        {
            //CR321
            List<string> listOfGeneralConfig = new List<string> { "DeadlineOfSubmittingCABApplication", "CreatedNewPeriodDate", "SalaryPeriodFrom",
            "SalaryPeriodTo", "NewSalaryPeriod"};

            List<string> listOfCABConfig = new List<string> { "DeadlineOfSubmittingCABHQ", "DeadlineOfSubmittingCABStore", "TimeOfSubmittingCABHQ",
            "TimeOfSubmittingCABStore"};

            var result = new ResultDTO();
            foreach (var prop in arg.GetType().GetProperties())
            {
				switch (arg.DayConfigurationType)
				{
                    case "General":
                        {
							if (listOfGeneralConfig.Contains(prop.Name))
							{
                                var val = Convert.ToInt32(prop.GetValue(arg, null));
                                await AddUpdateDayConfiguration(prop.Name, val);
                            }
                        }
                        break;
                    case "CAB":
                        {
                            if (listOfCABConfig.Contains(prop.Name))
                            {
                                var val = Convert.ToInt32(prop.GetValue(arg, null));
                                await AddUpdateDayConfiguration(prop.Name, val);
                            }
                        }
                        break;
                    default:
                        var value = Convert.ToInt32(prop.GetValue(arg, null));
                        await AddUpdateDayConfiguration(prop.Name, value);
                        break;
				}				
            }
            return result;
            //         var result = new ResultDTO();

            //         var dayExists = await _uow.GetRepository<DaysConfiguration>().GetSingleAsync(x => x.Id == arg.Id);
            //if(dayExists == null)
            //         {
            //             var data = Mapper.Map<DaysConfiguration>(arg);
            //             _uow.GetRepository<DaysConfiguration>().Add(data);
            //         }
            //else
            //         {
            //             dayExists.SalaryPeriodFrom = arg.SalaryPeriodFrom;
            //             dayExists.SalaryPeriodTo = arg.SalaryPeriodTo;
            //             dayExists.DeadlineOfSubmittingCABApplication = arg.DeadlineOfSubmittingCABApplication;
            //             _uow.GetRepository<DaysConfiguration>().Update(dayExists);
            //         }
            //         await _uow.CommitAsync();
            //         return result;
        }
    }
}
