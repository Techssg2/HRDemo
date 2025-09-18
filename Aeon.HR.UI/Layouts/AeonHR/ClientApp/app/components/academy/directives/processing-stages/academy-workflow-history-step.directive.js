var ssgApp = angular.module("ssg.directive.academy.academyWorkflowStepModule", []);
ssgApp.directive("academyWorkflowStep", [
  function ($rootScope) {
    return {
      restrict: "E",
      templateUrl:
        "ClientApp/app/components/academy/directives/processing-stages/academy-workflow-history-step.template.html?v=" +
        edocV,
      scope: {
        step: "=",
        flow: "=",
      },
      link: function ($scope, element, attr, modelCtrl) {},
      controller: [
        "$scope",
        function ($scope) {
          $scope.showStep = {}
          // var instances = $scope.instances;
          if ($scope.flow?.length) {
            let rounds = [];
            let round = _.find($scope.flow, {
              stepNumber: $scope.step.stepNumber,
            });
            if (round) {
              rounds.push(round);
            }
            $scope.showStep.rounds = rounds;
            $scope.showStep.stepName = $scope.step.stepName.trim();
          }
        },
      ],
    };
  },
]);
