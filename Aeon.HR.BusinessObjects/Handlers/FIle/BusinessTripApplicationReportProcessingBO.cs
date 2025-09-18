using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class BusinessTripApplicationReportProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "BusinessTripApplicationReport";
        private ReportType Type { get; set; }
        private string[] HotelInvisibleColumns = new string[] { "OnTime", "HasBudget", "FlightNumber", "Status", "HotelName", "ReferenceNumber", "Reason", "Mobile", "Email", "FlightNumberName", "Arrival", "Departure", "IsCancel", "DeptLine" };
        private string[] FlightInvisibleColumns = new string[] { "OnTime", "HasBudget", "Hotel", "Status", "HotelName", "TotalDays", "RoomType", "Reason", "ReferenceNumber", "FlightNumberName", "CheckInHotelDate", "CheckOutHotelDate", "IsCancel", "Arrival", "Departure", "IsCancel", "DepartFlight", "ReturnFlight", "DeptLine" };
        private string[] StatusInvisibleColumns = new string[] { "Hotel", "FlightNumber", "TotalDays", "IDCard", "Passport", "DepartFlight", "ReturnFlight" };
        private ILogger logger;
        public BusinessTripApplicationReportProcessingBO(IUnitOfWork uow, ILogger _logger) : base(uow)
        {
            logger = _logger;
        }

        public async Task<ResultDTO> ExportReport(Aeon.HR.ViewModels.Args.ExportReportArg args)
        {
            var fieldMappings = ReadConfigurationFromFile();


            var allBusinessTrips = await _uow.GetRepository<BusinessTripApplicationDetail>().GetAllAsync<BtaDetailViewModel>();
            if (Type == ReportType.Flight || Type == ReportType.Hotel)
            {
                if (Type == ReportType.Hotel)
                {
                    fieldMappings = fieldMappings.Where(x => !HotelInvisibleColumns.Contains(x.Name)).ToList();
                }
                else
                {
                    fieldMappings = fieldMappings.Where(x => !FlightInvisibleColumns.Contains(x.Name)).ToList();
                }
                allBusinessTrips = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync<BtaDetailViewModel>(x => x.BusinessTripApplication.Status == "Completed" || x.BusinessTripApplication.Status == "Completed Changing");
            }
            else
            {
                fieldMappings = fieldMappings.Where(x => !StatusInvisibleColumns.Contains(x.Name)).ToList();
            }
            var headers = fieldMappings.Select(y => y.DisplayName);
            DataTable tbl = new DataTable();
            foreach (var headerItem in headers)
            {
                tbl.Columns.Add(headerItem);
            }
            if (allBusinessTrips != null && allBusinessTrips.Any())
            {
                var allChangeBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => !x.IsCancel);
                var allCancelBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => x.IsCancel);
                var allRooms = await _uow.GetRepository<RoomOrganization>().GetAllAsync(string.Empty, x => x.RoomUserMappings);
                if (args.Codes != null)
                {
                    allRooms = allRooms.Where(i => args.Codes != null && (args.Codes.Length == 0 || args.Codes.Contains(i.RoomTypeName)));
                }
                var BTAItems = allBusinessTrips;
                allBusinessTrips = allBusinessTrips.Where(x => !allCancelBTAItems.Any(y => y.BusinessTripApplicationDetailId == x.Id)).OrderByDescending(X => X.ReferenceNumber);
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
                            item.Reason = changedItem.Reason;
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
                if (allBusinessTrips.Any())
                {
                    var exportReportViewModels = new List<ExportBusinessTripApplicationViewModel>();
                    var hotels = await _uow.GetRepository<Hotel>().GetAllAsync<HotelViewModel>(string.Empty);
                    if (Type == ReportType.Hotel)
                    {
                        allBusinessTrips = allBusinessTrips.AsQueryable().Where(args.Predicate, args.PredicateParameters);
                        if (allBusinessTrips.Any())
                        {
                            allBusinessTrips = allBusinessTrips.Where(x => allRooms.Any(y => y.RoomUserMappings.Any(z => z.BusinessTripApplicationDetailId == x.Id))).OrderBy(x => x.HotelName);
                        }
                    }
                    else if (Type == ReportType.Flight)
                    {
                        //allBusinessTrips = allBusinessTrips.AsQueryable().Where(args.Predicate, args.PredicateParameters);
                        //allBusinessTrips = allBusinessTrips.Where(x => !string.IsNullOrEmpty(x.FlightNumberCode)).OrderBy(x => x.FlightNumberName);
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
                        allBusinessTrips = list.Select(x=>x.Trim()).AsQueryable().Where(args.Predicate, args.PredicateParameters);
                        //
                    }
                    else
                    {
                        allBusinessTrips.ToList().ForEach(x => x.StatusItem = "In Progress");
                        IEnumerable<BtaDetailViewModel> BTAChangeCancel = null;
                        if (args.PredicateParameters.Contains(RuleValidate.Changing))
                        {
                            var ChangeCancel = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().GetAllAsync();
                            BTAChangeCancel = BTAItems.Where(x => ChangeCancel.Any(s => s.BusinessTripApplicationDetailId == x.Id));
                            foreach (var change in BTAChangeCancel)
                            {
                                change.StatusItem = RuleValidate.Changing;
                                change.Reason = GetResonFromChanging(change.Id);
                            }
                            allBusinessTrips.Concat(BTAChangeCancel);
                        }

                        allBusinessTrips = allBusinessTrips.AsQueryable().Where(args.Predicate, args.PredicateParameters).OrderBy(x => x.ReferenceNumber);
                    }
                    if (allBusinessTrips != null && allBusinessTrips.Any())
                    {
                        var allWorkflowInstances = await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.WorkflowName.Contains("BTA"));
                        foreach (var detail in allBusinessTrips)
                        {
                            var hotelDetail = GetHotelByCode(detail.HotelCode, hotels);
                            var roomType = allRooms.FirstOrDefault(x => x.RoomUserMappings.Any(y => y.User.SAPCode == detail.SAPCode && y.BusinessTripApplicationDetailId == detail.Id));
                            var itemHaveChange = allChangeBTAItems.FirstOrDefault(x => x.UserId == detail.UserId && x.BusinessTripApplicationDetailId == detail.Id);
                            if (itemHaveChange != null)
                            {
                                roomType = allRooms.FirstOrDefault(x => x.RoomUserMappings.Any(y => y.User.SAPCode == detail.SAPCode && y.ChangeCancelBusinessTripApplicationDetailId == itemHaveChange.Id));
                            }
                            double totalDays = 0;
                            if (detail.CheckOutHotelDate.HasValue && detail.CheckInHotelDate.HasValue)
                            {
                                totalDays = (detail.CheckOutHotelDate.Value - detail.CheckInHotelDate.Value).TotalDays;
                            }
                            exportReportViewModels.Add(new ExportBusinessTripApplicationViewModel
                            {
                                Hotel = string.Format("{0}-{1}-{2}", detail.HotelName, hotelDetail?.Address, hotelDetail?.Telephone),
                                DepartmentName = detail.DepartmentName,
                                FlightNumber = detail.FlightNumberName,
                                FullName = detail.FullName,
                                Reason = detail.StatusItem != RuleValidate.Changing ? GetReasonFromItem(detail.BusinessTripApplicationId, allWorkflowInstances) : detail.Reason,
                                Email = detail.Email,
                                Mobile = detail.Mobile,
                                SAPCode = detail.SAPCode,
                                Status = detail.StatusItem,
                                FromDate = detail.FromDate.HasValue ? detail.FromDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : string.Empty,
                                ToDate = detail.ToDate.HasValue ? detail.ToDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : string.Empty,
                                HotelName = detail.HotelName,
                                //TotalDays = detail.CheckOutHotelDate.HasValue && detail.CheckInHotelDate.HasValue ? int.Parse(Math.Floor((detail.CheckOutHotelDate.Value - detail.CheckInHotelDate.Value).TotalDays).ToString()) : 0,
                                TotalDays = totalDays != 0 ? GetTotalDateFromDoubleDate(totalDays) : totalDays,
                                RoomType = roomType?.RoomTypeName,
                                ReferenceNumber = detail.ReferenceNumber,
                                FlightNumberName = detail.FlightNumberName,
                                Arrival = detail.AirlineName,
                                Departure = detail.DepartureName,
                                CheckInHotelDate = detail.CheckInHotelDate.HasValue ? detail.CheckInHotelDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : string.Empty,
                                CheckOutHotelDate = detail.CheckOutHotelDate.HasValue ? detail.CheckOutHotelDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : string.Empty,
                                IDCard = detail.IDCard,
                                Passport = detail.Passport,
                                DepartFlight = detail.FlightNumberName,
                                ReturnFlight = detail.ComebackFlightNumberName
                            });
                        }
                        for (int rowNum = 0; rowNum < exportReportViewModels.Count(); rowNum++)
                        {
                            DataRow row = tbl.Rows.Add();
                            var data = exportReportViewModels.ElementAt(rowNum);
                            for (int j = 0; j < fieldMappings.Count; j++)
                            {
                                try
                                {
                                    var fieldMapping = fieldMappings[j];
                                    var value = data.GetType().GetProperty(fieldMapping.Name).GetValue(data);
                                    HandleCommonType(row, value, j, fieldMapping);
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError($"Error at binding data for exporting: {ex.InnerException?.Message}");
                                }

                            }
                        }
                    }

                }

            }
            var creatingExcelFileReslult = ExportExcel(tbl);
            if (creatingExcelFileReslult == null)
            {
                return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
            }
            return new ResultDTO { Object = creatingExcelFileReslult };
        }
        private bool CheckIsRevokingItem(Guid itemId)
        {
            var isRevoking = false;
            var currentWfs = _uow.GetRepository<WorkflowInstance>().FindBy(x => !x.IsCompleted && x.WorkflowName.Contains("Revoke") && x.ItemId == itemId).OrderByDescending(x => x.Created);
            if (currentWfs.Any())
            {
                var lastWfs = currentWfs.FirstOrDefault();
                if (!lastWfs.IsCompleted)
                {
                    isRevoking = true;
                }
            }
            return isRevoking;
        }
        public async Task<ResultDTO> ExportTypeStatus(ViewModels.Args.ExportReportArg args)
        {
            var fieldMappings = ReadConfigurationFromFile().Where(x => !StatusInvisibleColumns.Contains(x.Name)).ToList();
            var detailStatusItems = new List<BtaDetailViewModel>();
            var headers = fieldMappings.Select(y => y.DisplayName);
            DataTable tbl = new DataTable();
            foreach (var headerItem in headers)
            {
                tbl.Columns.Add(headerItem);
            }
            //code moi 12/10
            var allRooms = await _uow.GetRepository<RoomOrganization>().GetAllAsync(string.Empty, x => x.RoomUserMappings);
            //
            var allBusinessTrips = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync<BtaDetailViewModel>(args.Predicate, args.PredicateParameters);

            var btaIds = allBusinessTrips.Select(x => x.BusinessTripApplicationId).ToList();
            //Dictionary<Guid, bool?> onTimeBTA = await this.AddOnTimeProperties(btaIds);
            var allCancelBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().GetAllAsync();
            var wfInstances = await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.WorkflowName.Contains("BTA"));
            foreach (var x in allBusinessTrips)
            {
                var isChanging = allCancelBTAItems.FirstOrDefault(i => i.BusinessTripApplicationDetailId == x.Id && !i.IsCancel);
                if (await CheckIsChangingCancellingItem(x.BusinessTripApplicationId))
                {
                    x.StatusItem = RuleValidate.Changing;
                }
                else if (x.StatusItem.Contains("Waiting"))
                {
                    var isRevoking = CheckIsRevokingItem(x.BusinessTripApplicationId);
                    if (isRevoking)
                    {
                        x.StatusItem = "Revoking";
                        //x.Reason = GetResonFromRevoke(x.BusinessTripApplicationId, wfInstances);
                        x.Reason = GetRequestorNoteFromBusinessTripApplicationId(x.BusinessTripApplicationId);
                        x.DetailReason = GetRequestorNoteDetailFromBusinessTripApplicationId(x.BusinessTripApplicationId);
                    }
                    else
                    {
                        x.StatusItem = "In Progress";
                        //x.Reason = GetReasonFromItem(x.BusinessTripApplicationId, wfInstances);
                        x.Reason = GetRequestorNoteFromBusinessTripApplicationId(x.BusinessTripApplicationId);
                        x.DetailReason = GetRequestorNoteDetailFromBusinessTripApplicationId(x.BusinessTripApplicationId);
                    }
                }
                else if (isChanging == null)
                {
                    //x.Reason = GetReasonFromItem(x.BusinessTripApplicationId, wfInstances);
                    x.Reason = GetRequestorNoteFromBusinessTripApplicationId(x.BusinessTripApplicationId);
                    x.DetailReason = GetRequestorNoteDetailFromBusinessTripApplicationId(x.BusinessTripApplicationId);
                }
                if (isChanging != null)
                {
                    x.FromDate = (isChanging.NewFromDate != null && isChanging.NewFromDate.HasValue) ? isChanging.NewFromDate : isChanging.FromDate;
                    x.ToDate = (isChanging.NewToDate != null && isChanging.NewToDate.HasValue) ? isChanging.NewToDate : isChanging.ToDate;
                    x.HotelName = isChanging.NewHotelName;
                    x.FlightNumberName = isChanging.NewFlightNumberName;
                    x.Reason = isChanging?.Reason;
                    x.CheckInHotelDate = isChanging.NewCheckInHotelDate;
                    x.CheckOutHotelDate = isChanging.NewCheckOutHotelDate;
                    x.ArrivalName = isChanging.DestinationName;
                }
                else
                {
                    var isCancelling = allCancelBTAItems.FirstOrDefault(i => i.BusinessTripApplicationDetailId == x.Id && i.IsCancel);
                    if (isCancelling != null)
                    {
                        x.Reason = isCancelling?.Reason;
                    }
                }

                var ApprovedDays = GetBTAApprovedDay(x.BusinessTripApplicationId);
                if(ApprovedDays != null && ApprovedDays.HasValue && ApprovedDays.Value >= 0)
                    x.BTAApprovedDay = ApprovedDays.ToString();

            }

            //var export = allBusinessTrips.Where(i => args.Codes.Contains(i.StatusItem)).Select(detail => new ExportBusinessTripApplicationViewModel()
            //{
            //    DepartmentName = detail.DepartmentName,
            //    FlightNumber = detail.FlightNumberName,
            //    FullName = detail.FullName,
            //    SAPCode = detail.SAPCode,
            //    Status = detail.StatusItem,
            //    FromDate = detail.FromDate.HasValue ? detail.FromDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
            //    ToDate = detail.ToDate.HasValue ? detail.ToDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
            //    HotelName = detail.HotelName,
            //    ReferenceNumber = detail.ReferenceNumber,
            //    FlightNumberName = detail.FlightNumberName,
            //    Reason = detail.Reason,
            //    Arrival = detail.ArrivalName,
            //    Departure = detail.DepartureName,
            //    Email = detail.Email,
            //    Mobile = detail.Mobile,
            //    CheckInHotelDate = detail.CheckInHotelDate.HasValue ? detail.CheckInHotelDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
            //    CheckOutHotelDate = detail.CheckOutHotelDate.HasValue ? detail.CheckOutHotelDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
            //    IsCancel = allCancelBTAItems.Any(i => i.BusinessTripApplicationDetailId == detail.Id && i.IsCancel)
            //}).OrderBy(x => x.ReferenceNumber).ToList();

            //code moi 12/10 - them cot roomType
            var export = new List<ExportBusinessTripApplicationViewModel>();
            if(allBusinessTrips.Any())
            {
                allBusinessTrips = allBusinessTrips.Where(i => args.Codes.Contains(i.StatusItem));
                foreach (var detail in allBusinessTrips)
                {
                    var roomType = allRooms.FirstOrDefault(x => x.RoomUserMappings.Any(y => y.User.SAPCode == detail.SAPCode && y.BusinessTripApplicationDetailId == detail.Id));
                    var itemHaveChange = allCancelBTAItems.FirstOrDefault(x => x.UserId == detail.UserId && x.BusinessTripApplicationDetailId == detail.Id);
                    if (itemHaveChange != null)
                    {
                        roomType = allRooms.FirstOrDefault(x => x.RoomUserMappings.Any(y => y.User.SAPCode == detail.SAPCode && y.ChangeCancelBusinessTripApplicationDetailId == itemHaveChange.Id));
                    }

                    var BTAApplication = _uow.GetRepository<BusinessTripApplication>().FindById(detail.BusinessTripApplicationId);
                    if(BTAApplication != null)
                    {
                        detail.DeptLine = BTAApplication.DeptLine.Name;
                    }

                    var dataExport = new ExportBusinessTripApplicationViewModel
                    {
                        DepartmentName = detail.DepartmentName,
                        FlightNumber = detail.FlightNumberName,
                        FullName = detail.FullName,
                        SAPCode = detail.SAPCode,
                        Status = detail.StatusItem,
                        FromDate = detail.FromDate.HasValue ? detail.FromDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
                        ToDate = detail.ToDate.HasValue ? detail.ToDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
                        HotelName = detail.HotelName,
                        ReferenceNumber = detail.ReferenceNumber,
                        FlightNumberName = detail.FlightNumberName,
                        Reason = detail.Reason,
                        DetailReason = detail.DetailReason,
                        Arrival = detail.ArrivalName,
                        Departure = detail.DepartureName,
                        Email = detail.Email,
                        Mobile = detail.Mobile,
                        CheckInHotelDate = detail.CheckInHotelDate.HasValue ? detail.CheckInHotelDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
                        CheckOutHotelDate = detail.CheckOutHotelDate.HasValue ? detail.CheckOutHotelDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
                        IsCancel = allCancelBTAItems.Any(i => i.BusinessTripApplicationDetailId == detail.Id && i.IsCancel),
                        RoomType = roomType?.RoomTypeName,
                        DeptLine = detail.DeptLine != null ? detail.DeptLine : "",
                        BTAApprovedDay = detail.BTAApprovedDay
                    };

                    if (detail.HasBudget != null && detail.HasBudget.HasValue)
                        dataExport.HasBudget = detail.HasBudget.Value ? "Budget" : "Non-Budget";

                    // bool? onTime = onTimeBTA.Where(x => x.Key == detail.BusinessTripApplicationId).Select(y => y.Value).FirstOrDefault();
                    dataExport.OnTime = !string.IsNullOrEmpty(dataExport.BTAApprovedDay) ? (int.Parse(detail.BTAApprovedDay) >= 10 ? "True" : "False") : "";
                    if (BTAApplication != null)
                    {
                        switch (BTAApplication.Type)
                        {
                            case BTAType.Domestic:
                                dataExport.BusinessTripType = "Domestic Business Trip (No Flight Booking)";
                                break;
                            case BTAType.DomesticWithFlight:
                                dataExport.BusinessTripType = "Domestic Business Trip by Plane";
                                break;
                            case BTAType.International:
                                dataExport.BusinessTripType = "International Business Trip";
                                break;
                            case BTAType.InternationalNoFlight:
                                dataExport.BusinessTripType = "International Business Trip (No Flight Booking)";
                                break;
                        }
                    }
                    export.Add(dataExport);
                }
                export = export.OrderBy(x => x.ReferenceNumber).ToList();
            }
            //

            for (int rowNum = 0; rowNum < export.Count(); rowNum++)
            {
                DataRow row = tbl.Rows.Add();
                var data = export.ElementAt(rowNum);
                for (int j = 0; j < fieldMappings.Count; j++)
                {
                    try
                    {
                        var fieldMapping = fieldMappings[j];
                        var value = data.GetType().GetProperty(fieldMapping.Name).GetValue(data);
                        HandleCommonType(row, value, j, fieldMapping);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error at binding data for exporting: {ex.InnerException?.Message}");
                    }

                }
            }
            var creatingExcelFileReslult = ExportExcel(tbl);

            if (creatingExcelFileReslult == null)
            {
                return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
            }
            return new ResultDTO { Object = creatingExcelFileReslult };
        }

        public void SetAdditionalInfo(ReportType type)
        {
            Type = type;
        }

        public Task<ResultDTO> ImportAsync(FileStream stream)
        {
            throw new NotImplementedException();
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
        private string GetReasonFromItem(Guid itemId, IEnumerable<WorkflowInstance> workflowInstances)
        {
            var reason = "";
            try
            {
                var rd = workflowInstances.Where(x => x.ItemId == itemId && x.IsCompleted).FirstOrDefault();
                if (rd != null)
                {
                    var currentWf = Mapper.Map<WorkflowInstanceViewModel>(rd);
                    var lastHistory = currentWf.Histories.OrderByDescending(y => y.Created).FirstOrDefault();
                    if (lastHistory != null)
                    {
                        reason = lastHistory.Comment;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetReasonFromItem " + ex.Message);
            }
            return reason;
        }

        private string GetRequestorNoteFromBusinessTripApplicationId(Guid businessTripApplicationId)
        {
            var reason = "";
            try
            {
                var bta = _uow.GetRepository<BusinessTripApplication>().FindById(businessTripApplicationId);
                if (bta != null)
                {
                    reason = bta.RequestorNote;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetRequestorNoteFromBusinessTripApplicationId " + ex.Message);
            }
            return reason;
        }
        private string GetRequestorNoteDetailFromBusinessTripApplicationId(Guid businessTripApplicationId)
        {
            var reason = "";
            try
            {
                var bta = _uow.GetRepository<BusinessTripApplication>().FindById(businessTripApplicationId);
                if (bta != null)
                {
                    reason = bta.RequestorNoteDetail;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetRequestorNoteDetailFromBusinessTripApplicationId " + ex.Message);
            }
            return reason;
        }
        private double? GetBTAApprovedDay(Guid businessTripApplicationId)
        {
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
                            double? countDay = ((TimeSpan) (FromConverted.Date - wfTasks.Created.Date)).Days;
                            return (countDay > 0 ? countDay : null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetBTAApprovedDay " + ex.Message);
            }
            return null;
        }
        private string GetResonFromRevoke(Guid itemId, IEnumerable<WorkflowInstance> workflowInstances)
        {
            var reason = "";
            try
            {
                var rd = workflowInstances.Where(i => i.ItemId == itemId && i.WorkflowName.Contains("BTA Revoke"))?.FirstOrDefault();
                if (rd != null)
                {
                    reason = rd.Histories.OrderByDescending(o => o.Created).FirstOrDefault(i => i.Outcome == "Revoked")?.Comment;
                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetReasonFromItem " + ex.Message);
            }
            return reason;
        }
        private string GetResonFromChanging(Guid BTADetailId)
        {
            var reason = "";
            try
            {
                var rd = _uow.GetRepository<ChangeCancelBusinessTripDetail>().GetSingle(i => i.BusinessTripApplicationDetailId == BTADetailId);
                if (rd != null)
                {
                    reason = rd?.Reason;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at GetResonFromChanging " + ex.Message);
            }
            return reason;
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

        public Task<ResultDTO> ExportAsync(ViewModels.Args.QueryArgs parameters)
        {
            throw new NotImplementedException();
        }
    }
}
