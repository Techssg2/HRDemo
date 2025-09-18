using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Settings
{
    public class BookingContractController : SettingController
    {
        public BookingContractController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        [HttpPost]
        public async Task<IHttpActionResult> GetBookingContract(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetBookingContract(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBookingContract " + ex.Message);
                return BadRequest("System Error");
            }
        }
        public async Task<IHttpActionResult> UpdateBookingContract(BookingContractArgs args)
        {
            try
            {
                var res = await _settingBO.UpdateBookingContract(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateBookingContract " + ex.Message);
                return BadRequest("System Error");
            }
        }
    }
}
