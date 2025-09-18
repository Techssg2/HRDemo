var ssgApp = angular.module("ssg.directive.academy.academyProcessingStage", []);
ssgApp.directive("academyProcessingStage", [
  function ($rootScope) {
    return {
      restrict: "E",
      templateUrl:
        "ClientApp/app/components/academy/directives/processing-stages/academy-processing-stages.template.html?v=" +
        edocV,
      scope: {
        currentUser: "=",
        workflowInstances: "=",
      },
      link: function ($scope, element, attr, modelCtrl) {},
      controller: [
        "$rootScope",
        "$scope",
        "dataService",
        function ($rootScope, $scope, dataService) {
          $scope.roundIndex = 0;
          $scope.historysArray = [];
          $scope.updateIndex = function (index) {
            return $scope.roundIndex;
          };
          $scope.reverseArray = function (arr) {
            return _.orderBy(arr, "created", "asc");
          };
          $scope.$watch("workflowInstances", function () {
            if ($scope.workflowInstances && $scope.workflowInstances.histories)
              $scope.historysArray = groupArray(
                $scope.workflowInstances.histories
              );
          });
          function groupArray(arr) {
            if (arr) {
              if (arr.length) {
                let historieObj = _.groupBy(arr, "roundNumber");
                if (historieObj) {
                  return Object.values(historieObj);
                } else return [];
              }
            }
          }
        },
      ],
    };
  },
]);
