var ssgApp = angular.module('appModule', ['underscore', 'kendo.directives']);
ssgApp.controller('appController', function ($rootScope, navigationService, $scope, $state, appSetting, $translate, $stateParams, $history, commonData, $interval, $window, settingService, localStorageService, $timeout, settingService, dashboardService, ssgexService, $sce, $transitions, $window, Notification, attachmentFile, attachmentService, dataService) {
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

    $scope.isCheckedAll = false;
    var allTasks = [];
    $rootScope.navigationLeft = [];
    $scope.navigationLeft = [];
    $rootScope.listNavigation = [];
    $rootScope.navigationRight = [];
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

    async function updateSubMenu() {
        let res;
        if ($rootScope.currentUser !== null && $rootScope.currentUser.id == -1) {
            res = await navigationService.getInstance().navigation.getListNavigationByUserIsNotEdocHR().$promise;
        }
        else {
            try {
                res = await navigationService.getInstance().navigation.getListNavigationByUserIdAndDepartmentId().$promise;
            }
            catch (err) {
                res = await navigationService.getInstance().navigation.getListNavigationByUserIdAndDepartmentIdV2({ UserId: $rootScope.currentUser.id, DepartmentId: $rootScope.currentUser.deptId }).$promise;
            }
        }
        if (res && res.isSuccess) {
            if (res.object.data != null && res.object.count > 0) {
                let objectData = res.object.data;

                let navigationLeft = objectData.filter((x) => x.type == 0);
                if (navigationLeft && navigationLeft.length) {
                    navigationLeft.forEach(x => {
                        var parent = {
                            id: x.id,
                            title: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                            name: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                            url: x.url
                        }
                        let existsChild = _.findIndex($rootScope.navigationLeft, y => {
                            return x.id == y.id;
                        });
                        if (existsChild == -1) {
                            $rootScope.navigationLeft.push(parent);
                            $scope.navigationLeft.push(parent);
                        }
                    })
                }

                let navigationCenter = objectData.filter((x) => x.type == 1);
                if (navigationCenter && navigationCenter.length) {
                    navigationCenter.forEach(x => {
                        var el1 = {
                            title: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                            url: x.url
                        }
                        var sections = [];
                        let child = JSON.parse(x.jsonChild);
                        if (child && child.length) {
                            child.forEach(xx => {
                                var title = $translate.use() == 'en_US' ? xx.Title_EN : xx.Title_VI;

                                let existsChild = _.findIndex(sections, x => {
                                    return x.id == xx.Id;
                                });
                                if (existsChild == -1) {
                                    var title = $translate.use() == 'en_US' ? xx.Title_EN : xx.Title_VI;
                                    var childSection = {
                                        id: xx.Id,
                                        name: title,
                                        nameAdd: title,
                                        title: title,
                                        ref: xx.Url,
                                        url: '',
                                        icon: baseUrlApi.replace('/api', xx.ProfilePicture.trim()),
                                        addToMenu: true,
                                        gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                                        roles: [1, 2, 4, 8, 16, 32],
                                        type: [0],
                                        departments: xx.Departments,
                                        users: xx.Users
                                    };
                                    sections.push(childSection);
                                }
                            });
                            el1.sections = sections;
                            let existsCenter = _.findIndex($rootScope.listNavigation, y => {
                                return y.title == el1.title;
                            });
                            if (existsCenter == -1) {
                                $rootScope.listNavigation.push(el1);
                            }
                        }
                    })
                }

                let navigationRight = objectData.filter((x) => x.type == 2);
                if (navigationRight && navigationRight.length) {
                    navigationRight.forEach(x => {
                        var parent = {
                            title: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                            name: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                            url: x.url
                        }
                        let values = [];
                        if (x.jsonChild != null) {
                            let child = JSON.parse(x.jsonChild);
                            child.forEach(x => {
                                var child = {
                                    id: x.Id,
                                    title: $translate.use() == 'en_US' ? x.Title_EN : x.Title_VI,
                                    name: $translate.use() == 'en_US' ? x.Title_EN : x.Title_ENTitle_VI,
                                    url: x.Url
                                }

                                let existsChild = _.findIndex(values, z => {
                                    return x.Id == z.id;
                                });

                                if (existsChild == -1) {
                                    values.push(child);
                                }
                            })
                        }
                        parent.values = values;
                        let existsCenter = _.findIndex($rootScope.navigationRight, y => {
                            return y.title == parent.title;
                        });
                        if (existsCenter == -1) {
                            $rootScope.navigationRight.push(parent);
                        }
                    })
                }
            }
        }
        $scope.$apply();
        /*$rootScope.subMenuSection = subMenuSections;*/
    }
    async function createPageComponent(current) {
        if ($rootScope.currentUser !== null && $rootScope.currentUser.id == -1) {
            getListNavigationRighCentertLeftV2();
            await updateSubMenu(current);
        }
        else {
            getListNavigationRighCentertLeft();
            await updateSubMenu(current);
        }
    }

    function getListNavigationRighCentertLeft() {
        var dashboardItem = _.find(appSetting.menu, x => {
            return x.ref === 'home.navigation-home'
        });
        if (dashboardItem) {
            dashboardItem.selected = true;
            $rootScope.title = dashboardItem.title.toUpperCase();
            // fix sapcode cung
            let scopeChild = _.filter(dashboardItem.childMenus, y => {
                if (y.sapCodes) {
                    if ($rootScope.currentUser && $rootScope.currentUser.sapCode && y.sapCodes.includes($rootScope.currentUser.sapCode)) {
                        if (y.url == 'home.navigation-list') {
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            });
            $rootScope.childMenusLeft = scopeChild;
        }
    }

    function getListNavigationRighCentertLeftV2() {
        var dashboardItem = _.find(appSetting.menu, x => {
            return x.ref === 'home.navigation-home'
        });
        if (dashboardItem) {
            dashboardItem.selected = true;
            $rootScope.title = dashboardItem.title.toUpperCase();
            $rootScope.childMenusLeft = dashboardItem.childMenus;
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
        }
    };

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
            /* resetSelectedMenu();
             var dashboardItem = _.find(appSetting.menu, x => {
                 return x.ref === 'home.dashboard'
             });
             dashboardItem.selected = true;
             $rootScope.title = dashboardItem.title.toUpperCase();
             $rootScope.childMenus = dashboardItem.childMenus;
             $rootScope.isParentMenu = dashboardItem.isParentMenu;
             updateSubMenu(dashboardItem.ref);
             $history.resetAll();
             $state.go(dashboardItem.ref);*/
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
                width: "1117px",
                modal: true,
                visible: true,
                animation: {
                    open: {
                        effects: "fade:in"
                    }
                }
            });
            let boxReason = dialogReason.data("kendoDialog");
            console.log('dialog', dialogReason)
            boxReason.open();
            $rootScope.confirmProcessing = boxReason;
            return dialogReason;
        }
        $rootScope.visibleActionHistory = function ($translate) {
            let dialogReason = $("#actionHitories").kendoDialog({
                title: $translate.instant('History'),
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
            console.log('dialog', dialogReason)
            boxReason.open();
            $rootScope.confirmProcessing = boxReason;
            return dialogReason;
        }

        // show loading
        $rootScope.loadingDialog = function (message, open) {
            if (open === true) {
                let dialogReason = $("#dialog_Loading").kendoDialog({
                    width: "500px",
                    buttonLayout: "normal",
                    height: "400px",
                    closable: false,
                    modal: true,
                    visible: false,
                    content: "",
                    close: async function (e) {
                    }
                });
                if (message === null || message === undefined)
                    $scope.messageLoading = $translate.instant('COMMON_WAITING_PROCESSING_LOADING');
                else
                    $scope.messageLoading = $translate.instant(message);
                let boxReason = dialogReason.data("kendoDialog");
                boxReason.open();
                $rootScope.confirmProcessing = boxReason;
                return dialogReason;
            }
            else {
                let dialogs = $("#dialog_Loading").data("kendoDialog");
                dialogs.close();
            }
        }

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

        $rootScope.showConfirmYNDialog = function (title, content) {
            ssg.dialogConfirmParamYN.data = null;
            ssg.dialogYN = $("#dialogYNDialog").kendoDialog({
                title: $translate.instant(title),
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
            $scope.contentYN = $translate.instant(content);
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
        $rootScope.createPageComponent = function (current) {
            createPageComponent(current);
        }
    }
    function watch() {
        $rootScope.$watch('isLoading', function (newValue, oldValue) {
            kendo.ui.progress($("#loading"), newValue);
        });
    }
    this.$onInit = async function () {
        watch();
        run();
        await loadPage();
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
    $rootScope.getItemTypeByReferenceNumber = function (referenceNumber) {
        let returnValue = "";
        if (referenceNumber.startsWith('RES-')) returnValue = "ResignationApplication";
        else if (referenceNumber.startsWith('SHI-')) returnValue = "ShiftExchangeApplication";
        else if (referenceNumber.startsWith('LEA-')) returnValue = "LeaveApplication";
        else if (referenceNumber.startsWith('OVE-')) returnValue = "OvertimeApplication";
        else if (referenceNumber.startsWith('BOB-')) returnValue = "BusinessTripOverBudget";
        else if (referenceNumber.startsWith('TAR-')) returnValue = "TargetPlan";
        else if (referenceNumber.startsWith('RES-')) returnValue = "ResignationApplication";
        else if (referenceNumber.startsWith('MIS-')) returnValue = "MissingTimeClock";
        else if (referenceNumber.startsWith('PRO-')) returnValue = "PromoteAndTransfer";
        else if (referenceNumber.startsWith('BTA-')) returnValue = "BusinessTripApplication";
        else if (referenceNumber.startsWith('REQ-')) returnValue = "RequestToHire";
        else if (referenceNumber.startsWith('ACT-')) returnValue = "Acting";
        else if (referenceNumber == 'TrackingRequest') returnValue = "TrackingRequest";
        return returnValue;
    }

    $rootScope.downloadAttachment = async function (id) {
        let result = await attachmentFile.downloadAndSaveFile({
            id
        });
    }

    $scope.getFilePath = async function (id) {
        let result = await attachmentFile.getFilePath({
            id
        });

        return result.object;
    }

    $scope.downloadAttachment = function (id) {
        $rootScope.downloadAttachment(id);
    }

    $scope.viewFileOnline = function (id) {
        $rootScope.viewFileOnline(id);
    }

    $rootScope.openWindow = function (url) {
        $window.open(url, '_blank');
    }
    $rootScope.blockAccess = function () {

        if ($rootScope.currentUser.jobGradeValue == 1) {
            $timeout(function () {
                $state.go('accessDeniedPage');
            }, 0);
        }
    }
});