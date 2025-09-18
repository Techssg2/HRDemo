var ssgApp = angular.module('ssg.directiveSubModule', []);
ssgApp.directive("subMenuSection", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/sub-menu-section/sub-menu-section.template.html?v=" + edocV,
            scope: {
                titleSection: "=",
                listMenu: '='
            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope", "appSetting",
                function ($rootScope, $scope, appSetting) {
                    let role = $rootScope.currentUser.role;
                    let grade = $rootScope.currentUser.jobGradeValue;

                    let listMenu = _.filter($scope.listMenu, x => {
                        let indexRole = _.findIndex(x.roles, y => {
                            return (y & role) == y;
                        });
                        let type = _.findIndex(x.type, y => {
                            return (y & $rootScope.currentUser.type) == y;
                        });
                        let indexGrade = 0;
                        if (($rootScope.currentUser.role & appSetting.role.SAdmin) != appSetting.role.SAdmin && ($rootScope.currentUser.role & appSetting.role.HRAdmin) != appSetting.role.HRAdmin) {
                            indexGrade = _.findIndex(x.gradeUsers, y => {
                                return (y & $rootScope.currentUser.jobGradeValue) == y;
                            });
                            if (x.isFacility) {
                                return $rootScope.currentUser.isFacility;
                            } else {
                                if (x.isHQ && $rootScope.currentUser.jobGradeValue == 1) {
                                    if (x.isAdmin || x.isAccounting) {
                                        return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != x.isHQ && ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                                    }
                                    return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != x.isHQ;
                                }
                                if (x.isAdmin || x.isAccounting) {
                                    return indexRole > -1 && indexGrade > -1 && type > -1 && ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                                }
                            }
                        }
                        return indexRole > -1 && indexGrade > -1 && type > -1 && x.addToMenu;
                    });
                    console.log(listMenu);
                    $scope.vm = {
                        title: $scope.titleSection,
                        listMenu: listMenu
                    };
                },
            ],
        };
    },
]);