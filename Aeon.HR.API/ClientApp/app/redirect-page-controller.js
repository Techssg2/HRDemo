var ssgApp = angular.module('ssg.redirectPageModule', ["kendo.directives"]);
ssgApp.controller('redirectPageController', function ($rootScope, $scope, $location, $q, settingService, appSetting, localStorageService, $timeout, $state, $stateParams, $window) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = '404';
    $scope.isLoading = true;
    $scope.clearCache = function () {
        localStorageService.remove('currentUser');
        $timeout(function () {
            /*$state.go('home.dashboard');*/
            $state.go('home.navigation-home');
        }, 0);
    }

    function watch() {
        $rootScope.$watch('isLoading', function (newValue, oldValue) {
            kendo.ui.progress($("#loading"), true);
        });
    }
    this.$onInit = async function () {
        watch();
        $scope.isLoading = true;
        let responseUser = null;
        let responseDepartment = null;
        var one = $q.defer();
        var two = $q.defer();
        var all = $q.all([one.promise, two.promise]);
        all.then(allSuccess);

        function success(data) {
        }
        one.promise.then(success);
        two.promise.then(success);

        function allSuccess() {
            if (responseUser.object) {
                sessionStorage.setItem('currentUser', JSON.stringify(responseUser.object));
                if ($rootScope.redirectUrl) {
                    if ($rootScope.redirectUrl.includes('/index/redirectToPage') || $rootScope.redirectUrl === 'http://edoc.aeon.com.vn/_layouts/15/AeonHR/Default.aspx#!/home') {
                        $state.go('home.dashboard');
                        /*$state.go('home.navigation-home');*/
						//window.location.href = "ClientApp/UpgradeSystem.html";
						/*var password = prompt("Please enter the password:", "");
						if (password === "Net$1234") {
							$state.go('home.dashboard');
							// $window.location.href = $rootScope.redirectUrl;
						} else {
							window.location.href = "/HR/ClientApp/UpgradeSystem.html";
						}*/
                    } else {
                        $window.location.href = $rootScope.redirectUrl;
						//$window.location.href = 'http://edoc.aeon.com.vn/_layouts/15/AeonHR/Default.aspx#!/home/navigation-home';
						//window.location.href = "/ClientApp/UpgradeSystem.html";
						/*var password = prompt("Please enter the password:", "");
						if (password === "Net$1234") {
							$window.location.href = $rootScope.redirectUrl;
						} else {
							window.location.href = "/HR/ClientApp/UpgradeSystem.html";
						}*/
                    }
                }
            } else {
                $window.location.href = edoc1Url;
            }
            $scope.isLoading = false;
        }
        settingService.getInstance().users.getCurrentUser().$promise.then(result => {
            responseUser = result;
            one.resolve("one done");
        });
        sessionStorage.setItemWithSafe("departments", null);
        if (!sessionStorage.getItemWithSafe("departments")) {
            var arg = {
                predicate: "",
                predicateParameters: [],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
            settingService.getInstance().departments.getDepartmentByFilter(arg).$promise.then(result => {
                responseDepartment = result;
                sessionStorage.setItemWithSafe("departments", JSON.stringify(result.object.data), 1000000);
                two.resolve("two done");
            });
        } else {
            two.resolve("two done");
        }
    }

});

Storage.prototype.setItemWithSafe = function (name, value, iMaxLength) {
    var removeStorageItem = function (strge, txtName) {
        let counter = 0;
        while (strge.getItem(name + "_SAFE_STORAGE_" + counter) != null) {
            strge.removeItem(name + "_SAFE_STORAGE_" + counter);
            counter++;
        }
    }

    if (value != null && value != undefined && $.type(value) == "string") {
        var setStorageValue = function (name, value) {
            this.setItem(name, value);
        }

        function chunkSubstr(str, size) {
            const numChunks = Math.ceil(str.length / size)
            const chunks = new Array(numChunks)

            for (let i = 0, o = 0; i < numChunks; ++i, o += size) {
                chunks[i] = str.substr(o, size)
            }

            return chunks
        }
        var valueArray = chunkSubstr(value, iMaxLength);
        removeStorageItem(this, name);
        //for (var i = 0; i < valueArray.length; i++) {
        //    this.setItem(name + "_SAFE_STORAGE_" + i, valueArray[i]);
        //}

        $(document).data(name, value);
    }
    else {
        removeStorageItem(this, name);
        this.setItem(name, value);
        $(document).data(name, value);
    }
}

Storage.prototype.getItemWithSafe = function (name) {
    var returnValue = null;
    if (name != null && name != undefined && $.type(name) == "string") {
        //var startIndex = 0;
        // var allowContinous = true;
        //while (allowContinous) {
        //     var currentValue = this.getItem(name + "_SAFE_STORAGE_" + startIndex);
        ////     if (currentValue != null && currentValue != undefined && $.type(currentValue) == "string" && currentValue != "null") {
        //        if (currentValue.length > 0) {
        //             returnValue += currentValue;
        //             startIndex++;
        //         }
        //         else {
        //             allowContinous = false;
        //         }
        //     }
        //    else {
        //        allowContinous = false;
        //     }
        // }
        returnValue = $(document).data(name);
    }
    return returnValue;
}