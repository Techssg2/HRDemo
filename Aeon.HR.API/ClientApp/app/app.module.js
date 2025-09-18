var modules = [
    'ui.router',
    'pascalprecht.translate', 'ngCookies',
    'ssg.recruitmentModule',
    'ssg.applicantModule',
    'ssg.positionModule',
    'ssg.promoteAndTransferModule',
    'ssg.settingUserModule',
    'ssg.newStaffOnboardModule',
    'ssg.cbModule',
    'ssg.directiveItemIconModule',
    'ssg.directiveWorkflowCommentModule',
    'ssg.leaveManagementModule',
    'ssg.directiveUserModule',
    'ssg.dashboardModule',
    'ssg.directiveActionsModule',
    'ssg.directiveBTAModule',
    'ssg.directiveBTAChangeCancelModule',
    'ssg.directiveBTAFlightTicketModule',
    'kendo.directives',
    'appModule',
    'underscore',
    'excuteJQModule',
    'ssg.settingModules',
    'ssg.directiveWorkflowStatusModule',
    'ngResource',
    'moment-module',
    'constantModule',
    'attachmentfileModule',
    'ssg.requestToHireModule',
    'ssg.directiveSubModule',
    'ssg.directiveNotiModule',
    'ssg.overtimeApplicationModule',
    'ssg.shiftExchangeApplicationModule',
    'ssg.directiveSubModule',
    'ssg.missingTimelockModule',
    'ssg.historyModule',
    'ssg.jobGradeModule',
    'ssg.DepartmentModule',
    'ssg.resignationApplicationModule',
    'ssg.handoverModule',
    'ui-notification',
    'ssg.actionModule',
    'ssg.cAndBReasonModule',
    'ssg.settingWorkFlowModule',
    "ssg.budgetModule",
    "ssg.settingRecruitmentWorkingTimeModule",
    "ssg.settingRecruitmentPromoteTransferPrintModule",
    "ssg.settingRecruitmentItemListModule",
    "ssg.settingReferenceNumberModule",
    "ssg.settingRecruitmentAppreciationListModule",
    "ssg.settingRecruitmentApplicantStatusModule",
    "ssg.settingTrackingLogModule",
    "ssg.orgChartModule",
    "ssg.filterModule",
    "ssg.directiveWorkflowStatusModule",
    "ssg.settingRecruitmentPositionModule",
    "LocalStorageModule",
    "ssg.errorModule",
    "ssg.redirectPageModule",
    "ssg.directiveWorkflowStepModule",
    "ssg.directiveWorkflowTaskModule",
    "ssg.directiveStageModule",
    "ssg.directiveApplicantItemG2",
    "ssg.directiveApplicantItemG1",
    "ssg.settingRecruitmentCostCenterModule",
    "ssg.btaModule",
    "ssg.businessTripItemModule",
    "ssg.businessTripListModule",
    "ssg.businessTripReportModule",
    "ssg.airlineModule",
    "ssg.flightNumberModule",
    "ssg.hotelModule",
    "ssg.locationModule",
    "ssg.roomTypeModule",
    "ssg.btaPolicyModule",
    "ssg.btaPolicySpecialCaseModule",
    "ssg.settingDaysConfigurationLogModule",
    "ssg.settingRecruitmentWorkingAddressModule",
    "ssg.pendingTargetPlanModule",
    "ssg.settingRecruitmentCategoriesModule",
    "ssg.settingHolidayScheduleControllerModule",
    "ssg.shiftPlanSubmitPersonModule",
    "ssg.targetPlanSpecialModule",
    "ssg.targetPlanListModule",
    "ssg.shiftPlanModule",
    "ssg.targetPlanModule",
    "ssg.directiveLearnMoreModule",
    "ssg.reportTargetPlanModule",
    "ssg.navigationModule",
    "ssg.navigationHomeModule",
    "ssg.directiveSubNavModule",
    "ssg.overBudgetModule",
    "ssg.overBudgetListModule",
    "ssg.partitionModule",
    "ssg.globalLocationModule",
    "ssg.accessDeniedModule",
    "ssg.directiveTrackingHistoryModule",
    "ssg.workflowViewLogDetailModule",
    "ssg.shiftExchangeViewLogDetailModule",
    "ssg.targetPlanViewLogDetailModule",
    "ssg.settingMaintenantModule",
    "ssg.businessModelModule",
    "ssg.businessModelModuleUnitMapping",
    "ssg.settingShiftCodeControllerModule",
    "ssg.reasonModule",
    "ssg.btaErrorMessageModule",
    "ssg.settingTrackingSyncOrgchartModule"
];
var ssgApp = angular.module("ssg", modules);
// ssgApp.provider('globalsetting', function () {
//     this.alertInfo = function(){
//         alert("Info");
//     }
//     this.$get = function () {       
//         return "globalsetting";
//     }
// });
ssgApp
    .config(function ($stateProvider, $urlRouterProvider, $locationProvider, commonData, localStorageServiceProvider, $translateProvider) {
        $translateProvider.useStaticFilesLoader({
            prefix: 'ClientApp/resources/',
            suffix: '.txt?v=' + edocV
        });
        sessionStorage.removeItem('currentUser');
        sessionStorage.removeItem('departments');
        if (sessionStorage.lang && sessionStorage.lang == 'vi_VN') {
            // Tell the module what language to use by default
            $translateProvider.preferredLanguage('vi_VN');
        } else {

            $translateProvider.preferredLanguage('en_US');
        }
        //$locationProvider.html5Mode(true);
        // $provide.decorator('$exceptionHandler', function ($delegate) {
        //     return function (exception, cause) {
        //         $delegate(exception, cause);
        //         // alert('Error occurred! Please contact admin.');
        //         globalsettingProvider.alertInfo();
        //     };
        // });
        $stateProvider
            .state("home", {
                url: "/home",
                templateUrl: "ClientApp/app/app.html?v=" + edocV,
                controller: "appController",
                resolve: {
                    settingService: "settingService",
                    initData: function (settingService, localStorageService) {
                        if (!localStorageService.get('passTime')) {
                            var currentTime = new Date();
                            localStorageService.set('passTime', currentTime);
                            localStorageService.set('accessSystemTime', currentTime);
                            localStorageService.remove('isTimeOut');
                        }
                    }
                }
            })
            .state("home.dashboard", {
                // Dashboard
                url: "/dashboard",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            .state("home.todo", {
                // Dashboard
                url: "/todo",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            .state("home.orgchart", {
                // orgchart
                url: "/orgchart",
                templateUrl: "ClientApp/app/components/dashboard/orgchart/orgchart-view.html?v=" + edocV,
                controller: "orgChartController"
            })
            .state("home.recruitment", {
                // Recruitment
                url: "/recruitment",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            .state("home.position", {
                url: "/position",
                templateUrl: "ClientApp/app/components/recruitment/position/position-view.main.html?v=" + edocV,
            })
            .state("home.position.item", {
                url: "/item/:id",
                templateUrl: "ClientApp/app/components/recruitment/position/position-item-view.html?v=" + edocV,
                params: { action: { title: "Position items" }, id: "" },
                controller: "positionController"
            })
            .state("home.position.detail", {
                url: "/detail/:referenceValue",
                templateUrl: "ClientApp/app/components/recruitment/position/position-item-detail.html?v=" + edocV,
                params: { action: { title: "Position Detail" }, referenceValue: "" },
                controller: "positionController"
            })
            .state("home.position.myRequests", {
                url: "/myRequests/view=:type",
                templateUrl: "ClientApp/app/components/recruitment/position/position-list-view.html?v=" + edocV,
                params: { action: { title: "My Positions" }, isMyRequest: true, type: "" },
                controller: "positionController"
            })
            .state("home.position.allRequests", {
                url: "/allRequests/view=:type",
                templateUrl: "ClientApp/app/components/recruitment/position/position-list-view.html?v=" + edocV,
                params: { action: { title: "All Positions" }, isMyRequest: false, type: "" },
                controller: "positionController"
            })
            //Request To Hire
            .state("home.requestToHire", {
                // request to hire main
                url: "/requestToHire",
                templateUrl: "ClientApp/app/components/recruitment/request-to-hire/request-to-hire-view.main.html?v=" + edocV,
                //controller: "requestToHireController"
            })
            .state("home.requestToHire.item", {
                // Request to hire detail
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.requestToHire": {
                        templateUrl: "ClientApp/app/components/recruitment/request-to-hire/request-to-hire-item-view.html?v=" + edocV,
                        controller: "requestToHireController"
                    }
                },
                params: { action: { title: "NEW ITEM: RTH" }, referenceValue: "", id: "" }
            })
            .state("home.requestToHire.myRequests", {
                // Request to hire myrequest
                url: "/myRequests",
                views: {
                    "myRequest@home.requestToHire": {
                        templateUrl: "ClientApp/app/components/recruitment/request-to-hire/request-to-hire-list-view.html?v=" + edocV,
                        controller: "requestToHireController"
                    }
                },
                params: { action: { title: "My Requests To Hire" } }
            })
            .state("home.requestToHire.allRequests", {
                // Request to hire all request
                url: "/allRequests",
                views: {
                    "allRequest@home.requestToHire": {
                        templateUrl: "ClientApp/app/components/recruitment/request-to-hire/request-to-hire-list-view.html?v=" + edocV,
                        controller: "requestToHireController"
                    }
                },
                params: { action: { title: "All Requests To Hire" } }
            })
            .state("home.requestToHire.trackingImport", {
                url: "/trackingImport",
                views: {
                    "trackingImport@home.requestToHire": {
                        templateUrl: "ClientApp/app/components/recruitment/request-to-hire/request-to-hire-tracking-import.html?v=" + edocV,
                        controller: "requestToHireController"
                    }
                },
                params: { action: { title: "Tracking import" } }
            })
            //Applicants
            .state("home.applicant", {
                // Recruitment
                url: "/applicant",
                templateUrl: "ClientApp/app/components/recruitment/applicant/applicant-view.main.html?v=" + edocV,
            })
            .state("home.applicant.item", {
                // Applicant
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.applicant": {
                        templateUrl: "ClientApp/app/components/recruitment/applicant/applicant-item-view.html?v=" + edocV,
                        controller: "applicantController",
                    }
                },

                params: { action: { title: "NEW ITEM: APPLICANT" }, referenceValue: "", id: '' }
            })
            .state("home.applicant.myRequests", {
                // Applicant
                url: "/myRequests",
                views: {
                    "myRequest@home.applicant": {
                        templateUrl: "ClientApp/app/components/recruitment/applicant/applicant-list-view.html?v=" + edocV,
                        controller: "applicantController"
                    }
                },

                params: { action: { title: "My Applicants" }, isMyRequest: true }
            })
            .state("home.applicant.allRequests", {
                // Applicant
                url: "/allRequests",
                views: {
                    "allRequest@home.applicant": {
                        templateUrl: "ClientApp/app/components/recruitment/applicant/applicant-list-view.html?v=" + edocV,
                        controller: "applicantController"
                    }
                },
                params: { action: { title: "All Applicants" }, isMyRequest: false }
            })

            //Handover
            .state("home.handover", {
                // Handover main
                url: "/handover",
                templateUrl: "ClientApp/app/components/recruitment/handover/handover-view.main.html?v=" + edocV,
                //controller: "handoverController"
            })
            .state("home.handover.item", {
                // Handover item
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.handover": {
                        templateUrl: "ClientApp/app/components/recruitment/handover/handover-item-view.html?v=" + edocV,
                        controller: "handoverController"
                    }
                },
                params: { action: { title: "NEW ITEM: HANDOVER" }, referenceValue: "", id: '' }
            })
            .state("home.handover.myHandover", {
                // Handover my
                url: "/myHandover",
                views: {
                    "myHandover@home.handover": {
                        templateUrl: "ClientApp/app/components/recruitment/handover/handover-list-view.html?v=" + edocV,
                        controller: "handoverController"
                    }
                },
                params: { action: { title: "My Handovers" } }
            })
            .state("home.handover.allHandover", {
                // Handover all
                url: "/allHandover",
                views: {
                    "allHandover@home.handover": {
                        templateUrl: "ClientApp/app/components/recruitment/handover/handover-list-view.html?v=" + edocV,
                        controller: "handoverController"
                    }
                },
                params: { action: { title: "All Handovers" } }
            })
            .state("home.promoteAndTransfer", {
                url: "/promoteAndTransfer",
                templateUrl: "ClientApp/app/components/recruitment/promote-and-transfer/promote-and-transfer-view.main.html?v=" + edocV,
                //controller: "promoteAndTransferController"
            })
            .state("home.promoteAndTransfer.myRequests", {
                url: "/myRequests",
                views: {
                    "myRequest@home.promoteAndTransfer": {
                        templateUrl: "ClientApp/app/components/recruitment/promote-and-transfer/promote-and-transfer-list-view.html?v=" + edocV,
                        controller: "promoteAndTransferController"
                    }
                },
                params: { action: { title: "My Promote And Transfer Requests" } }
            })
            .state("home.promoteAndTransfer.allRequests", {
                // Promote and transfer // allRequests
                url: "/allRequests",
                views: {
                    "allRequest@home.promoteAndTransfer": {
                        templateUrl: "ClientApp/app/components/recruitment/promote-and-transfer/promote-and-transfer-list-view.html?v=" + edocV,
                        controller: "promoteAndTransferController"
                    }
                },
                params: { action: { title: "All Promote And Transfer Requests" } }
            })
            .state("home.promoteAndTransfer.item", {
                // Promote and transfer item
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.promoteAndTransfer": {
                        templateUrl: "ClientApp/app/components/recruitment/promote-and-transfer/promote-and-transfer-item-view.html?v=" + edocV,
                        controller: "promoteAndTransferController"
                    }
                },
                params: { action: { title: "NEW ITEM: PROMOTION & TRANSFER RECOMMENDATION" }, referenceValue: "", id: "" }
            })
            .state("home.promoteAndTransfer.approve", {
                // Promote and transfer // item-approve
                url: "/item/check/:referenceValue?",
                views: {
                    "itemApprove@home.promoteAndTransfer": {
                        templateUrl: "ClientApp/app/components/recruitment/promote-and-transfer/promote-and-transfer-item-for-approve-view.html?v=" + edocV
                    }
                },
                params: { action: {}, referenceValue: null, id: '' }
            })
            .state("home.newStaffOnboard", {
                // new staff onboard
                url: "/newStaffOnboards",
                templateUrl: "ClientApp/app/components/recruitment/new-staff-onboard/new-staff-onboard-view.html?v=" + edocV,
                controller: "newStaffOnboardController"
            })
            //Action
            .state("home.action", {
                // Action main
                url: "/action",
                templateUrl: "ClientApp/app/components/recruitment/action/action-view.main.html?v=" + edocV,
                controller: "actionController"
            })
            .state("home.action.item", {
                // Action item
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.action": {
                        templateUrl: "ClientApp/app/components/recruitment/action/action-item-view.html?v=" + edocV,
                        controller: "actionController"
                    }
                },
                params: { action: { title: "NEW ITEM: ACTING", state: 'home.action.item' }, referenceValue: "", id: '' }
            })
            .state("home.action.myRequests", {
                // Action my
                url: "/myRequests",
                views: {
                    "myRequest@home.action": {
                        templateUrl: "ClientApp/app/components/recruitment/action/action-list-view.html?v=" + edocV,
                        controller: "actionController"
                    }
                },
                params: { action: { title: "My Acting Requests" } }
            })
            .state("home.action.allRequests", {
                // Action all
                url: "/allRequests",
                views: {
                    "allRequest@home.action": {
                        templateUrl: "ClientApp/app/components/recruitment/action/action-list-view.html?v=" + edocV,
                        controller: "actionController"
                    }
                },
                params: { action: { title: "All Acting Requests" } }
            })
            .state("home.action.itemApprove", {
                url: "/item/check/:referenceValue?",
                views: {
                    "itemApprove@home.action": {
                        templateUrl: "ClientApp/app/components/recruitment/action/action-item-approve-view.html?v=" + edocV
                    }
                },
                params: { action: { title: "ACTING APPLICATION", state: 'home.action.itemApprove' }, referenceValue: null }
            })
            .state("home.action.itemAppraise", {
                url: "/item/appraise/:referenceValue?",
                views: {
                    "itemAppraise@home.action": {
                        templateUrl: "ClientApp/app/components/recruitment/action/action-item-appraise-view.html?v=" + edocV
                    }
                },
                params: { action: { title: "ACTING APPRAISING", state: 'home.action.itemAppraise' }, referenceValue: null }
            })
            // C&B
            .state("home.cb", {
                // C&B
                url: "/cb",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            .state("home.pendingTargetPlan", {
                url: "/pending-target-plan",
                templateUrl: "ClientApp/app/components/cb/shift-plan/target-plan-view.main.html?v=" + edocV,
            })
            .state("home.pendingTargetPlan.item", {
                url: "/add-target-plan",
                views: {
                    "item@home.pendingTargetPlan": {
                        templateUrl: "ClientApp/app/components/cb/shift-plan/pending-target-plan/pending-target-plan-item-view.html?v=" + edocV,
                        controller: "pendingTargetPlanController"
                    }
                },
                params: { action: { title: 'Add Target Plan' }, referenceValue: "", id: "" }
            })
            .state("home.targetPlan", {
                url: "/target-plan",
                templateUrl: "ClientApp/app/components/cb/shift-plan/target-plan-view.main.html?v=" + edocV,
            })
            .state("home.targetPlan.item", {
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.targetPlan": {
                        templateUrl: "ClientApp/app/components/cb/shift-plan/target-plan-list/target-plan-item/target-plan-item-view.html?v=" + edocV,
                        controller: "targetPlanController"
                    }
                },
                params: { action: { title: 'Target Plan' }, referenceValue: "", id: "" }
            })
            .state("home.targetPlan.viewlog", {
                url: "/viewlog/:id?",
                views: {
                    "viewlog@home.targetPlan": {
                        templateUrl: "ClientApp/app/components/view-log-detail/target-plan-view-log-detail-view.html?v=" + edocV,
                        controller: "targetPlanViewLogDetailController"
                    }
                },
                params: { action: { title: 'Target Plan' }, referenceValue: "", id: "" }
            })
            .state("home.targetPlan.myRequests", {
                url: "/myRequests",
                views: {
                    "myRequest@home.targetPlan": {
                        templateUrl: "ClientApp/app/components/cb/shift-plan/target-plan-list/target-plan-list-view.html?v=" + edocV,
                        controller: "targetPlanListController"
                    }
                },
                params: { action: { title: "My Requests" } }
            })
            .state("home.targetPlan.allRequests", {
                url: "/allRequests",
                views: {
                    "allRequest@home.targetPlan": {
                        templateUrl: "ClientApp/app/components/cb/shift-plan/target-plan-list/target-plan-list-view.html?v=" + edocV,
                        controller: "targetPlanListController"
                    }
                },
                params: { action: { title: "All Requests" } }
            })
            .state("home.targetPlan.shiftPlan", {
                url: "/shift-plan",
                views: {
                    "shiftPlan@home.targetPlan": {
                        templateUrl: "ClientApp/app/components/cb/shift-plan/shift-plan-item/shift-plan-item-view.html?v=" + edocV,
                        controller: "shiftPlanController"
                    }
                },
                params: { action: { title: "All Requests" } }
            })
            .state("home.targetPlan.reports", {
                url: "/report-target-plan",
                views: {
                    "reports@home.targetPlan": {
                        templateUrl: "ClientApp/app/components/cb/shift-plan/report-target-plan/report-target-plan-view.html?v=" + edocV,
                        controller: "reportTargetPlanController"
                    }
                },
                params: { action: { title: "Report Target Plan" } }
            })
            .state("home.shiftExchange", {
                // shiftExchange
                url: "/shift-exchange",
                templateUrl: "ClientApp/app/components/cb/shift-exchange-application/shift-exchange-application-view.main.html?v=" + edocV,
                controller: "shiftExchangeApplicationController"
            })
            .state("home.shiftExchange.itemView", {
                // shiftExchange // view readonly
                url: "/item/view/:referenceValue?:id?",
                views: {
                    "itemView@home.shiftExchange": {
                        templateUrl: "ClientApp/app/components/cb/shift-exchange-application/shift-exchange-application-item-readonly-view.html?v=" + edocV
                    }
                },
                params: { action: {}, referenceValue: "", id: "" }
            })
            .state("home.shiftExchange.viewlog", {
                url: "/viewlog/:id?",
                views: {
                    "viewlog@home.shiftExchange": {
                        templateUrl: "ClientApp/app/components/view-log-detail/shift-exchange-view-log-detail-view.html?v=" + edocV,
                        controller: "shiftExchangeViewLogDetailController"
                    }
                },
                params: { action: {}, id: "" }
            })
            .state("home.shiftExchange.item", {
                // shiftExchange // item add or edit
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.shiftExchange": {
                        templateUrl: "ClientApp/app/components/cb/shift-exchange-application/shift-exchange-application-item-view.html?v=" + edocV//,
                        //controller: "shiftExchangeApplicationController"
                    }
                },
                params: { action: {}, referenceValue: "", id: "" }
            })
            .state("home.shiftExchange.myRequests", {
                // shiftExchange myrequest
                url: "/myRequests",
                views: {
                    "myRequest@home.shiftExchange": {
                        templateUrl: "ClientApp/app/components/cb/shift-exchange-application/shift-exchange-application-list-view.html?v=" + edocV,
                        controller: "shiftExchangeApplicationController"
                    }
                },
                params: { action: { title: "My Shift Exchange Requests" } }
            })
            .state("home.shiftExchange.allRequests", {
                //  shiftExchange all request
                url: "/allRequests",
                views: {
                    "allRequest@home.shiftExchange": {
                        templateUrl: "ClientApp/app/components/cb/shift-exchange-application/shift-exchange-application-list-view.html?v=" + edocV,
                        controller: "shiftExchangeApplicationController"
                    }
                },
                params: { action: { title: "All Shift Exchange Requests" } }
            })
            .state("home.leavesManagement", {
                // Leaves management
                url: "/leaves-management",
                templateUrl: "ClientApp/app/components/cb/leave-management/leave-management-view.main.html?v=" + edocV,
                // controller: "leaveManagementController"
            })
            .state("home.leavesManagement.item", {
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.leavesManagement": {
                        templateUrl: "ClientApp/app/components/cb/leave-management/leave-management-item-view.html?v=" + edocV,
                        controller: "leaveManagementController"
                    }
                },
                params: { action: {}, referenceValue: "", id: "" }
            })
            .state("home.leavesManagement.myRequests", {
                url: "/myRequests",
                views: {
                    "myRequest@home.leavesManagement": {
                        templateUrl: "ClientApp/app/components/cb/leave-management/leave-management-list-view.html?v=" + edocV,
                        controller: "leaveManagementController"
                    }
                },
                params: { action: { title: "My Leave Requests" } }
            })
            .state("home.leavesManagement.allRequests", {
                url: "/allRequests",
                views: {
                    "allRequest@home.leavesManagement": {
                        templateUrl: "ClientApp/app/components/cb/leave-management/leave-management-list-view.html?v=" + edocV,
                        controller: "leaveManagementController"
                    }
                },
                params: { action: { title: "All Leave Requests" } }
            })
            .state("home.missingTimelock", {
                // missing timeClock // myRequests
                url: "/missingTimelock",
                templateUrl: "ClientApp/app/components/cb/missing-timelock/missing-timelock-view.main.html?v=" + edocV,
                controller: "missingTimelockController"
            })
            .state("home.missingTimelock.myRequests", {
                // missing timeClock // myRequests
                url: "/myRequests",
                views: {
                    "myRequest@home.missingTimelock": {
                        templateUrl: "ClientApp/app/components/cb/missing-timelock/missing-timelock-list-view.html?v=" + edocV,
                        controller: "missingTimelockController"
                    }
                },
                params: { action: { title: "My Missing TimeClock Requests" }, type: "myRequests" }
            })
            .state("home.missingTimelock.allRequests", {
                // missing timeClock // allRequests
                url: "/allRequests",
                views: {
                    "allRequest@home.missingTimelock": {
                        templateUrl: "ClientApp/app/components/cb/missing-timelock/missing-timelock-list-view.html?v=" + edocV,
                        controller: "missingTimelockController"
                    }
                },
                params: { action: { title: "All Missing TimeClock Requests" }, type: "allRequests" }
            })
            .state("home.missingTimelock.item", {
                // missing timeClock // item
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.missingTimelock": {
                        templateUrl: "ClientApp/app/components/cb/missing-timelock/missing-timelock-item-view.html?v=" + edocV,
                        controller: "missingTimelockController"
                    }
                },
                params: { action: {}, referenceValue: "", id: "" }
            })
            .state("home.missingTimelock.approve", {
                // missing timeClock // item approve
                url: "/item/:referenceValue?:id?",
                views: {
                    "itemApprove@home.missingTimelock": {
                        templateUrl: "ClientApp/app/components/cb/missing-timelock/missing-timelock-item-for-approve-view.html?v=" + edocV
                    }
                },
                params: { action: {}, referenceValue: "", id: "" }
            })
            .state('home.setting', { // C&B
                url: '/setting',
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            // overtime application
            .state("home.overtimeApplication", {
                //  overtime application main
                url: "/overtimeApplication",
                templateUrl: "ClientApp/app/components/cb/overtime-application/overtime-application-view.main.html?v=" + edocV,
                controller: "overtimeApplicationController"
            })
            .state("home.overtimeApplication.item", {
                //  overtime applicati1on detail
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.overtimeApplication": {
                        templateUrl: "ClientApp/app/components/cb/overtime-application/overtime-application-item-view.html?v=" + edocV//,
                        //controller: "overtimeApplicationController"
                    }
                },
                params: { action: {}, referenceValue: null, type: 'home.overtimeApplication.item', id: '' }
            })
            .state("home.overtimeApplication.view", {
                //  overtime application detail (readonly)
                url: "/item/:referenceValue?:id?",
                views: {
                    "view@home.overtimeApplication": {
                        templateUrl: "ClientApp/app/components/cb/overtime-application/overtime-application-item-readonly-view.html?v=" + edocV
                    }
                },
                params: { action: {}, referenceValue: null, type: 'home.overtimeApplication.view', id: '' }
            })
            .state("home.overtimeApplication.myRequests", {
                // overtime application myrequest
                url: "/myRequests",
                views: {
                    "myRequest@home.overtimeApplication": {
                        templateUrl: "ClientApp/app/components/cb/overtime-application/overtime-application-list-view.html?v=" + edocV,
                        controller: "overtimeApplicationController"
                    }
                },
                params: { action: { title: "My Overtime Requests" }, type: 'home.overtimeApplication.myRequests' }
            })
            .state("home.overtimeApplication.allRequests", {
                //  overtime application all request
                url: "/allRequests",
                views: {
                    "allRequest@home.overtimeApplication": {
                        templateUrl: "ClientApp/app/components/cb/overtime-application/overtime-application-list-view.html?v=" + edocV,
                        controller: "overtimeApplicationController"
                    }
                },
                params: { action: { title: "All Overtime Requests" }, type: 'home.overtimeApplication.allRequests' }
            })
            //Resignation Application
            .state("home.resignationApplication", {
                //Resignation Application // myRequests
                url: "/resignationApplication",
                templateUrl: "ClientApp/app/components/cb/resignation-application/resignation-application-view.main.html?v=" + edocV,
                controller: "resignationApplicationController"
            })
            .state("home.resignationApplication.myRequests", {
                //Resignation Application // myRequests
                url: "/myRequests",
                views: {
                    "myRequest@home.resignationApplication": {
                        templateUrl: "ClientApp/app/components/cb/resignation-application/resignation-application-list-view.html?v=" + edocV,
                        controller: "resignationApplicationController"
                    }
                },
                params: { action: { title: "My Resignation Request" }, type: "myRequests" }
            })
            .state("home.resignationApplication.allRequests", {
                //Resignation Application // allRequests
                url: "/allRequests",
                views: {
                    "allRequest@home.resignationApplication": {
                        templateUrl: "ClientApp/app/components/cb/resignation-application/resignation-application-list-view.html?v=" + edocV,
                        controller: "resignationApplicationController"
                    }
                },
                params: { action: { title: "All Resignation Request" }, type: "allRequests" }
            })
            .state("home.resignationApplication.item", {
                //Resignation Application // item
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.resignationApplication": {
                        templateUrl: "ClientApp/app/components/cb/resignation-application/resignation-application-item-view.html?v=" + edocV//,
                        //controller: "resignationApplicationController"
                    }
                },
                params: { action: {}, referenceValue: '', id: '', type: "item" }
            })
            .state("home.resignationApplication.approve", {
                //Resignation Application // approve
                url: "/item/check/:referenceValue?",
                views: {
                    "itemApprove@home.resignationApplication": {
                        templateUrl: "ClientApp/app/components/cb/resignation-application/resignation-application-item-approve-view.html?v=" + edocV
                    }
                },
                params: { action: {}, referenceValue: null, type: "approve" }
            })

            .state('home.user-setting', { // Setting
                url: '/user-setting',
                templateUrl: 'ClientApp/app/components/setting/users/user-view.html?v=' + edocV
            })
            .state('home.user-setting.user-profile', { // Setting
                url: '/user-profile/:referenceValue?',
                views: {
                    'userProfile@home.user-setting': {
                        templateUrl: 'ClientApp/app/components/setting/users/user-profile.html?v=' + edocV,
                        controller: 'settingUserController'
                    }
                },
                params: { action: {}, referenceValue: null }
            })
            .state('home.user-setting.user-list', { // Setting
                url: '/user-list',
                views: {
                    'userList@home.user-setting': {
                        templateUrl: 'ClientApp/app/components/setting/users/user-list.html?v=' + edocV,
                        controller: 'settingUserController'
                    }
                },
                params: { action: { title: 'users' } }
            })
            .state('home.departments', { // department management
                url: '/departments',
                templateUrl: 'ClientApp/app/components/setting/departments/department-view.html?v=' + edocV,
                controller: 'departmentController'
            })
            .state('home.budgets', { // budget management
                url: '/budgets',
                templateUrl: 'ClientApp/app/components/setting/budgets/budget-view.html?v=' + edocV,
                controller: 'budgetController'
            })
            .state("home.settingRecruitmentWorkingTime", {
                // working time
                url: "/workingTime",
                templateUrl: "ClientApp/app/components/setting/recruitments/working-time/recruitment-working-time-view.html?v=" + edocV,
                controller: "settingRecruitmentWorkingTimeController",
                params: { action: { title: 'Working Time' }, referenceValue: 'workingTime' }
            })
            .state("home.settingRecruitmentCategories", {
                url: "/categories",
                templateUrl: "ClientApp/app/components/setting/recruitments/category/category-view.html?v=" + edocV,
                controller: "settingRecruitmentCategoriesController",
                params: { action: { title: 'Categories' }, referenceValue: 'category' }
            })
            .state("home.settingRecruitmentPromoteTransferPrint", {
                url: "/promoteTransferPrint",
                templateUrl: "ClientApp/app/components/setting/recruitments/promoteTransferPrint/recruitment-promote-transfer-print-view.html?v=" + edocV,
                controller: "settingRecruitmentPromoteTransferPrintController",
                params: { action: { title: 'Promote & Transfer Print' }, referenceValue: 'promoteTransferPrint' }
            })
            .state("home.settingRecruitmenItemList", {
                // item list
                url: "/itemList",
                templateUrl: "ClientApp/app/components/setting/recruitments/item-list/recruitment-item-list.html?v=" + edocV,
                controller: "settingRecruitmentItemListController",
                params: { action: { title: 'Item List' }, referenceValue: 'itemList' }
            })
            .state("home.settingRecruitmentAppreciationList", {
                // working time
                url: "/appreciationList",
                templateUrl: "ClientApp/app/components/setting/recruitments/appreciation/recruitment-appreciation-list.html?v=" + edocV,
                controller: "settingRecruitmentAppreciationListController",
                params: { action: { title: 'Appreciation List' }, referenceValue: null }
            })
            .state("home.settingRecruitmentApplicantStatus", {
                // item list
                url: "/applicantStatus",
                templateUrl: "ClientApp/app/components/setting/recruitments/status/recruitment-applicant-status.html?v=" + edocV,
                controller: "settingRecruitmentApplicantStatusController",
                params: { action: { title: 'APPLICANT STATUS LIST' }, referenceValue: null }
            })
            .state("home.settingRecruitmentPosition", {
                // Position
                url: "/positions",
                templateUrl: "ClientApp/app/components/setting/recruitments/position/recruitment-position.html?v=" + edocV,
                controller: "settingRecruitmentPositionController",
                params: { action: { title: 'Position List' }, referenceValue: null }
            })
            .state("home.settingRecruitmentCostCenter", {
                // cost center
                url: "/costCenter",
                templateUrl: "ClientApp/app/components/setting/recruitments/cost-center/recruitment-cost-center.html?v=" + edocV,
                controller: "settingRecruitmentCostCenterController",
                params: { action: { title: 'Cost Center' }, referenceValue: null }
            })
            .state("home.settingRecruitmentWorkingAddress", {
                // cost center
                url: "/workingAddress",
                templateUrl: "ClientApp/app/components/setting/recruitments/working-address/recruitment-working-address.html?v=" + edocV,
                controller: "settingRecruitmentWorkingAddressController",
                params: { action: { title: 'Working Address' }, referenceValue: null }
            })
            // job Grade
            .state('home.jobGrades', { //  jobGrade main
                url: '/jobGrades',
                templateUrl: 'ClientApp/app/components/setting/job-grade/job-grade-view.main.html?v=' + edocV,
                controller: 'jobGradeController'
            })
            // missing time clock reason
            .state('home.missingTimeclockReasons', { //  missing time clock reason
                url: '/missingTimeclockReasons',
                templateUrl: 'ClientApp/app/components/setting/cb/c-and-b-reason-list-view.html?v=' + edocV,
                controller: 'cAndBReasonController',
                params: { action: { title: "Missing Timeclock Reasons" }, type: commonData.reasonType.MISSING_TIMECLOCK_REASON }
            })
            .state('home.overtimeReasons', { //  overtime reason
                url: '/overtimeReasons',
                templateUrl: 'ClientApp/app/components/setting/cb/c-and-b-reason-list-view.html?v=' + edocV,
                controller: 'cAndBReasonController',
                params: { action: { title: "Overtime Reasons" }, type: commonData.reasonType.OVERTIME_REASON }
            })
            .state('home.resignationReasons', { //  resignation reason
                url: '/resignationReasons',
                templateUrl: 'ClientApp/app/components/setting/cb/c-and-b-reason-list-view.html?v=' + edocV,
                controller: 'cAndBReasonController',
                params: { action: { title: "Resignation Reasons" }, type: commonData.reasonType.RESIGNATION_REASON }
            })
            .state('home.shiftExchangeReasons', { //  shift exchange reason
                url: '/shiftExchangeReasons',
                templateUrl: 'ClientApp/app/components/setting/cb/c-and-b-reason-list-view.html?v=' + edocV,
                controller: 'cAndBReasonController',
                params: { action: { title: "Shift Exchange Reasons" }, type: commonData.reasonType.SHIFT_EXCHANGE_REASON }
            })
            .state('home.holidaySchedule', {
                url: '/holidaySchedule',
                templateUrl: 'ClientApp/app/components/setting/cb/holiday-schedule/cb-holiday-schedule.html?v=' + edocV,
                controller: 'holidayScheduleController',
                params: { action: { title: "Holiday Schedule" } }
            })
            .state('home.shiftCode', {
                url: '/shiftCode',
                templateUrl: 'ClientApp/app/components/setting/cb/shift-code/cb-shift-code.html?v=' + edocV,
                controller: 'shiftCodeController',
                params: { action: { title: "Shift Code" } }
            })
            .state("home.workflowszzyyxx", {
                url: '/workflowszzyyxx',
                templateUrl: 'ClientApp/app/components/setting/workflows/workflow-view.main.html?v=' + edocV,
                controller: 'settingWorkFlowController'
            })
            .state("home.workflowszzyyxx.viewlog", {
                url: "/viewlog/:referenceValue?itemType=",
                views: {
                    "viewlog@home.workflowszzyyxx": {
                        templateUrl: 'ClientApp/app/components/view-log-detail/workflow-view-log-detail-view.html?v=' + edocV,
                        controller: "workFlowViewLogDetailController"
                    }
                },
                params: { action: {}, referenceValue: "", itemType: "" }
            })
            .state("home.workflowszzyyxx.item", {
                url: "/item/:referenceValue?itemType=",
                views: {
                    "item@home.workflowszzyyxx": {
                        templateUrl: 'ClientApp/app/components/setting/workflows/workflow-item-view.html?v=' + edocV,
                        controller: "settingWorkFlowController"
                    }
                },
                params: { action: {}, referenceValue: "", itemType: "" }
            })
            .state("home.workflowszzyyxx.myRequests", {
                url: "/workflows/myRequests",
                views: {
                    "myRequest@home.workflowszzyyxx": {
                        templateUrl: 'ClientApp/app/components/setting/workflows/workflow-list-view.html?v=' + edocV,
                        controller: "settingWorkFlowController"
                    }
                },
                params: { action: { title: "My Work flow" } }
            })
            .state('home.airline', { //  Airline
                url: '/airline',
                templateUrl: 'ClientApp/app/components/setting/bta/airline/airline-view.html?v=' + edocV,
                controller: 'airlineController',
                params: { action: { title: 'Airline' }, referenceValue: null }
            })
            .state('home.flightNumber', { // flight Number
                url: '/flightNumber',
                templateUrl: 'ClientApp/app/components/setting/bta/flight-number/flight-number-view.html?v=' + edocV,
                controller: 'flightNumberController',
                params: { action: { title: 'Flight Number' }, referenceValue: null }
            })
            .state('home.hotel', { //  Hotel
                url: '/hotel',
                templateUrl: 'ClientApp/app/components/setting/bta/hotel/hotel-view.html?v=' + edocV,
                controller: 'hotelController',
                params: { action: { title: 'Hotel' }, referenceValue: null }
            })
            .state('home.location', { //  Location
                url: '/location',
                templateUrl: 'ClientApp/app/components/setting/bta/location/location-view.html?v=' + edocV,
                controller: 'locationController',
                params: { action: { title: 'Location' }, referenceValue: null }
            })
            .state('home.roomType', { //  Room type
                url: '/roomType',
                templateUrl: 'ClientApp/app/components/setting/bta/room-type/room-type-view.html?v=' + edocV,
                controller: 'roomTypeController',
                params: { action: { title: 'Room Type' }, referenceValue: null }
            })
            .state('home.btaPolicy', { //  Room type
                url: '/btaPolicy',
                templateUrl: 'ClientApp/app/components/setting/bta/bta-policy/bta-policy-view.html?v=' + edocV,
                controller: 'btaPolicyController',
                params: { action: { title: 'Budget Limit: HQ & Store' }, referenceValue: null }
            })
            .state('home.btaPolicySpecial', { //  BTA Policy Special
                url: '/btaPolicySpecial',
                templateUrl: 'ClientApp/app/components/setting/bta/bta-policy-special-case/bta-policy-special-case-view.html?v=' + edocV,
                controller: 'btaPolicySpecialCaseController',
                params: { action: { title: 'Budget Limit: Special Case' }, referenceValue: null }
            })
            .state('home.reason', { //  Reason
                url: '/reason',
                templateUrl: 'ClientApp/app/components/setting/bta/reason/reason-view.html?v=' + edocV,
                controller: 'reasonController',
                params: { action: { title: 'Reason' }, referenceValue: null }
            })
            .state('home.referenceNumbers', { // reference number
                url: '/referenceNumbers',
                templateUrl: 'ClientApp/app/components/setting/reference-number/reference-number.html?v=' + edocV,
                controller: 'settingReferenceNumberController'
            })
            .state('home.trackingLogs', { // reference number
                url: '/tracking-logs',
                templateUrl: 'ClientApp/app/components/setting/tracking-logs/tracking-log-list-view.html?v=' + edocV,
                controller: 'settingTrackingLogController',
                params: { action: { title: "Tracking Logs" } }
            })
            .state('home.trackingSyncOrgcharts', { //tracking log sync orgchart
                url: '/tracking-sync-orgchart',
                templateUrl: 'ClientApp/app/components/setting/tracking-sync-orgchart/tracking-sync-orgchart.html?v=' + edocV,
                controller: 'settingTrackingSyncOrgchartController',
                params: { action: { title: "Tracking Sync Orgchart" } }
            })
            .state('notFoundPage', {
                url: '/page404',
                templateUrl: 'ClientApp/app/components/errors-page/not-found-page.html?v=' + edocV,
                controller: 'notPageFoundController',
                params: { action: { title: "404" } }
            })
            .state('redirectPage', { // reference number
                url: '/index/redirectToPage',
                templateUrl: 'ClientApp/app/redirect-page-view.html?v=' + edocV,
                controller: 'redirectPageController',
                params: { action: { title: "404" } }
            })
            .state('home.shiftPlanSubmitPerson', { // shift plan submit person
                url: '/shift-plan-submit-persons',
                templateUrl: 'ClientApp/app/components/setting/shift-plan-submit-person/shift-plan-submit-person-view.html?v=' + edocV,
                controller: 'shiftPlanSubmitPersonController'
            })
            .state('home.targetPlanSpecial', { // shift plan special case cr 9.9
                url: '/target-plan-special',
                templateUrl: 'ClientApp/app/components/setting/target-plan-special/target-plan-special-view.html?v=' + edocV,
                controller: 'targetPlanSpecialController'
            })
            // BTA
            .state("home.bta", {
                // BTA
                url: "/bta",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            .state("home.business-trip-application", {
                // Leaves management
                url: "/business-trip-application",
                templateUrl: "ClientApp/app/components/bta/business-trip-application/business-trip-view.main.html?v=" + edocV,
                // controller: "leaveManagementController"
            })
            .state("home.business-trip-application.item", {
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.business-trip-application": {
                        templateUrl: "ClientApp/app/components/bta/business-trip-application/business-trip-item/business-trip-item-view.html?v=" + edocV,
                        controller: "businessTripItemController"
                    }
                },
                params: { action: {}, referenceValue: "", id: "" }
            })
            .state("home.business-trip-application.myRequests", {
                url: "/myRequests",
                views: {
                    "myRequest@home.business-trip-application": {
                        templateUrl: "ClientApp/app/components/bta/business-trip-application/business-trip-list/business-trip-list-view.html?v=" + edocV,
                        controller: "businessTripListController"
                    }
                },
                params: { action: { title: "BTA_MY_REQUESTS" } }
            })
            .state("home.business-trip-application.allRequests", {
                url: "/allRequests",
                views: {
                    "allRequest@home.business-trip-application": {
                        templateUrl: "ClientApp/app/components/bta/business-trip-application/business-trip-list/business-trip-list-view.html?v=" + edocV,
                        controller: "businessTripListController"
                    }
                },
                params: { action: { title: "BTA_ALL_REQUESTS" } }
            })
            .state("home.business-trip-application-report", {
                // Business Trip Application Report
                url: "/business-trip-application-report",
                templateUrl: "ClientApp/app/components/bta/business-trip-report/business-trip-report-view.html?v=" + edocV,
                controller: "businessTripReportController",
                params: { action: { title: "Business Trip Application Report" }, type: '' }
            })
            .state('home.daysConfiguration', { // reference number
                url: '/days-configuration',
                templateUrl: 'ClientApp/app/components/setting/days-configuration/days-configuration-view.html?v=' + edocV,
                controller: 'settingDaysConfigurationController',
                params: { action: { title: "Days Configuration" } }
            })
            .state("home.maintenant", {
                url: '/maintenant',
                templateUrl: 'ClientApp/app/components/setting/maintenant/maintenant-view.main.html?v=' + edocV,
                controller: 'settingMaintenantController'
            })
            .state("home.maintenant.dashboard", {
                url: "/maintenant/dashboard",
                views: {
                    "dashboard@home.maintenant": {
                        templateUrl: 'ClientApp/app/components/setting/maintenant/maintenant-dashboard.html?v=' + edocV,
                        controller: "settingMaintenantController"
                    }
                },
                params: { action: { title: "Maintenant Dashboard" } }
            })
            //Over Budget
            .state("home.over-budget", {
                // Over Budget
                url: "/over-budget",
                templateUrl: "ClientApp/app/components/bta/business-trip-application/over-budget/over-budget-view.main.html?v=" + edocV,
            })
            .state("home.over-budget.item", {
                // Over Budget
                url: "/item/:referenceValue?:id?",
                views: {
                    "item@home.over-budget": {
                        templateUrl: "ClientApp/app/components/bta/business-trip-application/over-budget/over-budget-item-view.html?v=" + edocV,
                        controller: "overBudgetItemController",
                    }
                },

                params: { action: { title: "NEW ITEM: Over Budget" }, referenceValue: "", id: '' }
            })
            .state("home.over-budget.myRequests", {
                // Over Budget
                url: "/myRequests",
                views: {
                    "myRequest@home.over-budget": {
                        templateUrl: "ClientApp/app/components/bta/business-trip-application/over-budget/over-budget-list-view.html?v=" + edocV,
                        controller: "overBudgetListController"
                    }
                },

                params: { action: { title: "MY OVER BUDGET" }, isMyRequest: true }
            })
            .state("home.over-budget.allRequests", {
                // Over Budget
                url: "/allRequests",
                views: {
                    "allRequest@home.over-budget": {
                        templateUrl: "ClientApp/app/components/bta/business-trip-application/over-budget/over-budget-list-view.html?v=" + edocV,
                        controller: "overBudgetListController"
                    }
                },
                params: { action: { title: "ALL OVER BUDGET" }, isMyRequest: false }
            })
            .state('home.partition', {
                url: '/partition',
                templateUrl: 'ClientApp/app/components/setting/bta/partition/partition-view.html?v=' + edocV,
                controller: 'partitionController',
                params: { action: { title: "Partition" } }
            })
            //.state('home.globalLocation', {
            //    url: '/global-location',
            //    templateUrl: 'ClientApp/app/components/setting/bta/global-location/global-location-view.html?v=' + edocV,
            //    controller: 'globalLocationController',
            //    params: { action: { title: "Global Location" } }
            //})

            .state('home.navigation-list', { // Navigation management
                url: '/navigation-list',
                templateUrl: 'ClientApp/app/components/setting/navigation/navigation-list.html?v=' + edocV,
                controller: 'navigationController'
            })
            .state('home.navigation-home', { // Navigation home management
                url: '/navigation-home',
                templateUrl: 'ClientApp/app/components/navigation-home/navigation-view.html?v=' + edocV,
                controller: 'navigationHomeController'
            })

            // facilities
            .state("home.facility", {
                url: "/facility",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            // admin
            .state("home.admin", {
                url: "/admin",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            .state('accessDeniedPage', {
                url: '/page401',
                templateUrl: 'ClientApp/app/components/errors-page/access-denied-page.html?v=' + edocV,
                controller: 'accessDeniedController',
                params: { action: { title: "401" } }
            })
            .state('home.businessModel', { // business type
                url: '/business-model',
                templateUrl: 'ClientApp/app/components/setting/business-model/business-model-view.main.html?v=' + edocV,
                controller: 'businessModelController',
                params: { action: { title: "Business Model" } }
            })
            .state('home.businessModelUnitMapping', { // business type mapping business unit
                url: '/business-model-unit-mapping',
                templateUrl: 'ClientApp/app/components/setting/business-model-unit-mapping/business-model-unit-mapping-view.main.html?v=' + edocV,
                controller: 'businessModelUnitMappingController',
                params: { action: { title: "Business Model Unit Mapping" } }
            })
            .state("home.bta-error-message", {
                // working time
                url: "/bta-error-message",
                templateUrl: "ClientApp/app/components/setting/bta/bta-error-message/bta-error-message-list.html?v=" + edocV,
                controller: "btaErrorMessageListController",
                params: { action: { title: 'BTA Error List' } }
            })

        $urlRouterProvider.otherwise('/index/redirectToPage');
    }).run(function ($history, $state, $location, $transitions, $rootScope, appSetting, $stateParams, $timeout, $q, settingService, localStorageService) {
        $transitions.onStart({}, function () {
            $rootScope.redirectUrl = $location.$$absUrl;
            // dùng cho trường hợp click go back page khi đang mở các dialog popup của mấy button ở workflow
            if ($rootScope.confirmVoteDialog) {
                $rootScope.confirmVoteDialog.close();
            }
            // dùng cho trường hợp click go back page khi đang mở các dialog popup DELETE row
            if ($rootScope.confirmDialog) {
                $rootScope.confirmDialog.close();
            }
            // dùng cho trường hợp click go back page khi đang mở dialog chổ default asset item ben JobGrade
            if ($rootScope.defaultAssetDialog) {
                $rootScope.defaultAssetDialog.close();
            }

            //dùng cho trường hợp click go back page khi đang mở dialog applicant create
            if ($rootScope.confirmGrade) {
                $rootScope.confirmGrade.close();
            }
            // dùng cho truong hop popup add user bên overtime, shiftexchange
            if ($rootScope.confirmDialogAddItemsUser) {
                $rootScope.confirmDialogAddItemsUser.close();
            }

            //dung cho popup change password
            if ($rootScope.confirmDialogChangePassWord) {
                $rootScope.confirmDialogChangePassWord.close();
            }
            // employee Info bên position detail
            if ($rootScope.confirmDialogEmployeeInfo) {
                $rootScope.confirmDialogEmployeeInfo.close();
            }

            //processing stage
            if ($rootScope.confirmProcessing) {
                $rootScope.confirmProcessing.close();
            }   
            // cho popup re-assignee
            if ($rootScope.confirmDialogReassignee) {
                $rootScope.confirmDialogReassignee.close();
            }
            // cho popup report BTA
            if ($rootScope.confirmDialogReport) {
                $rootScope.confirmDialogReport.close();
            }
        });
        $transitions.onSuccess({}, function () {
            $history.push($state);
            var existMappingItem = _.find(appSetting.mappingStates, x => { return x.source === $state.current.name });
            if (existMappingItem) {
                $state.go(existMappingItem.destination, existMappingItem.params);
            } else {
                var isParentMenu = _.find(appSetting.parentMenus, x => { return x === $state.current.name });
                if (isParentMenu) {
                    $rootScope.isParentMenu = true;
                    if ($rootScope.currentUser) {
                        $rootScope.createPageComponent($state.current.name);
                    }
                } else {
                    $rootScope.isParentMenu = false;
                }
                $state.go($state.current.name);
            }
        });
        // $rootScope.$on('$locationChangeSuccess', function () {
        //     $rootScope.actualLocation = $location.path();
        // });
        // $rootScope.$watch(function () { return $location.path() }, function (newLocation, oldLocation) {

        // });

    });