//Role (Group) enums
// var HOD = 1, CHECKER = 2, MEMBER = 4, ASSISTANT = 8;
// public enum UserRole
//     {
//         None = 0,
//         SAdmin = 1, // Super Admin
//         Admin = 2, // Admin who allow support Setting
//         HR = 4, // View on HR Features
//         CB = 8, // View on C&B features
//         Member = 16, // View on Items which has permission,
//         HRAdmin = 32,
//         Accounting = 64
//     }

//Allow roles, grades and login types (AD or Membership)
var ACADEMY_ROLES = [1, 2, 4, 8, 16, 32],
  ACADEMY_GRADES = [1, 2, 3, 4, 5, 6, 7, 8, 9],
  ACADEMY_TYPES = [0, 1];

//Add to top menu under Dashboard
function academyBuildNavigation(appSetting) {
  var routeStates = appSetting.ACADAMY_ROUTES;

  appSetting.parentMenus.push("home.academy");
  //appSetting.menu.splice(-2, 0, {
  //  index: 4,
  //  url: ".academy",
  //  selected: false,
  //  name: "Academy",
  //  title: "Academy",
  //  ref: routeStates.home,
  //  isParentMenu: true,
  //  gradeUsers: ACADEMY_GRADES,
  //  roles: ACADEMY_ROLES,
  //  type: ACADEMY_TYPES,
  //  childMenus: [
  //    {
  //      name: "TRAINING_REQUEST",
  //      selected: false,
  //      url: [],
  //      index: 1,
  //      roles: ACADEMY_ROLES,
  //      gradeUsers: ACADEMY_GRADES,
  //      type: ACADEMY_TYPES,
  //      isParentMenu: false,
  //      ref: [
  //        {
  //          state: routeStates.home,
  //          isItem: true,
  //          title: "TRAINING_REQUEST",
  //        },
  //        { state: routeStates.myRequests, isItem: false },
  //        { state: routeStates.allRequests, isItem: false },
  //        { state: routeStates.categoryManagement, isItem: false },
  //        { state: routeStates.newRequest, isItem: false },
  //        { state: routeStates.editRequest, isItem: false },
  //        { state: routeStates.trainingRequestManagement, isItem: false },
  //        { state: routeStates.externalSupplierReport, isItem: false },
  //        { state: routeStates.viewInvitation, isItem: false },
  //        { state: routeStates.trainingTrackerReport, isItem: false },
  //        { state: routeStates.trainingInvitation, isItem: false },
  //        { state: routeStates.trainingBudgetBalanceReport, isItem: false },
  //        { state: routeStates.trainingSurveyReport, isItem: false },
  //        { state: routeStates.trainerContributionReport, isItem: false },
  //      ],
  //      actions: [
  //        {
  //          name: "TRAINING_REQUEST_COMMON_MY_REQUEST",
  //          title: "My Academy Requests",
  //          state: routeStates.myRequests,
  //          roles: ACADEMY_ROLES,
  //          type: 1,
  //        },
  //        {
  //          name: "TRAINING_REQUEST_COMMON_ALL_REQUEST",
  //          title: "All Academy Requests",
  //          state: routeStates.allRequests,
  //          roles: ACADEMY_ROLES,
  //          type: 2,
  //        },
  //      ],
  //    },
  //    {
  //      name: "TRAINING_INVITATION",
  //      selected: false,
  //      url: [],
  //      index: 1,
  //      roles: ACADEMY_ROLES,
  //      gradeUsers: ACADEMY_GRADES,
  //      type: ACADEMY_TYPES,
  //      isParentMenu: false,
  //      ref: [
  //        {
  //          state: routeStates.home,
  //          isItem: true,
  //          title: "TRAINING_REQUEST",
  //        },
  //        { state: routeStates.myInvitation, isItem: false },
  //        { state: routeStates.allInvitation, isItem: false },
  //        { state: routeStates.manageInvitation, isItem: false },
  //      ],
  //      actions: [
  //        {
  //          name: "TRAINING_INVITATION_COMMON_MY_REQUEST",
  //          title: "My Invitation",
  //          state: routeStates.myInvitation,
  //          roles: ACADEMY_ROLES,
  //          type: 1,
  //        },
  //        {
  //          name: "TRAINING_INVITATION_COMMON_ALL_REQUEST",
  //          title: "All Invitation",
  //          state: routeStates.allInvitation,
  //          roles: ACADEMY_ROLES,
  //          type: 2,
  //        },
  //        {
  //          name: "MANAGE_INVITATION",
  //          title: "Manage Invitation",
  //          state: routeStates.manageInvitation,
  //          roles: ACADEMY_ROLES,
  //          type: 2,
  //        },
  //      ],
  //    },
  //    {
  //      name: "ACADEMY_REPORT",
  //      selected: false,
  //      url: [],
  //      index: 1,
  //      roles: ACADEMY_ROLES,
  //      gradeUsers: ACADEMY_GRADES,
  //      type: ACADEMY_TYPES,
  //      isParentMenu: false,
  //      ref: [
  //        {
  //          state: routeStates.home,
  //          isItem: true,
  //          title: "ACADEMY_REPORT",
  //        },
  //        { state: routeStates.trainingTrackerReport, isItem: false },
  //      ],
  //      actions: [
  //        {
  //          name: "TRAINING_TRACKER_REPORT",
  //          title: "Training Tracker Report",
  //          state: routeStates.trainingTrackerReport,
  //          roles: ACADEMY_ROLES,
  //          type: 1,
  //        },
  //        {
  //          name: "TRAINING_SURVEY_REPORT",
  //          title: "Training Survey Report",
  //          state: routeStates.trainingSurveyReport,
  //          roles: ACADEMY_ROLES,
  //          type: 1,
  //        },
  //        {
  //          name: "TRAINING_BUDGET_BALANCE_REPORT",
  //          title: "Training Budget Balance Report",
  //          state: routeStates.trainingBudgetBalanceReport,
  //          roles: ACADEMY_ROLES,
  //          type: 1,
  //        },
  //        {
  //          name: "TRAINER_CONTRIBUTION_REPORT",
  //          title: "Trainer Contribution Report",
  //          state: routeStates.trainerContributionReport,
  //          roles: ACADEMY_ROLES,
  //          type: 1,
  //        },
  //      ],
  //    },
  //    {
  //      name: "TRAINING_REPORT",
  //      selected: false,
  //      url: [],
  //      index: 1,
  //      roles: ACADEMY_ROLES,
  //      gradeUsers: ACADEMY_GRADES,
  //      type: ACADEMY_TYPES,
  //      isParentMenu: false,
  //      ref: [
  //        {
  //          state: routeStates.home,
  //          isItem: true,
  //          title: "Training Request",
  //        },
  //        { state: routeStates.allTrainingReport, isItem: false },
  //        { state: routeStates.myTrainingReport, isItem: false },
  //      ],
  //      actions: [
  //        {
  //          name: "MY_TRAINING_REPORT",
  //          title: "My Report",
  //          state: routeStates.myTrainingReport,
  //          roles: ACADEMY_ROLES,
  //          type: 1,
  //        },
  //        {
  //          name: "ALL_TRAINING_REPORT",
  //          title: "All Training Report",
  //          state: routeStates.allTrainingReport,
  //          roles: ACADEMY_ROLES,
  //          type: 2,
  //        },
  //      ],
  //    },
  //  ],
  //});

  appSetting.menu.forEach((menu) => {
    if (menu.name == "COMMON_SETTING") {
      // menu.childMenus.splice(-1, 0, {
      //   name: "Academy",
      //   selected: false,
      //   url: [],
      //   index: 1,
      //   roles: ACADEMY_ROLES,
      //   gradeUsers: ACADEMY_GRADES,
      //   type: ACADEMY_TYPES,
      //   isParentMenu: false,
      //   ref: [
      //     {
      //       state: routeStates.home,
      //       isItem: true,
      //       title: "Academy",
      //     },
      //     { state: routeStates.courseManagement, isItem: false },
      //   ],
      //   actions: [
      //     {
      //       name: "Category Management",
      //       title: "Category Management",
      //       state: routeStates.categoryManagement,
      //       gradeUsers: ACADEMY_GRADES,
      //       roles: ACADEMY_ROLES,
      //       type: ACADEMY_TYPES,
      //     },
      //     {
      //       name: "Training Request Management",
      //       title: "Training Request Management",
      //       state: routeStates.trainingRequestManagement,
      //       gradeUsers: ACADEMY_GRADES,
      //       roles: ACADEMY_ROLES,
      //       type: ACADEMY_TYPES,
      //     },
      //   ],
      // });
      //menu.childMenus.forEach((item) => {
      //  if (item.name == "More_MENU") {
      //    item.actions = [
      //      ...item.actions,
      //      {
      //        name: "Reason of Training Request",
      //        title: "Reason of Training Request",
      //        state: routeStates.trainingReason,
      //        gradeUsers: ACADEMY_GRADES,
      //        roles: ACADEMY_ROLES,
      //        type: ACADEMY_TYPES,
      //      },
      //    ];
      //  }
      //});
    }
  });
}

//Add a new tile to Dashboard
function academyAddTileOnDashboard(appSetting) {
  var routeStates = appSetting.ACADAMY_ROUTES;
  appSetting.subSections.push({
    title: "Academy",
    ref: routeStates.home,
    gradeUsers: ACADEMY_GRADES,
    roles: ACADEMY_ROLES,
    type: ACADEMY_TYPES,
    sections: [
      {
        name: "NEW_TRAINING_REQUEST",
        title: "NEW ITEM: Training Request",
        ref: routeStates.newRequest,
        url: "",
        icon: "ClientApp/app/components/academy/assets/images/training-request.png",
        addToMenu: true,
        gradeUsers: ACADEMY_GRADES,
        roles: ACADEMY_ROLES,
        type: ACADEMY_TYPES,
      },
      {
        name: "CATEGORY_MANAGEMENT",
        title: "NEW ITEM: Category",
        ref: routeStates.categoryManagement,
        url: "",
        icon: "ClientApp/app/components/academy/assets/images/category.png",
        addToMenu: true,
        gradeUsers: ACADEMY_GRADES,
        roles: ACADEMY_ROLES,
        type: ACADEMY_TYPES,
      },
      {
        name: "TRAINING_REQUEST_MANAGEMENT",
        title: "Training Request Management",
        ref: routeStates.trainingRequestManagement,
        url: "",
        icon: "ClientApp/app/components/academy/assets/images/training-request-management.png",
        addToMenu: true,
        gradeUsers: ACADEMY_GRADES,
        roles: ACADEMY_ROLES,
        type: ACADEMY_TYPES,
      },
      {
        name: "MANAGE_TRAINING_INVITATION",
        title: "Training Invitation Management",
        ref: routeStates.manageInvitation,
        url: "",
        icon: "ClientApp/app/components/academy/assets/images/invitation-management.png",
        addToMenu: true,
        gradeUsers: ACADEMY_GRADES,
        roles: ACADEMY_ROLES,
        type: ACADEMY_TYPES,
      },
    ],
  });
}

//Add the Academy sub-menu into Plus (+) menu on the right hand side.
function academyBuildPlusNavigation($rootScope) {
  $rootScope.$watch("addSectionMenu", function (newValue, oldValue) {
    if (newValue && newValue.length > 0) {
      let existItem = _.find($rootScope.addSectionMenu, (x) => {
        return x.label == "COMMON_ACADEMY";
      });
      if (!existItem) {
        $rootScope.addSectionMenu.push({
          label: "COMMON_ACADEMY",
          title: "Academy",
          values: [],
        });
      }
      let current = _.find($rootScope.addSectionMenu, (x) => {
        return x.label == "COMMON_ACADEMY";
      });
      current.values = current.values.concat([
        {
          name: "NEW_TRAINING_REQUEST",
          title: "NEW ITEM: Training Request Form",
          ref: "home.academy-newRequest",
        },
      ]);
    }
  });
}
