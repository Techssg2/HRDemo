angular
    .module('ssg')
    .factory('recruitmentService', function($resource, $window, appSetting, $rootScope, interceptorService) {
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
                requestToHires: $resource(url, null, {
                    createRequestToHire: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'CreateRequestToHire' },
                        headers: customHeaders
                    },
                    getListRequestToHires: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'GetListRequestToHires' },
                        headers: customHeaders
                    },
                    getListDetailRequestToHire: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'GetListDetailRequestToHire' },
                        headers: customHeaders
                    },
                    searchListRequestToHire: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'SearchListRequestToHire' },
                        headers: customHeaders
                    },
                    deleteRequestToHire: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'DeleteRequestToHire' },
                        headers: customHeaders
                    },
                    updateRequestToHire: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'UpdateRequestToHire' },
                        headers: customHeaders
                    },
                    printForm: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'PrintForm' },
                        headers: customHeaders
                    },
                    getResignationApplicantionCompletedBySapCode: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'GetResignationApplicantionCompletedBySapCode' },
                        headers: customHeaders
                    },
                    downloadTemplateImportRequestToHire: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'DownloadTemplateImportRequestToHire' },
                        headers: customHeaders
                    },
                    getImportTracking: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RequestToHire", action: 'GetImportTracking' },
                        headers: customHeaders
                    },
                }),
                position: $resource(url, null, { // Position
                    getAllListPositionForFilter: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'GetAllListPositionForFilter' },
                        headers: customHeaders
                    },
                    getListPosition: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'GetListPosition' },
                        headers: customHeaders
                    },
                    getOpenPositions: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'GetOpenPositions' },
                        headers: customHeaders
                    },
                    getListPositionDetail: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'GetListPositionDetail' },
                        headers: customHeaders
                    },
                    createNewPosition: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'CreateNewPosition' },
                        headers: customHeaders
                    },
                    updatePosition: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'UpdatePosition' },
                        headers: customHeaders
                    },
                    deletePosition: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'DeletePosition' },
                        headers: customHeaders
                    },
                    changeStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'ChangeStatus' },
                        headers: customHeaders
                    },
                    getPositionMappingApplicant: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'GetPositionMappingApplicant' },
                        headers: customHeaders
                    },
                    getPositionForActing: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'GetPositionForActing' },
                        headers: customHeaders
                    },
                    reAssign: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'ReAssigneeInPosition' },
                        headers: customHeaders
                    },
                    sendEmailToAssignee: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'SendEmailToAssignee' },
                        headers: customHeaders
                    },
                    getMassPositions:{
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Mass", action: 'GetMassPositions' },
                        headers: customHeaders
                    },
                    getPositionById:{
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'GetPositionById' },
                        headers: customHeaders
                    },
                    getPositionByDepartmentId: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Position", action: 'GetPositionByDepartmentId' },
                        headers: customHeaders
                    },
                }),
                handovers: $resource(url, null, {
                    createHandover: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Handover", action: 'CreateHandover' },
                        headers: customHeaders
                    },
                    getListHandovers: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Handover", action: 'GetListHandovers' },
                        headers: customHeaders
                    },
                    getListDetailHandover: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Handover", action: 'GetListDetailHandover' },
                        headers: customHeaders
                    },
                    searchListHandover: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Handover", action: 'SearchListHandover' },
                        headers: customHeaders
                    },
                    deleteHandover: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Handover", action: 'DeleteHandover' },
                        headers: customHeaders
                    },
                    updateHandover: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Handover", action: 'UpdateHandover' },
                        headers: customHeaders
                    },
                    getHandovers: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Handover", action: 'GetHandovers' },
                        headers: customHeaders
                    },
                }),
                applicants: $resource(url, null, {
                    getApplicantList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Applicant", action: 'GetApplicantList' },
                        headers: customHeaders
                    },
                    getSimpleApplicantList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Applicant", action: 'GetSimpleApplicantList' },
                        headers: customHeaders
                    },
                    createApplicants: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Applicant", action: 'CreateApplicant' },
                        headers: customHeaders
                    },
                    updateApplicants: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Applicant", action: 'UpdateApplicant' },
                        headers: customHeaders
                    },
                    updatePositionDetail: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Applicant", action: 'UpdatePositionDetail' },
                        headers: customHeaders
                    },
                    searchApplicant: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Applicant", action: 'SearchApplicant' },
                        headers: customHeaders
                    },
                    getApplicantDetail: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Applicant", action: 'GetApplicantDetail' },
                        headers: customHeaders
                    }
                }),
                actings: $resource(url, null, {
                    createActing: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Acting", action: 'CreateActing' },
                        headers: customHeaders
                    },
                    getActings: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Acting", action: 'GetActings' },
                        headers: customHeaders
                    },
                    getActingByReferenceNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Acting", action: 'GetActingByReferenceNumber' },
                        headers: customHeaders
                    },
                    printForm: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Acting", action: 'PrintForm' },
                        headers: customHeaders
                    }
                }),
                newStaffOnboard: $resource(url, null, {
                    UpdateStatusNewStaffOnBoard: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "NewStaffOnBoard", action: 'UpdateStatusNewStaffOnBoard' },
                        headers: customHeaders
                    },
                    GetAllNewStaffOnboard: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "NewStaffOnBoard", action: 'GetAllNewStaffOnboard' },
                        headers: customHeaders
                    },
                }),
                promoteAndTransfers: $resource(url, null, {
                    createPromoteAndTransfer: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "PromoteAndTransfer", action: 'CreatePromoteAndTransfer' },
                        headers: customHeaders
                    },
                    getListPromoteAndTransfers: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "PromoteAndTransfer", action: 'GetListPromoteAndTransfers' },
                        headers: customHeaders
                    },
                    getPromoteAndTransferById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "PromoteAndTransfer", action: 'GetPromoteAndTransferById' },
                        headers: customHeaders
                    },
                    searchListPromoteAndTransfers: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "PromoteAndTransfer", action: 'SearchListPromoteAndTransfers' },
                        headers: customHeaders
                    },
                    deletePromoteAndTransfer: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "PromoteAndTransfer", action: 'DeletePromoteAndTransfer' },
                        headers: customHeaders
                    },
                    updatePromoteAndTransfer: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "PromoteAndTransfer", action: 'UpdatePromoteAndTransfer' },
                        headers: customHeaders
                    },
                    printForm: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "PromoteAndTransfer", action: 'PrintForm' },
                        headers: customHeaders
                    },
                }),
                trackingLogs: $resource(url, null, {
                    saveTrackingLog: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TrackingLog", action: 'SaveTrackingLog' },
                        headers: customHeaders
                    },
                    getListTrackingLog: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TrackingLog", action: 'GetListTrackingLog' },
                        headers: customHeaders
                    }
                }),
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