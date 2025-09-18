using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class CachingService : ICachingService
    {
        private static readonly object _lock = new object();
        public readonly IMemoryCache _cache;
        private IUnitOfWork _unitOfWork;
        public string MasterDataName { get; set; }
        private readonly string MasterDataInformationCachedName = "MasterDataInfo";
        public CachingService(IMemoryCache cache, IUnitOfWork unitOfWork, ISAPBO sap)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }
        public IDictionary<string, List<MasterExternalDataViewModel>> MasterExternalDatas
        {
            get
            {
                IDictionary<string, List<MasterExternalDataViewModel>> masterDataValues;
                lock (_lock)
                {
                    if (_cache.TryGetValue(MasterDataName, out masterDataValues))
                    {
                        // Phai lay theo luong, memory => sap => sql (khong duoc di nguoc).

                        return masterDataValues;
                        //var cachedData = _unitOfWork.GetRepository<CacheMasterData>().GetSingle(x => x.Name == MasterDataName);
                        //if (cachedData != null)
                        //{
                        //    masterDataValues = new Dictionary<string, List<MasterExternalDataViewModel>>() { [MasterDataName] = JsonConvert.DeserializeObject<List<MasterExternalDataViewModel>>(cachedData.Data) };
                        //    _cache.Set(MasterDataName, masterDataValues);
                        //}
                    }
                }
                return masterDataValues;
            }
            set
            {
                _cache.Set(MasterDataName, value);
            }
        }
        public IDictionary<string, string> MasterDataInformation
        {
            get
            {
                IDictionary<string, string> masterDataInformation;
                lock (_lock)
                {
                    if (!_cache.TryGetValue(MasterDataInformationCachedName, out masterDataInformation))
                    {
                        var cachedData = JsonHelper.GetJsonContentFromFile("Mapping", "master-data-info");
                        _cache.Set(MasterDataInformationCachedName, cachedData);
                    }
                }
                return masterDataInformation;
            }
        }
    }

}
