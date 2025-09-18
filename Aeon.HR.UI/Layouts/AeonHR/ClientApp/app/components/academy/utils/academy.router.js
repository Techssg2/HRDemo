function academyRegisterRoutes($stateProvider, appSetting) {
  var version = edocV;
  var ACADAMY_ROUTES = {
    home: "home.academy",
    //Training request
    myRequests: "home.academy-myRequests",
    newRequest: "home.academy-newRequest",
    allRequests: "home.academy-allRequests",
    trainingRequestManagement: "home.academy-trainingRequestManagement",
    editRequest: "home.academy-editRequest",

    //Training Invitation
    trainingInvitation: "home.academy-trainingInvitation",
    myInvitation: "home.academy-myInvitation",
    newInvitation: "home.academy-newInvitation",
    allInvitation: "home.academy-allInvitation",
    manageInvitation: "home.academy-manageInvitation",

    //Category
    categoryManagement: "home.academy-categoryManagement",

    //Course
    courseManagement: "home.academy-courseManagement",

    //Provider
    providerManagement: "home.academy-providerManagement",
    viewInvitation: "home.academy-viewInvitation",

    //Report
    trainingTrackerReport: "home.academy-trainingTrackerReport",
    trainingBudgetBalanceReport: "home.training-budget-balance-report",
    //master data
    trainingReason: "home.academy-trainingReason",
    externalSupplierReport: "home.academy-supplierReport",
    editExternalSupplierReport: "home.academy-editSupplierReport",

    trainingSurveyReport: "home.academy-trainingSurveyReport",
    trainerContributionReport: "home.academy-trainerContributionReport",
    allTrainingReport: "home.academy-allReport",
    myTrainingReport: "home.academy-myReport",
  };
  appSetting.ACADAMY_ROUTES = ACADAMY_ROUTES;

  //Home
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.home,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });

  //Request
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.myRequests,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.newRequest,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.editRequest,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.trainingRequestManagement,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.myRequests,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.allRequests,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.trainingSurveyReport,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });

  //Category
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.categoryManagement,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });

  //Course
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.courseManagement,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });

  //Provider
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.providerManagement,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  //Training Invitation
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.trainingInvitation,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.viewInvitation,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.manageInvitation,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.myInvitation,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.allInvitation,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.trainingReason,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.externalSupplierReport,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });

  //Report
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.trainingTrackerReport,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.trainingBudgetBalanceReport,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.trainerContributionReport,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.allTrainingReport,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  appSetting.activeStates.push({
    state: ACADAMY_ROUTES.myTrainingReport,
    roles: ACADEMY_ROLES,
    gradeUsers: ACADEMY_GRADES,
    type: ACADEMY_TYPES,
  });
  $stateProvider
    //Dashboard
    .state(ACADAMY_ROUTES.home, {
      url: "/academy",
      templateUrl:
        "ClientApp/app/components/dashboard/dashboard-view.html?v=" + version,
      controller: "DashboardController",
    })
    //Training request
    .state(ACADAMY_ROUTES.newRequest, {
      url: "/academy/new-request",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-request/request-item.view.html?v=" +
        version,
      controller: "TrainingRequestController",
    })
    .state(ACADAMY_ROUTES.editRequest, {
      url: "/academy/edit-request/:id",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-request/request-item.view.html?v=" +
        version,
      controller: "TrainingRequestController",
    })
    .state(ACADAMY_ROUTES.trainingRequestManagement, {
      url: "/academy/request-management",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-request/request-management.view.html?v=" +
        version,
      controller: "TrainingRequestManagementController",
    })
    .state(ACADAMY_ROUTES.myRequests, {
      url: "/myRequests",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-request/request-list.view.html?v=" +
        version,
      params: { action: { title: "My Academy Requests" } },
      controller: "RequestListController",
    })
    .state(ACADAMY_ROUTES.allRequests, {
      url: "/allRequests",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-request/request-list.view.html?v=" +
        version,
      params: { action: { title: "All Academy Requests" } },
      controller: "RequestListController",
    })
    //Category
    .state(ACADAMY_ROUTES.categoryManagement, {
      url: "/academy/category-management",
      templateUrl:
        "ClientApp/app/components/academy/pages/category/category-management.view.html?v=" +
        version,
      controller: "CategoryManagementController",
    })
    //Course
    .state(ACADAMY_ROUTES.courseManagement, {
      url: "/academy/course-management",
      templateUrl:
        "ClientApp/app/components/academy/pages/category/category-management.view.html?v=" +
        version,
      controller: "CourseManagementController",
    })
    //Training Invitation
    .state(ACADAMY_ROUTES.trainingInvitation, {
      url: "/academy/training-invitation/:id",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-invitation/new-training-invitation.view.html?v=" +
        version,
      controller: "TrainingInvitationController",
      params: { action: {}, invitationId: "" },
    })
    .state(ACADAMY_ROUTES.viewInvitation, {
      url: "/academy/view-invitation/:id",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-invitation/view-invitation.view.html?v=" +
        version,
      controller: "ViewInvitationController",
    })
    .state(ACADAMY_ROUTES.myInvitation, {
      url: "/academy/training-invitation/myRequests",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-invitation/myitem/training-invitation-myItem.view.html?v=" +
        version,
      controller: "TrainingInvitationMyItemsController",
    })
    .state(ACADAMY_ROUTES.allInvitation, {
      url: "/academy/training-invitation/allRequests",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-invitation/allitem/training-invitation-list.view.html?v=" +
        version,
      controller: "TrainingInvitationAllItemsController",
    })
    .state(ACADAMY_ROUTES.manageInvitation, {
      url: "/academy/training-invitation/manageRequest",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-invitation/manageItem/training-invitation-managedItem.view.html?v=" +
        version,
      controller: "TrainingInvitationManageItemController",
    })
    .state(ACADAMY_ROUTES.trainingReason, {
      url: "/academy/training-reason",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-reason/training-reason.view.html?v=" +
        version,
      controller: "TrainingReasonController",
    })
    .state(ACADAMY_ROUTES.externalSupplierReport, {
      url: "/academy/extenal-training-report/:id",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-report/external-training-report/external-training-report.view.html?v=" +
        version,
      params: { id: "", invitationId: "" },
      controller: "ExternalTrainingReport",
    })
    .state(ACADAMY_ROUTES.trainingTrackerReport, {
      url: "/academy/trainingTrackerReport",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-report/training-tracker-report/training-tracker-report.view.html?v=" +
        version,
      controller: "TrainingTrackerReportController",
    })
    .state(ACADAMY_ROUTES.trainingSurveyReport, {
      url: "/academy/training-survey-report",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-report/training-survey-report/training-survey-report.view.html?v=" +
        version,
      controller: "TrainingSurveyReport",
    })
    .state(ACADAMY_ROUTES.trainingBudgetBalanceReport, {
      url: "/academy/training-budget-balance-report",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-report/training-budget-balance-report/training-budget-balance-report.view.html?v=" +
        version,
      controller: "TrainingBudgetBalanceReport",
    })
    .state(ACADAMY_ROUTES.trainerContributionReport, {
      url: "/academy/trainer-contribution-report",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-report/trainer-contribution-report/trainer-contribution-report.view.html?v=" +
        version,
      controller: "TrainerContributionReport",
    })
    .state(ACADAMY_ROUTES.allTrainingReport, {
      url: "/academy/training-report/allReport",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-report/allItem/training-report-list.view.html?v=" +
        version,
      controller: "TrainingReportAllItemsController",
    })
    .state(ACADAMY_ROUTES.myTrainingReport, {
      url: "/academy/training-report/myreport",
      templateUrl:
        "ClientApp/app/components/academy/pages/training-report/myItem/training-report-myItem.view.html?v=" +
        version,
      controller: "TrainingReportMyItemController",
    });
  // .state(ACADAMY_ROUTES.editRxternalSupplierReport, {
  //   url: "/academy/edit-extenal-training-report/:id",
  //   templateUrl:
  //     "ClientApp/app/components/academy/pages/training-report/external-training-report/external-training-report.view.html?v=" +
  //     version,
  //   controller: "ExternalTrainingReport",
  // });
}
