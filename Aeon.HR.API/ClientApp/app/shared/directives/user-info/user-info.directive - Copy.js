var ssgApp = angular.module('ssg.directiveUserModule', []);
ssgApp.directive("userInfo", [
    function($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/user-info/user-info.template.html?v=" + edocV,
            scope: {
                currentUser: "=",
                avatar: "=" 
            },
            link: function($scope, element, attr, modelCtrl) {},
            controller: [
                "$rootScope", "$scope", "settingService", "localStorageService","$timeout",
                function($rootScope, $scope, settingService, localStorageService, $timeout) {
                    $scope.vm = {
                        currentUser: $rootScope.currentUser, 
                        avatar: $rootScope.avatar                   
                    };
                    $scope.vm.loginAsDifferentUser = function() {
                        // var curUrl = window.location.href;
                        // if (curUrl.indexOf("?") > -1) {
                        //     curUrl = curUrl.substr(0, curUrl.indexOf("?"));
                        // }
                        // window.location.href = _spPageContextInfo.webAbsoluteUrl +
                        //     "/_layouts/15/closeConnection.aspx?loginasanotheruser=true&source=" +
                        //     encodeURIComponent(curUrl);
                    }
                    $scope.vm.signOut = function() {
                        //window.location.href = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/SignOut.aspx";
                        sessionStorage.removeItem('currentUser');
                        sessionStorage.removeItem('departments');          
                        $timeout(function () {
                            localStorageService.remove('passTime'); // Thời gian đã trôi qua từ lúc đăng nhập vào hệ thống
                            localStorageService.remove('accessSystemTime'); // Thời điểm truy cập vào hệ thống
                            localStorageService.remove('isShowTimeOutPopup'); // Dùng để kiếm tra các popup timeout ở session khác còn đang mở
                            sessionStorage.removeItem('inVisibleConfirmPopup'); // Popup time out trên session hiện tại có đang mở
                            localStorageService.remove('waitingForLogout'); 
                            window.open(baseUrl + "_layouts/closeConnection.aspx?loginasanotheruser=true", '_blank');

                            setTimeout(function () {

                                window.location.href = "http://edoc_l_fin.aeon.com.vn/_layouts/closeConnection.aspx?loginasanotheruser=true";

                            }, 500);
                        }, 0);
                    }

                    async function getImageUser() {
                        if ($rootScope.currentUser) {
                            var res = await settingService.getInstance().users.getImageUserById({ userId: $rootScope.currentUser.id }).$promise;
                            if (res && res.isSuccess) {
                                if (res.object && res.object.data && res.object.data.profilePicture) {
                                    $scope.avatar =  baseUrlApi.replace('/api', '') + res.object.data.profilePicture.trim();                                    
                                }
                                else {
                                    $scope.avatar = 'ClientApp/assets/images/avatar.png';
                                }
                            }
                        }
                    }

                    $scope.avatar = $rootScope.avatar;
                    this.$onInit = async () => {
                        $scope.avatar = $rootScope.avatar;
                        await getImageUser();
                    }
                },
            ],
        };
    },
]);