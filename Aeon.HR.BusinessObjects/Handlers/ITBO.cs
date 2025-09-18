using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Jobs;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Aeon.HR.ViewModels.ExternalItem.ParseInfo;
using AutoMapper;
using System.Linq;
using Aeon.HR.ViewModels.ExternalItem;
using static Aeon.HR.ViewModels.CommonViewModel;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class ITBO : IITBO
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        public ITBO(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<ResultDTO> SaveResignation(ITSaveResignationArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var resignations = await _uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.ReferenceNumber == args.ReferenceNumber);
                if (resignations is null)
                {
                    resultDto.ErrorCodes = new List<int> { -1 };
                    resultDto.Messages = new List<string> { "ReferenceNumber not null!" };
                }

                if (args.StartingDate.HasValue)
                    resignations.StartingDate = args.StartingDate.Value;

                if (args.OfficialResignationDate.HasValue)
                    resignations.OfficialResignationDate = args.OfficialResignationDate.Value;

                if (args.SuggestionForLastWorkingDay.HasValue)
                    resignations.SuggestionForLastWorkingDay = args.SuggestionForLastWorkingDay.Value;

                if (args.IsUpdatePayload)
                {
                    var trackingLogInitData = await _uow.GetRepository<TrackingLogInitData>().GetSingleAsync(x => !string.IsNullOrEmpty(x.ReferenceNumber) && x.ReferenceNumber == args.ReferenceNumber);
                    if (!(trackingLogInitData is null))
                    {
                        if (trackingLogInitData.TrackingLogId.HasValue)
                        {
                            var trackingRequest = await _uow.GetRepository<TrackingRequest>().FindByIdAsync(trackingLogInitData.TrackingLogId.Value);
                            if (!(trackingRequest is null))
                            {
                                ResignationJsonInfo data = Mapper.Map<ResignationJsonInfo>(JsonConvert.DeserializeObject<ResignationJsonInfo>(trackingRequest.Payload));
                                data.Begda = resignations.OfficialResignationDate.LocalDateTime.ToString("yyyyMMdd");
                                trackingRequest.Payload = JsonConvert.SerializeObject(data);
                            }
                        }
                    }
                }
                await _uow.CommitAsync();
                resultDto.Object = Mapper.Map<ResignationApplicationViewModel>(resignations);
            }
            catch (Exception e)
            {
                resultDto.ErrorCodes = new List<int> { 501 };
                resultDto.Messages = new List<string> { "Error: " + e.Message };
            }
            return resultDto;
        }

        public async Task<ResultDTO> SaveRequestToHire(ITSaveActingArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var requestToHire = await _uow.GetRepository<RequestToHire>().GetSingleAsync(x => x.ReferenceNumber == args.ReferenceNumber);
                if (requestToHire is null)
                {
                    resultDto.ErrorCodes = new List<int> { -1 };
                    resultDto.Messages = new List<string> { "ReferenceNumber not null!" };
                }

                if (args.WorkingAddressRecruitmentId.HasValue)
                    requestToHire.WorkingAddressRecruitmentId = args.WorkingAddressRecruitmentId.Value;

                requestToHire.HasBudget = args.HasBudget;

                await _uow.CommitAsync();
                resultDto.Object = Mapper.Map<RequestToHireViewModel>(requestToHire);
            }
            catch (Exception e)
            {
                resultDto.ErrorCodes = new List<int> { 501 };
                resultDto.Messages = new List<string> { "Error: " + e.Message };
            }
            return resultDto;
        }

        public async Task<ResultDTO> SaveShiftExchange(ITSaveShiftExchangeArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var shiftExchangedetail = await _uow.GetRepository<ShiftExchangeApplicationDetail>().FindByAsync(x => x.ShiftExchangeApplication.ReferenceNumber == args.ReferenceNumber);
                if (shiftExchangedetail is null)
                {
                    resultDto.ErrorCodes = new List<int> { -1 };
                    resultDto.Messages = new List<string> { "ReferenceNumber not null!" };
                }
                foreach (var item in shiftExchangedetail)
                {
                    var currentRow = args.ShiftExchangeItemsData.Where(x => x.UserId == item.UserId && !x.CurrentShiftCode.Equals(item.CurrentShiftCode) && DateTimeOffset.Compare(item.ShiftExchangeDate, x.ShiftExchangeDate) == 0).FirstOrDefault();
                    if (currentRow is null) continue;
                    item.CurrentShiftCode = currentRow.CurrentShiftCode;
                    item.CurrentShiftName = currentRow.CurrentShiftName;
                    _uow.GetRepository<ShiftExchangeApplicationDetail>().Update(item);

                    if (args.IsUpdatePayload)
                    {
                        var user = await _uow.GetRepository<User>(true).FindByIdAsync(item.UserId);
                        if (!(user is null))
                        {
                            var trackingLogInitData = await _uow.GetRepository<TrackingLogInitData>().GetSingleAsync(x => !string.IsNullOrEmpty(x.ReferenceNumber) && x.ReferenceNumber == args.ReferenceNumber
                            && x.FromDate.HasValue && DateTimeOffset.Compare(x.FromDate.Value, item.ShiftExchangeDate) == 0 && x.ToDate.HasValue && DateTimeOffset.Compare(x.ToDate.Value, item.ShiftExchangeDate) == 0
                            && x.SAPCode == user.SAPCode);
                            if (!(trackingLogInitData is null))
                            {
                                var trackingRequest = await _uow.GetRepository<TrackingRequest>().FindByIdAsync(trackingLogInitData.TrackingLogId.Value);
                                if (!(trackingRequest is null))
                                {
                                    ShiftExchangeJsonInfo data = Mapper.Map<ShiftExchangeJsonInfo>(JsonConvert.DeserializeObject<ShiftExchangeJsonInfo>(trackingRequest.Payload));
                                    data.Cur_shift = item.CurrentShiftCode;
                                    trackingRequest.Payload = JsonConvert.SerializeObject(data);
                                    _uow.GetRepository<TrackingRequest>().Update(trackingRequest);
                                }
                            }
                        }
                    }
                }
                await _uow.CommitAsync();
                resultDto.Object = true;
            }
            catch (Exception e)
            {
                resultDto.ErrorCodes = new List<int> { 501 };
                resultDto.Messages = new List<string> { "Error: " + e.Message };
            }
            return resultDto;
        }
        public async Task<ResultDTO> SaveTargetPlan(ITSaveTargetPlanArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var targetPlanDetail = await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.TargetPlan.ReferenceNumber == args.ReferenceNumber);
                if (targetPlanDetail is null)
                {
                    resultDto.ErrorCodes = new List<int> { -1 };
                    resultDto.Messages = new List<string> { "ReferenceNumber not null!" };
                    goto Finish;
                }

                HashSet<string> sapCodeList = new HashSet<string>();
                foreach (var item in targetPlanDetail)
                {
                    var currentRow = args.TargetPlanDetails.Where(x => x.SAPCode == item.SAPCode && x.Type == item.Type && item.DepartmentCode == x.DepartmentCode).FirstOrDefault();
                    if (currentRow is null) continue;
                    item.ERDQuality = currentRow.ERDQuality;
                    item.PRDQuality = currentRow.PRDQuality;
                    item.DOFLQuality = currentRow.DOFLQuality;
                    item.JsonData = currentRow.JsonData;
                    _uow.GetRepository<TargetPlanDetail>().Update(item);
                    sapCodeList.Add(item.SAPCode);
                }
                await _uow.CommitAsync();

                await Task.Delay(1000);
                if (sapCodeList.Any() && args.IsUpdatePayload)
                {
                    foreach(var sapCode in sapCodeList)
                    {
                        var trackingLogInitData = await _uow.GetRepository<TrackingLogInitData>().GetSingleAsync(x => !string.IsNullOrEmpty(x.ReferenceNumber) && x.ReferenceNumber == args.ReferenceNumber && x.SAPCode == sapCode);
                        if (!(trackingLogInitData is null))
                        {
                            var trackingRequest = await _uow.GetRepository<TrackingRequest>().FindByIdAsync(trackingLogInitData.TrackingLogId.Value);
                            if (!(trackingRequest is null))
                            {
                                PayloadTrackingRequest data = Mapper.Map<PayloadTrackingRequest>(JsonConvert.DeserializeObject<PayloadTrackingRequest>(trackingRequest.Payload));
                                if (!(data is null))
                                {
                                    var targetPlan = await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.TargetPlan.ReferenceNumber == args.ReferenceNumber && x.SAPCode == sapCode && args.PeriodId == x.TargetPlan.PeriodId);
                                    if (!(targetPlan is null))
                                    {
                                        data.TargetData01Set = targetPlan.Where(x => x.Type == TypeTargetPlan.Target1).Select(y => Mapper.Map<List<DateValueItem>>(JsonConvert.DeserializeObject<List<DateValueArgs>>(y.JsonData))).FirstOrDefault();
                                        data.TargetData02Set = targetPlan.Where(x => x.Type == TypeTargetPlan.Target2).Select(y => Mapper.Map<List<DateValueItem>>(JsonConvert.DeserializeObject<List<DateValueArgs>>(y.JsonData))).FirstOrDefault();
                                        var currentPayload = JsonConvert.SerializeObject(data);
                                        if (currentPayload.ToLower().Equals(trackingRequest.Payload.ToLower())) continue; // neu trung payload thi continues
                                        trackingRequest.Payload = JsonConvert.SerializeObject(data);
                                        _uow.GetRepository<TrackingRequest>().Update(trackingRequest);
                                    }
                                }
                            }
                        }
                    }
                }
                await _uow.CommitAsync();
                resultDto.Object = true;
            }
            catch (Exception e)
            {
                resultDto.ErrorCodes = new List<int> { 501 };
                resultDto.Messages = new List<string> { "Error: " + e.Message };
            }
            Finish:
            return resultDto;
        }

        public async Task<ResultDTO> SavePromoteAndTransfer(ITSavePromoteAndTransferArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                if (string.IsNullOrEmpty(args.ReferenceNumber))
                {
                    resultDto.ErrorCodes = new List<int> { -1 };
                    resultDto.Messages = new List<string> { "ReferenceNumber not null!" };
                    goto Finish;
                }
                var promoteAndTransfer = await _uow.GetRepository<PromoteAndTransfer>().GetSingleAsync(x => x.ReferenceNumber == args.ReferenceNumber);
                if(!string.IsNullOrEmpty(args.NewWorkLocationName))
                    promoteAndTransfer.NewWorkLocationName = args.NewWorkLocationName;

                await _uow.CommitAsync();
                resultDto.Object = true;
            }
            catch (Exception e)
            {
                resultDto.ErrorCodes = new List<int> { 501 };
                resultDto.Messages = new List<string> { "Error: " + e.Message };
            }
            Finish:
            return resultDto;
        }

        public async Task<ResultDTO> SaveBTA(ITSaveBTAArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                if (string.IsNullOrEmpty(args.ReferenceNumber))
                {
                    resultDto.ErrorCodes = new List<int> { -1 };
                    resultDto.Messages = new List<string> { "ReferenceNumber not null!" };
                    goto Finish;
                }
                var findBTA = _uow.GetRepository<Data.Models.BusinessTripApplication>().GetSingle(x => x.ReferenceNumber == args.ReferenceNumber);
                if (findBTA is null)
                {
                    resultDto.ErrorCodes = new List<int> { -1 };
                    resultDto.Messages = new List<string> { $"Cannot find BTA with reference number {args.ReferenceNumber} !" };
                    goto Finish;
                }
                if (!string.IsNullOrEmpty(args.RequestorNote))
                {
                    findBTA.RequestorNote = args.RequestorNote;
                }
                if (!string.IsNullOrEmpty(args.RequestorNoteDetail))
                {
                    findBTA.RequestorNoteDetail = args.RequestorNoteDetail;
                }
                if (args.IsRoundTrip != null && args.IsRoundTrip.HasValue)
                {
                    findBTA.IsRoundTrip = args.IsRoundTrip.Value;
                }
                if (args.BTADetails != null && args.BTADetails.Any())
                {
                    var convertBTADetailArgs = JsonConvert.DeserializeObject<List<BusinessTripApplicationDetail>>(args.BTADetails);
                    if (convertBTADetailArgs == null)
                    {
                        resultDto.ErrorCodes = new List<int> { -1 };
                        resultDto.Messages = new List<string> { $"Cannot find any row BTA Details!" };
                        goto Finish;
                    }

                    var findBTADetails = _uow.GetRepository<BusinessTripApplicationDetail>().FindBy(x => x.BusinessTripApplicationId == findBTA.Id);
                    if (findBTADetails == null)
                    {
                        resultDto.ErrorCodes = new List<int> { -1 };
                        resultDto.Messages = new List<string> { $"Cannot find any row BTA Details with Ref: {findBTA.ReferenceNumber}" };
                        goto Finish;
                    }

                    foreach (var btaDtl in findBTADetails.ToList())
                    {
                        var findItemNeedUpdate = convertBTADetailArgs.FirstOrDefault(x => x.Id == btaDtl.Id);
                        if (findItemNeedUpdate == null) continue;

                        // if (btaDtl.IsCommitBooking)
                        // {

                        btaDtl.StayHotel = findItemNeedUpdate.StayHotel;
                        if ((!string.IsNullOrEmpty(findItemNeedUpdate.HotelCode) && !string.IsNullOrEmpty(findItemNeedUpdate.HotelName)) || !findItemNeedUpdate.StayHotel)
                        {
                            btaDtl.HotelCode = findItemNeedUpdate.HotelCode;
                            btaDtl.HotelName = findItemNeedUpdate.HotelName;
                        }

                        if (findItemNeedUpdate.CheckInHotelDate != null || !findItemNeedUpdate.StayHotel)
                        {
                            btaDtl.CheckInHotelDate = findItemNeedUpdate.CheckInHotelDate;
                        }
                        if (findItemNeedUpdate.CheckOutHotelDate != null || !findItemNeedUpdate.StayHotel)
                        {
                            btaDtl.CheckOutHotelDate = findItemNeedUpdate.CheckOutHotelDate;
                        }
                        // continue;
                        // }

                        if (findItemNeedUpdate.PartitionId != null && !string.IsNullOrEmpty(findItemNeedUpdate.PartitionInfo))
                        {
                            btaDtl.PartitionId = findItemNeedUpdate.PartitionId;
                            btaDtl.PartitionCode = findItemNeedUpdate.PartitionCode;
                            btaDtl.PartitionName = findItemNeedUpdate.PartitionName;
                            btaDtl.PartitionInfo = findItemNeedUpdate.PartitionInfo;
                        }

                        // Departure
                        if (!string.IsNullOrEmpty(findItemNeedUpdate.DepartureCode) && !string.IsNullOrEmpty(findItemNeedUpdate.DepartureInfo))
                        {
                            btaDtl.DepartureCode = findItemNeedUpdate.DepartureCode;
                            btaDtl.DepartureName = findItemNeedUpdate.DepartureName;
                            btaDtl.DepartureInfo = findItemNeedUpdate.DepartureInfo;
                        }

                        // Arrival
                        if (!string.IsNullOrEmpty(findItemNeedUpdate.ArrivalCode) && !string.IsNullOrEmpty(findItemNeedUpdate.ArrivalInfo))
                        {
                            btaDtl.ArrivalCode = findItemNeedUpdate.ArrivalCode;
                            btaDtl.ArrivalName = findItemNeedUpdate.ArrivalName;
                            btaDtl.ArrivalInfo = findItemNeedUpdate.ArrivalInfo;
                        }

                        if (findItemNeedUpdate.FromDate != null)
                            btaDtl.FromDate = findItemNeedUpdate.FromDate;

                        if (findItemNeedUpdate.ToDate != null)
                            btaDtl.ToDate = findItemNeedUpdate.ToDate;

                        btaDtl.IsForeigner = findItemNeedUpdate.IsForeigner;
                        btaDtl.StayHotel = findItemNeedUpdate.StayHotel;
                        if (!string.IsNullOrEmpty(findItemNeedUpdate.HotelCode) && !string.IsNullOrEmpty(findItemNeedUpdate.HotelName))
                        {
                            btaDtl.HotelCode = findItemNeedUpdate.HotelCode;
                            btaDtl.HotelName = findItemNeedUpdate.HotelName;
                        }

                        if (findItemNeedUpdate.CheckInHotelDate != null)
                        {
                            btaDtl.CheckInHotelDate = findItemNeedUpdate.CheckInHotelDate;
                        }
                        if (findItemNeedUpdate.CheckOutHotelDate != null)
                        {
                            btaDtl.CheckOutHotelDate = findItemNeedUpdate.CheckOutHotelDate;
                        }

                        if (!string.IsNullOrEmpty(findItemNeedUpdate.FirstName))
                        {
                            btaDtl.FirstName = findItemNeedUpdate.FirstName;
                        }

                        if (!string.IsNullOrEmpty(findItemNeedUpdate.LastName))
                        {
                            btaDtl.LastName = findItemNeedUpdate.LastName;
                        }

                        if (!string.IsNullOrEmpty(findItemNeedUpdate.Email))
                        {
                            btaDtl.Email = findItemNeedUpdate.Email;
                        }
                        if (!string.IsNullOrEmpty(findItemNeedUpdate.Mobile))
                        {
                            btaDtl.Mobile = findItemNeedUpdate.Mobile;
                        }
                        if (!string.IsNullOrEmpty(findItemNeedUpdate.IDCard))
                        {
                            btaDtl.IDCard = findItemNeedUpdate.IDCard;
                        }
                        if (findItemNeedUpdate.DateOfBirth != null)
                        {
                            btaDtl.DateOfBirth = findItemNeedUpdate.DateOfBirth;
                        }
                        if (!string.IsNullOrEmpty(findItemNeedUpdate.Passport))
                        {
                            btaDtl.Passport = findItemNeedUpdate.Passport;
                        }
                        if (findItemNeedUpdate.PassportDateOfIssue != null)
                        {
                            btaDtl.PassportDateOfIssue = findItemNeedUpdate.PassportDateOfIssue;
                        }
                        if (findItemNeedUpdate.PassportExpiryDate != null)
                        {
                            btaDtl.PassportExpiryDate = findItemNeedUpdate.PassportExpiryDate;
                        }
                        if (findItemNeedUpdate.HasBudget != null)
                        {
                            btaDtl.HasBudget = findItemNeedUpdate.HasBudget;
                        }
                        if (!string.IsNullOrEmpty(findItemNeedUpdate.CountryCode))
                        {
                            btaDtl.CountryCode = findItemNeedUpdate.CountryCode;
                            btaDtl.CountryName = findItemNeedUpdate.CountryName;
                            btaDtl.CountryInfo = findItemNeedUpdate.CountryInfo;
                        }
                        btaDtl.Gender = findItemNeedUpdate.Gender;
                    }
                }
                await _uow.CommitAsync();
                resultDto.Object = true;
            }
            catch (Exception e)
            {
                resultDto.ErrorCodes = new List<int> { 501 };
                resultDto.Messages = new List<string> { $"Error: {e.Message}" };
            }
            Finish:
            return resultDto;
        }
        
    }
}
