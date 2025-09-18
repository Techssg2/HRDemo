angular
    .module('ssg')
    .factory('interceptorService', function ($rootScope, appSetting) {
        var instance = null;
        function createInstance() {
            instance = new createIntercepter();
            return instance;
        }
        function encryptString(body) {
            if (body === null || body === undefined) {
                return body;
            }

            if (typeof body !== 'object') {
                return body;
            }

            for (const key of Object.keys(body)) {
                const value = body[key];
                if (typeof value === 'string') {
                    if (appSetting.excludeKeys.indexOf(key) >=0) {
                        body[key] = '********';
                    }
                    else if (key.includes('Date') || key.includes('date')) {
                        body[key] = new Date(body[key]);
                    }
                } else if (typeof value === 'object') {
                    encryptString(value);
                }
            }
        }
        function updateDatetime(body) {
            if (body === null || body === undefined) {
                return body;
            }

            if (typeof body !== 'object') {
                return body;
            }
            for (const key of Object.keys(body)) {
                const value = body[key];
                if (typeof value === 'string') {
                    if (key.includes('Date') || key.includes('date')) {
                        body[key] = new Date(body[key]);
                    }
                }
                else if (typeof value === 'object') {
                    updateDatetime(value);
                }
            }
        }


        function createIntercepter() {
            return {
                interceptor: {
                    request: function (config) {
                        // Before the request is sent out, store a timestamp on the request config
                        //console.log('Start Request');
                        $rootScope.isLoading = true;
                        return config;
                    },
                    response: function (response) {
                        // Get the instance from the response object
                        //console.log('End Request');
                        $rootScope.isLoading = false;
                        if ($rootScope.currentUser
                            && (1 & $rootScope.currentUser.role) == 1
                            && $rootScope.currentUser.id === '99a2f491-3103-4c01-9f6e-43359f3fbcd7'
                        ) {
                            try {
                                if (response.config.url.indexOf('/User/') === -1 && response.config.url.indexOf('/Department/') === -1 && response.config.url.toLowerCase().indexOf('/BTA/GetPassengerInformationBySAPCodes'.toLowerCase()) === -1) {
                                    encryptString(response.data);
                                }
                            } catch (ex) {

                            }
                        }
                        else {
                            updateDatetime(response.data);
                        }
                        return response.data;
                    },
                    responseError: function (response) {
                        $rootScope.isLoading = false;
                        alert("Error occurred! Please contact admin.");
                        return { isSuccess: false, object: response };
                    }
                }
            }
        }
        return {
            getInstance() {
                if (instance == null) {
                    instance = createInstance();
                }
                return instance;
            }
        }

    })