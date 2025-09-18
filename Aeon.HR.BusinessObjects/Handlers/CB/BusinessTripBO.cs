using Aeon.HR.BusinessObjects.Handlers.FIle;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.PrintFormViewModel;
using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using OfficeOpenXml;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Utilities;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Department = Aeon.HR.Data.Models.Department;

namespace Aeon.HR.BusinessObjects.Handlers.CB
{
    public class BusinessTripBO : IBusinessTripBO
    {
        private readonly IUnitOfWork _uow;
        private readonly IWorkflowBO _workflowBO;
        private readonly ISettingBO _settingBO;
        private readonly ILogger logger;
        private Guid? _refDeparmentId = null;
        public BusinessTripBO(IUnitOfWork uow, ILogger _logger, IWorkflowBO workflowBO, ISettingBO settingBO)
        {
            _uow = uow;
            logger = _logger;
            _workflowBO = workflowBO;
            _settingBO = settingBO;
        }
        /// <summary>
        /// Trả về phiếu BTA từ Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultDTO> GetItemById(Guid id)
        {
            bool allPermissionData = false;
            var currentUserId = _uow.UserContext.CurrentUserId;
            // Kiem tra user hien tai cho chua trong bta detail?
            var btaTripCurrentUser = await _uow.GetRepository<BusinessTripApplication>(true).GetSingleAsync<BTAViewModel>(x => x.Id == id);
            if (btaTripCurrentUser != null)
            {
                var btaDetail = await _uow.GetRepository<BusinessTripApplicationDetail>(true).FindByAsync(x => x.BusinessTripApplicationId == btaTripCurrentUser.Id);
                if (btaDetail != null)
                {
                    allPermissionData = btaDetail.Any(x => x.UserId == currentUserId);
                }
            }

            var result = new ResultDTO();
            if (allPermissionData)
            {
                var currentItem = await _uow.GetRepository<BusinessTripApplication>(allPermissionData).FindByIdAsync<BTAViewModel>(id);
                if (currentItem != null)
                {
                    var record = Mapper.Map<BusinessTripItemViewModel>(currentItem);
                    var details = _uow.GetRepository<BusinessTripApplicationDetail>(allPermissionData).FindBy<BusinessTripDetailDTO>(x => x.BusinessTripApplicationId == id, string.Empty).ToList();
                    if (details != null && details.Any())
                    {
                        record.BusinessTripDetails = details.OrderBy(x => x.FromDate).ThenBy(x => x.ToDate);
                    }
                    var changeCancelDetails = await _uow.GetRepository<ChangeCancelBusinessTripDetail>(allPermissionData).FindByAsync<ChangeCancelBusinessTripDTO>(x => x.BusinessTripApplicationId == id, string.Empty);
                    if (changeCancelDetails != null && changeCancelDetails.Any())
                    {
                        record.ChangeCancelBusinessTripDetails = changeCancelDetails.OrderBy(x => (x.NewFromDate != null && x.NewFromDate.HasValue) ? x.NewFromDate : x.FromDate).ThenBy(x => (x.NewToDate != null && x.NewToDate.HasValue) ? x.NewToDate : x.ToDate);
                    }
                    //lamnl
                    //tuhm confirm
                    var btaOverBudget = await _uow.GetRepository<BusinessTripOverBudget>(allPermissionData).FindByAsync<BTAOverBudgetViewModel>(x => x.BusinessTripApplicationId == id, "");
                    if (btaOverBudget != null && btaOverBudget.Any())
                    {
                        var listOverBudget = new List<BusinessOverBudgetDTO>();
                        foreach (var overs in btaOverBudget)
                        {
                            var over = Mapper.Map<BusinessOverBudgetDTO>(overs);
                            listOverBudget.Add(over);
                        }
                        record.BusinessOverBudgets = listOverBudget;
                    }
                    //
                    var roomOrganizationDetails = await _uow.GetRepository<RoomOrganization>(allPermissionData).FindByAsync<RoomOrganizationDTO>(x => x.BusinessTripApplicationId == id, "RoomTypeName");
                    if (roomOrganizationDetails != null && roomOrganizationDetails.Any())
                    {
                        var roomOrgIds = roomOrganizationDetails.Select(x => x.Id);
                        var roomUserMappings = await _uow.GetRepository<RoomUserMapping>(allPermissionData).FindByAsync(x => roomOrgIds.Contains(x.RoomOrganizationId), "", z => z.User);
                        if (roomUserMappings.Any())
                        {
                            var changeRooms = new List<RoomOrganizationDTO>();

                            foreach (var room in roomOrganizationDetails)
                            {
                                var usersInRoom = roomUserMappings.Where(x => x.RoomOrganizationId == room.Id);
                                if (usersInRoom != null && usersInRoom.Any())
                                {
                                    var changeRoom = new RoomOrganizationDTO { RoomTypeId = room.RoomTypeId, RoomTypeCode = room.RoomTypeCode, RoomTypeName = room.RoomTypeName, Id = room.Id };
                                    foreach (var item in usersInRoom)
                                    {
                                        var user = Mapper.Map<SimpleUserDTO>(item);
                                        if (user.IsChange)
                                        {
                                            changeRoom.Users.Add(user);
                                        }
                                        else
                                        {
                                            room.Users.Add(user);
                                        }
                                    }
                                    if (changeRoom.Users.Any())
                                    {
                                        changeRooms.Add(changeRoom);
                                    }
                                }
                            }
                            record.RoomOrganizations = roomOrganizationDetails.Where(x => x.Users.Any());
                            record.ChangedRoomOrganizations = changeRooms;
                        }
                    }
                    result.Object = record;
                }
                else
                {
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "No Data" };
                }
            }
            else
            {
                var currentItem = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync<BTAViewModel>(id);
                if (currentItem != null)
                {
                    var record = Mapper.Map<BusinessTripItemViewModel>(currentItem);
                    var details = _uow.GetRepository<BusinessTripApplicationDetail>().FindBy<BusinessTripDetailDTO>(x => x.BusinessTripApplicationId == id, string.Empty).ToList();
                    if (details != null && details.Any())
                    {
                        record.BusinessTripDetails = details.OrderBy(x => x.FromDate).ThenBy(x => x.ToDate);
                    }
                    var changeCancelDetails = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDTO>(x => x.BusinessTripApplicationId == id, string.Empty);
                    if (changeCancelDetails != null && changeCancelDetails.Any())
                    {
                        record.ChangeCancelBusinessTripDetails = changeCancelDetails.OrderBy(x => (x.NewFromDate != null && x.NewFromDate.HasValue) ? x.NewFromDate : x.FromDate).ThenBy(x => (x.NewToDate != null && x.NewToDate.HasValue) ? x.NewToDate : x.ToDate);
                    }
                    //lamnl
                    //tuhm confirm
                    var btaOverBudget = await _uow.GetRepository<BusinessTripOverBudget>().FindByAsync<BTAOverBudgetViewModel>(x => x.BusinessTripApplicationId == id, "");
                    if (btaOverBudget != null && btaOverBudget.Any())
                    {
                        var listOverBudget = new List<BusinessOverBudgetDTO>();
                        foreach (var overs in btaOverBudget)
                        {
                            var over = Mapper.Map<BusinessOverBudgetDTO>(overs);
                            listOverBudget.Add(over);
                        }
                        record.BusinessOverBudgets = listOverBudget;
                    }
                    //
                    var roomOrganizationDetails = await _uow.GetRepository<RoomOrganization>().FindByAsync<RoomOrganizationDTO>(x => x.BusinessTripApplicationId == id, "RoomTypeName");

                    if (roomOrganizationDetails != null && roomOrganizationDetails.Any())
                    {
                        var roomOrgIds = roomOrganizationDetails.Select(x => x.Id);
                        var roomUserMappings = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => roomOrgIds.Contains(x.RoomOrganizationId), "", z => z.User);
                        if (roomUserMappings.Any())
                        {
                            var changeRooms = new List<RoomOrganizationDTO>();

                            foreach (var room in roomOrganizationDetails)
                            {
                                var usersInRoom = roomUserMappings.Where(x => x.RoomOrganizationId == room.Id);
                                if (usersInRoom != null && usersInRoom.Any())
                                {
                                    var changeRoom = new RoomOrganizationDTO { RoomTypeId = room.RoomTypeId, RoomTypeCode = room.RoomTypeCode, RoomTypeName = room.RoomTypeName, Id = room.Id };
                                    foreach (var item in usersInRoom)
                                    {
                                        var user = Mapper.Map<SimpleUserDTO>(item);
                                        if (user.IsChange)
                                        {
                                            changeRoom.Users.Add(user);
                                        }
                                        else
                                        {
                                            room.Users.Add(user);
                                        }
                                    }
                                    if (changeRoom.Users.Any())
                                    {
                                        changeRooms.Add(changeRoom);
                                    }
                                }
                            }
                            record.RoomOrganizations = roomOrganizationDetails.Where(x => x.Users.Any());
                            record.ChangedRoomOrganizations = changeRooms;
                        }
                    }
                    result.Object = record;
                }
            }

            return result;
        }

        public async Task<BusinessTripApplication> GetById(Guid id)
        {
            var item = await _uow.GetRepository<BusinessTripApplication>(true).GetSingleAsync(x => x.Id == id);
            return item;
        }

        public async Task<BTAErrorMessageViewModel> GetBTAErrorMessageByCode(BTAErrorEnums type, string errorCode)
        {
            return await _settingBO.GetBTAErrorMessageByCode(type, errorCode);
        }

        /// <summary>
        /// Lấy danh sách My/All Request BTA
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<ResultDTO> GetList(QueryArgs arg)
        {
            var allRequestTrip = new List<BTAViewModel>();
            var count = 0;

            var currentUserId = _uow.UserContext.CurrentUserId;
            // Kiem tra user hien tai cho chua trong bta detail?

            var allBusinessTrips = await _uow.GetRepository<BusinessTripApplication>().FindByAsync<BTAViewModel>(arg.Predicate, arg.PredicateParameters);
            var countBusinessTrip = await _uow.GetRepository<BusinessTripApplication>().CountAsync(arg.Predicate, arg.PredicateParameters);


            allRequestTrip.AddRange(allBusinessTrips);
            count += countBusinessTrip;

            // loai tru man hinh my item
            if (string.IsNullOrEmpty(arg.Predicate) || (!string.IsNullOrEmpty(arg.Predicate) && !arg.Predicate.Contains("CreatedById")))
            {
                if (!string.IsNullOrEmpty(arg.Predicate))
                {
                    arg.Predicate += " and ";
                }

                // Predicate
                arg.Predicate += " BusinessTripApplicationDetails.Any(x => x.UserId == (@" + arg.PredicateParameters.Length + "))";
                var predicate = arg.PredicateParameters.ToList();
                predicate.Add(currentUserId);
                arg.PredicateParameters = predicate.ToArray();

                // Get List
                var btaTripCurrentUser = await _uow.GetRepository<BusinessTripApplication>(true).FindByAsync<BTAViewModel>(arg.Predicate, arg.PredicateParameters);
                btaTripCurrentUser = btaTripCurrentUser.Where(e => !allBusinessTrips.Any(x => x.Id == e.Id)).ToList();
                var countbtaTripCurrentUser = btaTripCurrentUser.Count(); //await _uow.GetRepository<BusinessTripApplication>(true).CountAsync(arg.Predicate, arg.PredicateParameters);
                if (btaTripCurrentUser.Any())
                {
                    btaTripCurrentUser = btaTripCurrentUser.Where(e => !allBusinessTrips.Any(x => x.Id == e.Id)).ToList();
                    countbtaTripCurrentUser = btaTripCurrentUser.Count();
                }
            }
            var allRequestPageSize = allRequestTrip.OrderByDescending(x => x.BusinessTripFrom).Skip((arg.Page - 1) * arg.Limit).Take(arg.Limit);

            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = allRequestPageSize,
                    Count = count
                }
            };
        }
        private async Task<int> CountUsersOnSameFlight(BookingFlightViewModel bookingDetail)
        {
            int result = 1;
            try
            {
                var count = await _uow.GetRepository<BookingFlight>(true).FindByAsync(x => x.BTADetail.BusinessTripApplicationId == bookingDetail.BTADetail.BusinessTripApplicationId
                && x.FlightDetail.FlightNo == bookingDetail.FlightDetail.FlightNo && x.FlightDetail.DepartureDateTime == bookingDetail.FlightDetail.DepartureDateTime
                && x.FlightDetail.ArrivalDateTime == bookingDetail.FlightDetail.ArrivalDateTime);
                result = count.Count();
            }
            catch (Exception)
            {
                result = 1;
            }
            return result;
        }
        public async Task<ResultDTO> GetReports(ReportType type, QueryArgs args)
        {
            var result = new List<IReportBTA>();
            int count = 0;

            if (type == ReportType.Flight || type == ReportType.Hotel)
            {
                var allBusinessTrips = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync<BtaDetailViewModel>(x => x.BusinessTripApplication.Status == "Completed" || x.BusinessTripApplication.Status == "Completed Changing");
                if (allBusinessTrips != null && allBusinessTrips.Any())
                {
                    var allChangeBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => !x.IsCancel);
                    var allCancelBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => x.IsCancel);
                    var allRooms = await _uow.GetRepository<RoomOrganization>().GetAllAsync(string.Empty, x => x.RoomUserMappings);
                    var allRoomTypes = await _uow.GetRepository<RoomType>().GetAllAsync();
                    allBusinessTrips = allBusinessTrips.Where(x => !allCancelBTAItems.Any(y => y.BusinessTripApplicationDetailId == x.Id)).ToList();
                    if (allChangeBTAItems.Any())
                    {
                        foreach (var item in allBusinessTrips)
                        {
                            var changedItem = allChangeBTAItems.FirstOrDefault(x => x.BusinessTripApplicationDetailId == item.Id);
                            if (changedItem != null)
                            {
                                item.FromDate = changedItem.FromDate;
                                item.FromDate = changedItem.ToDate;
                                item.ArrivalCode = changedItem.DestinationCode;
                                item.ArrivalName = changedItem.DestinationName;
                                item.HotelCode = changedItem.NewHotelCode;
                                item.HotelName = changedItem.NewHotelName;
                                item.CheckInHotelDate = changedItem.NewCheckInHotelDate;
                                item.CheckOutHotelDate = changedItem.NewCheckOutHotelDate;
                                item.AirlineCode = changedItem.NewAirlineCode;
                                item.AirlineName = changedItem.NewAirlineName;
                                item.ComebackAirlineCode = changedItem.NewComebackAirlineCode;
                                item.ComebackAirlineName = changedItem.NewComebackAirlineName;
                                item.FlightNumberCode = changedItem.NewFlightNumberCode;
                                item.FlightNumberName = changedItem.NewFlightNumberName;
                                item.ComebackFlightNumberCode = changedItem.NewComebackFlightNumberCode;
                                item.ComebackFlightNumberName = changedItem.NewComebackFlightNumberName;
                            }
                        }
                    }
                    //code moi
                    if (type == ReportType.Flight)
                    {
                        string jsonObj = JsonConvert.SerializeObject(allBusinessTrips);
                        List<BtaDetailViewModel> list = new List<BtaDetailViewModel>();
                        var list1 = JsonConvert.DeserializeObject<List<BtaDetailViewModel>>(jsonObj);
                        var list2 = JsonConvert.DeserializeObject<List<BtaDetailViewModel>>(jsonObj);
                        var allBusinessesCloneComeBackFlightNumbers = list1.Where(x => !string.IsNullOrEmpty(x.ComebackFlightNumberCode)).OrderBy(x => x.ComebackFlightNumberName);
                        foreach (var item in allBusinessesCloneComeBackFlightNumbers)
                        {
                            item.FlightNumberCode = item.ComebackFlightNumberCode;
                            item.FlightNumberName = item.ComebackFlightNumberName;
                            list.Add(item);
                        }
                        var allBusinessesCloneFlightNumbers = list2.Where(x => !string.IsNullOrEmpty(x.FlightNumberCode)).OrderBy(x => x.FlightNumberName);
                        foreach (var x in allBusinessesCloneFlightNumbers)
                        {
                            list.Add(x);
                        }
                        allBusinessTrips = list;
                    }
                    //
                    allBusinessTrips = allBusinessTrips.Select(x => x.Trim()).AsQueryable().Where(args.Predicate, args.PredicateParameters);
                    if (allBusinessTrips.Any())
                    {

                        if (type == ReportType.Hotel)
                        {
                            var groupsHotel = allBusinessTrips.Where(x => !string.IsNullOrEmpty(x.HotelCode) && x.CheckInHotelDate.HasValue && x.CheckOutHotelDate.HasValue).GroupBy(x => new { x.HotelCode, x.HotelName });
                            var allHotels = await _uow.GetRepository<Hotel>().GetAllAsync<HotelViewModel>();
                            count = groupsHotel.Count();
                            groupsHotel = groupsHotel.Skip((args.Page - 1) * args.Limit).Take(args.Limit);
                            if (groupsHotel.Any())
                            {
                                foreach (var group in groupsHotel)
                                {
                                    var Ids = group.Select(x => x.Id).ToList();
                                    var changeIds = allChangeBTAItems.Where(x => Ids.Contains(x.BusinessTripApplicationDetailId)).Select(t => t.Id).ToList();
                                    //var allRoomInHotels = allRooms.Where(x => x.RoomUserMappings.Any(y => group.Any(z => z.Id == y.BusinessTripApplicationDetailId || (changeIds.Any() && y.ChangeCancelBusinessTripApplicationDetailId.HasValue && changeIds.Contains(y.ChangeCancelBusinessTripApplicationDetailId.Value)))));

                                    var allRoomInHotels = allRooms.Where(x => (x.BusinessTripApplication.Status.Trim().Contains("Completed") || x.BusinessTripApplication.Status.Trim().Contains("Completed Changing")) && x.RoomUserMappings.Any(y => group.Any(z => z.Id == y.BusinessTripApplicationDetailId)));
                                    if (allRoomInHotels.Any())
                                    {
                                        var currentHotel = GetHotelByCode(group.Key.HotelCode, allHotels);
                                        //var totalDays = group.Sum(x => (x.CheckOutHotelDate.Value - x.CheckInHotelDate.Value).TotalDays);
                                        var currentHotelData = new ReportForHotelViewModel
                                        {
                                            Name = group.Key.HotelName,
                                            Code = group.Key.HotelCode,
                                            Address = currentHotel?.Address,
                                            TelePhone = currentHotel?.Telephone,
                                            //TotalDays = GetTotalDateFromDoubleDate(totalDays),
                                            //TotalRooms = allRoomInHotels.Count()
                                        };
                                        if (allRoomInHotels.Any())
                                        {
                                            //var temp = result;
                                            ////var temp = new List<ReportDetailUserInRoomViewModel>();
                                            //var itemChangedRoom = temp.Cast<ReportDetailUserInRoomViewModel>().Where(x => x.IsChangeRoom).ToList();
                                            //var _itemChangedRoom = temp.Cast<ReportDetailUserInRoomViewModel>().Where(x => !itemChangedRoom.Any(y => y.Id == x.Id)).ToList();
                                            //foreach (var item in itemChangedRoom)
                                            //{
                                            //    var trackItemChange = result.Where(x => x.TrackId == item.TrackId).FirstOrDefault();
                                            //    if (trackItemChange != null)
                                            //    {
                                            //        temp.Remove(trackItemChange);
                                            //    }
                                            //}
                                            var groupByRoomTypes = allRoomInHotels.Where(x => !x.RoomUserMappings.Any(i => i.IsChange && x.Id == i.RoomOrganizationId)).GroupBy(x => new { x.RoomTypeCode, x.RoomTypeName });
                                            int totalRooms = 0;
                                            if (groupByRoomTypes.Any())
                                            {
                                                foreach (var groupRoom in groupByRoomTypes)
                                                {
                                                    var sapCodeInRooms = new List<Guid?>();
                                                    foreach (var mapping in groupRoom.ToList())
                                                    {
                                                        //var sapCodes = mapping.RoomUserMappings.Where(x => Ids.Contains(x.BusinessTripApplicationDetailId.Value)).Select(y => y.BusinessTripApplicationDetailId);
                                                        //sapCodeInRooms.AddRange(sapCodes);
                                                        //code moi
                                                        var filterUserRooms = mapping.RoomUserMappings.Where(x => Ids.Contains(x.BusinessTripApplicationDetailId.Value)).ToList();
                                                        foreach (var x in filterUserRooms)
                                                        {

                                                            var itemHaveChange = allChangeBTAItems.FirstOrDefault(y => y.BusinessTripApplicationDetailId == x.BusinessTripApplicationDetailId && y.UserId == x.UserId);
                                                            if (itemHaveChange != null)
                                                            {
                                                                var currentRoomUserMapping = mapping.RoomUserMappings.FirstOrDefault(y => y.BusinessTripApplicationDetailId == x.BusinessTripApplicationDetailId && y.IsChange);
                                                                if (currentRoomUserMapping != null)
                                                                {
                                                                    sapCodeInRooms.Add(x.BusinessTripApplicationDetailId);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                sapCodeInRooms.Add(x.BusinessTripApplicationDetailId);
                                                            }
                                                        }
                                                    }
                                                    try
                                                    {
                                                        var totalDaysInRoom = group.Where(x => sapCodeInRooms.Contains(x.Id)).Sum(y => GetTotalDateFromDoubleDate((y.CheckOutHotelDate.Value - y.CheckInHotelDate.Value).TotalDays));
                                                        if (sapCodeInRooms.Any())
                                                        {
                                                            var reportForHotelDetailItem = new ReportForHotelDetailViewModel
                                                            {
                                                                RoomTypeName = groupRoom.Key.RoomTypeName,
                                                                RoomTypeCode = groupRoom.Key.RoomTypeCode,
                                                                TotalDays = totalDaysInRoom,
                                                                TotalStaffs = group.Where(x => sapCodeInRooms.Contains(x.Id)).Count()
                                                            };
                                                            totalRooms += GetTotalRoomsFromStaff(reportForHotelDetailItem.TotalStaffs, groupRoom.Key.RoomTypeCode, allRoomTypes);
                                                            currentHotelData.Items.Add(reportForHotelDetailItem);
                                                        }

                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }


                                                }
                                            }
                                            currentHotelData.TotalDays = currentHotelData.Items.Sum(x => x.TotalDays);
                                            currentHotelData.TotalRooms = totalRooms;
                                            result.Add(currentHotelData);
                                        }
                                    }


                                }
                            }
                        }
                        else if (type == ReportType.Flight)
                        {
                            var groupsFlightNumber = allBusinessTrips.Where(rd => !string.IsNullOrEmpty(rd.FlightNumberCode)).GroupBy(x => new { x.FlightNumberCode, x.FlightNumberName });
                            if (groupsFlightNumber.Any())
                            {
                                count = groupsFlightNumber.Count();
                                //groupsFlightNumber = groupsFlightNumber.Skip((args.Page - 1) * args.Limit);
                                groupsFlightNumber = groupsFlightNumber.Skip((args.Page - 1) * args.Limit).Take(args.Limit);
                                foreach (var group in groupsFlightNumber)
                                {
                                    //var staffs = group.GroupBy(x => x.SAPCode).ToList();
                                    //var currentHotelData = new ReportForFightNumberViewModel
                                    //{
                                    //    FlightNumber = group.Key.FlightNumberName,
                                    //    FlightNumberCode = group.Key.FlightNumberCode,
                                    //    TotalStaffs = staffs.Count()
                                    //};
                                    //result.Add(currentHotelData);
                                    //code moi 09/10
                                    var arrayTemporary = new List<BtaDetailViewModel>();
                                    foreach (var x in group)
                                    {
                                        arrayTemporary.Add(x);
                                    }
                                    var currentHotelData = new ReportForFightNumberViewModel
                                    {
                                        FlightNumber = group.Key.FlightNumberName,
                                        FlightNumberCode = group.Key.FlightNumberCode,
                                        TotalStaffs = arrayTemporary.Count()
                                    };
                                    result.Add(currentHotelData);
                                    //

                                }
                            }
                        }

                    }
                }
            }
            else if (type == ReportType.Status)
            {
                var allItems = await _uow.GetRepository<BusinessTripApplication>().GetAllAsync();
                if (allItems.Any())
                {
                    foreach (var x in allItems)
                    {
                        if (await CheckIsChangingCancellingItem(x.Id))
                        {
                            x.Status = "Changing/ Cancelling Business Trip";
                        }
                        else if (x.Status.Contains("Waiting"))
                        {
                            var isRevoking = await CheckIsRevokingItem(x.Id);
                            if (isRevoking)
                            {
                                x.Status = "Revoking";
                            }
                            else
                            {
                                x.Status = "In Progress";
                            }
                        }
                    }
                    var groupsStatus = allItems.AsQueryable().Where(args.Predicate, args.PredicateParameters).GroupBy(g => g.Status);
                    if (groupsStatus.Any())
                    {
                        count = groupsStatus.Count();
                        groupsStatus = groupsStatus.Skip((args.Page - 1) * args.Limit);
                        foreach (var group in groupsStatus)
                        {
                            var currentHotelData = new ReportForStatusViewModel
                            {
                                Status = group.Key,
                                TotalRequest = group.Count()
                            };
                            result.Add(currentHotelData);
                        }
                    }
                }
            }
            //CR - Flights booking
            else if (type == ReportType.FlightsBooking)
            {
                var allBookingFlight = await _uow.GetRepository<BookingFlight>().FindByAsync("Modified desc", args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                count = await _uow.GetRepository<BookingFlight>().CountAsync(args.Predicate, args.PredicateParameters);
                if (allBookingFlight.Any())
                {
                    foreach (var item in allBookingFlight)
                    {
                        var dataTimeFlightString = "";
                        var bookingFlightInfors = "";
                        var bookingFlightsViewModel = Mapper.Map<BookingFlightViewModel>(item);

                        if (bookingFlightsViewModel.FlightDetail.PricedItineraries != null)
                        {
                            var pricedItineraries = bookingFlightsViewModel.FlightDetail.PricedItineraries;

                            var dataFlight = pricedItineraries.OriginDestinationOptions;
                            if (dataFlight.Count > 0)
                            {
                                var originDateTimeHour = dataFlight[0].OriginDateTime.Hour < 10 ? $"0{dataFlight[0].OriginDateTime.Hour}" : dataFlight[0].OriginDateTime.Hour.ToString();
                                var originDateTimeMinute = dataFlight[0].OriginDateTime.Minute < 10 ? $"0{dataFlight[0].OriginDateTime.Minute}" : dataFlight[0].OriginDateTime.Minute.ToString();
                                var destinationDateTimeHour = dataFlight[0].DestinationDateTime.Hour < 10 ? $"0{dataFlight[0].DestinationDateTime.Hour}" : dataFlight[0].DestinationDateTime.Hour.ToString();
                                var destinationDateTimeMinute = dataFlight[0].DestinationDateTime.Minute < 10 ? $"0{dataFlight[0].DestinationDateTime.Minute}" : dataFlight[0].DestinationDateTime.Minute.ToString();

                                dataTimeFlightString = $"{originDateTimeHour}:{originDateTimeMinute} - {destinationDateTimeHour}:{destinationDateTimeMinute}";
                            }

                            bookingFlightInfors = $"{bookingFlightsViewModel.FlightDetail.FlightNo} {bookingFlightsViewModel.FlightDetail.OriginLocationCode}-{bookingFlightsViewModel.FlightDetail.DestinationLocationCode} {dataTimeFlightString}";

                            var reasonToCancel = "";

                            if (item.IsCancel)
                            {
                                var changeCancelDetail = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().GetSingleAsync<ChangeCancelBusinessTripDetailViewModel>(x => x.BusinessTripApplicationDetailId == item.BTADetailId);

                                if (changeCancelDetail != null)
                                {
                                    if (!item.DirectFlight)
                                    {
                                        reasonToCancel = changeCancelDetail.ReasonForInBoundFlight;
                                    }
                                    else
                                    {
                                        reasonToCancel = changeCancelDetail.ReasonForOutBoundFlight;
                                    }
                                }
                            }
                            //var countUsers = await CountUsersOnSameFlight(bookingFlightsViewModel);
                            var bookingflight = new ReportForBookingFlightViewModel
                            {
                                Status = item.Status,
                                SapCode = item.BTADetail.SAPCode,
                                FullName = item.BTADetail.FullName,
                                BookingDate = item.Created.Date,
                                FlightInformation = bookingFlightInfors,
                                Cost = pricedItineraries.AirItineraryPricingInfo.AdultFare.passengerFare.baseFare.Amount + pricedItineraries.AirItineraryPricingInfo.AdultFare.passengerFare.serviceTax.Amount,
                                ReasonforBusinessTrip = item.BTADetail.BusinessTripApplication.RequestorNote,
                                ReasonforBusinessTripDetail = !string.IsNullOrEmpty(item.BTADetail.BusinessTripApplication.RequestorNoteDetail) ? item.BTADetail.BusinessTripApplication.RequestorNoteDetail : "",
                                BTAStatus = item.BTADetail.BusinessTripApplication.Status,
                                ReasonforCancel = reasonToCancel,
                                CancellationFee = item.IsCancel ? item.PenaltyFee.ToString() : "",
                                ServiceFee = bookingFlightsViewModel.FlightDetail.IncludeEquivfare,
                                BTAId = item.BTADetail.BusinessTripApplicationId,
                                ReferenceNumber = item.BTADetail.BusinessTripApplication.ReferenceNumber,
                                DeptName = item.BTADetail.BusinessTripApplication.DeptName
                            };
                            result.Add(bookingflight);
                        }
                    }
                }
            }
            else if (type == ReportType.OverBudget)
            {

                var allOverBudget = await _uow.GetRepository<BusinessTripOverBudget>().FindByAsync("Modified desc", args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                count = await _uow.GetRepository<BusinessTripOverBudget>().CountAsync(args.Predicate, args.PredicateParameters);
                if (allOverBudget.Any())
                {
                    foreach (var item in allOverBudget)
                    {

                        var overBudgetsViewModel = Mapper.Map<BTAOverBudgetViewModel>(item);
                        var btaParent = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(overBudgetsViewModel.BusinessTripApplicationId.Value);
                        var allOverDetails = await _uow.GetRepository<BusinessTripOverBudgetsDetail>().FindByAsync(x => x.BusinessTripOverBudgetId == overBudgetsViewModel.Id);
                        foreach (var detail in allOverDetails)
                        {
                            var overBudget = new ReportBTAOverBudgetViewModel
                            {
                                Status = item.Status,
                                SapCode = detail.SAPCode,
                                FullName = detail.FullName,
                                OverBudgetDate = item.Created.Date,
                                BTAStatus = btaParent.Status,
                                BTAId = btaParent.Id,
                                ReferenceNumber = item.ReferenceNumber,
                                DeptName = btaParent.DeptName,
                                DeptDivisionName = detail.DepartmentName,
                                BTA_ReferenceNumber = btaParent.ReferenceNumber,
                                Id = item.Id,
                                OldBudget = btaParent.IsRoundTrip ? detail.MaxBudgetAmount : detail.MaxBudgetAmount / 2,
                                NewBudget = detail.ExtraBudget,
                                DepartureName = detail.DepartureName,
                                ArrivalName = detail.ArrivalName,
                                FromDate = detail.FromDate,
                                ToDate = detail.FromDate
                            };
                            result.Add(overBudget);
                        }

                    }
                }
            }
            return new ResultDTO { Object = new ArrayResultDTO { Data = result, Count = count } };
        }
        public async Task<ResultDTO> GetRevokingBTA(ViewModels.Args.RevokingArg args)
        {
            var wfInstances = await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => !x.IsCompleted && x.WorkflowName.Contains("BTA Revoke"));
            var tempItems = new List<ReportForStatusViewModel>();
            var result = new List<ReportForStatusViewModel>();
            int count = 0;
            bool isNotContainProgress = false;
            var ChangeCancel = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().GetAllAsync();
            //(args.Statuses.Contains(RuleValidate.Changing) && ChangeCancel.Any(s => s.BusinessTripApplicationDetailId == x.Id)
            //args.Statuses.Contains(x.Status)
            var allItems = await _uow.GetRepository<BusinessTripApplication>().GetAllAsync<BTAViewModel>();
            if (!args.Statuses.Any(x => x.ToString() == "In Progress"))
            {
                isNotContainProgress = true;
            }
            else
            {
                isNotContainProgress = false;

            }
            var ruleValidateInprogress = new string[] { "Draft", "Cancelled", "Rejected", "Completed", "Requested To Change", "Completed Changing", "Pending" };
            var inProgressItems = await _uow.GetRepository<BusinessTripApplication>().FindByAsync<BTAViewModel>(x => !ruleValidateInprogress.Contains(x.Status));
            if (inProgressItems.Any())
            {
                allItems = allItems.Concat(inProgressItems);
            }
            if (allItems.Any())
            {
                if (args.FromDate.HasValue)
                {
                    allItems = allItems.Where(x => x.Created > args.FromDate);
                }
                if (args.ToDate.HasValue)
                {
                    allItems = allItems.Where(x => x.Created < args.ToDate.Value.AddDays(1));
                }
                foreach (var item in allItems)
                {
                    var isChangeCancel = _uow.GetRepository<ChangeCancelBusinessTripDetail>().Any(i => i.BusinessTripApplicationId == item.Id);

                    if (wfInstances.Any(x => x.ItemId == item.Id) && item.Status.Contains("Waiting") && !ruleValidateInprogress.Contains(item.Status))
                    {
                        tempItems.Add(new ReportForStatusViewModel
                        {
                            Status = "Revoking"
                        });
                    }
                    else if (isChangeCancel)
                    {
                        tempItems.Add(new ReportForStatusViewModel
                        {
                            Status = "Changing/ Cancelling Business Trip"
                        });
                    }
                    else
                    {
                        if (item.Status.Contains("Waiting"))
                        {
                            tempItems.Add(new ReportForStatusViewModel
                            {
                                Status = "In Progress"
                            });
                        }
                        else
                        {
                            tempItems.Add(new ReportForStatusViewModel
                            {
                                Status = item.Status
                            });
                        }

                    }
                }
                if (tempItems.Count > 0)
                {
                    if (isNotContainProgress)
                    {
                        tempItems = tempItems.Where(x => x.Status != "In Progress").ToList();
                    }
                    var groups = tempItems.Where(x => args.Statuses.Contains(x.Status)).GroupBy(x => x.Status);
                    count = groups.Count();
                    foreach (var group in groups)
                    {
                        result.Add(new ReportForStatusViewModel
                        {
                            Status = group.Key,
                            TotalRequest = group.Count()
                        });
                    }

                }

            }
            return new ResultDTO { Object = new ArrayResultDTO { Data = result, Count = count } };
        }
        private async Task<bool> CheckIsRevokingItem(Guid itemId)
        {
            var isRevoking = false;
            try
            {
                var currentWfs = (await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.ItemId == itemId && !x.IsCompleted && x.WorkflowName.Contains("BTA Revoke"))).OrderByDescending(x => x.Created);
                if (currentWfs.Any())
                {
                    var lastWfs = currentWfs.FirstOrDefault();
                    if (!lastWfs.IsCompleted)
                    {
                        isRevoking = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error at CheckIsRevokingItem, error Message is {ex.Message}: {itemId}");
                return false;
            }
            return isRevoking;
        }
        private async Task<bool> CheckIsChangingCancellingItem(Guid itemId)
        {
            var isChangingCancelling = false;
            var currentWfs = (await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => !x.IsCompleted && x.WorkflowName.Contains("BTA Changing") && x.ItemId == itemId)).OrderByDescending(x => x.Created)?.FirstOrDefault();
            if (currentWfs != null)
            {
                var lastWfs = currentWfs.Histories.FirstOrDefault(o => o.Outcome == "Changed Business Trip");
                if (lastWfs != null)
                {
                    isChangingCancelling = true;
                }
            }
            return isChangingCancelling;
        }
        private HotelViewModel GetHotelByCode(string code, IEnumerable<HotelViewModel> hotels)
        {
            HotelViewModel hotel = null;
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var currentRoom = hotels.FirstOrDefault(x => x.Code == code);
                    if (currentRoom != null)
                    {
                        hotel = currentRoom;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetRoomFromStaffNumbers " + ex.Message);
            }
            return hotel;
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật phiếu BTA 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ResultDTO> Save(BusinessTripDTO data)
        {
            var result = new ResultDTO();
            if (data == null)
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            else if (!data.Id.HasValue)
            {
                string maxGrade = data.MaxGrade;
                if (!string.IsNullOrEmpty(data.BusinessTripDetails))
                {
                    var maxGradeFromDetails = await GetMaxGradeFromDetails(data.BusinessTripDetails);
                    maxGrade = !string.IsNullOrEmpty(maxGradeFromDetails) ? string.Format($"G{maxGradeFromDetails}") : data.MaxGrade;
                }
                var record = new BusinessTripApplication
                {
                    DeptLineId = data.DeptLineId,
                    DeptCode = data.DeptCode,
                    DeptName = data.DeptName,
                    DeptDivisionId = data.DeptDivisionId,
                    DeptDivisionCode = data.DeptDivisionCode,
                    DeptDivisionName = data.DeptDivisionName,
                    UserSAPCode = data.UserSAPCode,
                    UserFullName = data.UserCreatedFullName,
                    MaxGrade = maxGrade,
                    // MaxGrade = data.MaxGrade,
                    IsStore = data.IsStore,
                    MaxDepartmentId = data.MaxDepartmentId,
                    RequestorNote = data.RequestorNote,
                    RequestorNoteDetail = data.RequestorNoteDetail,
                    CarRental = data.CarRental,
                    DocumentDetails = data.DocumentDetails,
                    Type = data.Type,
                    DocumentChanges = data.DocumentChanges,
                    IsRoundTrip = data.IsRoundTrip,
                    StayHotel = data.StayHotel,
                    BookingContact = data.BookingContact,
                    CarRentalAttachmentDetails = data.CarRentalAttachmentDetails,
                    VisaAttachmentDetails = data.VisaAttachmentDetails
                };
                _uow.GetRepository<BusinessTripApplication>().Add(record);
                await _uow.CommitAsync();
                if (!string.IsNullOrEmpty(data.BusinessTripDetails))
                {
                    AddBusinessTripDetail(data.BusinessTripDetails, record);
                }
                result.Object = record;
            }
            else
            {
                string maxGrade = data.MaxGrade;
                if (!string.IsNullOrEmpty(data.BusinessTripDetails))
                {
                    var maxGradeFromDetails = await GetMaxGradeFromDetails(data.BusinessTripDetails);
                    maxGrade = !string.IsNullOrEmpty(maxGradeFromDetails) ? string.Format($"G{maxGradeFromDetails}") : data.MaxGrade;
                }
                var record = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(data.Id.Value);
                if (record != null)
                {
                    record.DeptLineId = data.DeptLineId;
                    record.DeptCode = data.DeptCode;
                    record.DeptName = data.DeptName;
                    record.DeptDivisionId = data.DeptDivisionId;
                    record.DeptDivisionCode = data.DeptDivisionCode;
                    record.DeptDivisionName = data.DeptDivisionName;
                    //record.MaxGrade = data.MaxGrade;
                    record.MaxGrade = maxGrade;
                    record.IsStore = data.IsStore;
                    record.Type = data.Type;
                    record.MaxDepartmentId = data.MaxDepartmentId;
                    record.DocumentDetails = data.DocumentDetails;
                    record.DocumentChanges = data.DocumentChanges;
                    record.RequestorNote = data.RequestorNote;
                    record.RequestorNoteDetail = data.RequestorNoteDetail;
                    record.CarRental = data.CarRental;
                    record.IsRoundTrip = data.IsRoundTrip;
                    record.StayHotel = data.StayHotel;
                    record.BookingContact = data.BookingContact;
                    record.CarRentalAttachmentDetails = data.CarRentalAttachmentDetails;
                    record.VisaAttachmentDetails = data.VisaAttachmentDetails;
                    if (record.Status == "Draft")
                    {
                        var existChangeCancelDetails = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync(x => x.BusinessTripApplicationId == data.Id);
                        if (existChangeCancelDetails != null && existChangeCancelDetails.Any())
                        {
                            _uow.GetRepository<ChangeCancelBusinessTripDetail>().Delete(existChangeCancelDetails);
                        }
                        if (!string.IsNullOrEmpty(data.BusinessTripDetails))
                        {
                            AddBusinessTripDetail(data.BusinessTripDetails, record);
                        }
                    }
                    else
                    {
                        var existBusinessTripDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync(x => x.BusinessTripApplicationId == data.Id);

                        var businesstripDetailTemp = JsonConvert.DeserializeObject<List<BusinessTripApplicationDetail>>(data.BusinessTripDetails);
                        var detailTempIds = businesstripDetailTemp.Select(x => x.Id);
                        var updatedItems = new List<BusinessTripApplicationDetail>();
                        var addItems = new List<BusinessTripApplicationDetail>();
                        var deletedItems = existBusinessTripDetails.Where(x => !detailTempIds.Contains(x.Id)).Select(x => x.Id);
                        if (businesstripDetailTemp != null && businesstripDetailTemp.Any())
                        {
                            foreach (var item in businesstripDetailTemp)
                            {
                                var bta_DetailItem = existBusinessTripDetails.FirstOrDefault(x => x.Id == item.Id);

                                if (bta_DetailItem != null && bta_DetailItem.Id != null)
                                {
                                    List<string> ignoreProperties = new List<string>() { "Id", "BusinessTripApplication", "Department", "User", "TripGroup", "Comments" };
                                    if (!string.IsNullOrEmpty(bta_DetailItem.Comments))
                                    {
                                        ignoreProperties.Remove("Comments");
                                    }
                                    if (bta_DetailItem.TripGroup > 0)
                                    {
                                        ignoreProperties.Remove("TripGroup");
                                    }
                                    bta_DetailItem = item.TransformValues(bta_DetailItem, ignoreProperties);
                                    bta_DetailItem.BusinessTripApplicationId = data.Id.Value;
                                    bta_DetailItem.PartitionId = item.PartitionId;
                                    updatedItems.Add(bta_DetailItem);
                                }
                                else
                                {
                                    bta_DetailItem = item.TransformValues(item, new List<string> { "Id", "BusinessTripApplication", "Department", "User", "TripGroup", "Comments" });
                                    bta_DetailItem.BusinessTripApplicationId = data.Id.Value;
                                    bta_DetailItem.PartitionId = item.PartitionId;
                                    if (bta_DetailItem.RememberInformation)
                                    {
                                        bta_DetailItem.SavePassengerInformation(_uow);
                                    }
                                    addItems.Add(bta_DetailItem);
                                }
                            }
                            if (updatedItems.Any())
                            {
                                _uow.GetRepository<BusinessTripApplicationDetail>().Update(updatedItems);
                            }
                            if (addItems.Any())
                            {
                                _uow.GetRepository<BusinessTripApplicationDetail>().Add(addItems);
                            }
                            if (deletedItems.Any())
                            {
                                foreach (var deleteItem in deletedItems)
                                {
                                    var deletedRecord = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync(x => deletedItems.Contains(x.Id));
                                    _uow.GetRepository<BusinessTripApplicationDetail>().Delete(deletedRecord);
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(data.ChangeCancelBusinessTripDetails))
                    {
                        if (record.Status == "Completed" || record.Status == "Completed Changing")
                        {
                            var existBusinessTripDetails = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync(x => x.BusinessTripApplicationId == data.Id);
                            if (existBusinessTripDetails != null && existBusinessTripDetails.Any())
                            {
                                _uow.GetRepository<ChangeCancelBusinessTripDetail>().Delete(existBusinessTripDetails);
                            }
                            AddChangeCancelBusinessTripDetail(data.ChangeCancelBusinessTripDetails, record.Id);
                        }
                        else
                        {
                            var existChangeBusinessTripDetails = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => x.BusinessTripApplicationId == data.Id);
                            var changeBusinesstripDetailTemp = JsonConvert.DeserializeObject<List<ChangeCancelBusinessTripDTO>>(data.ChangeCancelBusinessTripDetails);
                            if (changeBusinesstripDetailTemp != null && changeBusinesstripDetailTemp.Any())
                            {
                                var updatedItems = new List<ChangeCancelBusinessTripDetail>();
                                var addItems = new List<ChangeCancelBusinessTripDetail>();
                                foreach (var item in changeBusinesstripDetailTemp)
                                {
                                    var rd = Mapper.Map<ChangeCancelBusinessTripDetail>(item);
                                    rd.BusinessTripApplicationId = record.Id;
                                    if (rd.Id != Guid.Empty)
                                    {
                                        updatedItems.Add(rd);
                                    }
                                    else
                                    {
                                        addItems.Add(rd);
                                    }
                                }
                                if (addItems.Any())
                                {
                                    _uow.GetRepository<ChangeCancelBusinessTripDetail>().Add(addItems);
                                }
                                if (updatedItems.Any())
                                {
                                    _uow.GetRepository<ChangeCancelBusinessTripDetail>().Update(updatedItems);
                                }
                            }
                        }

                    }
                    result.Object = Mapper.Map<BTAViewModel>(record);
                }
                else
                {
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "No Data" };
                }
            }
            await _uow.CommitAsync();

            return result;
        }
        private async Task<string> GetMaxGradeFromDetails(string btaDetails)
        {
            string maxGrade = string.Empty;
            try
            {
                var businessTripDetails = JsonConvert.DeserializeObject<List<BusinessTripDetailDTO>>(btaDetails);
                if (businessTripDetails != null && businessTripDetails.Any())
                {
                    var departmentIds = businessTripDetails.Select(x => x.DepartmentId).ToList();
                    var departments = await _uow.GetRepository<Department>().FindByAsync(x => departmentIds.Contains(x.Id));
                    maxGrade = departments.Max(x => x.JobGrade.Grade).ToString();
                }
            } catch (Exception ex)
            {
                logger.LogError("Error at GetMaxGradeFromDetails " + ex.Message);
            }
            return maxGrade;
        }
        public async Task<ResultDTO> SaveRevokeInfo(RevokeBTAInfoViewModel data)
        {
            var result = new ResultDTO();
            if (data == null || data.BusinessTripApplicationId == Guid.Empty || data.ChangeCancelBusinessTripDetails.Count == 0)
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            else
            {
                var record = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(data.BusinessTripApplicationId);

                if (record != null)
                {
                    if (data.ChangeCancelBusinessTripDetails.Count > 0)
                    {

                        var existBusinessTripDetails = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync(x => x.BusinessTripApplicationId == data.BusinessTripApplicationId);
                        if (existBusinessTripDetails != null && existBusinessTripDetails.Any())
                        {
                            _uow.GetRepository<ChangeCancelBusinessTripDetail>().Delete(existBusinessTripDetails);
                        }
                        AddChangeCancelBusinessTripDetail(data.ChangeCancelBusinessTripDetails, record.Id);
                        result.Object = Mapper.Map<BTAViewModel>(record);

                    }
                    else
                    {
                        result.ErrorCodes = new List<int> { 1004 };
                        result.Messages = new List<string> { "No Data" };
                    }
                }
                else
                {
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "No Data" };
                }
            }
            _uow.Commit();

            return result;
        }
        public async Task<ResultDTO> SaveCancellationFee_Revoke(RevokeBTAInfoViewModel data)
        {
            var result = new ResultDTO();
            try
            {
                if (data == null || data.BusinessTripApplicationId == Guid.Empty || data.ChangeCancelBusinessTripDetails.Count == 0)
                {
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "No Data" };
                }
                else
                {
                    //Update Booking Flight
                    foreach (var itemCancel in data.ChangeCancelBusinessTripDetails)
                    {
                        var cBookingFlights = await _uow.GetRepository<BookingFlight>().FindByAsync(x => x.BTADetailId == itemCancel.BusinessTripApplicationDetailId);
                        if (cBookingFlights.Any())
                        {
                            var dataUpdate = cBookingFlights.Select(cItem =>
                            {
                                cItem.PenaltyFee = itemCancel.PenaltyFee;
                                cItem.IsCancel = true;
                                cItem.Status = "Cancelled";
                                return cItem;
                            }).ToList();

                            _uow.GetRepository<BookingFlight>().Update(dataUpdate);
                        }
                        else
                        {
                            result.ErrorCodes = new List<int> { 1004 };
                            result.Messages = new List<string> { "No Data" };
                        }
                    }
                    //Update ChangeCancelFlight
                    UpdateChangeCancelBusinessTripDetail(data.ChangeCancelBusinessTripDetails, data.BusinessTripApplicationId);

                    //return object to reload page
                    var record = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(data.BusinessTripApplicationId);
                    if (record != null)
                    {
                        result.Object = Mapper.Map<BTAViewModel>(record);
                    }

                }
                _uow.Commit();
            }
            catch (Exception ex)
            {
                ex.LogError(logger, "SaveCancellationFee_Revoke");
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "Save data failed." };
            }
            return result;
        }
        public async Task<ResultDTO> SaveCancellationFee_Changing(RevokeBTAInfoViewModel data)
        {
            var result = new ResultDTO();
            try
            {
                if (data == null || data.BusinessTripApplicationId == Guid.Empty || data.ChangeCancelBusinessTripDetails.Count == 0)
                {
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "No Data" };
                }
                else
                {
                    //Update Booking Flight
                    foreach (var itemCancel in data.ChangeCancelBusinessTripDetails)
                    {
                        if (itemCancel.IsCancel || itemCancel.IsCancelInBoundFlight || itemCancel.IsCancelOutBoundFlight)
                        {
                            var cBookingFlights = await _uow.GetRepository<BookingFlight>().FindByAsync(x => x.BTADetailId == itemCancel.BusinessTripApplicationDetailId);
                            if (cBookingFlights.Any())
                            {
                                if (itemCancel.IsCancelInBoundFlight)
                                {
                                    var dataUpdate = cBookingFlights.Where(x => !x.DirectFlight).FirstOrDefault();
                                    if (dataUpdate != null)
                                    {
                                        if (!string.IsNullOrEmpty(itemCancel.CancellationFeeObj))
                                        {
                                            var cancellationFee = JsonConvert.DeserializeObject<CancellationFeeObject>(itemCancel.CancellationFeeObj);
                                            dataUpdate.PenaltyFee = cancellationFee.InBound;
                                        }
                                        dataUpdate.IsCancel = itemCancel.IsCancelInBoundFlight;
                                        dataUpdate.Status = "Cancelled";
                                        _uow.GetRepository<BookingFlight>().Update(dataUpdate);
                                    }
                                }
                                else if (itemCancel.IsCancelOutBoundFlight)
                                {
                                    var dataUpdate = cBookingFlights.Where(x => x.DirectFlight).FirstOrDefault();
                                    if (dataUpdate != null)
                                    {
                                        if (!string.IsNullOrEmpty(itemCancel.CancellationFeeObj))
                                        {
                                            var cancellationFee = JsonConvert.DeserializeObject<CancellationFeeObject>(itemCancel.CancellationFeeObj);
                                            dataUpdate.PenaltyFee = cancellationFee.OutBound;
                                        }
                                        dataUpdate.IsCancel = itemCancel.IsCancelOutBoundFlight;
                                        dataUpdate.Status = "Cancelled";
                                        _uow.GetRepository<BookingFlight>().Update(dataUpdate);
                                    }
                                }
                            }
                            else
                            {
                                result.ErrorCodes = new List<int> { 1004 };
                                result.Messages = new List<string> { "No Data" };
                            }
                        }
                    }
                    //Update ChangeCancelFlight
                    UpdateChangeCancelBusinessTripDetail(data.ChangeCancelBusinessTripDetails, data.BusinessTripApplicationId);

                    //return object to reload page
                    var record = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(data.BusinessTripApplicationId);
                    if (record != null)
                    {
                        result.Object = Mapper.Map<BTAViewModel>(record);
                    }

                }
                _uow.Commit();
            }
            catch (Exception ex)
            {
                ex.LogError(logger, "SaveCancellationFee_Revoke");
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "Save data failed." };
            }
            return result;
        }

        public async Task<ResultDTO> SaveRoomOrganization(Guid id, string roomDetails, bool isChange = false)
        {
            if (!string.IsNullOrEmpty(roomDetails))
            {
                if (!isChange)
                {
                    var existRoomOrganizationDetails = await _uow.GetRepository<RoomOrganization>().FindByAsync(x => x.BusinessTripApplicationId == id);
                    if (existRoomOrganizationDetails != null && existRoomOrganizationDetails.Any())
                    {
                        foreach (var room in existRoomOrganizationDetails)
                        {
                            var mappings = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => x.RoomOrganizationId == room.Id);
                            if (mappings != null && mappings.Any())
                            {
                                _uow.GetRepository<RoomUserMapping>().Delete(mappings);
                                _uow.GetRepository<RoomOrganization>().Delete(room);
                            }
                        }
                    }
                }
                else
                {
                    var deleteItemRoomOrganizations = new List<RoomOrganization>();
                    var roomUsers = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => x.RoomOrganization.BusinessTripApplicationId == id && x.IsChange == true);
                    if (roomUsers.Any())
                    {
                        foreach (var item in roomUsers)
                        {
                            var result = deleteItemRoomOrganizations.Find(x => x.Id == item.RoomOrganizationId);
                            if (result == null)
                            {
                                var roomOrganization = new RoomOrganization
                                {
                                    Id = new Guid(item.RoomOrganizationId.ToString())
                                };
                                deleteItemRoomOrganizations.Add(roomOrganization);
                            }
                        }
                        _uow.GetRepository<RoomUserMapping>().Delete(roomUsers);
                    }
                    if (deleteItemRoomOrganizations.Any())
                    {
                        _uow.GetRepository<RoomOrganization>().Delete(deleteItemRoomOrganizations);
                    }
                }
                AddRoomOrganizationDetail(roomDetails, id, isChange);
            }
            await _uow.CommitAsync();
            return new ResultDTO { Object = roomDetails };
        }

        private void AddBusinessTripDetail(string details, BusinessTripApplication bta)
        {
            var businesstripDetailTemp = JsonConvert.DeserializeObject<List<BusinessTripDetailDTO>>(details);

            #region Remove businesstripDetailTemp
            List<Guid> businesstripDetailIds_Client = businesstripDetailTemp.Where(x => x != null && x.Id.HasValue && x.Id.Value != Guid.Empty).Select(x => x.Id.Value).ToList();
            List<Guid> businesstripDetailIds_DB = _uow.GetRepository<BusinessTripApplicationDetail>(true).FindBy(x => x.BusinessTripApplicationId == bta.Id).Select(x => x.Id).ToList();
            List<Guid> removedBTADetailIds = businesstripDetailIds_DB.Except(businesstripDetailIds_Client).ToList();
            if (removedBTADetailIds.Any())
            {
                _uow.GetRepository<BusinessTripApplicationDetail>().Delete(_uow.GetRepository<BusinessTripApplicationDetail>(true).FindBy(x => removedBTADetailIds.Contains(x.Id)));
            }
            #endregion

            #region Add new or update
            if (businesstripDetailTemp != null && businesstripDetailTemp.Any())
            {
                bta.BusinessTripFrom = businesstripDetailTemp.OrderBy(x => x.FromDate).FirstOrDefault().FromDate;
                bta.BusinessTripTo = businesstripDetailTemp.OrderByDescending(x => x.ToDate).FirstOrDefault().ToDate;
                var businessTripDetails = new List<BusinessTripApplicationDetail>();
                foreach (var item in businesstripDetailTemp)
                {
                    BusinessTripApplicationDetail detail = null;
                    if (item.Id.HasValue)
                    {
                        detail = _uow.GetRepository<BusinessTripApplicationDetail>().FindById(item.Id.Value);
                    }
                    else
                    {
                        detail = new BusinessTripApplicationDetail();
                    }

                    detail = item.TransformValues(detail, new List<string> { "Id", "BusinessTripApplication", "Department", "User" });
                    detail.BusinessTripApplicationId = bta.Id;
                    detail.PartitionId = item.PartitionId;
                    if (detail.RememberInformation)
                    {
                        detail.SavePassengerInformation(_uow);
                    }
                    if (!item.Id.HasValue || item.Id == Guid.Empty)
                    {
                        _uow.GetRepository<BusinessTripApplicationDetail>().Add(detail);
                    }
                    else
                    {
                        _uow.GetRepository<BusinessTripApplicationDetail>().Update(detail);
                    }
                }
            };
            #endregion
            _uow.Commit();
        }
        private void AddChangeCancelBusinessTripDetail(string details, Guid businessTripId)
        {
            var changeCancelBusinesstripDetailTemp = JsonConvert.DeserializeObject<List<ChangeCancelBusinessTripDTO>>(details);
            if (changeCancelBusinesstripDetailTemp != null && changeCancelBusinesstripDetailTemp.Any())
            {

                var changeCancelBusinesstripDetails = new List<ChangeCancelBusinessTripDetail>();
                foreach (var item in changeCancelBusinesstripDetailTemp)
                {
                    var detail = Mapper.Map<ChangeCancelBusinessTripDetail>(item);
                    detail.BusinessTripApplicationId = businessTripId;
                    detail.Email = detail.UserId.GetUserById(_uow).Email;
                    changeCancelBusinesstripDetails.Add(detail);
                }
                _uow.GetRepository<ChangeCancelBusinessTripDetail>().Add(changeCancelBusinesstripDetails);
            };
        }
        private void UpdateChangeCancelBusinessTripDetail(List<ChangeCancelBusinessTripDTO> changeCancelBusinesstripDetailTemp, Guid businessTripId)
        {
            if (changeCancelBusinesstripDetailTemp != null && changeCancelBusinesstripDetailTemp.Any())
            {
                var changeCancelBusinesstripDetails = new List<ChangeCancelBusinessTripDetail>();
                foreach (var item in changeCancelBusinesstripDetailTemp)
                {
                    var detail = Mapper.Map<ChangeCancelBusinessTripDetail>(item);
                    detail.BusinessTripApplicationId = businessTripId;
                    changeCancelBusinesstripDetails.Add(detail);
                }
                _uow.GetRepository<ChangeCancelBusinessTripDetail>().Update(changeCancelBusinesstripDetails);
            };
        }
        private void AddChangeCancelBusinessTripDetail(List<ChangeCancelBusinessTripDTO> changeCancelBusinesstripDetailTemp, Guid businessTripId)
        {
            if (changeCancelBusinesstripDetailTemp != null && changeCancelBusinesstripDetailTemp.Any())
            {

                var changeCancelBusinesstripDetails = new List<ChangeCancelBusinessTripDetail>();
                foreach (var item in changeCancelBusinesstripDetailTemp)
                {
                    var detail = Mapper.Map<ChangeCancelBusinessTripDetail>(item);
                    detail.BusinessTripApplicationId = businessTripId;
                    detail.Email = detail.UserId.GetUserById(_uow).Email;
                    changeCancelBusinesstripDetails.Add(detail);
                }
                _uow.GetRepository<ChangeCancelBusinessTripDetail>().Add(changeCancelBusinesstripDetails);
            };
        }
        private void AddRoomOrganizationDetail(string details, Guid businessTripId, bool isChange = false)
        {
            var roomOrganizationTemp = JsonConvert.DeserializeObject<List<RoomOrganizationDTO>>(details);
            if (roomOrganizationTemp != null && roomOrganizationTemp.Any())
            {
                if (isChange)
                {
                    foreach (var item in roomOrganizationTemp)
                    {
                        var detail = new RoomOrganization
                        {
                            RoomTypeId = item.RoomTypeId,
                            RoomTypeCode = item.RoomTypeCode,
                            RoomTypeName = item.RoomTypeName,
                            BusinessTripApplicationId = businessTripId,
                            TripGroup = item.TripGroup
                        };
                        if (item.Id != null)
                        {
                            detail.Id = new Guid(item.Id.ToString());
                        }
                        _uow.GetRepository<RoomOrganization>().Add(detail);
                        var roomUserMappings = new List<RoomUserMapping>();
                        if (item.Users != null && item.Users.Any())
                        {
                            foreach (var user in item.Users)
                            {
                                roomUserMappings.Add(new RoomUserMapping
                                {
                                    RoomOrganizationId = detail.Id,
                                    UserId = user.UserId,
                                    ChangeCancelBusinessTripApplicationDetailId = user.ChangeCancelBusinessTripApplicationDetailId,
                                    BusinessTripApplicationDetailId = user.BusinessTripApplicationDetailId,
                                    IsChange = isChange
                                });
                            }
                            _uow.GetRepository<RoomUserMapping>().Add(roomUserMappings);
                        }
                    }
                }
                else
                {
                    foreach (var item in roomOrganizationTemp)
                    {
                        var detail = new RoomOrganization
                        {
                            RoomTypeId = item.RoomTypeId,
                            RoomTypeCode = item.RoomTypeCode,
                            RoomTypeName = item.RoomTypeName,
                            BusinessTripApplicationId = businessTripId,
                            TripGroup = item.TripGroup
                        };
                        if (item.Id != null)
                        {
                            detail.Id = new Guid(item.Id.ToString());
                        }
                        _uow.GetRepository<RoomOrganization>().Add(detail);
                        var roomUserMappings = new List<RoomUserMapping>();
                        if (item.Users != null && item.Users.Any())
                        {
                            foreach (var user in item.Users)
                            {
                                roomUserMappings.Add(new RoomUserMapping
                                {
                                    RoomOrganizationId = detail.Id,
                                    UserId = user.UserId,
                                    BusinessTripApplicationDetailId = user.BusinessTripApplicationDetailId,
                                    IsChange = isChange
                                });
                            }
                            _uow.GetRepository<RoomUserMapping>().Add(roomUserMappings);
                        }
                    }
                }
            }
        }

        public async Task<ResultDTO> GetDetailUsersInRoom(ReportType type, string Code, QueryArgs args)
        {
            var result = new List<IReportDetailBTA>();
            int count = 0;

            IEnumerable<BtaDetailViewModel> allBusinessTrips = new List<BtaDetailViewModel>();
            if (type == ReportType.Flight || type == ReportType.Hotel)
            {
                allBusinessTrips = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync<BtaDetailViewModel>(x => x.BusinessTripApplication.Status == "Completed" || x.BusinessTripApplication.Status == "Completed Changing");
            }
            else
            {

                allBusinessTrips = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync<BtaDetailViewModel>(args.Predicate, args.PredicateParameters);
            }
            if (allBusinessTrips != null && allBusinessTrips.Any())
            {
                var allChangeBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => !x.IsCancel);
                var allCancelBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => x.IsCancel);
                //code moi 9/10
                //allBusinessTrips = allBusinessTrips.Where(x => !allCancelBTAItems.Any(y => y.BusinessTripApplicationDetailId == x.Id)).ToList();
                //
                if (allChangeBTAItems.Any())
                {
                    foreach (var item in allBusinessTrips)
                    {
                        var changedItem = allChangeBTAItems.FirstOrDefault(x => x.BusinessTripApplicationDetailId == item.Id);
                        if (changedItem != null)
                        {
                            item.FromDate = changedItem.FromDate;
                            item.ToDate = changedItem.ToDate;
                            item.ArrivalCode = changedItem.DestinationCode;
                            item.ArrivalName = changedItem.DestinationName;
                            item.HotelCode = changedItem.NewHotelCode;
                            item.HotelName = changedItem.NewHotelName;
                            item.CheckInHotelDate = changedItem.NewCheckInHotelDate;
                            item.CheckOutHotelDate = changedItem.NewCheckOutHotelDate;
                            item.AirlineCode = changedItem.NewAirlineCode;
                            item.AirlineName = changedItem.NewAirlineName;
                            item.ComebackAirlineCode = changedItem.NewComebackAirlineCode;
                            item.ComebackAirlineName = changedItem.NewComebackAirlineName;
                            item.FlightNumberCode = changedItem.NewFlightNumberCode;
                            item.FlightNumberName = changedItem.NewFlightNumberName;
                            item.ComebackFlightNumberCode = changedItem.NewComebackAirlineCode;
                            item.ComebackFlightNumberName = changedItem.NewComebackAirlineName;
                        }
                    }
                }
                if (type == ReportType.Hotel)
                {
                    allBusinessTrips = allBusinessTrips.Where(x => !allCancelBTAItems.Any(y => y.BusinessTripApplicationDetailId == x.Id)).Select(x => x.Trim()).AsQueryable().Where(args.Predicate, args.PredicateParameters);
                    var allRooms = await _uow.GetRepository<RoomOrganization>().FindByAsync(x => (x.BusinessTripApplication.Status.Trim().Contains("Completed") || x.BusinessTripApplication.Status.Trim().Contains("Completed Changing")) && x.RoomTypeCode == Code, string.Empty, x => x.RoomUserMappings);
                    if (allBusinessTrips.Any())
                    {
                        var allRoomInHotels = allRooms.Where(x => x.RoomUserMappings.Any(y => allBusinessTrips.Any(z => z.Id == y.BusinessTripApplicationDetailId)));
                        if (allRoomInHotels.Any())
                        {

                            var dictChangeRooms = new Dictionary<Guid?, bool>();
                            var temps = new List<ReportDetailUserInRoomViewModel>();
                            foreach (var room in allRoomInHotels)
                            {
                                var detailIds = new List<Guid>();
                                detailIds.AddRange(room.RoomUserMappings.Select(x => x.BusinessTripApplicationDetailId.Value));
                                var tempDictChangeRooms = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => detailIds.Contains(x.BusinessTripApplicationDetailId.Value) && x.IsChange);
                                if (tempDictChangeRooms.Any())
                                {
                                    dictChangeRooms = tempDictChangeRooms.ToDictionary(t => t.BusinessTripApplicationDetailId, t => t.IsChange && t.RoomOrganization.RoomTypeCode != room.RoomTypeCode);
                                }
                                if (detailIds.Any())
                                {
                                    var detailsInfo = allBusinessTrips.Where(x => detailIds.Contains(x.Id));
                                    count = detailsInfo.Count();
                                    detailsInfo = detailsInfo.Skip((args.Page - 1) * args.Limit);
                                    foreach (var detail in detailsInfo)
                                    {
                                        var totalDays = (detail.CheckOutHotelDate.Value - detail.CheckInHotelDate.Value).TotalDays;
                                        var isChangeRoom = dictChangeRooms.Where(x => x.Key == detail.Id).FirstOrDefault();
                                        if (temps.All(x => x.Id != detail.Id))
                                        {
                                            temps.Add(new ReportDetailUserInRoomViewModel
                                            {
                                                Id = detail.Id,
                                                SAPCode = detail.SAPCode,
                                                FullName = detail.FullName,
                                                DepartmentCode = detail.DepartmentCode,
                                                DepartmentName = detail.DepartmentName,
                                                FromDate = detail.FromDate.Value.ToLocalTime(),
                                                ToDate = detail.ToDate.Value.ToLocalTime(),
                                                TotalDays = GetTotalDateFromDoubleDate(totalDays),
                                                CheckInDate = detail.CheckInHotelDate.Value.ToLocalTime(),
                                                CheckOutDate = detail.CheckOutHotelDate.Value.ToLocalTime(),
                                                IsChangeRoom = isChangeRoom.Value,
                                                TrackId = Guid.NewGuid(),
                                                Created = detail.Created
                                            });
                                        }
                                    }
                                }
                            }
                            result.AddRange(temps);
                        }
                        if (result.Any())
                        {
                            var temp = result;
                            //var temp = new List<ReportDetailUserInRoomViewModel>();
                            var itemChangedRoom = temp.Cast<ReportDetailUserInRoomViewModel>().Where(x => x.IsChangeRoom).ToList();
                            var _itemChangedRoom = temp.Cast<ReportDetailUserInRoomViewModel>().Where(x => !itemChangedRoom.Any(y => y.Id == x.Id)).ToList();
                            foreach (var item in itemChangedRoom)
                            {
                                var trackItemChange = result.Where(x => x.TrackId == item.TrackId).FirstOrDefault();
                                if (trackItemChange != null)
                                {
                                    temp.Remove(trackItemChange);
                                }
                            }
                        }
                    }
                }
                else if (type == ReportType.Flight)
                {
                    //code moi
                    allBusinessTrips = allBusinessTrips.Where(x => !allCancelBTAItems.Any(y => y.BusinessTripApplicationDetailId == x.Id));

                    string jsonObj = JsonConvert.SerializeObject(allBusinessTrips);
                    List<BtaDetailViewModel> list = new List<BtaDetailViewModel>();
                    var list1 = JsonConvert.DeserializeObject<List<BtaDetailViewModel>>(jsonObj);
                    var list2 = JsonConvert.DeserializeObject<List<BtaDetailViewModel>>(jsonObj);
                    var allBusinessesCloneComeBackFlightNumbers = list1.Where(x => !string.IsNullOrEmpty(x.ComebackFlightNumberCode)).OrderBy(x => x.ComebackFlightNumberName);
                    foreach (var item in allBusinessesCloneComeBackFlightNumbers)
                    {
                        item.FlightNumberCode = item.ComebackFlightNumberCode;
                        item.FlightNumberName = item.ComebackFlightNumberName;
                        list.Add(item);
                    }
                    var allBusinessesCloneFlightNumbers = list2.Where(x => !string.IsNullOrEmpty(x.FlightNumberCode)).OrderBy(x => x.FlightNumberName);
                    foreach (var x in allBusinessesCloneFlightNumbers)
                    {
                        list.Add(x);
                    }
                    allBusinessTrips = list;
                    allBusinessTrips = allBusinessTrips.Select(x => x.Trim()).AsQueryable().Where(args.Predicate, args.PredicateParameters);

                    var detailFlightNumberOtems = allBusinessTrips.Where(x => x.FlightNumberCode == Code).GroupBy(y => y.SAPCode);
                    if (detailFlightNumberOtems.Any())
                    {
                        count = detailFlightNumberOtems.Count();
                        detailFlightNumberOtems = detailFlightNumberOtems.Skip((args.Page - 1) * args.Limit);
                        foreach (var detail in detailFlightNumberOtems)
                        {
                            //var firstUser = detail.FirstOrDefault();
                            //if (firstUser != null)
                            //{
                            //    result.Add(new ReportDetailUserInFlightNumberViewModel
                            //    {
                            //        Id = firstUser.Id,
                            //        DepartmentCode = firstUser.DepartmentCode,
                            //        DepartmentName = firstUser.DepartmentName,
                            //        SAPCode = firstUser.SAPCode,
                            //        FullName = firstUser.FullName,
                            //        FromDate = firstUser.FromDate.Value.ToLocalTime(),
                            //        ToDate = firstUser.ToDate.Value.ToLocalTime()
                            //    });
                            //}
                            //
                            //code moi 9/10
                            var arrayTemporary = new List<ReportDetailUserInFlightNumberViewModel>();
                            foreach (var x in detail)
                            {
                                arrayTemporary.Add(new ReportDetailUserInFlightNumberViewModel
                                {
                                    Id = x.Id,
                                    DepartmentCode = x.DepartmentCode,
                                    DepartmentName = x.DepartmentName,
                                    SAPCode = x.SAPCode,
                                    FullName = x.FullName,
                                    FromDate = x.FromDate.Value.ToLocalTime(),
                                    ToDate = x.ToDate.Value.ToLocalTime()
                                });
                            }
                            result.AddRange(arrayTemporary);
                            //
                        }
                    }


                }
                else if (type == ReportType.Status)
                {
                    foreach (var x in allBusinessTrips)
                    {
                        if (await CheckIsChangingCancellingItem(x.BusinessTripApplicationId))
                        {
                            x.StatusItem = RuleValidate.Changing;
                        }
                        else if (x.StatusItem.Contains("Waiting"))
                        {
                            var isRevoking = await CheckIsRevokingItem(x.BusinessTripApplicationId);
                            if (isRevoking && Code == "Revoking")
                            {
                                x.StatusItem = "Revoking";
                            }
                            else
                            {
                                x.StatusItem = "In Progress";
                            }
                        }
                        var isChanging = allCancelBTAItems.FirstOrDefault(i => i.BusinessTripApplicationDetailId == x.Id && !i.IsCancel);
                        if (isChanging != null)
                        {
                            x.FromDate = isChanging.FromDate;
                            x.ToDate = isChanging.ToDate;
                            x.HotelName = isChanging.NewHotelName;
                            x.FlightNumberName = isChanging.NewFlightNumberName;
                            x.Reason = isChanging?.Reason;
                            x.CheckInHotelDate = isChanging.NewCheckInHotelDate;
                            x.CheckOutHotelDate = isChanging.NewCheckOutHotelDate;
                            x.ArrivalName = isChanging.DestinationName;
                        }
                    }
                    allBusinessTrips = allBusinessTrips.Where(i => Code.Contains(i.StatusItem)).OrderBy(o => o.ReferenceNumber);

                    if (allBusinessTrips.Any())
                    {
                        count = allBusinessTrips.Count();
                        allBusinessTrips = allBusinessTrips.Skip((args.Page - 1) * args.Limit);
                        foreach (var detail in allBusinessTrips)
                        {
                            //var totalDays = (detail.CheckOutHotelDate.Value - detail.CheckInHotelDate.Value).TotalDays;
                            result.Add(new ReportDetailUserInStatusViewModel
                            {
                                Id = detail.Id,
                                DepartmentCode = detail.DepartmentCode,
                                DepartmentName = detail.DepartmentName,
                                SAPCode = detail.SAPCode,
                                FullName = detail.FullName,
                                ReferenceNumber = detail.ReferenceNumber,
                                FlightNumberCode = detail.FlightNumberCode,
                                FlightNumberName = detail.FlightNumberName,
                                HotelCode = detail.HotelCode,
                                HotelName = detail.HotelName,
                                FromDate = detail.FromDate.Value.ToLocalTime(),
                                ToDate = detail.ToDate.Value.ToLocalTime(),
                                Created = detail.Created,
                                CheckInDate = detail.CheckInHotelDate?.ToLocalTime(),
                                CheckOutDate = detail.CheckOutHotelDate?.ToLocalTime()
                                //TotalDays = GetTotalDateFromDoubleDate(totalDays)
                            });
                        }
                    }
                }
            }
            return new ResultDTO { Object = new ArrayResultDTO { Data = result, Count = count } };
        }

        public async Task<ResultDTO> Validate(BusinessTripValidateArg arg)
        {
            var invalidItems = new List<BtaDetailViewModel>();
            var arrayUserHaveTicket = new List<BtaDetailViewModel>();
            //var statusToCheckes = new string[] { "Draft", "Cancelled", "Rejected", "Requested To Change", "Revoke" };
            var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft" };
            var statusToCheckes_Com = new string[] { "Completed", "Completed Changing"};
            var businesstripDetailTemp = JsonConvert.DeserializeObject<List<BusinessTripDetailDTO>>(arg.BusinessTripDetails);
            if (businesstripDetailTemp != null && businesstripDetailTemp.Any())
            {
                var arrayNew = businesstripDetailTemp;
                foreach (var newItem in arrayNew)
                {
                    var businessTripDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync<BtaDetailViewModel>(x => (arg.BusinessTripApplicationId.HasValue ? x.BusinessTripApplicationId != arg.BusinessTripApplicationId.Value : true)
                    && (x.BusinessTripApplication.Status == "Completed" || !statusToCheckes.Contains(x.BusinessTripApplication.Status)) && x.SAPCode == newItem.SAPCode && x.Id != newItem.Id);
                    if (businessTripDetails != null)
                    {
                        var currentSAPs = businessTripDetails.ToList();
                        var removeItem = new List<BtaDetailViewModel>();
                        var allChangeBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => !x.IsCancel && (x.BusinessTripApplication.Status == "Completed Changing"));
                        if (allChangeBTAItems.Any())
                        {
                            foreach (var item in currentSAPs)
                            {
                                var changedItem = allChangeBTAItems.FirstOrDefault(x => x.BusinessTripApplicationDetailId == item.Id);
                                if (changedItem != null)
                                {
                                    item.FromDate = changedItem.NewFromDate;
                                    item.ToDate = changedItem.NewToDate;
                                }

                                var changeCancelItem = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => x.IsCancel && x.BusinessTripApplicationDetailId == item.Id && (statusToCheckes.Contains(x.BusinessTripApplication.Status) || statusToCheckes_Com.Contains(x.BusinessTripApplication.Status)));
                                if (changeCancelItem.Any())
                                    removeItem.Add(item);
                            }
                        }
                        if (removeItem.Any())
                            currentSAPs = currentSAPs.Where(x => !removeItem.Any(y => y.Id.Equals(x.Id))).ToList();

                        if (currentSAPs.Any())
                        {
                            var invalidDateItems = currentSAPs.Where(x => CheckValidDate(newItem.FromDate.Value, newItem.ToDate.Value, x)).ToList();
                            if (invalidDateItems.Any())
                            {
                                foreach (var item in invalidDateItems)
                                {
                                    var result = invalidItems.Find(x => x.Id == item.Id);
                                    if (result == null)
                                    {
                                        item.FromDate = item.FromDate.Value.LocalDateTime;
                                        item.ToDate = item.ToDate.Value.LocalDateTime;
                                        invalidItems.Add(item);
                                    }

                                }
                            }
                        }
                    }
                }
            };
            return new ResultDTO { Object = new ArrayResultDTO { Data = invalidItems, Count = invalidItems.Count } };
        }
        /// <summary>
        /// from la cua new, to la cua new
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="oldItem"></param>
        /// <returns></returns>
        private bool CheckValidDate(DateTimeOffset from, DateTimeOffset to, BtaDetailViewModel oldItem)
        {
            bool flag = false;
            // Trung From hoac To Date
            if (from == oldItem.FromDate || to == oldItem.ToDate)
            {
                flag = true;
            }
            // from va to thuoc from/to itemChecked
            if ((from >= oldItem.FromDate && from <= oldItem.ToDate) || (to <= oldItem.ToDate && to >= oldItem.FromDate))
            {
                flag = true;
            }
            //from nho hon itemCheck va to lon hon itemCheck
            if (from < oldItem.FromDate && to > oldItem.ToDate)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<ResultDTO> ExportReport(ReportType type, ViewModels.Args.ExportReportArg args)
        {
            try
            {
                var businessTripApplicationReportExportBO = new BusinessTripApplicationReportProcessingBO(_uow, logger);
                businessTripApplicationReportExportBO.SetAdditionalInfo(type);
                return await businessTripApplicationReportExportBO.ExportReport(args);

            }
            catch (Exception ex)
            {
                logger.LogError($"Found error at ExportReport: ExportReport. Message: {ex.Message}");
                return new ResultDTO { Object = null };
            }

        }
        public async Task<ResultDTO> ExportTypeStatus(ReportType type, ViewModels.Args.ExportReportArg args)
        {
            try
            {
                var businessTripApplicationReportExportBO = new BusinessTripApplicationReportProcessingBO(_uow, logger);
                businessTripApplicationReportExportBO.SetAdditionalInfo(type);
                return await businessTripApplicationReportExportBO.ExportTypeStatus(args);

            }
            catch (Exception ex)
            {
                logger.LogError($"Found error at ExportRovoking: ExportRovoking. Message: {ex.Message}");
                return new ResultDTO { Object = null };
            }

        }

        private double GetTotalDateFromDoubleDate(double days)
        {
            var integerDay = Math.Floor(days);
            double fourHour = (double)1 / 6;
            double eightHour = (double)1 / 3;
            if (days - integerDay >= eightHour) // nhỏ hơn 4 tiếng thì tính nửa ngày - lớn hơn hoặc bằng 8 tiếng
            {
                return integerDay + 1;
            }
            else if (days - integerDay < eightHour && days - integerDay >= fourHour)       // lớn hơn hoặc bằng 4 tiếng và nhỏ hơn 8 tiếng
            {
                return integerDay + 0.5;
            }
            return integerDay;
        }
        private int GetTotalRoomsFromStaff(long staffs, string RoomTypeCode, IEnumerable<RoomType> rooms)
        {
            int result = 0;
            if (staffs > 0 & rooms != null && rooms.Any())
            {
                var currentRoom = rooms.FirstOrDefault<RoomType>(x => x.Code == RoomTypeCode);
                if (currentRoom != null)
                {
                    result = (int)staffs / (currentRoom.Quota);
                    if (result * currentRoom.Quota < staffs)
                    {
                        result++;
                    }
                }
            }
            return result;
        }
        public async Task<byte[]> PrintForm(Guid Id)
        {
            // BTAInfo
            byte[] result = null;
            Dictionary<string, string> emptyDic = new Dictionary<string, string>();
            emptyDic.Add("", "");
            BTAPrintFormViewModel printModel = new BTAPrintFormViewModel();
            var btaDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync<BTADetailViewModel>(i => i.BusinessTripApplicationId == Id);
            if (btaDetails.Any())
            {
                printModel.Details = btaDetails.OrderBy(x => x.FromDate).ThenBy(x => x.ToDate).ToList();
                printModel.ChangeDetails = (await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBTADetailViewModel>(i => i.BusinessTripApplicationId == Id)).ToList();
            }
            printModel.Head = await GetBTAHeadAsync(Id);
            //result = ExportXLS("Buz trip application.xlsx", printModel);
            //CR- 137
            var properties = typeof(BTAHeadViewModel).GetProperties();
            var pros = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                var value = Convert.ToString(property.GetValue(printModel.Head));
                pros[property.Name] = SecurityElement.Escape(value);
            }
            var tbPros = new List<List<Dictionary<string, string>>>();

            //Table 1 - BTA details
            var BTADetails_Properties = typeof(BTADetailViewModel).GetProperties();
            var stt = 1;
            var _BTADetails = new List<Dictionary<string, string>>();
            foreach (var item in printModel.Details)
            {
                var fromDate = !string.IsNullOrEmpty(item.FromDate.ToString()) ? String.Format("{0:d/M/yyyy hh:mm tt}", item.FromDate.Value.LocalDateTime) : "";
                var toDate = !string.IsNullOrEmpty(item.ToDate.ToString()) ? String.Format("{0:d/M/yyyy hh:mm tt}", item.ToDate.Value.LocalDateTime) : "";
                var BTADetails = new Dictionary<string, string>();
                foreach (var property in BTADetails_Properties)
                {
                    if (property.Name.Contains("FromDate"))
                    {
                        BTADetails[property.Name] = SecurityElement.Escape(fromDate);
                        continue;
                    }
                    if (property.Name.Contains("ToDate"))
                    {
                        BTADetails[property.Name] = SecurityElement.Escape(toDate);
                        continue;
                    }
                    var value = Convert.ToString(property.GetValue(item));
                    BTADetails[property.Name] = SecurityElement.Escape(value);
                }
                BTADetails["TT"] = stt + string.Empty;
                stt++;
                _BTADetails.Add(BTADetails);
            }
            if (printModel.Details.Count == 0)
            {
                _BTADetails.Add(emptyDic);
            }
            tbPros.Add(_BTADetails);

            //Table 2 - BTA signed
            var BTAHead_Properties = typeof(CommonBTAViewModel).GetProperties();
            var BTAHead = new Dictionary<string, string>();
            var _BTAHead = new List<Dictionary<string, string>>();
            var items = new CommonBTAViewModel
            {
                AdminDepartmentRemarkSignedDate = !string.IsNullOrEmpty(printModel.Head.AdminDepartmentRemarkSignedDate.ToString()) ? String.Format("{0:d/M/yyyy}", printModel.Head.AdminDepartmentRemarkSignedDate.Value.LocalDateTime) : "",
                ApplicantSignedDate = !string.IsNullOrEmpty(printModel.Head.ApplicantSignedDate.ToString()) ? String.Format("{0:d/M/yyyy}", printModel.Head.ApplicantSignedDate.Value.LocalDateTime) : "",
                SGeneralManagerSignedDate = !string.IsNullOrEmpty(printModel.Head.SGeneralManagerSignedDate.ToString()) ? String.Format("{0:d/M/yyyy}", printModel.Head.SGeneralManagerSignedDate.Value.LocalDateTime) : "",
                RequestorNote = printModel.Head.RequestorNote,
                DeptLine = printModel.Head.DeptLine,
                ReferenceNumber = printModel.Head.ReferenceNumber
            };
            foreach (var property in BTAHead_Properties)
            {
                var value = Convert.ToString(property.GetValue(items));
                BTAHead[property.Name] = SecurityElement.Escape(value);
            }
            BTAHead["AdminDepartmentRemark"] = !string.IsNullOrEmpty(printModel.Head.AdminDepartmentRemark) ? printModel.Head.AdminDepartmentRemark + @" Signed/Đã ký" : "";
            BTAHead["Applicant"] = !string.IsNullOrEmpty(printModel.Head.Applicant) ? printModel.Head.Applicant + @" Signed/Đã ký" : "";
            BTAHead["SGeneralManager"] = !string.IsNullOrEmpty(printModel.Head.SGeneralManager) ? printModel.Head.SGeneralManager + @" Signed/Đã ký" : "";
            BTAHead["GeneralDirector"] = !string.IsNullOrEmpty(printModel.Head.GeneralDirector) ? printModel.Head.GeneralDirector + @" Signed/Đã ký" : "";
            BTAHead["GeneralDirectorSignedDate"] = !string.IsNullOrEmpty(printModel.Head.GeneralDirectorSignedDate.ToString()) ? String.Format("{0:d/M/yyyy}", printModel.Head.GeneralDirectorSignedDate.Value.LocalDateTime) : "";
            _BTAHead.Add(BTAHead);
            tbPros.Add(_BTAHead);

            //Table 3 - BTA change cancel
            var BTAChange_Properties = typeof(ChangeCancelBTADetailViewModel).GetProperties();
            var _stt = 1;
            var _BTAChange = new List<Dictionary<string, string>>();
            foreach (var item in printModel.ChangeDetails)
            {
                var BTAChange = new Dictionary<string, string>();
                foreach (var property in BTAChange_Properties)
                {
                    if (property.Name.Contains("NewFromDate"))
                    {
                        string fromDate = !string.IsNullOrEmpty(item.NewFromDate.ToString()) ? item.NewFromDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") + " -" : "";
                        BTAChange[property.Name] = SecurityElement.Escape(fromDate);
                        continue;
                    }
                    if (property.Name.Contains("NewToDate"))
                    {
                        var toDate = !string.IsNullOrEmpty(item.NewToDate.ToString()) ? item.NewToDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") + ":" : "";
                        BTAChange[property.Name] = SecurityElement.Escape(toDate);
                        continue;
                    }
                    if (property.Name.Contains("GeneralManager"))
                    {
                        string generalManager = !string.IsNullOrEmpty(printModel.Head.GeneralManager) ? printModel.Head.GeneralManager + @" Signed/Đã ký" : "";
                        BTAChange[property.Name] = SecurityElement.Escape(generalManager);
                        continue;
                    }
                    var value = Convert.ToString(property.GetValue(item));
                    BTAChange[property.Name] = SecurityElement.Escape(value);
                }
                BTAChange["TT"] = _stt + string.Empty;
                _stt++;
                _BTAChange.Add(BTAChange);
            }
            if (printModel.ChangeDetails.Count == 0)
            {
                _BTAChange.Add(emptyDic);
            }
            tbPros.Add(_BTAChange);

            if (pros.Count > 0 && tbPros.Count > 0)
            {
                result = WordAutomation.ExportPDF("BTAForm.docx", pros, tbPros);
            }

            return result;
        }

        public byte[] ExportXLS(string template, BTAPrintFormViewModel printModel)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(path, "PrintDocument", template);
            var memoryStream = new MemoryStream();
            try
            {
                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    using (var pck = new ExcelPackage(stream))
                    {
                        ExcelWorkbook WorkBook = pck.Workbook;
                        ExcelWorksheet worksheet = WorkBook.Worksheets[2];
                        InsertTargetData(worksheet, 14, printModel.Details);
                        InsertTargetData(worksheet, 17 + (printModel.Details.Count - 1), printModel.Head);
                        InsertTargetData(worksheet, 27 + (printModel.Details.Count - 1), printModel.ChangeDetails, printModel.Head.GeneralManager);
                        //code moi
                        var properties = typeof(CommonBTAViewModel).GetProperties();
                        var pros = new Dictionary<string, string>();
                        var item = new CommonBTAViewModel
                        {
                            AdminDepartmentRemarkSignedDate = !string.IsNullOrEmpty(printModel.Head.AdminDepartmentRemarkSignedDate.ToString()) ? String.Format("{0:d/M/yyyy}", printModel.Head.AdminDepartmentRemarkSignedDate.Value.LocalDateTime) : "",
                            ApplicantSignedDate = !string.IsNullOrEmpty(printModel.Head.ApplicantSignedDate.ToString()) ? String.Format("{0:d/M/yyyy}", printModel.Head.ApplicantSignedDate.Value.LocalDateTime) : "",
                            SGeneralManagerSignedDate = !string.IsNullOrEmpty(printModel.Head.SGeneralManagerSignedDate.ToString()) ? String.Format("{0:d/M/yyyy}", printModel.Head.SGeneralManagerSignedDate.Value.LocalDateTime) : "",
                            RequestorNote = printModel.Head.RequestorNote,
                            DeptLine = printModel.Head.DeptLine,
                            ReferenceNumber = printModel.Head.ReferenceNumber
                        };
                        foreach (var property in properties)
                        {
                            var value = Convert.ToString(property.GetValue(item));
                            pros[property.Name] = SecurityElement.Escape(value);
                        }
                        var regex = new Regex(@"\[\[[\d\w\s]*\]\]", RegexOptions.IgnoreCase);
                        var tokens = worksheet.Cells.Where(x => x.Value != null && regex.Match(x.Value.ToString()).Success);
                        foreach (var token in tokens)
                        {
                            var fieldToken = token.Value.ToString().Trim(new char[] { '[', ']' });
                            if (pros.ContainsKey(fieldToken))
                            {
                                token.Value = pros[fieldToken];
                            }
                        }
                        //
                        pck.SaveAs(memoryStream);

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("", ex.Message);
            }
            return memoryStream.ToArray();
        }
        public async Task<BTAHeadViewModel> GetBTAHeadAsync(Guid Id)
        {
            // dat code
            BTAHeadViewModel headModel = new BTAHeadViewModel();
            var wfInstances = await _uow.GetRepository<WorkflowInstance>().FindByAsync(i => i.ItemId == Id && !i.WorkflowName.Contains("Revoke"));
            var instanceChanging = wfInstances.Where(i => i.WorkflowName.Contains("Changing")).OrderByDescending(o => o.Created)?.FirstOrDefault();
            var instanceCompleted = wfInstances.Where(i => !i.WorkflowName.Contains("Changing")).OrderByDescending(o => o.Created)?.FirstOrDefault();

            if (instanceChanging != null)
            {
                //Step có status là completed changing
                headModel.GeneralManager = instanceChanging.Histories.FirstOrDefault(i => i.Outcome == "Completed Changing")?.ApproverFullName;
                headModel.GeneralManagerSignedDate = instanceChanging.Histories.FirstOrDefault(i => i.Outcome == "Completed Changing")?.Modified;
            }
            if (instanceCompleted != null)
            {
                var workFlowSteps = wfInstances.FirstOrDefault().WorkflowData.Steps;
                string[] JobGrade1 = { "G1", "G2", "G3" };
                string[] JobGrade2 = { "G5", "G6", "G7", "G8" };
                
                var allGrade = await _uow.GetRepository<JobGrade>().GetAllAsync();
                var jobGrade3 = allGrade.FirstOrDefault(x => x.Title.ToUpper().Equals(("G3")));
                var jobGrade4 = allGrade.FirstOrDefault(x => x.Title.ToUpper().Equals(("G4")));

                var JDTitle3 = jobGrade3?.Title ?? "G3";
                var JDTitle4 = jobGrade4?.Title ?? "G4";
                
                if (jobGrade3 != null && jobGrade4 != null)
                {
                    JobGrade1 = allGrade.Where(x => x.Grade <= jobGrade3.Grade).Select(x => x.Title).ToArray();
                    JobGrade2 = allGrade.Where(x => x.Grade >= (jobGrade4.Grade + 1)).Select(x => x.Title).ToArray();
                }
                
                //Step cuối của flow tạo mới
                headModel.AdminDepartmentRemark = instanceCompleted.Histories.FirstOrDefault(i => i.Outcome == "Completed")?.ApproverFullName;
                headModel.AdminDepartmentRemarkSignedDate = instanceCompleted.Histories.FirstOrDefault(i => i.Outcome == "Completed")?.Modified;
                //1st Approval:
                var firstApproval = workFlowSteps.FirstOrDefault(x => x.StepName.Equals("1st approval", StringComparison.CurrentCultureIgnoreCase));

                headModel.SGeneralManager = instanceCompleted.Histories.FirstOrDefault(i => i.StepNumber == firstApproval?.StepNumber)?.ApproverFullName;
                headModel.SGeneralManagerSignedDate = instanceCompleted.Histories.FirstOrDefault(i => i.StepNumber == firstApproval?.StepNumber)?.Modified;
                var BTA = _uow.GetRepository<BusinessTripApplication>(true).FindById(instanceCompleted.ItemId);
                int? setStep = null;
                //if (JobGrade1.Contains(BTA.MaxGrade) || (BTA.MaxGrade == "G4" && !BTA.IsStore))
                if (JobGrade1.Contains(BTA.MaxGrade) || (BTA.MaxGrade == JDTitle4 && !BTA.IsStore))
                {
                    // 2nd
                    var secondApproval = workFlowSteps.FirstOrDefault(x => x.StepName.Equals("2nd approval", StringComparison.CurrentCultureIgnoreCase));
                    setStep = secondApproval?.StepNumber;

                }
                //else if (JobGrade2.Contains(BTA.MaxGrade) || (BTA.MaxGrade == "G4" && BTA.IsStore))
                else if (JobGrade2.Contains(BTA.MaxGrade) || (BTA.MaxGrade == JDTitle4 && BTA.IsStore))
                {
                    // 1st
                    setStep = firstApproval?.StepNumber;

                }
                headModel.Applicant = instanceCompleted.Histories.FirstOrDefault(i => i.Outcome == "Submitted")?.ApproverFullName;
                headModel.ApplicantSignedDate = instanceCompleted.Histories.FirstOrDefault(i => i.Outcome == "Submitted")?.Modified;
                headModel.GeneralDirector = instanceCompleted.Histories.FirstOrDefault(i => setStep != null && i.StepNumber == setStep)?.ApproverFullName;
                headModel.GeneralDirectorSignedDate = instanceCompleted.Histories.FirstOrDefault(i => setStep != null && i.StepNumber == setStep)?.Modified;
                headModel.RequestorNote = BTA.RequestorNote;
                headModel.DeptLine = BTA.DeptName;
                headModel.ReferenceNumber = BTA.ReferenceNumber;
            }
            return headModel;
        }
        private void InsertTargetData(ExcelWorksheet worksheet, int styleRow, List<BTADetailViewModel> details)
        {
            var index = 0;
            var fromRow = styleRow + 1;
            var toRow = fromRow + details.Count;
            for (int i = fromRow; i < toRow; i++)
            {
                var target = details.ElementAt(index);
                worksheet.InsertRow(i, 1, styleRow);
                var row = worksheet.Row(i);
                row.Height = 23;
                worksheet.Cells[$"A{i}"].Value = ++index;
                worksheet.Cells[$"B{i}"].Value = target.SAPCode;
                worksheet.Cells[$"C{i}"].Value = target.FullName;
                worksheet.Cells[$"D{i}"].Value = target.DepartmentName;
                worksheet.Cells[$"E{i}"].Value = target.Gender;
                worksheet.Cells[$"F{i}"].Value = target.DepartureName;
                worksheet.Cells[$"G{i}"].Value = target.ArrivalName;
                worksheet.Cells[$"H{i}"].Value = target.FromDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt");
                worksheet.Cells[$"I{i}"].Value = target.ToDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt");
                worksheet.Cells[$"J{i}"].Value = target.HotelName;
                worksheet.Cells[$"K{i}"].Value = target.FlightNumberName + "\n" + target.ComebackFlightNumberName;
            }
            worksheet.DeleteRow(styleRow);
        }
        private void InsertTargetData(ExcelWorksheet worksheet, int styleRow, List<ChangeCancelBTADetailViewModel> changeDetails, string GeneralManager = null)
        {
            var index = 0;
            var fromRow = styleRow + 1;
            var toRow = fromRow + changeDetails.Count;
            string signed = @" Signed/Đã ký";
            for (int i = fromRow; i < toRow; i++)
            {
                var target = changeDetails.ElementAt(index);
                worksheet.InsertRow(i, 1, styleRow);
                var row = worksheet.Row(i);
                row.Height = 23;
                ExcelRange mergeRange1 = worksheet.Cells[$"G{i}:I{i}"];
                ExcelRange mergeRange2 = worksheet.Cells[$"J{i}:K{i}"];
                mergeRange1.Merge = true;
                mergeRange2.Merge = true;
                worksheet.Cells[$"A{i}"].Value = ++index;
                worksheet.Cells[$"B{i}"].Value = target.SAPCode;
                worksheet.Cells[$"C{i}"].Value = target.FullName;
                worksheet.Cells[$"D{i}"].Value = target.FromDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt");
                worksheet.Cells[$"E{i}"].Value = target.ToDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt");
                worksheet.Cells[$"F{i}"].Value = target.DestinationName;
                worksheet.Cells[$"G{i}"].Value = target.Reason;
                worksheet.Cells[$"J{i}"].Value = !string.IsNullOrEmpty(GeneralManager) ? GeneralManager + signed : string.Empty;
                //if (!string.IsNullOrEmpty(GeneralManager))
                //{
                //    using (ExcelRange Rng = worksheet.Cells[$"J{i}"])
                //    {
                //        ExcelRichTextCollection RichTxtCollection = Rng.RichText;
                //        ExcelRichText RichText = RichTxtCollection.Add(worksheet.Cells[$"J{i}"].Value.ToString());
                //        RichText.Bold = true;
                //        RichText.Size = 10;
                //    }
                //}
                worksheet.Cells[$"J{i}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"J{i}"].Style.Font.Bold = true;
                worksheet.Cells[$"J{i}"].Style.Font.Size = 11;

            }
            worksheet.DeleteRow(styleRow);
        }
        private void InsertTargetData(ExcelWorksheet worksheet, int styleRow, BTAHeadViewModel head)
        {
            var i = styleRow + 1;
            worksheet.InsertRow(i, 1, styleRow);
            var row = worksheet.Row(i);
            row.Height = 23;
            ExcelRange mergeRange1 = worksheet.Cells[$"A{i}:C{i}"];
            ExcelRange mergeRange2 = worksheet.Cells[$"D{i}:F{i}"];
            ExcelRange mergeRange3 = worksheet.Cells[$"G{i}:I{i}"];
            ExcelRange mergeRange4 = worksheet.Cells[$"J{i}:K{i}"];

            mergeRange1.Merge = true;
            mergeRange2.Merge = true;
            mergeRange3.Merge = true;
            mergeRange4.Merge = true;

            var cellA = worksheet.Cells[$"A{i}"];
            var cellD = worksheet.Cells[$"D{i}"];
            var cellG = worksheet.Cells[$"G{i}"];
            var cellJ = worksheet.Cells[$"J{i}"];
            string signed = @" Signed/Đã ký";

            cellA.Value = !string.IsNullOrEmpty(head.AdminDepartmentRemark) ? head.AdminDepartmentRemark + signed : string.Empty;
            cellD.Value = !string.IsNullOrEmpty(head.Applicant) ? head.Applicant + signed : string.Empty;
            cellG.Value = !string.IsNullOrEmpty(head.SGeneralManager) ? head.SGeneralManager + signed : string.Empty;
            cellJ.Value = !string.IsNullOrEmpty(head.GeneralDirector) ? head.GeneralDirector + signed : string.Empty;

            // Align Center
            cellA.Style.Font.Bold = true;
            cellA.Style.Font.Size = 11;
            cellD.Style.Font.Bold = true;
            cellD.Style.Font.Size = 11;
            cellG.Style.Font.Bold = true;
            cellG.Style.Font.Size = 11;
            cellJ.Style.Font.Bold = true;
            cellJ.Style.Font.Size = 11;
            cellA.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cellD.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cellG.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cellJ.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.DeleteRow(styleRow);
        }

        public async Task<ResultDTO> GetTripGroups(Guid Id)
        {
            List<BTAPassengerViewModel> returnValue = new List<BTAPassengerViewModel>();
            try
            {
                BTAViewModel btaItem = Id.GetAsBtaViewModel(_uow);
                if (btaItem != null)
                {
                    List<BtaDetailViewModel> details = btaItem.GetAsBtaDetailsViewModel(_uow);
                    #region Funcs
                    Func<IEnumerable<BtaDetailViewModel>, int, List<BTAPassengerViewModel>> SetGroupNumber = (IEnumerable<BtaDetailViewModel> itemList, int groupNumber) =>
                    {
                        int count = 0;
                        List<BTAPassengerViewModel> returnGroupValue = new List<BTAPassengerViewModel>();
                        foreach (BtaDetailViewModel item in itemList)
                        {
                            count++;
                            decimal maxBudget = 0;
                            var btaOverBudget = _uow.GetRepository<BusinessTripOverBudget>().FindBy(x => x.BusinessTripApplicationId == item.BusinessTripApplicationId && x.Status == "Completed", "Created desc").ToList();
                            if (btaOverBudget != null)
                            {
                                foreach (var items in btaOverBudget)
                                {
                                    BusinessTripOverBudgetsDetail btaOverBudgetDetails = _uow.GetRepository<BusinessTripOverBudgetsDetail>(true)
                                    .GetSingle(x => x.BusinessTripOverBudgetId == items.Id && x.ExtraBudget > 0 && x.BtaDetailId == item.Id);
                                    if (btaOverBudgetDetails != null)
                                    {
                                        if (items.IsRoundTrip)
                                        {
                                            maxBudget = btaOverBudgetDetails.ExtraBudget > 0 ? btaOverBudgetDetails.ExtraBudget : 0;
                                        }
                                        else
                                        {
                                            maxBudget = (btaOverBudgetDetails.ExtraBudget > 0 ? btaOverBudgetDetails.ExtraBudget : 0) * 2;
                                        }

                                        break;
                                    }
                                }
                            }
                            if (maxBudget > 0)
                                item.MaxBudgetAmount = maxBudget;
                            else
                                item.MaxBudgetAmount = item.GetMaxBudgetLimit(_uow);
                            item.TripGroup = groupNumber;

                            item.GroupMemberCount = itemList.Count();
                            item.FlightDetails = item.GetFlightDetails(_uow);
                            //lamnl
                            var btaOverBudgetNone = _uow.GetRepository<BusinessTripOverBudgetsDetail>().GetSingle(x => x.BtaDetailId == item.Id && x.BusinessTripOverBudget.Status != "Cancelled", "Created desc");
                            if (btaOverBudgetNone != null)
                            {
                                item.BusinessTripOverBudgetId = btaOverBudgetNone.Id;
                                item.ReferenceNumberOverBudget = btaOverBudgetNone.BusinessTripOverBudget.ReferenceNumber;
                                item.Comments = btaOverBudgetNone.BusinessTripOverBudget.Comment;
                            }
                            returnGroupValue.Add(item.ConvertTo<BTAPassengerViewModel>());
                            //tudm
                            List<int> arr = new List<int>(10) { 9, 18, 27, 36, 54, 63, 72, 81, 90, 99 };
                            if (arr.IndexOf(count) > -1)
                                groupNumber++;
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
                    var groups = details.GroupBy(x => new BTATripGroupInfo(x, _uow).key).ToList();
                    if (groups.Any())
                    {
                        //assign group already had tripgroup
                        List<IGrouping<string, BtaDetailViewModel>> groupHasTripGroup = groups.Where(x => x != null && x.Any() && x.ToList().FirstOrDefault().TripGroup != 0).ToList();
                        foreach (var itemList in groupHasTripGroup)
                        {
                            returnValue.AddRange(SetGroupNumber(itemList, itemList.ToList()[0].TripGroup));
                        }

                        //assign group still not have tripgroup
                        List<IGrouping<string, BtaDetailViewModel>> groupNotHaveTripGroup = groups.Except(groupHasTripGroup).ToList();
                        foreach (var itemList in groupNotHaveTripGroup)
                        {
                            returnValue.AddRange(SetGroupNumber(itemList, GetAvailableGroupNumber()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError(logger, "GetTripGroups");
            }
            return new ResultDTO { Object = returnValue };
        }
        public async Task<ResultDTO> SavePassengerInfo(List<BTAPassengerViewModel> btaPassengerInfoArray)
        {
            ResultDTO returnValue = new ResultDTO();
            try
            {
                if (btaPassengerInfoArray != null)
                {
                    foreach (BTAPassengerViewModel currentPassenger in btaPassengerInfoArray)
                    {
                        currentPassenger.Update(_uow);
                    }

                    if (!btaPassengerInfoArray.Where(x => x.IsOverBudget).Any())
                    {
                        BusinessTripApplicationDetail btaDetailItem = await _uow.GetRepository<BusinessTripApplicationDetail>(true).FindByIdAsync(btaPassengerInfoArray.First().Id);
                        if (btaDetailItem != null)
                        {
                            btaDetailItem.BusinessTripApplication.IsOverBudget = false;
                            _uow.GetRepository<BusinessTripApplication>().Update(btaDetailItem.BusinessTripApplication);
                            await _uow.CommitAsync();
                        }
                    }

                    List<Guid> btaItemIds = btaPassengerInfoArray.Select(x => x.Id).ToList();
                    if (btaItemIds.Any())
                    {
                        var FlightDetailIds = await _uow.GetRepository<FlightDetail>()
                             .FindByAsync(x => x.BusinessTripApplicationDetailId.HasValue && btaItemIds.Contains(x.BusinessTripApplicationDetailId.Value));

                        var selectId = FlightDetailIds.Select(x => x.Id).ToList();
                        returnValue.Object = new { FlightDetailIds = selectId };
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError(logger, "SavePassengerInfo");
                returnValue.ErrorCodes = new List<int> { 1004 };
                returnValue.Messages = new List<string> { "Save data failed." };
            }
            return returnValue;
        }
        public ResultDTO GetUserTicketsInfo(Guid BTADetailId)
        {
            ResultDTO returnValue = new ResultDTO();
            try
            {
                var BTADetail = _uow.GetRepository<BusinessTripApplicationDetail>(true).GetSingle(x => x.Id == BTADetailId);
                if (BTADetail != null)
                {
                    returnValue.Object = BTADetail.UserId.GetUserById(_uow, true).GetUserFlightTickets(BTADetailId, _uow);
                }
            }
            catch (Exception ex)
            {
                ex.LogError(logger, "GetUserTicketsInfo");
                returnValue.ErrorCodes = new List<int> { 1004 };
                returnValue.Messages = new List<string> { "Get data failed." };
            }
            return returnValue;
        }
        public async Task<ResultDTO> SaveBookingInfo(List<BookingFlightViewModel> btaBookingInfoArray)
        {
            ResultDTO returnValue = new ResultDTO();
            try
            {
                logger.LogInformation("Start Save Booking Flight");
                if (btaBookingInfoArray != null)
                {
                    logger.LogInformation("Count Save Booking Flight: " + btaBookingInfoArray.Count);
                    foreach (BookingFlightViewModel currentBooking in btaBookingInfoArray)
                    {
                        logger.LogInformation("Before Save Booking Flight: bookingCode:" + currentBooking.BookingCode + " bookingNumber:" + currentBooking.BookingNumber + " BTADetailId:" + currentBooking.BTADetailId + " DirectFlight: " + currentBooking.DirectFlight);
                        await currentBooking.Update(_uow, logger);
                        logger.LogInformation("After Save Booking Flight");
                        //lamnl: check booking Complete
                        var btaDetail = _uow.GetRepository<BusinessTripApplicationDetail>().FindById(currentBooking.BTADetailId);
                        btaDetail.CheckBookingCompleted = true;
                    }
                    _uow.Commit();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error SaveBookingInfo: " + ex.Message + " Stact Trace:" + ex.StackTrace);
                returnValue.ErrorCodes = new List<int> { 1004 };
                returnValue.Messages = new List<string> { "Save data failed." };
            }
            return returnValue;
        }

        public async Task<ResultDTO> GetPassengerInformationBySAPCodes(List<string> btaPassengerSAPCodeArray)
        {
            ResultDTO returnValue = new ResultDTO();
            try
            {
                if (btaPassengerSAPCodeArray != null && btaPassengerSAPCodeArray.Any())
                {
                    returnValue.Object = await _uow.GetRepository<PassengerInformation>().FindByAsync(x => btaPassengerSAPCodeArray.Contains(x.SAPCode));
                }
            }
            catch (Exception ex)
            {
                ex.LogError(logger, "GetPassengerInformationBySAPCodes");
                returnValue.ErrorCodes = new List<int> { 1004 };
                returnValue.Messages = new List<string> { "Get data failed." };
            }
            return returnValue;
        }

        public async Task<ResultDTO> GetDetailItemById(Guid id)
        {
            var result = new ResultDTO();
            var currentItem = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByIdAsync<BusinessTripDetailDTO>(id);
            result.Object = currentItem;
            return result;
        }

        public async Task<ResultDTO> GetEmailSubmitter(Guid id)
        {
            //var result = new ResultDTO();
            //var currentBTA = _uow.GetRepository<BusinessTripApplication>(true).GetSingle(x => x.Id == id);
            //if (currentBTA != null)
            //{
            //    var infoSubmitter = await _uow.GetRepository<User>().GetSingleAsync(x => x.SAPCode == currentBTA.UserSAPCode);
            //    result.Object = infoSubmitter;
            //}
            //return result;
            var result = new ResultDTO();
            if (id != null)
            {
                BTAViewModel btaItem = id.GetAsBtaViewModel(_uow);
                if (btaItem != null)
                {
                    List<BtaDetailViewModel> details = btaItem.GetAsBtaDetailsViewModel(_uow);
                    var detail = details.Where(e => e.IsBookingContact).FirstOrDefault();
                    if (detail != null)
                    {
                        var data = new BookingContract()
                        {
                            EmailBookingContract = detail.Email,
                            FirstName = detail.FirstName,
                            SurName = detail.LastName,
                            PhoneNumber = detail.Mobile,
                        };
                        result.Object = data;
                    }
                    else
                    {
                        if (btaItem.BookingContact != null || btaItem.BookingContact != "[]")
                        {
                            var items = JsonConvert.DeserializeObject<List<BookingContactDTO>>(btaItem.BookingContact);
                            var item = items.FirstOrDefault();
                            var data = new BookingContract()
                            {
                                EmailBookingContract = item.Email,
                                FirstName = item.FirstName,
                                SurName = item.LastName,
                                PhoneNumber = item.Mobile,
                            };
                            result.Object = data;
                        }
                    }
                }
            }
            else
            {
                var currentItem = await _uow.GetRepository<BookingContract>().GetAllAsync();
                result.Object = currentItem.FirstOrDefault();
            }
            return result;
        }
        public async Task<ResultDTO> GetInfoAdmin()
        {
            var result = new ResultDTO();
            var currentItem = await _uow.GetRepository<BookingContract>().GetAllAsync();
            result.Object = currentItem.FirstOrDefault();
            return result;
        }
        //lamnl CR
        public async Task<ResultDTO> GetBtaRoomHotel(Guid Id)
        {
            var result = new ResultDTO();
            if (Id != null)
            {
                var roomOrganizationDetails = await _uow.GetRepository<RoomOrganization>().FindByAsync<RoomOrganizationDTO>(x => x.BusinessTripApplicationId == Id, "RoomTypeName");
                if (roomOrganizationDetails != null && roomOrganizationDetails.Any())
                {
                    var roomOrgIds = roomOrganizationDetails.Select(x => x.Id);
                    var roomUserMappings = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => roomOrgIds.Contains(x.RoomOrganizationId), "", z => z.User);
                    if (roomUserMappings.Any())
                    {
                        var changeRooms = new List<RoomOrganizationDTO>();

                        foreach (var room in roomOrganizationDetails)
                        {
                            var usersInRoom = roomUserMappings.Where(x => x.RoomOrganizationId == room.Id);
                            if (usersInRoom != null && usersInRoom.Any())
                            {
                                var changeRoom = new RoomOrganizationDTO { RoomTypeId = room.RoomTypeId, RoomTypeCode = room.RoomTypeCode, RoomTypeName = room.RoomTypeName, Id = room.Id };
                                foreach (var item in usersInRoom)
                                {
                                    var user = Mapper.Map<SimpleUserDTO>(item);
                                    room.Users.Add(user);
                                }
                            }
                        }
                        result.Object = roomOrganizationDetails.Where(x => x.Users.Any());
                    }
                }
            }
            else
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            return result;
        }
        public async Task<ResultDTO> DeleteTripGroup(CommonDTO agrs)
        {
            var result = new ResultDTO();
            if (agrs.BtaDetails == null)
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            else
            {
                var record = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(agrs.Id);
                var existBusinessTripDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync(x => x.BusinessTripApplicationId == agrs.Id);

                var businesstripDetailTemp = JsonConvert.DeserializeObject<List<BusinessTripApplicationDetail>>(agrs.BtaDetails);
                var detailTempIds = businesstripDetailTemp.Select(x => x.Id);
                var deletedItems = existBusinessTripDetails.Where(x => detailTempIds.Contains(x.Id)).Select(x => x.Id);
                foreach (var item in businesstripDetailTemp)
                {
                    if (deletedItems.Any())
                    {
                        foreach (var deleteItem in deletedItems)
                        {
                            try
                            {
                                var deletedRecord = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync(x => deletedItems.Contains(x.Id));
                                var flightDetails = await _uow.GetRepository<FlightDetail>().FindByAsync(x => deletedItems.Contains(x.BusinessTripApplicationDetailId.Value));
                                if (flightDetails.Any())
                                {
                                    foreach (var flightDetail in flightDetails)
                                    {
                                        //var deletedflightDetail = await _uow.GetRepository<FlightDetail>().FindByAsync(x => x.Id == flightDetail.Id);
                                        _uow.GetRepository<FlightDetail>().Delete(flightDetail);
                                    }
                                }
                                _uow.GetRepository<BusinessTripApplicationDetail>().Delete(deletedRecord);
                                await _uow.CommitAsync();
                            }
                            catch (Exception ex)
                            {
                                logger.LogError("Error at Delete Trip group", ex.Message);
                                result.ErrorCodes = new List<int> { 1004 };
                                result.Messages = new List<string> { "Error at Delete Trip group" };
                            }
                        }
                    }
                }
                result.Object = Mapper.Map<BTAViewModel>(record);
            }
            return result;
        }
        public async Task<ResultDTO> SaveRoomBookingFlight(Guid id, string roomDetails, int tripGroup, bool isChange = false)
        {
            if (!string.IsNullOrEmpty(roomDetails))
            {
                if (!isChange)
                {
                    var existRoomOrganizationDetails = await _uow.GetRepository<RoomOrganization>().FindByAsync(x => x.BusinessTripApplicationId == id && x.TripGroup == tripGroup);
                    if (existRoomOrganizationDetails != null && existRoomOrganizationDetails.Any())
                    {
                        foreach (var room in existRoomOrganizationDetails)
                        {
                            //tudm
                            var roomOrganizationTemp = JsonConvert.DeserializeObject<List<RoomOrganizationDTO>>(roomDetails);
                            if (roomOrganizationTemp != null && roomOrganizationTemp.Any())
                            {
                                foreach (var item in roomOrganizationTemp)
                                {
                                    var roomUserMappings = new List<RoomUserMapping>();
                                    if (item.Users != null && item.Users.Any())
                                    {
                                        foreach (var user in item.Users)
                                        {
                                            var items = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => x.UserId == user.UserId);
                                            _uow.GetRepository<RoomUserMapping>().Delete(items);
                                        }
                                    }

                                }
                            }
                            //
                            var mappings = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => x.RoomOrganizationId == room.Id);
                            if (mappings != null && mappings.Any())
                            {
                                _uow.GetRepository<RoomUserMapping>().Delete(mappings);
                                _uow.GetRepository<RoomOrganization>().Delete(room);
                            }
                        }
                    }

                }
                else
                {
                    var deleteItemRoomOrganizations = new List<RoomOrganization>();
                    var roomUsers = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => x.RoomOrganization.BusinessTripApplicationId == id && x.IsChange == true);
                    if (roomUsers.Any())
                    {
                        foreach (var item in roomUsers)
                        {
                            var result = deleteItemRoomOrganizations.Find(x => x.Id == item.RoomOrganizationId);
                            if (result == null)
                            {
                                var roomOrganization = new RoomOrganization
                                {
                                    Id = new Guid(item.RoomOrganizationId.ToString())
                                };
                                deleteItemRoomOrganizations.Add(roomOrganization);
                            }
                        }
                        _uow.GetRepository<RoomUserMapping>().Delete(roomUsers);
                    }
                    if (deleteItemRoomOrganizations.Any())
                    {
                        _uow.GetRepository<RoomOrganization>().Delete(deleteItemRoomOrganizations);
                    }
                }
                AddRoomOrganizationDetail(roomDetails, id, isChange);
            }
            await _uow.CommitAsync();
            return new ResultDTO { Object = roomDetails };
        }

        //lamnl: check booking Completed in bta detail
        public ResultDTO CheckBookingCompleted(BtaDTO agrs)
        {
            var result = new ResultDTO();
            var btaDetails = _uow.GetRepository<BusinessTripApplicationDetail>().FindBy<BusinessTripDetailDTO>(x => x.BusinessTripApplicationId == agrs.Id && !x.CheckBookingCompleted);
            if (btaDetails != null && btaDetails.Any())
            {
                result.Object = false;
                return result;
            }
            result.Object = true;
            return result;
        }
        // check user
        public async Task<ResultDTO> CheckAdminDept(BTAAdminDeptDTO agrs)
        {
            var result = new ResultDTO();
            try
            {
                var wfInstance = await _uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.ItemId == agrs.Id, "Created desc");
                if (wfInstance != null)
                {
                    var wfStep = wfInstance.WorkflowData.Steps.FirstOrDefault(x => x.StepName == "Admin Checker");
                    if (wfStep != null)
                    {
                        var lastHistory = await _uow.GetRepository<WorkflowHistory>(true).GetSingleAsync(x => x.InstanceId == wfInstance.Id, "Created desc");

                        Department indexDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.UserDepartmentMappings.Any(t => t.UserId == agrs.UserId && t.IsHeadCount), "", x => x.JobGrade);


                        var jobGrades = await _uow.GetRepository<JobGrade>(true).GetAllAsync();
                        var indexJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
                        var nextStepJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(wfStep.JobGrade));
                        var nextStepMaxJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(wfStep.MaxJobGrade));
                        if (nextStepMaxJobGrade == null) { nextStepMaxJobGrade = nextStepJobGrade; }

                        Department foundDept = await FindMatchedAdminDept(wfStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade);
                        //ReCheck to get HQ if store is not found
                        if (foundDept == null)
                        {
                            //Force to get next department type level if navigate to HQ
                            wfStep.DepartmentType = wfStep.NextDepartmentType;
                            foundDept = await FindMatchedAdminDept(wfStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade, true);
                        }

                        if (foundDept != null)
                        {
                            var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>(true).AnyAsync(x => x.DepartmentId == foundDept.Id && wfStep.DepartmentType == x.Role && x.UserId == agrs.UserId);
                            if (hasParticipants)
                            {
                                result.Object = true;
                            }
                            else
                            {
                                _refDeparmentId = foundDept.Id;
                                var skip = false;
                                while (!skip)
                                {
                                    if (foundDept.ParentId.HasValue)
                                    {
                                        foundDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == foundDept.ParentId);
                                        if (foundDept != null)
                                        {
                                            var foundJobGrade = jobGrades.FirstOrDefault(x => x.Id == foundDept.JobGradeId);
                                            if (foundJobGrade.Grade >= nextStepJobGrade.Grade)
                                            {
                                                if (foundJobGrade.Grade > nextStepMaxJobGrade.Grade)
                                                {
                                                    result.Object = false;
                                                }
                                                //If next step is large than department type, get the next step
                                                if (foundJobGrade.Grade > nextStepJobGrade.Grade)
                                                {
                                                    wfStep.DepartmentType = wfStep.NextDepartmentType;
                                                }
                                                //Check next department type
                                                hasParticipants = await _uow.GetRepository<UserDepartmentMapping>(true).AnyAsync(x => x.DepartmentId == foundDept.Id && wfStep.DepartmentType == x.Role && x.UserId == agrs.UserId);
                                                if (hasParticipants)
                                                {
                                                    result.Object = true;
                                                }
                                            }
                                        }
                                        else { return null; }
                                    }
                                    if (foundDept == null || !foundDept.ParentId.HasValue)
                                    {
                                        result.Object = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        result.Object = false;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Object = false;
                return result;
            }
            return result;
        }
        private async Task<Department> FindMatchedAdminDept(WorkflowStep nextStep, Department indexDept, JobGrade nextStepJobGrade, JobGrade nextStepMaxJobGrade, bool forceHQ = false)
        {
            IEnumerable<Department> hrDepts = new List<Department>();
            if (forceHQ)
            {
                hrDepts = await _uow.GetRepository<Department>(true).FindByAsync(x => x.IsAdmin
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                  && x.IsStore == false, string.Empty, x => x.JobGrade);
            }
            else
            {
                hrDepts = await _uow.GetRepository<Department>(true).FindByAsync(x => x.IsAdmin
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                 && (!nextStep.IsHRHQ || x.IsStore != nextStep.IsHRHQ), string.Empty, x => x.JobGrade);
            }
            return await FindDept(indexDept, forceHQ, hrDepts);
        }
        private async Task<Department> FindDept(Department indexDept, bool forceHQ, IEnumerable<Department> hrDepts)
        {
            var allParentDeptIds = new List<Guid>();
            allParentDeptIds.Add(indexDept.Id);
            while (indexDept != null && indexDept.ParentId.HasValue)
            {
                indexDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == indexDept.ParentId && (x.JobGrade.Grade <= 6 || forceHQ == true));
                if (indexDept != null)
                {
                    if (indexDept.JobGrade.Grade == 6 && !ListUtilities.notInDept.Contains(indexDept.Code))
                    {
                        continue;
                    }
                    allParentDeptIds.Add(indexDept.Id);
                }
            }

            Department foundDept = null;
            var lastIdx = 10;
            hrDepts = hrDepts.OrderBy(x => x.JobGrade.Grade).ToList();
            foreach (var hrDept in hrDepts)
            {
                var skip = false;
                var parentDept = hrDept;
                while (!skip)
                {
                    if (parentDept.ParentId.HasValue)
                    {
                        parentDept = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == parentDept.ParentId);
                        if (parentDept != null && allParentDeptIds.Contains(parentDept.Id))
                        {
                            if (allParentDeptIds.IndexOf(parentDept.Id) < lastIdx)
                            {
                                lastIdx = allParentDeptIds.IndexOf(parentDept.Id);
                                foundDept = hrDept;
                            }
                        }
                    }
                    if (parentDept == null || !parentDept.ParentId.HasValue)
                    {
                        skip = true;
                    }
                }
            }

            return foundDept;
        }

        // update details
        public async Task<ResultDTO> SaveBTADetail(BusinessTripDTO data)
        {
            var result = new ResultDTO();

            var record = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(data.Id.Value);
            if (record != null)
            {
                record.CheckRoomNextStep = data.CheckRoomNextStep;
                record.BookingContact = data.BookingContact;
                var existBusinessTripDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync(x => x.BusinessTripApplicationId == data.Id);

                var businesstripDetailTemp = JsonConvert.DeserializeObject<List<BusinessTripApplicationDetail>>(data.BusinessTripDetails);
                var detailTempIds = businesstripDetailTemp.Select(x => x.Id);
                var updatedItems = new List<BusinessTripApplicationDetail>();
                var addItems = new List<BusinessTripApplicationDetail>();
                if (businesstripDetailTemp != null && businesstripDetailTemp.Any())
                {
                    foreach (var item in businesstripDetailTemp)
                    {
                        var bta_DetailItem = existBusinessTripDetails.FirstOrDefault(x => x.Id == item.Id);

                        if (bta_DetailItem != null && bta_DetailItem.Id != null)
                        {
                            List<string> ignoreProperties = new List<string>() { "Id", "BusinessTripApplication", "Department", "User", "TripGroup", "Comments" };
                            if (!string.IsNullOrEmpty(bta_DetailItem.Comments))
                            {
                                ignoreProperties.Remove("Comments");
                            }
                            if (bta_DetailItem.TripGroup > 0)
                            {
                                ignoreProperties.Remove("TripGroup");
                            }
                            bta_DetailItem = item.TransformValues(bta_DetailItem, ignoreProperties);
                            bta_DetailItem.BusinessTripApplicationId = data.Id.Value;
                            updatedItems.Add(bta_DetailItem);
                        }
                        else
                        {
                            addItems.Add(bta_DetailItem);
                        }
                    }
                    if (updatedItems.Any())
                    {
                        _uow.GetRepository<BusinessTripApplicationDetail>().Update(updatedItems);
                    }

                }
                result.Object = Mapper.Map<BTAViewModel>(record);
            }
            else
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            _uow.Commit();

            return result;
        }
        public async Task<ResultDTO> UpdateBeforeCommitBookingForBTADetail(List<BusinessTripApplicationDetail> datas, string bookingNumber, string bookingCode, bool isCommit)
        {
            var result = new ResultDTO();
            try
            {
                foreach (var item in datas)
                {
                    var existBusinessTripDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByIdAsync(item.Id);

                    existBusinessTripDetails.IsCommitBooking = isCommit;
                    existBusinessTripDetails.BookingNumber = bookingNumber;
                    existBusinessTripDetails.BookingCode = bookingCode;
                    _uow.GetRepository<BusinessTripApplicationDetail>().Update(existBusinessTripDetails);
                    _uow.Commit();
                }
                result.Object = true;
            }
            catch (Exception ex)
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }

            return result;
        }
        public async Task<bool> CheckCommitBookingForBTADetail(List<BusinessTripApplicationDetail> datas)
        {
            foreach (var item in datas)
            {
                var existBusinessTripDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByIdAsync(item.Id);

                if (existBusinessTripDetails.IsCommitBooking)
                    return false;
            }

            return true;
        }
        public async Task<bool> CheckHasBookingNumberForBTADetail(List<BusinessTripApplicationDetail> datas)
        {
            foreach (var item in datas)
            {
                var existBusinessTripDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByIdAsync(item.Id);

                if (string.IsNullOrEmpty(existBusinessTripDetails.BookingNumber))
                    return false;
            }

            return true;
        }
        public bool SaveBTALog(Guid? btaID, string message)
        {
            try
            {
                var record = new BTALog
                {
                    BusinessTripApplicationId = btaID,
                    Message = message
                };
                _uow.GetRepository<BTALog>().Add(record);
                _uow.Commit();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public async Task<ResultDTO> SendEmailDeleteRows(BtaDTO agrs)
        {
            var result = new ResultDTO();
            if (agrs.UserId != null)
            {
                try
                {
                    var current = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(agrs.Id);
                    if (current != null)
                    {
                        EmailNotification emailNotification = new EmailNotification(logger, _uow);
                        EmailTemplateName type = EmailTemplateName.BTASendEmailWhenDeleteBTADetails;
                        User user = await _uow.GetRepository<User>().FindByIdAsync(agrs.UserId);
                        logger.LogInformation($"Send email notification to {user.Email}");
                        var mergeFields = new Dictionary<string, string>();
                        mergeFields["BusinessTripApplicationNumber"] = current.ReferenceNumber;
                        mergeFields["ListUsers"] = GetListUserBTADetails(current.Id).ToString();
                        var recipients = GetListUserApproved(current.Id);
                        await emailNotification.SendEmail(type, EmailTemplateName.BTASendEmailWhenDeleteBTADetails, mergeFields, recipients);

                    }
                    else
                    {
                        logger.LogError($"BTA is not found");
                        result.Messages.Add($"BTA {agrs.Id} is not found");
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError($"Send email notification to approval is fail: {ex.Message}");
                    result.Messages.Add(ex.Message);
                }
            }
            return result;
        }

        public List<string> GetListUserApproved(Guid? id)
        {
            var result = new List<string>();
            try
            {
                var wfInstance = _uow.GetRepository<WorkflowInstance>(true).GetSingle(x => x.ItemId == id, "Created desc");
                if (wfInstance != null)
                {
                    var wfIn = Mapper.Map<WorkflowInstanceViewModel>(wfInstance);
                    var lastHistory = _uow.GetRepository<WorkflowHistory>(true).FindBy(x => x.InstanceId == wfIn.Id && (x.StepNumber == 2 || x.StepNumber == 3), "Created desc");
                    foreach (var hisDetail in lastHistory)
                    {
                        var user = _uow.GetRepository<User>().FindById(hisDetail.ApproverId.Value);
                        result.Add(user.Email);
                    }

                }

            }
            catch (Exception ex)
            {
                result = new List<string>();
            }
            return result;


        }
        public List<string> GetListUserBTADetails(Guid? id)
        {
            var result = new List<string>();
            try
            {
                var btaDetails = _uow.GetRepository<BusinessTripApplicationDetail>().FindBy<BusinessTripDetailDTO>(x => x.BusinessTripApplicationId == id);
                if (btaDetails.Any())
                {
                    foreach (var btaDetail in btaDetails)
                    {
                        result.Add(btaDetail.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<string>();
            }
            return result;


        }

        //lamnl check room hotel
        public async Task<ResultDTO> CheckRoomHotel(BtaDTO Agrs)
        {
            var result = new ResultDTO();
            try
            {
                var users = new List<SimpleUserDTO>();
                var record = await _uow.GetRepository<BusinessTripApplication>().FindByIdAsync(Agrs.Id);
                var roomOrganizationDetails = await _uow.GetRepository<RoomOrganization>().FindByAsync<RoomOrganizationDTO>(x => x.BusinessTripApplicationId == record.Id, "RoomTypeName");
                if (roomOrganizationDetails != null && roomOrganizationDetails.Any())
                {
                    var roomOrgIds = roomOrganizationDetails.Select(x => x.Id);
                    var roomUserMappings = await _uow.GetRepository<RoomUserMapping>().FindByAsync(x => roomOrgIds.Contains(x.RoomOrganizationId), "", z => z.User);
                    if (roomUserMappings.Any())
                    {
                        var changeRooms = new List<RoomOrganizationDTO>();

                        foreach (var room in roomOrganizationDetails)
                        {
                            var usersInRoom = roomUserMappings.Where(x => x.RoomOrganizationId == room.Id);
                            if (usersInRoom != null && usersInRoom.Any())
                            {
                                var changeRoom = new RoomOrganizationDTO { RoomTypeId = room.RoomTypeId, RoomTypeCode = room.RoomTypeCode, RoomTypeName = room.RoomTypeName, Id = room.Id };
                                foreach (var item in usersInRoom)
                                {
                                    var user = Mapper.Map<SimpleUserDTO>(item);
                                    users.Add(user);
                                }
                            }
                        }
                    }
                }
                var details = _uow.GetRepository<BusinessTripApplicationDetail>().FindBy<BusinessTripDetailDTO>(x => x.BusinessTripApplicationId == record.Id && x.StayHotel, string.Empty).ToList();
                if (details != null)
                {
                    foreach (var detail in details)
                    {
                        var checkRoom = users.FindAll(x => x.UserId == detail.UserId);
                        if (checkRoom == null || !checkRoom.Any())
                        {
                            record.CheckRoomNextStep = false;
                            _uow.Commit();
                            return new ResultDTO { Object = false };
                        }
                    }

                }
                else
                {
                    record.CheckRoomNextStep = true;
                    _uow.Commit();
                    return new ResultDTO { Object = true };
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Check Room Hotel not found: {ex.Message}");
                result.Messages.Add(ex.Message);
            }
            return result;
        }

        public async Task<ResultDTO> GetFareRulesByFareSourceCode(ViewModels.Args.FareRulesRequestArgs args)
        {
            var result = new ResultDTO();
            var fareRuleViewModel = new CommonViewModel.BusinessTripApplication.FareRuleViewModel();
            try
            {
                // view business trip application
                fareRuleViewModel = await _uow.GetRepository<FlightDetail>().GetSingleAsync<CommonViewModel.BusinessTripApplication.FareRuleViewModel>(x => x.GroupId == args.GroupId && x.FareSourceCode == x.FareSourceCode && x.SearchId == x.SearchId, "created desc");
                //if (fareRuleViewModel == null || (fareRuleViewModel != null && (string.IsNullOrEmpty(fareRuleViewModel.Title) || string.IsNullOrEmpty(fareRuleViewModel.Detail)))) errorFlag = true;

                // show business trip over budget
                if (fareRuleViewModel == null || (fareRuleViewModel != null && (string.IsNullOrEmpty(fareRuleViewModel.Title) || string.IsNullOrEmpty(fareRuleViewModel.Detail))))
                {
                    fareRuleViewModel = new CommonViewModel.BusinessTripApplication.FareRuleViewModel();
                    var overBG = await _uow.GetRepository<BusinessTripOverBudgetsDetail>().GetSingleAsync(x => (x.DepartureSearchId == args.SearchId) || (x.ReturnSearchId == args.SearchId));
                    if (overBG != null)
                    {
                        if (overBG.DepartureSearchId != null && args.SearchId == overBG.DepartureSearchId)
                        {
                            fareRuleViewModel.Title = overBG.TitleDepartureFareRule;
                            fareRuleViewModel.Detail = overBG.DetailDepartureFareRule;
                        }
                        else if (overBG.ReturnSearchId != null && args.SearchId == overBG.ReturnSearchId)
                            fareRuleViewModel.Title = overBG.TitleReturnFareRule;
                        fareRuleViewModel.Detail = overBG.DetailDepartureFareRule;
                    }
                }
                result.Object = fareRuleViewModel;

                if (fareRuleViewModel == null || (fareRuleViewModel != null && (string.IsNullOrEmpty(fareRuleViewModel.Title) || string.IsNullOrEmpty(fareRuleViewModel.Detail)))) result.ErrorCodes.Add(0);
            }
            catch (Exception ex)
            {
                result.ErrorCodes.Add(0);
                result.Messages.Add(ex.Message);
            }
            return result;
        }

        public async Task<string> GetBTAApprovedDay(Guid businessTripApplicationId)
        {
            var days = -1;
            try
            {
                var bta = _uow.GetRepository<BusinessTripApplication>().FindById(businessTripApplicationId);
                if (bta != null)
                {
                    var btadetails = _uow.GetRepository<BusinessTripApplicationDetail>().GetSingle(x => x.BusinessTripApplicationId == businessTripApplicationId, "FromDate asc");
                    if (btadetails != null)
                    {
                        var wfTasks = _uow.GetRepository<WorkflowTask>(true).GetSingle(x => x.ItemId == businessTripApplicationId
                                                                                 && x.Status.ToLower().Equals("waiting for booking flight"), "created desc");
                        if (wfTasks != null)
                        {
                            DateTimeOffset FromConverted = btadetails.FromDate.Value.ToOffset(wfTasks.Created.Offset);
                            days = (int)((TimeSpan)(FromConverted.Date - wfTasks.Created.Date)).Days;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetBTAApprovedDay " + ex.Message);
            }
            
            var res = days == -1 ? "" : days.ToString();
            return res;
        }


        public async Task<ResultDTO> ValidationBTADetails(ViewModels.Args.ValidateBTADetailsArgs btaArgs)
        {
            var resultDTO = new ResultDTO() { };
            if (btaArgs != null && btaArgs.BTADetails != null)
            {
                #region Validate BTA has ticket
                foreach(var btaDetailModel in btaArgs.BTADetails)
                {
                    var findAllDetailIsBooked = await _uow.GetRepository<BusinessTripApplicationDetail>(true).GetSingleAsync(x => btaDetailModel.Id.Equals(x.Id)
                    && x.BookingNumber.Equals(btaDetailModel.BookingNumber)
                    && x.CheckBookingCompleted == btaDetailModel.CheckBookingCompleted
                    && x.IsCommitBooking == btaDetailModel.IsCommitBooking);
                    if (findAllDetailIsBooked is null)
                    {
                        resultDTO.ErrorCodes = new List<int>() { -1 };
                        resultDTO.Messages = new List<string>() { "BTA_ALREADY_CHANGE" };
                        goto Finish;
                    }
                }
                #endregion
            }
        Finish:
            return resultDTO;
        }
    }
}
