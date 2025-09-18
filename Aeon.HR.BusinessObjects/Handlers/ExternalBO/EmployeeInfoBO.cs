using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.ExternalItem;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.BusinessObjects.Handlers.Other;

namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    public class EmployeeInfoBO : ExternalExcution
    {
        private readonly Applicant model;
        private IMasterDataB0 _masterbo;
        private ISAPBO _sapBo;
        public EmployeeInfoBO(ILogger log, IUnitOfWork uow, Applicant applicant, ITrackingBO _trackingBO, IMasterDataB0 masterbo) : base(log, uow, "employeeInfo.json", applicant, _trackingBO)
        {
            model = applicant;
            _masterbo = masterbo;
            _sapBo = new SAPBO();
        }
        public override async Task SubmitData(bool allowSendToSAP)
        {

            var mapping = Mapper.Map<EmployeeInfo>(model);
            mapping.EdocId = mapping.EdocId.ToUpper();
            var requestor = await _uow.GetRepository<User>().FindByIdAsync(_uow.UserContext.CurrentUserId);
            if(requestor!= null)
            {
                mapping.EdocUser = requestor.SAPCode;
            }
            var keys = new List<string>();
            var sapCode = "";
            if (!string.IsNullOrEmpty(model.IDCard9Number))
            {
                keys.Add(model.IDCard9Number);
            }
            if (!string.IsNullOrEmpty(model.IDCard12Number))
            {
                keys.Add(model.IDCard12Number);
            }
            if (!string.IsNullOrEmpty(model.PassportNo))
            {
                keys.Add(model.PassportNo);
            }
            if (keys.Count > 0)
            {
                sapCode = await _sapBo.CheckValidEmployeeFromSAP(keys.ToArray());
            }
            ItemId = model.Id;
            mapping.Title = model.AdditionalPositionCode;
            mapping.SapCode = sapCode;
            mapping.UpdateEmployee = !string.IsNullOrEmpty(sapCode) ? "X" : "";
            mapping.IDCardInfos.Add(new IDCardInfo
            {
                IDCardType = "01",
                IDCardDate = model.IDCard9Date.HasValue ? model.IDCard9Date.Value.LocalDateTime.ToSAPFormat() : "",
                IDCardNo = model.IDCard9Number,
                IDCardPlace = model.IDCard9PlaceName,
            });
            if (model.HaveIdentifyCardNumber12.HasValue && !string.IsNullOrEmpty(model.IDCard12Number))
            {
                mapping.IDCardInfos.Add(new IDCardInfo
                {
                    IDCardType = "02",
                    IDCardDate = model.IDCard12Date.HasValue ? model.IDCard12Date.Value.LocalDateTime.ToSAPFormat() : "",
                    IDCardNo = model.IDCard12Number,
                    IDCardPlace = model.IDCard12PlaceName,
                });
            }

            if (!string.IsNullOrEmpty(mapping.WorkLocation))
            {
                var personnelArea = await GetMasterNameDataByCode("PersonalArea", mapping.WorkLocation);
                if (personnelArea != null)
                {
                    mapping.PersonnelArea = personnelArea.Code;
                }
            }
            mapping.AddressInfomations.Add(new AddressInfo
            {
                Address = model.PermanentResidentAddress,
                AddressType = "1",
                CountryCode = "",
                DistrictCode = model.PermanentResidentCityCode,
                ProvinceCode = model.PermanentResidentDistrictCode,
                WardCode = model.PermanentResidentWardCode
            });
            mapping.AddressInfomations.Add(new AddressInfo
            {
                Address = model.ProvisionalResidentAddress,
                AddressType = "2",
                CountryCode = "",
                DistrictCode = model.ProvisionalResidentCityCode,
                ProvinceCode = model.ProvisionalResidentDistrictCode,
                WardCode = model.ProvisionalResidentWardCode
            });
            if (model.ApplicantEducations.Count > 0)
            {
                foreach (var item in mapping.ApplicantEducations)
                {
                    item.EducationLevel = model.EducationLevelCode;
                }
            }
            base.SetSAPEntity(mapping);
            await SubmitAPI(allowSendToSAP);
        }
        public override void ConvertToPayload()
        {

        }

        public override Task<object> GetData(string predicate, string[] param)
        {
            throw new System.NotImplementedException();
        }
        private async Task<MasterDataViewModel> GetMasterNameDataByCode(string type, string code)
        {
            MasterDataViewModel result = null;
            if (!string.IsNullOrEmpty(code))
            {
                var masterData = await _masterbo.GetMasterDataValues(new MasterDataArgs { Name = type, ParentCode = code });
                if (masterData.IsSuccess)
                {
                    var arrayData = JsonConvert.DeserializeObject<ArrayResultDTO>(JsonConvert.SerializeObject(masterData.Object)).Data;
                    List<MasterDataViewModel> masterDataValues = Mapper.Map<List<MasterDataViewModel>>(arrayData);
                    result = masterDataValues.FirstOrDefault();
                }
            }
            return result;

        }
     
    }
}