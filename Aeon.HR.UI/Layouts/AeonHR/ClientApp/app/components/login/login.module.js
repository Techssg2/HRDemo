'use strict';
var app = angular.module("ssg", [
    //vendor modules
    'ui.router',
    'pascalprecht.translate',
    'ngCookies',
    'ngResource',
    'ssg.login.module'
]);

app.config(function ($stateProvider, $translateProvider) {
    //Remove all cache data stored in session storage
    sessionStorage.removeItem('currentUser');
    sessionStorage.removeItem('departments');

    var loginState = {
        name: "login",
        url: "",
        templateUrl: "ClientApp/app/components/login/pages/login/login.view.html?v=" + edocV,
        controller: "LoginController",
    }

    $stateProvider.state(loginState);
});
