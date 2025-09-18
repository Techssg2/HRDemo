angular.module('ssg').factory('navigationService', function ($resource, $window, appSetting, $rootScope, interceptorService) {
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
            navigation: $resource(url, null, {
                getListNavigation: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'GetListNavigation' },
                    headers: customHeaders
                },
                create: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'CreateNavigation' },
                    headers: customHeaders
                },
                update: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'UpdateNavigationById' },
                    headers: customHeaders
                },
                delete: {
                    method: 'GET',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'DeleteNavigationById' },
                    headers: customHeaders
                },
                updateImageNavigation: {
                    method: 'POST',
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'UpdateImageNavigation' },
                    headers: customHeaders
                },
                getAll: {
                    method: 'GET',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'GetAll' },
                    headers: customHeaders
                },
                getListNavigationByUserIdAndDepartmentId: {
                    method: 'GET',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'GetListNavigationByUserIdAndDepartmentId' },
                    headers: customHeaders
                },
                getListNavigationByUserIdAndDepartmentIdV2: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'GetListNavigationByUserIdAndDepartmentIdV2' },
                    headers: customHeaders
                },
                getChildNavigationByParentId: {
                    method: 'GET',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'GetChildNavigationByParentId' },
                    headers: customHeaders
                },
                getChildNavigationByType: {
                    method: 'GET',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "Navigation", action: 'GetChildNavigationByType' },
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