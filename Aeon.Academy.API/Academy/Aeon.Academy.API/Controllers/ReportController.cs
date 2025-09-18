using Aeon.Academy.API.Core;
using Aeon.Academy.API.Filters;
using Aeon.Academy.API.Mappers;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Services;
using System;

using System.Web.Http;

namespace Aeon.Academy.API.Controllers
{
    public class ReportController : BaseAuthApiController
    {
        private readonly IReportService reportService;
        private readonly ILogger _logger;

        public ReportController(IReportService reportService, ILogger logger)
        {
            this.reportService = reportService;
            this._logger = logger;
        }

        [HttpGet]
        public IHttpActionResult GetIndividualReports(Guid userId)
        {
            var individualReport = reportService.GetIndividualReports(userId);
            if (individualReport == null) return NotFound();

            return Ok(individualReport);
        }

        [HttpPost]
        public IHttpActionResult GetTrackerReports(TrainingTrackerReportFilter filter)
        {
            var trackerReports = reportService.GetTrainingTrackerReports(filter);
            if (trackerReports == null) return NotFound();

            return Ok(new
            {
                Count = trackerReports.TotalCount,
                Totalpages = trackerReports.TotalPages,
                Data = trackerReports
            });

        }
        [HttpPost]
        [ValidateModel]
        public IHttpActionResult GetSurveyReports(TrainingSurveyReportFilter filter)
        {
            var reports = reportService.GetTrainingSurveyReport(filter);
            if (reports == null) return NotFound();

            return Ok(new
            {
                Count = reports.TotalCount,
                Totalpages = reports.TotalPages,
                Data = reports.ToDto()
            });
        }
        [HttpPost]
        public IHttpActionResult GetTrainerContributionReports(TrainingSurveyReportFilter filter)
        {
            var reports = reportService.GetTrainerContributionReport(filter);
            if (reports == null) return NotFound();

            return Ok(new
            {
                Count = reports.TotalCount,
                Totalpages = reports.TotalPages,
                Data = reports
            });
        }
        [HttpPost]
        public IHttpActionResult GetTrainingBudgetBalanceReport(TrainingBudgetBalanceReportFilter filter)
        {
            var reports = reportService.GetTrainingBudgetBalanceReport(filter);
            if (reports == null) return NotFound();

            return Ok(new
            {
                Count = reports.TotalCount,
                Totalpages = reports.TotalPages,
                Data = reports
            });
        }
    }
}