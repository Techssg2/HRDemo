angular.module('ssg').factory('trackingHistoryService', function ($resource, $window, interceptorService) {
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
            trackingHistory: $resource(url, null, {
                getTrackingHistoryByItemId: {
                    method: 'GET',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "TrackingHistory", action: 'GetTrackingHistoryByItemId' },
                    headers: customHeaders
                },
                saveTrackingHistory: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "TrackingHistory", action: 'SaveTrackingHistory' },
                    headers: customHeaders
                },
                getTrackingHistoryById: {
                    method: 'GET',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "TrackingHistory", action: 'GetTrackingHistoryById' },
                    headers: customHeaders
                },
                getTrackingHistoryByTypeAndItemType: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "TrackingHistory", action: 'GetTrackingHistoryByTypeAndItemType' },
                    headers: customHeaders
                }
            }),
            trackingSyncOrgchartHistory: $resource(url, null, {
                getTrackingUserDepartmentsRequest: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "TrackingSyncOrgchart", action: 'GetTrackingUserDepartmentsRequest' },
                    headers: customHeaders
                },
                getTrackingUsersLogRequest: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "TrackingSyncOrgchart", action: 'GetTrackingUsersLogRequest' },
                    headers: customHeaders
                },
                getTrackingDepartmentsLogRequest: {
                    method: 'POST',
                    url: url,
                    interceptor: interceptorService.getInstance().interceptor,
                    params: { controller: "TrackingSyncOrgchart", action: 'GetTrackingDepartmentsLogRequest' },
                    headers: customHeaders
                },
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