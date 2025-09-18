angular
    .module('ssg')
    .factory('ssgexService', function($resource, $window, appSetting, $rootScope, interceptorService) {
        var _ = $window._;
        var instance = null;

        function createInstance() {
            instance = new createfactoryService();
            return instance;
        }

        function createfactoryService() {
            var url = baseUrlApi + "/:controller/:action/";
            var customHeaders = buildCustomHeader();
            return {
                applicants: $resource(url, null, {
                    submitApplicant: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Applicant", action: 'SubmitApplicant' },
                        headers: customHeaders
                    },
                }),
                trackingLogs: $resource(url, null, {
                    retry: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SSGEx", action: 'Retry' },
                        headers: customHeaders
                    },
                }),
                remoteDatas: $resource(url, null, {
                    getLeaveBalanceSet: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SSGEx", action: 'GetLeaveBalanceSet' },
                        headers: customHeaders
                    },
                    getShiftSetByDate: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SSGEx", action: 'GetShiftSetByDate' },
                        headers: customHeaders
                    },
                    getShiftSetArrayByDate: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SSGEx", action: 'GetShiftSetArrayByDate' },
                        headers: customHeaders
                    },
                }),
                sapData: $resource(url, null, {
                    submitData: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SSGEx", action: 'Test' },
                        headers: customHeaders
                    },
                }),
                edoc1: $resource(url, null, {
                    getTasks: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SSGEx", action: 'GetTasks' },
                        headers: customHeaders
                    },
                    createF2Form: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SSGEx", action: 'CreateF2Form' },
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