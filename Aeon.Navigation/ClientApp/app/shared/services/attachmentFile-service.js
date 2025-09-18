angular
    .module('ssg')
    .factory('attachmentService', function($resource, $window, appSetting, $rootScope, interceptorService) {
        var _ = $window._;
        var instance = null;

        function createInstance() {
            instance = new createfactoryService();
            return instance;
        }

        function createfactoryService() {
            var url = baseUrlApi + "/:controller/:action/:id";
            var customHeaders = buildCustomHeader();
            var customHeadersForUpload = buildCustomHeader();
            customHeadersForUpload['Content-Type'] = undefined;
            return {
                attachmentFile: $resource(url, null, {
                    upload: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'UploadFiles'
                        },
                        headers: customHeadersForUpload
                    },
                    download: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'Download'
                        },
                        headers: customHeaders
                    },
                    getFilePath: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'GetFilePath'
                        },
                        headers: customHeaders
                    },
                    get: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'Get'
                        },
                        headers: customHeaders
                    },
                    getImage: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'GetImage'
                        },
                        headers: customHeaders
                    },
                    delete: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'Delete'
                        },
                        headers: customHeaders
                    },
                    uploadImage: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'UploadImage'
                        },
                        headers: customHeadersForUpload
                    },
                    deleteMultiFile: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'DeleteMultiFile'
                        },
                        headers: customHeaders
                    },
                    uploadImageByType: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "AttachmentFile",
                            action: 'UploadImageByType'
                        },
                        headers: customHeadersForUpload
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