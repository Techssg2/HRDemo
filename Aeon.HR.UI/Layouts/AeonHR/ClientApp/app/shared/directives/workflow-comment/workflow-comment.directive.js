var ssgApp = angular.module('ssg.directiveWorkflowCommentModule', []);
ssgApp.directive("workflowComment", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/workflow-comment/workflow-comment.template.html?v=" + edocV,
            scope: {},
            controller: ["$rootScope", "$scope", "$cacheFactory","dataService",
                async function ($rootScope, $scope, $cacheFactory, dataService) {
                    $scope.status = dataService.workflowStatus;
                }
            ],
        };
    },
]);