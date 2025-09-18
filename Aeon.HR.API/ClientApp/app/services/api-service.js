angular
    .module('ssg')
    .factory('apiService', function ($resource, $window, appSetting, $rootScope, interceptorService) {
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
                apiEdoc2: $resource(url, null, {
                    getItemByReferenceNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "APIEdoc2", action: 'GetItemByReferenceNumber' },
                        headers: customHeaders
                    },
                    getAllBTA_API: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "APIEdoc2", action: 'GetAllBTA_API' },
                        headers: customHeaders
                    },
                    updateStatusBTA: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "APIEdoc2", action: 'UpdateStatusBTA' },
                        headers: customHeaders
                    },
                    getDepartmentTree_API: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "APIEdoc2", action: 'GetDepartmentTree_API' },
                        headers: customHeaders
                    },
                    getAllUser_API: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "APIEdoc2", action: 'GetAllUser_API' },
                        headers: customHeaders
                    },
                    getAllUser_APIV2: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "APIEdoc2", action: 'GetAllUser_APIV2' },
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