using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface IReportService
    {
        List<IndividualReport> GetIndividualReports(Guid userId);
        PagedList<TrainingTrackerReport> GetTrainingTrackerReports(TrainingTrackerReportFilter filter);
        PagedList<TrainingReport> GetTrainingSurveyReport(TrainingSurveyReportFilter filter);
        PagedList<TrainerContributionReport> GetTrainerContributionReport(TrainingSurveyReportFilter filter);
        PagedList<TrainingBudgetBalanceReport> GetTrainingBudgetBalanceReport(TrainingBudgetBalanceReportFilter filter);
    }
}
