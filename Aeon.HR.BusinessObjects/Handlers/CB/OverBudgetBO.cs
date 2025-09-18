using Aeon.HR.BusinessObjects.Handlers.CB.Class;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.CB
{
    public class OverBudgetBO : IOverBudgetBO
    {
        private readonly IUnitOfWork _uow;
        private readonly IWorkflowBO _workflowBO;
        private readonly ILogger logger;
        public OverBudgetBO(IUnitOfWork uow, ILogger _logger, IWorkflowBO workflowBO)
        {
            _uow = uow;
            logger = _logger;
            _workflowBO = workflowBO;
        }

        /// <summary>
        /// Trả về phiếu overbudget từ Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultDTO> GetItemOverBudgetById(Guid id)
        {
            var result = new ResultDTO();
            var currentItem = await _uow.GetRepository<BusinessTripOverBudget>().FindByIdAsync<BTAOverBudgetViewModel>(id);
            if (currentItem != null)
            {
                var record = Mapper.Map<BusinessTripOverBudgetItemViewModel>(currentItem);
                await this.AddMoreProperties(record, record.Id);
                var details = _uow.GetRepository<BusinessTripOverBudgetsDetail>().FindBy<BusinessOverBudgetDetailDTO>(x => x.BusinessTripOverBudgetId == id, string.Empty).ToList();
                if (details != null && details.Any())
                {
                    foreach (var detail in details)
                    {
                        if (detail.FlightDetails != null)
                        {
                            var flightDetail = Mapper.Map<List<FlightDetailViewModel>>(JsonConvert.DeserializeObject<List<FlightDetailViewModel>>(detail.FlightDetails.ToString()));
                            //List<Information> userGroups = Mapper.Map<List<Information>>(JsonConvert.DeserializeObject<List<Information>>(f1.UserGroups));
                            detail.FlightDetails = flightDetail;
                        }
                    }
                    record.BusinessTripOverBudgetDetails = details.OrderBy(x => x.FromDate).ThenBy(x => x.ToDate);
                }

                result.Object = record;
            }
            else
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            return result;
        }

        public async Task AddMoreProperties(BusinessTripOverBudgetItemViewModel wfItem, Guid ItemId)
        {
            try
            {
                var bobEntity = await _uow.GetRepository<BusinessTripOverBudget>().GetSingleAsync(x => x.Id == ItemId);
                if (bobEntity != null && bobEntity.BusinessTripApplication != null && bobEntity.BusinessTripApplicationId != null && bobEntity.BusinessTripApplicationId.HasValue)
                {
                    var wfInstanceBookingFlight = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == bobEntity.BusinessTripApplicationId.Value, "created desc");
                    if (wfInstanceBookingFlight != null)
                    {
                        double? approvedDay = BTAHelper.GetBTAApprovedDay(wfInstanceBookingFlight.ItemId, _uow);
                        if (approvedDay != null && approvedDay.HasValue)
                        {
                            var parseIntValue = int.Parse(approvedDay.Value.ToString());
                            if (parseIntValue >= 10)
                            {
                                wfItem.LargerThanOrEqual10Day = true;
                            } else
                            {
                                wfItem.LessThan10Day = true;
                            }
                        }
                        /*
                         * string statusBookingFlight = ("Waiting for Booking Flight").Replace(" ", "").Trim().ToLower();
                        var wfTaskInprocessBookingFlight = await _uow.GetRepository<WorkflowTask>().GetSingleAsync(x => x.WorkflowInstanceId == wfInstanceBookingFlight.Id && x.ItemId == wfInstanceBookingFlight.ItemId
                        && !string.IsNullOrEmpty(x.Status) && x.Status.Replace(" ", "").Trim().ToLower().Equals(statusBookingFlight) && x.AssignedToId != null && x.AssignedToId.HasValue
                        && !x.IsCompleted, "created desc");
                        if (wfTaskInprocessBookingFlight != null)
                        {
                            var btaDetail = await _uow.GetRepository<BusinessTripApplicationDetail>(true).FindByAsync(x => x.BusinessTripApplicationId == bobEntity.BusinessTripApplicationId);
                            DateTimeOffset lastFromDateBTADetail = btaDetail.Where(y => y.FromDate != null && y.FromDate.HasValue).OrderBy(x => x.FromDate).Select(y => y.FromDate.Value).FirstOrDefault();
                            double day = (lastFromDateBTADetail - wfTaskInprocessBookingFlight.Created).TotalDays;
                            if (day >= 10)
                            {
                                wfItem.LargerThanOrEqual10Day = true;
                            }
                            else
                            {
                                wfItem.LessThan10Day = true;
                            }
                        }*/
                    }
                }
            }
            catch (Exception e) { }
        }
        public async Task<ResultDTO> GetListOverBudget(QueryArgs arg)
        {
            var allBusinessTrips = await _uow.GetRepository<BusinessTripOverBudget>().FindByAsync<BTAOverBudgetViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var count = await _uow.GetRepository<BusinessTripOverBudget>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = allBusinessTrips,
                    Count = count
                }
            };
        }
        public async Task<ResultDTO> SaveRequestOverBudget(RequestOverBudgetDTO data)
        {
            var result = new ResultDTO();
            if (data == null)
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            else if (data.Id.HasValue)
            {
                var overBudgetDetailTemp = JsonConvert.DeserializeObject<List<BusinessOverBudgetDetailDTO>>(data.OverBudgetInfos);
                if (overBudgetDetailTemp != null)
                {
                    var btaDetailIds = overBudgetDetailTemp.Select(x => x.BtaDetailId).ToList();
                    var existsOverBudget = await _uow.GetRepository<BusinessTripOverBudgetsDetail>(true).FindByAsync(x => btaDetailIds.Contains(x.BtaDetailId));
                    var ignoreList = new List<string>() { "Cancelled", "Rejected", "Completed" };
                    if (existsOverBudget != null && existsOverBudget.Any(x => !ignoreList.Contains(x.BusinessTripOverBudget.Status)))
                    {
                        var tripGroup = existsOverBudget.Select(x => x.SAPCode).Distinct().ToList();
                        result.ErrorCodes = new List<int> () { -1 };
                        result.ErrorCodeStr = new List<string> (tripGroup);
                        result.Messages = new List<string> { $"EXIST_OVER_BUDGET" };
                        return result;
                    }
                }

                var record = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(data.Id.Value);
                var overBudget = new BusinessTripOverBudget
                {
                    DeptLineId = record.DeptLineId,
                    DeptCode = record.DeptCode,
                    DeptName = record.DeptName,
                    DeptDivisionId = record.DeptDivisionId,
                    DeptDivisionCode = record.DeptDivisionCode,
                    DeptDivisionName = record.DeptDivisionName,
                    UserSAPCode = record.UserSAPCode,
                    UserFullName = record.UserFullName,
                    MaxGrade = record.MaxGrade,
                    IsStore = record.IsStore,
                    MaxDepartmentId = record.MaxDepartmentId,
                    RequestorNote = record.RequestorNote,
                    CarRental = record.CarRental,
                    DocumentDetails = record.DocumentDetails,
                    Type = record.Type,
                    DocumentChanges = record.DocumentChanges,
                    IsRoundTrip = record.IsRoundTrip,
                    StayHotel = record.StayHotel,
                    BusinessTripApplicationId = data.Id,
                    BTAReferenceNumber = record.ReferenceNumber,
                    Comment = data.Comment

                };
                overBudget = overBudget.TransformValues(overBudget, new List<string> { "Id", "BusinessTripApplication", "Department", "User" });
                _uow.GetRepository<BusinessTripOverBudget>().Add(overBudget);
                _uow.Commit();
                if (!string.IsNullOrEmpty(data.OverBudgetInfos))
                {
                    AddBusinessTripOverBudgetDetail(data.OverBudgetInfos, overBudget);
                }
                result.Object = Mapper.Map<BTAOverBudgetViewModel>(overBudget);
            }

            return result;
        }
        public async Task<ResultDTO> SaveBudget(BusinessOverBudgetDTO data)
        {
            var result = new ResultDTO();
            if (data == null)
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }

            var record = await _uow.GetRepository<BusinessTripOverBudget>().FindByIdAsync(data.Id.Value);
            if (record != null)
            {
                record.RequestorNote = data.RequestorNote;
                record.DocumentDetails = data.DocumentDetails;
                record.DocumentChanges = data.DocumentChanges;
                //record = record.TransformValues(record, new List<string> { "Id", "BusinessTripApplication", "Department", "User" });
                //record.BusinessTripApplication = null;
                if (!string.IsNullOrEmpty(data.BusinessTripOverBudgetDetails))
                {
                    AddBusinessTripOverBudgetDetail(data.BusinessTripOverBudgetDetails, record);
                }

            };
            _uow.Commit();
            result.Object = Mapper.Map<BTAOverBudgetViewModel>(record);


            return result;
        }
        private void AddBusinessTripOverBudgetDetail(string details, BusinessTripOverBudget overBudget)
        {
            var overBudgetDetailTemp = JsonConvert.DeserializeObject<List<BusinessOverBudgetDetailDTO>>(details);

            #region Remove businesstripDetailTemp
            List<Guid> overBudgetDetailIds_Client = overBudgetDetailTemp.Where(x => x != null && x.Id.HasValue && x.Id.Value != Guid.Empty).Select(x => x.Id.Value).ToList();
            List<Guid> overBudgetDetailIds_DB = _uow.GetRepository<BusinessTripOverBudgetsDetail>(true).FindBy(x => x.BusinessTripOverBudgetId == overBudget.Id).Select(x => x.Id).ToList();
            List<Guid> removedOverBudgetDetailIds = overBudgetDetailIds_DB.Except(overBudgetDetailIds_Client).ToList();
            if (removedOverBudgetDetailIds.Any())
            {
                _uow.GetRepository<BusinessTripOverBudgetsDetail>().Delete(_uow.GetRepository<BusinessTripOverBudgetsDetail>(true).FindBy(x => removedOverBudgetDetailIds.Contains(x.Id)));
            }
            #endregion

            #region Add new or update
            if (overBudgetDetailTemp != null && overBudgetDetailTemp.Any())
            {
                overBudget.BusinessTripFrom = overBudgetDetailTemp.OrderBy(x => x.FromDate).FirstOrDefault().FromDate;
                overBudget.BusinessTripTo = overBudgetDetailTemp.OrderByDescending(x => x.ToDate).FirstOrDefault().ToDate;
                foreach (var item in overBudgetDetailTemp)
                {
                    bool flag = false;
                    BusinessTripOverBudgetsDetail detail = null;
                    //detail = new BusinessTripOverBudgetsDetail();
                    if (item.Id.HasValue)
                    {
                        detail = _uow.GetRepository<BusinessTripOverBudgetsDetail>().FindById(item.Id.Value);
                        if (detail == null)
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        detail = new BusinessTripOverBudgetsDetail();
                    }

                    detail = item.TransformValues(detail, new List<string> { "Id", "BusinessTripOverBudget", "Department", "User" });
                    detail.BusinessTripOverBudgetId = overBudget.Id;
                    detail.UserGradeValue = item.UserGradeValue;

                    // Luu thong tin chuyen bay
                    detail.DepartureSearchId = item.DepartureSearchId;
                    detail.ReturnSearchId = item.ReturnSearchId;

                    detail.TitleDepartureFareRule = item.TitleDepartureFareRule;
                    detail.DetailDepartureFareRule = item.DetailDepartureFareRule;
                    detail.TitleReturnFareRule = item.TitleReturnFareRule;
                    detail.DetailReturnFareRule = item.DetailReturnFareRule;

                    if (detail.BtaDetailId.HasValue)
                    {
                        detail.BtaDetailId = item.BtaDetailId;
                        var btaDetail = _uow.GetRepository<BusinessTripApplicationDetail>(true).FindById(detail.BtaDetailId.Value);
                        if (btaDetail != null)
                            detail.UserGradeId = btaDetail.UserGradeId;
                    }
                    if (item.FlightDetails != null)
                    {
                        detail.FlightDetails = JsonConvert.SerializeObject(item.FlightDetails);
                    }
                    if (flag)
                    {
                        _uow.GetRepository<BusinessTripOverBudgetsDetail>().Add(detail);
                    }
                    else
                    {
                        _uow.GetRepository<BusinessTripOverBudgetsDetail>().Update(detail);
                    }
                }
            };
            #endregion
            _uow.Commit();
        }
        public ResultDTO GetTripOverBudgetGroups(Guid Id)
        {
            List<BTAOverBudgetGroupViewModel> returnValue = new List<BTAOverBudgetGroupViewModel>();
            try
            {
                BTAOverBudgetViewModel btaItem = Id.GetAsBtaOverBudgetViewModel(_uow);
                if (btaItem != null)
                {
                    List<BusinessTripOverBudgetDetailViewModel> details = btaItem.GetAsBtaOverBudgetDetailsViewModel(_uow);
                    #region Funcs
                    Func<IEnumerable<BusinessTripOverBudgetDetailViewModel>, int, List<BTAOverBudgetGroupViewModel>> SetGroupNumber = (IEnumerable<BusinessTripOverBudgetDetailViewModel> itemList, int groupNumber) =>
                    {
                        List<BTAOverBudgetGroupViewModel> returnGroupValue = new List<BTAOverBudgetGroupViewModel>();
                        foreach (BusinessTripOverBudgetDetailViewModel item in itemList)
                        {
                            item.TripGroup = groupNumber;
                            item.MaxBudgetAmount = item.GetMaxBudgetOverLimit(_uow);
                            item.GroupMemberCount = itemList.Count();
                            item.FlightDetails = item.GetFlightOverBudgetDetails(_uow);
                            returnGroupValue.Add(item.ConvertTo<BTAOverBudgetGroupViewModel>());
                        }
                        return returnGroupValue;
                    };

                    Func<int> GetAvailableGroupNumber = () =>
                    {
                        List<int> usedGroupNumber = new List<int>();
                        usedGroupNumber.AddRange(returnValue.Where(x => x != null && x.TripGroup > 0).Select(x => x.TripGroup).Distinct().ToList());
                        usedGroupNumber.AddRange(details.Where(x => x != null && x.TripGroup > 0).Select(x => x.TripGroup).Distinct().ToList());
                        usedGroupNumber = usedGroupNumber.Distinct().OrderBy(i => i).ToList();
                        List<int> rangOfGroupNumber = Enumerable.Range(1, details.Count()).ToList();
                        List<int> availableGroupNumber = rangOfGroupNumber.Except(usedGroupNumber).ToList();
                        return availableGroupNumber.FirstOrDefault();
                    };
                    #endregion
                    var groups = details.GroupBy(x => new BTATripOverBudgetGroupInfo(x, _uow).key).ToList();
                    if (groups.Any())
                    {
                        //assign group already had tripgroup
                        List<IGrouping<string, BusinessTripOverBudgetDetailViewModel>> groupHasTripGroup = groups.Where(x => x != null && x.Any() && x.ToList().FirstOrDefault().TripGroup != 0).ToList();
                        foreach (var itemList in groupHasTripGroup)
                        {
                            returnValue.AddRange(SetGroupNumber(itemList, itemList.ToList()[0].TripGroup));
                        }

                        //assign group still not have tripgroup
                        List<IGrouping<string, BusinessTripOverBudgetDetailViewModel>> groupNotHaveTripGroup = groups.Except(groupHasTripGroup).ToList();
                        foreach (var itemList in groupNotHaveTripGroup)
                        {
                            returnValue.AddRange(SetGroupNumber(itemList, GetAvailableGroupNumber()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError(logger, "GetTripOverBudgetGroups");
            }
            return new ResultDTO { Object = returnValue };
        }
    }

}
