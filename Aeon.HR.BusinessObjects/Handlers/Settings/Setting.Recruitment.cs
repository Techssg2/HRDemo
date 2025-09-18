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
        #region recruitmentSetting - Categories
        public async Task<ResultDTO> GetRecruitmentCategories(QueryArgs arg)
        {
            var recruitmentCategories = await _uow.GetRepository<RecruitmentCategory>().FindByAsync<RecruitmentCategoryViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<RecruitmentCategory>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = recruitmentCategories, Count = totalList } };
        }
        public async Task<ResultDTO> SearchRecruitmentCategories(MasterdataArgs arg)
        {
            var categories = await _uow.GetRepository<RecruitmentCategory>().FindByAsync(arg.QueryArgs.Order, arg.QueryArgs.Page, arg.QueryArgs.Limit, arg.QueryArgs.Predicate, arg.QueryArgs.PredicateParameters);
            var totalList = await _uow.GetRepository<RecruitmentCategory>().CountAllAsync();
            return new ResultDTO { Object = new ArrayResultDTO { Data = categories, Count = totalList } };
            
        }
        public async Task<ResultDTO> CreateRecruitmentCategory(RecruitmentCategoryArgs data)
        {
            var category = Mapper.Map<RecruitmentCategory>(data);
            _uow.GetRepository<RecruitmentCategory>().Add(category);
            await _uow.CommitAsync();
            data.Id = category.Id;
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteRecruitmentCategory(RecruitmentCategoryArgs data)
        {
            var existCategory = await _uow.GetRepository<RecruitmentCategory>().FindByAsync(x => x.Id == data.Id);
            if (existCategory == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<RecruitmentCategory>().Delete(existCategory);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateRecruitmentCategory(RecruitmentCategoryArgs data)
        {
            var existCategory = await _uow.GetRepository<RecruitmentCategory>().FindByAsync(x => x.Id == data.Id);
            if (existCategory == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                existCategory.FirstOrDefault().Priority = data.Priority;
                existCategory.FirstOrDefault().Name = data.Name;
                existCategory.FirstOrDefault().ParentId = data.ParentId;
                _uow.GetRepository<RecruitmentCategory>().Update(existCategory);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        #endregion
        #region recruitmentSetting - WorkingTime
        public async Task<ResultDTO> GetWorkingTimeRecruiments(QueryArgs arg)
        {
            var workingTimes = await _uow.GetRepository<WorkingTimeRecruitment>().FindByAsync<WorkingTimeRecruimentViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<WorkingTimeRecruitment>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = workingTimes, Count = totalList } };
        }
        public async Task<ResultDTO> SearchWorkingTimeRecruiment(MasterdataArgs arg)
        {
            var workingTimes = await _uow.GetRepository<WorkingTimeRecruitment>().GetAllAsync(arg.QueryArgs.Order, arg.QueryArgs.Page, arg.QueryArgs.Limit);
            if (arg.QueryArgs.PredicateParameters[0].ToString() != "")
            {
                var workingTimesSeach = workingTimes.Where(x => x.Name.ToLower().Contains(arg.QueryArgs.PredicateParameters[0].ToString().ToLower()) || x.Code.ToLower().Contains(arg.QueryArgs.PredicateParameters[0].ToString().ToLower())).ToList();
                var data = Mapper.Map<IList<WorkingTimeRecruimentViewModel>>(workingTimesSeach);
                return new ResultDTO { Object = new ArrayResultDTO { Data = data, Count = data.Count } };
            }
            else
            {
                var data = Mapper.Map<IList<WorkingTimeRecruimentViewModel>>(workingTimes);
                var totalList = await _uow.GetRepository<WorkingTimeRecruitment>().CountAllAsync();
                return new ResultDTO {Object = new ArrayResultDTO { Data = data, Count = totalList } };
            }
        }
        public async Task<ResultDTO> CreateWorkingTimeRecruitment(WorkingTimeRecruimentArgs data)
        {
            var workingTime = Mapper.Map<WorkingTimeRecruitment>(data);
            _uow.GetRepository<WorkingTimeRecruitment>().Add(workingTime);
            await _uow.CommitAsync();
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> DeleteWorkingTimeRecruitment(WorkingTimeRecruimentArgs data)
        {
            var existWorkingTime = await _uow.GetRepository<WorkingTimeRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (existWorkingTime == null)
            {
                return new ResultDTO {  Object = data };
            }
            else
            {
                _uow.GetRepository<WorkingTimeRecruitment>().Delete(existWorkingTime);
                await _uow.CommitAsync();
            }
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> UpdateWorkingTimeRecruitment(WorkingTimeRecruimentArgs data)
        {
            var existWorkingTime = await _uow.GetRepository<WorkingTimeRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (existWorkingTime == null)
            {
                return new ResultDTO {  Object = data };
            }
            else
            {
                existWorkingTime.FirstOrDefault().Code = data.Code;
                existWorkingTime.FirstOrDefault().Name = data.Name;
                _uow.GetRepository<WorkingTimeRecruitment>().Update(existWorkingTime);
                await _uow.CommitAsync();
            }
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> GetWorkingTimeRecruimentByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var workingTime = await _uow.GetRepository<WorkingTimeRecruitment>().FindByAsync<WorkingTimeRecruimentViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = workingTime, Count = workingTime.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var workingTime = await _uow.GetRepository<WorkingTimeRecruitment>().FindByAsync<WorkingTimeRecruimentViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = workingTime, Count = workingTime.Count() };
            }

            return result;
        }
        #endregion

        #region recruitmentSetting - ItemList
        public async Task<ResultDTO> GetItemListRecruiments(QueryArgs arg)
        {
            var itemLists = await _uow.GetRepository<ItemListRecruitment>().FindByAsync<ItemListViewRecruitmentArgs>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<ItemListRecruitment>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO {  Object = new ArrayResultDTO { Data = itemLists, Count = totalList } };
        }
        public async Task<ResultDTO> SearchItemListRecruimentByName(MasterdataArgs arg)
        {
            var itemLists = await _uow.GetRepository<ItemListRecruitment>().GetAllAsync(arg.QueryArgs.Order, arg.QueryArgs.Page, arg.QueryArgs.Limit);
            if (arg.QueryArgs.PredicateParameters[0].ToString() != "")
            {
                var itemListsSearch = itemLists.Where(x => x.Name.ToLower().Contains(arg.QueryArgs.PredicateParameters[0].ToString().ToLower()) || x.Code.ToLower().Contains(arg.QueryArgs.PredicateParameters[0].ToString().ToLower())).ToList();
                var data = Mapper.Map<IList<ItemListViewRecruitmentViewModel>>(itemListsSearch);
                return new ResultDTO {  Object = new ArrayResultDTO { Data = data, Count = data.Count } };
            }
            else
            {
                var data = Mapper.Map<IList<ItemListViewRecruitmentViewModel>>(itemLists);
                var totalList = await _uow.GetRepository<ItemListRecruitment>().CountAllAsync();
                return new ResultDTO {  Object = new ArrayResultDTO { Data = data, Count = totalList } };
            }
        }
        public async Task<ResultDTO> CreateItemListRecruitment(ItemListViewRecruitmentArgs data)
        {

            var itemList = Mapper.Map<ItemListRecruitment>(data);
            _uow.GetRepository<ItemListRecruitment>().Add(itemList);
            await _uow.CommitAsync();
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> DeleteItemListRecruitment(ItemListViewRecruitmentArgs data)
        {
            var itemList = await _uow.GetRepository<ItemListRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (itemList == null)
            {
                return new ResultDTO {  Object = data };
            }
            else
            {
                _uow.GetRepository<ItemListRecruitment>().Delete(itemList);
                await _uow.CommitAsync();
            }
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> UpdateItemListRecruitment(ItemListViewRecruitmentArgs data)
        {
            var itemList = await _uow.GetRepository<ItemListRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (itemList == null)
            {
                return new ResultDTO {  Object = data };
            }
            else
            {
                itemList.FirstOrDefault().Code = data.Code;
                itemList.FirstOrDefault().Name = data.Name;
                itemList.FirstOrDefault().Unit = data.Unit;
                _uow.GetRepository<ItemListRecruitment>().Update(itemList);
                await _uow.CommitAsync();
            }
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> GetItemListByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var itemList = await _uow.GetRepository<ItemListRecruitment>().FindByAsync<ItemListViewRecruitmentViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = itemList, Count = itemList.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var itemList = await _uow.GetRepository<ItemListRecruitment>().FindByAsync<ItemListViewRecruitmentViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = itemList, Count = itemList.Count() };
            }

            return result;
        }
        #endregion

        #region recruitmentSetting - ApplicantStatus
        public async Task<ResultDTO> GetAllApplicantStatusRecruiments()
        {
            var applicantStatuses = await _uow.GetRepository<ApplicantStatusRecruitment>().GetAllAsync<ApplicantStatusRecruitmentViewModel>();
            return new ResultDTO { Object = new ArrayResultDTO { Data = applicantStatuses, Count = applicantStatuses.Count()} };
        }
        public async Task<ResultDTO> GetApplicantStatusRecruiments(QueryArgs arg)
        {
            var applicantStatuses = await _uow.GetRepository<ApplicantStatusRecruitment>().FindByAsync<ApplicantStatusRecruitmentViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<ApplicantStatusRecruitment>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO {  Object = new ArrayResultDTO { Data = applicantStatuses, Count = totalList } };
        }
        public async Task<ResultDTO> SearchApplicantStatusRecruimentByName(MasterdataArgs arg)
        {
            var applicantStatuses = await  _uow.GetRepository<ApplicantStatusRecruitment>().GetAllAsync(arg.QueryArgs.Order, arg.QueryArgs.Page, arg.QueryArgs.Limit);
            if (arg.QueryArgs.PredicateParameters[0].ToString() != "")
            {
                var applicantStatusesSearch = applicantStatuses.Where(x => x.Name.ToLower().Contains(arg.QueryArgs.PredicateParameters[0].ToString().ToLower()) || x.Code.ToLower().Contains(arg.QueryArgs.PredicateParameters[0].ToString().ToLower())).ToList();
                var data = Mapper.Map<IList<ApplicantStatusRecruitmentArgs>>(applicantStatusesSearch);
                return new ResultDTO {  Object = new ArrayResultDTO { Data = data, Count = data.Count } };
            }
            else
            {
                var data = Mapper.Map<IList<ApplicantStatusRecruitmentArgs>>(applicantStatuses);
                var totalList = await _uow.GetRepository<ApplicantStatusRecruitment>().CountAllAsync();
                return new ResultDTO {  Object = new ArrayResultDTO { Data = data, Count = totalList } };
            }
        }
        public async Task<ResultDTO> CreateApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data)
        {
            var applicantStatus = Mapper.Map<ApplicantStatusRecruitment>(data);
            _uow.GetRepository<ApplicantStatusRecruitment>().Add(applicantStatus);
            await _uow.CommitAsync();
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> DeleteApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data)
        {
            var applicantStatuse = await _uow.GetRepository<ApplicantStatusRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (applicantStatuse == null)
            {
                return new ResultDTO {  Object = data };
            }
            else
            {
                _uow.GetRepository<ApplicantStatusRecruitment>().Delete(applicantStatuse);
                await _uow.CommitAsync();
            }
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> UpdateApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data)
        {
            var applicantStatus = await _uow.GetRepository<ApplicantStatusRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (applicantStatus == null)
            {
                return new ResultDTO {  Object = data };
            }
            else
            {
                applicantStatus.FirstOrDefault().Code = data.Code;
                applicantStatus.FirstOrDefault().Name = data.Name;
                applicantStatus.FirstOrDefault().Arrangement = data.Arrangement;
                _uow.GetRepository<ApplicantStatusRecruitment>().Update(applicantStatus);
                await _uow.CommitAsync();
            }
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> GetApplicantStatusRecruitmentByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var applicantStatus = await _uow.GetRepository<ApplicantStatusRecruitment>().FindByAsync<ApplicantStatusRecruitmentViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = applicantStatus, Count = applicantStatus.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var applicantStatus = await _uow.GetRepository<ApplicantStatusRecruitment>().FindByAsync<ApplicantStatusRecruitmentViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = applicantStatus, Count = applicantStatus.Count() };
            }

            return result;
        }
        #endregion

        #region recruitmentSetting - AppreciationList
        public async Task<ResultDTO> GetAppreciationListRecruiments(QueryArgs arg)
        {
            var appreciationLists = await _uow.GetRepository<AppreciationListRecruitment>().FindByAsync<AppreciationListRecruitmentViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<AppreciationListRecruitment>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO {  Object = new ArrayResultDTO { Data = appreciationLists, Count = totalList } };
        }
        public async Task<ResultDTO> SearchAppreciationListRecruimentByName(MasterdataArgs arg)
        {
            var appreciationLists = await _uow.GetRepository<AppreciationListRecruitment>().GetAllAsync(arg.QueryArgs.Order, arg.QueryArgs.Page, arg.QueryArgs.Limit);
            if(arg.QueryArgs.PredicateParameters[0].ToString() != "")
            {
                var appreciationListsSearch = appreciationLists.Where(x => x.Name.ToLower().Contains(arg.QueryArgs.PredicateParameters[0].ToString().ToLower()) || x.Code.ToLower().Contains(arg.QueryArgs.PredicateParameters[0].ToString().ToLower())).ToList();
                var data = Mapper.Map<IList<AppreciationListRecruitmentViewModel>>(appreciationListsSearch);
                return new ResultDTO {  Object = new ArrayResultDTO { Data = data, Count = data.Count } };
            }
            else
            {
                var data = Mapper.Map<IList<AppreciationListRecruitmentViewModel>>(appreciationLists);
                var totalList = await _uow.GetRepository<AppreciationListRecruitment>().CountAllAsync();
                return new ResultDTO {  Object = new ArrayResultDTO { Data = data, Count = totalList } };
            }
        }
        public async Task<ResultDTO> CreateAppreciationListRecruitment(AppreciationListRecruitmentArgs data)
        {

            var appreciationList = Mapper.Map<AppreciationListRecruitment>(data);
            _uow.GetRepository<AppreciationListRecruitment>().Add(appreciationList);
            await _uow.CommitAsync();
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> DeleteAppreciationListRecruitment(AppreciationListRecruitmentArgs data)
        {
            var appreciationList = await _uow.GetRepository<AppreciationListRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (appreciationList == null)
            {
                return new ResultDTO {  Object = data };
            }
            else
            {
                _uow.GetRepository<AppreciationListRecruitment>().Delete(appreciationList);
                await _uow.CommitAsync();
            }
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> UpdateAppreciationListRecruitment(AppreciationListRecruitmentArgs data)
        {
            var appreciationList = await _uow.GetRepository<AppreciationListRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (appreciationList == null)
            {
                return new ResultDTO {  Object = data };
            }
            else
            {
                appreciationList.FirstOrDefault().Code = data.Code;
                appreciationList.FirstOrDefault().Name = data.Name;
                _uow.GetRepository<AppreciationListRecruitment>().Update(appreciationList);
                await _uow.CommitAsync();
            }
            return new ResultDTO {  Object = data };
        }
        public async Task<ResultDTO> GetAppreciationListRecruimentByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var appreciationList = await _uow.GetRepository<AppreciationListRecruitment>().FindByAsync<AppreciationListRecruitmentViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = appreciationList, Count = appreciationList.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var appreciationList = await _uow.GetRepository<AppreciationListRecruitment>().FindByAsync<AppreciationListRecruitmentViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = appreciationList, Count = appreciationList.Count() };
            }
                
            return result;
        }
        #endregion


        #region recruitmentSetting - Position
        public async Task<ResultDTO> GetPositionRecruiments(QueryArgs arg)
        {
            var positions = await _uow.GetRepository<MasterData>().FindByAsync<ReasonViewModelForCABSetting>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<MasterData>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = positions, Count = totalList } };
        }
       
        public async Task<ResultDTO> CreatePositionRecruitment(PositionRecruitmentArgs data)
        {
            var positionMetadataType = await _uow.GetRepository<MetadataType>().GetSingleAsync(x => x.Value == data.TypeName);
            var position = new MasterData
            {
                Code = data.Code,
                Name = data.Name,
                JobGradeId = data.JobGradeId,
                MetaDataTypeId = positionMetadataType.Id
            };
            _uow.GetRepository<MasterData>().Add(position);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeletePositionRecruitment(PositionRecruitmentArgs data)
        {
            var position = await _uow.GetRepository<MasterData>().FindByAsync(x => x.Id == data.Id && x.MetadataType.Value == data.TypeName);
            if (position == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<MasterData>().Delete(position);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdatePositionRecruitment(PositionRecruitmentArgs data)
        {
            var position = await _uow.GetRepository<MasterData>().GetSingleAsync(x => x.Id == data.Id && x.MetadataType.Value == data.TypeName);
            if (position == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                position.Code = data.Code;
                position.Name = data.Name;
                position.JobGradeId = data.JobGradeId;
                _uow.GetRepository<MasterData>().Update(position);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> GetPositionRecruimentByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var code = arg.PredicateParameters[0].ToString().ToLower();
            var metadataType = arg.PredicateParameters[1].ToString().ToLower();
            if (arg.PredicateParameters[2] == null)
            {
                var positions = await _uow.GetRepository<MasterData>().FindByAsync<ReasonViewModelForCABSetting>(x => x.Code.ToLower() == code && x.MetadataType.Value == metadataType);
                result.Object = new ArrayResultDTO { Data = positions, Count = positions.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[2].ToString().ToLower());
                var positions = await _uow.GetRepository<MasterData>().FindByAsync<ReasonViewModelForCABSetting>(x => x.Code.ToLower() == code && x.MetadataType.Value == metadataType && x.Id != id);
                result.Object = new ArrayResultDTO { Data = positions, Count = positions.Count() };
            }

            return result;
        }
        #endregion

        #region recruitmentSetting - CostCenter
        public async Task<ResultDTO> GetCostCenterRecruiments(QueryArgs arg)
        {
            var costCenters = await _uow.GetRepository<CostCenterRecruitment>().FindByAsync<CostCenterRecruitmentViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<CostCenterRecruitment>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = costCenters, Count = totalList } };
        }
        public async Task<ResultDTO> GetCostCenterRecruitmentByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var costCenter = await _uow.GetRepository<CostCenterRecruitment>().FindByAsync<CostCenterRecruitmentViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = costCenter, Count = costCenter.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var costCenter = await _uow.GetRepository<CostCenterRecruitment>().FindByAsync<CostCenterRecruitmentViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = costCenter, Count = costCenter.Count() };
            }

            return result;
        }
        public async Task<ResultDTO> CreateCostCenterRecruitment(CostCenterRecruitmentArgs data)
        {
            var costCenters = Mapper.Map<CostCenterRecruitment>(data);
            _uow.GetRepository<CostCenterRecruitment>().Add(costCenters);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteCostCenterRecruitment(CostCenterRecruitmentArgs data)
        {
            var costCenter = await _uow.GetRepository<CostCenterRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (costCenter == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<CostCenterRecruitment>().Delete(costCenter);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateCostCenterRecruitment(CostCenterRecruitmentArgs data)
        {
            var costCenters = await _uow.GetRepository<CostCenterRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (costCenters == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                costCenters.FirstOrDefault().Code = data.Code;
                costCenters.FirstOrDefault().Description = data.Description;
                _uow.GetRepository<CostCenterRecruitment>().Update(costCenters);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> GetCostCenterByDepartmentId(Guid id)
        {
            var allDepartments = await _uow.GetRepository<Department>().GetAllAsync();
            var costCenterRecruitmentId = FindCostInDeparments(allDepartments, id);
            return new ResultDTO { Object = costCenterRecruitmentId };
        }

        Guid? FindCostInDeparments(IEnumerable<Department> allDepartments, Guid id)
        {
            var currentDepartment = allDepartments.FirstOrDefault(x => x.Id == id);

            if (currentDepartment.CostCenterRecruitmentId != null)
            {
                return currentDepartment.CostCenterRecruitmentId;
            }

            if (currentDepartment.ParentId == null)
            {
                return null;
            }

            return FindCostInDeparments(allDepartments, currentDepartment.ParentId.Value);
        }
        #endregion

        #region recruitmentSetting - Working Address
        public async Task<ResultDTO> GetWorkingAddressRecruiments(QueryArgs arg)
        {
            var workingAddress = await _uow.GetRepository<WorkingAddressRecruitment>().FindByAsync<WorkingAddressRecruitmentViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<WorkingAddressRecruitment>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = workingAddress, Count = totalList } };
        }
        public async Task<ResultDTO> GetWorkingAddressRecruimentByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var workingAddress = await _uow.GetRepository<WorkingAddressRecruitment>().FindByAsync<WorkingAddressRecruitmentViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = workingAddress, Count = workingAddress.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var workingAddress = await _uow.GetRepository<WorkingAddressRecruitment>().FindByAsync<WorkingAddressRecruitmentViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = workingAddress, Count = workingAddress.Count() };
            }

            return result;
        }
        public async Task<ResultDTO> CreateWorkingAddressRecruiment(WorkingAddressRecruimentArgs data)
        {
            var workingAddress = Mapper.Map<WorkingAddressRecruitment>(data);
            _uow.GetRepository<WorkingAddressRecruitment>().Add(workingAddress);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteWorkingAddressRecruiment(WorkingAddressRecruimentArgs data)
        {
            var workingAddress = await _uow.GetRepository<WorkingAddressRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (workingAddress == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<WorkingAddressRecruitment>().Delete(workingAddress);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateWorkingAddressRecruiment(WorkingAddressRecruimentArgs data)
        {
            var workingAddress = await _uow.GetRepository<WorkingAddressRecruitment>().FindByAsync(x => x.Id == data.Id);
            if (workingAddress == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                workingAddress.FirstOrDefault().Code = data.Code;
                workingAddress.FirstOrDefault().Address = data.Address;
                _uow.GetRepository<WorkingAddressRecruitment>().Update(workingAddress);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        #endregion
        #region recruitmentSetting - Promote And Tranfer Print - Removing
        public async Task<ResultDTO> GetPromoteAndTranferPrintValue(QueryArgs arg)
        {
            var removingValues = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().FindByAsync<PromoteAndTranferPrintViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = removingValues, Count = totalList } };
        }
        public async Task<ResultDTO> GetPromoteAndTranferPrintByName(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var removingValue = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().FindByAsync<PromoteAndTranferPrintViewModel>(x => x.RemovingValue.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = removingValue, Count = removingValue.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var removingValue = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().FindByAsync<PromoteAndTranferPrintViewModel>(x => x.RemovingValue.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = removingValue, Count = removingValue.Count() };
            }

            return result;
        }
        public async Task<ResultDTO> CreatePromoteAndTranferPrint(PromoteAndTranferPrintArgs data)
        {
            var removingValue = Mapper.Map<MaintainPromoteAndTranferPrint>(data);
            _uow.GetRepository<MaintainPromoteAndTranferPrint>().Add(removingValue);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeletePromoteAndTranferPrint(PromoteAndTranferPrintArgs data)
        {
            var removingValue = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().FindByAsync(x => x.Id == data.Id);
            if (removingValue == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<MaintainPromoteAndTranferPrint>().Delete(removingValue);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdatePromoteAndTranferPrint(PromoteAndTranferPrintArgs data)
        {
            var removingValue = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().FindByAsync(x => x.Id == data.Id);
            if (removingValue == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                removingValue.FirstOrDefault().RemovingValue = data.RemovingValue;
                _uow.GetRepository<MaintainPromoteAndTranferPrint>().Update(removingValue);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        #endregion
    }
}