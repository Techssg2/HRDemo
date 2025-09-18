using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class MasterDataBO : IMasterDataB0
    {
        private readonly IUnitOfWork _uow;
        protected readonly ISAPBO _sap;
        protected readonly ICachingService _cache;
        private readonly string _master_data_json_file = "master-data-info.json";
        private readonly string[] LocalMasterData = { "PreferedTime", "StartWorkingFrom", "Experience", "Skill", "Certificate", "Language", "ImportantFactor", "EducationLevel" };
        private readonly RemoteMasterDataDetailInformation[] RemoteMasterData =
        {
            new RemoteMasterDataDetailInformation ("WorkLocation","","GetWorkLocationSet", false),
            new RemoteMasterDataDetailInformation ("WorkLocationByCode","Btrtl eq '{0}'","GetWorkLocationSet", true),
            new RemoteMasterDataDetailInformation ("PersonalArea","Btrtl eq '{0}'","GetWorkLocationSet", true),
            new RemoteMasterDataDetailInformation ("Province","","GetProvinceSet",false),
            new RemoteMasterDataDetailInformation ("District","Zprovince eq '{0}'","GetProvinceSet",true),
            new RemoteMasterDataDetailInformation ("Ward","Zprovince eq '{0}' and  Zdistrict eq '{1}'","GetProvinceSet",true),
            new RemoteMasterDataDetailInformation ("ShiftCode","","GetShiftCodeSet",false),
            new RemoteMasterDataDetailInformation ("JobTitle","","GetTitleSet",false),
            new RemoteMasterDataDetailInformation ("JobTitleByCode","Stell eq '{0}'","GetTitleSet",true),
            new RemoteMasterDataDetailInformation ("Gender","","GetGenderSet",false),
            new RemoteMasterDataDetailInformation ("GenderByCode","Anred eq '{0}'","GetGenderSet",true),
            new RemoteMasterDataDetailInformation ("BranchOfStudy","","GetBranchOfStudySet",false),
            new RemoteMasterDataDetailInformation ("LeaveKind","","GetLeaveKindSet",false),
            new RemoteMasterDataDetailInformation ("ActionType","","GetActionTypesSet",false),
            new RemoteMasterDataDetailInformation ("EducationalEst","","GetEducationalEstSet",false),
            new RemoteMasterDataDetailInformation ("Institute","","GetInstituteSet",false),
            new RemoteMasterDataDetailInformation ("ActionTypeResignation","Massn eq 'Z9'","GetActionTypesSet",false),
            new RemoteMasterDataDetailInformation ("ActionTypePromoteAndTransfer","Massn eq 'Z3' or Massn eq 'Z6'","GetActionTypesSet",false),
            new RemoteMasterDataDetailInformation ("ContractType","","GetContractTypeSet",false),
            new RemoteMasterDataDetailInformation ("Ethnic","","GetEthnicSet",false),
            new RemoteMasterDataDetailInformation ("EmployeeGroupCode","","GetEmployeeGroupCodeSet",false),
            new RemoteMasterDataDetailInformation ("Position","","GetPositionSet",false),
            new RemoteMasterDataDetailInformation ("PositionByCode","Plans eq '{0}'","GetPositionSet",true),
            new RemoteMasterDataDetailInformation ("MaritalStatus","","GetMaritalStatusSet",false),
            new RemoteMasterDataDetailInformation ("Nationality","","GetNationalitySet",false),
            new RemoteMasterDataDetailInformation ("Relationship","","GetRelationshipSet",false),
            new RemoteMasterDataDetailInformation ("Religion","","GetReligionSet",false),
            new RemoteMasterDataDetailInformation ("ReasonHiring","Massn eq 'Z1'","GetActionTypesSet",false),
            new RemoteMasterDataDetailInformation ("EmployeeGroup","","GetEmployeeGroupCodeSet",false),
            new RemoteMasterDataDetailInformation ("EmployeeSubgroup","Persg eq '{0}'","GetEmployeeGroupCodeSet",true),
            new RemoteMasterDataDetailInformation ("CurrentShift","","GetCurrentShiftSet",false),
        };
        public MasterDataBO(IUnitOfWork uow, ICachingService cachingService, ISAPBO sap)
        {
            _cache = cachingService;
            _uow = uow;
            _sap = sap;

        }
        private async Task<List<MasterExternalDataViewModel>> GetMasterDataFromExternalAPI(RemoteMasterDataDetailInformation arg, string cacheName)
        {
            List<MasterExternalDataViewModel> result = null;
            ResultDTO masterDataResult = await _sap.GetMasterData(arg);
            var data = masterDataResult.Object;
            var jsonData = data != null ? JsonConvert.SerializeObject(data) : "";
            if (!string.IsNullOrEmpty(jsonData))
            {
                result = JsonConvert.DeserializeObject<List<MasterExternalDataViewModel>>(jsonData);
                _cache.MasterExternalDatas = new Dictionary<string, List<MasterExternalDataViewModel>>() { [cacheName] = result };
            }
            if (result != null && result.Any())
            {
                _uow.GetRepository<CacheMasterData>().Add(new CacheMasterData
                {
                    Name = cacheName,
                    Data = jsonData
                });
            }
            await _uow.CommitAsync();
            return result;
        }

        public async Task<ResultDTO> GetMasterDataValues(MasterDataArgs arg)
        {
            var result = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "No Data" }, Object = new ArrayResultDTO() };
            if (LocalMasterData.Any(x => x == arg.Name))
            {
                // Local Data
                var items = await _uow.GetRepository<MasterData>().FindByAsync<MasterDataViewModel>(x => x.MetadataType.Name == arg.Name, string.Empty);
                if (items != null && items.Any())
                {
                    result.ResetResult();
                    result.Object = new ArrayResultDTO { Data = items, Count = items.Count() };
                }
            }
            else
            {
                var instance = RemoteMasterData.FirstOrDefault(x => x.Name == arg.Name);
                _cache.MasterDataName = arg.Name;
                if (instance != null && !string.IsNullOrEmpty(arg.ParentCode) && instance.HasParentCode)
                {
                    if (arg.Name == "Ward")
                    {
                        var _prarams = arg.ParentCode.Split(',');
                        if (_prarams != null && _prarams.Length > 1)
                        {
                            _cache.MasterDataName = string.Format(_cache.MasterDataName + "{0}{1}", _prarams[0], _prarams[1]);
                        }
                        else
                        {
                            return result;
                        }
                    }
                    else
                    {
                        _cache.MasterDataName += arg.ParentCode;
                    }

                }
                var cacheData = _cache.MasterExternalDatas;
                if (cacheData != null && cacheData.Count > 0)
                {
                    cacheData.TryGetValue(_cache.MasterDataName, out List<MasterExternalDataViewModel> data);
                    if (data != null)
                    {
                        result.ResetResult();
                        result.Object = new ArrayResultDTO { Data = data, Count = data.Count };
                    }
                }
                else
                {
                    // Remote                    
                    if (instance != null)
                    {

                        if (string.IsNullOrEmpty(arg.ParentCode) && instance.HasParentCode)
                        {
                            result.ResetResult();
                            result.ErrorCodes.Add(1001);
                            result.Messages.Add("Parent Code not found");
                            result.Object = arg;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(instance.Filter))
                            {
                                if (arg.Name == "Ward")
                                {
                                    string[] prarams = arg.ParentCode.Split(',');
                                    instance.Filter = string.Format("$filter=" + instance.Filter, prarams[0], prarams[1]);
                                }
                                else
                                {
                                    instance.Filter = string.Format("$filter=" + instance.Filter, arg.ParentCode);
                                }
                            }
                            var data = await GetMasterDataFromExternalAPI(instance, _cache.MasterDataName);
                            if (data != null)
                            {
                                result.ResetResult();
                                result.Object = new ArrayResultDTO { Data = data, Count = data.Count };                               
                            }
                            else
                            {
                                result.ResetResult();
                                var cachedData = _uow.GetRepository<CacheMasterData>().GetSingle(x => x.Name == _cache.MasterDataName, "Created desc");
                                if (cachedData != null)
                                {
                                    var _data = JsonConvert.DeserializeObject<List<MasterExternalDataViewModel>>(cachedData.Data);                                    
                                    result.Object = new ArrayResultDTO { Data = _data, Count = _data.Count };
                                }
                                
                            }
                        }

                    }
                }
            }
            return result;
        }
    }

}
