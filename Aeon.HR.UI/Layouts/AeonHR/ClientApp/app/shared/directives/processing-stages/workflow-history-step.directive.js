var ssgApp = angular.module('ssg.directiveWorkflowStepModule', []);
ssgApp.directive("workflowStep", [
    function ($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/processing-stages/workflow-history-step.template.html?v=" + edocV,
            scope: {
                step: "=",
                flow: "=",
                model: "="
            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope", "dataService",
                function ($rootScope, $scope, dataService) {
                    $scope.allowShowThisStep = function () {
                        let returnValue = true;
                        if ($scope.step.isStepWithConditions == true) {
                            //filter failed case
                          
                            let failedResult = $($scope.step.stepConditions)
                                .map(function (cIndex, cCondition) {
                                    return $scope.doesMatchStepConditions(cCondition);
                                })
                                .filter(function (cIndex, cItem) {
                                    return cItem == false;
                                });

                            //If has any case mot match this step will not be display
                            returnValue = failedResult.length == 0;

                        }
                        return returnValue;
                    }

                    $scope.doesMatchStepConditions = function (stepCondition) {
                        let returnValue = true;
                        if (!$.isEmptyObject(stepCondition) && !$.isEmptyObject(stepCondition.fieldValues)) {

                            let getFieldValueIgnoreCaseSensitive = function (obj, keyName) {
                                let allKeys = Object.keys(obj);
                                let matchKeys = $(allKeys).filter((cIndex, cKey) => cKey.toLowerCase() == keyName.toLowerCase());
                                if (!$.isEmptyObject(matchKeys) && matchKeys.length > 0) {
                                    return obj[matchKeys[0]] + "";
                                }
                            }

                            let fieldValue = getFieldValueIgnoreCaseSensitive($scope.model, stepCondition.fieldName);
                            returnValue = stepCondition.fieldValues.indexOf(fieldValue) > -1;
                        }
                        return returnValue;
                    }

                    $scope.init = function () {
                        var instances = dataService.workflowStatus.workflowInstances;
                        if (instances) {
                            var index = _.findIndex(instances, x => { return x.id == $scope.flow.id });
                            var rounds = [];
                            var round = _.find(instances[index].histories, { stepNumber: $scope.step.stepNumber });
                            if (round) {
                                if (round.stepNumber < instances[index].histories.length) {
                                    round.isStepCompleted = true;
                                }
                                rounds.push(round);
                            } else {
                                if ($scope.allowShowThisStep()) {
                                    rounds.push({})
                                }
                            }
                            $scope.step.rounds = rounds;
                            $scope.step.stepName = $scope.step.stepName.trim();
                        }
                    }
                    $scope.init();
                },
            ],
        };
    },
]);