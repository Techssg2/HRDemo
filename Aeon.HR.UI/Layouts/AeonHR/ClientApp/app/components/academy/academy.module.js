"use strict";
var app = angular.module("ssg");

app.requires.push("ssg.module.base.ctrl");
app.requires.push("ssg.module.common.dashboard.ctrl");

app.requires.push("ssg.module.academy.training-request.ctrl");
app.requires.push("ssg.module.academy.category-management.ctrl");
app.requires.push("ssg.module.academy.course-management.ctrl");
app.requires.push("ssg.module.academy.training-request-management.ctrl");
app.requires.push("ssg.module.academy.request-list.ctrl");

app.requires.push("ssg.module.academy.training-invitation.ctrl");
app.requires.push("ssg.module.academy.view-invitation.ctrl");
app.requires.push("ssg.module.academy.training-invitation.my-items.ctrl");
app.requires.push("ssg.module.academy.training-invitation.all-items.ctrl");
app.requires.push("ssg.module.academy.training-invitation.manage-item.ctrl");
app.requires.push("ssg.module.academy.training-reson.ctrl");
app.requires.push("ssg.module.academy.training-tracker-report.ctrl");

app.requires.push("ssg.module.academy.training-request.svc");
app.requires.push("ssg.module.academy.training-invitation.svc");
app.requires.push("ssg.module.academy.category.svc");
app.requires.push("ssg.module.academy.course.svc");
app.requires.push("ssg.module.academy.account.svc");
app.requires.push("ssg.module.academy.training-reason.svc");
app.requires.push("ssg.module.academy.training-report.svc");
app.requires.push("ssg.module.academy.f2-integration.svc");

app.requires.push("ssg.module.academy.currency.directive");
app.requires.push("ssg.directive.academy.courseDurationDetail");
app.requires.push("ssg.directive.academy.academyProcessingStage");
app.requires.push("ssg.directive.academy.academyWorkflowStepModule");
app.requires.push("ssg.directive.academy.academyWorkflowTaskModule");
app.requires.push("ssg.directive.academy.academyWorkflowCommentModule");
app.requires.push("ssg.academy.external-training-report.module");
app.requires.push("ssg.academy.training-survey-report.module");
app.requires.push("ssg.academy.training-budget-balance-report.module");
app.requires.push("ssg.academy.trainer-contribution-report.module");
app.requires.push("ssg.module.academy.training-report.all-items.ctrl");
app.requires.push("ssg.module.academy.training-report.my-item.ctrl");

app.config(function (
  $stateProvider,
  $translateProvider,
  $httpProvider,
  appSetting
) {
  academyRegisterRoutes($stateProvider, appSetting);
  academyBuildNavigation(appSetting);
  academyAddTileOnDashboard(appSetting);
  filterMenuBasedOnRole(appSetting);
  $httpProvider.interceptors.push("academy.interceptor");
});

app.run([
  "$rootScope",
  function ($rootScope, appSetting) {
    academyBuildPlusNavigation($rootScope);
  },
]);

app.run(function ($uiRouter) {
  academyOverwriteRoutes($uiRouter);
});

app.run(function ($uiRouter) {
  todoOverwriteRoutes($uiRouter);
});
