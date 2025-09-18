using Aeon.Academy.Common.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aeon.Intergation.FakeApi.Controllers
{
    public class SapIntegrationController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SyncData(AcademyTrainingRequest request)
        {
            IsValidDate("Begda", request.Begda);
            IsValidDate("Endda", request.Endda);
            IsValidDate("Zstart", request.Zstart);
            IsValidDate("Zend", request.Zend);
            if (!ModelState.IsValid)
            {
                var errors = string.Empty;
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        errors = errors + error.ErrorMessage + " ";
                    }
                }
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(errors),
                };
                return response;
            }
            var dto = new AcademyTrainingResponse
            {
                Data = new Data
                {
                    Begda = request.Begda,
                    Endda = request.Endda,
                    Message = "",
                    Metadata = new Metadata(),
                    Pernr = request.Pernr,
                    Status = "S",
                    Zagency = request.Zagency,
                    Zend = request.Zend,
                    ZhoursDay = request.Zhours_day,
                    ZinEx = request.Zin_ex,
                    Znumofday = request.Znumofday,
                    ZprgCode = request.Zprg_code,
                    Zprogram = request.Zprogram,
                    Zstart = request.Zstart,
                    ZtotalHours = request.Ztotal_hours,
                    ZtrainCont = request.Ztrain_cont,
                    ZtrainCost = request.Ztrain_cost,
                    Ztrainer = request.Ztrainer,
                    ZtrainLoc = request.Ztrain_loc
                },
            };
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }
        private void IsValidDate(string key, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            DateTime date;
            bool isValid = DateTime.TryParseExact(value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date);
            if (!isValid)
            {
                ModelState.AddModelError(key, $"The {key} field invalid date format yyyymmdd.");
            }
            return;
        }
    }
}
