angular
    .module('ssg')
    .factory('permissionService', function($resource, $window, appSetting, $rootScope, interceptorService) {
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
                permission: $resource(url, null, {
                    getPerm: {
                        method: 'GET',
                        url: url,
                        cache:true,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Permission", action: 'GetPerm' },
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