using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Settings
{
    public class RecruitmentSettingController : SettingController
    {
        public RecruitmentSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        #region Categories
        [HttpPost]
        public async Task<ResultDTO> GetRecruitmentCategories([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetRecruitmentCategories(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetRecruitmentCategories", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> SearchRecruitmentCategories([FromBody] MasterdataArgs arg)
        {
            try
            {
                var res = _settingBO.SearchRecruitmentCategories(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchRecruitmentCategories", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateRecruitmentCategory(RecruitmentCategoryArgs data)
        {
            try
            {
                var res = await _settingBO.CreateRecruitmentCategory(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateRecruitmentCategory", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteRecruitmentCategory(RecruitmentCategoryArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteRecruitmentCategory(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteRecruitmentCategory", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateRecruitmentCategory(RecruitmentCategoryArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateRecruitmentCategory(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateRecruitmentCategory", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion
        #region WorkingTime 
        [HttpPost]
        public async Task<ResultDTO> GetWorkingTimeRecruiments([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetWorkingTimeRecruiments(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetWorkingTime", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> SearchWorkingTimeRecruiment([FromBody] MasterdataArgs arg)
        {
            try
            {
                var res = _settingBO.SearchWorkingTimeRecruiment(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchWorkingTime", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateWorkingTimeRecruitment(WorkingTimeRecruimentArgs data)
        {
            try
            {
                var res = await _settingBO.CreateWorkingTimeRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateWorkingTime", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteWorkingTimeRecruitment(WorkingTimeRecruimentArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteWorkingTimeRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteWorkingTime", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateWorkingTimeRecruitment(WorkingTimeRecruimentArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateWorkingTimeRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateWorkingTime", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetWorkingTimeRecruimentByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetWorkingTimeRecruimentByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetWorkingTimeRecruimentByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion

        #region ItemList 
        [HttpPost]
        public async Task<ResultDTO> GetItemListRecruiments([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetItemListRecruiments(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetItemList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> SearchItemListRecruimentByName([FromBody] MasterdataArgs arg)
        {
            try
            {
                var res = await _settingBO.SearchItemListRecruimentByName(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchItemList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateItemListRecruitment(ItemListViewRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.CreateItemListRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateWorkingTime", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteItemListRecruitment(ItemListViewRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteItemListRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteWorkingTime", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateItemListRecruitment(ItemListViewRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateItemListRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateWorkingTime", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetItemListByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetItemListByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetItemListByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion

        #region recruitmentSetting - ApplicantStatus 
        [HttpPost]
        public async Task<ResultDTO> GetAllApplicantStatusRecruiments()
        {
            try
            {
                return await _settingBO.GetAllApplicantStatusRecruiments();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetApplicantStatusRecruiments", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetApplicantStatusRecruiments([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetApplicantStatusRecruiments(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetApplicantStatusRecruiments", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> SearchApplicantStatusRecruimentByName([FromBody] MasterdataArgs arg)
        {
            try
            {
                var res = _settingBO.SearchApplicantStatusRecruimentByName(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchApplicantStatus", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.CreateApplicantStatusRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateApplicantStatus", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteApplicantStatusRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteApplicantStatus", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateApplicantStatusRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateApplicantStatus", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetApplicantStatusRecruitmentByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetApplicantStatusRecruitmentByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetApplicantStatusRecruitmentByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion

        #region recruitmentSetting - AppreciationList 
        [HttpPost]
        public async Task<ResultDTO> GetAppreciationListRecruiments([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetAppreciationListRecruiments(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> SearchAppreciationListRecruimentByName([FromBody] MasterdataArgs arg)
        {
            try
            {
                var res = _settingBO.SearchAppreciationListRecruimentByName(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateAppreciationListRecruitment(AppreciationListRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.CreateAppreciationListRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteAppreciationListRecruitment(AppreciationListRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteAppreciationListRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateAppreciationListRecruitment(AppreciationListRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateAppreciationListRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateAppreciationList", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetAppreciationListRecruimentByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetAppreciationListRecruimentByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAppreciationListRecruimentByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion

        #region recruitmentSetting - PositionList 
        [HttpPost]
        public async Task<ResultDTO> GetPositionRecruiments([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetPositionRecruiments(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPositionRecruiments", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreatePositionRecruitment(PositionRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.CreatePositionRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreatePositionRecruitment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeletePositionRecruitment(PositionRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.DeletePositionRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeletePositionRecruitment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdatePositionRecruitment(PositionRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.UpdatePositionRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdatePositionRecruitment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetPositionRecruimentByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetPositionRecruimentByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPositionRecruimentByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion

        #region recruitmentSetting - CostCenter
        [HttpPost]
        public async Task<ResultDTO> GetCostCenterRecruiments([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetCostCenterRecruiments(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetCostCenterRecruiments", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetCostCenterRecruitmentByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetCostCenterRecruitmentByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetCostCenterRecruitmentByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateCostCenterRecruitment(CostCenterRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.CreateCostCenterRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateCostCenterRecruitment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteCostCenterRecruitment(CostCenterRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteCostCenterRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteCostCenterRecruitment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateCostCenterRecruitment(CostCenterRecruitmentArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateCostCenterRecruitment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateCostCenterRecruitment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetCostCenterByDepartmentId(Guid id)
        {
            try
            {
                var res = await _settingBO.GetCostCenterByDepartmentId(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetCostCenterByDepartmentId", ex.Message);
                return BadRequest("Error System");
            }
        }
        #endregion

        #region recruitmentSetting - Working Address
        [HttpPost]
        public async Task<ResultDTO> GetWorkingAddressRecruiments([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetWorkingAddressRecruiments(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetWorkingAddressRecruiments", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetWorkingAddressRecruimentByCode(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetWorkingAddressRecruimentByCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetWorkingAddressRecruimentByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateWorkingAddressRecruiment(WorkingAddressRecruimentArgs data)
        {
            try
            {
                var res = await _settingBO.CreateWorkingAddressRecruiment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateWorkingAddressRecruiment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteWorkingAddressRecruiment(WorkingAddressRecruimentArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteWorkingAddressRecruiment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteWorkingAddressRecruiment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateWorkingAddressRecruiment(WorkingAddressRecruimentArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateWorkingAddressRecruiment(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateWorkingAddressRecruiment", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion

        #region recruitmentSetting - Promote And Tranfer Print - Removing
        [HttpPost]
        public async Task<ResultDTO> GetPromoteAndTranferPrintValue([FromBody] QueryArgs arg)
        {
            try
            {
                var res = _settingBO.GetPromoteAndTranferPrintValue(arg);
                return await res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPromoteAndTranferPrintValue", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetPromoteAndTranferPrintByName(QueryArgs data)
        {
            try
            {
                var res = await _settingBO.GetPromoteAndTranferPrintByName(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GePromoteAndTranferPrintByName", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreatePromoteAndTranferPrint(PromoteAndTranferPrintArgs data)
        {
            try
            {
                var res = await _settingBO.CreatePromoteAndTranferPrint(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreatePromoteAndTranferPrint", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeletePromoteAndTranferPrint(PromoteAndTranferPrintArgs data)
        {
            try
            {
                var res = await _settingBO.DeletePromoteAndTranferPrint(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeletePromoteAndTranferPrint", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdatePromoteAndTranferPrint(PromoteAndTranferPrintArgs data)
        {
            try
            {
                var res = await _settingBO.UpdatePromoteAndTranferPrint(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdatePromoteAndTranferPrint", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion
    }
}
