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
                "$rootScope", "$scope", "settingService", "localStorageService", "$timeout", "$translate", "Notification",
                function ($rootScope, $scope, settingService, localStorageService, $timeout, $translate, Notification) {
                    $scope.vm = {
                        currentUser: $rootScope.currentUser, 
                        avatar: $rootScope.avatar                   
                    };
                    $scope.changePwdDialog = {};
                    $scope.vm.loginAsDifferentUser = function() {
                        // var curUrl = window.location.href;
                        // if (curUrl.indexOf("?") > -1) {
                        //     curUrl = curUrl.substr(0, curUrl.indexOf("?"));
                        // }
                        // window.location.href = _spPageContextInfo.webAbsoluteUrl +
                        //     "/_layouts/15/closeConnection.aspx?loginasanotheruser=true&source=" +
                        //     encodeURIComponent(curUrl);
                    }
					
					$scope.refresh = function () {
                        window.location.reload(true);
                    }
                    $scope.pwdModel = { errors: [] };
                    $scope.changePwdDialogOpts = {
                        width: "450px",
                        buttonLayout: "normal",
                        closable: true,
                        modal: true,
                        visible: false,
                        content: "",
                        actions: [{
                            text: $translate.instant('COMMON_BUTTON_OK'),
                            action: function (e) {
                                $scope.pwdModel.id = $rootScope.currentUser.id;
                                $scope.pwdModel.errors = [];
                                if (!$scope.pwdModel.oldPassword) {
                                    //$scope.pwdModel.errors.push("Current password is required")
                                    $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_CURRENT_VALIDATE'))
                                }
                                if (!$scope.pwdModel.newPassword) {
                                    // $scope.pwdModel.errors.push("New password is required")
                                    $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_NEW_PASSWORD_VALIDATE'))
                                }
                                if (!$scope.pwdModel.verifyPassword) {
                                    //$scope.pwdModel.errors.push("Verify password is required")
                                    $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_VERIFY_PASSWORD_VALIDATE'))
                                }
                                if ($scope.pwdModel.newPassword != $scope.pwdModel.verifyPassword) {
                                    //$scope.pwdModel.errors.push("New password and verify password is not the same")
                                    $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_NEW_VERIFY_VALIDATE'))
                                }
                                if ($scope.pwdModel.newPassword) {
                                    /*var matchedItems = $scope.pwdModel.newPassword.match(/(.{7,})/g);
                                    if (!matchedItems) {
                                        $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_CHARACTER_VALIDATE'))
                                    }*/
                                    var newPassword = $scope.pwdModel.newPassword;
                                    
                                }
                                if ($scope.pwdModel.errors.length == 0) {
                                    settingService.getInstance().users.changePassword($scope.pwdModel).$promise.then(function (result) {
                                        if (result.object) {
                                            Notification.success("Your password was changed successfully");
                                            $scope.changePwdDialog.close();
                                        } else {
                                            //$scope.pwdModel.errors.push("Your current password is not correct. Please try again")
                                            if (result.messages[0] != null) {
                                                $scope.pwdModel.errors.push($translate.instant(result.messages[0]));
                                            } else {
                                                $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_CURRENT_CORRECT_VALIDATE'))
                                            }
                                        }
                                    });
                                    $scope.$apply();
                                    return false;
                                } else {
                                    $scope.$apply();
                                    return false;
                                }
                            },
                            primary: true
                        }],
                        close: async function (e) {
                            $scope.pwdModel = { errors: [] }
                        }
                    };

                    $scope.changePassword = function () {
                        $scope.changePwdDialog.title($translate.instant('USER_PROFILE_CHANGE_PASS'));
                        $scope.changePwdDialog.open();
                        $rootScope.confirmDialogChangePassWord = $scope.changePwdDialog;
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
                            window.open(baseUrl + "sku/Account/Logout", '_blank');

                            setTimeout(function () {
                                window.location.href = "https://edocv2.aeon.com.vn/sso/logout.aspx?returnUrl=https://edocv2.aeon.com.vn/sku/Account/Logout?returnUrl=https://edocv2.aeon.com.vn/homev2/Account/Logouts";
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