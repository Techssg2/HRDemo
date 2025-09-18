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
        public async Task<ResultDTO> GetListPartitions(QueryArgs arg)
        {
            var partitions = await _uow.GetRepository<Partition>().FindByAsync<PartitionViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<Partition>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = partitions, Count = totalList } };
        }

        public async Task<ResultDTO> CreatePartition(PartitionArgs data)
        {
            var partition = Mapper.Map<Partition>(data);
            _uow.GetRepository<Partition>().Add(partition);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeletePartition(PartitionArgs data)
        {
            var partition = await _uow.GetRepository<Partition>().FindByAsync(x => x.Id == data.Id);
            if (partition == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<Partition>().Delete(partition);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }

        public async Task<ResultDTO> UpdatePartition(PartitionArgs data)
        {
            var partition = await _uow.GetRepository<Partition>().FindByAsync(x => x.Id == data.Id);
            if (partition == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                partition.FirstOrDefault().Code = data.Code;
                partition.FirstOrDefault().Name = data.Name;
                _uow.GetRepository<Partition>().Update(partition);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }

        public async Task<ResultDTO> GetPartitionByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var partitionSettings = await _uow.GetRepository<Partition>().FindByAsync<PartitionViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = partitionSettings, Count = partitionSettings.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var partitionSettings = await _uow.GetRepository<Partition>().FindByAsync<PartitionViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = partitionSettings, Count = partitionSettings.Count() };
            }
            return result;
        }

    }
}
