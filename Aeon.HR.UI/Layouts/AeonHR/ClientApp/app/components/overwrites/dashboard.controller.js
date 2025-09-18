angular
  .module("ssg.module.common.dashboard.ctrl", ["kendo.directives"])
  .controller(
    "DashboardController",
    function (
      $rootScope,
      $scope,
      $stateParams,
      $state,
      $window,
      $translate,
      $controller,
      Notification,
      TrainingRequestService,
      appSetting
    ) {
      $controller("dashboardController", { $scope: $scope });

      academyOverwriteColumnTemplate(
        $scope.myItemsGridOptions,
        function (data) {
          if (data.itemType == "TrainingReport") {
            return `<a ui-sref="home.academy-supplierReport({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
          }
          return `<a ui-sref="home.academy-editRequest({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
        }
      );

      academyOverwriteColumnTemplate(
        $scope.toDoListGridOptions,
        function (data) {
          if (data.itemType == "TrainingReport") {
            return `<a ui-sref="home.academy-supplierReport({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
          }
          if (data.itemType == "TrainingRequest") {
            return `<a ui-sref="home.academy-editRequest({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
          }
          if (data.itemType == "TrainingInvitation") {
            return `<a ui-sref="home.academy-viewInvitation({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
          }
        }
      );

      filterMenuBasedOnRole(appSetting);
    }
  );
function academyOverwriteColumnTemplate(gridOptions, newTemplateFunc) {
  var baseRefNumColMyItem = gridOptions.columns.find(function (col) {
    return col.field == "referenceNumber";
  });
  var baseRefNumColTemp = baseRefNumColMyItem.template;
  baseRefNumColMyItem.template = function (data) {
    var template = baseRefNumColTemp(data);
    if (!template || template.indexOf('<a href=null target="_blank">') == 0) {
      return newTemplateFunc(data);
    }
    return template;
  };
}
function academyOverwriteRoutes($uiRouter) {
  var state = $uiRouter.stateRegistry.deregister("home.dashboard");
  state = {
    name: "home.dashboard",
    url: "/dashboard",
    templateUrl:
      "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
    controller: "DashboardController",
  };
  $uiRouter.stateRegistry.register(state);
}

function todoOverwriteRoutes($uiRouter) {
  var state = $uiRouter.stateRegistry.deregister("home.todo");
  state = {
    name: "home.todo",
    url: "/todo",
    templateUrl:
      "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
    controller: "DashboardController",
  };
  $uiRouter.stateRegistry.register(state);
}

function filterMenuBasedOnRole(appSetting) {
  var curUser = JSON.parse(sessionStorage.getItem("currentUser"));
  if (
    curUser && curUser.deptCode != deptAcademyCode &&
    curUser.sapCode !== "Administrator"
  ) {
    const { subSections, menu } = appSetting;

    subSections.forEach((element) => {
      if (element.title === "Academy") {
        element.sections = element.sections.filter(
          (section) =>
            section.name !== "CATEGORY_MANAGEMENT" &&
            section.name !== "TRAINING_REQUEST_MANAGEMENT" &&
            section.name !== "MANAGE_TRAINING_INVITATION"
        );
      }
    });
    menu.forEach((element) => {
      if (element.title === "Academy") {
        const { childMenus } = element;
        childMenus.forEach((element) => {
          if (element.name === "TRAINING_INVITATION") {
            element.actions = element.actions.filter(
              (act) => act.name !== "MANAGE_INVITATION"
            );
          }
        });

        element.childMenus = element.childMenus.filter(
          (childMn) => childMn.name !== "ACADEMY_REPORT"
        );
      }
    });
  }
}
