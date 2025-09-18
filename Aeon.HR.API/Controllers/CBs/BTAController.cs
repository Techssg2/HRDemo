using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.CBs;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.API.Helpers;
using Aeon.HR.ViewModels.Args;
using QueryArgs = Aeon.HR.Infrastructure.QueryArgs;
using Aeon.HR.BusinessObjects.Handlers;
using Newtonsoft.Json;

namespace Aeon.HR.API.Controllers.CBs
{
    public class BTAController : BaseController
    {
        protected readonly IBusinessTripBO bo;
        public BTAController(ILogger _logger, IBusinessTripBO _bo) : base(_logger)
        {
            bo = _bo;
        }
        [HttpPost]
        public async Task<IHttpActionResult> Save(BusinessTripDTO data)
        {
            try
            {
                var res = await bo.Save(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Save BTA " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetList(QueryArgs arg)
        {
            try
            {
                var res = await bo.GetList(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetList BTA " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetItemById(Guid id)
        {
            try
            {
                var res = await bo.GetItemById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetItemById BTA " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveRoomOrganization(RoomOrganizationArg arg)
        {
            try
            {
                var res = await bo.SaveRoomOrganization(arg.BusinessTripApplicationId, arg.Data, arg.IsChange);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveRoomOrganization BTA " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetReports(ReportType type, QueryArgs args)
        {
            try
            {
                var res = await bo.GetReports(type, args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetReports BTA " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetRevokingBTA(RevokingArg args)
        {
            try
            {
                var res = await bo.GetRevokingBTA(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetRevokingBTA " + ex.Message);
                return BadRequest("Error System");
            }

        }
        [HttpPost]
        public async Task<IHttpActionResult> GetDetailUsersInRoom(ReportType type, string Code, QueryArgs args)
        {
            try
            {
                var res = await bo.GetDetailUsersInRoom(type, Code, args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDetailUsersInRoom BTA " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetEmployeeTripGroup(Guid id)
        {
            try
            {
                var res = await bo.GetTripGroups(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: BTAPrintForm " + ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Validate(BusinessTripValidateArg arg)
        {
            try
            {
                var res = await bo.Validate(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Validate " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> Export(ReportType type, [FromBody] Aeon.HR.ViewModels.Args.ExportReportArg args)
        {
            try
            {

                var arrayContent = await bo.ExportReport(type, args);
                if (arrayContent.Object == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent.Object,
                    FileName = string.Format("{0} Requests_{1}.xlsx", FileProcessingType.BUSINESSTRIPAPPLICATIONREPORT.GetEnumDescription(), DateTime.Now.ToLocalTime().ToString("dd_MM_yyyy HH:mm:ss")),
                    Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: {type.ToString()} " + ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ExportTypeStatus(ReportType type, [FromBody] ViewModels.Args.ExportReportArg args)
        {
            try
            {

                var arrayContent = await bo.ExportTypeStatus(type, args);
                if (arrayContent.Object == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent.Object,
                    FileName = string.Format("{0} Requests_{1}.xlsx", FileProcessingType.BUSINESSTRIPAPPLICATIONREPORT.GetEnumDescription(), DateTime.Now.ToLocalTime().ToString("dd_MM_yyyy HH:mm:ss")),
                    Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: {type.ToString()} " + ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> PrintForm(Guid id)
        {
            try
            {

                var arrayContent = await bo.PrintForm(id);
                if (arrayContent == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent,
                    FileName = string.Format("Record-{0}.pdf", id),
                    Type = "Application/pdf"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: BTAPrintForm " + ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }


        [HttpPost]
        public async Task<IHttpActionResult> SavePassengerInfo(List<BTAPassengerViewModel> arg)
        {
            try
            {
                var res = await bo.SavePassengerInfo(arg.ToList());
                return Ok(res);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Validate " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveBookingInfo(List<BookingFlightViewModel> arg)
        {
            try
            {
                var res = await bo.SaveBookingInfo(arg.ToList());
                return Ok(res);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveBookingInfo" + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveRevokeInfo(RevokeBTAInfoViewModel data)
        {
            try
            {
                var res = await bo.SaveRevokeInfo(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveRevokeInfo " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveCancellationFee_Revoke(RevokeBTAInfoViewModel data)
        {
            try
            {
                var res = await bo.SaveCancellationFee_Revoke(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveCancellationFee " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveCancellationFee_Changing(RevokeBTAInfoViewModel data)
        {
            try
            {
                var res = await bo.SaveCancellationFee_Changing(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveCancellationFee_Changing " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetPassengerInformationBySAPCodes(List<string> arg)
        {
            try
            {
                var res = await bo.GetPassengerInformationBySAPCodes(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPassengerInfo " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public IHttpActionResult GetUserTicketsInfo(Guid BTADetailId)
        {
            try
            {
                var res = bo.GetUserTicketsInfo(BTADetailId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetUserTicketsInfo", ex.Message);
                return BadRequest("Error System");
            }
        }
        //lamnl 
        [HttpPost]
        public async Task<IHttpActionResult> GetBtaRoomHotel(Guid id)
        {
            try
            {
                var res = await bo.GetBtaRoomHotel(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Get Room Hotel", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public IHttpActionResult CheckBookingCompleted(BtaDTO agrs)
        {
            try
            {
                var res = bo.CheckBookingCompleted(agrs);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: Check Booking", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckAdminDept(BTAAdminDeptDTO agrs)
        {
            try
            {
                var res = await bo.CheckAdminDept(agrs);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: Check Admin Dept", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveBTADetail(BusinessTripDTO data)
        {
            try
            {
                var res = await bo.SaveBTADetail(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Save BTA", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SendEmailDeleteRows(BtaDTO agrs)
        {
            try
            {
                var res = await bo.SendEmailDeleteRows(agrs);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SendEmail Delete BTA", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckRoomHotel(BtaDTO Agrs)
        {
            try
            {
                var res = await bo.CheckRoomHotel(Agrs);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Check Room Hotel", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteTripGroup(CommonDTO agrs)
        {
            try
            {
                var res = await bo.DeleteTripGroup(agrs);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Delete Trip Group", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveRoomBookingFlight(RoomOrganizationArg arg)
        {
            try
            {
                var res = await bo.SaveRoomBookingFlight(arg.BusinessTripApplicationId, arg.Data, arg.TripGroup, arg.IsChange);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveRoomOrganization BTA", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetFareRulesByFareSourceCode(FareRulesRequestArgs args)
        {
            try
            {
                var res = await bo.GetFareRulesByFareSourceCode(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Check Room Hotel", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetBTAApprovedDay(Guid businessTripApplicationId)
        {
            try
            {
                var res = await bo.GetBTAApprovedDay(businessTripApplicationId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBTAApprovedDay", ex.Message);
                return BadRequest("Error System");
            }
        }


        [HttpPost]
        public async Task<IHttpActionResult> ValidatationBTADetails(ValidateBTADetailsArgs btaDetails)
        {
            try
            {
                var res = await bo.ValidationBTADetails(btaDetails);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ValidatationBTADetails", ex.Message);
                return BadRequest("Error System");
            }
        }

        
    }
}
