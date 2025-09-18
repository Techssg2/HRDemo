var ssgApp = angular.module('ssg.directiveWorkflowTaskModule', []);
ssgApp.directive("workflowTask", [

    function ($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/processing-stages/workflow-history-task.template.html?v=" + edocV,
            scope: {
                round: "="

            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope",
                function ($rootScope, $scope) {
                    $scope.formatDate = "dd/MM/yyyy HH:mm:ss";

                    //0:None
                    //1: Approve
                    //2: Reject
                    //3: Request To Change
                    if ($scope.round && $scope.round.id) {
                        $scope.roundStyle = "task-status-in-progress task-outcome-none";
                        if ($scope.round.voteType == 1 || $scope.round.voteType == 0 && $scope.round.isStepCompleted) {
                            $scope.roundStyle = "task-status-completed task-outcome-approved";
                        } else if ($scope.round.voteType == 2) {
                            $scope.roundStyle = "task-status-completed task-outcome-rejected";
                        } else if ($scope.round.voteType == 3) {
                            $scope.roundStyle = "task-status-completed task-outcome-requested-to-change";
                        } else if ($scope.round.voteType == 4) {
                            $scope.roundStyle = "task-status-completed task-outcome-cancelled";
                        } else if ($scope.round.voteType == 5) {
                            $scope.roundStyle = "task-status-completed task-outcome-cancelled";
                        }
                    } else {
                        $scope.roundStyle = "task-status-none";
                    }
                },
            ],
        };
    },
]);

ssgApp.directive('showMore', function () {
    var templateShowMore = '<span class="{{class}}" ">{{truncate}}</span>' +
        '<a class="no-underline" id="wordForTruncate"  ng-click="checkCondition()">{{replace && text.length > number  ? more : !replace && text.length > number ? less : ""}}</a>';
    return {
        restrict: "E",
        scope: {
            more: "@more",
            less: "@less",
            text: "=text",
            number: "@number",
            class: "@class",
        },
        template: templateShowMore,
        link: function ($scope, $element, attr) {
            var comment = $scope.text && $scope.text.substring($scope.number, $scope.text.length);
            $scope.truncate = $scope.text && $scope.text.replace(comment, $scope.text.length > $scope.number ? "..." : "");
            $scope.replace = true;
            $scope.checkCondition = function () {
                if ($scope.replace && $scope.text.length > $scope.number) {
                    $scope.showFullText();
                } else {
                    $scope.hideFullText();
                }
            };

            $scope.showFullText = function () {
                $scope.truncate = $scope.text;
                $scope.replace = false;
            };

            $scope.hideFullText = function () {
                $scope.text.substring($scope.number, $scope.text.length);
                $scope.truncate = $scope.text.replace(comment, "...");
                $scope.replace = true;
            };
        }
    };
});
