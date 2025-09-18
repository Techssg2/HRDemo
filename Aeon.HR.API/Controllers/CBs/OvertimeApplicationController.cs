using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.DTOs;
using System.Security;
using Aeon.HR.ViewModels.PrintFormViewModel;
using Aeon.HR.ViewModels;
using System.IO;
using System.Web;

namespace SSG2.API.Controllers.CBs
{
    public class OvertimeApplicationController : CandBController
    {
        public OvertimeApplicationController(ILogger logger, ICBBO cbBO) : base(logger, cbBO) { }
        #region CRUD METHOD
        [HttpPost]
        public async Task<IHttpActionResult> SaveOvertimeApplication(OvertimeApplicationArgs model)
        {
            try
            {
                var res = await _cbBO.SaveOvertimeApplication(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateOvertimeApplication", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateOvertimeApplication(OvertimeApplicationArgs model)
        {
            try
            {
                var res = await _cbBO.UpdateOvertimeApplication(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateOvertimeApplication", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetOvertimeApplicationById(Guid id)
        {
            try
            {
                var res = await _cbBO.GetOvertimeApplicationById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetOvertimeApplication", ex.Message);
                return BadRequest("System Error");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetOvertimeApplicationList(QueryArgs args)
        {
            try
            {
                var res = await _cbBO.GetOvertimeApplicationList(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetOvertimeApplication", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetOvertimeApplications(QueryArgs args)
        {
            try
            {
                var res = await _cbBO.GetOvertimeApplications(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetOvertimeApplications", ex.Message);
                return BadRequest("System Error");
            }
        }
        #endregion
        [HttpGet]
        public async Task<IHttpActionResult> PrintForm(Guid id)
        {
            try
            {

                var arrayContent = await _cbBO.PrintFormOvertime(id);
                if (arrayContent == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent,
                    FileName = string.Format("Record-{0}.xlsx", id),
                    Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Print Form: OverTime Application ", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }


        private string _uploadedFilesFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Attachments");
        #region Import file excel
        [HttpPost]
        public async Task<IHttpActionResult> Import(string sapCodes)
        {
            try
            {
                var arg = new OvertimeQuerySAPCodeArg
                {
                    SAPCodes = sapCodes.Split(',').ToList()
                };
                Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists
                MemoryStream content = new MemoryStream();
                var test = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                await test.CopyToAsync(content);
                var result = await _cbBO.UploadDataForOvertime(arg, content);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at: Import ", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ImportActual(string sapCodes)
        {
            try
            {
                var arg = new OvertimeQuerySAPCodeArg
                {
                    SAPCodes = sapCodes.Split(',').ToList()
                };
                Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists
                MemoryStream content = new MemoryStream();
                var test = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                await test.CopyToAsync(content);
                var result = await _cbBO.UploadActualDataForOvertime(arg, content);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at: ImportActual ", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }
        #endregion
    }
}
