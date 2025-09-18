using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SSG2.API.Controllers.Others
{

    public class FileController : BaseController
    {
        protected readonly IExcuteFileProcessing _excuteFileProcessing;
        public FileController(ILogger logger, IExcuteFileProcessing excuteFileProcessing) : base(logger)
        {
            _excuteFileProcessing = excuteFileProcessing;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Import(FileProcessingType type)
        {
            try
            {
                var content = HttpContext.Current.Request.GetBufferlessInputStream(true);
                var result = await _excuteFileProcessing.ImportAsync(type, content as FileStream);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at: Import {type.GetEnumDescription()}", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Export(FileProcessingType type, [FromBody] QueryArgs args)
        {
            try
            {
                var arrayContent = await _excuteFileProcessing.ExportAsync(type, args);
                if (arrayContent.Object == null)
                {
                    return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
                }
                var fileContent = new FileResultDto
                {
                    Content = arrayContent.Object,
                    FileName = string.Format("{0} Requests_{1}.xlsx", type.GetEnumDescription(), DateTime.Now.ToLocalTime().ToString("dd_MM_yyyy HH:mm:ss")),
                    Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
                return Ok(new ResultDTO { Object = fileContent });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: {type.ToString()}", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }
    }
}
