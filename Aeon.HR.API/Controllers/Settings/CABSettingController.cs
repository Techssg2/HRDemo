using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace SSG2.API.Controllers.Settings
{
    [RoutePrefix("api/CABSetting")]
    public class CABSettingController : SettingController
    {
        private string _uploadedFilesFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Attachments");
        public CABSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        [Route("GetReasons")]
        public async Task<IHttpActionResult> GetReasons([FromBody] MasterdataArgs args)
        {
            try
            {
                var result = await _settingBO.GetReasons(args);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetReasons " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        [Route("AddReason")]
        public async Task<IHttpActionResult> AddReason([FromBody] object args)
        {
            try
            {
                var result = await _settingBO.AddReason(args);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: AddReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        [Route("UpdateReason")]
        public async Task<IHttpActionResult> UpdateReason([FromBody] object args)
        {
            try
            {
                var result = await _settingBO.UpdateReason(args);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        [Route("DeleteReason")]
        public async Task<IHttpActionResult> DeleteReason([FromBody] object args)
        {
            try
            {
                var result = await _settingBO.DeleteReason(args);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        [Route("SearchReason/{type}/{searchString}")]
        public async Task<IHttpActionResult> SearchReason(string searchString, string type)
        {
            try
            {
                var result = await _settingBO.SearchReason(searchString, type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchReason " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetAllReason([FromBody] MetadataTypeArgs arg)
        {
            try
            {
                var result = await _settingBO.GetAllReason(arg);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetShiftPlanSubmitPersons([FromBody] QueryArgs arg)
        {
            try
            {
                var result = await _settingBO.GetShiftPlanSubmitPersons(arg);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetShiftPlanSubmitPersonById([FromBody] ShiftPlanSubmitPersonArg arg)
        {
            try
            {
                var result = await _settingBO.GetShiftPlanSubmitPersonById(arg.DepartmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateShiftPlanSubmitPerson([FromBody] ShiftPlanSubmitPersonArg arg)
        {
            try
            {
                var result = await _settingBO.CreateShiftPlanSubmitPerson(arg);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteShiftPlanSubmitPerson([FromBody] ShiftPlanSubmitPersonArg arg)
        {
            try
            {
                var result = await _settingBO.DeleteShiftPlanSubmitPerson(arg.DepartmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetDepartmentTargetPlansByUserId(Guid id)
        {
            try
            {
                var res = await _settingBO.GetDepartmentTargetPlans(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentsByUserSubmitPlanId " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> FilterDivisionByUserNotSubmit([FromBody] DepartmentTargetPlanViewModel model)
        {
            try
            {
                var res = await _settingBO.FilterDivisionByUserNotSubmit(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: FilterDivisionByUserNotSubmit " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckIsSubmitPersonOfDepartment(Guid divisionId, string currentUserSapCode)
        {
            try
            {
                var res = await _settingBO.CheckIsSubmitPersonOfDepartment(divisionId, currentUserSapCode);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CheckIsSubmitPersonOfDepartment " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckSubmitPersonFromDepartmentId(Guid id)
        {
            try
            {
                var res = await _settingBO.CheckSubmitPersonFromDepartmentId(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CheckSubmitPersonFromDepartmentId " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckDepartmentExist([FromBody] ShiftPlanSubmitPersonArg arg)
        {
            try
            {
                var result = await _settingBO.CheckDepartmentExist(arg.DepartmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllReason " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveShiftPlanSubmitPerson([FromBody] ShiftPlanSubmitPersonArg arg)
        {
            try
            {
                var result = await _settingBO.SaveShiftPlanSubmitPerson(arg);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllReason " + ex.Message);
                return BadRequest("Error System");
            }
        }
        #region CABSetting - Holiday Schedule
        
        [HttpPost]
        public async Task<ResultDTO> GetHolidaySchedules([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _settingBO.GetHolidaySchedules(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetHolidaySchedules " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateHolidaySchedule(HolidayScheduleArgs data)
        {
            try
            {
                var res = await _settingBO.CreateHolidaySchedule(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateHolidaySchedule " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateHolidaySchedule(HolidayScheduleArgs data)
        {
            try
            {
                var res = await _settingBO.UpdateHolidaySchedule(data);
                if (res.Object == null)
                    return new ResultDTO { Object = null, ErrorCodes = { 1002 }, Messages = { "Error System" } };
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateHolidaySchedule " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> DeleteHolidaySchedule(HolidayScheduleArgs data)
        {
            try
            {
                var res = await _settingBO.DeleteHolidaySchedule(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteHolidaySchedule " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetYearHolidays()
        {
            try
            {
                var res = await _settingBO.GetYearHolidays();
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetHolidaySchedules " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion


        #region CABSetting - Shift Code

        [HttpPost]
        public async Task<ResultDTO> GetDataShiftCode([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _settingBO.GetDataShiftCode(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDataShiftCode " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> CreateShiftCode(ShiftCodeAgrs data)
        {
            try
            {
                var res = await _settingBO.CreateShiftCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateShiftCode " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> UpdateShiftCode(ShiftCodeAgrs data)
        {
            try
            {
                var res = await _settingBO.UpdateShiftCode(data);
                if (res.Object == null)
                    return new ResultDTO { Object = null, ErrorCodes = { 1002 }, Messages = { "Error System" } };
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateHolidaySchedule " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> DeleteShiftCode(ShiftCodeAgrs data)
        {
            try
            {
                var res = await _settingBO.DeleteShiftCode(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteShiftCode " + ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DownloadTemplate()
        {
            try
            {
                var arrayContent = await _settingBO.DownloadTemplate();
                if (arrayContent == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent,
                    FileName = "ShiftCode_Template.xlsx",
                    Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at Print Form: Shift Plan {0}", string.IsNullOrEmpty(ex.Message) ? ex.InnerException.Message : ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadData()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var arg = new ShiftCodeAgrs();
            Directory.CreateDirectory(_uploadedFilesFolder);
            var provider = new MultipartFormDataStreamProvider(_uploadedFilesFolder);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            var model = result.FormData["argData"];
            if (model == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            else
            {
                var uploadData = JsonConvert.DeserializeObject<ShiftCodeAgrs>(model);
                arg.Module = uploadData.Module;
                arg.FileName = uploadData.FileName;
            }
            //TODO: Do something with the JSON data.  
            var file = result.FileData[0];
            var byteArray = File.ReadAllBytes(file.LocalFileName);
            MemoryStream content = new MemoryStream();
            content.Write(byteArray, 0, byteArray.Length);
            var res = await _settingBO.UploadData(arg, content);
            return Ok(res);
        }

        #endregion

        #region cr9.9 them mang hinh targetplan - special for prd
        [HttpPost]
        public async Task<IHttpActionResult> GetTargetPlanSpecial([FromBody] QueryArgs arg)
        {
            try
            {
                var result = await _settingBO.GetTargetPlanSpecial(arg); // 
                //var result = new ResultDTO();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTargetPlanSpecial " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateTargetPlanSpecial([FromBody] TargetPlanSpecialArgs arg)
        {
            try
            {
                var result = await _settingBO.CreateTargetPlanSpecial(arg);
                //var result = new ResultDTO();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTargetPlanSpecial " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> DeleteTargetPlanSpecial(Guid Id)
        {
            try
            {
                var result = await _settingBO.DeleteTargetPlanSpecial(Id);
                //var result = new ResultDTO();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTargetPlanSpecial " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CheckDepartmentExistInSpecial([FromBody] TargetPlanSpecialArgs arg)
        {
            try
            {
                var result = await _settingBO.CheckDepartmentExistInSpecial(arg.DepartmentId);
                //var result = new ResultDTO();


                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CheckDepartmentExistInSpecial " + ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveTargetPlanSpecial([FromBody] TargetPlanSpecialArgs arg)
        {
            try
            {
                var result = await _settingBO.SaveTargetPlanSpecial(arg);
                //var result = new ResultDTO();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveTargetPlanSpecial " + ex.Message);
                return BadRequest("Error System");
            }
        }
        #endregion
    }
    }