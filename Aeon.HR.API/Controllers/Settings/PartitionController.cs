using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.DTOs;

namespace SSG2.API.Controllers.Settings
{
    public class PartitionSettingController : SettingController
    {
        public PartitionSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<ResultDTO> GetListPartitions(QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetListPartitions(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListPartitions", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreatePartition(PartitionArgs data)
        {
            try
            {
                var res = _settingBO.CreatePartition(data);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreatePartition", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeletePartition(PartitionArgs data)
        {
            try
            {
                var res = _settingBO.DeletePartition(data);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeletePartition", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdatePartition(PartitionArgs data)
        {
            try
            {
                var res = await _settingBO.UpdatePartition(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdatePartition", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetPartitionByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetPartitionByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPartitionByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}
