angular
    .module('ssg')
    .factory('cbService', function ($resource, $window, appSetting, $rootScope, interceptorService) {
        var _ = $window._;
        var instance = null;

        function createInstance() {
            instance = new createfactoryService();
            return instance;
        }

        function createfactoryService() {
            var url = baseUrlApi + "/:controller/:action/:id";
            var customHeaders = buildCustomHeader();
            return {
                missingTimelocks: $resource(url, null, {
                    saveMissingTimelock: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "MissingTimelock", action: 'SaveMissingTimelock' },
                        headers: customHeaders
                    },
                    submitMissingTimelock: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "MissingTimelock", action: 'SubmitMissingTimelock' },
                        headers: customHeaders
                    },
                    getMissingTimelockByReferenceNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "MissingTimelock", action: 'GetMissingTimelockByReferenceNumber' },
                        headers: customHeaders
                    },
                    getAllMissingTimelockByUser: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "MissingTimelock", action: 'GetAllMissingTimelockByUser' },
                        headers: customHeaders
                    },
                    getMissingTimelocks: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "MissingTimelock", action: 'GetMissingTimelocks' },
                        headers: customHeaders
                    }
                }),
                resignationApplication: $resource(url, null, {
                    getAllResignationApplication: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'GetAllResignationApplicantion' },
                        headers: customHeaders
                    },
                    checkInProgressResignationApplicantion: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'CheckInProgressResignationApplicantion' },
                        headers: customHeaders
                    },
                    //function check moi
                    checkInProgressResignationWithIsActive: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'CheckInProgressResignationWithIsActive' },
                        headers: customHeaders
                    },
                    GetResignationApplicantionByReferenceNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'GetResignationApplicantionByReferenceNumber' },
                        headers: customHeaders
                    },
                    getResignationApplicationByReferenceNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'GetAllResignationApplicantion' },
                        headers: customHeaders
                    },
                    saveResignationApplication: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'SaveResignationApplicantion' },
                        headers: customHeaders
                    },
                    submitResignationApplication: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'SubmitResignationApplicantion' },
                        headers: customHeaders
                    },
                    getResignationApplicantionById: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'GetResignationApplicantionById' },
                        headers: customHeaders
                    },
                    printForm: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'PrintForm' },
                        headers: customHeaders
                    },
                    getSubmitedFirstDate: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'GetSubmitedFirstDate' },
                        headers: customHeaders
                    },
                    countExitInterview: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ResignationApplicationCB", action: 'CountExitInterview' },
                        headers: customHeaders
                    }
                }),
                leaveManagement: $resource(url, null, {
                    getListLeaveApplication: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "LeaveManagement", action: 'GetListLeaveApplication' },
                        headers: customHeaders
                    },
                    createNewLeaveApplication: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "LeaveManagement", action: 'CreateNewLeaveApplication' },
                        headers: customHeaders
                    },
                    updateLeaveApplication: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "LeaveManagement", action: 'UpdateLeaveApplication' },
                        headers: customHeaders
                    },
                    checkValidLeaveKind: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "LeaveManagement", action: 'CheckValidLeaveKind' },
                        headers: customHeaders
                    },
                    getLeaveApplicantDetail: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "LeaveManagement", action: 'GetLeaveApplicantDetail' },
                        headers: customHeaders
                    },
                    getLeaveApplicationById: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "LeaveManagement", action: 'GetLeaveApplicationById' },
                        headers: customHeaders
                    },
                    getLeaveApplicationFromUserId: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "LeaveManagement", action: 'GetLeaveApplicationFromUserId' },
                        headers: customHeaders
                    },
                    getAllLeaveManagementByUserId: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "LeaveManagement", action: 'GetAllLeaveManagementByUserId' },
                        headers: customHeaders
                    },
                }),
                overtimeApplication: $resource(url, null, {
                    createOvertimeApplication: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OvertimeApplication", action: 'CreateOvertimeApplication' },
                        headers: customHeaders
                    },
                    saveOvertimeApplication: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OvertimeApplication", action: 'SaveOvertimeApplication' },
                        headers: customHeaders
                    },
                    getOvertimeApplication: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OvertimeApplication", action: 'GetOvertimeApplicationById' },
                        headers: customHeaders
                    },
                    getOvertimeApplicationList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OvertimeApplication", action: 'GetOvertimeApplicationList' },
                        headers: customHeaders
                    },
                    getOvertimeApplications: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OvertimeApplication", action: 'GetOvertimeApplications' },
                        headers: customHeaders
                    },
                    printForm: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OvertimeApplication", action: 'PrintForm' },
                        headers: customHeaders
                    },
                    import: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OvertimeApplication", action: 'Import' },
                        headers: customHeaders
                    },
                    importActual: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OvertimeApplication", action: 'importActual' },
                        headers: customHeaders
                    },
                }),
                shiftExchange: $resource(url, null, {
                    getAllShiftExchange: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'GetAllShiftExchange' },
                        headers: customHeaders
                    },
                    getShiftExchanges: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'GetShiftExchanges' },
                        headers: customHeaders
                    },
                    getShiftExchange: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'GetShiftExchange' },
                        headers: customHeaders
                    },
                    saveShiftExchange: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'SaveShiftExchange' },
                        headers: customHeaders
                    },
                    checkShiftExchangeComplete: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'CheckTargetPlanComplete' },
                        headers: customHeaders
                    },
                    submitShiftExchange: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'SubmitShiftExchange' },
                        headers: customHeaders
                    },
                    getShiftExchangeDetailById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'GetShiftExchangeDetailById' },
                        headers: customHeaders
                    },
                    getAvailableLeaveBalance: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'GetAvailableLeaveBalances' },
                        headers: customHeaders
                    },
                    validateERDShiftExchangeDetail: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'ValidateERDShiftExchangeDetail' },
                        headers: customHeaders
                    },
                    getCurrentShiftCodeFromShiftPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ShiftExchange", action: 'GetCurrentShiftCodeFromShiftPlan' },
                        headers: customHeaders
                    }
                }),
                targetPlan: $resource(url, null, {
                    getTargetPlanPeriods: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetTargetPlanPeriods' },
                        headers: customHeaders
                    },
                    savePendingTargetPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'SavePendingTargetPlan' },
                        headers: customHeaders
                    },
                    savePendingBeforeSubmit: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'SavePendingBeforeSubmit' },
                        headers: customHeaders
                    },
                    sendRequest_TargetPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'SendRequest_TargetPlan' },
                        headers: customHeaders
                    },
                    submit: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'Submit' },
                        headers: customHeaders
                    },
                    getPendingTargetPlanDetailFromSAPCodesAndPeriod: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetPendingTargetPlanDetailFromSAPCodesAndPeriod' },
                        headers: customHeaders
                    },
                    getActualShiftPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetActualShiftPlan' },
                        headers: customHeaders
                    },
                    getPendingTargetPlanDetails: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetPendingTargetPlanDetails' },
                        headers: customHeaders
                    },
                    getUserSAPForTargetPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetUserSAPForTargetPlan' },
                        headers: customHeaders
                    },
                    getPendingTargetPlanFromDepartmentAndPeriod: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetPendingTargetPlanFromDepartmentAndPeriod' },
                        headers: customHeaders
                    },
                    getValidUserSAPForTargetPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetValidUserSAPForTargetPlan' },
                        headers: customHeaders
                    },
                    getTargetPlanDetailIsCompleted: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetTargetPlanDetailIsCompleted' },
                        headers: customHeaders
                    },
                    setSubmittedStateForDetailPendingTargetPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'SetSubmittedStateForDetailPendingTargetPlan' },
                        headers: customHeaders
                    },
                    getList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetList' },
                        headers: customHeaders
                    },
                    getItem: {
                        method: 'Post',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetItem' },
                        headers: customHeaders
                    },
                    getSAPCode_PendingTargetPlanDetails: {
                        method: 'Get',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetSAPCode_PendingTargetPlanDetails' },
                        headers: customHeaders
                    },
                    validateTargetPlan: {
                        method: 'Post',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'ValidateTargetPlan' },
                        headers: customHeaders
                    },
                    validateTargetPlanV2: {
                        method: 'Post',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'ValidateTargetPlanV2' },
                        headers: customHeaders
                    },
                    checkIsSubmitPersonOfDepartment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'CheckIsSubmitPersonOfDepartment' },
                        headers: customHeaders
                    },
                    import: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'Import' },
                        headers: customHeaders
                    },
                    updatePermissionUserInTargetPlanItem: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'UpdatePermissionUserInTargetPlanItem' },
                        headers: customHeaders
                    },
                    cancelPendingTargetPLan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'CancelPendingTargetPLan' },
                        headers: customHeaders
                    },
                    getPermissionInfoOnPendingTargetPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetPermissionInfoOnPendingTargetPlan' },
                        headers: customHeaders
                    },
                    getPendingTargetPlans: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'GetPendingTargetPlans' },
                        headers: customHeaders
                    },
                    requestToChange: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'requestToChange' },
                        headers: customHeaders
                    },
                    printForm: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'PrintForm' },
                        headers: customHeaders
                    },
                    downloadTemplate: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'DownloadTemplate' },
                        headers: customHeaders
                    },
                    targetPlanReport: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'TargetPlanReport' },
                        headers: customHeaders
                    },
                    validateSubmitPendingTargetPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TargetPlan", action: 'ValidateSubmitPendingTargetPlan' },
                        headers: customHeaders
                    }
                })
            }
        }
        return {
            getInstance() {
                if (!instance) {
                    instance = new createInstance();
                }
                return instance;
            }
        }
    });