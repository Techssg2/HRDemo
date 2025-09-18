var ssgApp = angular.module('ssg.directiveWorkflowStatusModule', []);
ssgApp.directive("workflowStatus", [
    function($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/workflow-status/workflow-status.template.html?v=" + edocV,
            scope: {
                status: "@status",
            },
            link: function($scope, element, attr, modelCtrl) {
                $scope.status = attr.status
            }
        };
    },
]);