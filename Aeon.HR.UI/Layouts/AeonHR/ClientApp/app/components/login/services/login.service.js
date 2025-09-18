angular
    .module('ssg.login.module')
    .factory('LoginService', function ($resource) {
        return $resource(null, null, {
            login: {
                method: 'POST',
                url: baseUrlApi + "/Account/Login"
            }
        });
    });