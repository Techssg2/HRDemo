var ssgApp = angular.module('appModule', ['underscore', 'kendo.directives']);
ssgApp.controller('appController', function ($rootScope, $scope, $state, appSetting, $translate, $stateParams, $history, commonData, $interval, $window, settingService, localStorageService, $timeout, settingService, dashboardService, ssgexService, $sce, $transitions, $window) {
    var ssg = this;
    $("#dialog").kendoDialog({
        visible: false
    });
    ssg.dialogConfirmParam = {
        data: null
    };
    ssg.dialogConfirmParamYN = {
        data: null
    };

    $rootScope.window = angular.element($window);
    $rootScope.width = $rootScope.window.innerWidth();
    $rootScope.window.bind('resize', function () {
        $rootScope.width = $rootScope.window.innerWidth();
    });

    $rootScope.baseUrl = baseUrl;
    $rootScope.sortDateFormat = appSetting.sortDateFormat;
    $rootScope.longDateFormat = appSetting.longDateFormat;
    $rootScope.actionNeedShowPopup = appSetting.actionNeedShowPopup;
    $rootScope.pageSizeDefault = 10;
    $rootScope.isLoading = false;
    $rootScope.reasonObject = {
        comment: ''
    };
    $rootScope.childMenus = [];
    $rootScope.addSectionMenu = [];
    $rootScope.isParentMenu = true;
    $rootScope.dialogVisible = false;
    $rootScope.checkedPermission = false;
    $rootScope.workflowItemStatus = appSetting.workflowItemStatus;
    $rootScope.switchLang = function () {
        // You can change the language during runtime
        if ($translate.use() == 'en_US') {
            sessionStorage.lang = 'vi_VN';
            localStorageService.set('lang', 'vi_VN');
        } else {
            sessionStorage.lang = 'en_US';
            localStorageService.set('lang', 'en_US');
        }
        location.reload();
    };
    $rootScope.setLang = function (langKey) {
        // You can change the language during runtime
        $translate.use(langKey);
    };
    $rootScope.getLang = function () {
        // You can change the language during runtime
        return $translate.use();
    };
    if ($state.current && $state.current.params) {
        $state.current.params.id = $stateParams.id;
        $state.current.params.referenceValue = $stateParams.referenceValue;
    }
    
    function getAddSectionMenu() {
        let subMenuSections = _.filter(appSetting.subSections, x => {
            let indexRole = _.findIndex(x.roles, y => {
                return (y & $rootScope.currentUser.role) == y;
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
                        return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                    }
                }

            }
            return indexRole > -1 && indexGrade > -1 && type > -1;
        });
        subMenuSections.forEach(element => {
            let sections = _.filter(element.sections, x => {
                let indexRole = _.findIndex(x.roles, y => {
                    return (y & $rootScope.currentUser.role) == y;
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
                            return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                        }
                    }

                }

                return indexRole > -1 && indexGrade > -1 && type > -1;
            });
            let tempMenus = [];
            sections.forEach(x => {
                tempMenus.push(x);
            });
            if (element.title == 'COMMON_FIN') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_FIN';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_FIN', title: $translate.instant('COMMON_FIN'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_FIN';
                });
                current.values = tempMenus;
            } else if (element.title == 'COMMON_RECRUIT') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_RECRUIT';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_RECRUIT', title: $translate.instant('COMMON_RECRUIT'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_RECRUIT';
                });
                current.values = current.values.concat(tempMenus);
            } else if (element.title == 'COMMON_CB') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_CB';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_CB', title: $translate.instant('COMMON_CB'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_CB';
                });
                current.values = current.values.concat(tempMenus);
            }
            else if (element.title == 'COMMON_SETTING') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_SETTING';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_SETTING', title: $translate.instant('COMMON_SETTING'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_SETTING';
                });
                current.values = current.values.concat(tempMenus);
            }
            else if (element.title == 'COMMON_REPORT') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_REPORT';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_REPORT', title: $translate.instant('COMMON_REPORT'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_REPORT';
                });
                current.values = current.values.concat(tempMenus);
            }
            else if (element.title == 'COMMON_BUDGET') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_BUDGET';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_BUDGET', title: $translate.instant('COMMON_BUDGET'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_BUDGET';
                });
                current.values = current.values.concat(tempMenus);
            }
            else if (element.title == 'COMMON_CONTRACT') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_CONTRACT';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_CONTRACT', title: $translate.instant('COMMON_CONTRACT'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_CONTRACT';
                });
                current.values = current.values.concat(tempMenus);
            }
            else if (element.title == 'COMMON_PURCHASING') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_PURCHASING';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_PURCHASING', title: $translate.instant('COMMON_PURCHASING'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_PURCHASING';
                });
                current.values = current.values.concat(tempMenus);
            }
            else if (element.title == 'COMMON_BTA') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_BTA';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_BTA', title: $translate.instant('COMMON_BTA'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_BTA';
                });
                current.values = current.values.concat(tempMenus);
            }
            else if (element.title == 'COMMON_FACILITY') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_FACILITY';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_FACILITY', title: $translate.instant('COMMON_FACILITY'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_FACILITY';
                });
                current.values = current.values.concat(tempMenus);
            }
        })
    }

    function resetSelectedMenu() {
        $rootScope.menus.forEach(element => {
            element.selected = false;
        });
        $timeout(function () {
            $rootScope.menus = _.filter(appSetting.menu, x => {
                let indexRole = _.findIndex(x.roles, y => {
                    return (y & $rootScope.currentUser.role) == y;
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
                            return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                        }
                    }

                }
                return indexRole > -1 && indexGrade > -1 && type > -1;
            });
            if ($rootScope.menus.length) {
                $rootScope.menus.forEach(x => {
                    if (x.childMenus) {
                        x.childMenus = _.filter(x.childMenus, child => {
                            let indexRole = _.findIndex(child.roles, y => {
                                return (y & $rootScope.currentUser.role) == y;
                            });
                            let type = _.findIndex(child.type, y => {
                                return (y & $rootScope.currentUser.type) == y;
                            });
                            let indexGrade = -1;
                            if (($rootScope.currentUser.role & appSetting.role.SAdmin) != appSetting.role.SAdmin && ($rootScope.currentUser.role & appSetting.role.HRAdmin) != appSetting.role.HRAdmin) {
                                indexGrade = _.findIndex(child.gradeUsers, y => {
                                    return (y & $rootScope.currentUser.jobGradeValue) == y;
                                });
                                if (child.isFacility) {
                                    return $rootScope.currentUser.isFacility;
                                } else {
                                    if (child.isHQ && $rootScope.currentUser.jobGradeValue == 1) {
                                        if (x.isAdmin || x.isAccounting) {
                                            return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != child.isHQ && ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                                        }
                                        return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != child.isHQ;
                                    }
                                    if (child.isAdmin || child.isAccounting) {
                                        return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                                    }
                                }

                            }
                            else {
                                indexGrade = 0;
                            }
                            return indexRole > -1 && indexGrade > -1 && type > -1;
                        });
                    }
                });
            }
        }, 0);
    }

    function updateSubMenu(stateName) {
        var current = stateName;
        let subMenuSections = _.filter(appSetting.subSections, x => {
            let indexRole = _.findIndex(x.roles, y => {
                return (y & $rootScope.currentUser.role) == y;
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
                        return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                    }
                }

            }
            return indexRole > -1 && indexGrade > -1 && type > -1;
        });
        if (subMenuSections.length) {
            subMenuSections.forEach(element => {
                element.sections = _.filter(element.sections, x => {
                    let indexRole = _.findIndex(x.roles, y => {
                        return (y & $rootScope.currentUser.role) == y;
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
                                return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                            }
                        }

                    }
                    return indexRole > -1 && indexGrade > -1 && type > -1;
                });
            });
        }
        if (current == 'home' || current == 'home.dashboard') {
            $rootScope.subMenuSection = subMenuSections;
        } else {
            if (current) {
                var subMenuSection = _.find(subMenuSections, x => {
                    return x.ref === current;
                });
                $rootScope.subMenuSection = [subMenuSection];
            }
        }
    }
    function createPageComponent(current) {
        var selectedMenu = {};
        $rootScope.currentUser.role = $rootScope.currentUser.role ? $rootScope.currentUser.role : 0;
        let role = $rootScope.currentUser.role;
        let jobGrade = $scope.currentUser.jobGradeValue;
        // Cấp cha        
        if (current === 'home') {
            resetSelectedMenu();
            selectedMenu = _.find($rootScope.menus, x => {
                return x.index === 0;
            });
            selectedMenu.selected = true;
        } else {
            if (current) {
                selectedMenu = _.find($rootScope.menus, x => {
                    if (current == 'home.user-setting.user-profile') {
                        return x.ref === 'home.setting';
                    }
                    return x.ref === current;
                });
            } else {
                selectedMenu = _.find($rootScope.menus, x => {
                    return x.selected === true;
                });
            }
        }
        if (selectedMenu) {
            $rootScope.title = selectedMenu.title.toUpperCase();
            $rootScope.childMenus = _.filter(selectedMenu.childMenus, x => {
                let indexRole = _.findIndex(x.roles, y => {
                    return (y & role) == y;
                });
                let type = _.findIndex(x.type, y => {
                    return (y & $rootScope.currentUser.type) == y;
                });
                let indexGrade = 0;
                if (($rootScope.currentUser.role & appSetting.role.SAdmin) != appSetting.role.SAdmin && ($rootScope.currentUser.role & appSetting.role.HRAdmin) != appSetting.role.HRAdmin) {
                    indexGrade = _.findIndex(x.gradeUsers, y => {
                        return (y & jobGrade) == y;
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
                            return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                        }
                    }

                }

                return indexRole > -1 && indexGrade > -1 && type > -1;
            });
            $rootScope.isParentMenu = selectedMenu.isParentMenu;
            updateSubMenu(current);
        } else {
            // Cấp con
            var item = null;
            appSetting.menu.forEach(x => {
                var temp = _.find(x.childMenus, y => {
                    return (_.findIndex(y.ref, x => {
                        return x.state === current
                    }) > -1);
                });
                if (temp) {
                    item = x;
                    $rootScope.isParentMenu = temp.isParentMenu;
                }
            });
            if (item) {
                $rootScope.title = item.title.toUpperCase();
                $rootScope.childMenus = item.childMenus;
            }
            navigateToState();
        }
    }

    async function loadPage() {
        var current = $state.current.name;
        if (localStorageService.get('invalidPermission')) {
            localStorageService.set('invalidPermission', false);
            window.location.href = baseUrl + "_layouts/15/SignOut.aspx";
        }
        if (!sessionStorage.currentUser || sessionStorage.currentUser == "undefined") {
            $timeout(function () {
                $state.go('redirectPage');
            }, 0);
        } else {
            $rootScope.currentUser = JSON.parse(sessionStorage.currentUser);
            if (!sessionStorage.lang) {
                //auto set language to vn for Grade 1
                if ($rootScope.currentUser.jobGradeValue == 1) {
                    sessionStorage.lang = 'vi_VN';
                    location.reload();
                    return;
                }
            }
            if (!$rootScope.checkedPermission) {
                $rootScope.menus = _.filter(appSetting.menu, x => {
                    let indexRole = _.findIndex(x.roles, y => {
                        return (y & $rootScope.currentUser.role) == y;
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
                                return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                            }
                        }
                    }
                    return indexRole > -1 && indexGrade > -1 && type > -1;
                });
                if ($rootScope.menus.length) {
                    $rootScope.menus.forEach(x => {
                        if (x.childMenus) {
                            x.childMenus = _.filter(x.childMenus, child => {
                                let indexRole = _.findIndex(child.roles, y => {
                                    return (y & $rootScope.currentUser.role) == y;
                                });
                                let type = _.findIndex(child.type, y => {
                                    return (y & $rootScope.currentUser.type) == y;
                                });
                                let indexGrade = 0;
                                if (($rootScope.currentUser.role & appSetting.role.SAdmin) != appSetting.role.SAdmin && ($rootScope.currentUser.role & appSetting.role.HRAdmin) != appSetting.role.HRAdmin) {
                                    indexGrade = _.findIndex(child.gradeUsers, y => {
                                        return (y & $rootScope.currentUser.jobGradeValue) == y;
                                    });
                                    if (child.isFacility) {
                                        return $rootScope.currentUser.isFacility;
                                    } else {
                                        if (child.isHQ && $rootScope.currentUser.jobGradeValue == 1) {
                                            if (x.isAdmin || x.isAccounting) {
                                                return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != child.isHQ && ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                                            }
                                            return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != child.isHQ;
                                        }
                                        if (child.isAdmin || child.isAccounting) {
                                            return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                                        }
                                    }

                                }
                                return indexRole > -1 && indexGrade > -1 && type > -1;
                            });
                        }
                    });
                }
                $rootScope.checkedPermission = true;
            }
            let isContinue = false;
            if ($rootScope.checkedPermission) {
                isContinue = checkActiveState($rootScope.currentUser.role, $rootScope.currentUser.jobGradeValue);
            }
            if (isContinue) {
                localStorageService.set('invalidPermission', false);
                getAddSectionMenu();
                //createPageComponent(current);
            } else {
                $timeout(function () {
                    $rootScope.permissionErrorMessage = '';
                    if (!$rootScope.currentUser.jobGradeValue) {
                        $rootScope.permissionErrorMessage = 'This user has not been assigned to a department yet/ Bạn chưa được phân bổ vào phòng ban';
                    } else {
                        $rootScope.permissionErrorMessage = 'You has not been  granted permission to access to the system/ Bạn chưa được cấp quyền truy cập vào hệ thống';
                    }
                    $state.go('notFoundPage');
                }, 0);
            }
        }

    };

    function checkActiveState(role, grade) {
        let currentState = $state.current.name;
        let valisActiveStateIndex = _.findIndex(appSetting.activeStates, x => {
            if (x.state == currentState) {
                let indexRole = _.findIndex(x.roles, y => {
                    return (y & role) == y && x.state == currentState;
                });
                let type = _.findIndex(x.type, y => {
                    return (y & $rootScope.currentUser.type) == y;
                });
                let indexGrade = 0;
                if (($rootScope.currentUser.role & appSetting.role.SAdmin) != appSetting.role.SAdmin && ($rootScope.currentUser.role & appSetting.role.HRAdmin) != appSetting.role.HRAdmin) {
                    indexGrade = _.findIndex(x.gradeUsers, y => {
                        return (y & grade) == y && x.state == currentState;
                    });
                    //Task - 516
                    if (grade === 1 && (x.state == "home.promoteAndTransfer.item" || x.state == "home.action.item")) {
                        indexGrade = 0;
                    }
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
                            return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || ($rootScope.currentUser.role & 64) == $rootScope.currentUser.role);
                        }
                    }

                }

                return indexRole > -1 && indexGrade > -1 && type > -1;
            }
        });
        return valisActiveStateIndex > -1;
    }

    function updatePermissionOnUI() {
        let role = $rootScope.currentUser.role;
    }

    function getCurrentStateInfo(stateParams) {
        if (stateParams.action) {
            if (!stateParams.action.title) {
                appSetting.menu.forEach(x => {
                    if (x.childMenus) {
                        x.childMenus.forEach(y => {
                            var currentRef = _.find(y.ref, item => {
                                return item.state === $state.current.name;
                            });
                            if (currentRef) {
                                isItem = currentRef.isItem;
                                var currentAction = null;

                                if (isItem) {
                                    if (stateParams.referenceValue) {
                                        currentAction = {
                                            title: `${currentRef.title}: ${stateParams.referenceValue}`
                                        }
                                    } else {
                                        currentAction = {
                                            title: `NEW ITEM: ${currentRef.title}`
                                        };
                                    }
                                } else {
                                    currentAction = _.find(y.actions, x => {
                                        return x.state === $state.current.name;
                                    });
                                }
                                if (currentAction) {
                                    stateParams.action.title = currentAction.title;
                                }
                            }
                        });
                    }

                });
            } else {
                stateParams.title = stateParams.action.title;
            }
        }
        return stateParams;
    }

    function navigateToState() {
        $stateParams = getCurrentStateInfo($state.params);
        // $state.go($state.current.name);
    }
    function updateChildMenus(childMenus) {
        let role = $rootScope.currentUser.role;
        let jobGrade = $rootScope.currentUser.jobGradeValue;
        let res = _.filter(childMenus, x => {
            if (x.actions && x.actions.length) {
                x.actions = _.filter(x.actions, k => {
                    return _.findIndex(k.roles, m => {
                        return (m & role) == m;
                    }) > -1;
                })
            }
            let indexRole = _.findIndex(x.roles, y => {
                return (y & role) == y;
            });
            let type = _.findIndex(x.type, y => {
                return (y & $rootScope.currentUser.type) == y;
            });
            let indexGrade = 0;
            if (($rootScope.currentUser.role & appSetting.role.SAdmin) != appSetting.role.SAdmin && ($rootScope.currentUser.role & appSetting.role.HRAdmin) != appSetting.role.HRAdmin) {
                indexGrade = _.findIndex(x.gradeUsers, y => {
                    return (y & jobGrade) == y;
                });
                if (x.isFacility) {
                    return $rootScope.currentUser.isFacility;
                } else {
                    if (x.isHQ && $rootScope.currentUser.jobGradeValue == 1) {
                        if (x.isAdmin || x.isAccounting) {
                            return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != x.isHQ && ($rootScope.currentUser.isAdmin || role & 64 == role);
                        }
                        return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != x.isHQ;
                    }
                    if (x.isAdmin || x.isAccounting) {
                        return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || role & 64 == role);
                    }
                }

            }
            return indexRole > -1 && indexGrade > -1 && type > -1;
        });
        return res;
    }

    function run() {
        $rootScope.selectedItem = function (item, isRoot = true) {
            if (item.isPhrase1) {
                $window.open(item.url, '_blank');
            } else if (item.childMenus || !item.actions) {
                resetSelectedMenu();
                if (item.isParentMenu && !item.actions) {
                    item.selected = true;
                    if (isRoot) {
                        $rootScope.title = item.title.toUpperCase();
                    }
                    item.childMenus = updateChildMenus(item.childMenus);
                    $rootScope.childMenus = item.childMenus;
                    $rootScope.isParentMenu = item.isParentMenu;
                    updateSubMenu(item.ref);
                } else {
                    $rootScope.isParentMenu = item.isParentMenu;
                }
            }
        };
        $rootScope.gotoDashboard = function () {
            resetSelectedMenu();
            var dashboardItem = _.find(appSetting.menu, x => {
                return x.ref === 'home.dashboard'
            });
            dashboardItem.selected = true;
            $rootScope.title = dashboardItem.title.toUpperCase();
            $rootScope.childMenus = dashboardItem.childMenus;
            $rootScope.isParentMenu = dashboardItem.isParentMenu;
            updateSubMenu(dashboardItem.ref);
            $history.resetAll();
            $state.go(dashboardItem.ref);
        }
        $rootScope.changeToChildView = function () {
            $rootScope.isParentMenu = false;
            //console.log($state);
            $state.go($state.current.name);
        }
        // base
        $rootScope.validateForm = function (form, requiredFields, model) {
            var errors = [];
            angular.forEach(form.$$controls, function (control) {
                if (!model[control.$name]) {
                    var field = _.find(requiredFields, x => {
                        return x.fieldName === control.$name
                    })
                    if (field) {
                        if (_.findIndex(errors, x => {
                            return x.fieldName === field.fieldName
                        }) === -1) {
                            errors.push({
                                controlName: field.title,
                                fieldName: field.fieldName
                            });
                        }
                    }
                }
            });
            return errors;
        }
        $rootScope.isoDate = function (date) {
            var dateNew = null;
            if (!date) {
                return null
            }
            date = moment(date, 'DD-MM-YYYY').toDate();
            var month = 1 + date.getMonth()
            if (month < 10) {
                month = '0' + month
            }
            var day = date.getDate()
            if (day < 10) {
                day = '0' + day
            }
            dateNew = date.getFullYear() + '-' + month + '-' + day;
            return dateNew;
        }
        $rootScope.cancel = function () {
            if ($history.previous().controller == "redirectPageController") {
                $rootScope.gotoDashboard();
            }
            else {
                $history.back();
            }
        }
        $rootScope.clearSearch = function (searchObject) {
            for (const property in searchObject) {
                if (Array.isArray(searchObject[property])) {
                    searchObject[property] = [];
                } else {
                    searchObject[property] = '';
                }
            }
        }
        $rootScope.reloadPage = function (current) {
            loadPage(current);
        }
        $rootScope.getCurrentStateInfo = function (stateParams) {
            return getCurrentStateInfo(stateParams);
        }
        $rootScope.updateSubMenu = function (stateName) {
            updateSubMenu(stateName);
        }
        $rootScope.openNotificationPanel = function () {
            //form notification
            var notification = document.getElementsByClassName("page-quick-sidebar-wrapper");
            notification[0].style.transition = "right 200ms linear";
            notification[0].style.right = "0";
            //icon close
            var iconClose = document.getElementsByClassName("page-quick-sidebar-toggler");
            iconClose[0].style.display = "block";
        }

        $rootScope.closeNotificationPanel = function () {
            //form notification
            var notification = document.getElementsByClassName("page-quick-sidebar-wrapper");
            notification[0].style.transition = "right 200ms linear";
            notification[0].style.right = "-320px";
            //icon close
            var iconClose = document.getElementsByClassName("page-quick-sidebar-toggler");
            iconClose[0].style.display = "none";
        }

        $rootScope.totalTask = 1;

        $rootScope.getStatusClass = function (dataItem) {
            let classStatus = "";
            switch (dataItem.status) {
                case commonData.Status.Waiting:
                    classStatus = "fa-circle font-yellow-lemon";
                    break;
                case commonData.Status.Completed:
                    classStatus = "fa-check-circle font-green-jungle";
                    break;
                case commonData.Status.Rejected:
                    classStatus = "fa-ban font-red";
                    break;
                default:
            }
            return classStatus;
        }
        $rootScope.getStatusTimeClass = function (dataItem) {
            let classStatus = "";
            switch (dataItem.status) {
                case commonData.StatusMissingTimeLock.WaitingHOD:
                    classStatus = "fa-circle font-yellow-lemon";
                    break;
                case commonData.StatusMissingTimeLock.WaitingCMD:
                    classStatus = "fa-circle font-yellow-lemon";
                    break;
                case commonData.StatusMissingTimeLock.Completed:
                    classStatus = "fa-check-circle font-green-jungle";
                    break;
                case commonData.StatusMissingTimeLock.Rejected:
                    classStatus = "fa-ban font-red";
                    break;
                default:
            }
            return classStatus;
        }
        // Dialog
        $rootScope.confirm = function (e) {
            ssg.dialogConfirmParam.data = {
                typeAction: 'confirm',
                value: true
            };
            var result = ssg.confirmDialog.close();
        }
        $rootScope.showConfirmDelete = function (title, content, actionName) {
            ssg.dialogConfirmParam.data = null;
            ssg.dialog = $("#dialogDelete").kendoDialog({
                title: title,
                width: "500px",
                modal: true,
                visible: false,
                animation: {
                    open: {
                        effects: "fade:in"
                    }
                },
                close: function (e) {
                    e.data = ssg.dialogConfirmParam.data;
                }
            });
            $scope.content = content;
            $scope.actionName = actionName;
            ssg.confirmDialog = ssg.dialog.data("kendoDialog");
            ssg.confirmDialog.open();
            $rootScope.confirmDialog = ssg.confirmDialog;
            return ssg.confirmDialog;
            // dialog.bind("close", confirm);
        }

        $rootScope.visibleProcessingStages = function ($translate) {
            let dialogReason = $("#processingStates").kendoDialog({
                // title: "Processing Stages",
                title: $translate.instant('COMMON_PROCESSING_STAGE'),
                width: "1000px",
                modal: true,
                visible: true,
                animation: {
                    open: {
                        effects: "fade:in"
                    }
                }
            });
            let boxReason = dialogReason.data("kendoDialog");
            boxReason.open();
            $rootScope.confirmProcessing = boxReason;
            return dialogReason;
        }

        //$rootScope.showConfirm = function(title, content, actionName) {
        //        ssg.dialogConfirmParam.data = null;
        //        ssg.dialog = $("#dialog").kendoDialog({
        //            title: title,
        //            width: "500px",
        //            modal: true,
        //            visible: false,
        //            animation: {
        //                open: {
        //                    effects: "fade:in"
        //                }
        //            },
        //            close: function(e) {
        //                e.data = ssg.dialogConfirmParam.data;
        //            }
        //        });
        //        $scope.content = content;
        //        $scope.actionName = actionName;
        //        ssg.confirmDialog = ssg.dialog.data("kendoDialog");
        //        ssg.confirmDialog.open();
        //        return ssg.confirmDialog;
        //        // dialog.bind("close", confirm);
        //    }
        // YES NO DIALOG
        $rootScope.confirmYN = function (choice) {
            if (choice === 1) {
                ssg.dialogConfirmParamYN.data = {
                    typeAction: 'confirm',
                    value: true
                };
            } else {
                ssg.dialogConfirmParamYN.data = {
                    typeAction: 'confirm',
                    value: false
                };
            }
            var result = ssg.confirmDialogYN.close();
        }
        $rootScope.showConfirmYN = function (title, content) {
            ssg.dialogConfirmParamYN.data = null;
            ssg.dialogYN = $("#dialogYN").kendoDialog({
                title: title,
                width: "500px",
                modal: true,
                visible: false,
                animation: {
                    open: {
                        effects: "fade:in"
                    }
                },
                close: function (e) {
                    e.data = ssg.dialogConfirmParamYN.data;
                }
            });
            $scope.contentYN = content;
            ssg.confirmDialogYN = ssg.dialogYN.data("kendoDialog");
            ssg.confirmDialogYN.open();
            return ssg.confirmDialogYN;
        }
        $rootScope.showPopup = function (propOfScope, config, idModal, actionFunc) {
            $scope[propOfScope] = {
                title: config.title,
                btnName: config.btnName,
                iconClass: config.iconClass,
                btnAction: function () {
                    $rootScope.sendReason(actionFunc, '#dialogReason');
                }
            }

            $rootScope.reasonObject.comment = '';

            let dialogReason = $(idModal).kendoDialog({
                title: config.title,
                width: "600px",
                modal: true,
                visible: false,
                animation: {
                    open: {
                        effects: "fade:in"
                    }
                }
            });
            let boxReason = dialogReason.data("kendoDialog");
            boxReason.open();
        }
        $rootScope.showDialogTimeOut = function (title) {
            ssg.dialogTimeOut = $("#dialogTimeOutId").kendoDialog({
                title: title,
                width: "500px",
                modal: true,
                visible: false,
                animation: {
                    open: {
                        effects: "fade:in"
                    }
                },
                close: function (e) {

                }
            });
            ssg.dialogTimeOut = ssg.dialogTimeOut.data("kendoDialog");
            ssg.dialogTimeOut.open();
            return ssg.dialogTimeOut;
        }
        $rootScope.sendReason = function (sendFunc, idModal) {
            if (sendFunc()) {
                let dialog = $(idModal).data("kendoDialog");
                dialog.close();
            }
        }
        $rootScope.dateValidator = function (customDateFields, model) {
            var errors = [];
            angular.forEach(customDateFields, function (field) {
                if (model[field.fieldName]) {
                    var date = moment(model[field.fieldName], appSetting.sortDateFormat);
                    if (!date._isValid) {
                        errors.push({
                            controlName: field.title,
                            fieldName: field.fieldName,
                            message: 'Incorrect format'
                        });
                    } else {
                        if (field.range) {
                            Z
                            let age = moment().diff(moment(model[field.fieldName], appSetting.sortDateFormat), 'years');
                            if (age < field.range.greaterThan || age > field.range.lessThan) {
                                errors.push({
                                    controlName: field.title,
                                    fieldName: field.fieldName,
                                    message: `Age is not valid (${field.range.greaterThan} - ${field.range.lessThan})`
                                });
                            }
                        }
                    }
                }
            });
            return errors;
        }
        $rootScope.createPageComponent = function (current) {
            createPageComponent(current);
        }

        $rootScope.dropdownFilter = function (e) {
            var filterValue = e.filter != undefined ? e.filter.value : "";
            e.preventDefault();

            this.dataSource.filter({
                logic: "or",
                filters: [{
                    field: "code",
                    operator: "contains",
                    value: filterValue
                },
                {
                    field: "name",
                    operator: "contains",
                    value: filterValue
                }
                ]
            });
        }
        // $interval(function () {
        //     if (sessionStorage.getItem('isShowTimeOutPopup') == null 
        //     || sessionStorage.getItem('isShowTimeOutPopup') == 'null' 
        //     || (localStorageService.get('inVisibleConfirmPopup') == '1' && localStorageService.get('waitingForLogout') != '1')) {
        //         sessionStorage.setItem('isShowTimeOutPopup', '1');
        //     }
        //     if (localStorageService.get('isTimeOut')) {
        //         $timeout(function () {
        //             localStorageService.remove('passTime'); // Thời gian đã trôi qua từ lúc đăng nhập vào hệ thống
        //             localStorageService.remove('accessSystemTime'); // Thời điểm truy cập vào hệ thống
        //             localStorageService.remove('isShowTimeOutPopup'); // Dùng để kiếm tra các popup timeout ở session khác còn đang mở
        //             sessionStorage.removeItem('inVisibleConfirmPopup'); // Popup time out trên session hiện tại có đang mở
        //             localStorageService.remove('waitingForLogout');
        //         }, 0);
        //         window.location.href = baseUrl + "_layouts/closeConnection.aspx?loginasanotheruser=true";
        //     } else {
        //         let passTime = new Date(localStorageService.get('passTime'));
        //         let accessTime = new Date(localStorageService.get('accessSystemTime'));
        //         let timeToCheck = Math.floor((passTime - accessTime) / 1000);
        //         $scope.timeOut = appSetting.expiredTimeOut - timeToCheck;
        //         //console.log(Math.floor(timeToCheck));
        //         if (timeToCheck >= appSetting.expiredTimeOut) {
        //             $timeout(function () {
        //                 localStorageService.remove('passTime');
        //                 localStorageService.remove('accessSystemTime');
        //                 sessionStorage.removeItem('isShowTimeOutPopup');
        //                 localStorageService.remove('inVisibleConfirmPopup');
        //                 localStorageService.remove('waitingForLogout');
        //             }, 0);
        //             window.location.href = baseUrl + "_layouts/closeConnection.aspx?loginasanotheruser=true";
        //             localStorageService.set('isTimeOut', '1');
        //         }
        //         if (timeToCheck >= appSetting.notifyTimeOut && sessionStorage.getItem('isShowTimeOutPopup') == '1' && localStorageService.get('waitingForLogout') != '1') {
        //             $rootScope.showDialogTimeOut('Time Out');
        //             sessionStorage.setItem('isShowTimeOutPopup', '0');
        //             localStorageService.set('passTime', new Date());
        //             localStorageService.set('inVisibleConfirmPopup', '0');
        //         } else {
        //             localStorageService.set('passTime', new Date());
        //             if (ssg && ssg.dialogTimeOut && localStorageService.get('inVisibleConfirmPopup') == '1') {
        //                 ssg.dialogTimeOut.close();                        
        //                 //sessionStorage.setItem('isShowTimeOutPopup', '0');
        //             }
        //         }
        //     }
        // }, 1000);
        $scope.confirmTimeOut = async function (choose) {
            if (choose) {
                var currentTime = new Date();
                localStorageService.set('passTime', currentTime);
                localStorageService.set('accessSystemTime', currentTime);
                sessionStorage.setItem('isShowTimeOutPopup', '1');
            } else {
                sessionStorage.setItem('isShowTimeOutPopup', null);
                localStorageService.set('waitingForLogout', '1');

            }

            localStorageService.set('inVisibleConfirmPopup', '1');
            ssg.dialogTimeOut.close();
        }
    }
    turnOnNotifyTimeOut = function () {

    }
    tunrnOffNotifyTimeOut = function () {

    }

    function watch() {
        $rootScope.$watch('isLoading', function (newValue, oldValue) {
            kendo.ui.progress($("#loading"), newValue);
        });
    }
    $rootScope.validateSapCode = function (sapCode) {
        let pattern = /^[a-zA-Z0-9-_]*$/;
        return pattern.test(sapCode);
    }
    this.$onInit = async function () {
        watch();
        run();
        await loadPage();
        await getSalaryDayConfiguration();
        $rootScope.setLang(localStorageService.get('lang'));
    }
    $scope.getTasks = async function () {
        let res = await ssgexService.getInstance().edoc1.getTasks({
            "LoginName": "edoc04",
            "Top": "10",
            "Skip": "0",
            "OrderBy": "Created"
        }).$promise;
    }

    $rootScope.validateInRecruitment = function (requiredFields, data) {
        let errors = [];
        Object.keys(data).forEach(function (fieldName) {
            let requiredFieldIndex = _.findIndex(requiredFields, x => {
                return x.fieldName === fieldName
            });
            if (requiredFieldIndex != -1 && !data[fieldName]) {
                errors.push({
                    fieldName: fieldName,
                    controlName: requiredFields[requiredFieldIndex].title,
                    errorDetail: 'is required',
                });
            }
        });
        if (errors.length === 0) {
            if (!$rootScope.validateSapCode(data.code)) {
                errors.push({
                    fieldName: 'Code',
                    controlName: 'Code',
                    errorDetail: 'has wrong format. (A-Z,0-9, - and _ )',
                });
            }
            //them cho xử lý timepicker:
            if (data.departureTime) {
                if (data.departureTime.indexOf('minutes') != -1 || data.departureTime.indexOf('hours') != -1) {
                    errors.push({
                        fieldName: 'Departure Time',
                        controlName: 'Departure Time',
                        errorDetail: 'has wrong format',
                    });
                }
            }
            //end
        }
        return errors;
    }

    $rootScope.getStatusTranslate = function (status) {
        let result = '';
        switch (status) {
            case 'Draft':
                result = $translate.instant('STATUS_DRAFT');
                break;
            case 'Pending':
                result = $translate.instant('STATUS_PENDING');
                break;
            case 'Rejected':
                result = $translate.instant('STATUS_REJECT');
                break;
            case 'Completed':
                result = $translate.instant('STATUS_COMPLETED');
                break;
            case 'Cancelled':
                result = $translate.instant('STATUS_CANCEL');
                break;
            case 'Out Of Period':
                result = $translate.instant('STATUS_OUT_OF_PERIOD');
                break;
            case 'Requested To Change':
                result = $translate.instant('STATUS_REQUEST_CHANGE');
                break;
            default:
                if (status != null && status != undefined && status.includes("Waiting for")) {
                    // var strReplaceWaiting = status.replace("Waiting", " ");
                    // var strReplaceFor = strReplaceWaiting.replace("for", " ");
                    // var strReplaceApproval = strReplaceFor.replace("Approval", " ");
                    // var res = strReplaceApproval.split(" ");
                    // var nameWorkFlow = '';
                    // res.forEach(item => {
                    //     if (item) {
                    //         nameWorkFlow = nameWorkFlow + ' ' + item + ' ';
                    //     }
                    // })
                    // result = $translate.instant('STATUS_WAITING') + ' ' + nameWorkFlow + $translate.instant('STATUS_APPROVAL')
                    var strReplaceWaiting = status.replace("Waiting for", $translate.instant('STATUS_WAITING'));
                    if (status.includes('Approval')) {
                        var translateApproval = $translate.instant('STATUS_APPROVAL');
                        result = strReplaceWaiting.replace("Approval", translateApproval);
                    } else if (status.includes('Submit')) {
                        result = strReplaceWaiting.replace("Submit", $translate.instant('STATUS_SUBMIT'));
                    } else {
                        var strAfterWaitingFor = status.replace('Waiting for', '');
                        var translateTextAfterWaitingFor = strAfterWaitingFor.trim();
                        result = strReplaceWaiting.replace(translateTextAfterWaitingFor, $translate.instant(translateTextAfterWaitingFor));
                    }
                } else {
                    result = $translate.instant(status);
                }
                break;
        }
        return result;
    }
    $scope.keyDown = function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            $rootScope.$emit('isEnterKeydown', { state: $state.current.name });
        }
    }

    $rootScope.keyDownTimePicker = function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 8 || keyCode === 13 || $($event.target).is("input, number")) {
            $event.preventDefault();
        }
    }
    $rootScope.keyDownTimePicker = function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 8 || keyCode === 13 || $($event.target).is("input, number")) {
            $event.preventDefault();
        }
    }
    async function getSalaryDayConfiguration() {
        let res = await settingService.getInstance().salaryDayConfiguration.getSalaryDayConfigurations({
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }).$promise;
        if (res && res.isSuccess) {
            let temp = [{}];
            res.object.data.forEach(el => {
                if (el.name) {
                    temp[0][el.name.charAt(0).toLowerCase() + el.name.substring(1)] = el.value;
                }
            });
            $rootScope.salaryDayConfiguration = temp[0];
        }
    }
    //Load lại thanh menu trong trường hợp nhấn nút back.
    $transitions.onSuccess({}, async function () {
        let current = $state.current.name;
        findMenuBar(current);
    });

    function findMenuBar(current) {
        // Vì lúc đầu load thì cái currentUser undefined, chạy lần 2 mói có, tránh lỗi.
        if ($rootScope.currentUser) {
            var selectedMenu = {};
            $rootScope.currentUser.role = $rootScope.currentUser.role ? $rootScope.currentUser.role : 0;
            let role = $rootScope.currentUser.role;
            let jobGrade = $scope.currentUser.jobGradeValue;
            // Cấp cha        
            if (current === 'home') {
                resetSelectedMenu();
                selectedMenu = _.find($rootScope.menus, x => {
                    return x.index === 0;
                });
                selectedMenu.selected = true;
            } else {
                if (current) {
                    selectedMenu = _.find($rootScope.menus, x => {
                        if (current == 'home.user-setting.user-profile') {
                            return x.ref === 'home.setting';
                        }
                        return x.ref === current;
                    });
                } else {
                    selectedMenu = _.find($rootScope.menus, x => {
                        return x.selected === true;
                    });
                }
            }
            if (selectedMenu) {
                $rootScope.title = selectedMenu.title.toUpperCase();
                $rootScope.childMenus = _.filter(selectedMenu.childMenus, x => {
                    if (x.actions && x.actions.length) {
                        x.actions = _.filter(x.actions, k => {
                            return _.findIndex(k.roles, m => {
                                return (m & role) == m;
                            }) > -1;
                        })
                    }
                    let indexRole = _.findIndex(x.roles, y => {
                        return (y & role) == y;
                    });
                    let type = _.findIndex(x.type, y => {
                        return (y & $rootScope.currentUser.type) == y;
                    });
                    let indexGrade = 0;
                    if (($rootScope.currentUser.role & appSetting.role.SAdmin) != appSetting.role.SAdmin && ($rootScope.currentUser.role & appSetting.role.HRAdmin) != appSetting.role.HRAdmin) {
                        indexGrade = _.findIndex(x.gradeUsers, y => {
                            return (y & jobGrade) == y;
                        });
                        if (x.isFacility) {
                            return $rootScope.currentUser.isFacility;
                        } else {
                            if (x.isHQ && $rootScope.currentUser.jobGradeValue == 1) {
                                if (x.isAdmin || x.isAccounting) {
                                    return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != x.isHQ && ($rootScope.currentUser.isAdmin || role & 64 == role);
                                }
                                return indexRole > -1 && indexGrade > -1 && type > -1 && $rootScope.currentUser.isStore != x.isHQ;
                            }
                            if (x.isAdmin || x.isAccounting) {
                                return (indexRole > -1 && indexGrade > -1 && type > -1) || ($rootScope.currentUser.isAdmin || role & 64 == role);
                            }
                        }

                    }
                    return indexRole > -1 && indexGrade > -1 && type > -1;
                });
                $rootScope.isParentMenu = selectedMenu.isParentMenu;
                updateSubMenu(current);
            } else {
                // Cấp con
                var item = null;
                appSetting.menu.forEach(x => {
                    var temp = _.find(x.childMenus, y => {
                        return (_.findIndex(y.ref, x => {
                            return x.state === current
                        }) > -1);
                    });
                    if (temp) {
                        item = x;
                        $rootScope.isParentMenu = temp.isParentMenu;
                    }
                });
                if (item) {
                    item.childMenus = updateChildMenus(item.childMenus);
                    $rootScope.title = item.title.toUpperCase();
                    $rootScope.childMenus = item.childMenus;
                }
                navigateToState();
            }
        }

    }

});