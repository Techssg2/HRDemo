using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SSG2.API.Controllers.CBs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.CBs
{

    public class TargetPlanController : CandBController
    {
        protected readonly ITargetPlanBO _target;
        private string _uploadedFilesFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Attachments");
        public TargetPlanController(ILogger logger, ICBBO cbBO, ITargetPlanBO target) : base(logger, cbBO)
        {
            _target = target;
        }
        //[HttpPost]
        //public async Task<IHttpActionResult> GetTargetPlanFromDepartment(Guid departmentId)
        //{
        //    try
        //    {
        //        var res = await _target.GetTargetPlanFromDepartment(departmentId);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error at method: GetTargetPlanFromDepartment {0}", ex.Message);
        //        return BadRequest("Error System");
        //    }
        //}

        [HttpPost]
        public async Task<IHttpActionResult> GetPendingTargetPlanFromDepartmentAndPeriod(TargetPlanQueryArg args)
        {
            try
            {
                var res = await _target.GetPendingTargetPlanFromDepartmentAndPeriod(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTargetPlanFromDepartmentAndPeriod {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetPendingTargetPlanDetailFromSAPCodesAndPeriod(TargetPlanQuerySAPCodeArg args)
        {
            try
            {
                var res = await _target.GetPendingTargetPlanDetailFromSAPCodesAndPeriod(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTargetPlanFromDepartmentAndPeriod {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetValidUserSAPForTargetPlan(Guid id, Guid userId, Guid periodId)
        {
            try
            {
                var res = await _target.GetValidUserSAPForTargetPlan(id, userId, periodId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetValidUserSAPForTargetPlan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetTargetPlanDetails(Guid id)
        {
            try
            {
                var res = await _target.GetPendingTargetPlanDetails(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTargetPlanDetails {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SavePendingTargetPlan(bool isSend, TargetPlanArg arg)
        {
            try
            {
                //Khiem - fixed UTC datetime
                arg.PeriodFromDate = arg.PeriodFromDate.LocalDateTime;
                arg.PeriodToDate = arg.PeriodToDate.LocalDateTime;
                var res = await _target.SavePendingTargetPlan(isSend, arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveTargetPlan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SavePendingBeforeSubmit(TargetPlanArg arg)
        {
            try
            {
                //Khiem - fixed UTC datetime
                arg.PeriodFromDate = arg.PeriodFromDate.LocalDateTime;
                arg.PeriodToDate = arg.PeriodToDate.LocalDateTime;
                var res = await _target.SavePendingBeforeSubmit(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveTargetPlan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SendRequest_TargetPlan(bool isSend, TargetPlanArg arg)
        {
            try
            {
                var res = await _target.SendRequest_TargetPlan(isSend, arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SendRequest_TargetPlan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetTargetPlanPeriods()
        {
            try
            {
                var res = await _target.GetTargetPlanPeriods();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTargetPlanPeriods {0}", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetActualShiftPlan(ActualTargetPlanArg args)
        {
            try
            {
                var res = await _target.GetActualShiftPlan(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetActualShiftPlan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        // not used
        //[HttpPost]
        //public async Task<IHttpActionResult> SubmitTargetPlanToSAP(ActualTargetPlanArg arg)
        //{
        //    try
        //    {
        //        var res = await _target.SubmitTargetPlanToSAP(arg);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error at method: SubmitTargetPlanToSAP {0}", ex.Message);
        //        return BadRequest("Error System");
        //    }
        //}

        [HttpPost]
        public async Task<IHttpActionResult> Submit(SubmitDataArg arg)
        {
            try
            {
                var res = await _target.Submit(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Submit {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdatePermissionUserInTargetPlanItem(Guid id)
        {
            try
            {
                var res = await _target.UpdatePermissionUserInTargetPlanItem(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdatePermissionUserInTargetPlanItem {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetTargetPlanDetailIsCompleted(TargetPlanDetailQueryArg arg)
        {
            try
            {
                var res = await _target.GetTargetPlanDetailIsCompleted(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetTargetPlanDetailIsCompleted {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SetSubmittedStateForDetailPendingTargetPlan(SubmitDetailPendingTartgetPlanSAPArg arg)
        {
            try
            {
                var res = await _target.SetSubmittedStateForDetailPendingTargetPlan(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SetSubmittedStateForDetailPendingTargetPlan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetList(QueryArgs arg)
        {
            try
            {
                var res = await _target.GetList(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetList", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetItem(Guid id)
        {
            try
            {
                var res = await _target.GetItem(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetItem", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetSAPCode_PendingTargetPlanDetails(Guid id)
        {
            try
            {
                var res = await _target.GetSAPCode_PendingTargetPlanDetails(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetItem", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ValidateTargetPlan(TargetPlanArg arg)
        {
            try
            {
                var res = await _target.ValidateTargetPlan(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ValidateTargetPlan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ValidateTargetPlanV2(TargetPlanArg arg)
        {
            try
            {
                var res = await _target.ValidateTargetPlanV2(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ValidateTargetPlan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> Import(Guid deptId, Guid periodId, string sapCodes, bool visibleSubmit, Guid? divisionId = null)
        {
            try
            {
                var arg = new TargetPlanQuerySAPCodeArg
                {
                    DeptId = deptId,
                    DivisionId = divisionId,
                    PeriodId = periodId,
                    VisibleSubmit = visibleSubmit,
                    SAPCodes = sapCodes.Split(',').ToList()
                };
                Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists
                MemoryStream content = new MemoryStream();
                var test = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                await test.CopyToAsync(content);
                var result = await _target.UploadData(arg, content);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at: Import ", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> PostFileWithData()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var arg = new TargetPlanQuerySAPCodeArg();
            //var root = HttpContext.Current.Server.MapPath("~/App_Data/Uploadfiles");
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

                var uploadData = JsonConvert.DeserializeObject<UploadTargetPlanDTO>(model);
                if (uploadData != null)
                {
                    arg.DeptId = uploadData.DeptId;
                    arg.DivisionId = uploadData.DivisionId;
                    arg.PeriodId = uploadData.PeriodId;
                    arg.VisibleSubmit = uploadData.VisibleSubmit;
                    arg.SAPCodes = uploadData.SAPCodes.Split(',').ToList();
                };
            }
            //TODO: Do something with the JSON data.  
            var file = result.FileData[0];
            var byteArray = File.ReadAllBytes(file.LocalFileName);
            MemoryStream content = new MemoryStream();
            content.Write(byteArray, 0, byteArray.Length);
            var res = await _target.UploadData(arg, content);
            return Ok(res);
        }

        [HttpPost]
        public async Task<ResultDTO> GetPermissionInfoOnPendingTargetPlan(TargetPlanQueryPermissionInfo arg)
        {
            var result = new ResultDTO { };
            try
            {
                result = await _target.GetPermissionInfoOnPendingTargetPlan(arg);
                return result;
            }
            catch (Exception ex)
            {
                result.Messages.Add("Not Found");
                result.ErrorCodes.Add(1004);
            }
            return result;
        }

        [HttpPost]
        public async Task<IHttpActionResult> CancelPendingTargetPLan(CancelPendingTargetArg arg)
        {
            try
            {
                var res = await _target.CancelPendingTargetPLan(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CancelPendingTargetPLan {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> RequestToChange(TargetPlanRequestToChangeArg arg)
        {
            try
            {
                var res = await _target.RequestToChange(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: RequestToChange {0}", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> PrintForm(TargetPlanDetailQueryArg args)
        {
            try
            {

                var arrayContent = await _target.PrintForm(args);
                if (arrayContent == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent,
                    FileName = string.Format("Record-{0}.xlsx", args.PeriodId),
                    Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Print Form: Shift Plan ", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DownloadTemplate(DataToPrintTemplateArgs args)
        {
            try
            {
                var arrayContent = await _target.DownloadTemplate(args);
                if (arrayContent == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent,
                    FileName = "Template.xlsx",
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
        public async Task<IHttpActionResult> GetPendingTargetPlans(QueryArgs arg)
        {
            try
            {
                var res = await _target.GetPendingTargetPlans(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPendingTargetPlans {0}", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> TargetPlanReport(Guid departmentId, Guid periodId, int limit, int page, string searchText, bool isMade = false)
        {
            try
            {
                var res = await _target.TargetPlanReport(departmentId, periodId, limit, page, searchText, isMade);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: TargetPlanReport {0}", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ValidateSubmitPendingTargetPlan(ValidateExistTargetPlanArgs args)
        {
            try
            {
                var res = await _target.ValidateSubmitPendingTargetPlan(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: TargetPlanReport {0}", ex.Message);
                return BadRequest("Error System");
            }
        }
    }
}