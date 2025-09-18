using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class CreateTargetPlanPeriodJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;

        public CreateTargetPlanPeriodJob(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }
        public void DoCreateTargetPlanPeriod()
        {
            _logger.LogInformation("Start DoCreateTargetPlanPeriod: ");
            var now = DateTimeOffset.Now;
            var nextMonthDate = now.AddMonths(1);
            var dayConfiguration = _uow.GetRepository<DaysConfiguration>().GetSingle(x => x.Name == "CreatedNewPeriodDate");
            if (now.Day == dayConfiguration.Value)
            {
                var nexMonth = nextMonthDate.Month;
                var obj = new TargetPlanPeriod
                {
                    Id = Guid.NewGuid(),
                    Created = DateTimeOffset.Now,
                    Modified = DateTimeOffset.Now,
                    CreatedBy = "AEON System",
                    ModifiedBy = "AEON System",
                    CreatedByFullName = "AEON System",
                    ModifiedByFullName = "AEON System",
                    FromDate = new DateTimeOffset(new DateTime(now.Year, now.Month, 26)),
                    ToDate = new DateTimeOffset(new DateTime(nextMonthDate.Year, nexMonth, 25)),
                    Name = String.Format("{0}/{1}", nexMonth, nextMonthDate.Year)
                };
                var exists = _uow.GetRepository<TargetPlanPeriod>().GetSingle(x => x.Name == obj.Name);
                if (exists == null)
                {
                    _uow.GetRepository<TargetPlanPeriod>().Add(obj);
                    _uow.Commit();
                }
            }
        }
    }
}