using Aeon.HR.BusinessObjects;
using Aeon.HR.BusinessObjects.Handlers.ExternalBO;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.CustomSection;
using Aeon.HR.ViewModels.DTOs;
using AEON.Integrations.Gotadi.API;
using AEON.Integrations.Gotadi.Common;
using AEON.Integrations.Gotadi.DTO.AirTicket;
using AEON.Integrations.Gotadi.QueryParams.AirTicket;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static System.Net.WebRequestMethods;
using AirlineCodeInfo = AEON.Integrations.Gotadi.DTO.AirTicket.AirlineCodeInfo;

namespace Aeon.HR.API.Controllers.Others
{
    public class AirTicketController : BaseController
    {
        protected readonly IBusinessTripBO bo;
        public AirTicketController(ILogger logger, IBusinessTripBO _bo) : base(logger)
        {
            bo = _bo;
        }

        [HttpPost]
        public async Task<IHttpActionResult> SearchFlight(SearchQueryParams args)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                var searchAirTicket = await airTicketsApi.LowFareSearchAsync(args);

                if (searchAirTicket is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Search the air ticket failed. Please try again");
                }
                else
                {
                    result.Object = searchAirTicket;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: SearchFlight", ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> FareRules(string language, FareRulesRequest args)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                if (string.IsNullOrEmpty(language))
                {
                    language = "vi";
                }

                FarerulesQueryParams getLang = new FarerulesQueryParams();
                getLang.Language = language;
                var searchFareRules = await airTicketsApi.Farerules(getLang, args);

                if (searchFareRules is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Search the fare rules failed. Please try again");
                }
                else
                {
                    result.Object = searchFareRules;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: FareRules", ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> SearchAirport(string queryText, string country)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                SearchAirPortParams searchAirportParams = new SearchAirPortParams();
                searchAirportParams.country = string.IsNullOrEmpty(queryText) ? country : string.Empty;
                searchAirportParams.query = queryText;
                searchAirportParams.page = "0";
                searchAirportParams.size = "50";
                searchAirportParams.sort = "city,asc";

                var searchAirport = await airTicketsApi.SearchAirports(searchAirportParams);

                if (!string.IsNullOrEmpty(country) && string.IsNullOrEmpty(searchAirportParams.country))
                {
                    searchAirport = searchAirport.Where(x => x != null && x.CountryCode.Equals(country, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (searchAirport is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Search airport failed. Please try again");
                }
                else
                {
                    result.Object = searchAirport;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: FareRSearchAirportules", ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" + ex.Message + ". " + ex.StackTrace } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Revalidate(ItineraryInfo args)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                _logger.LogInformation("Info at: Revalidate " + JsonConvert.SerializeObject(args));
                var revalidateResult = await airTicketsApi.Revalidate(args);

                if (revalidateResult is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Revalidate ticket failed. Please try again");
                }
                else
                {
                    result.Object = revalidateResult;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: Revalidate", ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DraftBooking(Guid? btaID, DraftBookingRequest args)
        {
            try
            {
                var result = new ResultDTO();
                _logger.LogError("Info Payload at: DraftBooking " + JsonConvert.SerializeObject(args));
                bo.SaveBTALog(btaID, "Info at: DraftBooking " + JsonConvert.SerializeObject(args));
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                var revalidateResult = await airTicketsApi.DraftBooking(args);
                if (revalidateResult is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Draft booking ticket failed. Please try again");
                    bo.SaveBTALog(btaID, "Info at: DraftBooking null ");
                }
                else
                {
                    // string response = "{\"bookingCode\":{\"bookingCode\":\"BOD::250206::5b05cde6-0fc1-4a83-89a6-88bf2711c86c\",\"bookingNumber\":\"ADPO25020628967020\"},\"bookingType\":\"DOME\",\"departDraftItineraryInfo\":{\"bookingDirection\":\"DEPARTURE\",\"fareSourceCode\":\"domb655a8ed-27de-4e42-824f-cf770fc3c983#d5-0\",\"groupId\":\"16edb929-dc28-45ea-9a3b-ad50d20303c9\",\"itinTotalFare\":{\"baseFare\":{\"amount\":2937000.0,\"currencyCode\":null,\"decimalPlaces\":2},\"equivFare\":{\"amount\":60000.0,\"currencyCode\":null,\"decimalPlaces\":2},\"serviceTax\":{\"amount\":804000.0,\"currencyCode\":null,\"decimalPlaces\":2},\"totalFare\":{\"amount\":3801000.0,\"currencyCode\":null,\"decimalPlaces\":2},\"totalTax\":{\"amount\":0.0,\"currencyCode\":null,\"decimalPlaces\":2},\"totalPaxFee\":null},\"searchId\":\"ATD::250206::f143a954-6fd5-4143-9210-443ea51c485b\"},\"duration\":1831,\"errors\":[{\"code\":\"8_RESERVE_TICKET_FAILED\",\"id\":null,\"message\":\"Failed to reserve flight ticket\"},{\"code\":\"5_BOOKING_TRANSACTION_STATUS_INFO_EMPTY\",\"id\":null,\"message\":\"BookingTransactionStatusInfo list is empty\"},{\"code\":\"5_BOOKING_EMPTY\",\"id\":\"5001\",\"message\":\"Booking Empty - No Info\"}],\"infos\":null,\"isPerBookingType\":true,\"isRoundTripType\":true,\"isSuccess\":true,\"markupType\":\"PER_BOOKING\",\"returnDraftItineraryInfo\":{\"bookingDirection\":\"RETURN\",\"fareSourceCode\":\"domb655a8ed-27de-4e42-824f-cf770fc3c983#r11-0\",\"groupId\":\"03975b4a-3534-4962-9455-036adf1f9210\",\"itinTotalFare\":{\"baseFare\":{\"amount\":1538000.0,\"currencyCode\":null,\"decimalPlaces\":2},\"equivFare\":null,\"serviceTax\":{\"amount\":693000.0,\"currencyCode\":null,\"decimalPlaces\":2},\"totalFare\":{\"amount\":2231000.0,\"currencyCode\":null,\"decimalPlaces\":2},\"totalTax\":{\"amount\":0.0,\"currencyCode\":null,\"decimalPlaces\":2},\"totalPaxFee\":null},\"searchId\":\"ATD::250206::f143a954-6fd5-4143-9210-443ea51c485b-R\"},\"roundType\":\"RoundTrip\",\"success\":true,\"textMessage\":null}";
                    // DraftBookingResponse revalidateResult = JsonConvert.DeserializeObject<DraftBookingResponse>(response);
                    if (revalidateResult.Errors != null)
                    {
                        result.Object = revalidateResult;
                        _logger.LogInformation("DraftBooking.Response Gotadi: " + revalidateResult.ToString());
                        bo.SaveBTALog(btaID, "Info at: DraftBooking response " + JsonConvert.SerializeObject(revalidateResult));
                        result.ErrorCodes = new List<int>() { 1001 };
                        return Ok(result);
                    }
                    result.Object = revalidateResult;
                    _logger.LogInformation("DraftBooking.Response Gotadi: " + revalidateResult.ToString());
                    bo.SaveBTALog(btaID, "Info at: DraftBooking response " + JsonConvert.SerializeObject(revalidateResult));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: DraftBooking", ex.Message + ". " + ex.StackTrace);
                bo.SaveBTALog(btaID, "Error at: DraftBooking" + ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetBookingDetail(string bookingNumber)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                BookingDetailQueryParams args = new BookingDetailQueryParams() { BookingNumber = bookingNumber };
                var revalidateResult = await airTicketsApi.GetBookingDetail(args);

                if (revalidateResult is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Get booking detail failed. Please try again");
                }
                else
                {
                    result.Object = revalidateResult;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: GetBookingDetail", ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddBookingTraveller(TravellerArgs travellerArgs)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                _logger.LogInformation("Info at Add booking traveller: " + travellerArgs.btaDetailItem[0].BusinessTripApplicationId);

                //Check BookingOld
                if (travellerArgs.btaDetailItem != null)
                {
                    foreach (var item in travellerArgs.btaDetailItem)
                    {
                        if (!string.IsNullOrEmpty(item.BookingNumber))
                        {
                            BookingDetailQueryParams checkStatusPr = new BookingDetailQueryParams() { BookingNumber = item.BookingNumber };
                            var revalidateResultStatus = await airTicketsApi.GetBookingDetail(checkStatusPr);
                            if (revalidateResultStatus is null)
                            {
                                _logger.LogInformation("Info at: Get booking detail failed. Please try again");
                            }
                            else
                            {
                                bo.SaveBTALog(travellerArgs.btaDetailItem[0].BusinessTripApplicationId, "Info at: Get booking detail: detailId " + item.Id + " BookingNumber " + item.BookingNumber + " Status " + revalidateResultStatus.BookingInfo.Status + " PaymentStatus " + revalidateResultStatus.BookingInfo.PaymentStatus + " IssuedStatus " + revalidateResultStatus.BookingInfo.IssuedStatus);
                                _logger.LogInformation("Info at: Get booking detail: Status " + revalidateResultStatus.BookingInfo.Status + " PaymentStatus " + revalidateResultStatus.BookingInfo.PaymentStatus + " IssuedStatus " + revalidateResultStatus.BookingInfo.IssuedStatus);

                                if (revalidateResultStatus.BookingInfo.Status.Equals("BOOKED"))
                                {
                                    result.ErrorCodes.Add(1009);
                                    result.Messages.Add($"Add booking traveller failed. {item.BookingNumber} has ticket (eDoc). Pls check for Admin!");
                                    return Ok(result);
                                }
                            }
                        }
                    }
                    var updateBtaDetail = await bo.UpdateBeforeCommitBookingForBTADetail(travellerArgs.btaDetailItem, null, null, false);
                    if (!updateBtaDetail.IsSuccess)
                    {
                        result.ErrorCodes.Add(1009);
                        result.Messages.Add("Add booking traveller failed. Save BookingNumber Error");
                        return Ok(result);
                    }

                }
                //end 
                List<BookingContact> bookingContact = new List<BookingContact>();
                List<BookingTravelerInfo> bookingTravlerInfos = new List<BookingTravelerInfo>();
                string bookingNumber = travellerArgs.bookingNumber;
                BookingContact adminContact = new BookingContact();
                var getInfoAdmin = await bo.GetInfoAdmin();
                if (getInfoAdmin.IsSuccess && getInfoAdmin.Object != null)
                {
                    adminContact.Email = getInfoAdmin.Object.GetPropertyValue("EmailBookingContract") + string.Empty;
                    adminContact.FirstName = TrimCharacters(getInfoAdmin.Object.GetPropertyValue("FirstName").ToString()) + string.Empty;
                    adminContact.SurName = TrimCharacters(getInfoAdmin.Object.GetPropertyValue("SurName").ToString()) + string.Empty;
                    adminContact.PhoneNumber1 = getInfoAdmin.Object.GetPropertyValue("PhoneNumber") + string.Empty;
                    adminContact.PhoneCode1 = "84";
                    adminContact.BookingNumber = bookingNumber;
                }
                else
                {
                    adminContact.Email = "phavatam@gmail.com";
                    adminContact.FirstName = "VAN TAM";
                    adminContact.SurName = "PHAM";
                    adminContact.PhoneNumber1 = "0961137753";
                    adminContact.PhoneCode1 = "84";
                    adminContact.BookingNumber = bookingNumber;
                }

                bookingContact.Add(adminContact);

                if (null != travellerArgs.btaDetailItem && travellerArgs.btaDetailItem.Count > 0)
                {
                    //Get Submitter BusinessTripApplication
                    var businessTripApplication = await bo.GetEmailSubmitter(travellerArgs.Id);
                    if (businessTripApplication != null && businessTripApplication.Object != null)
                    {
                        //var infoSubmiterUser = businessTripApplication.Object;
                        //string fullName = TrimCharacters(infoSubmiterUser.GetPropertyValue("FullName") + string.Empty);
                        //var firstSpaceIndex = fullName.IndexOf(" ");
                        //var strSureName = fullName.Substring(0, firstSpaceIndex);
                        //var strFirstName = fullName.Substring(firstSpaceIndex + 1);
                        //BookingContact userInfo = new BookingContact()
                        //{
                        //    Email = infoSubmiterUser.GetPropertyValue("Email") + string.Empty,
                        //    FirstName = strFirstName,
                        //    SurName = strSureName,
                        //    PhoneNumber1 = infoSubmiterUser.GetPropertyValue("Mobile") + string.Empty,
                        //    PhoneCode1 = "84",
                        //    BookingNumber = bookingNumber
                        //};
                        var infoSubmiterUser = businessTripApplication.Object;
                        //string fullName = TrimCharacters(infoSubmiterUser.GetPropertyValue("FullName") + string.Empty);
                        //var firstSpaceIndex = fullName.IndexOf(" ");
                        //var strSureName = fullName.Substring(0, firstSpaceIndex);
                        //var strFirstName = fullName.Substring(firstSpaceIndex + 1);
                        BookingContact userInfo = new BookingContact()
                        {
                            Email = infoSubmiterUser.GetPropertyValue("EmailBookingContract") + string.Empty,
                            FirstName = TrimCharacters(infoSubmiterUser.GetPropertyValue("FirstName").ToString()) + string.Empty,
                            SurName = TrimCharacters(infoSubmiterUser.GetPropertyValue("SurName").ToString()) + string.Empty,
                            PhoneNumber1 = infoSubmiterUser.GetPropertyValue("PhoneNumber") + string.Empty,
                            PhoneCode1 = "84",
                            BookingNumber = bookingNumber
                        };

                        if (HasDiacritics(userInfo.FirstName))
                        {
                            userInfo.FirstName = TrimCharactersV2(userInfo.FirstName);
                        }

                        if (HasDiacritics(userInfo.SurName))
                        {
                            userInfo.SurName = TrimCharactersV2(userInfo.SurName);
                        }
                        bookingContact.Add(userInfo);
                        bookingContact.Reverse(); // dao nguoc phan tu. Doi thong tin lien he
                    }

                    foreach (var item in travellerArgs.btaDetailItem)
                    {
                        var airline = new AirlineCodeInfo();
                        var member = new MembershipDTO();
                        if (travellerArgs.airlines != null)
                        {
                            var airliness = JsonConvert.DeserializeObject<List<AirlineCodeInfo>>(travellerArgs.airlines);
                            airline = airliness != null ? airliness.FirstOrDefault(e => e.Direction.Equals("DEPARTURE")) : null;
                            var memberCards = new List<MembershipDTO>();
                            if (item.Memberships != null)
                            {
                                memberCards = JsonConvert.DeserializeObject<List<MembershipDTO>>(item.Memberships);
                                member = airline != null ? memberCards.Where(e => e.AirlineCode == airline.AirlineCode).FirstOrDefault() : null;
                                if (member == null)
                                {
                                    airline = airliness != null ? airliness.FirstOrDefault(e => e.Direction.Equals("RETURN")) : null;
                                    member = airline != null ? memberCards.Where(e => e.AirlineCode == airline.AirlineCode).FirstOrDefault() : null;
                                }

                            }
                        }
                        BookingTravelerInfo bookingTraveler = new BookingTravelerInfo();
                        var traveler = new Traveler()
                        {
                            AdultType = "ADT",
                            DocumentNumber = item.Passport,
                            DocumentExpiredDate = item.PassportExpiryDate.HasValue ? item.PassportExpiryDate.Value.LocalDateTime.AddHours(12) : item.PassportExpiryDate,
                            FirstName = item.FirstName,
                            SurName = item.LastName,
                            Gender = item.Gender == Gender.Male ? "MALE" : "FEMALE",
                            MemberCard = member != null && member.AirlineCode != null ? true : false,
                            MemberCardType = member != null && member.AirlineCode != null ? (object)member.AirlineCode : false,
                            MemberCardNumber = member != null && member.AirlineCode != null ? (object)member.IdCard : false,
                            BookingNumber = bookingNumber
                        };
                        if (item.DateOfBirth.HasValue)
                            traveler.Dob = item.DateOfBirth.Value.LocalDateTime.AddHours(12);

                        if (!string.IsNullOrEmpty(item.CountryCode))
                            traveler.DocumentIssuingCountry = item.CountryCode;

                        // PP = Passport, PID = Personal ID (CCCD)
                        if (item.BusinessTripApplicationId == null)
                        {
                            result.ErrorCodes.Add(-1);
                            result.Messages.Add($"Cannot find BusinessTripApplicationId with BTA Detail -> {item.Id}");
                            return Ok(result);
                        }

                        var findBTA = await bo.GetById(item.BusinessTripApplicationId.Value);
                        if (findBTA == null)
                        {
                            result.ErrorCodes.Add(-1);
                            result.Messages.Add($"Cannot find BusinessTripApplication with Id -> {item.BusinessTripApplicationId}");
                            return Ok(result);
                        }
                        var isPassport = (findBTA.Type == BTAType.International || (item.IsForeigner && findBTA.Type == BTAType.Domestic));
                        if (isPassport)
                        {
                            traveler.DocumentType = "PP";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.IDCard))
                            {
                                traveler.DocumentType = "PID";
                                traveler.DocumentNumber = item.IDCard;
                            } else
                            {
                                traveler.DocumentType = "PP";
                                traveler.DocumentNumber = item.Passport;
                            }
                        }

                        // traveler.DocumentType = null;
                        bookingTraveler.Traveler = traveler;
                        bookingTraveler.ServiceRequests = new List<ServiceRequest>();

                        bookingTravlerInfos.Add(bookingTraveler);
                        //var objDetailUser = await bo.GetDetailItemById(Guid.Parse(item.Id + string.Empty));
                        //if (objDetailUser != null && objDetailUser.Object != null)
                        //{
                        //	var currentUser = objDetailUser.Object;

                        //	BookingContact userInfo = new BookingContact()
                        //	{
                        //		Email = currentUser.GetPropertyValue("Email") + string.Empty,
                        //		FirstName = item.FirstName,
                        //		SurName = item.LastName,
                        //		PhoneNumber1 = currentUser.GetPropertyValue("Mobile") + string.Empty,
                        //		PhoneCode1 = "84",
                        //		BookingNumber = bookingNumber
                        //	};
                        //	bookingContact.Add(userInfo);

                        //}
                    }
                }
                AddBookingTravellerResquest objRequest = new AddBookingTravellerResquest();
                TaxReceiptRequestInfo taxReceipt = new TaxReceiptRequestInfo()
                {
                    TaxReceiptRequest = false,
                    BookingNumber = bookingNumber
                };
                //lamnl
                List<OsiCodesRequestInfo> osiCodesInfos = new List<OsiCodesRequestInfo>();
                if (travellerArgs.airlines != null)
                {
                    var airliness = JsonConvert.DeserializeObject<List<AirlineCodeInfo>>(travellerArgs.airlines);
                    foreach (var airline in airliness)
                    {
                        if (airline.AirlineCode == "VN" || airline.AirlineCode == "BL")
                        {
                            var CA_VNA = ConfigurationManager.AppSettings["CA_VNA"];
                            OsiCodesRequestInfo osiCodesInfo = new OsiCodesRequestInfo()
                            {
                                Direction = airline.Direction,
                                OsiCode = CA_VNA,
                                TourCode = CA_VNA,
                            };
                            osiCodesInfos.Add(osiCodesInfo);

                        }
                    }
                }
                //end
                objRequest.BookingNumber = bookingNumber;
                objRequest.TaxReceiptRequest = taxReceipt;
                objRequest.BookingContacts = bookingContact;
                objRequest.OsiCodes = osiCodesInfos;
                objRequest.BookingTravelerInfos = bookingTravlerInfos;

                _logger.LogError("Info at: AddBookingTraveller " + JsonConvert.SerializeObject(objRequest));
                bo.SaveBTALog(travellerArgs.btaDetailItem[0].BusinessTripApplicationId, "Info at: AddBookingTraveller " + JsonConvert.SerializeObject(objRequest));

                var revalidateResult = await airTicketsApi.AddBookingTraveller(objRequest);

                if (revalidateResult is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Add booking traveller failed. Please try again");
                    bo.SaveBTALog(travellerArgs.btaDetailItem[0].BusinessTripApplicationId, "Info at: AddBookingTraveller null ");
                }
                else
                {
                    result.Object = revalidateResult;
                    bo.SaveBTALog(travellerArgs.btaDetailItem[0].BusinessTripApplicationId, "Info at: AddBookingTraveller Res " + JsonConvert.SerializeObject(revalidateResult));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                object errorContent = ex.GetPropertyValue("Content");
                if (errorContent != null)
                {
                    try
                    {
                        var content = JsonConvert.DeserializeObject<AddBookingTravellerResponse>(errorContent.ToString());
                        if (content != null)
                        {
                            return Ok(new ResultDTO { ErrorCodes = new List<int>() { -1 }, Object = content });
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("AddBookingTraveller Catch ERR Parse Content " + e.Message + ". " + e.StackTrace);
                        bo.SaveBTALog(travellerArgs.btaDetailItem[0].BusinessTripApplicationId, $"AddBookingTraveller Catch ERR Parse Content {errorContent.ToString()}");
                    }
                }
                bo.SaveBTALog(travellerArgs.btaDetailItem[0].BusinessTripApplicationId, "AddBookingTraveller Catch ERR " + travellerArgs.bookingNumber + " mes: " + ex.Message);
                _logger.LogError("Error at: AddBookingTraveller " + ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { ex.Message } });
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> FilterAvailability(int PageNum, FilterAvailabilityRequest bodyQuery)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                FilterAvailabilityQueryParams headQuery = new FilterAvailabilityQueryParams();
                headQuery.IncludeEquivfare = false;
                headQuery.Page = PageNum;
                headQuery.Size = 15;
                headQuery.Sort = "stopOptions,asc";

                var filterResult = await airTicketsApi.FilterAvailability(headQuery, bodyQuery);
                if (filterResult is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("filter detail failed. Please try again");
                }
                else
                {
                    result.Object = filterResult;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: FilterAvailability", ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CommitBooking(CommitBooking args)
        {
            var result = new ResultDTO();
            var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
            try
            {
                _logger.LogInformation("Info at: CommitBooking: " + args);
                if (args.btaDetailItem != null)
                {
                    var checkIsCommit = await bo.CheckCommitBookingForBTADetail(args.btaDetailItem);
                    if (!checkIsCommit)
                    {
                        result.ErrorCodes.Add(0);
                        result.Messages.Add("BTA has isCommitBooking!");
                        return Ok(result);
                    }
                }
                
                _logger.LogInformation("Info at: key: " + args.key + " data:" + args.data + " bookingnumber: " + args.booking_number);
                args.secret_key = RSAHelper.RandomSecretKey();
                args.GenerateKey();
                args.GenerateData();
                bo.SaveBTALog(args.btaDetailItem[0].BusinessTripApplicationId, "Info at CommitBooking: key: " + args.key + " data:" + args.data + " bookingnumber: " + args.booking_number);
                _logger.LogInformation("Info at: key: " + args.key + " data:" + args.data + " bookingnumber: " + args.booking_number);
                #region [AEON-3301] Check lỗi xuất vé 2 lần trên PROD (Khóa lại không cần biết call được hay không)
                await bo.UpdateBeforeCommitBookingForBTADetail(args.btaDetailItem, args.booking_number, args.booking_code, true);
                #endregion
                var revalidateResults = await airTicketsApi.CommitBooking(new CommitBookingRequest()
                {
                    key = args.key,
                    data = args.data
                });

                if (revalidateResults is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Commit booking ticket failed. Please try again");
                    bo.SaveBTALog(args.btaDetailItem[0].BusinessTripApplicationId, "Commit Booking null ");
                }
                else
                {
                    bo.SaveBTALog(args.btaDetailItem[0].BusinessTripApplicationId, "Commit Booking Response: " + JsonConvert.SerializeObject(revalidateResults));
                    var revalidateResult = JsonConvert.DeserializeObject<FarerulesResponse>(revalidateResults.ToString());

                    #region 
                    var parseModelCommit = JsonConvert.DeserializeObject<ModelDataCommit>(revalidateResults.ToString());
                    var keyCommit = ConvertBase64(parseModelCommit.Key);
                    var dataCommit = ConvertBase64(parseModelCommit.Data);

                    GotadiSettingsSection GOTADI_CONFIG = (GotadiSettingsSection)ConfigurationManager.GetSection("airTicketSettings");
                    string privateKeyGotadi = RSAHelper.GetKeyToString(GOTADI_CONFIG.Certificates.AEON_BTA_Private_Key);
                    privateKeyGotadi = privateKeyGotadi.Replace("\r", "").Replace("\n", "\n");

                    RSA privateKey = CreateCipherDecrypt(privateKeyGotadi);
                    byte[] randomKey = DecryptRSAToByte(keyCommit, privateKey);
                    string originalData = CryptoUtils.DecryptTripleDes(dataCommit, randomKey);
                    bo.SaveBTALog(args.btaDetailItem[0].BusinessTripApplicationId, $"Convert API Response Commit: {originalData}");
                    var parseResponseGotadi = originalData.Split('|').ToList();
                    var errorCode = parseResponseGotadi[2].ToString();

                    var findErrorCode = await bo.GetBTAErrorMessageByCode(BTAErrorEnums.CommitBooking, errorCode);
                    if (findErrorCode != null)
                    {
                        string showMessage = string.Format("{0} => {1} (2)", findErrorCode.ErrorCode, findErrorCode.MessageVI, findErrorCode.MessageEN);
                        result.Messages = new List<string>() { showMessage };
                    } else
                    {
                        // Success
                        if (!errorCode.Equals("00"))
                            result.ErrorCodeStr.Add(errorCode);

                        switch (errorCode)
                        {
                            case "00":
                                // result.Messages.Add("Yêu cầu đã được xử lý thành công.");
                                break;
                            case "01":
                                result.ErrorCodes.Add(01);
                                result.Messages.Add("01. Yêu cầu đang được xử lý.");
                                break;
                            case "02":
                                result.ErrorCodes.Add(02);
                                result.Messages.Add("02. Yêu cầu đã được xử lý thất bại.");
                                break;
                            case "03":
                                result.ErrorCodes.Add(03);
                                result.Messages.Add("03. Yêu bị từ chối do Xác thực tài khoản đại lý thất bại.");
                                break;
                            case "04":
                                result.ErrorCodes.Add(04);
                                result.Messages.Add("04. Yêu bị từ chối do Chữ ký điện tử không hợp lệ.");
                                break;
                            case "05":
                                result.ErrorCodes.Add(05);
                                result.Messages.Add("05. Yêu bị từ chối do Giải mã dữ liệu không thành công.");
                                break;
                            case "06":
                                result.ErrorCodes.Add(06);
                                result.Messages.Add("06. Yêu bị từ chối do Mã xác thực (Access Code) không hợp lệ.");
                                break;
                            case "07":
                                result.ErrorCodes.Add(07);
                                result.Messages.Add("07. Yêu bị từ chối do Dữ liệu sai định dạng.");
                                break;
                            case "08":
                                result.ErrorCodes.Add(08);
                                result.Messages.Add("08. Yêu bị từ chối do Đã được xử lý trước đó.");
                                break;
                            case "09":
                                result.ErrorCodes.Add(09);
                                result.Messages.Add("09. Yêu cầu chưa được xử lý.");
                                break;
                            case "10":
                                result.ErrorCodes.Add(10);
                                result.Messages.Add("10. Yêu cầu commit booking bị từ chối do bị trùng thông tin với booking khác đã gọi commit trước đó.");
                                break;
                            case "99":
                            default:
                                result.ErrorCodes.Add(99);
                                result.Messages.Add("99. Lỗi khác.");
                                break;
                        }
                    }

                    #endregion

                    BookingDetailQueryParams checkStatusPr = new BookingDetailQueryParams() { BookingNumber = args.booking_number };
                    var revalidateResultStatus = await airTicketsApi.GetBookingDetail(checkStatusPr);
                    bool status = false;
                    if (revalidateResultStatus is null)
                    {
                        _logger.LogInformation("Info at: Get booking detail failed. Please try again");
                    }
                    else
                    {
                        bo.SaveBTALog(args.btaDetailItem[0].BusinessTripApplicationId, "Info at: Get booking detail Success Commit Booking: " + args.booking_number + " Status " + revalidateResultStatus.BookingInfo.Status + " PaymentStatus " + revalidateResultStatus.BookingInfo.PaymentStatus + " IssuedStatus " + revalidateResultStatus.BookingInfo.IssuedStatus);
                        _logger.LogInformation("Info at: Get booking detail: Status " + revalidateResultStatus.BookingInfo.Status + " PaymentStatus " + revalidateResultStatus.BookingInfo.PaymentStatus + " IssuedStatus " + revalidateResultStatus.BookingInfo.IssuedStatus);

                        if (revalidateResultStatus.BookingInfo.Status.Equals("BOOKED") && revalidateResultStatus.BookingInfo.PaymentStatus.Equals("SUCCEEDED")
                            && revalidateResultStatus.BookingInfo.IssuedStatus.Equals("SUCCEEDED"))
                        {
                            status = true;
                        }
                    }

                    await bo.UpdateBeforeCommitBookingForBTADetail(args.btaDetailItem, args.booking_number, args.booking_code, true);
                    revalidateResult.Success = status;
                    result.Object = revalidateResult;
                    _logger.LogInformation("Info at: CommitBooking detail: " + JsonConvert.SerializeObject(revalidateResult));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Info at: CommitBooking: " + ex.StackTrace);
                _logger.LogError("Error at: CommitBooking", ex.Message + ". " + ex.StackTrace);
                BookingDetailQueryParams checkStatusPr = new BookingDetailQueryParams() { BookingNumber = args.booking_number };
                var revalidateResultStatus = await airTicketsApi.GetBookingDetail(checkStatusPr);
                bool status = false;
                if (revalidateResultStatus is null)
                {
                    _logger.LogInformation("Info at: Get booking detail failed. Please try again");
                }
                else
                {
                    bo.SaveBTALog(args.btaDetailItem[0].BusinessTripApplicationId, "Info at: Get booking detail Error Commit Booking: " + args.booking_number + " Status " + revalidateResultStatus.BookingInfo.Status + " PaymentStatus " + revalidateResultStatus.BookingInfo.PaymentStatus + " IssuedStatus " + revalidateResultStatus.BookingInfo.IssuedStatus);
                    _logger.LogInformation("Info at: Get booking detail: Status " + revalidateResultStatus.BookingInfo.Status + " PaymentStatus " + revalidateResultStatus.BookingInfo.PaymentStatus + " IssuedStatus " + revalidateResultStatus.BookingInfo.IssuedStatus);

                    if (revalidateResultStatus.BookingInfo.Status.Equals("BOOKED") && revalidateResultStatus.BookingInfo.PaymentStatus.Equals("SUCCEEDED")
                        && revalidateResultStatus.BookingInfo.IssuedStatus.Equals("SUCCEEDED"))
                    {
                        status = true;
                    }
                }
                bo.SaveBTALog(args.btaDetailItem[0].BusinessTripApplicationId, "Commit Booking Catch: " + ex.Message);

                var updateBtaDetail = await bo.UpdateBeforeCommitBookingForBTADetail(args.btaDetailItem, args.booking_number, args.booking_code, true);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { ex.Message + ". " + ex.StackTrace + " key: " + args.key } });
            }
        }

        public static byte[] DecryptRSAToByte(string encryptedKey, RSA privateKey)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedKey);
            return privateKey.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
        }

        public static RSA CreateCipherDecrypt(string privateKeyPem)
        {
            using (TextReader reader = new StringReader(privateKeyPem))
            {
                PemReader pemReader = new PemReader(reader);
                AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);

                RSA rsa = RSA.Create();
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
        }

        public string ConvertBase64(string data)
        {
            string _return = data.Replace('-', '+').Replace('_', '/');

            // Thêm padding nếu thiếu
            switch (_return.Length % 4)
            {
                case 2: _return += "=="; break;
                case 3: _return += "="; break;
            }
            return _return;
        }

        private class ModelDataCommit
        {
            public string Data { get; set; }
            public string Key { get; set; }
        }
        private string TrimCharacters(string textTrim)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d",
                "e","e","e","e","e","e","e","e","e","e","e",
                "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u",
                "y","y","y","y","y",};

            for (int i = 0; i < arr1.Length; i++)
            {
                textTrim = textTrim.Replace(arr1[i], arr2[i]);
                textTrim = textTrim.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return textTrim;
        }

        [HttpPost]
        public async Task<IHttpActionResult> GroupIntinerary(int PageNum, GroupIntineraryRequest bodyQuery)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                GroupItineraryQueryParams headQuery = new GroupItineraryQueryParams();
                headQuery.Page = PageNum;
                headQuery.Size = 15;
                headQuery.Sort = "stopOptions,asc";

                var filterResult = await airTicketsApi.GroupIntinerary(bodyQuery.Filter.GroupId, headQuery, bodyQuery);
                if (filterResult is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("GroupI tinerary failed. Please try again");
                }
                else
                {
                    var jdson = JsonConvert.SerializeObject(filterResult);
                    result.Object = filterResult;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: GroupItinerary", ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> SearchCountry(string queryText)
        {
            try
            {
                var result = new ResultDTO();
                var airTicketsApi = RestApiHelper.GetRestServiceApi<IAirTicketsApi>();
                SearchCountryParams searchCountryParams = new SearchCountryParams();
                searchCountryParams.page = "0";
                searchCountryParams.size = "1000";
                searchCountryParams.sort = "sortname,asc";

                var searchCountry = await airTicketsApi.SearchCountryCodes(searchCountryParams);

                if (!string.IsNullOrEmpty(queryText))
                {
                    searchCountry = searchCountry.Where(x => x != null && x.Name.ToLower().Contains(queryText.ToLower())).OrderBy(y => y.Name).ToList();
                }

                if (searchCountry is null)
                {
                    result.ErrorCodes.Add(0);
                    result.Messages.Add("Search airport failed. Please try again");
                }
                else
                {
                    result.Object = searchCountry;
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: SearchCountry", ex.Message + ". " + ex.StackTrace);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" + ex.Message + ". " + ex.StackTrace } });
            }
        }

        private string TrimCharactersV2(string textTrim)
        {
            string normalized = textTrim.Normalize(NormalizationForm.FormD);

            // Sử dụng Regex để loại bỏ các ký tự không phải ASCII (các dấu)
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string cleaned = regex.Replace(normalized, string.Empty);

            // Chuẩn hóa lại về dạng chuẩn Form C
            return cleaned.Normalize(NormalizationForm.FormC).ToUpper();
        }

        private bool HasDiacritics(string text)
        {
            // Regex kiểm tra các ký tự dấu (diacritical marks)
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");

            // Chuyển chuỗi về dạng Normalization Form D để tách các ký tự và dấu
            string normalized = text.Normalize(NormalizationForm.FormD);

            // Kiểm tra xem chuỗi đã chuẩn hóa có chứa ký tự dấu hay không
            return regex.IsMatch(normalized);
        }
    }
}