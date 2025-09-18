using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure;
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
        public async Task<ResultDTO> GetTimeSettings(MasterdataArgs args)
        {
            var result = new ResultDTO();
            var reasons = await _uow.GetRepository<MasterData>().FindByAsync(i=> i.MetadataType.Value == args.Type);
            var total = reasons.Count();
            result.Object = new ArrayResultDTO { Data = reasons, Count = total };
            return result;
        }
        public async Task<ResultDTO> UpdateConfiguration(ConfigurationViewModel args)
        {
            var result = new ResultDTO();
            var recordInDb = await _uow.GetRepository<MasterData>().GetSingleAsync(x => x.Id == args.Id);
            if(recordInDb != null)
            {
                recordInDb.Code = args.Code;
                _uow.GetRepository<MasterData>().Update(recordInDb);
                await _uow.CommitAsync();
            }
            return result;
        }
    }
}
