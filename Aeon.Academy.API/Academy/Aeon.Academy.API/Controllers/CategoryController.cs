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

    public class CategoryController : BaseAuthApiController
    {
        private readonly ICategoryService categoryService;


        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            var category = categoryService.Get(id);
            if (category == null) return NotFound();

            var dto = category.ToDto();

            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult List()
        {
            var categories = categoryService.ListAll();
            if (categories == null) return NotFound();

            var dto = categories.ToDtos();

            return Ok(dto);
        }

        [AuthFilterByRole]
        [HttpPost]
        [ValidateModel]
        public IHttpActionResult Save(CategoryDto dto)
        {
            var category = categoryService.Get(dto.Id);
            category = dto.ToEntity(category, CurrentUser);
            bool isValid = categoryService.Validate(category);
            if (!isValid)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Name already exists")
                };
                return ResponseMessage(response);
            }
            var id = categoryService.Save(category);
            return Ok(new { Id = id });
        }

        [AuthFilterByRole]
        [HttpDelete]
        public IHttpActionResult Delete(Guid id)
        {
            var category = categoryService.Get(id);
            if (category == null) return NotFound();

            bool delete = categoryService.Delete(category);
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
        [HttpGet]
        public IHttpActionResult ListAll()
        {
            var categories = categoryService.ListAll(true);
            if (categories == null) return NotFound();

            var dto = categories.ToDtos();

            return Ok(dto);
        }

        [AuthFilterByRole]
        [HttpPost]
        public IHttpActionResult Restore(Guid id)
        {
            var category = categoryService.Get(id);
            if (category == null) return NotFound();
            if(category.ParentId != null)
            {
                var parent = categoryService.Get(category.ParentId.Value);
                if (!parent.IsActivated)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                    {
                        Content = new StringContent("Parent is deactived. Can not restore")
                    };
                    return ResponseMessage(response);
                }
            }
            category.IsActivated = true;
            var restoreId = categoryService.Save(category);

            return Ok(new { Id = restoreId });
        }
    }
}