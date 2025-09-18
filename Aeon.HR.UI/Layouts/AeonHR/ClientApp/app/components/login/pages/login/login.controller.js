angular
    .module('ssg.login.module', ["kendo.directives"])
    .controller('LoginController', function ($scope, $rootScope, $stateParams, $state, LoginService) {
        var AUTH_COOKIE = "auth-token";
        $rootScope.title = "SSG - Login";
        eraseCookie(AUTH_COOKIE);
        localStorage.removeItem('currentUser')

        $scope.formSubmit = function () {
            kendo.ui.progress($("#loading"), true);
            var model = {
                loginName: $scope.username,
                password: $scope.password
            }
            LoginService.login(model, function (data) {
                $rootScope.userName = $scope.username;
                createCookie(AUTH_COOKIE, data.token);
                window.location.href = "/default.html";
            }, function (err) {
                $scope.error = "Incorrect username/password !";
                kendo.ui.progress($("#loading"), false);
            })
        };
    });