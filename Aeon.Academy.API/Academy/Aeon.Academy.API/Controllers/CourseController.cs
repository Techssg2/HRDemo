using Aeon.Academy.API.Core;
using Aeon.Academy.API.DTOs;
using Aeon.Academy.API.Filters;
using Aeon.Academy.API.Mappers;
using Aeon.Academy.API.Utils;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aeon.Academy.API.Controllers
{
    public class CourseController : BaseAuthApiController
    {
        private readonly ICourseService courseService;
        private readonly SharepointFile sharepointFile;

        public CourseController(ICourseService courseService)
        {
            this.courseService = courseService;

            this.sharepointFile = new SharepointFile(DocumentLibraryName.Course);
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            var course = courseService.Get(id);
            if (course == null) return NotFound();

            var dto = course.ToDto();
            var attachments = sharepointFile.GetCourseDocument(course.Name);
            dto.Documents = attachments;

            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetAll()
        {
            var courses = courseService.GetAll();
            if (courses == null) return NotFound();

            var dto = courses.ToDtos();

            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetAllBySadmin()
        {
            var courses = courseService.GetAll(true);
            if (courses == null) return NotFound();

            var dto = courses.ToDtos();

            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult List(Guid categoryId)
        {
            var courses = courseService.ListByCategoryId(categoryId);
            if (courses == null) return NotFound();

            var dto = courses.ToDtos();

            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult ListBySadmin(Guid categoryId)
        {
            var courses = courseService.ListByCategoryId(categoryId, true);
            if (courses == null) return NotFound();

            var dto = courses.ToDtos();

            return Ok(dto);
        }

        [HttpPost]
        [ValidateModel]
        [AuthFilterByRole]
        public IHttpActionResult Save(CourseDto dto)
        {
            var course = courseService.Get(dto.Id);
            course = dto.ToEntity(course, CurrentUser);
            bool isValid = courseService.Validate(course);
            if (!isValid)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Name already exists")
                };
                return ResponseMessage(response);
            }
            var result = sharepointFile.UploadFiles(dto.Name, dto.Documents);
            if (!string.IsNullOrEmpty(result))
            {
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent(result)
                };
                return ResponseMessage(response);
            }
            var id = courseService.Save(course);
            return Ok(new { Id = id });
        }

        [HttpDelete]
        [AuthFilterByRole]
        public IHttpActionResult Delete(Guid id)
        {
            var course = courseService.Get(id);
            if (course == null) return NotFound();

            var delete = courseService.Delete(course);
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

        [AuthFilterByRole]
        [HttpPost]
        public IHttpActionResult Restore(Guid id)
        {
            var course = courseService.Get(id);
            if (course == null) return NotFound();
            course.IsActivated = true;
            var restoreId = courseService.Save(course);
            if (restoreId == Guid.Empty)
            {
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent("Can not restore")
                };
                return ResponseMessage(response);
            }

            return Ok(new { Id = restoreId });
        }
    }
}