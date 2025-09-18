var modules = [
    'ui.router',
    'pascalprecht.translate',
    'ngCookies',
    'excuteJQModule',
    'ssg.dashboardModule',
    'kendo.directives',
    'appModule',
    'underscore',
    'ssg.settingModules',
    'ssg.directiveItemIconModule',
    'ngResource',
    'moment-module',
    'constantModule',
    'attachmentfileModule',
    'ssg.historyModule',
    'ui-notification',
    'ssg.directiveUserModule',
    "ssg.filterModule",
    "LocalStorageModule",
    "ssg.errorModule",
    "ssg.redirectPageModule",
    "ssg.navigationModule",
    "ssg.navigationHomeModule",
    "ssg.directiveSubNavModule",
    "ssg.accessDeniedModule",
    "ssg.settingUserModule"
];
var ssgApp = angular.module("ssg", modules);
// ssgApp.provider('globalsetting', function () {
//     this.alertInfo = function(){
//         alert("Info");
//     }
//     this.$get = function () {       
//         return "globalsetting";
//     }
// });
ssgApp
    .config(function ($stateProvider, $urlRouterProvider, $locationProvider, commonData, localStorageServiceProvider, $translateProvider) {
        $translateProvider.useStaticFilesLoader({
            prefix: 'ClientApp/resources/',
            suffix: '.txt?v=' + edocV
        });
        sessionStorage.removeItem('currentUser');
        sessionStorage.removeItem('departments');
        if (sessionStorage.lang && sessionStorage.lang == 'vi_VN') {
            // Tell the module what language to use by default
            $translateProvider.preferredLanguage('vi_VN');
        } else {

            $translateProvider.preferredLanguage('en_US');
        }
        $stateProvider
            .state("home", {
                url: "/home",
                templateUrl: "ClientApp/app/app.html?v=" + edocV,
                controller: "appController",
                resolve: {
                    settingService: "settingService",
                    initData: function (settingService, localStorageService) {
                        if (!localStorageService.get('passTime')) {
                            var currentTime = new Date();
                            localStorageService.set('passTime', currentTime);
                            localStorageService.set('accessSystemTime', currentTime);
                            localStorageService.remove('isTimeOut');
                        }
                    }
                }
            })
            .state('home.navigation-home', { // Navigation home management
                url: '',
                templateUrl: 'ClientApp/app/components/navigation-home/navigation-view.html?v=' + edocV,
                controller: 'navigationHomeController'
            })
            .state("home.dashboard", {
                url: "/dashboard",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            .state("home.todo", {
                url: "/todo",
                templateUrl: "ClientApp/app/components/dashboard/dashboard-view.html?v=" + edocV,
                controller: "dashboardController"
            })
            .state("home.orgchart", {
                url: "/orgchart",
                templateUrl: "ClientApp/app/components/dashboard/orgchart/orgchart-view.html?v=" + edocV,
                controller: "orgChartController"
            })
            .state('notFoundPage', {
                url: '/page404',
                templateUrl: 'ClientApp/app/components/errors-page/not-found-page.html?v=' + edocV,
                controller: 'notPageFoundController',
                params: { action: { title: "404" } }
            })
            .state('redirectPage', { // reference number
                url: '/index/redirectToPage',
                templateUrl: 'ClientApp/app/redirect-page-view.html?v=' + edocV,
                controller: 'redirectPageController',
                params: { action: { title: "404" } }
            })

            .state('home.navigation-list', { // Navigation management
                url: '/navigation-list',
                templateUrl: 'ClientApp/app/components/setting/navigation/navigation-list.html?v=' + edocV,
                controller: 'navigationController'
            })

            .state('accessDeniedPage', {
                url: '/page401',
                templateUrl: 'ClientApp/app/components/errors-page/access-denied-page.html?v=' + edocV,
                controller: 'accessDeniedController',
                params: { action: { title: "401" } }
            })
            .state('home.user-setting', { // Setting
                url: '/user-setting',
                templateUrl: 'ClientApp/app/components/setting/users/user-view.html?v=' + edocV
            })
            .state('home.user-setting.user-profile', { // Setting
                url: '/user-profile/:referenceValue?',
                views: {
                    'userProfile@home.user-setting': {
                        templateUrl: 'ClientApp/app/components/setting/users/user-profile.html?v=' + edocV,
                        controller: 'settingUserController'
                    }
                },
                params: { action: {}, referenceValue: null }
            })
            .state('home.user-setting.user-list', { // Setting
                url: '/user-list',
                views: {
                    'userList@home.user-setting': {
                        templateUrl: 'ClientApp/app/components/setting/users/user-list.html?v=' + edocV,
                        controller: 'settingUserController'
                    }
                },
                params: { action: { title: 'users' } }
            })

        $urlRouterProvider.otherwise('/index/redirectToPage');
    }).run(function ($history, $state, $location, $transitions, $rootScope, appSetting, $stateParams, $timeout, $q, settingService, localStorageService) {
        $transitions.onStart({}, function () {
            $rootScope.redirectUrl = $location.$$absUrl;
            // dùng cho trường hợp click go back page khi đang mở các dialog popup của mấy button ở workflow
            if ($rootScope.confirmVoteDialog) {
                $rootScope.confirmVoteDialog.close();
            }
            // dùng cho trường hợp click go back page khi đang mở các dialog popup DELETE row
            if ($rootScope.confirmDialog) {
                $rootScope.confirmDialog.close();
            }
            // dùng cho trường hợp click go back page khi đang mở dialog chổ default asset item ben JobGrade
            if ($rootScope.defaultAssetDialog) {
                $rootScope.defaultAssetDialog.close();
            }

            //dùng cho trường hợp click go back page khi đang mở dialog applicant create
            if ($rootScope.confirmGrade) {
                $rootScope.confirmGrade.close();
            }
            // dùng cho truong hop popup add user bên overtime, shiftexchange
            if ($rootScope.confirmDialogAddItemsUser) {
                $rootScope.confirmDialogAddItemsUser.close();
            }

            //dung cho popup change password
            if ($rootScope.confirmDialogChangePassWord) {
                $rootScope.confirmDialogChangePassWord.close();
            }
            // employee Info bên position detail
            if ($rootScope.confirmDialogEmployeeInfo) {
                $rootScope.confirmDialogEmployeeInfo.close();
            }

            //processing stage
            if ($rootScope.confirmProcessing) {
                $rootScope.confirmProcessing.close();
            }   
            // cho popup re-assignee
            if ($rootScope.confirmDialogReassignee) {
                $rootScope.confirmDialogReassignee.close();
            }
            // cho popup report BTA
            if ($rootScope.confirmDialogReport) {
                $rootScope.confirmDialogReport.close();
            }
        });
        $transitions.onSuccess({}, function () {
            $history.push($state);
            var existMappingItem = _.find(appSetting.mappingStates, x => { return x.source === $state.current.name });
            if (existMappingItem) {
                $state.go(existMappingItem.destination, existMappingItem.params);
            } else {
                var isParentMenu = _.find(appSetting.parentMenus, x => { return x === $state.current.name });
                if (isParentMenu) {
                    $rootScope.isParentMenu = true;
                    if ($rootScope.currentUser) {
                        $rootScope.createPageComponent($state.current.name);
                    }
                } else {
                    $rootScope.isParentMenu = false;
                }
                $state.go($state.current.name);
            }
        });
        // $rootScope.$on('$locationChangeSuccess', function () {
        //     $rootScope.actualLocation = $location.path();
        // });
        // $rootScope.$watch(function () { return $location.path() }, function (newLocation, oldLocation) {

        // });

    });