var ssgApp = angular.module('appModule', ['underscore', 'kendo.directives']);
ssgApp.controller('appController', function ($rootScope, $scope, $state, appSetting, $translate, $stateParams, $history, commonData, $interval, $window, settingService, localStorageService, $timeout, settingService, dashboardService, ssgexService, $sce, $transitions, $window, Notification, attachmentFile, attachmentService, dataService, $http) {
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
    $scope.urlNavigation = appSetting.NavgationUrl ? appSetting.NavgationUrl : "";

    $rootScope.window = angular.element($window);
    $rootScope.width = $rootScope.window.innerWidth();
    $rootScope.window.bind('resize', function () {
        $rootScope.width = $rootScope.window.innerWidth();
    });

    $rootScope.showLoading = function () {
        var target = $("body");
        kendo.ui.progress(target, true);
        target.addClass("k-loading-mask");
        target.addClass("k-loading-mask-new");
    }
    $rootScope.hideLoading = function () {
        var target = $("body");
        kendo.ui.progress(target, false);
        target.removeClass("k-loading-mask");
        target.removeClass("k-loading-mask-new");
    }

    $rootScope.showLoadingByElement = function (id) {
        var target = $(`#${id}`);
        kendo.ui.progress(target, true);
        target.addClass("k-loading-mask");
    }

    $rootScope.hideLoadingByElement = function (id) {
        var target = $(`#${id}`);
        kendo.ui.progress(target, false);
        target.addClass("k-loading-mask");
    }

    //$scope.homeUpgradeUrlApi = "https://edocv2.aeon.com.vn";
    $scope.avatarDefault = "https://static.vecteezy.com/system/resources/previews/009/292/244/original/default-avatar-icon-of-social-media-user-vector.jpg";
    $rootScope.baseUrl = baseUrl;
    $rootScope.sortDateFormat = appSetting.sortDateFormat;
    $rootScope.longDateFormat = appSetting.longDateFormat;
    $rootScope.actionNeedShowPopup = appSetting.actionNeedShowPopup;
    $scope.deptAcademyCode = '50022367';
    $rootScope.pageSizeDefault = 10;
    $rootScope.isLoading = false;
    $rootScope.reasonObject = {
        comment: ''
    };
    $rootScope.childMenus = [];
    $rootScope.addSectionMenu = [];
    //$scope.sectionMenuHR = {
    //    childMenus: [
    //        {
    //            name: "COMMON_ADD_LEAVE_MANAGEMENT",
    //            title: "Leave",
    //            ref: "home.leavesManagement.allRequests",
    //            url: '',
    //            icon: "ClientApp/assets/images/icon/Leave-management.jpg",
    //            addToMenu: true,
    //            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
    //            roles: [1, 2, 4, 8, 16, 32],
    //            isNotTargetPlan: true,
    //            type: [0, 1],
    //        },
    //        {
    //            name: "COMMON_ADD_MISSING_TIMECLOCK",
    //            title: "Missing timeclock",
    //            ref: "home.missingTimelock.allRequests",
    //            url: '',
    //            icon: "ClientApp/assets/images/icon/Missing-Timelock.png",
    //            addToMenu: true,
    //            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
    //            roles: [1, 2, 4, 8, 16, 32],
    //            isNotTargetPlan: true,
    //            type: [0, 1],
    //        },
    //        {
    //            name: "COMMON_ADD_OVERTIME",
    //            title: "Overtime",
    //            ref: "home.overtimeApplication.allRequests",
    //            url: '',
    //            icon: "ClientApp/assets/images/icon/OverTime.jpg",
    //            addToMenu: true,
    //            gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
    //            roles: [1, 2, 4, 8, 16, 32],
    //            isNotTargetPlan: true,
    //            type: [0, 1],
    //        },
    //        {
    //            name: "ADD_TARGET_PLAN",
    //            title: "Target plan",
    //            ref: "home.targetPlan.allRequests",
    //            url: '',
    //            icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
    //            addToMenu: true,
    //            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
    //            roles: [1, 2, 4, 8, 16, 32],
    //            type: [0, 1],
    //            isNotTargetPlan: true,
    //            isHQ: true
    //        },
    //        {
    //            name: "SHIFT_PLAN",
    //            title: "Shift Plan",
    //            ref: "home.targetPlan.shiftPlan",
    //            url: '',
    //            icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
    //            addToMenu: true,
    //            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
    //            roles: [1, 2, 4, 8, 16, 32],
    //            type: [0, 1],
    //            isNotTargetPlan: true,
    //            isHQ: true
    //        },
    //        {
    //            name: "REPORT_TARGET_PLAN",
    //            title: "Target Plan Report",
    //            ref: "home.targetPlan.reports",
    //            url: '',
    //            icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
    //            addToMenu: true,
    //            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
    //            roles: [1, 2, 4, 8, 16, 32],
    //            type: [0, 1],
    //            isNotTargetPlan: true,
    //            isHQ: true
    //        },
    //        {
    //            name: "COMMON_ADD_SHIFT_EXHCNAGE",
    //            title: "Shift exchange",
    //            ref: "home.shiftExchange.allRequests",
    //            url: '',
    //            icon: "ClientApp/assets/images/icon/Shift-Exchange.jpg",
    //            addToMenu: true,
    //            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
    //            roles: [1, 2, 4, 8, 16, 32],
    //            type: [0, 1],
    //            isNotTargetPlan: true,
    //            isHQ: true
    //        },
    //        {
    //            name: "COMMON_ADD_RESIGNATION",
    //            title: "Resignation",
    //            ref: "home.resignationApplication.allRequests",
    //            url: '',
    //            icon: "ClientApp/assets/images/icon/Resignation.jpg",
    //            addToMenu: true,
    //            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
    //            roles: [1, 2, 4, 8, 16, 32],
    //            type: [0, 1],
    //        }
    //    ]
    //}


    $scope.sectionRecruitment = {
        childMenus: [
            {
                title: "Applicant Status List",
                ref: "home.settingRecruitmentApplicantStatus"
            },
            {
                title: "Appreciation List",
                ref: "home.settingRecruitmentAppreciationList"
            },
            {
                title: "Cost Center",
                ref: "home.settingRecruitmentCostCenter"
            },
            {
                title: "Item List",
                ref: "home.settingRecruitmenItemList"
            },
            {
                title: "Position List",
                ref: "home.settingRecruitmentPosition"
            },
            {

                title: "Working Address",
                ref: "home.settingRecruitmentWorkingAddress"

            },
            {
                title: "Working Time",
                ref: "home.settingRecruitmentWorkingTime"
            },
            {
                title: "Categories",
                ref: "home.settingRecruitmentCategories"
            }
        ]
    }

    $scope.sectionAca = {
        childMenus: [
            {
                title: "Training Request",
                ref: "home.academy-allRequests"
            },
            {
                title: "Training Invitation",
                ref: "home.academy-allInvitation"
            },
            {
                title: "Training Report",
                ref: "home.academy-allReport"
            },
            {
                title: "Report",
                childLV2: [
                    {
                        title: "Training Tracker Report",
                        ref: "home.academy-trainingTrackerReport"
                    },
                    {
                        title: "Training Survey Report",
                        ref: "home.academy-trainingSurveyReport"
                    },
                    {
                        title: "Training Budget Balance Report",
                        ref: "home.training-budget-balance-report"
                    },
                    {
                        title: "Trainer Contribution Report",
                        ref: "home.academy-trainerContributionReport"
                    }
                ]
            }
        ]
    };


    $scope.sectionAcaMaster = {
        childMenus: [
            {
                title: "Reason of Training Request",
                ref: "home.academy-trainingReason"
            },
            {
                title: "Course management",
                ref: "home.academy-courseManagement"
            },
            {
                title: "Training Request Management",
                ref: "home.academy-trainingRequestManagement"
            },
            {
                title: "Training Invitation Management",
                ref: "home.academy-manageInvitation"
            }
        ]
    };


    $scope.sectionCB = {
        childMenus: [
            {
                title: "Missing Timeclock Reasons",
                ref: "home.missingTimeclockReasons"
            },
            {
                title: "Overtime Reasons",
                ref: "home.overtimeReasons"
            },
            {
                title: "Shift Exchange Reasons",
                ref: "home.shiftExchangeReasons"
            },
            {
                title: "Resignation Reasons",
                ref: "home.resignationReasons"
            },
            {
                title: "Shift Plan",
                childLV2: [
                    {
                        title: "Shift Plan Submit Person",
                        ref: "home.shiftPlanSubmitPerson"
                    },
                    {
                        title: "Holiday Schedule",
                        ref: "home.holidaySchedule"
                    },
                    {
                        title: "Shift Code",
                        ref: "home.shiftCode"
                    },
                    {
                        title: "Target Plan Special",
                        ref: "home.targetPlanSpecial"
                    }
                ]
            }
        ]
    };

    $scope.sectionMore = {
        childMenus: [
            {
                title: "Workflow",
                ref: "home.workflowszzyyxx"
            },
            {
                title: "Reference Number",
                ref: "home.referenceNumbers"
            },
            {
                title: "Job Grade",
                ref: "home.jobGrades"
            },
            {
                title: "Tracking Log",
                ref: "home.trackingLogs"
            },
            {
                title: "Tracking Sync Orgchart ",
                ref: "home.trackingSyncOrgcharts"
            },
            {
                title: "Business Trip Application",
                childLV2: [
                    {
                        title: "Reason",
                        ref: "home.reason"
                    }, {
                        title: "Budget Limit: Special Case",
                        ref: "home.btaPolicySpecial"
                    }, {
                        title: "Budget Limit: HQ & Store",
                        ref: "home.btaPolicy"
                    }, {
                        title: "Room Type",
                        ref: "home.roomType"
                    }, {
                        title: "Business Trip Location",
                        ref: "home.location"
                    }, {
                        title: "Flight Number",
                        ref: "home.flightNumber"
                    }, {
                        title: "Hotel",
                        ref: "home.hotel"
                    }, {
                        title: "Partition",
                        ref: "home.partition"
                    }, {
                        title: "Airline",
                        ref: "home.airline"
                    }
                ]
            },
            {
                title: "Days Configuration",
                ref: "home.daysConfiguration"
            },
            {
                title: "Business Model Unit Mapping",
                ref: "home.businessModelUnitMapping"
            },
            {
                title: "Business Model",
                ref: "home.businessModel"
            }
        ]
    };



    $scope.sectionMenuHR = {
        childMenus: [
            {
                name: "COMMON_ADD_LEAVE_MANAGEMENT",
                title: "Leave",
                ref: "home.leavesManagement.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/Leave-management.jpg",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                isNotTargetPlan: true,
                type: [0, 1],
            },
            {
                name: "COMMON_ADD_MISSING_TIMECLOCK",
                title: "Missing timeclock",
                ref: "home.missingTimelock.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/Missing-Timelock.png",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                isNotTargetPlan: true,
                type: [0, 1],
            },
            {
                name: "COMMON_ADD_OVERTIME",
                title: "Overtime",
                ref: "home.overtimeApplication.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/OverTime.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                isNotTargetPlan: true,
                type: [0, 1],
            },
            {
                name: "SHIFT_PLAN",
                title: "Shift Plan",
                url: '',
                icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isNotTargetPlan: true,
                isHQ: true,
                childLV2: [
                    {
                        name: "ADD_TARGET_PLAN",
                        title: "Target plan",
                        ref: "home.targetPlan.allRequests",
                        url: '',
                        icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
                        addToMenu: true,
                        gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                        roles: [1, 2, 4, 8, 16, 32],
                        type: [0, 1],
                        isNotTargetPlan: true,
                        isHQ: true
                    },
                    {
                        name: "SHIFT_PLAN",
                        title: "Shift Plan",
                        ref: "home.targetPlan.shiftPlan",
                        url: '',
                        icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
                        addToMenu: true,
                        gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                        roles: [1, 2, 4, 8, 16, 32],
                        type: [0, 1],
                        isNotTargetPlan: true,
                        isHQ: true
                    },
                    {
                        name: "REPORT_TARGET_PLAN",
                        title: "Target Plan Report",
                        ref: "home.targetPlan.reports",
                        url: '',
                        icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
                        addToMenu: true,
                        gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                        roles: [1, 2, 4, 8, 16, 32],
                        type: [0, 1],
                        isNotTargetPlan: true,
                        isHQ: true
                    }]
            },
            {
                name: "COMMON_ADD_SHIFT_EXHCNAGE",
                title: "Shift exchange",
                ref: "home.shiftExchange.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/Shift-Exchange.jpg",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isNotTargetPlan: true,
                isHQ: true
            },
            {
                name: "COMMON_ADD_RESIGNATION",
                title: "Resignation",
                ref: "home.resignationApplication.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/Resignation.jpg",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            }
        ]
    }


    $scope.sectionMenuRecruitment = {
        childMenus: [
            {
                name: "COMMON_ADD_REQUEST_TO_HIRE",
                title: "Request to hire",
                ref: "home.requestToHire.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/request-to-hire.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            },
            {
                name: "COMMON_POSITION",
                title: "Position",
                ref: "home.position.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/request-to-hire.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            },
            {
                name: "COMMON_ADD_APPLICANT",
                title: "Applicant",
                ref: "home.applicant.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/Application.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 4, 32],
                type: [0, 1],
            },
            {
                name: "NEW_STAFF_MENU",
                title: "New Staff Onboard",
                ref: "home.newStaffOnboard",
                url: '',
                icon: "ClientApp/assets/images/icon/Application.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 4, 32],
                type: [0, 1],
            },
            {
                name: "COMMON_ADD_PROMOTE_TRANSFER",
                title: "Promote & transfer",
                ref: "home.promoteAndTransfer.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/promote.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            },
            {
                name: "COMMON_ADD_ACTING",
                title: "Acting",
                ref: "home.action.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/Acting.jpg",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            }
        ]
    }
    $scope.sectionMenuHRSystem = {
        childMenus: [
            {
                title: "User",
                ref: "home.user-setting.user-list"
            },
            {
                title: "Department",
                ref: "home.departments"
            },
            {
                title: "Applicant Status List",
                ref: "home.settingRecruitmentApplicantStatus"
            },
            {
                title: "Appreciation List",
                ref: "home.settingRecruitmentAppreciationList"
            },
            {
                title: "Headcount",
                ref: "home.budgets"
            },
            {
                title: "Cost Center",
                ref: "home.settingRecruitmentCostCenter"
            },
            {
                title: "Item List",
                ref: "home.settingRecruitmenItemList"
            },
            {
                title: "Position List",
                ref: "home.settingRecruitmentPosition"
            },
            {
                title: "Working Address",
                ref: "home.settingRecruitmentWorkingAddress"
            },
            {
                title: "Working Time",
                ref: "home.settingRecruitmentWorkingTime"
            },
            {
                title: "Categories",
                ref: "home.settingRecruitmentCategories"
            },
            {
                title: "Missing Timeclock Reasons",
                ref: "home.missingTimeclockReasons"
            },
            {
                title: "Overtime Reasons",
                ref: "home.overtimeReasons"
            },
            {
                title: "Shift Exchange Reasons",
                ref: "home.shiftExchangeReasons"
            },
            {
                title: "Resignation Reasons",
                ref: "home.resignationReasons"
            },
            {
                title: "Job Grade",
                ref: "home.jobGrades"
            },
            {
                title: "Reason",
                ref: "home.reason"
            },
            {
                title: "Holiday Schedule",
                ref: "home.holidaySchedule"
            },
            {
                title: "Reference Number",
                ref: "home.referenceNumbers"
            },
            {
                title: "Target Plan Special",
                ref: "home.targetPlanSpecial"
            },
            {
                title: "Hotel",
                ref: "home.hotel"
            },
            {
                title: "Business Trip Location",
                ref: "home.location"
            },
            {
                title: "Budget Limit: HQ & Store",
                ref: "home.btaPolicy"
            },
            {
                title: "Room Type",
                ref: "home.roomType"
            },
            {
                title: "Flight Number",
                ref: "home.flightNumber"
            },
            {
                title: "Airline",
                ref: "home.airline"
            },
            {
                title: "Business Model Unit Mapping",
                ref: "home.businessModelUnitMapping"
            },
            {
                title: "Business Model",
                ref: "home.businessModel"
            },
            {
                title: "Budget Limit: Special Case",
                ref: "home.btaPolicySpecial"
            },
            {
                title: "Partition",
                ref: "home.partition"
            },
            {
                title: "Shift Code",
                ref: "home.shiftCode"
            },
            {
                title: "Workflow",
                ref: "home.workflowszzyyxx"
            },
            {
                title: "Tracking Log",
                ref: "home.trackingLogs"
            },
            {
                title: "Shift Plan Submit Person",
                ref:"home.shiftPlanSubmitPerson"
            },
            {
                title: "Days Configuration",
                ref:"home.daysConfiguration"
            },


        ]
    }
    $scope.sectionMenuAdmin = {
        childMenus: [
            {
                name: "HANDOVER_MENU",
                title: "Hanover",
                ref: "home.handover.allHandover",
                url: '',
                icon: "ClientApp/assets/images/icon/handover.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 32],
                type: [0, 1],
            },
            {
                name: "BUSINESS_TRIP_APPLICATION",
                title: "Business Trip Application",
                ref: "home.business-trip-application.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/business_trip.png",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32, 64],
                type: [0, 1],
            },
            {
                name: "BUSINESS_TRIP_APPLICATION_REPORT",
                title: "Business Trip Application Report",
                ref: "home.business-trip-application-report",
                url: '',
                icon: "ClientApp/assets/images/icon/business_trip.png",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32, 64],
                type: [0, 1],
            },
            {
                name: "BTA_OVER_BUDGET",
                title: "Over Budget",
                ref: "home.over-budget.allRequests",
                url: '',
                icon: "ClientApp/assets/images/icon/over_budget.png",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32, 64],
                type: [0, 1],
            }
        ]
    }
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

    $scope.switchLayout = function () {
        window.location.href = "/HRV1";
    }

    $scope.chatbotConfig = true;

    function getAddSectionMenu() {
        let subMenuSections = _.filter(appSetting.subSections, x => {
            let indexRole = _.findIndex(x.roles, y => {
                return (y & $rootScope.currentUser.role) == y;
            });
            let type = _.findIndex(x.type, y => {
                return (y & $rootScope.currentUser.type) == y;
            });
            if (x.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                return !$rootScope.currentUser.isNotTargetPlan;
            }
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
                if (x.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                    return !$rootScope.currentUser.isNotTargetPlan;
                }
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
            else if (element.title == 'COMMON_ADMIN') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_ADMIN';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'COMMON_ADMIN', title: $translate.instant('COMMON_ADMIN'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_ADMIN';
                });
                current.values = current.values.concat(tempMenus);
            }
            else if (element.title == 'COMMON_TRADE_CONTRACT') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'COMMON_TRADE_CONTRACT';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({
                        label: 'COMMON_TRADE_CONTRACT', title: $translate.instant('COMMON_TRADE_CONTRACT'), url: "http://edoc_l_trade.aeon.com.vn/", isPhrase1: true, values: []
                    });
                }
            }
            /*else if (element.title == 'SUPPLIER_MANAGEMENT') {
                let existItem = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'SUPPLIER_MANAGEMENT';
                });
                if (!existItem) {
                    $rootScope.addSectionMenu.push({ label: 'SUPPLIER_MANAGEMENT', title: $translate.instant('SUPPLIER_MANAGEMENT'), values: [] });
                }
                let current = _.find($rootScope.addSectionMenu, x => {
                    return x.label == 'SUPPLIER_MANAGEMENT';
                });
                current.values = current.values.concat(tempMenus);
            }*/
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
                if (x.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                    return !$rootScope.currentUser.isNotTargetPlan;
                }
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
                            if (x.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                                return !$rootScope.currentUser.isNotTargetPlan;
                            }
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
            if (x.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                return !$rootScope.currentUser.isNotTargetPlan;
            }
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
                    if (x.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                        return !$rootScope.currentUser.isNotTargetPlan;
                    }
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
                if (x.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                    return !$rootScope.currentUser.isNotTargetPlan;
                }
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
        if ($rootScope.menus) {
            $rootScope.settingSidebar = updateChildMenus($rootScope.menus.find(item => item.index === 7).childMenus);
            if ($scope.currentUser.deptCode != $scope.deptAcademyCode) {
                $rootScope.settingSidebar = $rootScope.settingSidebar.filter(item => item.index != 44); //index 44 is academy
            }

            //Breadcrumb in header
            $rootScope.nameBreadcrumb = $rootScope.menus.find(item => item.urlV2 === current)?.name;
            const dsBreadcrumb = $rootScope.menus.filter(item => item.index !== 7);
            if (current == 'home.dashboard') {
                $rootScope.nameBreadcrumb = 'To-do List';
            }
            if (!$rootScope.nameBreadcrumb) {
                for (const item of dsBreadcrumb) {
                    const subMenu = item.childMenus?.find(child => child.urlV2 === current);
                    const subM = item.ref?.find(child => child.state === current);

                    if (subMenu || subM) {
                        $rootScope.nameBreadcrumb = subMenu?.name || subM?.name || item.name;
                        break;
                    }

                    for (const childItem of item.childMenus || []) {
                        const matchInAction = childItem.actions?.find(action => action.urlV2 === current);
                        const matchInRef = childItem.ref?.find(ref => ref.state === current);

                        const matchInChildren = (childItem.actions || []).find(action =>
                            action.children?.some(child => child.state === current)
                        );

                        if (matchInChildren) {
                            $rootScope.nameBreadcrumb = matchInChildren.name || childItem.name;
                            break;
                        }

                        if (matchInAction || matchInRef) {
                            $rootScope.nameBreadcrumb = matchInAction?.name || childItem.name;
                            break;
                        }
                    }

                    if ($rootScope.nameBreadcrumb) break;
                }

                if (!$rootScope.nameBreadcrumb) {
                    const settingMenu = $rootScope.settingSidebar.find(item => item.urlV2 === current);
                    if (settingMenu) {
                        $rootScope.nameBreadcrumb = settingMenu.name;
                    } else {
                        let found = false;
                        for (const item of $rootScope.settingSidebar) {
                            const subMenu = item.actions?.find(child => child.urlV2 === current);
                            if (subMenu) {
                                $rootScope.nameBreadcrumb = subMenu.name;
                                found = true;
                                break;
                            }
                            for (const childItem of item.actions || []) {
                                const subMenuLv2 = childItem.children?.find(action => action.urlV2 === current);
                                if (subMenuLv2) {
                                    $rootScope.nameBreadcrumb = subMenuLv2.name;
                                    found = true;
                                    break;
                                }
                                const refSubMenuLv2 = childItem.ref?.find(action => action.state === current);
                                if (refSubMenuLv2) {
                                    $rootScope.nameBreadcrumb = childItem.name;
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;
                        }
                    }
                }
            }
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
                    if (x.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                        return !$rootScope.currentUser.isNotTargetPlan;
                    }
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
                                if (child.isNotTargetPlan && $rootScope.currentUser.isNotTargetPlan) {
                                    return false;
                                }
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
            if ($rootScope.undefinedUser) {
                getAddSectionMenu();
            } else if (isContinue) {
                localStorageService.set('invalidPermission', false);
                getAddSectionMenu();
                //createPageComponent(current);
            } else {
                $timeout(function () {
                    $rootScope.errorMessages = '';
                    if ($rootScope.currentUser && $rootScope.currentUser.userNotExistEdoc2) {
                        $rootScope.errorMessages = '';
                        $state.go('home.navigation-home');
                    } else {
                        $rootScope.permissionErrorMessage = '';
                        if (!$rootScope.currentUser.jobGradeValue) {
                            $rootScope.permissionErrorMessage = 'This user has not been assigned to a department yet/ Bạn chưa được phân bổ vào phòng ban';
                        } else {
                            $rootScope.permissionErrorMessage = 'You has not been  granted permission to access to the system/ Bạn chưa được cấp quyền truy cập vào hệ thống';
                        }
                        $state.go('notFoundPage');
                    }
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
            /*var dashboardItem = _.find(appSetting.menu, x => {
                return x.ref === 'home.dashboard'
            });
            dashboardItem.selected = true;
            $rootScope.title = dashboardItem.title.toUpperCase();
            $rootScope.childMenus = dashboardItem.childMenus;
            $rootScope.isParentMenu = dashboardItem.isParentMenu;
            updateSubMenu(dashboardItem.ref);*/
            $history.resetAll();
            $state.go('home.dashboard');
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

        $rootScope.showConfirmWarning = function (title, content, actionName, isTrushAsHTML) {
            ssg.dialogConfirmParam.data = null;
            ssg.dialog = $("#dialogWarning").kendoDialog({
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
            $scope.isTrushAsHTML = isTrushAsHTML;
            if ($scope.isTrushAsHTML)
                $scope.content = $sce.trustAsHtml(content);

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

        $rootScope.visibleTrackingHistory = function ($translate, width) {
            let dialogReason = $("#trackingHistoryDialog").kendoDialog({
                title: $translate.instant('COMMON_TRACKING_HISTORY'),
                width: width,
                modal: true,
                visible: true,
                animation: {
                    open: {
                        effects: "fade:in"
                    }
                },
                close: function (e) {
                    /*e.data = ssg.dialogConfirmParam.data;*/
                    /*e.preventDefault();*/
                }
            });
            let boxReason = dialogReason.data("kendoDialog");
            boxReason.open();
            $rootScope.confirmProcessing = boxReason;
            return dialogReason;
        }

        $rootScope.visibleLogHRAmin = function ($translate, width) {
            let dialogReason = $("#LogHRAdminDialog").kendoDialog({
                title: $translate.instant('COMMON_HR_ADMIN_HISTORY'),
                width: width,
                modal: true,
                visible: true,
                animation: {
                    open: {
                        effects: "fade:in"
                    }
                },
                close: function (e) {
                    /*e.data = ssg.dialogConfirmParam.data;*/
                    /*e.preventDefault();*/
                }
            });
            let boxReason = dialogReason.data("kendoDialog");
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

        $('.k-grid-header').addClass('table table-borderless table-rounded display-10 leading-18');
        //$('.k-grid-header').css('padding-right', '');
        $('.k-widget.k-window.k-dialog').addClass('rounded-4');

        //$('.k-picker-wrap').css('height', '100%');
        //$('.k-picker-wrap').on('focus', '*', function () {
        //    $(this).css('box-shadow', '0 0 0 5rem #0072c6');
        //}).on('blur', '*', function () {
        //    $(this).css('box-shadow', 'none');
        //});

        //$('.k-numeric-wrap').css({
        //    'border-color': '#e4e7ea',
        //    'padding': '2px'
        //});

        initScript(); // init file script.js
        if ($rootScope.currentUser) {
            getListNotification($rootScope.currentUser.loginName);
            initChatBotScript($rootScope.currentUser.loginName); // init chatbot
            // connectSignalR($rootScope.currentUser.loginName);
            await $scope.getConfigChatBot();
        }
        // Start the connection
        //startConnection().catch(console.error);
        showSidebar();
    }

    $scope.countBell = 0;
    let prefixAPIIntegr = baseUrlApi != null ? baseUrlApi.replace('/api', '') : "";
    //prefixAPIIntegr = "https://hrapi50.aeon.com.vn"
    function connectSignalR(loginName) {
        // $.connection.hub.url = baseUrlApi.replace('/api', '') + "/EdocIntegr/notificationHub";
        $.connection.hub.url = prefixAPIIntegr + "/EdocIntegr/notificationHub";
        $.connection.hub.qs = { "loginName": loginName };
        var chat = $.connection.notificationHub;

        chat.client.notificationBell = function () {
            $timeout(function () {
                Notification.success($translate.instant("HAVE_A_NEW_NOTIFICATION"));
                getListNotification($rootScope.currentUser.loginName);
            }, 0);
        };

        $.connection.hub.start().done(function () {
            console.log('Connectd signal Successfully: ' + moment(new Date()).format('HH:mm'));
            /*setTimeout(function () {
                // $.connection.hub.disconnected();
				$.connection.hub.stop();
            }, 5000);*/
        }).fail(function (err) {
            console.log('Error: ' + err.toString());
        });

        $.connection.hub.disconnected(function () {
            console.log("SignalR disconnected. Attempting to reconnect...");
            setTimeout(function () {
                $.connection.hub.start();
            }, 5000);
        });
    }

    $scope.showNotification = function () {
        if ($rootScope.currentUser) {
            getListNotification($rootScope.currentUser.loginName);
        }
    }

    $scope.changeDarkMode = function () {
        window.initDarkMode();
    }

    $scope.readAllNotification = function () {
        if ($rootScope.currentUser) {
            readAllNotification($rootScope.currentUser.loginName);
        }
    }

    $scope.deleteAllNotification = function () {
        if ($rootScope.currentUser) {
            deleteAllNotification($rootScope.currentUser.loginName);
        }
    }

    $scope.readNotification = function (item) {
        if (item && !item.IsRead) {
            readNotification(item.ID);
        }
    }

    $scope.goToDetailItem = function (item) {
        if (item) {
            readNotification(item.ID);
            $window.open(item.Url, '_blank');
        }
    }

    $scope.saveConfigChatbot = async function (value) {
        const requestData = {
            UserID: $rootScope.currentUser.id,
            Type: "chatbot",
            IsHide: value,
            Module: "hr"
        };
        $http
            .post(baseUrlApi.replace('/api', '') + `/home-api/SaveConfigChatBot`, requestData)
            .then(async function (response) {
                if (response.status = 200) {
                    await $scope.getConfigChatBot();
                }
            })
            .catch(function (error) {
                console.error("Error fetching suggestions:", error);
            });
    }

    $scope.getConfigChatBot = async function () {
        var requestDataGet = {
            UserID: $rootScope.currentUser.id,
            Type: "chatbot",
            Module: "hr"
        };

        $http
            .post(baseUrlApi.replace('/api', '') + `/home-api/GetConfigChatBot`, requestDataGet)
            .then(function (responseGet) {
                if (responseGet.status = 200)
                {
                    $scope.chatbotConfig = responseGet.data.data.IsHide ?? false;
                }
            })
            .catch(function (error) {
                console.error("Error fetching suggestions:", error);
            });
    }

    function readNotification(id) {
        try {
            $.ajax({
                type: "GET",
                url: prefixAPIIntegr + "/EdocIntegr/api/ReadNotification?notificationId=" + id,
                contentType: "application/json",
                dataType: "json",
                timeout: 50 * 1000,
                success: function (response) {
                    getListNotification($rootScope.currentUser.loginName);
                    //Notification.success("Read notification successfully!")
                },
            });
        } catch (error) {
            console.error("An error occurred during the AJAX request:", error);
        }
    }

    function readAllNotification(loginName) {
        try {
            $.ajax({
                type: "GET",
                url: prefixAPIIntegr + "/EdocIntegr/api/ReadAllNotification?loginName=" + loginName,
                contentType: "application/json",
                dataType: "json",
                timeout: 50 * 1000,
                success: function (response) {
                    getListNotification($rootScope.currentUser.loginName);
                    Notification.success("Read all notification successfully!")
                },
            });
        } catch (error) {
            console.error("An error occurred during the AJAX request:", error);
        }
    }

    function deleteAllNotification(loginName) {
        try {
            $.ajax({
                type: "DELETE",
                url: prefixAPIIntegr + "/EdocIntegr/api/DeleteAllNotification?loginName=" + loginName,
                contentType: "application/json",
                dataType: "json",
                timeout: 50 * 1000,
                success: function (response) {
                    getListNotification($rootScope.currentUser.loginName);
                    Notification.success("Delete all notification successfully!")
                },
            });
        } catch (error) {
            console.error("An error occurred during the AJAX request:", error);
        }
    }

    $rootScope.notificationBe = 0;
    async function getListNotification(loginName) {
        try {
            await $.ajax({
                type: "GET",
                url: prefixAPIIntegr + "/EdocIntegr/api/GetNotificationList?loginName=" + loginName +"&pageSize=1&limit=5&isASCCreated=false",
                contentType: "application/json",
                dataType: "json",
                timeout: 50 * 1000,
                success: function (response) {
                    var responseData = response.Object;
                    $rootScope.notificationBe = responseData.CountUnRead;
                    $scope.notificatioList = responseData.Data.map(x => {
                        x.TimeAgo = timeAgo(x.Created);
                        return x;
                    });
                    $scope.$apply();
                },
            });
        } catch (error) {
            console.error("An error occurred during the AJAX request:", error);
        }
    }

    function timeAgo(time) {
        const now = new Date();
        const past = new Date(time);
        const seconds = Math.floor((now - past) / 1000);

        if (seconds < 60) return `${seconds} seconds ago`;

        const minutes = Math.floor(seconds / 60);
        if (minutes < 60) return `${minutes} minutes ago`;

        const hours = Math.floor(minutes / 60);
        if (hours < 24) return `${hours} hours ago`;

        const days = Math.floor(hours / 24);
        if (days < 30) return `${days} days ago`;

        const months = Math.floor(days / 30.44);  // Approximate average days per month
        if (months < 12) return `${months} months ago`;

        const years = Math.floor(days / 365.25);  // Account for leap years
        return `${years} years ago`;
    }

    function initChatBotScript(loginName) {
        try {
            $.ajax({
                type: "GET",
                url: baseUrlApi.replace('/api','') + "/EdocIntegr/api/GetSessionChatbot?loginName=" + loginName,
                contentType: "application/json",
                dataType: "json",
                timeout: 50 * 1000,
                success: function (response) {
                    var responseData = response.Object;
                    console.log("responseData:", responseData);
                    ChatAI.init({
                        targetElementId: 'app',
                        clientKey: '507dbd8f-13b7-45d3-8307-d46149a07516',
                        clientToken: responseData.CurrentToken,
                        token: responseData.SWOToken,
                        sessionId: responseData.SessionId
                    });
                },
            });
        } catch (error) {
            console.error("An error occurred during the AJAX request:", error);
        }
    }


    function initScript() {
        let sidebar = document.getElementById("bst-sidebar");
        if (!sidebar) {
            return;
        }
        sidebar.classList.add("show");

        sidebar = document.querySelector("#bst-sidebar .group-board");
        const sidebarCollapse = document.querySelector(
            "#bst-sidebar-collapse .group-board"
        );

        if (!sidebar || !sidebarCollapse) {
            return;
        }

        sidebar.addEventListener("mouseover", function () {
            setTimeout(() => {
                if (!this.classList.contains("overflow-auto")) {
                    this.classList.add("overflow-auto");
                }
            }, 300); // 0.3 seconds delay
        });
        sidebar.addEventListener("mouseout", function () {
            setTimeout(() => {
                if (this.classList.contains("overflow-auto")) {
                    this.classList.remove("overflow-auto");
                }
            }, 300); // 0.3 seconds delay
        });

        sidebarCollapse.addEventListener("mouseover", function () {
            setTimeout(() => {
                if (!this.classList.contains("overflow-auto")) {
                    this.classList.add("overflow-auto");
                }
            }, 300); // 0.3 seconds delay
        });
        sidebarCollapse.addEventListener("mouseout", function () {
            setTimeout(() => {
                if (this.classList.contains("overflow-auto")) {
                    this.classList.remove("overflow-auto");
                }
            }, 300); // 0.3 seconds delay
        });
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

    async function getImageUser() {
        if ($rootScope.currentUser != null && $rootScope.currentUser.id) {
            var res = await settingService.getInstance().users.getImageUserById({ userId: $rootScope.currentUser.id }).$promise;
            if (res.isSuccess) {
                if (res.object.data.profilePicture) {
                    $timeout(function () {
                        $scope.avatar = baseUrlApi.replace('/api', '') + res.object.data.profilePicture.trim();;
                        $rootScope.avatar = $scope.avatar;
                    }, 0);

                } else {
                    $timeout(function () {
                        $scope.avatar = $scope.avatarDefault;
                        $rootScope.avatar = $scope.avatar;
                    }, 0);
                }
            }
        }
    }

    $scope.signOut = function () {
        //window.location.href = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/SignOut.aspx";
        sessionStorage.removeItem('currentUser');
        sessionStorage.removeItem('departments');
        $timeout(function () {
            localStorageService.remove('passTime'); // Thời gian đã trôi qua từ lúc đăng nhập vào hệ thống
            localStorageService.remove('accessSystemTime'); // Thời điểm truy cập vào hệ thống
            localStorageService.remove('isShowTimeOutPopup'); // Dùng để kiếm tra các popup timeout ở session khác còn đang mở
            sessionStorage.removeItem('inVisibleConfirmPopup'); // Popup time out trên session hiện tại có đang mở
            localStorageService.remove('waitingForLogout');
            setTimeout(function () {
                window.location.href = "/SSO/logout.aspx";
            }, 500);
        }, 0);
    }

    $scope.switchLayout = function () {
        window.location.href = "/HR";
    }

    function findMenuBar(current) {
        // Vì lúc đầu load thì cái currentUser undefined, chạy lần 2 mói có, tránh lỗi.
        if ($rootScope.currentUser) {
            getImageUser();
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
        if ($rootScope.menus) {

            $rootScope.settingSidebar = updateChildMenus($rootScope.menus.find(item => item.index === 7).childMenus);
            if ($scope.currentUser.deptCode != $scope.deptAcademyCode) {
                $rootScope.settingSidebar = $rootScope.settingSidebar.filter(item => item.index != 44); //index 44 is academy
            }

            //Breadcrumb in header
            $rootScope.nameBreadcrumb = $rootScope.menus.find(item => item.urlV2 === current )?.name;
            const dsBreadcrumb = $rootScope.menus.filter(item => item.index !== 7);
            if (current == 'home.dashboard') {
                $rootScope.nameBreadcrumb = 'To-do List';
            }
            if (!$rootScope.nameBreadcrumb) {
                for (const item of dsBreadcrumb) {
                    const subMenu = item.childMenus?.find(child => child.urlV2 === current);
                    const subM = item.ref?.find(child => child.state === current);

                    if (subMenu || subM) {
                        $rootScope.nameBreadcrumb = subMenu?.name || subM?.name || item.name;
                        break;
                    }

                    for (const childItem of item.childMenus || []) {
                        const matchInAction = childItem.actions?.find(action => action.urlV2 === current);
                        const matchInRef = childItem.ref?.find(ref => ref.state === current);

                        const matchInChildren = (childItem.actions || []).find(action =>
                            action.children?.some(child => child.state === current)
                        );

                        if (matchInChildren) {
                            $rootScope.nameBreadcrumb = matchInChildren.name || childItem.name;
                            break;
                        }

                        if (matchInAction || matchInRef) {
                            $rootScope.nameBreadcrumb = matchInAction?.name || childItem.name;
                            break;
                        }
                    }

                    if ($rootScope.nameBreadcrumb) break;
                }

                if (!$rootScope.nameBreadcrumb) {
                    const settingMenu = $rootScope.settingSidebar.find(item => item.urlV2 === current);
                    if (settingMenu) {
                        $rootScope.nameBreadcrumb = settingMenu.name;
                    } else {
                        let found = false;
                        for (const item of $rootScope.settingSidebar) {
                            const subMenu = item.actions?.find(child => child.urlV2 === current);
                            if (subMenu) {
                                $rootScope.nameBreadcrumb = subMenu.name;
                                found = true;
                                break;
                            }
                            for (const childItem of item.actions || []) {
                                const subMenuLv2 = childItem.children?.find(action => action.urlV2 === current);
                                if (subMenuLv2) {
                                    $rootScope.nameBreadcrumb = subMenuLv2.name;
                                    found = true;
                                    break;
                                }
                                const refSubMenuLv2 = childItem.ref?.find(action => action.state === current);
                                if (refSubMenuLv2) {
                                    $rootScope.nameBreadcrumb = childItem.name;
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;
                        }
                    }
                }
            }
        }

    }

    $rootScope.currentUserModulePermissions = function (modulePermission) {
        let permissions = [];
        for (var i = 0; i < appSetting.permission.length; i++) {
            if ((appSetting.permission[i].code & modulePermission) > 0) {
                permissions.push(appSetting.permission[i].code);
            }
        }
        return permissions;
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

    $rootScope.viewFileOnline = async function (id) {
        let filePath = await $scope.getFilePath(id);
        if ($.type(filePath) == "string" && filePath.length > 0) {
            callHandler({
                    Action: "viewFileOnWeb",
                    filePath: filePath,
                    itemID: id
                },
                function (resultData) {
                    if (!isNullOrUndefined(resultData) && resultData.data.length > 0) {
                        let attachmentViewDialog = null;
                        if (!$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))) {
                            attachmentViewDialog = $("#attachmentViewDialog").data("kendoDialog");
                        }
                        else {
                            attachmentViewDialog = $("#attachmentViewDialog").kendoDialog({
                                width: "70%",
                                height: "100%",
                                close: function (e) {
                                    $("#attachmentViewDialog").hide();
                                }
                            }).data("kendoDialog");
                        }
                        $("#attachmentViewDialog").show();
                        $("#attachment_owa")[0].setAttribute("src", resultData.data);
                        attachmentViewDialog.open();

                    }
                    else {
                        Notification.error($translate.instant('NOT_SUPPORT_VIEW_ONLINE'));
                        $scope.$apply();
                    }

                }
            );
        }
    }

    $rootScope.showViewLogDetaiLog = function (title, data) {
        ssg.dialogConfirmParam.data = null;
        ssg.dialog = $("#viewHistoryDialog").kendoDialog({
            title: $translate.instant('COMMON_TRACKING_HISTORY'),
            width: "1850px",
            height: "500px",
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

        data.forEach(x => {
            x.created = moment(x.created).format(appSetting.longDateFormat);
            /* Type: UpdatePayload */
            if (x.type == appSetting.TrackingType.UpdatePayload) {
                if (x.dataStr) {
                    var dataStr = JSON.parse(x.dataStr);
                    x.documents = dataStr.Documents;
                    x.payload = dataStr.Payload;
                }
            }
        });
        $scope.histories = data;
        ssg.confirmDialog = ssg.dialog.data("kendoDialog");
        ssg.confirmDialog.open();
        $rootScope.confirmDialog = ssg.confirmDialog;
        return ssg.confirmDialog;
    }

    $rootScope.openWindow = function (url) {
        $window.open(url, '_blank');
    }

    $scope.openWindowPortal = function () {
        var href = "https://hr.aeon.com.vn";
        $rootScope.openWindow(href);
    }

    $rootScope.blockAccess = function () {

        if ($rootScope.currentUser.jobGradeValue == 1) {
            $timeout(function () {
                $state.go('accessDeniedPage');
            }, 0);
        }
    }

    $rootScope.showConfirmWarningHRPortal = function (title, content, actionName) {
        ssg.dialogConfirmParam.data = null;
        ssg.dialog = $("#dialogWarningHRPortal").kendoDialog({
            title: title,
            width: "500px",
            modal: true,
            visible: false,
            animation: {
                open: {
                    effects: "fade:in"
                }
            },
            close: function (e) { }
        });
        $scope.content = content;
        $scope.actionName = actionName;
        ssg.confirmDialog = ssg.dialog.data("kendoDialog");
        ssg.confirmDialog.open();
        $rootScope.confirmDialog = ssg.confirmDialog;
        return ssg.confirmDialog;
    }

    $scope.gotoNotificationList = function () {
        let url = $window.location.origin + "/home/Notification/AllNotification";
        $window.open(url, '_blank');
    }

    $scope.pwdModel = { errors: [] };
    $scope.changePwdDialogOpts = {
        width: "450px",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                $scope.pwdModel.id = $rootScope.currentUser.id;
                $scope.pwdModel.errors = [];
                if (!$scope.pwdModel.oldPassword) {
                    //$scope.pwdModel.errors.push("Current password is required")
                    $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_CURRENT_VALIDATE'))
                }
                if (!$scope.pwdModel.newPassword) {
                    // $scope.pwdModel.errors.push("New password is required")
                    $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_NEW_PASSWORD_VALIDATE'))
                }
                if (!$scope.pwdModel.verifyPassword) {
                    //$scope.pwdModel.errors.push("Verify password is required")
                    $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_VERIFY_PASSWORD_VALIDATE'))
                }
                if ($scope.pwdModel.newPassword != $scope.pwdModel.verifyPassword) {
                    //$scope.pwdModel.errors.push("New password and verify password is not the same")
                    $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_NEW_VERIFY_VALIDATE'))
                }
                if ($scope.pwdModel.newPassword) {
                    /*var matchedItems = $scope.pwdModel.newPassword.match(/(.{7,})/g);
                    if (!matchedItems) {
                        $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_CHARACTER_VALIDATE'))
                    }*/
                    var newPassword = $scope.pwdModel.newPassword;

                }
                if ($scope.pwdModel.errors.length == 0) {
                    settingService.getInstance().users.changePassword($scope.pwdModel).$promise.then(function (result) {
                        if (result.object) {
                            Notification.success("Your password was changed successfully");
                            $scope.changePwdDialog.close();
                        } else {
                            //$scope.pwdModel.errors.push("Your current password is not correct. Please try again")
                            if (result.messages[0] != null) {
                                $scope.pwdModel.errors.push($translate.instant(result.messages[0]));
                            } else {
                                $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_CURRENT_CORRECT_VALIDATE'))
                            }
                        }
                    });
                    $scope.$apply();
                    return false;
                } else {
                    $scope.$apply();
                    return false;
                }
            },
            primary: true
        }],
        close: async function (e) {
            $scope.pwdModel = { errors: [] }
        }
    };

    $scope.changePassword = function () {
        $scope.changePwdDialog.title($translate.instant('USER_PROFILE_CHANGE_PASS'));
        $scope.changePwdDialog.open();
        $rootScope.confirmDialogChangePassWord = $scope.changePwdDialog;
    }

    $scope.getSuggestions = async function (searchText) {
        if (!searchText) {
            $scope.suggestions = [];
            return;
        }
        const requestData = {
            Query: searchText,
            Modules: ["hredoc2"],
            Size: 5,
        };
        $http
            .post(homeUpgradeUrlAPI +`/SmartSearchAPI/api/Search/realtime-suggest`, requestData)
            .then(function (response) {
                const seen = new Set();
                $scope.suggestions = response.data;
            })
            .catch(function (error) {
                console.error("Error fetching suggestions:", error);
            });
    };

    $scope.selectSuggestion = function (suggestionText) {
        $scope.searchQuery = suggestionText;
        $scope.search(suggestionText);
    };
    $scope.search = async function (query) {
        const lowerCaseKeyword = query.toLowerCase();
        const redirectUrl = redirectMap[lowerCaseKeyword];
        if (redirectUrl) {
            window.open(redirectUrl, "_blank");
        }
        else {
            window.location.href = homeUpgradeUrlAPI + `/home/Search/AdvanceSearch?query=${query}&&module=hredoc2`;
        }
    };
    $scope.changeHome = function () {
        if ($rootScope.currentUser) {
            // 0 AD, 1 MS
            //window.location.href = $rootScope.currentUser.type == 0 ? "/Home" : "/Home/HR";
            var domain = window.location.origin;
            var redirectUrl = $rootScope.currentUser.type == 0 ? "/Home" : "/Home/HR";
            var fullUrl = domain + redirectUrl;

            // Mở tab mới và lưu đối tượng của window đó
            var newWindow = window.open(fullUrl, "_blank");

            // Sau 2 giây, chuyển hướng trong tab hiện tại và đóng tab mới
            setTimeout(function () {
                window.location.href = fullUrl; // Chuyển hướng trong tab hiện tại
                newWindow.close(); // Đóng window đã mở
            }, 100);
        }
    }
    function showSidebar() {
        const isHidden = localStorage.getItem('Hide_Show_Menu') === 'true';
        if (isHidden) {
            document.querySelector("#bst-header").classList.add('sync-collapse');
            document.querySelector("#bst-sidebar").classList.add('sync-collapse');
            document.querySelector("#bst-sidebar-shadow").classList.add('sync-collapse');
            document.querySelector("#bst-sidebar-collapse").classList.add('sync-collapse');
            document.querySelector(".bst-main").classList.add('sync-collapse');
            document.querySelector("#bst-sidebar").classList.remove('show');
            document.querySelector("#bst-sidebar-shadow").classList.add('show');
            document.querySelector("#bst-sidebar-collapse").classList.add('show');
        } else {
            document.querySelector("#bst-header").classList.remove('sync-collapse');
            document.querySelector("#bst-sidebar").classList.remove('sync-collapse');
            document.querySelector("#bst-sidebar-shadow").classList.remove('sync-collapse');
            document.querySelector("#bst-sidebar-collapse").classList.remove('sync-collapse');
            document.querySelector(".bst-main").classList.remove('sync-collapse');

            document.querySelector("#bst-sidebar").classList.add('show');
            document.querySelector("#bst-sidebar-shadow").classList.remove('show');
            document.querySelector("#bst-sidebar-collapse").classList.remove('show');
        }
    }
    const redirectMap = {
        "//re": subDomainEdoc1 + `/#/new-reimbursement`,
        "//ca": subDomainEdoc1 + `/#/new-advance`,
        "//rf": subDomainEdoc1 + `/#/new-refund-card`,
        "//tf": subDomainEdoc1 + `/#/new-transfer-cash`,
        "//pr": subDomainEdoc1 + `/#/new-payment`,
        "//rp": subDomainEdoc1 + `/#/new-reimbursement-payment`,
        "//cn": subDomainEdoc1 + `/#/new-credit-note`,

        "//f2": subDomainEdoc1 + `/#/new-purchase`,
        "//f2m": subDomainEdoc1 + `/#/new-purchase-multi-budget`,
        "//f3": subDomainEdoc1 + `/#/new-contract-multiF2`,
        "//f3m": subDomainEdoc1 + `/#/new-contract-custom`,
        "//f4": subDomainEdoc1 + `/#/new-non-expense-contract`,
        //HR
        "//lea": subDomainHR + `/#!/home/leaves-management/item/?id=`,
        "//mis": subDomainHR + `/#!/home/missingTimelock/item/?id=`,
        "//ot": subDomainHR + `/#!/home/overtimeApplication/item/?id=`,
        "//tar": subDomainHR + `/#!/home/pending-target-plan/add-target-plan`,
        "//shi": subDomainHR + `/#!/home/shift-exchange/item/?id=`,
        "//res": subDomainHR + `/#!/home/resignationApplication/item/?id=`,
        "//rth": subDomainHR + `/#!/home/requestToHire/item/?id=`,
        "//pro": subDomainHR + `/#!/home/promoteAndTransfer/item/?id=`,
        "//act": subDomainHR + `/#!/home/action/item/?id=`,
        "//bta": subDomainHR + `/#!/home/business-trip-application/item/?id=`,
        //Academy
        "//atr": subDomainHR + `/#!/home/academy/new-request`,
        //SKU
        "//np": subDomainSKU + `/Product/NewProductAssessmentRequest`,
        "//ep": subDomainSKU + `/ProductUpdate/UpdateProductAssessmentRequest`,
        //Academy
        //Liquor License:
        "//rl": subDomainLiquor + `/RetailLicense/RetailLicense`,
        "//doc": subDomainLiquor + `/Document/Document`,
        "//project": subDomainLiquor + `/Project/ProJect`,
        //Trade
        "//sa": "https://edoctrade.aeon.com.vn/default.aspx#/create/SA",
        "//saf": "https://edoctrade.aeon.com.vn/default.aspx#/create/SAF",
        "//ds": "https://edoctrade.aeon.com.vn/default.aspx#/create/DS",
    };
});