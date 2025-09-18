var ssgApp = angular.module('ssg.directiveLearnMoreModule', []);
ssgApp.directive("learnMore", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/learn-more/learn-more.template.html?v=" + edocV,
            scope: {
                group: "="
            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope", "appSetting",
                function ($rootScope, $scope, appSetting) {
                    $scope.link = '';
                    let actions = appSetting.linkActions;
                    if (actions) {
                        let currentLink = _.find(actions, x => { return x.group == $scope.group });
                        if (currentLink) {
                            $scope.link = currentLink.link;
                        }
                    }
                }
            ],
        };
    },
]);