using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using SSG2.API.Controllers.CBs;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace SSG2.API.Controllers.Recruitments
{
    public class RequestToHireController : RecruitmentController
    {
        private string _uploadedFilesFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Attachments");
        public RequestToHireController(ILogger logger, IRecruitmentBO recruitment) : base(logger, recruitment)
        {
        }
        #region RequestToHire
        [HttpPost]
        public async Task<IHttpActionResult> CreateRequestToHire([FromBody] RequestToHireDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.CreateRequestToHire(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateRequestToHire", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListRequestToHires([FromBody] QueryArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetListRequestToHires(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListRequestToHires", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetListDetailRequestToHire([FromBody] RequestToHireDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.GetListDetailRequestToHire(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListDeatilRequestToHire", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> SearchListRequestToHire([FromBody] RequestToHireDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.SearchListRequestToHire(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SearchListRequestToHire", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteRequestToHire([FromBody] RequestToHireDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.DeleteRequestToHire(model);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteRequestToHire", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateRequestToHire([FromBody] RequestToHireDataForCreatingArgs model)
        {
            try
            {
                var res = await _recruitmentBO.UpdateRequestToHire(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateRequestToHire", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> PrintForm(Guid id)
        {
            try
            {

                var arrayContent = await _recruitmentBO.PrintRequestToHire(id);
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
                _logger.LogError($"Error at Print File: Request To Hire", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult>  GetResignationApplicantionCompletedBySapCode(string sapcode)
        {
            try
            {
                var res = await _recruitmentBO.GetResignationApplicantionCompletedBySapCode(sapcode);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetResignationApplicantionCompletedBySapCode", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DownloadTemplateImportRequestToHire(DataToImportRequestToHireTemplateArgs args)
        {
            try
            {
                var arrayContent = await _recruitmentBO.DownloadTemplateImportRequestToHire(args);
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
        public async Task<IHttpActionResult> PostFileWithData()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var arg = new ImportRequestToHireArg();
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
                var uploadData = JsonConvert.DeserializeObject<ImportRequestToHireArg>(model);
                arg.Module = uploadData.Module;
                arg.FileName = uploadData.FileName;
                arg.AttachmentFileId = uploadData.AttachmentFileId;
                arg.IsImportManual = true;
            }
            //TODO: Do something with the JSON data.  
            var file = result.FileData[0];
            var byteArray = File.ReadAllBytes(file.LocalFileName);
            MemoryStream content = new MemoryStream();
            content.Write(byteArray, 0, byteArray.Length);
            var res = await _recruitmentBO.UploadData(arg, content);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetImportTracking([FromBody] TrackingImportArgs arg)
        {
            try
            {
                var res = await _recruitmentBO.GetImportTracking(arg);
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListRequestToHires", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> AutoImportRequestToHire([FromBody] DataToImportRequestToHireTemplateArgs args)
        {
            try
            {
                var res = await _recruitmentBO.AutoImportRequestToHire(args);
                return Ok(res);
            } catch (Exception e)
            {
                _logger.LogError("Error at method: AutoImportRequestToHire", e.Message);
                return BadRequest("Error System");
            }
        }
        #endregion
    }
}
