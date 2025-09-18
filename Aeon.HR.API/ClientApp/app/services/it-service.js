angular.module('ssg').factory('itService', function ($resource, $window, interceptorService) {
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
            it: $resource(url, null, {
                saveResignation: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "it", action: 'SaveResignation' },
                    headers: customHeaders
                },
                saveRequestToHire: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "it", action: 'SaveRequestToHire' },
                    headers: customHeaders
                },
                saveShiftExchange: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "it", action: 'SaveShiftExchange' },
                    headers: customHeaders
                },
                saveTargetPlan: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "it", action: 'SaveTargetPlan' },
                    headers: customHeaders
                },
                savePromoteAndTransfer: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "it", action: 'SavePromoteAndTransfer' },
                    headers: customHeaders
                },
                saveBTA: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "it", action: 'SaveBTA' },
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