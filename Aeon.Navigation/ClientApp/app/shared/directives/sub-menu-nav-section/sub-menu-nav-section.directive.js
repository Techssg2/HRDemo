var ssgApp = angular.module('ssg.directiveSubNavModule', []);
ssgApp.directive("subMenuNavSection", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/sub-menu-nav-section/sub-menu-nav-section.template.html?v=" + edocV,
            scope: {
                titleSection: "=",
                listMenu: '=',
                urlSection: "="
            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope", "appSetting", "$window",
                function ($rootScope, $scope, appSetting, $window) {
                    let role = $rootScope.currentUser.role;
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
                    //console.log(listMenu);
                    $scope.vm = {
                        title: $scope.titleSection,
                        url: $scope.urlSection,
                        listMenu: $scope.listMenu
                    };

                    $scope.openNewWindow = function (item, isRoot = true) {
                        if (item) {
                            $window.open(item, '_blank');
                        }
                    };
                },
            ],
        };
    },
]);