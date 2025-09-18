var ssgApp = angular.module('ssg.settingDaysConfigurationLogModule', ["kendo.directives"]);
ssgApp.controller('settingDaysConfigurationController', function ($rootScope, $q, $scope, $timeout, appSetting, $stateParams, $state,$translate, moment, commonData, Notification, settingService, $q, attachmentFile, ssgexService, fileService) {
    // create a message to display in our view
    var ssg = this;
    $scope.value = 0;
    var currentAction = null;
    isItem = true;
    $scope.title = $stateParams.action.title;

    $scope.total = 0;
    //$scope.daysConfigurations = [
    //    {
    //        no: 1,
    //        salaryPeriodFrom: 26,
    //        salaryPeriodTo: 25,
    //        deadlineOfSubmittingCABApplication: 28
    //    }
    //];
    $scope.daysConfigurations = [
        {
            no: 1
        }
    ];
    $scope.daysConfigurations[0]["salaryPeriodFrom"] = 26;
    $scope.daysConfigurations[0]["salaryPeriodTo"] = 26;
    $scope.daysConfigurations[0]["deadlineOfSubmittingCABApplication"] = 26;
    $scope.daysConfigurations[0]["createdNewPeriodDate"] = 10;
    $scope.numeric = {
        format: 1,
        min: 1,
        max: 30
    }

    $scope.daysConfigurationOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getDaysConfigurations(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.daysConfigurations }
            }
        },
        sortable: false,
        editable: {
            mode: "inline",
            confirmation: false
        },
        // pageable: {
        //     alwaysVisible: true,
        //     pageSizes: appSetting.pageSizesArray
        // },
        columns: [
            {
                field: "no",
                title: "NO.",
                width: "50px",
            },
            {
                field: "salaryPeriodFrom",
                title: "Salary Period From",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box id="salaryPeriodFrom" k-min="1" k-max="30" name="salaryPeriodFrom" k-format="'#,0'" k-ng-model="dataItem.salaryPeriodFrom" style="width: 100%;"/>`
                }
            },
            {
                field: "salaryPeriodTo",
                title: "Salary Period To",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box id="salaryPeriodTo" k-min="1" k-max="30" name="salaryPeriodTo" k-format="'#,0'" k-ng-model="dataItem.salaryPeriodTo" style="width: 100%;"/>`
                }
            },
            // {
            //     field: "newSalaryPeriod",
            //     title: "New Salary Period",
            //     width: "100px",
            //     template: function (dataItem) {
            //         return `<input kendo-numeric-text-box id="newSalaryPeriod" k-min="1" k-max="30" name="newSalaryPeriod" k-format="'#,0'" k-ng-model="dataItem.newSalaryPeriod" style="width: 100%;"/>`
            //     }
            // },
            {
                field: "deadlineOfSubmittingCABApplication",
                title: "Deadline of Submitting C&B Application",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box id="deadlineOfSubmittingCABApplication" k-min="1" k-max="30" name="deadlineOfSubmittingCABApplication" k-format="'#,0'" k-ng-model="dataItem.deadlineOfSubmittingCABApplication" style="width: 100%;"/>`
                }
            },
            {
                field: "createdNewPeriodDate",
                title: "Created New Period Date",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box id="createdNewPeriodDate" k-min="1" k-max="30" name="createdNewPeriodDate" k-format="'#,0'" k-ng-model="dataItem.createdNewPeriodDate" style="width: 100%;"/>`
                }
            },
            {
                title: "ACTIONS",
                width: "150px",
                template: function (dataItem) {
                    return `<a class="btn btn-sm default green-stripe " ng-click="save(dataItem)">Save</a>`
                }
            }
        ]
    };

    //CR321===========
    $scope.daysCABConfigurations = [
        {
            no: 1
        }
    ];
    $scope.daysCABConfigurations[0]["deadlineOfSubmittingCABHQ"] = 28;
    $scope.daysCABConfigurations[0]["deadlineOfSubmittingCABStore"] = 28;
    $scope.daysCABConfigurations[0]["timeOfSubmittingCABHQ"] = 0;
    $scope.daysCABConfigurations[0]["timeOfSubmittingCABStore"] = 0;

    $scope.daysConfigurationCABOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getDaysConfigurations(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.daysCABConfigurations }
            }
        },
        sortable: false,
        editable: {
            mode: "inline",
            confirmation: false
        },
        // pageable: {
        //     alwaysVisible: true,
        //     pageSizes: appSetting.pageSizesArray
        // },
        columns: [
            {
                field: "no",
                title: "NO.",
                width: "50px",
            },
            {
                field: "deadlineOfSubmittingCABHQ",
                title: "(HQ) Day of Deadline Submitting C&B",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box id="deadlineOfSubmittingCABHQ" k-min="1" k-max="30" name="deadlineOfSubmittingCABHQ" k-format="'#,0'" k-ng-model="dataItem.deadlineOfSubmittingCABHQ" style="width: 100%;"/>`
                }
            },
            {
                field: "deadlineOfSubmittingCABStore",
                title: "(Store) Day of Deadline Submitting C&B",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box id="deadlineOfSubmittingCABStore" k-min="1" k-max="30" name="deadlineOfSubmittingCABStore" k-format="'#,0'" k-ng-model="dataItem.deadlineOfSubmittingCABStore" style="width: 100%;"/>`
                }
            },
            {
                field: "timeOfSubmittingCABHQ",
                title: "(HQ) Time of Deadline Submitting C&B",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box id="timeOfSubmittingCABHQ" k-max="23" name="timeOfSubmittingCABHQ" k-format="'#,0'" k-ng-model="dataItem.timeOfSubmittingCABHQ" style="width: 100%;"/>`
                }
            },
            {
                field: "timeOfSubmittingCABStore",
                title: "(Store) Time of Deadline Submitting C&B",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box id="timeOfSubmittingCABStore" k-max="23" name="timeOfSubmittingCABStore" k-format="'#,0'" k-ng-model="dataItem.timeOfSubmittingCABStore" style="width: 100%;"/>`
                }
            },
            {
                title: "ACTIONS",
                width: "150px",
                template: function (dataItem) {
                    return `<a class="btn btn-sm default green-stripe " ng-click="saveConfigurationCAB(dataItem)">Save</a>`
                }
            }
        ]
    };
    //==========

    $scope.timeConfigurationOptions = {
        dataSource: {
            pageSize: appSetting.pageSizeDefault,
            serverPaging: true,
            transport: {
                read: async function (e) {
                    await getTimeConfigurations(e);
                }
            },
            schema: {
                total: () => { return $scope.timeConfigurations.length },
                data: () => { return $scope.timeConfigurations }
            }
        },
        sortable: false,
        autoBind: true,
        valuePrimitive: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray
        },
        columns: [
            {
                field: "no",
                title: "NO.",
                width: "50px",
            },
            {
                field: "name",
                title: "Name",
                width: "100px",
                template: function (dataItem) {
                    return `<label>{{dataItem.name}}</label>`
                }
            },
            {
                field: "code",
                title: "Value",
                width: "100px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box k-min="1" k-max="60" k-format="'#,0'" k-ng-model="dataItem.code" style="width: 100%;"/>`
                }
            },
            {
                title: "ACTIONS",
                width: "150px",
                template: function (dataItem) {
                    return `<a class="btn btn-sm default green-stripe " ng-click="updateConfiguration(dataItem)">Save</a>`
                }
            }
        ]
    };

    $scope.save = async function (dataItem) {
        $scope.dayConfigurationType = "General";
        let model = {
            id: dataItem.id,
            salaryPeriodFrom: dataItem["salaryPeriodFrom"],
            salaryPeriodTo: dataItem["salaryPeriodTo"],
            deadlineOfSubmittingCABApplication: dataItem["deadlineOfSubmittingCABApplication"],
            createdNewPeriodDate: dataItem["createdNewPeriodDate"],
            dayConfigurationType: $scope.dayConfigurationType
        }
        let result = await settingService.getInstance().salaryDayConfiguration.saveSalaryDayConfiguration(model).$promise;
        if(result.isSuccess) {
            Notification.success('Data Successfully Saved');
        }
        else
            Notification.error('Cannot Save. Something wrong!');
    }
    //CR321============
    $scope.dayConfigurationType = "";
    $scope.saveConfigurationCAB = async function (dataItem) {
        $scope.dayConfigurationType = "CAB";
        if ((dataItem["timeOfSubmittingCABHQ"] > 23 || dataItem["timeOfSubmittingCABHQ"] < 0) ||
            (dataItem["timeOfSubmittingCABStore"] > 23 || dataItem["timeOfSubmittingCABStore"] < 0)) {
            Notification.error('The Value Of Time Deadline C&B does not correct format.');
            return;
        }
        let model = {
            id: dataItem.id,
            deadlineOfSubmittingCABHQ: dataItem["deadlineOfSubmittingCABHQ"],
            deadlineOfSubmittingCABStore: dataItem["deadlineOfSubmittingCABStore"],
            timeOfSubmittingCABHQ: dataItem["timeOfSubmittingCABHQ"],
            timeOfSubmittingCABStore: dataItem["timeOfSubmittingCABStore"],
            dayConfigurationType: $scope.dayConfigurationType
        }
        let result = await settingService.getInstance().salaryDayConfiguration.saveSalaryDayConfiguration(model).$promise;
        if (result.isSuccess) {
            Notification.success('Data Successfully Saved');
        }
        else
            Notification.error('Cannot Save. Something wrong!');
    }
    //================
    $scope.updateConfiguration = async function(model) {
        if(!model.code){
            Notification.error("Code : " + $translate.instant('COMMON_FIELD_IS_REQUIRED'));
            return false;
        }
        if (model.id) {
            let args = {
                id: model.id,
                code:model.code
            };
            const result = await settingService.getInstance().timeConfiguration.updateConfiguration(args).$promise;
            if (result.isSuccess){
                Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
            } else {
                if (result.messages.length) {
                    Notification.error(result.messages[0]);
                } else {
                    Notification.error("Error");
                }
            }
        }
      
    }
    async function getDaysConfigurations(option) {
        let model = {
            predicate: "",
            predicateParameters: [],
            order: "Modified desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };

        var result = await settingService.getInstance().salaryDayConfiguration.getSalaryDayConfigurations(model).$promise;
        if (result.isSuccess) {
            $scope.daysConfigurations = [{ no: 1 }];
            result.object.data.forEach(el => {
                if (el.name) {
                    $scope.daysConfigurations[0][el.name.charAt(0).toLowerCase() + el.name.substring(1)] = el.value;   
                    $scope.daysCABConfigurations[0][el.name.charAt(0).toLowerCase() + el.name.substring(1)] = el.value;     
                }
            });
            //$scope.daysConfigurations = result.object.data;
            //$scope.daysConfigurations.forEach(item => {
            //    item['no'] = 1;
            //});
        }
        option.success($scope.daysConfigurations)
    }
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };
    async function getTimeConfigurations(option){
        let args = {
            queryArgs: $scope.currentQuery,
            type: commonData.reasonType.TIME_CONFIGURATION,
        }
        var result = await settingService.getInstance().timeConfiguration.getTimeConfigurations(args).$promise;
        if(result.isSuccess){
            $scope.timeConfigurations = result.object.data;
            let count = 1;
            $scope.timeConfigurations.forEach(item => {
                item['no'] = count;
                count++;
            });
        }
        option.success($scope.timeConfigurations);
    }
    //Booking Contract
    $scope.bookingContract = [];
    async function getBookingContract(option) {
        let model = {
            predicate: "",
            predicateParameters: [],
            order: "Modified desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        var result = await settingService.getInstance().bookingContract.getBookingContract(model).$promise;
        if (result.isSuccess) {
            $scope.bookingContract = result.object.data;
            let count = 1;
            $scope.bookingContract.forEach(item => {
                item['no'] = count;
                count++;
            });
        }
        option.success($scope.bookingContract);
    }
    $scope.bookingContractOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getBookingContract(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.bookingContract }
            }
        },
        sortable: false,
        editable: {
            mode: "inline",
            confirmation: false
        },
        columns: [
            {
                field: "no",
                title: "NO.",
                width: "50px",
            },
            {
                field: "fullName",
                title: "Full Name",
                width: "100px",
                template: function (dataItem) {
                    return `<input class="k-textbox w100" name="fullName" ng-model="dataItem.fullName" style="width: 100%;"/>`
                }
            },
            {
                field: "emailBookingContract",
                title: "Email",
                width: "100px",
                template: function (dataItem) {
                    return `<input class="k-textbox w100" name="emailBookingContract" ng-model="dataItem.emailBookingContract"/>`;
                }
            },
            {
                field: "phoneNumber",
                title: "Phone Number",
                width: "100px",
                template: function (dataItem) {
                    return `<input class="k-textbox w100" name="phoneNumber" ng-model="dataItem.phoneNumber" style="width: 100%;"/>`
                }
            },
            {
                title: "ACTIONS",
                width: "150px",
                template: function (dataItem) {
                    return `<a class="btn btn-sm default green-stripe " ng-click="saveBookingContract(dataItem)">Save</a>`
                }
            }
        ]
    };
    let requiredFields = [
        {
            fieldName: "fullName",
            title: "Full Name"
        },
        {
            fieldName: "emailBookingContract",
            title: "Email"
        },
        {
            fieldName: "phoneNumber",
            title: "Phone Number"
        }
    ];
    function validateEmail(email) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }
    function validatePhoneNumber(number) {
        var regex = /^\d{10}$/;
        return regex.test(number);
    }
    $scope.saveBookingContract = async function (model) {
        var hasError = false;
        let errors = $rootScope.validateInRecruitment(requiredFields, model);
        if (errors.length > 0) {
            let errorList = errors.map(x => {
                return x.controlName + " " + x.errorDetail;
            });
            if (errorList.length > 0) {
                hasError = true;
                Notification.error(`Some fields are required: </br>
                    <ul>${errorList.join('<br/>')}</ul>`);
                return;
            }
        }
        if (!validateEmail(model.emailBookingContract)) {
            Notification.error(`Email not correct format.`);
            return;
        }
        if (!validatePhoneNumber(model.phoneNumber)) {
            Notification.error(`Phone number not correct format.`);
            return;
        }
        if (!hasError) {
            settingService.getInstance().bookingContract.updateBookingContract({ Id: model.id, FullName: model.fullName, EmailBookingContract: model.emailBookingContract, PhoneNumber: model.phoneNumber }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Saved");
                    return true;
                }
            });
        }
    }

});