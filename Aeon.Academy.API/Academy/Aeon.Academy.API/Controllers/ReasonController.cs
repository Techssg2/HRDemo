using Aeon.Academy.API.Core;
using Aeon.Academy.API.DTOs;
using Aeon.Academy.API.Filters;
using Aeon.Academy.API.Mappers;
using Aeon.Academy.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aeon.Academy.API.Controllers
{
    public class ReasonController : BaseAuthApiController
    {
        private readonly IReasonOfTrainingRequestService reasonService;

        public ReasonController(IReasonOfTrainingRequestService reasonService)
        {
            this.reasonService = reasonService;
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            var reason = reasonService.Get(id);
            if (reason == null) return NotFound();
            var dto = reason.ToDto();

            return Ok(dto);
        }


        [HttpGet]
        public IHttpActionResult List()
        {
            var reasons = reasonService.List();
            if (reasons == null) return NotFound();
            var dto = reasons.ToDtos();

            return Ok(dto);
        }

        [AuthFilterByRole]
        [HttpPost]
        [ValidateModel]
        public IHttpActionResult Save(ReasonDto dto)
        {
            var reason = reasonService.Get(dto.Id);
            reason = dto.ToEntity(reason);
            var id = reasonService.Save(reason);
            return Ok(new { Id = id });
        }

        [AuthFilterByRole]
        [HttpDelete]
        public IHttpActionResult Delete(Guid id)
        {
            bool delete = reasonService.Delete(id);
            if (!delete)
            {
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent("Can not delete")
                };
                return ResponseMessage(response);
            }
            return Ok();
        }
    }
}