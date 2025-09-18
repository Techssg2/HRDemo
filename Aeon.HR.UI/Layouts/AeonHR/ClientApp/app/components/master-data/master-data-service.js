angular
    .module('ssg')
    .factory('masterDataService', function ($resource, $window, appSetting, $rootScope, interceptorService) {
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
                masterData: $resource(url, null, {
                    GetMasterDataInfo: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "MasterData", action: 'GetMasterData' }, headers: customHeaders
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