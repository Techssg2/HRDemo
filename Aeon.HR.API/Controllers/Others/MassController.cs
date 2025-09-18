using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.Others
{
    public class MassController : BaseController
    {
        protected readonly IMassBO _bo;
        public MassController(ILogger logger, IMassBO bo) : base(logger)
        {
            _bo = bo;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetMassLocations()
        {
            try
            {
                return Ok(await _bo.GetMassLocations());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: GetMassLocations", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateRecruitmentCategory(RecruitmentCategoryArgs args)
        {
            try
            {
                return Ok(await _bo.UpdateCategory(args));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: UpdateRecruitmentCategory", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CreateRecruitmentCategory(RecruitmentCategoryArgs args)
        {
            try
            {
                return Ok(await _bo.UpdateCategory(args));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: CreateRecruitmentCategory", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteRecruitmentCategory(RecruitmentCategoryArgs args)
        {
            try
            {
                return Ok(await _bo.DeleteCategory(args.Id.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: DeleteRecruitmentCategory", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetMassPositions(QueryArgs queryArgs)
        {
            try
            {
                return Ok(await _bo.GetMassPositions(queryArgs));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: GetMassPositions", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }
    }
}