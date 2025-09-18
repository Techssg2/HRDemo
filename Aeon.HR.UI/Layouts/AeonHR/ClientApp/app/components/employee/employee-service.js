angular
    .module('ssg')
    .factory('employeeService', function ($resource, $window, appSetting, $rootScope, interceptorService) {
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
                employee: $resource(url, null, {
                    getEmployees: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Employee", action: 'GetEmployees' }, headers: customHeaders
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