var ssgApp = angular.module("ssg.directiveWorkflowCommentModule", []);
ssgApp.directive("workflowComment", [
  function () {
    return {
      restrict: "E",
      templateUrl:
        "ClientApp/app/components/academy/directives/workflow-comment/workflow-comment.template.html?v=" +
        edocV,
      scope: {
        workflowInstances: "=",
      },
      controller: [
        "$rootScope",
        "$scope",
        "$cacheFactory",
        "dataService",
        async function ($rootScope, $scope, $cacheFactory, dataService) {
          $scope.$watch("workflowInstances", function () {
            if (
              $scope.workflowInstances &&
              $scope.workflowInstances.histories &&
              $scope.workflowInstances.histories.length > 0
            ) {
              $scope.lastStep = $scope.workflowInstances?.histories[0];
            }
          });
        },
      ],
    };
  },
]);
