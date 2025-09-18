var ssgApp = angular.module('ssg.directiveActionHistoryModule', []);
ssgApp.directive("actionHistory", [
    function ($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/action-history/action-history.template.html?v=" + edocV,
            scope: {
                currentUser: "=",
                workflowInstances: "=",
                model: "="
            },
            link: function ($rootScope, $scope, dataService, appSetting, $translate, attachmentFile) { },
            controller: [
                "$rootScope", "$scope", "dataService", "appSetting", "$translate", "attachmentFile",
                function ($rootScope, $scope, dataService) {
                    $scope.roundIndex = 0;
                    $scope.reverseArray = function (arr) {
                        return _.orderBy(arr, 'created', 'asc');
                    }
                    $scope.updateIndex = function (index) {
                        return $scope.roundIndex;
                    }
                },
            ],
        };
    },
]);