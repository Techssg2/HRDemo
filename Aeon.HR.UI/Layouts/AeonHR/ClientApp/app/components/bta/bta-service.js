angular
    .module('ssg')
    .factory('btaService', function($resource, $window, appSetting, $rootScope, interceptorService) {
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
                    saveRoomOrganization: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'SaveRoomOrganization' },
                        headers: customHeaders
                    },
                    getItemById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetItemById' },
                        headers: customHeaders
                    },                     
                    getList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTA", action: 'GetList' },
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