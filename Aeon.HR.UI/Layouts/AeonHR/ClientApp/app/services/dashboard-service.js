angular
    .module('ssg')
    .factory('dashboardService', function($resource, $window, appSetting, $rootScope, interceptorService) {
        var _ = $window._;
        var instance = null;

        function createInstance() {
            instance = new createfactoryService();
            return instance;
        }

        function createfactoryService() {
            var url = baseUrlApi + "/:controller/:action";
            var customHeaders = buildCustomHeader();
            return {
                dashboard: $resource(url, null, {
                    getMyItems: {
                        method: 'GET',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Dashboard", action: 'GetMyItems' },
                        headers: customHeaders
                    },
                    getEmployeeNodesByDepartment: {
                        method: 'GET',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Dashboard", action: 'GetEmployeeNodesByDepartment' },
                        headers: customHeaders
                    },
                    refreshDepartmentNodes: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Dashboard", action: 'RefreshDepartmentNodes' },
                        headers: customHeaders
                    },
                    addDepartment: {
                        method: 'GET',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Dashboard", action: 'InsertDepartment' },
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