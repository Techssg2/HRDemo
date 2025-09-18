using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class BusinessTripApplicationProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "BusinessTripApplication";
        public BusinessTripApplicationProcessingBO(IUnitOfWork uow) : base(uow)
        {

        }

        public Task<ResultDTO> ImportAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }
        public async Task<ResultDTO> ExportAsync(QueryArgs parameters)
        {
            var fieldMappings = ReadConfigurationFromFile();
            var headers = fieldMappings.Select(y => y.DisplayName);
            // Create Headers
            DataTable tbl = new DataTable();
            foreach (var headerItem in headers)
            {
                tbl.Columns.Add(headerItem);
            }
            var allItems = await _uow.GetRepository<BusinessTripApplication>().GetAllAsync<BTAViewModel>();
            var btaIds = new List<Guid>();
            if (allItems.Any())
            {
                btaIds = allItems.Select(x => x.Id).ToList();
            }
            // Add Row            
            var allBusinessTrips = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync<BtaDetailViewModel>(x=> btaIds.Contains(x.BusinessTripApplicationId.Value));
            if (allBusinessTrips.Any())
            {

                var allChangeBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => !x.IsCancel);
                var allCancelBTAItems = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync<ChangeCancelBusinessTripDetailViewModel>(x => x.IsCancel);
                var allRooms = await _uow.GetRepository<RoomOrganization>().GetAllAsync(string.Empty, x => x.RoomUserMappings);
                var allRoomTypes = await _uow.GetRepository<RoomType>().GetAllAsync();
                //allBusinessTrips = allBusinessTrips.Where(x => !allCancelBTAItems.Any(y => y.BusinessTripApplicationDetailId == x.Id)).ToList();
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
                if (allCancelBTAItems.Any())
                {
                    foreach (var item in allBusinessTrips)
                    {
                        var cancelledItem = allCancelBTAItems.FirstOrDefault(x => x.BusinessTripApplicationDetailId == item.Id);
                        if (cancelledItem != null)
                        {
                            item.IsCancelled = true;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(parameters.Predicate))
                {
                    parameters.Predicate = parameters.Predicate.Replace("Status", "StatusItem");
                }
                allBusinessTrips = allBusinessTrips.Select(x => x.Trim()).AsQueryable().Where(parameters.Predicate, parameters.PredicateParameters).OrderByDescending(x => x.ReferenceNumber);

                var exportBTADetailViewModel = new List<ExportBusinessTripApplicantionViewModel>();
                foreach (var detail in allBusinessTrips)
                {
                    exportBTADetailViewModel.Add(new ExportBusinessTripApplicantionViewModel
                    {
                        ReferenceNumber = detail.ReferenceNumber,
                        Status = detail.StatusItem,
                        DepartFlight = detail.FlightNumberName,
                        ReturnFlight = detail.ComebackFlightNumberName,
                        SubmittedFullName = detail.CreatedByFullName,
                        SubmittedSAPCode = detail.CreatedBySapCode,
                        SAPCode = detail.SAPCode,
                        FullName = detail.FullName,
                        DepartmentName = !string.IsNullOrEmpty(detail.DeptName) ? detail.DeptName : "",
                        DivisionName = !string.IsNullOrEmpty(detail.DivisionName) ? detail.DivisionName : "",
                        Email = detail.Email,
                        Mobile = detail.Mobile,
                        IsForeigner = detail.IsForeigner,
                        Gender = Enum.GetName(typeof(Gender), detail.Gender),
                        Departure = detail.DepartureName,
                        Arrival = detail.ArrivalName,
                        HotelName = detail.HotelName,
                        FromDate = detail.FromDate.HasValue ? detail.FromDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
                        ToDate = detail.ToDate.HasValue ? detail.ToDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
                        CheckInHotelDate = detail.CheckInHotelDate.HasValue ? detail.CheckInHotelDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
                        CheckOutHotelDate = detail.CheckOutHotelDate.HasValue ? detail.CheckOutHotelDate.Value.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt") : "",
                        CreateDate = detail.Created.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt"),
                        ModifiedDate = detail.Modified.ToLocalTime().ToString("dd/MM/yyyy hh:mm tt"),
                        IDCard = detail.IDCard,
                        Passport = detail.Passport,
                        IsCancelled = detail.IsCancelled
                    });
                }

                for (int rowNum = 0; rowNum < exportBTADetailViewModel.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportBTADetailViewModel.ElementAt(rowNum);
                    for (int j = 0; j < fieldMappings.Count; j++)
                    {
                        var fieldMapping = fieldMappings[j];
                        var value = data.GetType().GetProperty(fieldMapping.Name).GetValue(data);
                        HandleCommonType(row, value, j, fieldMapping);
                    }
                }
            }
            else
            {
                return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
            }
            var creatingExcelFileReslult = ExportExcel(tbl);
            if (creatingExcelFileReslult == null)
            {
                return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
            }
            return new ResultDTO { Object = creatingExcelFileReslult };
        }
    }
}
