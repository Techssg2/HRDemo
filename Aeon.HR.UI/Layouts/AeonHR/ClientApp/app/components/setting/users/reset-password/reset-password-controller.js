var ssgApp = angular.module("ssg.resetpasswordModule", ["kendo.directives"]);
ssgApp.controller('resetpasswordController', function ($scope, settingService, $timeout, $rootScope) {
    $scope.errorMessages = '';
    $scope.isResetSuccess = false;
    $scope.isLoading = false;
    $scope.model = {
        email: '',
        userName: ''
    }

    $scope.confirm = async function () {
        if ($scope.model.userName && $scope.model.email) {
            kendo.ui.progress($("#loading-reset-rassword"), true);
            var result = await settingService.getInstance().users.forgotPassword({ username: $scope.model.userName, email: $scope.model.email }, {}).$promise;
            if (result && result.isSuccess) {
                $timeout(function () {
                    $scope.isResetSuccess = true;
                    $scope.errorMessages = 'A new password has been sent to your email. Please check your email to get new password/ Password mới đã được gửi vào mail của bạn. Vui lòng mở mail để lấy password mới.';
                    changeColorRequired('#c9c9c9');
                }, 0);
            } else {
                $timeout(function () {
                    $scope.errorMessages = 'Your account or email is not valid/Account hoặc email của bạn không tồn tại.';
                    changeColorRequired('#bf0000');
                });
            }
        } else {
            if (!$scope.model.email) {
                $scope.errorMessages = 'Email is required field/Thư điện tử là trường bắt buộc nhập'
            } else if (!$scope.model.userName) {
                $scope.errorMessages = 'Username is required field/Tên đăng nhập là trường bắt buộc nhập'
            }
        }
        kendo.ui.progress($("#loading-reset-rassword"), false);
    }

    function changeColorRequired(color) {
        for (let i = 0; i < document.getElementsByClassName("k-textbox").length; i++) {
            document.getElementsByClassName("k-textbox")[i].style.borderColor = color;
        }
    }
    $scope.ifEnter = function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            $scope.confirm();
        }
    }
    $scope.returnLoginPage = function () {
        window.href.location = baseUrl + "_layouts/";
    }
});