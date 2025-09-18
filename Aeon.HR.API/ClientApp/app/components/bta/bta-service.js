angular
    .module('ssg')
    .factory('btaService', function ($resource, $window, appSetting, $rootScope, interceptorService) {
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
                bussinessTripApps: $resource(url, null, {
                    save: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'Save' },
                        headers: customHeaders
                    },
                    saveRevokeInfo: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveRevokeInfo' },
                        headers: customHeaders
                    },
                    saveCancellationFee_revoke: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveCancellationFee_Revoke' },
                        headers: customHeaders
                    },
                    saveCancellationFee_changing: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveCancellationFee_Changing' },
                        headers: customHeaders
                    },
                    saveRoomBookingFlight: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveRoomBookingFlight' },
                        headers: customHeaders
                    },
                    saveRoomOrganization: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveRoomOrganization' },
                        headers: customHeaders
                    },
                    getBtaRoomHotel: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetBtaRoomHotel' },
                        headers: customHeaders
                    },
                    getItemById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetItemById' },
                        headers: customHeaders
                    },
                    getItemOverBudgetById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OverBudget", action: 'GetItemOverBudgetById' },
                        headers: customHeaders
                    },
                    getList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetList' },
                        headers: customHeaders
                    },
                    getListOverBudget: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OverBudget", action: 'GetListOverBudget' },
                        headers: customHeaders
                    },
                    getReports: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetReports' },
                        headers: customHeaders
                    },
                    getDetailUsersInRoom: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetDetailUsersInRoom' },
                        headers: customHeaders
                    },
                    getEmployeeTripGroup: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetEmployeeTripGroup' },
                        headers: customHeaders
                    },
                    getTripOverBudgetGroups: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OverBudget", action: 'GetTripOverBudgetGroups' },
                        headers: customHeaders
                    },
                    getBtaListConfirm: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetBtaListConfirm' },
                        headers: customHeaders
                    },
                    saveConfirmBTADetail: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveConfirmBTADetail' },
                        headers: customHeaders
                    },
                    getPassengerInformationBySAPCodes: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetPassengerInformationBySAPCodes' },
                        headers: customHeaders
                    },
                    validate: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'Validate' },
                        headers: customHeaders
                    },
                    export: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'Export' },
                        headers: customHeaders
                    },
                    getRevokingBTA: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetRevokingBTA' },
                        headers: customHeaders
                    },
                    exportTypeStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'ExportTypeStatus' },
                        headers: customHeaders
                    },
                    searchFlight: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'SearchFlight' },
                        headers: customHeaders
                    },
                    fareRules: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'FareRules' },
                        headers: customHeaders
                    },
                    searchAirport: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'SearchAirport' },
                        headers: customHeaders
                    },
                    searchCountry: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'SearchCountry' },
                        headers: customHeaders
                    },
                    reValidateBooking: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'Revalidate' },
                        headers: customHeaders
                    },
                    draftBooking: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'DraftBooking' },
                        headers: customHeaders
                    },
                    addBookingTraveller: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'AddBookingTraveller' },
                        headers: customHeaders
                    },
                    savePassengerInfo: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SavePassengerInfo' },
                        headers: customHeaders
                    },
                    saveBookingInfo: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveBookingInfo' },
                        headers: customHeaders
                    },
                    saveBookingInfo: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveBookingInfo' },
                        headers: customHeaders
                    },
                    filterAvailability: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'FilterAvailability' },
                        headers: customHeaders
                    },
                    groupIntinerary: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'GroupIntinerary' },
                        headers: customHeaders
                    },
                    getUserTicketsInfo: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetUserTicketsInfo' },
                        headers: customHeaders
                    },
                    commitBooking: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'CommitBooking' },
                        headers: customHeaders
                    },
                    saveRequestOverBudget: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OverBudget", action: 'SaveRequestOverBudget' },
                        headers: customHeaders
                    },
                    saveBudget: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "OverBudget", action: 'SaveBudget' },
                        headers: customHeaders
                    },
                    deleteTripGroup: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'DeleteTripGroup' },
                        headers: customHeaders
                    },
                    getItemByReferenceNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetItemByReferenceNumber' },
                        headers: customHeaders
                    },
                    getAllBTA_API: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetAllBTA_API' },
                        headers: customHeaders
                    },
                    updateStatusBTA: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'UpdateStatusBTA' },
                        headers: customHeaders
                    },
                    checkBookingCompleted: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'CheckBookingCompleted' },
                        headers: customHeaders
                    },
                    checkAdminDept: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'CheckAdminDept' },
                        headers: customHeaders
                    },
                    saveBTADetail: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveBTADetail' },
                        headers: customHeaders
                    },
                    sendEmailDeleteRows: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SendEmailDeleteRows' },
                        headers: customHeaders
                    },
                    checkRoomHotel: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'CheckRoomHotel' },
                        headers: customHeaders
                    },
                    getFareRulesByFareSourceCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetFareRulesByFareSourceCode' },
                        headers: customHeaders
                    },
                    getBookingDetail: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "AirTicket", action: 'GetBookingDetail' },
                        headers: customHeaders
                    },
                    getBTAApprovedDay: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetBTAApprovedDay' },
                        headers: customHeaders
                    },
                    validatationBTADetails: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'ValidatationBTADetails' },
                        headers: customHeaders
                    }
                }),
                actings: $resource(url, null, {
                    printForm: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'PrintForm' },
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