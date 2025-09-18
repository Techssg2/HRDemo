angular
  .module("ssg.module.academy.training-report.svc", [])
  .factory("TrainingReportService", function ($resource, interceptorService) {
    return $resource(null, null, {
      get: {
        method: "GET",
        interceptor: interceptorService.getInstance().interceptor,
        url: baseUrlApi + "/TrainingReport/Get/:id",
      },
      save: {
        method: "POST",
        interceptor: interceptorService.getInstance().interceptor,
        url: baseUrlApi + "/TrainingReport/Save",
      },
      submit: {
        method: "POST",
        interceptor: interceptorService.getInstance().interceptor,
        url: baseUrlApi + "/TrainingReport/Submit",
      },
      approve: {
        method: "POST",
        interceptor: interceptorService.getInstance().interceptor,
        url: baseUrlApi + "/TrainingReport/Approve",
      },
      reject: {
        method: "POST",
        interceptor: interceptorService.getInstance().interceptor,

        url: baseUrlApi + "/TrainingReport/Reject",
      },
      requestToChange: {
        method: "POST",
        url: baseUrlApi + "/TrainingReport/RequestToChange",
        interceptor: interceptorService.getInstance().interceptor,
      },
      progressingStage: {
        method: "GET",
        url: baseUrlApi + "/TrainingReport/progressingStage/:id",
      },
      myItems: {
        method: "GET",
        url: baseUrlApi + "/TrainingReport/MyItems",
        interceptor: interceptorService.getInstance().interceptor,
      },
      getTrackerReports: {
        method: "POST",
        url: baseUrlApi + "/Report/GetTrackerReports",
        interceptor: interceptorService.getInstance().interceptor,
      },
      getSurveyReports: {
        method: "POST",
        url: baseUrlApi + "/Report/GetSurveyReports",
        interceptor: interceptorService.getInstance().interceptor,
      },
      getTrainerContributionReports: {
        method: "POST",
        url: baseUrlApi + "/Report/GetTrainerContributionReports",
        interceptor: interceptorService.getInstance().interceptor,
      },
      getTrainingBudgetBalanceReport: {
        method: "POST",
        url: baseUrlApi + "/Report/GetTrainingBudgetBalanceReport",
        interceptor: interceptorService.getInstance().interceptor,
      },
      getReportInvitationId: {
        method: "GET",
        interceptor: interceptorService.getInstance().interceptor,
        url: baseUrlApi + "/TrainingReport/GetByInvitation/:id",
      },
      listMyReport: {
        method: "POST",
        url: baseUrlApi + "/TrainingReport/ListMyReport",
      },
      listAllReports: {
        method: "POST",
        url: baseUrlApi + "/TrainingReport/ListAllReport",
      },
    });
  });
