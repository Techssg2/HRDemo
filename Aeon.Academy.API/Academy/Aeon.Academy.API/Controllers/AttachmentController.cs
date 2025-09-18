using Aeon.Academy.API.Filters;
using Aeon.Academy.API.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Aeon.Academy.API.Controllers
{
    [CustomExceptionFilter]
    public class AttachmentController : ApiController
    {
        private readonly SharepointFile sp = new SharepointFile("");
        [HttpGet]
        public IHttpActionResult DownloadDocument(string filePath)
        {
            var result = sp.Download(filePath);
            if (result == null)
            {
                return NotFound();
            }
            var fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(result);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return ResponseMessage(response);
        }
    }
}
