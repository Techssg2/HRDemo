using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using static Aeon.HR.ViewModels.Args.CommonArgs.User;
using DateValueArgs = Aeon.HR.ViewModels.Args.CommonArgs.User.DateValueArgs;
using QueryArgs = Aeon.HR.Infrastructure.QueryArgs;

namespace Aeon.HR.BusinessObjects.Handlers.Other
{
    public class APIEdoc2BO : IAPIEdoc2BO
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger logger;
        private Guid? _refDeparmentId = null;
        protected readonly IBusinessTripBO bo;
        public APIEdoc2BO(IUnitOfWork uow, ILogger _logger, IBusinessTripBO _bo)
        {
            _uow = uow;
            logger = _logger;
            bo = _bo;
        }

        public async Task<ResultDTO> GetItemByReferenceNumber(CommonDTO agrs)
        {
            var result = new ResultDTO();
            var currentItem = await _uow.GetRepository<BusinessTripApplication>(true).GetSingleAsync<BTAViewModel>(x => x.ReferenceNumber == agrs.ReferenceNumber);
            if (currentItem != null)
            {
                var record = Mapper.Map<BusinessTripItemViewModel_API>(currentItem);

                string url = Convert.ToString(ConfigurationManager.AppSettings["siteUrl"]) + "/_layouts/15/AeonHRTUSSG/Default.aspx#!/home/business-trip-application/item/" + record.ReferenceNumber +
                    "?id=" + currentItem.Id;

                record.UrlBTA = url;
                var details = _uow.GetRepository<BusinessTripApplicationDetail>(true).FindBy<BusinessTripDetail_API_DTO>(x => x.BusinessTripApplicationId == currentItem.Id, string.Empty).ToList();
                if (details != null && details.Any())
                {
                    record.BusinessTripDetails = details.OrderBy(x => x.FromDate).ThenBy(x => x.ToDate);

                }
                var changeCancelDetails = await _uow.GetRepository<ChangeCancelBusinessTripDetail>(true).FindByAsync<ChangeCancelBusinessTripDTO>(x => x.BusinessTripApplicationId == currentItem.Id, string.Empty);
                if (changeCancelDetails != null && changeCancelDetails.Any())
                {
                    foreach (var item in changeCancelDetails)
                    {
                        foreach (var detail in details)
                        {
                            if (detail.SAPCode.Contains(item.SAPCode))
                            {
                                detail.FromDate = item.NewCheckInHotelDate;
                                detail.ToDate = item.NewCheckOutHotelDate;
                                detail.ArrivalName = item.DestinationName;
                                break;
                            }
                        }
                    }
                    record.BusinessTripDetails = details.OrderBy(x => x.FromDate).ThenBy(x => x.ToDate);
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

        public async Task<ResultDTO> GetAllBTA_API(string ReferenceNumber, string Limit)
        {
            var listItems = new List<BusinessTripItemViewModel_API>();
            var allBusinessTrips = await _uow.GetRepository<BusinessTripApplication>(true).FindByAsync<BTAViewModel>(x => (x.Status == "Completed" || x.Status == "Completed Changing")
                                        && !x.IsCheckRe && (ReferenceNumber != null ? x.ReferenceNumber.Contains(ReferenceNumber) : true));
            if (Limit != null)
            {
                try
                {
                    if (Int32.Parse(Limit) > -1)
                    {
                        allBusinessTrips = allBusinessTrips.Take(Int32.Parse(Limit));
                    }

                }
                catch (Exception ex)
                {

                }
            }

            foreach (var businessTrip in allBusinessTrips)
            {
                var record = Mapper.Map<BusinessTripItemViewModel_API>(businessTrip);

                string url = Convert.ToString(ConfigurationManager.AppSettings["siteUrl"]) + "/_layouts/15/AeonHR/Default.aspx#!/home/business-trip-application/item/" + record.ReferenceNumber +
                    "?id=" + businessTrip.Id;

                var changingIds = new List<Guid>();
                var hasChanging = _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindBy(x => x.IsCancel && x.BusinessTripApplicationId == businessTrip.Id);
                if (hasChanging != null && hasChanging.Any())
                {
                    changingIds = hasChanging.Select(x => x.BusinessTripApplicationDetailId).ToList();
                }

                record.UrlBTA = url;
                if (record.Type != BTAType.Domestic)
                {
                    var details = _uow.GetRepository<BusinessTripApplicationDetail>(true).FindBy(x => !changingIds.Contains(x.Id) && x.BusinessTripApplicationId == businessTrip.Id, string.Empty).ToList();
                    if (details != null && details.Any())
                    {
                        foreach (var item in details)
                        {
                            item.FromDate = item.FromDate.Value.LocalDateTime;
                            item.ToDate = item.ToDate.Value.LocalDateTime;
                            this.UpdateFromDateToDateByChangingCancelBTA(item);
                        }
                        /*var listdetails = new List<BusinessTripDetail_API_DTO>();
                        foreach (var item in details)
                        {
                            var detail = Mapper.Map<BusinessTripDetail_API_DTO>(item);
                            var flightDetail = (await _uow.GetRepository<FlightDetail>(true).FindByAsync(x => x.BusinessTripApplicationDetailId == item.Id, string.Empty)).ToList();
                            // noi dia khu hoi
                            if (record.Type == BTAType.DomesticWithFlight)
                            {
                                if (flightDetail.Any() && flightDetail.Count() > 0)
                                {
                                    foreach (var dtl in flightDetail)
                                    {
                                        // chieu di
                                        if (dtl.DirectFlight)
                                        {
                                            PricedItinerariesViewModel origin = Mapper.Map<PricedItinerariesViewModel>(JsonConvert.DeserializeObject<PricedItinerariesViewModel>(dtl.PricedItinerariesStr));
                                            if (origin != null && origin.OriginDestinationOptions != null && origin.OriginDestinationOptions.Count() > 0)
                                            {
                                                detail.FromDate = origin.OriginDestinationOptions[0].OriginDateTime;
                                            }
                                        }
                                        else
                                        {
                                            // chieu ve
                                            PricedItinerariesViewModel destination = Mapper.Map<PricedItinerariesViewModel>(JsonConvert.DeserializeObject<PricedItinerariesViewModel>(dtl.PricedItinerariesStr));
                                            if (destination != null && destination.OriginDestinationOptions != null && destination.OriginDestinationOptions.Count() > 0)
                                            {
                                                detail.ToDate = destination.OriginDestinationOptions[0].DestinationDateTime;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Quoc te khu hoi
                                if (flightDetail != null && flightDetail.Count() > 0 && flightDetail[0] != null && !string.IsNullOrEmpty(flightDetail[0].PricedItinerariesStr))
                                {
                                    PricedItinerariesViewModel originDestination = Mapper.Map<PricedItinerariesViewModel>(JsonConvert.DeserializeObject<PricedItinerariesViewModel>(flightDetail[0].PricedItinerariesStr));
                                    if (originDestination != null)
                                    {
                                        if (originDestination.OriginDestinationOptions != null && originDestination.OriginDestinationOptions.Count() > 0)
                                        {
                                            // From date
                                            OriginDestinationOptionsViewModel origin = (originDestination.OriginDestinationOptions.Count() == 1 && originDestination.OriginDestinationOptions[0] != null) ? originDestination.OriginDestinationOptions[0] : null;
                                            if (origin != null) detail.FromDate = origin.OriginDateTime;
                                            // To date
                                            OriginDestinationOptionsViewModel destination = (originDestination.OriginDestinationOptions.Count() == 2 && originDestination.OriginDestinationOptions[1] != null) ? originDestination.OriginDestinationOptions[1] : null;
                                            if (destination != null) detail.ToDate = destination.DestinationDateTime;
                                        }
                                    }
                                }
                            }
                            listdetails.Add(detail);
                        }*/
                        record.BusinessTripDetails = (Mapper.Map<List<BusinessTripDetail_API_DTO>>(details)).OrderBy(x => x.FromDate).ThenBy(x => x.ToDate);
                    }
                }
                else
                {
                    var details = _uow.GetRepository<BusinessTripApplicationDetail>(true).FindBy(x => !changingIds.Contains(x.Id) && x.BusinessTripApplicationId == businessTrip.Id, string.Empty).ToList();
                    foreach (var item in details)
                    {
                        item.FromDate = item.FromDate.Value.LocalDateTime;
                        item.ToDate = item.ToDate.Value.LocalDateTime;
                        this.UpdateFromDateToDateByChangingCancelBTA(item);
                    }
                    record.BusinessTripDetails = (Mapper.Map<List<BusinessTripDetail_API_DTO>>(details)).OrderBy(x => x.FromDate).ThenBy(x => x.ToDate);

                }
                if (record.BusinessTripDetails != null && record.BusinessTripDetails.Any())
                {
                    listItems.Add(record);
                }
            }
            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = listItems,
                    Count = listItems.Count()
                }
            };
        }

        private void UpdateFromDateToDateByChangingCancelBTA(BusinessTripApplicationDetail btaDetail)
        {
            if (btaDetail != null)
            {
                var changingCancelBTA = _uow.GetRepository<ChangeCancelBusinessTripDetail>(true).GetSingle(x => x.BusinessTripApplicationDetailId == btaDetail.Id && x.BusinessTripApplicationId == btaDetail.BusinessTripApplicationId, "created desc");
                if (changingCancelBTA != null)
                {
                    btaDetail.FromDate = changingCancelBTA.NewFromDate != null && changingCancelBTA.NewFromDate.HasValue ? changingCancelBTA.NewFromDate : changingCancelBTA.FromDate;
                    btaDetail.ToDate = changingCancelBTA.NewToDate != null && changingCancelBTA.NewToDate.HasValue ? changingCancelBTA.NewToDate : changingCancelBTA.FromDate;
                }
            }
        }

        public async Task<ResultDTO> UpdateStatusBTA(CommonDTO agrs)
        {
            bo.SaveBTALog(agrs.Id, "API: UpdateStatusBTA Payload: " + JsonConvert.SerializeObject(agrs));
            var result = new ResultDTO();
            var currentItem = await _uow.GetRepository<BusinessTripApplication>(true).GetSingleAsync(x => x.ReferenceNumber == agrs.ReferenceNumber);
            if (currentItem != null)
            {
                if (agrs.Status == 1)
                {
                    if (agrs.Url == null)
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "No URL RE" };
                        return result;
                    }
                    else
                    {
                        if (agrs.ReferenceNumberRE == null)
                        {
                            result.ErrorCodes = new List<int> { 1004 };
                            result.Messages = new List<string> { "No Reference Number RE" };
                            return result;
                        }
                        currentItem.IsCheckRe = true;
                        currentItem.Url_RE = agrs.Url;
                        currentItem.ReferenceNumberRE = agrs.ReferenceNumberRE;
                        currentItem.ModifiedRE = DateTimeOffset.Now;
                    }
                }
                else
                {
                    currentItem.IsCheckRe = false;
                    currentItem.Url_RE = null;
                    currentItem.ReferenceNumberRE = null;
                }

                _uow.Commit();
                result.Messages = new List<string> { "Success" };
                bo.SaveBTALog(currentItem.Id, "API: UpdateStatusBTA Response: " + JsonConvert.SerializeObject(result));
            }
            else
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            return result;
        }
        public async Task<ResultDTO> GetDepartmentTree_API()
        {
            var lstDepartment = await _uow.GetRepository<Department>(true).FindByAsync<DepartmentTreeAPIViewModel>(x => true);
            List<DepartmentTreeAPIViewModel> departmentTree = new List<DepartmentTreeAPIViewModel>();
            List<DepartmentTreeAPIViewModel> vmLstDepartment = lstDepartment.OrderByDescending(x => x.JobGradeGrade).ToList();
            var highestDepartment = lstDepartment.Where(x => x.ParentId == x.Id).FirstOrDefault();
            if (highestDepartment != null)
            {
                vmLstDepartment.ForEach(x =>
                {
                    x.Items = vmLstDepartment.Where(y => y.ParentId == x.Id && y.Id != x.Id).OrderByDescending(k => k.Name).ToList();
                }
                );

                departmentTree = vmLstDepartment.Where(item => item.Id == highestDepartment.Id).ToList();
                departmentTree.AddRange(vmLstDepartment.Where(item => !item.ParentId.HasValue).ToList());
                ResultDTO result = new ResultDTO
                {
                    Object = new ArrayResultDTO
                    {
                        Data = departmentTree,
                        Count = 1,
                    },
                };
                return result;
            }
            else
            {
                vmLstDepartment.ForEach(x =>
                x.Items = vmLstDepartment.Where(y => y.ParentId == x.Id).OrderByDescending(k => k.Name).ToList()

                );
                departmentTree = vmLstDepartment.Where(item => !item.ParentId.HasValue).ToList();
                if (!departmentTree.Any())
                {
                    departmentTree = vmLstDepartment;
                }
                ResultDTO result = new ResultDTO
                {
                    Object = new ArrayResultDTO
                    {
                        Data = departmentTree,
                        Count = 1,
                    },
                };
                return result;
            }
        }
        public async Task<ArrayResultDTO> GetAllUser_API()
        {
            var items = await _uow.GetRepository<User>(true).GetAllAsync<UserListAPIViewModel>();
            return new ArrayResultDTO { Data = items, Count = items.Count() };
        }
        public async Task<ArrayResultDTO> GetAllUser_APIV2(IntergationAPI args)
        {
            ArrayResultDTO rs = new ArrayResultDTO { };
            QueryArgs query = new QueryArgs();

            List<object> predicateParameters = new List<object>();

            try
            {
                if (!string.IsNullOrEmpty(args.KeyWord))
                {
                    query.Predicate = "(LoginName.contains(@0) or SAPCode.contains(@0) or Email.contains(@0) or FullName.contains(@0))";
                    predicateParameters.Add(args.KeyWord);
                }

                if (args.IsActivated.HasValue)
                {
                    if (!string.IsNullOrEmpty(query.Predicate))
                    {
                        query.Predicate += " and ";
                    }
                    query.Predicate += "IsActivated = (@" + predicateParameters.Count + ")";
                    predicateParameters.Add(args.IsActivated.Value);
                }

                // 1 MS, 0 AD
                if (!string.IsNullOrEmpty(args.Type))
                {
                    bool IsTypeInvalid = new List<string> { "ad", "ms" }.Contains(args.Type.ToLower());
                    if (!IsTypeInvalid)
                    {
                        rs.Data = "Type Invalid!";
                        goto Finish;
                    }
                    if (!string.IsNullOrEmpty(query.Predicate))
                    {
                        query.Predicate += " and ";
                    }
                    query.Predicate += "Type = (@" + predicateParameters.Count + ")";
                    predicateParameters.Add(args.Type.ToLower().Equals("ad") ? 0 : 1);
                }

                // 2022-12-26
                if (args.CreatedFromDate.HasValue)
                {
                    if (!string.IsNullOrEmpty(query.Predicate))
                    {
                        query.Predicate += " and ";
                    }
                    query.Predicate += "Created >= (@" + predicateParameters.Count + ")";
                    predicateParameters.Add(args.CreatedFromDate.Value);
                }

                if (args.CreatedToDate.HasValue)
                {
                    if (!string.IsNullOrEmpty(query.Predicate))
                    {
                        query.Predicate += " and ";
                    }
                    query.Predicate += "Created <= (@" + predicateParameters.Count + ")";
                    predicateParameters.Add(args.CreatedToDate.Value);
                }

                if (args.ModifiedFromDate.HasValue)
                {
                    if (!string.IsNullOrEmpty(query.Predicate))
                    {
                        query.Predicate += " and ";
                    }
                    query.Predicate += "Modified >= (@" + predicateParameters.Count + ")";
                    predicateParameters.Add(args.ModifiedFromDate.Value);
                }

                if (args.ModifiedToDate.HasValue)
                {
                    if (!string.IsNullOrEmpty(query.Predicate))
                    {
                        query.Predicate += " and ";
                    }
                    query.Predicate += "Modified <= (@" + predicateParameters.Count + ")";
                    predicateParameters.Add(args.ModifiedToDate.Value);
                }

                query.PredicateParameters = predicateParameters.ToArray();
                var items = await _uow.GetRepository<User>(true).FindByAsync<UserListAPIViewModel>(query.Predicate, query.PredicateParameters);

                rs.Data = items;
                rs.Count = items.Count();
            }
            catch (Exception e)
            {
                rs.Data = "Error: " + e.Message.ToString();
            }

        Finish:
            return rs;
        }

        public async Task<ResultDTO> GetSpecificShiftPlan_API(ShiftPlanAPIArgs args)
        {
            var result = new ResultDTO { };
            var data = new ArrayResultDTO();

            try
            {
                #region Period 
                if (args.Day == 0)
                {
                    args.Day = DateTime.Now.Day;
                }
                if (args.Month == 0)
                {
                    args.Month = DateTime.Now.Month;
                }
                if (args.Year == 0)
                {
                    args.Year = DateTime.Now.Year;
                }

                var _fromDate = new DateTime();
                var _toDate = new DateTime();

                /*if (args.Month == 1)
                {
                    _fromDate = new DateTime(args.Year - 1, 12, 26);
                    _toDate = new DateTime(args.Year, args.Month, 25);
                }
                else
                {
                    _fromDate = new DateTime(args.Year, args.Month - 1, 26);
                    _toDate = new DateTime(args.Year, args.Month, 25);
                }*/

                if (args.TimeType.ToLower().Equals("day"))
                {
                    _fromDate = new DateTime(args.Year, args.Month, args.Day);
                    _toDate = new DateTime(args.Year, args.Month, args.Day + 1);
                }
                else if (args.TimeType.ToLower().Equals("week"))
                {
                    _fromDate = new DateTime(args.Year, args.Month, args.Day);
                    _toDate = new DateTime(args.Year, args.Month, args.Day + 7);
                }
                else
                {
                    //_fromDate va _toDate la ngay dau va cuoi cua thang
                    _fromDate = new DateTime(args.Year, args.Month, 1);
                    _toDate = _fromDate.AddMonths(1);
                }

                var _period1 = args.Month + "/" + args.Year;
                var _period2 = (args.Month + 1) + "/" + args.Year;
                if (args.Month == 12)
                {
                    _period2 = 1 + "/" + (args.Year + 1);
                }

                //mang gom period1 va period2
                var _period = new string[] { _period1, _period2 };
                #endregion

                List<object> listResult = new List<object>();
                List<object> listResultBTA = new List<object>();
                List<DepartmentViewModel> department = new List<DepartmentViewModel>();
                var targetPlanPeriod = await _uow.GetRepository<TargetPlanPeriod>(true).FindByAsync(x => _period.Contains(x.Name));
                if (targetPlanPeriod != null)
                {
                    if (string.IsNullOrEmpty(args.LoginName))
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "LoginName must have value!" };
                        return result;
                    }

                    var getUserCurrent = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.LoginName == args.LoginName && x.IsActivated);
                    if (getUserCurrent.Count() == 0)
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "This user does not exist in the system" };
                        return result;
                    }

                    var userId = getUserCurrent.FirstOrDefault().Id;

                    var useInDeparment = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync<UserDepartmentMappingViewModel>(x => x.UserId == userId && x.Role == Group.Member);
                    if (useInDeparment.Count() == 0)
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "This user does not exist in any department" };
                        return result;
                    }

                    var departmentId = useInDeparment.Select(x => x.DepartmentId).ToList();

                    //lay departmentItems co id thuoc departmentId
                    var departmentItems = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => departmentId.Contains(x.Id) && !x.IsDeleted);
                    if (departmentItems.Count() > 0)
                    {
                        foreach (var departmentItem in departmentItems)
                        {
                            await LoadDepartmentHierarchyAsync(departmentItem.Id, department);
                        }
                        departmentItems = departmentItems.Concat(department);

                        departmentItems = departmentItems.Distinct();
                    }

                    var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft" };

                    if (args.ShiftPlan.Contains("BTA"))
                    {
                        var departmentCodes = departmentItems.Select(x => x.Code).ToList();

                        var BTAFromDate = new DateTimeOffset(_fromDate);
                        var BTAToDate = new DateTimeOffset(_toDate);


                        var listChangeCancelDetails = await _uow.GetRepository<ChangeCancelBusinessTripDetail>(true).FindByAsync(x => ((BTAFromDate <= x.NewFromDate && x.NewFromDate < BTAToDate)
                                    ||
                                    (BTAFromDate <= x.NewToDate && x.NewToDate < BTAToDate)) && departmentCodes.Contains(x.DepartmentCode) && !x.IsCancel
                                    && x.NewFromDate != null && x.NewToDate != null && x.BusinessTripApplicationDetail.BusinessTripApplication.Status.Equals("Completed Changing") && !statusToCheckes.Contains(x.BusinessTripApplicationDetail.BusinessTripApplication.Status));
                        List<Guid> listBTADetailIds = new List<Guid>();
                        if (listChangeCancelDetails.Any())
                        {
                            listBTADetailIds = listChangeCancelDetails.Select(x => (Guid)x.BusinessTripApplicationDetailId).ToList();
                        }

                        var listBTADetails = await _uow.GetRepository<BusinessTripApplicationDetail>(true).FindByAsync(x => ((BTAFromDate <= x.FromDate && x.FromDate < BTAToDate)
                                    ||
                                    (BTAFromDate <= x.ToDate && x.ToDate < BTAToDate)) &&
                                     departmentCodes.Contains(x.DepartmentCode) && x.BusinessTripApplication.Status.Equals("Completed") && !statusToCheckes.Contains(x.BusinessTripApplication.Status));

                        if (listBTADetails.Count() > 0)
                        {
                            listBTADetails = listBTADetails.Where(x => !listBTADetailIds.Contains(x.Id)).ToList();
                        }

                        List<Guid> listRevokeBTAIds = new List<Guid>();
                        foreach (var item in listBTADetails)
                        {
                            var currentWf = await _uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.ItemId == item.BusinessTripApplicationId, "Created desc");
                            if (currentWf != null && currentWf.WorkflowName.ToLower().Contains("bta revoke") && currentWf.IsCompleted)
                            {
                                listRevokeBTAIds.Add(item.Id);
                            }
                        }

                        if (listRevokeBTAIds.Count() > 0)
                        {
                            listBTADetails = listBTADetails.Where(x => !listRevokeBTAIds.Contains(x.Id)).ToList();
                        }

                        List<BTAShifPlanValue> BTAShifPlanValues = new List<BTAShifPlanValue>();
                        foreach (var item in listChangeCancelDetails)
                        {
                            BTAShifPlanValues.Add(new BTAShifPlanValue
                            {
                                SAPCode = item.SAPCode,
                                FromDate = item.NewFromDate,
                                ToDate = item.NewToDate,
                                DepartureName = item.BusinessTripApplicationDetail.DepartureName,
                                ArrivalName = item.BusinessTripApplicationDetail.ArrivalName,
                                UserName = item.BusinessTripApplicationDetail.FullName
                            });
                        }

                        foreach (var item in listBTADetails)
                        {
                            BTAShifPlanValues.Add(new BTAShifPlanValue
                            {
                                SAPCode = item.SAPCode,
                                FromDate = item.FromDate,
                                ToDate = item.ToDate,
                                DepartureName = item.DepartureName,
                                ArrivalName = item.ArrivalName,
                                UserName = item.FullName
                            });
                        }

                        var groupBTAs = BTAShifPlanValues.GroupBy(x => x.SAPCode).ToList();
                        foreach (var group in groupBTAs)
                        {
                            List<ActualBTAValue> jsonActual1 = new List<ActualBTAValue>();
                            dynamic _data = new ExpandoObject();
                            foreach (var item in group)
                            {
                                if (!jsonActual1.Any(x => x.FromDate == item.FromDate && x.ToDate == item.ToDate && x.DepartureName == item.DepartureName && x.ArrivalName == item.ArrivalName))
                                {
                                    jsonActual1.Add(new ActualBTAValue
                                    {
                                        FromDate = item.FromDate,
                                        ToDate = item.ToDate,
                                        DepartureName = item.DepartureName,
                                        ArrivalName = item.ArrivalName,
                                        UserName = item.UserName
                                    });
                                }
                            }
                            _data.Actual1 = JsonConvert.SerializeObject(jsonActual1);
                            var expandoDict = _data as IDictionary<string, object>;
                            try
                            {
                                listResultBTA.Add(new
                                {
                                    SAPCode = group.Key,
                                    Actual1 = GetValue(expandoDict, "Actual1")
                                });

                            }
                            catch (Exception ex)
                            {
                                result.ErrorCodes = new List<int> { 1004 };
                                result.Messages = new List<string> { ex.Message };
                            }
                        }

                    }

                    var shiftPlanArr = args.ShiftPlan.Where(x => !x.Contains("BTA")).ToList();

                    if (shiftPlanArr.Count > 0 && targetPlanPeriod.Count() > 0)
                    {
                        var targetPlanPeriodID = targetPlanPeriod.Select(x => x.Id).ToList();

                        var departmentCodes = departmentItems.Select(x => x.Code).ToList();
                        var departmentIds = departmentItems.Select(x => x.Id).ToList();
                        var targetplanDetails = await _uow.GetRepository<TargetPlanDetail>(true).FindByAsync<TargetPlanDetailViewModel>(x => targetPlanPeriodID.Contains(x.TargetPlan.PeriodId) && departmentCodes.Contains(x.DepartmentCode) && x.TargetPlan.Status == "Completed" && x.Type == TypeTargetPlan.Target1);
                        List<TargetPlanDetailViewModel> tempTargetPlanDetails = new List<TargetPlanDetailViewModel>();
                        /*if (departmentItems.Count() > 0)
                        {
                            tempTargetPlanDetails = new List<TargetPlanDetailViewModel>();
                            foreach (var d in departmentItems)
                                foreach (var t in targetplanDetails)
                                    if (d.Code == t.DepartmentCode)
                                        tempTargetPlanDetails.Add(t);
                            targetplanDetails = tempTargetPlanDetails;
                        }*/
                        //tempTargetPlanDetails = new List<TargetPlanDetailViewModel>();
                        var trackingRequestForGetList = new List<TrackingRequestForGetListViewModel>();
                        var tempTrackingLogInitDatas = new List<TrackingLogInitDatasViewModel>();
                        var tempTargetDetailsId = targetplanDetails.Select(x => x.TargetPlanId).ToList();
                        var targetPlan = await _uow.GetRepository<TargetPlan>().FindByAsync<TargetPlanViewModel>(x => tempTargetDetailsId.Contains(x.Id));

                        List<string> targetPlanReferences = new List<string>();
                        if (targetPlan.Count() > 0)
                        {
                            targetPlanReferences = targetPlan.Select(x => x.ReferenceNumber).Distinct().ToList();
                        }

                        var trackingLog = await _uow.GetRepository<TrackingLogInitData>().FindByAsync<TrackingLogInitDatasViewModel>(x => targetPlanReferences.Contains(x.ReferenceNumber));
                        tempTrackingLogInitDatas.AddRange((List<TrackingLogInitDatasViewModel>)trackingLog);

                        /*foreach (var t in targetPlan)
                        {
                            var trackingLog = await _uow.GetRepository<TrackingLogInitData>().FindByAsync<TrackingLogInitDatasViewModel>(x => x.ReferenceNumber == t.ReferenceNumber);
                            tempTrackingLogInitDatas.AddRange((List<TrackingLogInitDatasViewModel>)trackingLog);
                        }*/

                        var trackingLogInitDatas = tempTrackingLogInitDatas != null ? tempTrackingLogInitDatas : new List<TrackingLogInitDatasViewModel>();
                        var idTrackingLog = trackingLogInitDatas.Select(x => x.TrackingLogId).ToList();
                        var trackingRequest = await _uow.GetRepository<TrackingRequest>().FindByAsync<TrackingRequestForGetListViewModel>(x => idTrackingLog.Contains(x.Id) && (x.Status.Trim().ToLower().Equals("success") || x.Status.Trim().ToLower().Equals("save data success")));
                        foreach (var temp in trackingRequest)
                        {
                            foreach (var target in targetplanDetails)
                            {
                                var jsonDetails = JsonConvert.DeserializeObject<TrackingPayLoad>(temp.Payload);
                                if (jsonDetails.RefNum == target.ReferenceNumber)
                                {
                                    tempTargetPlanDetails.Add(target);
                                }
                            }
                        }
                        targetplanDetails = tempTargetPlanDetails.Distinct().ToList();

                        var listLeaveDetails = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => x.LeaveApplication.UserSAPCode != null && x.LeaveApplication.Status == "Completed" &&
                                    ((_fromDate <= x.FromDate && x.FromDate < _toDate)
                                    ||
                                    (_fromDate <= x.ToDate && x.ToDate < _toDate)) && shiftPlanArr.Contains(x.LeaveCode) && (departmentCodes.Contains(x.LeaveApplication.DeptCode) || departmentCodes.Contains(x.LeaveApplication.DivisionCode)), "", y => y.LeaveApplication);
                        var listShiftDetails = await _uow.GetRepository<ShiftExchangeApplicationDetail>().FindByAsync(x =>
                                    (_fromDate <= x.ShiftExchangeDate && x.ShiftExchangeDate < _toDate) && (shiftPlanArr.Contains(x.NewShiftCode) || shiftPlanArr.Contains(x.CurrentShiftCode)) && (departmentIds.Contains((Guid)x.ShiftExchangeApplication.DeptLineId) || departmentIds.Contains((Guid)x.ShiftExchangeApplication.DeptDivisionId)) && x.ShiftExchangeApplication.Status == "Completed");

                        var tempIds = listLeaveDetails.Select(x => x.LeaveApplicationId).ToList();

                        var listLeaves = await _uow.GetRepository<LeaveApplication>(true).FindByAsync(x => tempIds.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                        var listLeavesId = listLeaves.Select(x => x.Id).ToList();
                        var tempIdShifts = listShiftDetails.Select(x => x.ShiftExchangeApplicationId).ToList();
                        var listShifts = await _uow.GetRepository<ShiftExchangeApplication>(true).FindByAsync(x => tempIdShifts.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                        var listShiftsId = listShifts.Select(x => x.Id).ToList();
                        var groupTargetPlans = targetplanDetails.GroupBy(x => x.SAPCode).ToList();

                        foreach (var group in groupTargetPlans)
                        {
                            List<ActualShiftPlanValue> jsonActual1 = new List<ActualShiftPlanValue>();
                            List<ActualShiftPlanValue> jsonActual2 = new List<ActualShiftPlanValue>();
                            dynamic _data = new ExpandoObject();
                            foreach (var target in group)
                            {

                                if (target.Type == TypeTargetPlan.Target1)
                                {
                                    var dictList = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);

                                    dictList = dictList.Where(x => _fromDate <= DateTime.ParseExact(x.Date, "yyyyMMdd", CultureInfo.InvariantCulture) && DateTime.ParseExact(x.Date, "yyyyMMdd", CultureInfo.InvariantCulture) < _toDate).ToList();
                                    foreach (var dict in dictList)
                                    {
                                        var lUser = getUserCurrent.FirstOrDefault(x => x.SAPCode == target.SAPCode);
                                        var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == target.SAPCode && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.Date));
                                        var lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && listShiftsId.Contains((Guid)x.ShiftExchangeApplicationId) && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                        int actualValue = 0;
                                        var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, target.Type, out actualValue);
                                        if (res.Value == null || res.Value == "") res = dict;


                                        if (actualValue == 0 && shiftPlanArr.Contains(res.Value))
                                        {
                                            //k add vao jsonActual1 neu jsonActual co phan tu co Date va Value trung voi res
                                            if (!jsonActual1.Any(x => x.Date == res.Date && x.Value == res.Value))
                                            {
                                                jsonActual1.Add(new ActualShiftPlanValue
                                                {
                                                    Date = res.Date,
                                                    Value = res.Value,
                                                    UserName = target.FullName
                                                });
                                            }
                                        }
                                    }
                                    _data.Actual1 = JsonConvert.SerializeObject(jsonActual1);
                                }
                                /*else
                                {
                                    var dictList = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);
                                    dictList = dictList.Where(x => _fromDate <= DateTime.ParseExact(x.Date, "yyyyMMdd", CultureInfo.InvariantCulture) && DateTime.ParseExact(x.Date, "yyyyMMdd", CultureInfo.InvariantCulture) < _toDate && shiftPlanArr.Contains(x.Value)).ToList();
                                    foreach (var dict in dictList)
                                    {
                                        var lUser = getUserCurrent.FirstOrDefault(x => x.SAPCode == target.SAPCode);
                                        var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == target.SAPCode && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.Date));
                                        var lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                        int actualValue = 0;
                                        var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, target.Type, out actualValue);
                                        if (res.Value == null || res.Value == "") res = dict;
                                        if (actualValue == 1 && shiftPlanArr.Contains(res.Value))
                                        {
                                            if (!jsonActual2.Any(x => x.Date == res.Date && x.Value == res.Value))
                                            {
                                                jsonActual2.Add(new ActualShiftPlanValue
                                                {
                                                    Date = res.Date,
                                                    Value = res.Value,
                                                    UserName = target.FullName
                                                });
                                            }
                                        }
                                    }
                                    _data.Actual2 = JsonConvert.SerializeObject(jsonActual2);
                                }*/
                            }
                            var expandoDict = _data as IDictionary<string, object>;
                            try
                            {
                                listResult.Add(new
                                {
                                    SAPCode = group.Key,
                                    Actual1 = GetValue(expandoDict, "Actual1"),
                                    Actual2 = GetValue(expandoDict, "Actual2")
                                });

                            }
                            catch (Exception ex)
                            {
                                result.ErrorCodes = new List<int> { 1004 };
                                result.Messages = new List<string> { ex.Message };
                            }
                        }
                    }

                }

                data.Data = new { BTA = listResultBTA, ShiftPlan = listResult };
                result.Object = data;

                return result;
            }
            catch (Exception ex)
            {
                result.Messages = new List<string> { ex.Message };
                return result;
            }

        }

        public async Task LoadDepartmentHierarchyAsync(Guid parentId, List<DepartmentViewModel> department)
        {
            var deptChild = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.ParentId == parentId);

            if (deptChild.Any())
            {
                foreach (var child in deptChild)
                {
                    department.Add(child);
                    await LoadDepartmentHierarchyAsync(child.Id, department);
                }
            }
        }

        public async Task<ResultDTO> GetActualShiftPlan_API(IntergationAPI args)
        {
            var result = new ResultDTO { };
            var data = new ArrayResultDTO();

            try
            {
                #region Period 
                if (args.Month == 0)
                {
                    if (args.Year != 0)
                        args.Month = DateTime.Now.Month;
                    else
                    {
                        args.Month = DateTime.Now.Month;
                        args.Year = DateTime.Now.Year;
                    }
                }
                else
                {
                    if (args.Year == 0) args.Year = DateTime.Now.Year;
                }
                var _fromDate = new DateTime();
                var _toDate = new DateTime();
                if (args.Month == 1)
                    _fromDate = new DateTime(args.Year - 1, 12, 26);
                else
                    _fromDate = new DateTime(args.Year, args.Month - 1, 26);

                _toDate = new DateTime(args.Year, args.Month, 25);
                var _period = args.Month + "/" + args.Year;
                #endregion
                List<DepartmentViewModel> department = new List<DepartmentViewModel>();
                List<object> listResult = new List<object>();
                var targetPlanPeriod = await _uow.GetRepository<TargetPlanPeriod>(true).FindByAsync(x => x.Name == _period);
                if (targetPlanPeriod != null)
                {
                    if (string.IsNullOrEmpty(args.DeptCode) && string.IsNullOrEmpty(args.LoginName) && string.IsNullOrEmpty(args.SAPCode))
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "DeptCode or LoginName or SAPCode must have value!" };
                        return result;
                    }
                    var getUserCurrent = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => (!string.IsNullOrEmpty(args.LoginName) && !string.IsNullOrEmpty(args.SAPCode)) ? (x.LoginName == args.LoginName && x.SAPCode == args.SAPCode)
                    : (string.IsNullOrEmpty(args.LoginName) ? ((string.IsNullOrEmpty(args.SAPCode) ? x.SAPCode != null : x.SAPCode == args.SAPCode)) : x.LoginName == args.LoginName));
                    if (getUserCurrent.Count() == 0)
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "This user does not exist in the system" };
                        return result;
                    }
                    var departmentItems = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => string.IsNullOrEmpty(args.DeptCode) ? !x.IsDeleted : x.Code == args.DeptCode);
                    if (!string.IsNullOrEmpty(args.DeptCode))
                    {
                        Guid id = departmentItems.FirstOrDefault().Id;
                        var deptChild = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.ParentId == id);
                        if (deptChild.Count() > 0)
                        {
                            foreach (var child in deptChild)
                                department.Add((DepartmentViewModel)child);
                        }
                        departmentItems = departmentItems.Concat(department);
                    }

                    var targetPlanPeriodID = targetPlanPeriod.FirstOrDefault().Id;
                    var targetplanDetails = await _uow.GetRepository<TargetPlanDetail>(true).FindByAsync<TargetPlanDetailViewModel>(x => x.TargetPlan.PeriodId == targetPlanPeriodID && x.TargetPlan.Status == "Completed");
                    List<TargetPlanDetailViewModel> tempTargetPlanDetails = new List<TargetPlanDetailViewModel>();
                    if (getUserCurrent.Count() > 0)
                    {
                        foreach (var u in getUserCurrent)
                            foreach (var t in targetplanDetails)
                                if (u.SAPCode == t.SAPCode) tempTargetPlanDetails.Add(t);
                        targetplanDetails = tempTargetPlanDetails;
                    }
                    if (departmentItems.Count() > 0)
                    {
                        tempTargetPlanDetails = new List<TargetPlanDetailViewModel>();
                        foreach (var d in departmentItems)
                            foreach (var t in targetplanDetails)
                                if (d.Code == t.DepartmentCode)
                                    tempTargetPlanDetails.Add(t);
                        targetplanDetails = tempTargetPlanDetails;

                    }
                    //getAllTrackingLogInitData
                    //choose item have RefenreceNumber == getAllTrackingLogInitData.RefenreceNumber && SAPCode == getAllTrackingLogInitData.SAPCode

                    /*tempTargetPlanDetails = new List<TargetPlanDetailViewModel>();
                    var trackingRequestForGetList = new List<TrackingRequestForGetListViewModel>();
                    var tempTrackingLogInitDatas = new List<TrackingLogInitDatasViewModel>();
                    var tempTargetDetailsId = targetplanDetails.Select(x => x.TargetPlanId).ToList();
                    var targetPlan = await _uow.GetRepository<TargetPlan>().FindByAsync<TargetPlanViewModel>(x => tempTargetDetailsId.Contains(x.Id));
                    foreach(var t in targetPlan)
                    {
                        var trackingLog = await _uow.GetRepository<TrackingLogInitData>().FindByAsync<TrackingLogInitDatasViewModel>(x => x.ReferenceNumber == t.ReferenceNumber *//*&& x.SAPCode == t.UserSAPCode*//*);
                        tempTrackingLogInitDatas.AddRange((List<TrackingLogInitDatasViewModel>)trackingLog);
                    }
                    var trackingLogInitDatas = tempTrackingLogInitDatas != null ? tempTrackingLogInitDatas : new List<TrackingLogInitDatasViewModel>();
                    var idTrackingLog = trackingLogInitDatas.Select(x=> x.TrackingLogId).ToList();
                    var trackingRequest = await _uow.GetRepository<TrackingRequest>().FindByAsync<TrackingRequestForGetListViewModel>(x => idTrackingLog.Contains(x.Id));
                    foreach (var temp in trackingRequest)
                        foreach (var target in targetplanDetails)
                        {
                            var jsonDetails = JsonConvert.DeserializeObject<TrackingPayLoad>(temp.Payload);
                            if (jsonDetails.RefNum == target.ReferenceNumber && (temp.Status.Trim().ToLower().Equals("success") || temp.Status.Trim().ToLower().Equals("save data success"))) 
                            {
                                tempTargetPlanDetails.Add(target);
                            }
                        }
                    targetplanDetails = tempTargetPlanDetails;*/

                    var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft" };

                    var listLeaveDetails = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => x.LeaveApplication.UserSAPCode != null && x.LeaveApplication.Status == "Completed" && !statusToCheckes.Contains(x.LeaveApplication.Status) &&
                                ((_fromDate <= x.FromDate && x.FromDate <= _toDate)
                                ||
                                (_fromDate <= x.ToDate && x.ToDate <= _toDate)), "", y => y.LeaveApplication);
                    var listShiftDetails = await _uow.GetRepository<ShiftExchangeApplicationDetail>().FindByAsync(x =>
                                (_fromDate <= x.ShiftExchangeDate && x.ShiftExchangeDate <= _toDate) && x.ShiftExchangeApplication.Status == "Completed" && !statusToCheckes.Contains(x.ShiftExchangeApplication.Status));

                    var tempIds = listLeaveDetails.Select(x => x.LeaveApplicationId).ToList();

                    var listLeaves = await _uow.GetRepository<LeaveApplication>(true).FindByAsync(x => tempIds.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                    var listLeavesId = listLeaves.Select(x => x.Id).ToList();
                    var tempIdShifts = listShiftDetails.Select(x => x.ShiftExchangeApplicationId).ToList();
                    var listShifts = await _uow.GetRepository<ShiftExchangeApplication>(true).FindByAsync(x => tempIdShifts.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                    var listShiftsId = listShifts.Select(x => x.Id).ToList();
                    var groupTargetPlans = targetplanDetails.GroupBy(x => x.SAPCode).ToList();

                    foreach (var group in groupTargetPlans)
                    {
                        List<DateValueArgs> jsonActual1 = new List<DateValueArgs>();
                        List<DateValueArgs> jsonActual2 = new List<DateValueArgs>();
                        dynamic _data = new ExpandoObject();
                        foreach (var target in group)
                        {
                            if (target.Type == TypeTargetPlan.Target1)
                            {
                                var dictList = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);
                                foreach (var dict in dictList)
                                {
                                    var lUser = getUserCurrent.FirstOrDefault(x => x.SAPCode == target.SAPCode);
                                    var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == target.SAPCode && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.Date));
                                    var lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                    int actualValue = 0;
                                    var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, target.Type, out actualValue);
                                    if (res.Value == null || res.Value == "") res = dict;
                                    if (actualValue == 0)
                                        jsonActual1.Add(res);
                                }
                                _data.Actual1 = JsonConvert.SerializeObject(jsonActual1);
                            }
                            else
                            {
                                var dictList = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);
                                foreach (var dict in dictList)
                                {
                                    var lUser = getUserCurrent.FirstOrDefault(x => x.SAPCode == target.SAPCode);
                                    var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == target.SAPCode && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.Date));
                                    var lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                    int actualValue = 0;
                                    var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, target.Type, out actualValue);
                                    if (res.Value == null || res.Value == "") res = dict;
                                    if (actualValue == 1)
                                        jsonActual2.Add(res);
                                }
                                _data.Actual2 = JsonConvert.SerializeObject(jsonActual2);
                            }
                        }
                        var expandoDict = _data as IDictionary<string, object>;
                        try
                        {
                            listResult.Add(new
                            {
                                SAPCode = group.Key,
                                Actual1 = GetValue(expandoDict, "Actual1"),
                                Actual2 = GetValue(expandoDict, "Actual2")
                            });

                        }
                        catch (Exception ex)
                        {
                            result.ErrorCodes = new List<int> { 1004 };
                            result.Messages = new List<string> { ex.Message };
                        }
                    }
                }
                data.Data = listResult;
                data.Count = listResult.Count();
                result.Object = data;
                return result;
            }
            catch (Exception ex)
            {

                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }

        private DateValueArgs GetLeaveOrShiftCode(DateValueArgs dict, LeaveApplicationDetail leave, ShiftExchangeApplicationDetail shift, IEnumerable<LeaveApplication> listLeaves, IEnumerable<ShiftExchangeApplication> listShifts, TypeTargetPlan typeTarget, out int actualValue)
        {
            var shiftCode = _uow.GetRepository<ShiftCode>().FindBy<ShiftCodeViewModel>(x => x.IsActive && !x.IsDeleted && !x.Code.Contains("*"));
            var mappingsTarget1 = shiftCode.Where(x => x.Line == ShiftLine.FULLDAY).ToList();
            var mappingsTarget2 = shiftCode.Where(x => x.Line == ShiftLine.HAFTDAY).ToList();
            DateValueArgs res = new DateValueArgs
            {
                Date = dict.Date,
                Value = ""
            };
            actualValue = 0; //is actual 1
            int assignFrom = 0;
            if ((leave != null) && (shift == null))
                assignFrom = 1;
            else if ((leave == null) && (shift != null))
                assignFrom = 2;
            if ((leave != null) && (shift != null))
            {
                //check leave<>shift modified/created sau cùng thì lấy
                var theLeave = listLeaves.FirstOrDefault(x => x.Id == leave.LeaveApplicationId);
                var theShift = listShifts.FirstOrDefault(x => x.Id == shift.ShiftExchangeApplicationId);
                var date1 = (theLeave != null) ? ((theLeave.Modified != null) ? theLeave.Modified : theLeave.Created) : DateTimeOffset.MinValue;
                var date2 = (theShift != null) ? ((theShift.Modified != null) ? theShift.Modified : theShift.Created) : DateTimeOffset.MinValue;
                if (date1 >= date2)
                {
                    if (typeTarget == TypeTargetPlan.Target1)
                    {
                        var targetValid = mappingsTarget1.Where(x => x.Code == leave.LeaveCode);
                        if (targetValid.Any())
                            assignFrom = 1;
                        else
                            assignFrom = 2;
                    }
                    else
                    {
                        var targetValid = mappingsTarget2.Where(x => x.Code == leave.LeaveCode);
                        if (targetValid.Any())
                            assignFrom = 1;
                        else
                            assignFrom = 2;
                    }
                }
                else
                {
                    assignFrom = 2;
                }
            }

            // List<string> halfCodes = ConfigurationManager.AppSettings["validateTargetPlanTarget1"].Split(',').Select(x => x.Trim()).ToList();
            if (assignFrom == 1)
            {   //check is actual2 or 1
                if (mappingsTarget2.Any(x => x.Code == leave.LeaveCode))
                    actualValue = 1;
                res = new DateValueArgs()
                {
                    Date = dict.Date,
                    Value = leave.LeaveCode
                };
            }
            else if (assignFrom == 2)
            {
                if (mappingsTarget2.Any(x => x.Code == shift.NewShiftCode))
                    actualValue = 1;
                res = new DateValueArgs()
                {
                    Date = dict.Date,
                    Value = shift.NewShiftCode,
                };
            }
            return res;
        }
        private object GetValue(IDictionary<string, object> queryWhere, string key)
        {
            object returnValue;
            if (!queryWhere.TryGetValue(key, out returnValue))
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        private bool CompareDateRangeAndStringDate(DateTimeOffset fromDate, DateTimeOffset toDate, string compareStringDate)
        {
            var dateToCheck = fromDate;
            bool isStopResult = false;
            while (!isStopResult && DateTime.Compare(dateToCheck.Date, toDate.Date.AddDays(1)) < 0)
            {
                isStopResult = CompareString(dateToCheck.DateTime.ToSAPFormat(), compareStringDate) == 0;
                dateToCheck = dateToCheck.AddDays(1);
            }
            return isStopResult;
        }
        private int CompareString(string input1, string input2)
        {
            return String.Compare(input1, input2, comparisonType: StringComparison.OrdinalIgnoreCase);
        }
        public async Task<ResultDTO> AccountVerification_API(string username, string password)
        {

            var result = new ResultDTO { };
            var data = new ArrayResultDTO();
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "Username/password are required!" };
                    return result;
                }
                var user = await _uow.GetRepository<User>(true).GetSingleAsync(x => x.LoginName.ToLower().Equals(username.ToLower()));
                if (user == null) {
                    return new ResultDTO() { ErrorCodes = new List<int>() { -1 } , Messages = new List<string> { "User does not exists!" } };
                }

                if (!user.IsActivated)
                {
                    return new ResultDTO() { ErrorCodes = new List<int>() { -1 }, Messages = new List<string> { "User has not been activated" } };
                }

                if (user.Type == LoginType.Membership)
                {
                    var findUserMS = Membership.GetUser(user.LoginName);
                    if (!findUserMS.IsApproved)
                    {
                        return new ResultDTO() { ErrorCodes = new List<int>() { -1 }, Messages = new List<string> { "User has been locked!" } };
                    }

                    bool isUserMSValid = Membership.ValidateUser(user.LoginName, password);
                    if (!isUserMSValid)
                    {
                        return new ResultDTO() { ErrorCodes = new List<int>() { -1 }, Messages = new List<string> { "MS - Incorrect password!" } };
                    }
                    result.Object = Mapper.Map<UserListAPIViewModel>(user);
                } else
                {
                    var domain = ConfigurationManager.AppSettings["siteUrl"];
                    domain = domain.Replace("http://", "").Replace("https://", "").Replace("/", "");
                    domain = domain.Contains("ssg") ? domain : domain.Replace("edoc.", "");
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
                    {
                        bool isValid = pc.ValidateCredentials(user.LoginName, password);
                        if (!isValid)
                        {
                            return new ResultDTO() { ErrorCodes = new List<int>() { -1 }, Messages = new List<string> { "AD - Incorrect password!" } };
                        }
                        // Tìm thông tin user trong AD
                        UserPrincipal adUser = UserPrincipal.FindByIdentity(pc, user.LoginName);
                        if (adUser == null)
                        {
                            return new ResultDTO() { ErrorCodes = new List<int>() { -1 }, Messages = new List<string> { "User not found in Active Directory!" } };
                        }
                        result.Object = Mapper.Map<UserListAPIViewModel>(user);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }

        public async Task<ResultDTO> GetActualShiftPlanForDWS_API(IntergationAPI args)
        {
            var result = new ResultDTO { };
            var data = new ArrayResultDTO();

            try
            {
                if (args.FromDate.HasValue && args.ToDate.HasValue && args.FromDate.Value > args.ToDate.Value)
                {
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "FromDate must be earlier than ToDate!" };
                    return result;
                }

                #region Period 
                var _fromDate = new DateTime();
                var _toDate = new DateTime();

                var lstTargetPlanPeriod = new List<TargetPlanPeriod>();
                if (args.FromDate.HasValue && args.ToDate.HasValue)
                {
                    _fromDate = args.FromDate.Value;
                    _toDate = args.ToDate.Value;

                    var periodFrom = await _uow.GetRepository<TargetPlanPeriod>().GetSingleAsync(x => x.FromDate <= _fromDate && _fromDate <= x.ToDate);
                    var periodTo = await _uow.GetRepository<TargetPlanPeriod>().GetSingleAsync(x => x.FromDate <= _toDate && _toDate <= x.ToDate);
                    var periodMid = await _uow.GetRepository<TargetPlanPeriod>().FindByAsync(x => x.FromDate <= _toDate && _fromDate <= x.ToDate);

                    if (periodFrom != null)
                    {
                        lstTargetPlanPeriod.Add(periodFrom);
                    }

                    if (periodTo != null && !lstTargetPlanPeriod.Any(x => x.Id == periodTo.Id))
                    {
                        lstTargetPlanPeriod.Add(periodTo);
                    }

                    if (periodMid.Any() && periodMid.Count() > 0)
                    {
                        foreach (var item in periodMid)
                        {
                            if (!lstTargetPlanPeriod.Any(x => x.Id == item.Id))
                                lstTargetPlanPeriod.Add(item);
                        }
                    }
                }
                else
                {
                    if (args.Month == 0)
                    {
                        if (args.Year != 0)
                            args.Month = DateTime.Now.Month;
                        else
                        {
                            args.Month = DateTime.Now.Month;
                            args.Year = DateTime.Now.Year;
                        }
                    }
                    else
                    {
                        if (args.Year == 0) args.Year = DateTime.Now.Year;
                    }
                    if (args.Month == 1)
                        _fromDate = new DateTime(args.Year - 1, 12, 26);
                    else
                        _fromDate = new DateTime(args.Year, args.Month - 1, 26);

                    _toDate = new DateTime(args.Year, args.Month, 25);
                }

                var _period = args.Month + "/" + args.Year;
                #endregion
                List<DepartmentViewModel> department = new List<DepartmentViewModel>();
                List<object> listResult = new List<object>();
                var targetPlanPeriod = await _uow.GetRepository<TargetPlanPeriod>(true).FindByAsync(x => x.Name == _period);

                if (lstTargetPlanPeriod.Count > 0)
                {
                    var periodIds = lstTargetPlanPeriod.Select(x => x.Id).ToList();
                    targetPlanPeriod = await _uow.GetRepository<TargetPlanPeriod>(true).FindByAsync(x => periodIds.Contains(x.Id), "FromDate asc");
                }

                if (targetPlanPeriod.Any())
                {
                    if (string.IsNullOrEmpty(args.DeptCode) && string.IsNullOrEmpty(args.LoginName) && string.IsNullOrEmpty(args.SAPCode))
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "DeptCode or LoginName or SAPCode must have value!" };
                        return result;
                    }

                    // --- AEONDWS-153 - Thực hiện điều chỉnh API chỉ lấy G1 --- \\
                    var getUserCurrent = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => (!string.IsNullOrEmpty(args.LoginName) && !string.IsNullOrEmpty(args.SAPCode)) ? (x.LoginName == args.LoginName && x.SAPCode == args.SAPCode)
                    : (string.IsNullOrEmpty(args.LoginName) ? ((string.IsNullOrEmpty(args.SAPCode) ? x.SAPCode != null : x.SAPCode == args.SAPCode)) : x.LoginName == args.LoginName)
                    && x.UserDepartmentMappings.Any(udm => udm.IsHeadCount && udm.Department.JobGrade.Grade == 1));

                    if (getUserCurrent.Count() == 0)
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "This user does not exist in the system" };
                        return result;
                    }

                    var departmentItems = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => string.IsNullOrEmpty(args.DeptCode) ? !x.IsDeleted : x.Code == args.DeptCode);
                    if (!string.IsNullOrEmpty(args.DeptCode))
                    {
                        var departmentList = new List<Guid>();
                        var allDepartments = await _uow.GetRepository<Department>().GetAllAsync<DepartmentTreeViewModel>();
                        var focusDepartment = await _uow.GetRepository<Department>().GetSingleAsync<DepartmentTreeViewModel>(x => x.Code == args.DeptCode);
                        if (focusDepartment != null && focusDepartment.IsIncludeChildren.HasValue && focusDepartment.IsIncludeChildren.Value)
                        {
                            CustomExpandAllNodes(departmentList, focusDepartment, allDepartments);
                        }

                        if (departmentList.Count() > 0)
                        {
                            var lstDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => departmentList.Contains(x.Id));
                            if (lstDepartment.Any())
                            {
                                foreach (var dept in lstDepartment)
                                {
                                    department.Add(dept);
                                }
                            }
                            departmentItems = departmentItems.Concat(department);
                        }
                    }

                    var targetPlanPeriodID = targetPlanPeriod.FirstOrDefault().Id;
                    var targetplanDetails = await _uow.GetRepository<TargetPlanDetail>(true).FindByAsync<TargetPlanDetailViewModel>(x => x.TargetPlan.PeriodId == targetPlanPeriodID && x.TargetPlan.Status == "Completed");
                    List<TargetPlanDetailViewModel> tempTargetPlanDetails = new List<TargetPlanDetailViewModel>();

                    foreach (var period in targetPlanPeriod)
                    {
                        targetPlanPeriodID = period.Id;
                        targetplanDetails = await _uow.GetRepository<TargetPlanDetail>(true).FindByAsync<TargetPlanDetailViewModel>(x => x.TargetPlan.PeriodId == targetPlanPeriodID && x.TargetPlan.Status == "Completed");

                        if (getUserCurrent.Count() > 0)
                        {
                            foreach (var u in getUserCurrent)
                            {
                                foreach (var t in targetplanDetails)
                                {
                                    if (u.SAPCode == t.SAPCode)
                                    {
                                        if (args.FromDate.HasValue && args.ToDate.HasValue && tempTargetPlanDetails.Any(x => x.SAPCode == t.SAPCode && x.Type == t.Type))
                                        {
                                            var temp = tempTargetPlanDetails.FirstOrDefault(x => x.SAPCode == t.SAPCode && x.Type == t.Type);
                                            var tempJsonData = JsonConvert.DeserializeObject<List<DateValueArgs>>(temp.JsonData);
                                            var jsonData = JsonConvert.DeserializeObject<List<DateValueArgs>>(t.JsonData);

                                            var finalJsonData = new List<DateValueArgs>();

                                            foreach (var d in tempJsonData)
                                            {
                                                if (args.FromDate.Value <= DateTime.ParseExact(d.Date, "yyyyMMdd", CultureInfo.InvariantCulture) && DateTime.ParseExact(d.Date, "yyyyMMdd", CultureInfo.InvariantCulture) <= args.ToDate.Value)
                                                {
                                                    finalJsonData.Add(d);
                                                }
                                            }

                                            foreach (var d in jsonData)
                                            {
                                                if (args.FromDate.Value <= DateTime.ParseExact(d.Date, "yyyyMMdd", CultureInfo.InvariantCulture) && DateTime.ParseExact(d.Date, "yyyyMMdd", CultureInfo.InvariantCulture) <= args.ToDate.Value)
                                                {
                                                    finalJsonData.Add(d);
                                                }
                                            }

                                            var index = tempTargetPlanDetails.IndexOf(temp);
                                            tempTargetPlanDetails[index].JsonData = JsonConvert.SerializeObject(finalJsonData);
                                        }
                                        else
                                        {
                                            if (args.FromDate.HasValue && args.ToDate.HasValue && (args.FromDate.Value > period.FromDate || args.ToDate.Value < period.ToDate))
                                            {
                                                var tempJsonData = JsonConvert.DeserializeObject<List<DateValueArgs>>(t.JsonData);
                                                var finalJsonData = new List<DateValueArgs>();

                                                foreach (var d in tempJsonData)
                                                {
                                                    if (args.FromDate.Value <= DateTime.ParseExact(d.Date, "yyyyMMdd", CultureInfo.InvariantCulture) && DateTime.ParseExact(d.Date, "yyyyMMdd", CultureInfo.InvariantCulture) <= args.ToDate.Value)
                                                    {
                                                        finalJsonData.Add(d);
                                                    }
                                                }

                                                t.JsonData = JsonConvert.SerializeObject(finalJsonData);
                                            }

                                            tempTargetPlanDetails.Add(t);
                                        }
                                    }
                                }
                            }
                            targetplanDetails = tempTargetPlanDetails;
                        }
                        if (departmentItems.Count() > 0)
                        {
                            tempTargetPlanDetails = new List<TargetPlanDetailViewModel>();
                            foreach (var d in departmentItems)
                            {
                                foreach (var t in targetplanDetails)
                                {
                                    if (d.Code == t.DepartmentCode)
                                    {
                                        if (args.FromDate.HasValue && args.ToDate.HasValue && tempTargetPlanDetails.Any(x => x.SAPCode == t.SAPCode && x.Type == t.Type))
                                        {
                                            var temp = tempTargetPlanDetails.FirstOrDefault(x => x.SAPCode == t.SAPCode && x.Type == t.Type);
                                            var tempJsonData = JsonConvert.DeserializeObject<List<DateValueArgs>>(temp.JsonData);
                                            var jsonData = JsonConvert.DeserializeObject<List<DateValueArgs>>(t.JsonData);

                                            var finalJsonData = new List<DateValueArgs>();

                                            foreach (var a in tempJsonData)
                                            {
                                                if (args.FromDate.Value <= DateTime.ParseExact(a.Date, "yyyyMMdd", CultureInfo.InvariantCulture) && DateTime.ParseExact(a.Date, "yyyyMMdd", CultureInfo.InvariantCulture) <= args.ToDate.Value)
                                                {
                                                    finalJsonData.Add(a);
                                                }
                                            }

                                            foreach (var a in jsonData)
                                            {
                                                if (args.FromDate.Value <= DateTime.ParseExact(a.Date, "yyyyMMdd", CultureInfo.InvariantCulture) && DateTime.ParseExact(a.Date, "yyyyMMdd", CultureInfo.InvariantCulture) <= args.ToDate.Value)
                                                {
                                                    finalJsonData.Add(a);
                                                }
                                            }

                                            var index = tempTargetPlanDetails.IndexOf(temp);
                                            tempTargetPlanDetails[index].JsonData = JsonConvert.SerializeObject(finalJsonData);
                                        }
                                        else
                                        {
                                            if (args.FromDate.HasValue && args.ToDate.HasValue && (args.FromDate.Value > period.FromDate || args.ToDate.Value < period.ToDate))
                                            {
                                                var tempJsonData = JsonConvert.DeserializeObject<List<DateValueArgs>>(t.JsonData);
                                                var finalJsonData = new List<DateValueArgs>();

                                                foreach (var a in tempJsonData)
                                                {
                                                    if (args.FromDate.Value <= DateTime.ParseExact(a.Date, "yyyyMMdd", CultureInfo.InvariantCulture) && DateTime.ParseExact(a.Date, "yyyyMMdd", CultureInfo.InvariantCulture) <= args.ToDate.Value)
                                                    {
                                                        finalJsonData.Add(a);
                                                    }
                                                }

                                                t.JsonData = JsonConvert.SerializeObject(finalJsonData);
                                            }

                                            tempTargetPlanDetails.Add(t);
                                        }
                                    }
                                }
                            }
                            targetplanDetails = tempTargetPlanDetails;

                        }
                    }

                    var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft" };

                    var listLeaveDetails = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => x.LeaveApplication.UserSAPCode != null && x.LeaveApplication.Status == "Completed" && !statusToCheckes.Contains(x.LeaveApplication.Status) &&
                                ((_fromDate <= x.FromDate && x.FromDate <= _toDate)
                                ||
                                (_fromDate <= x.ToDate && x.ToDate <= _toDate)), "", y => y.LeaveApplication);
                    var listShiftDetails = await _uow.GetRepository<ShiftExchangeApplicationDetail>().FindByAsync(x =>
                                (_fromDate <= x.ShiftExchangeDate && x.ShiftExchangeDate <= _toDate) && x.ShiftExchangeApplication.Status == "Completed" && !statusToCheckes.Contains(x.ShiftExchangeApplication.Status));

                    var tempIds = listLeaveDetails.Select(x => x.LeaveApplicationId).ToList();

                    var listLeaves = await _uow.GetRepository<LeaveApplication>(true).FindByAsync(x => tempIds.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                    var listLeavesId = listLeaves.Select(x => x.Id).ToList();
                    var tempIdShifts = listShiftDetails.Select(x => x.ShiftExchangeApplicationId).ToList();
                    var listShifts = await _uow.GetRepository<ShiftExchangeApplication>(true).FindByAsync(x => tempIdShifts.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                    var listShiftsId = listShifts.Select(x => x.Id).ToList();
                    var groupTargetPlans = targetplanDetails.GroupBy(x => x.SAPCode).ToList();

                    foreach (var group in groupTargetPlans)
                    {
                        List<DateValueArgs> jsonActual1 = new List<DateValueArgs>();
                        List<DateValueArgs> jsonActual2 = new List<DateValueArgs>();
                        dynamic _data = new ExpandoObject();
                        foreach (var target in group)
                        {
                            if (target.Type == TypeTargetPlan.Target1)
                            {
                                var dictList = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);
                                foreach (var dict in dictList)
                                {
                                    var lUser = getUserCurrent.FirstOrDefault(x => x.SAPCode == target.SAPCode);
                                    var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == target.SAPCode && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.Date));
                                    var lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                    int actualValue = 0;
                                    var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, target.Type, out actualValue);
                                    if (res.Value == null || res.Value == "") res = dict;
                                    if (actualValue == 0)
                                        jsonActual1.Add(res);
                                }
                                _data.Actual1 = JsonConvert.SerializeObject(jsonActual1);
                            }
                            else
                            {
                                var dictList = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);
                                foreach (var dict in dictList)
                                {
                                    var lUser = getUserCurrent.FirstOrDefault(x => x.SAPCode == target.SAPCode);
                                    var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == target.SAPCode && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.Date));
                                    var lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                    int actualValue = 0;
                                    var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, target.Type, out actualValue);
                                    if (res.Value == null || res.Value == "") res = dict;
                                    if (actualValue == 1)
                                        jsonActual2.Add(res);
                                }
                                _data.Actual2 = JsonConvert.SerializeObject(jsonActual2);
                            }
                        }
                        var expandoDict = _data as IDictionary<string, object>;
                        try
                        {
                            listResult.Add(new
                            {
                                SAPCode = group.Key,
                                Actual1 = GetValue(expandoDict, "Actual1"),
                                Actual2 = GetValue(expandoDict, "Actual2")
                            });

                        }
                        catch (Exception ex)
                        {
                            result.ErrorCodes = new List<int> { 1004 };
                            result.Messages = new List<string> { ex.Message };
                        }
                    }
                }
                data.Data = listResult;
                data.Count = listResult.Count();
                result.Object = data;
                return result;
            }
            catch (Exception ex)
            {

                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }

        public async Task<ResultDTO> GetUsersForTargetPlanByDeptIdDWS(UserForDWSArg arg)
        {
            var result = new ResultDTO();
            string message = "";
            try
            {
                bool isContinue = true;
                IEnumerable<ResignationApplicationViewModel> allResinations = null;
                var ids = arg.Items.Select(s => s.Id);
                var currentPeriod = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId.Value);
                if (currentPeriod == null)
                {
                    return new ResultDTO
                    {
                        ErrorCodes = new List<int> { 1004 },
                        Messages = new List<string> { "Target plan period not found!" }
                    };
                }
                int totalItems = 0;
                var res = new List<UserForTreeViewModel>();
                var tempDatas = new List<UserForTreeViewModel>();
                //var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.LoginName == arg.LoginName);
                /*if (currentUser != null)
                {

                    if (currentUser.IsStore.Value && currentUser.JobGradeValue >= 4 || !currentUser.IsStore.Value && currentUser.JobGradeValue >= 5)
                    {
                        if (arg.PeriodId.HasValue)
                        {
                            //code moi
                            if (arg.Type == TypeTaget.ShiftPlan)
                            {
                                var sapCodes = new List<UserForTreeViewModel>();
                                var sapCodesCompare = new List<string>();
                                var departmentList = new List<Guid>();
                                var dept = new Department();
                                if (arg.DivisionId != null)
                                {
                                    dept = await _uow.GetRepository<Department>().FindByIdAsync(arg.DivisionId.Value);
                                    departmentList.Add(arg.DivisionId.Value);
                                }
                                else
                                {
                                    dept = await _uow.GetRepository<Department>().FindByIdAsync(currentUser.DepartmentId.Value);
                                    departmentList.Add(currentUser.DepartmentId.Value);
                                }
                                if (dept != null)
                                {
                                    var allDepts = await _uow.GetRepository<Department>().GetAllAsync();
                                    ExpandAllNodes(departmentList, dept, allDepts);
                                    foreach (var item in departmentList)
                                    {
                                        var userDepartments = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.DepartmentId == item && x.User.IsActivated);
                                        if (userDepartments.Any())
                                        {
                                            var users = userDepartments.Select(x => x.User).ToList();
                                            sapCodes.AddRange(Mapper.Map<List<UserForTreeViewModel>>(users));
                                            sapCodesCompare.AddRange(userDepartments.Select(x => x.User.SAPCode));
                                        }
                                    }
                                }
                                if (sapCodes.Any())
                                {
                                    tempDatas.AddRange(sapCodes);
                                    //tempDatas = tempDatas.Skip((arg.Query.Page - 1) * arg.Query.Limit).Take(arg.Query.Limit).ToList();
                                }
                                var allPendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PendingTargetPlan.PeriodId == arg.PeriodId && sapCodesCompare.Contains(x.SAPCode) || ids.Contains(x.PendingTargetPlan.DeptId.Value) || ids.Contains(x.PendingTargetPlan.DivisionId.Value));
                                foreach (var item in tempDatas)
                                {
                                    var currentDetailItem = allPendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                                    if (currentDetailItem != null)
                                    {
                                        item.IsSent = currentDetailItem.IsSent;
                                        item.IsSubmitted = currentDetailItem.IsSubmitted;
                                    }
                                }
                                //
                                tempDatas.ForEach(x =>
                                {
                                    if (!x.IsSent.HasValue)
                                    {
                                        x.IsSent = false;
                                    }
                                    if (!x.IsSubmitted.HasValue)
                                    {
                                        x.IsSubmitted = false;
                                    }
                                });
                                if (!String.IsNullOrEmpty(arg.Query.Predicate))
                                {
                                    tempDatas = tempDatas.AsQueryable().Where(arg.Query.Predicate, arg.Query.PredicateParameters).ToList();
                                }
                                if (tempDatas.Any())
                                {
                                    res.AddRange(tempDatas);
                                }
                            }
                            else
                            {
                                var isValid = true;
                                if (!currentUser.StartDate.HasValue)
                                {
                                    currentUser.StartDate = new DateTime(2016, 01, 01);
                                }
                                if (currentUser.StartDate <= currentPeriod.ToDate)
                                {
                                    tempDatas.Add(currentUser);
                                    totalItems = 1;
                                }
                                var lastResignation = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync<ResignationApplicationViewModel>(x => x.Status.Contains("Completed") && currentUser.SAPCode == x.UserSAPCode, "Created desc");
                                if (lastResignation != null)
                                {
                                    if (currentUser.StartDate.HasValue && lastResignation.OfficialResignationDate.Date >= currentUser.StartDate.Value.Date)
                                    {
                                        if (lastResignation.OfficialResignationDate.Date >= currentPeriod.FromDate.Date && lastResignation.OfficialResignationDate.Date <= currentPeriod.ToDate.Date)
                                        {
                                            currentUser.OfficialResignationDate = lastResignation.OfficialResignationDate;
                                        }
                                        else if (currentUser.StartDate.Value.Date <= currentPeriod.ToDate.Date && lastResignation.OfficialResignationDate.Date.Date > currentPeriod.FromDate.Date)
                                        {
                                            currentUser.OfficialResignationDate = null;
                                        }
                                        else
                                        {
                                            isValid = false;
                                        }
                                    }
                                }
                                if (isValid)
                                {
                                    var allPendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PendingTargetPlan.PeriodId == arg.PeriodId && x.SAPCode == currentUser.SAPCode || ids.Contains(x.PendingTargetPlan.DeptId.Value) || ids.Contains(x.PendingTargetPlan.DivisionId.Value));
                                    foreach (var item in tempDatas)
                                    {
                                        var currentDetailItem = allPendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                                        if (currentDetailItem != null)
                                        {
                                            item.IsSent = currentDetailItem.IsSent;
                                            item.IsSubmitted = currentDetailItem.IsSubmitted;
                                        }
                                    }
                                    tempDatas.ForEach(x =>
                                    {
                                        if (!x.IsSent.HasValue)
                                        {
                                            x.IsSent = false;
                                        }
                                        if (!x.IsSubmitted.HasValue)
                                        {
                                            x.IsSubmitted = false;
                                        }
                                    });
                                    if (!String.IsNullOrEmpty(arg.Query.Predicate))
                                    {
                                        tempDatas = tempDatas.AsQueryable().Where(arg.Query.Predicate, arg.Query.PredicateParameters).ToList();
                                    }
                                    if (tempDatas.Any())
                                    {
                                        res.AddRange(tempDatas);
                                    }
                                }
                            }

                        }
                        else
                        {
                            res.AddRange(tempDatas);
                        }
                    }
                    else
                    {
                        isContinue = true;
                    }

                }*/
                if (isContinue)
                {
                    message = "isContinue";
                    var invalidUsers = new List<string>(); // check resignation date
                    var allSubmitPersons = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync<UserSubmitPersonDepartmentMappingViewModel>(x => x.IsSubmitPerson);
                    var allDepartments = await _uow.GetRepository<Department>().GetAllAsync<DepartmentTreeViewModel>();

                    message = "allDepartments";
                    try
                    {
                        foreach (var department in arg.Items)
                        {
                            var dept = allDepartments.FirstOrDefault(x => x.Id == department.Id);
                            DepartmentTreeViewModel forcusDerpartment = null;
                            if (dept != null && dept.Type == DepartmentType.Department)
                            {
                                // Tìm department có người submit
                                var hasSubmitPerson = allSubmitPersons.Any(x => x.DepartmentId == dept.Id);
                                while (!hasSubmitPerson && dept != null && dept.ParentId.HasValue)
                                {
                                    dept = allDepartments.FirstOrDefault(x => x.Id == dept.ParentId);
                                    if (dept != null)
                                    {
                                        hasSubmitPerson = allSubmitPersons.Any(x => x.DepartmentId == dept.Id);
                                    }
                                }
                                if (hasSubmitPerson)
                                {
                                    forcusDerpartment = dept;
                                }

                            }
                            else
                            {
                                forcusDerpartment = dept;
                            }
                            var currentUsers = new List<UserForTreeViewModel>();
                            if (forcusDerpartment != null)
                            {
                                forcusDerpartment.IsIncludeChildren = department.IsIncludeChildren;
                                var departmentList = new List<Guid>();
                                if (arg.IsNoDivisionChosen)
                                {
                                    if (forcusDerpartment.JobGradeGrade < 4)
                                    {
                                        while (forcusDerpartment.JobGradeGrade < 4)
                                        {
                                            forcusDerpartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentTreeViewModel>(forcusDerpartment.ParentId.Value);
                                        }
                                    }
                                    if (forcusDerpartment.ParentId.HasValue)
                                    {
                                        departmentList.Add(forcusDerpartment.ParentId.Value);
                                    }
                                }
                                departmentList.Add(forcusDerpartment.Id);
                                currentUsers = await GetDetailUserInTargetPlan(departmentList, forcusDerpartment, allDepartments, allSubmitPersons);
                            }
                            else
                            {
                                var departmentList = new List<Guid>();
                                var currentDept = allDepartments.FirstOrDefault(x => x.Id == department.Id);
                                departmentList.Add(department.Id);
                                currentUsers = await GetDetailUserInTargetPlan(departmentList, currentDept, allDepartments, allSubmitPersons);
                            }
                            if (currentUsers.Count > 0)
                            {
                                //Khiem - Fix target plan load user co start date sau period
                                currentUsers.RemoveAll(x => x.StartDate > currentPeriod.ToDate);
                                tempDatas.AddRange(currentUsers);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                    }
                    var sapCodes = tempDatas.Select(x => x.SAPCode);
                    allResinations = await _uow.GetRepository<ResignationApplication>(true).FindByAsync<ResignationApplicationViewModel>(x => x.Status.Contains("Completed") && sapCodes.Contains(x.UserSAPCode), "Created desc");
                    if (allResinations.Any())
                    {
                        foreach (var item in allResinations)
                        {
                            var condition1 = item.OfficialResignationDate < currentPeriod.FromDate;
                            var condition2 = tempDatas.FirstOrDefault(x => x.SAPCode == item.UserSAPCode)?.StartDate.Value.Date < item.OfficialResignationDate.Date;
                            if (condition1 && condition2)
                            {
                                invalidUsers.Add(item.UserSAPCode);
                            }
                        }
                    }
                    message = "arg.PeriodId.HasValue";
                    if (arg.PeriodId.HasValue)
                    {
                        var allPendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PendingTargetPlan.PeriodId == arg.PeriodId && (sapCodes.Contains(x.SAPCode) || ids.Contains(x.PendingTargetPlan.DeptId.Value) || ids.Contains(x.PendingTargetPlan.DivisionId.Value)));

                        var allTargetPlans = await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanDetailViewModel>(x => x.TargetPlan.PeriodId == arg.PeriodId && (sapCodes.Contains(x.SAPCode) || ids.Contains(x.TargetPlan.DeptId.Value) || ids.Contains(x.TargetPlan.DivisionId.Value)));

                        foreach (var item in tempDatas)
                        {
                            var currentDetailItem = allPendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                            var currentTargetDetailItem = allTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);

                            //Kiem tra xem user da co target plan chua
                            if (currentDetailItem != null || currentTargetDetailItem != null)
                            {
                                item.IsHasTargetPlan = true;
                            }

                            if (currentDetailItem != null)
                            {
                                item.IsSent = currentDetailItem.IsSent;
                                item.IsSubmitted = currentDetailItem.IsSubmitted;
                            }
                        }
                        tempDatas.ForEach(x =>
                        {
                            if (!x.IsSent.HasValue)
                            {
                                x.IsSent = false;
                            }
                            if (!x.IsSubmitted.HasValue)
                            {
                                x.IsSubmitted = false;
                            }
                        });
                        if (arg.Query != null && !String.IsNullOrEmpty(arg.Query.Predicate))
                        {
                            tempDatas = tempDatas.AsQueryable().Where(arg.Query.Predicate, arg.Query.PredicateParameters).ToList();
                        }

                    }

                    if (arg.ActiveUsers != null && arg.ActiveUsers.Length > 0)
                    {
                        message = "arg.ActiveUsers";
                        var currenrtActiveUsers = tempDatas.Where(x => arg.ActiveUsers.ToList().Contains(x.SAPCode));
                        if (currenrtActiveUsers.Any())
                        {

                            tempDatas = tempDatas.Where(x => !currenrtActiveUsers.Contains(x)).ToList();
                            res.AddRange(currenrtActiveUsers);
                            res.AddRange(tempDatas);
                        }
                        else
                        {
                            res.AddRange(tempDatas);
                        }
                    }
                    else
                    {
                        res.AddRange(tempDatas);
                    }
                    message = " res = res.Where(x => !invalidUsers.Contains(x.SAPCode)).";

                    //--- AEONDWS-150 - AEONDWS-156 --- chi lay user G1 ---\\
                    res = res.Where(x => !invalidUsers.Contains(x.SAPCode)).Where(x => x.JobGradeValue == 1).OrderByDescending(x => x.JobGradeValue).ToList();
                    message = " res = res.Where(x => !invalidUsers.Contains(x.SAPCode)).";
                    /*var notIncludeSAPCodes = new List<string>();
                    foreach (var item in res)
                    {
                        if ((await IsG5UpHQOrG4Store(item.SAPCode, currentUser.SAPCode)))
                        {
                            notIncludeSAPCodes.Add(item.SAPCode);
                        }
                    }
                    if (notIncludeSAPCodes.Any())
                    {
                        res = res.Where(x => !notIncludeSAPCodes.Contains(x.SAPCode)).ToList();
                    }*/
                    totalItems = res.Count();
                    if (arg.Query != null)
                    {
                        res = res.Skip((arg.Query.Page - 1) * arg.Query.Limit).Take(arg.Query.Limit).ToList();
                    }

                }
                if (res.Any() && allResinations != null)
                {
                    message = "allResinations";
                    var removeUsers = new List<string>();
                    foreach (var item in res)
                    {
                        var currentResignation = allResinations.FirstOrDefault(x => x.UserSAPCode == item.SAPCode);
                        if (currentResignation != null)
                        {

                            if (currentResignation.OfficialResignationDate >= item.StartDate)
                            {
                                if (currentResignation.OfficialResignationDate.Date <= currentPeriod.FromDate.Date)
                                {
                                    removeUsers.Add(item.SAPCode);
                                }
                                else
                                {
                                    item.OfficialResignationDate = currentResignation.OfficialResignationDate;
                                }
                            }
                        }
                    }
                    if (removeUsers.Count > 0)
                    {
                        res = res.Where(x => !removeUsers.Contains(x.SAPCode)).ToList();
                        totalItems = res.Count();
                    }
                }
                message = "// kiem tra them nhan vien co can lam targetplant hay khong";
                // kiem tra them nhan vien co can lam targetplant hay khong
                if (res.Any())
                {
                    message = "if (res.Any())";
                    res = res.Where(x => x.IsNotTargetPlan == false).ToList();

                    foreach (var user in res)
                    {
                        var userDetail = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == user.Id);
                        if (userDetail != null)
                        {
                            var quotaData = string.IsNullOrEmpty(userDetail.QuotaDataJson) ? null : JsonConvert.DeserializeObject<QuotaDataJsonDTO>(userDetail.QuotaDataJson);
                            if (quotaData != null)
                            {
                                var currentQuotaData = quotaData.JsonData.FirstOrDefault(x => x.Year == currentPeriod.ToDate.Year);
                                if (currentQuotaData != null)
                                {
                                    user.ERDRemain = currentQuotaData.ERDRemain;
                                    user.ALRemain = currentQuotaData.ALRemain;
                                    user.DOFLRemain = currentQuotaData.DOFLRemain;
                                }
                                if (currentPeriod.FromDate.Year != currentPeriod.ToDate.Year)
                                {
                                    var allRemain = quotaData.JsonData.Where(x => x.Year == currentPeriod.FromDate.Year || x.Year == currentPeriod.ToDate.Year);
                                    user.ERDRemain = allRemain.Sum(x => x.ERDRemain);
                                    user.ALRemain = allRemain.Sum(x => x.ALRemain);
                                    user.DOFLRemain = allRemain.Sum(x => x.DOFLRemain);
                                }
                            }
                        }
                    }

                    // kiem tra user hien tai co quyen lam target lan hay khong
                    /*if (currentUser.IsNotTargetPlan == true)
                    {
                        res = new List<UserForTreeViewModel>();
                    }*/
                    totalItems = res.Count();
                }
                result.Object = new { Items = res, TotalItems = totalItems };
            }
            catch (Exception e)
            {
                result = new ResultDTO { Object = e, ErrorCodes = { 1004 }, Messages = { e.Message } };
            }
            return result;
        }

        private void ExpandAllNodes(List<Guid> deptList, Department parentNode, IEnumerable<Department> allNodes)
        {
            var childNodes = allNodes.Where(x => x.ParentId == parentNode.Id).ToList();
            if (childNodes.Count() > 0)
            {
                deptList.AddRange(childNodes.Select(x => x.Id).ToList());
                foreach (var childNode in childNodes)
                {
                    ExpandAllNodes(deptList, childNode, allNodes);
                }
            }
        }

        private async Task<List<UserForTreeViewModel>> GetDetailUserInTargetPlan(List<Guid> departmentList, DepartmentTreeViewModel focusDepartment, IEnumerable<DepartmentTreeViewModel> allDepartments, IEnumerable<UserSubmitPersonDepartmentMappingViewModel> allSubmitPersons)
        {
            if (focusDepartment != null)
            {
                if (focusDepartment.IsIncludeChildren.HasValue)
                {
                    if (focusDepartment.IsIncludeChildren == true)
                    {
                        CustomExpandAllNodes(departmentList, focusDepartment, allDepartments);
                    }
                    else
                    {
                        departmentList = new List<Guid> { focusDepartment.Id };
                    }
                }
                else
                {
                    CustomExpandAllNodes(departmentList, focusDepartment, allDepartments);
                }
            }

            var items = (await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => x.IsActivated && x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value) && y.IsHeadCount))).ToList();
            return items;
        }

        private void CustomExpandAllNodes(List<Guid> deptList, DepartmentTreeViewModel parentNode, IEnumerable<DepartmentTreeViewModel> allNodes)
        {
            var childNodes = allNodes.Where(x => x.ParentId == parentNode.Id).ToList();
            if (childNodes.Count() > 0)
            {
                deptList.AddRange(childNodes.Select(x => x.Id).ToList());
                foreach (var childNode in childNodes)
                {
                    CustomExpandAllNodes(deptList, childNode, allNodes);
                }
            }
        }

        private async Task<bool> IsG5UpHQOrG4Store(string checkSAPCode, string currentLoginSapCode)
        {
            var result = false;
            try
            {

                var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode == checkSAPCode && x.IsActivated);
                if (currentUser != null && currentUser.SAPCode != currentLoginSapCode && ((currentUser.JobGradeValue.Value >= 5 && !currentUser.IsStore.Value) || (currentUser.JobGradeValue.Value >= 4 && currentUser.IsStore.Value)))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
