using AutoMapper;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.Args;
using System.Web;
using Aeon.HR.Infrastructure.Enums;
using System.Text.RegularExpressions;
using System.Threading;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        #region ReferencyNumber
        public async Task<ResultDTO> GetReferencyNumberRecruiments()
        {
            QueryArgs arg = new QueryArgs();
            arg.Page = 1;
            arg.Limit = int.MaxValue;
            arg.Order = "ModuleType asc";
            var referencyNumbers = await _uow.GetRepository<ReferenceNumber>().GetAllAsync<ReferencyNumberViewModel>(arg.Order, arg.Page, arg.Limit);
            return new ResultDTO { Object = new ArrayResultDTO { Data = referencyNumbers, Count = referencyNumbers.Count() } };
        }

        public async Task<ResultDTO> UpdateReferencyNumberRecruitment(ReferencyNumberArgs data)
        {
            var referencyNumber = _uow.GetRepository<ReferenceNumber>().FindBy(x => x.Id == data.Id).SingleOrDefault();
            if (referencyNumber.Equals(null))
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                referencyNumber.IsNewYearReset = data.IsNewYearReset;
                referencyNumber.CurrentNumber = data.CurrentNumber;
                referencyNumber.Formula = data.Formula;
                _uow.GetRepository<ReferenceNumber>().Update(referencyNumber);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        #endregion
    }
}