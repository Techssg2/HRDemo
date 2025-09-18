using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using AutoMapper;
using Aeon.HR.Infrastructure.Enums;
using System.IO;
using Aeon.HR.ViewModels.BTA;
using Newtonsoft.Json;
using Aeon.HR.Infrastructure.Constants;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class FilghtsBookingProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "FlightsBooking";
        public FilghtsBookingProcessingBO(IUnitOfWork uow) : base(uow)
        {

        }
        private async Task<int> CountUsersOnSameFlight(BookingFlightViewModel bookingDetail)
        {
            int result = 1;
			try
			{
                var count = await _uow.GetRepository<BookingFlight>(true).FindByAsync(x=> x.BTADetail.BusinessTripApplicationId == bookingDetail.BTADetail.BusinessTripApplicationId
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

        private async Task<string> FindParentDepartmentName(DepartmentViewModel dept)
        {
            string parentDeptName = dept.Name;
            try
            {
                var findTitleG5 =
                    await _uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Title.ToUpper().Equals("G5"));
                int G5Grade = findTitleG5?.Grade ?? 5;
                if (dept.JobGradeGrade >= G5Grade)
                {
                    return parentDeptName;
                }
                else
                {
                    var tempDept = dept;
                    bool isContinue = true;
                    while (isContinue)
                    {
                        var parentDeptModel = await _uow.GetRepository<Department>().GetSingleAsync<DepartmentViewModel>(x => x.Id == tempDept.ParentId);
                        if (parentDeptModel != null)
                        {
                            if (parentDeptModel.JobGradeGrade > G5Grade)
                            {
                                isContinue = false;
                            }
                            else
                            {
                                tempDept = parentDeptModel;
                                parentDeptName = parentDeptModel.Name;
                            }
                        }
                        else
                        {
                            isContinue = false;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                parentDeptName = dept.Name;
            }
            return parentDeptName;
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
            // Add Row
            var items = await _uow.GetRepository<BookingFlight>(true).FindByAsync(parameters.Order, 1, Int32.MaxValue, parameters.Predicate, parameters.PredicateParameters);
            if (items.Any())
            {
                for (int rowNum = 0; rowNum < items.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = items.ElementAt(rowNum);
                    var bookingFlightsViewModel = Mapper.Map<BookingFlightViewModel>(data);
                    var pricedItineraries = new PricedItinerariesViewModel();
                    if (bookingFlightsViewModel.FlightDetail != null && bookingFlightsViewModel.FlightDetail.PricedItineraries != null)
                    {
                        pricedItineraries = bookingFlightsViewModel.FlightDetail.PricedItineraries;
                    }

                    DateTimeOffset? btaApprovedDate = null;
                    for (int j = 0; j < fieldMappings.Count; j++)
                    {
                        var fieldMapping = fieldMappings[j];
						if (fieldMapping.Name == "SapCode")
						{
                            var valueItem = bookingFlightsViewModel.BTADetail.SAPCode;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "ReferenceNumber")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.ReferenceNumber;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "FullName")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.FullName;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "Mobile")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.Mobile;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "Email")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.Email;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "DepartmentName")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.DepartmentName;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "ParentDepartmentName")
                        {
                            var valueItem = await FindParentDepartmentName(bookingFlightsViewModel.BTADetail.Department);
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "Departure")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.DepartureName;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "Arrival")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.ArrivalName;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "ToDate")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.ToDate;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "FromDate")
                        {
                            var valueItem = bookingFlightsViewModel.BTADetail.FromDate;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "CreatedDate")
                        {
                            /*var valueItem = data.Created.Date;*/
                            var valueItem = data.BTADetail.BusinessTripApplication.Created;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "FlightNumber")
                        {
                            var dataTimeFlightString = "";
                            var bookingFlightInfors = "";

                            if (bookingFlightsViewModel.FlightDetail != null && bookingFlightsViewModel.FlightDetail.PricedItineraries != null)
                            {
                                if (pricedItineraries != null)
                                {
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
                                }
                            }
                            HandleCommonType(row, bookingFlightInfors, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "Airfare")
                        {
                            //var countUsers = await CountUsersOnSameFlight(bookingFlightsViewModel);
                            var valueItem = pricedItineraries.AirItineraryPricingInfo.AdultFare.passengerFare.baseFare.Amount + pricedItineraries.AirItineraryPricingInfo.AdultFare.passengerFare.serviceTax.Amount;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "ReasonforBusinessTrip")
                        {
                            var valueItem = data.BTADetail.BusinessTripApplication.RequestorNote;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "ReasonforBusinessTripDetail")
                        {
                            var valueItem = data.BTADetail.BusinessTripApplication.RequestorNoteDetail;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "ServiceCharge")
                        {
                            var serviceCharge = Convert.ToInt32(bookingFlightsViewModel.FlightDetail.IncludeEquivfare);
                            HandleCommonType(row, serviceCharge, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "SubTotal")
                        {
                            var serviceCharge = Convert.ToInt32(bookingFlightsViewModel.FlightDetail.IncludeEquivfare);
                            var valueItem = pricedItineraries.AirItineraryPricingInfo.AdultFare.passengerFare.baseFare.Amount + pricedItineraries.AirItineraryPricingInfo.AdultFare.passengerFare.serviceTax.Amount + serviceCharge;
                            HandleCommonType(row, valueItem, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "OverBudget")
                        {
                            var overBudget = bookingFlightsViewModel.BTADetail.IsOverBudget ? "Yes" : "No";
                            HandleCommonType(row, overBudget, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "IsCancel")
                        {
                            var isCancel = bookingFlightsViewModel.IsCancel ? "Yes" : "No";
                            HandleCommonType(row, isCancel, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "CancellationFee")
                        {
                            var cancellationFee = bookingFlightsViewModel.IsCancel ? Convert.ToInt32(bookingFlightsViewModel.PenaltyFee).ToString() : "";
                            HandleCommonType(row, cancellationFee, j, fieldMapping);
                            continue;
                        }
                        if (fieldMapping.Name == "CancellationReason")
                        {
                            if (bookingFlightsViewModel.IsCancel)
                            {
                                var reasonToCancel = "";
                                var changeCancelDetail = await _uow.GetRepository<ChangeCancelBusinessTripDetail>().GetSingleAsync<ChangeCancelBusinessTripDetailViewModel>(x => x.BusinessTripApplicationDetailId == bookingFlightsViewModel.BTADetailId);

                                if (changeCancelDetail != null)
                                {
                                    if (!bookingFlightsViewModel.DirectFlight)
                                    {
                                        reasonToCancel = changeCancelDetail.ReasonForInBoundFlight;
                                    }
                                    else
                                    {
                                        reasonToCancel = changeCancelDetail.ReasonForOutBoundFlight;
                                    }
                                }
                                HandleCommonType(row, reasonToCancel, j, fieldMapping);
                            }
                            continue;
                        }
                        if (fieldMapping.Name == "BTAApprovedDate")
                        {
                            var workflowHistories = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync(x => x.Instance != null && x.Instance.ItemId == bookingFlightsViewModel.BTADetail.BusinessTripApplicationId && !string.IsNullOrEmpty(x.Outcome) && BusinessTripApplicationOutcomeConstants.FLIGHT_SELECTED.Equals(x.Outcome, StringComparison.OrdinalIgnoreCase) && x.IsStepCompleted, "Created desc");
                            if (workflowHistories != null)
                            {
                                btaApprovedDate = workflowHistories.Created;
                            }
                            HandleCommonType(row, btaApprovedDate, j, fieldMapping);
                            continue;
                        }

                        if (fieldMapping.Name == "BookingDuration")
                        {
                            var boookingDuration = "";
                            if (btaApprovedDate.HasValue)
                            {
                                var FromDate = ((DateTimeOffset)bookingFlightsViewModel.BTADetail.FromDate).Date;
                                var ApproveDate = ((DateTimeOffset)btaApprovedDate).Date;
                                boookingDuration = (FromDate - ApproveDate).Days.ToString();
                            }
                            HandleCommonType(row, boookingDuration, j, fieldMapping);
                            continue;
                        }
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

        public Task<ResultDTO> ImportAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }
    }
}