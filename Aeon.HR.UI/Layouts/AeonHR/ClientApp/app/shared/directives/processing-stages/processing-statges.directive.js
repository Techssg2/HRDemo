var ssgApp = angular.module('ssg.directiveStageModule', []);
ssgApp.directive("processingStage", [
    function($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/processing-stages/processing-stages.template.html?v=" + edocV,
            scope: {
                currentUser: "=",
                workflowInstances: "=",
                model:"="
            },
            link: function($scope, element, attr, modelCtrl) {},
            controller: [
                "$rootScope", "$scope", "dataService",
                function ($rootScope, $scope, dataService) {
                    $scope.roundIndex = 0;
                    $scope.workflowStatus = dataService.workflowStatus;                  
                    $scope.reverseArray = function (arr){
                        return _.orderBy(arr, 'created', 'asc');                   
                    }
                    $scope.updateIndex = function(index){                        
                        return $scope.roundIndex;
                    }
                },
            ],
        };
    },
]);