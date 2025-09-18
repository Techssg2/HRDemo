var ssgApp = angular.module('ssg.settingUserModule', ['kendo.directives']);
ssgApp.controller('settingUserController', function ($rootScope, $scope, $location, $translate, appSetting, Notification, commonData, $stateParams, $state, settingService, cbService, fileService, attachmentService, $timeout, ssgexService, $history) {
    // create a message to display in our view
    var ssg = this;
    $scope.files = [];
    $scope.test = "1";
    $scope.total = 0;
    $scope.userSelected = [];
    $scope.testBase64 = '';
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }
    $scope.avatar = '';
    $scope.avatarDefault = 'ClientApp/assets/images/avatar.png';
    $scope.baseUrlApi_test = 'abc/api';

    $scope.checkuser = 0;
    var userId = $stateParams.referenceValue;
    var leaveBalances = [];
    this.$onInit = async () => {
        $scope.data = [];
        $scope.userGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getUsers(e);
                    }
                },
                schema: {
                    total: () => { return $scope.total },
                    data: () => { return $scope.data }
                }
            },
            sortable: false,
            autoBind: true,
            valuePrimitive: false,
            editable: {
                mode: "inline",
                confirmation: false
            },
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArray
            },
            columns: [{
                field: "no",
                title: "No.",
                width: "80px"
            }, {
                field: "loginName",
                title: "Login Name",
                template: function (dataItem) {
                    if (dataItem.selected || !dataItem.id) {
                        return `<input class="k-textbox w100" name="sapCode" ng-model="dataItem.loginName"/>`;
                    } else {
                        if (dataItem.id) {
                            return `<a ui-sref="home.user-setting.user-profile({referenceValue: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">{{dataItem.loginName}}</a>`

                        } else {
                            return '';
                        }
                    }
                }
            },
            {
                field: "sapCode",
                title: "Employee Code",
                template: function (dataItem) {
                    if (dataItem.selected || !dataItem.id) {
                        return `<input class="k-textbox w100" name="sapCode" ng-model="dataItem.sapCode"/>`;
                    } else {
                        return `<span>{{dataItem.sapCode}}</span>`
                    }
                }
            }, {
                field: "fullName",
                title: 'Employee Name',
                template: function (dataItem) {
                    if (dataItem.selected || !dataItem.id) {
                        return `<input class="k-textbox w100" name="sapCode" ng-model="dataItem.fullName"/>`;
                    } else {
                        return `<span>{{dataItem.fullName}}</span>`
                    }
                }
            }, {
                field: "email",
                title: 'Email',
                template: function (dataItem) {
                    if (dataItem.selected || !dataItem.id) {
                        return `<input class="k-textbox w100" name="email" ng-model="dataItem.email"/>`;
                    } else {
                        return `<span>{{dataItem.email}}</span>`
                    }
                }
            },
            {
                field: "type",
                title: "Login Type",
                template: function (dataItem) {
                    if (!dataItem.id) {
                        return `<select kendo-drop-down-list style="width: 120px"
                            data-k-ng-model="dataItem.type"
                            k-data-text-field="'name'"
                            k-data-value-field="'code'"
                            k-auto-bind="'false'"
                            k-value-primitive="'false'"
                            k-data-source="[{'code':'0','name':'AD'}, {'code':'1','name':'MS'}]"
                            > </select>`
                    } else {
                        if (dataItem.selected && ($rootScope.currentUser.sapCode == '00313739' || $rootScope.currentUser.sapCode == '00406121')) {
                            return `<select kendo-drop-down-list style="width: 120px"
                            data-k-ng-model="dataItem.type"
                            k-data-text-field="'name'"
                            k-data-value-field="'code'"
                            k-auto-bind="'false'"
                            k-value-primitive="'false'"
                            k-data-source="[{'code':'0','name':'AD'}, {'code':'1','name':'MS'}]"
                            > </select>`
                        }
                        else {
                            return `<span>{{dataItem.type == 0 ?'AD':'MS'}}</span>`
                        }
                    }
                }
            },
            {
                field: "Module Permission",
                template: (dataItem) => customTemplate(dataItem, 'role', 'dropdown')
            },
            {

                field: "profilePicture",
                title: "Profile Picture",
                template: function (dataItem) {

                    if (dataItem.profilePicture) {
                        let fullImageLink = baseUrlApi.replace('/api', dataItem.profilePicture.trim());
                        return `
                                <div class='container-image'>
                                <img src="${fullImageLink}" alt='Avatar' class='image'>
                                <div class='overlay' ng-click='uploadImage(dataItem, $event)'>
                                <a class='icon' title='Upload photo'> <i class='fa fa-camera'></i></a></div></div>`
                    } else {
                        return `
                                <div class='container-image'>
                                <img src="ClientApp/assets/images/avatar.png" alt='Avatar' class='image'>
                                <div class='overlay' ng-click='uploadImage(dataItem, $event)'>
                                <a class='icon' title='Upload photo'> <i class='fa fa-camera'></i></a></div></div>`
                    }
                },

            },
            {
                field: "Actions",
                width: "260px",
                template: function (dataItem) {
                    if (!dataItem.no) {
                        return `<a class="btn btn-sm btn-primary" ng-click="excuteAction('create',dataItem)"><i class="fa fa-plus right-5"></i>Create</a>`
                    } else {
                        if (dataItem.selected) {
                            if ($scope.checkuser == 0) {
                                return `<a ng-click="lockUser('unLockUser',dataItem)"  class='btn btn-sm default green-stripe' >UnLock</a>
                                    <a ng-click="excuteAction('save',dataItem, $event)"  class='btn btn-sm default green-stripe' >Save</a>
                                    <a ng-click="excuteAction('cancel',dataItem)"  class='btn btn-sm default' >Cancel</a>`
                            } else {
                                return `
                                    <a ng-click="lockUser('lockUser',dataItem)"  class='btn btn-sm default green-stripe' >Lock</a>
                                    <a ng-click="excuteAction('save',dataItem, $event)"  class='btn btn-sm default green-stripe' >Save</a>
                                    <a ng-click="excuteAction('cancel',dataItem)"  class='btn btn-sm default' >Cancel</a>`
                            }
                        }
                        if (!dataItem.selected) {
                            return `<a ng-click="excuteAction('edit',dataItem)" class='btn btn-sm default blue-stripe'>Edit</a>
                                <a ng-if="${dataItem.type != 0 && dataItem.isActivated}" ng-click="excuteAction('resetPassword',dataItem)" class='btn btn-sm default green-stripe' >Reset Password</a>
                                <a ng-show="${dataItem.isActivated}"  ng-click="excuteAction('statusChange',dataItem)" class='btn btn-sm default red-stripe' >InActive</a>
                                <a ng-show="${!dataItem.isActivated}"  ng-click="excuteAction('statusChange',dataItem)" class='btn btn-sm default red-stripe' >Active</a>                                                                
                                `
                        }
                    }
                }
            }
            ],
            dataBound: function (e) {
                var rows = e.sender.tbody.children();
                for (var j = 0; j < rows.length; j++) {
                    var row = $(rows[j]);
                    var dataItem = e.sender.dataItem(row);

                    var inactive = dataItem.get("isActivated");
                    if (!inactive) {
                        row.addClass("inactive-row");
                    }

                    //   var cell = row.children().eq(columnIndex);
                    //   cell.addClass(getUnitsInStockClass(units));
                }
                // console.log(rows);
            },
            selectable: false
        }
        await initData();
        await getImageUser();
        if ($state.current.name === 'home.user-setting.user-profile') {
            if (userId) {
                await getUserProfileDataById();
            }
        } else {
            //get list
        }
    }
    //sort array
    function SortByDate(a, b) {
        var aDate = a.date;
        var bDate = b.date;
        return ((aDate > bDate) ? -1 : ((aDate < bDate) ? 1 : 0));
    }
    function SortByFromDate(a, b) {
        var aDate = a.fromDate;
        var bDate = b.fromDate;
        return ((aDate > bDate) ? -1 : ((aDate < bDate) ? 1 : 0));
    }
    function SortByShiftExchangeDate(a, b) {
        var aDate = a.shiftExchangeDate;
        var bDate = b.shiftExchangeDate;
        return ((aDate > bDate) ? -1 : ((aDate < bDate) ? 1 : 0));
    }
    async function getUserProfileDataById() {
        if (userId) { //View Profile
            let userProfile = await settingService.getInstance().users.getUserProfileCustomById({ userId: userId,isActivated:false }).$promise;
            if (userProfile.isSuccess && userProfile.object) {
                $scope.userSapCode = userProfile.object.sapCode;
                //get data leave balance
                await getLeaveBalance();
                // goi cai ham lien quan 4 bảng
                $timeout(function () {
                    $scope.model.personal = userProfile.object;
                    console.log($scope.model.personal);
                    if ($scope.model.personal && $scope.model.personal.profilePicture) {
                        $scope.model.personal.profilePicture = baseUrlApi.replace('/api', '') + $scope.model.personal.profilePicture.trim();
                    }
                    //$scope.model.personal.department = $scope.model.personal.divisionName ? $scope.model.personal.divisionName : $scope.model.personal.deptName;
                    $scope.model.personal.department = $scope.model.personal.divisionName && $scope.model.personal.divisionCode ? $scope.model.personal.divisionCode + '-' + $scope.model.personal.divisionName : $scope.model.personal.deptCode + '-' + $scope.model.personal.deptName;
                    if (userProfile.object.leaveApplications) {
                        userProfile.object.leaveApplications.sort(SortByFromDate);
                        setDataSourceGrid('leave', userProfile.object.leaveApplications, userProfile.object.leaveApplications.length);
                    }
                    if (userProfile.object.missingTimeClocks) {
                        let missingTimeClocks = userProfile.object.missingTimeClocks;
                        $scope.missingTimeClockData = [];
                        missingTimeClocks.forEach(element => {
                            let listReasons = JSON.parse(element.listReason);
                            listReasons.forEach(ele => {
                                ele.id = element.id;
                                ele.status = element.status;
                                ele.referenceNumber = element.referenceNumber;
                            });
                            $scope.missingTimeClockData = $scope.missingTimeClockData.concat(listReasons);
                        });
                        $scope.missingTimeClockData.sort(SortByDate);
                        setDataSourceGrid('missingTimeclock', $scope.missingTimeClockData, $scope.missingTimeClockData.length);
                    }
                    if (userProfile.object.overtimeApplications) { 
                        userProfile.object.overtimeApplications.sort(SortByDate);
                        setDataSourceGrid('overtime', userProfile.object.overtimeApplications, userProfile.object.overtimeApplications.length);
                    }
                    if (userProfile.object.shiftExchangeApplications) {
                        userProfile.object.shiftExchangeApplications.sort(SortByShiftExchangeDate);
                        setDataSourceGrid('shiftExchange', userProfile.object.shiftExchangeApplications, userProfile.object.shiftExchangeApplications.length);
                    }
                }, 0);
            }
        }
    }

    async function getLeaveBalance() {
        if ($scope.userSapCode) {
            var res = await ssgexService.getInstance().remoteDatas.getLeaveBalanceSet({ sapCode: $scope.userSapCode }, null).$promise;
            if (res.isSuccess && res.object) {
                leaveBalances = res.object;
            } else {
                leaveBalances = [];
            }
            setLeaveBalance(leaveBalances);
        }
    }

    function setLeaveBalance(data) {
        let grid = $("#gridLeaveTitle").data("kendoGrid");
        let dataSourceLeaveManagement = new kendo.data.DataSource({
            data: data,
        });
        grid.setDataSource(dataSourceLeaveManagement);
    }

    function setDataSourceGrid(title, data, total) {
        if (title == 'leave') {
            var dataSource = new kendo.data.DataSource({
                data: data,
                pageSize: appSetting.pageSizeDefault,
                //page: pageIndex,
                schema: {
                    total: function () {
                        return total;
                    }
                },
            });
            var grid = $("#leaveInformationGrid").data("kendoGrid");
        }
        if (title == 'missingTimeclock') {
            var dataSource = new kendo.data.DataSource({
                data: data,
                pageSize: appSetting.pageSizeDefault,
                //page: pageIndex,
                schema: {
                    total: function () {
                        return total;
                    }
                },
            });
            var grid = $("#missingTimeclockApplicantGrid").data("kendoGrid");
        }
        if (title == 'overtime') {
            var dataSource = new kendo.data.DataSource({
                data: data,
                pageSize: appSetting.pageSizeDefault,
                //page: pageIndex,
                schema: {
                    total: function () {
                        return total;
                    }
                },
            });
            var grid = $("#overtimeApplicantGrid").data("kendoGrid");
        }
        if (title == 'shiftExchange') {
            var dataSource = new kendo.data.DataSource({
                data: data,
                pageSize: appSetting.pageSizeDefault,
                //page: pageIndex,
                schema: {
                    total: function () {
                        return total;
                    }
                },
            });
            var grid = $("#shiftExchangeApplicantGrid").data("kendoGrid");
        }
        grid.setDataSource(dataSource);
    }

    let viewStateOvertime = "home.overtimeApplication.item";
    let viewStateLeave = "home.leavesManagement.item";
    let viewStateML = "home.missingTimelock.item";
    async function initData() {
        if ($stateParams.action) {
            $scope.title = 'Users';
        }
        ssg.requiredValueFields = [
            { fieldName: 'loginName', title: "Login Name" },
            { fieldName: 'sapCode', title: "Employee Code" },
            { fieldName: 'fullName', title: "Employee Name" },
            { fieldName: 'email', title: "Email" },
            { fieldName: 'type', title: "Login Type" },
        ];
        ssg.requiredArrayFields = [];
    };

    $scope.model = {
        personal: {
        }
    }

    $scope.uploadImage = function (dataItem, event) {

        if (!$scope.userSelected.length) {
            $scope.userSelected.push({ item: dataItem });
        }
        $("#files").click();
        var res = $("#files").kendoUpload({
            multiple: false,
            showFileList: true,
            autoUpload: false,
            select: async (e) => {
                let uploadResult = await uploadAction(e.files[0], dataItem.id, dataItem.profilePictureId);
            },
            validation: {
                allowedExtensions: [".jpg", ".png"]
            }
        }).getKendoUpload();
    }

    async function onSuccess(e) {
        const blob = new Blob([e.files[0].rawFile], { type: e.files[0].rawFile.type });
        var base64Value = await getBase64(blob);
        $scope.testBase64 = base64Value;
        //$scope.data[2].profilePicture = base64Value;
        $scope.data.push({ no: '', loginName: '', fullName: '', sapCode: '', mail: '', profilePicture: base64Value, jobTile: '', permissions: [], isActivated: true })

        $scope.userSelected[0].profilePicture = base64Value;
        return base64Value;
    }

    $scope.uploadImages = function () {
        alert('uploadImages');
    }
    $scope.onChange = function (data) {
        console.log(data);
    }

    //lamnl: 20/01/2022
    $scope.lockUser = async function (action, dataItem) {
        var grid = $("#grid").data("kendoGrid");
        var role = 0;
        for (var i = 0; i < dataItem.permissions.length; i++) {
            role = role | dataItem.permissions[i];
        }
        dataItem.role = role;
        switch (action) {
            case 'lockUser':
                ssg.dialog = $rootScope.showConfirmDelete("LOCK", 'Are you sure you want to lock this user ?', 'Confirm');
                ssg.dialog.bind("close", async function (result) {
                    if (result && result.data && result.data.value) {
                        var result = await settingService.getInstance().users.lockUserMembership({
                            userId: dataItem.id, isActivated: dataItem.isActivated, lockType: "lockUser"
                        }).$promise;
                        if (result.isSuccess) {
                            await getUsers();
                            Notification.success("Lock User Success");
                        } else {
                            Notification.error(result.messages[0]);
                        }
                        grid.refresh();
                        $scope.userSelected = [];
                    }
                });
                /**$scope.dataTempory = dataItem;
                ssg.dialog = $rootScope.showConfirmDelete("LOCK", 'Are you sure you want to lock this user ?', 'Confirm');
                ssg.dialog.bind("close", comfirmlockUserMebership);*/
                break;
            case 'unLockUser':
                ssg.dialog = $rootScope.showConfirmDelete("UNLOCK", 'Are you sure you want to lock this user ?', 'Confirm');
                ssg.dialog.bind("close", async function (result) {
                    if (result && result.data && result.data.value) {
                        var result = await settingService.getInstance().users.lockUserMembership({
                            userId: dataItem.id, isActivated: dataItem.isActivated, lockType: "unLockUser"
                        }).$promise;
                        if (result.isSuccess) {
                            await getUsers();
                            Notification.success("UnLock User Success");
                        } else {
                            Notification.error(result.messages[0]);
                        }
                        grid.refresh();
                        $scope.userSelected = [];
                    }
                });
                break;
        }
    }
    //end
    //lamnl: 20/01/2022
    comfirmlockUserMebership = async function (e, action) {
        if (e.data && e.data.value) {
            var lag = "";
            var grid = $("#grid").data("kendoGrid");
            dataItem = $scope.dataTempory;
            var result = await settingService.getInstance().users.lock1({ userId: dataItem.id, isActivated: dataItem.isActivated }).$promise;
            console.log("aa: " + result);
            if (result.isSuccess) {
                await getUsers();
                Notification.success("UnLock User Success");
            } else {
                Notification.error(result.messages[0]);
            }
            grid.refresh();
        }
    }
    //end
    confirmStatusChange = async function (e) {
        if (e.data && e.data.value) {
            var grid = $("#grid").data("kendoGrid");
            dataItem = $scope.dataTempory;
            dataItem['isActivated'] = !dataItem['isActivated'];
            var result = await settingService.getInstance().users.changeStatus({ userId: dataItem.id, isActivated: dataItem.isActivated }).$promise;
            if (result.isSuccess) {
                await getUsers();
                Notification.success("The user status has been changed");
            } else {
                Notification.error(result.messages[0]);
            }
            grid.refresh();
        }
    }

    $scope.dataTempory = [];
    $scope.excuteAction = async function (action, dataItem, $event = null) {
        var grid = $("#grid").data("kendoGrid");
        var role = 0;
        for (var i = 0; i < dataItem.permissions.length; i++) {
            role = role | dataItem.permissions[i];
        }
        dataItem.role = role;
        switch (action) {
            case commonData.gridActions.Create:
                {
                    if ($scope.userSelected.length > 0 && $scope.userSelected[0].item.id) {
                        Notification.error("Please save selected item before create other item");
                        break;
                    }
                    else {
                        var isContinue = validateDataInGrid(dataItem, ssg.requiredValueFields, [], Notification, ssg.requiredArrayFields);
                        if (isContinue) {
                            var result = await settingService.getInstance().users.createUser(dataItem).$promise;
                            if (result.isSuccess) {
                                await getUsers();
                                Notification.success("Create User Success");
                                grid.refresh();
                            } else {
                                Notification.error(result.messages[0]);
                            }
                        }
                        $scope.userSelected = [];
                        break;
                    }
                }
            case commonData.gridActions.Save:
                var isContinue = validateDataInGrid(dataItem, ssg.requiredValueFields, [], Notification, ssg.requiredArrayFields);
                if (isContinue) {
                    var checkData = await findUserByCodeLoginEmail(dataItem.id, dataItem.sapCode, dataItem.loginName, dataItem.email);
                    if (checkData) {
                        var result = await settingService.getInstance().users.update(dataItem).$promise;
                        if (result.isSuccess) {
                            await getUsers();
                            Notification.success("Update User Success");
                            grid.refresh();
                        } else {
                            Notification.error(result.messages[0]);
                        }
                    }
                }
                $scope.userSelected = [];
                break;
            case commonData.gridActions.Edit:
                if ($scope.userSelected.length > 0 && $scope.userSelected[0].item.id) {
                    Notification.error("Please save selected item before edit other item");
                } else {
                    dataItem['selected'] = true;
                    $scope.userSelected = [];
                    $scope.userSelected.push({ item: dataItem });
                    //lamnl add 24/01/2022 check unlock display
                    var result = await settingService.getInstance().users.checkLockUser({ userId: dataItem.id }).$promise;
                    if (result.isSuccess) {
                        if (result.checkLock)
                            $scope.checkuser = 1;
                        else
                            $scope.checkuser = 0;
                    } else {
                        Notification.error(result.messages[0]);
                    }//end
                }

                grid.refresh();
                // Notification.success("Update success");
                break;
            case commonData.gridActions.StatusChange:
                if ($scope.userSelected.length > 0 && $scope.userSelected[0].item.id) {
                    Notification.error("Please save selected item before inactive/ active other item");
                }
                else {
                    // dataItem['inactive'] = !dataItem['inactive'];
                    // grid.refresh();
                    // dataItem['inactive'] ? Notification.success("Inactive success") : Notification.success("Active success");
                    $scope.dataTempory = dataItem;
                    ssg.dialog = $rootScope.showConfirmDelete(dataItem['isActivated'] ? 'INACTIVE' : 'ACTIVE', dataItem['isActivated'] ? commonData.confirmContents.inactive : commonData.confirmContents.active, 'Confirm');
                    // ssg.dialog.bind("close", async function () {
                    //     dataItem['isActivated'] = !dataItem['isActivated'];
                    //     var result = await settingService.getInstance().users.changeStatus({ userId: dataItem.id, isActivated: dataItem.isActivated }).$promise;
                    //     if (result.isSuccess) {
                    //         await getUsers();
                    //         Notification.success("The user status has been changed");
                    //     } else {
                    //         Notification.error(result.messages[0]);
                    //     }
                    //     grid.refresh();
                    // });
                    ssg.dialog.bind("close", confirmStatusChange);
                }
                break;
            case 'resetPassword':
                if ($scope.userSelected.length > 0 && $scope.userSelected[0].item.id) {
                    Notification.error("Please save selected item before reset password other item");
                }
                else {
                    ssg.dialog = $rootScope.showConfirmDelete("RESET PASSWORD", commonData.confirmContents.resetPassword, 'Confirm');
                    ssg.dialog.bind("close", async function (result) {
                        if (result && result.data && result.data.value) {
                            var result = await settingService.getInstance().users.resetPassword({ userId: dataItem.id }).$promise;
                            if (result.isSuccess) {
                                await getUsers();
                                Notification.success("The user password has been reset");
                            } else {
                                Notification.error(result.messages[0]);
                            }
                            grid.refresh();
                        }
                    });
                }
                break;
            case commonData.gridActions.Cancel:
                $scope.userSelected = [];
                // dataItem = tempDataItem;
                dataItem.selected = false;
                grid.dataSource.read();
                $scope.checkuser = 0;
                break;
        }
    }

    $scope.addSubItem = function () {
        // $scope.data.push();
        var grid = $("#grid").data("kendoGrid");
        grid.dataSource.add({
            No: '',
            Id: grid.dataSource.total() + 1,
            LoginName: '',
            sapCode: '',
            fullName: '',
            Email: '',
            JobTitle: '',
            ProfilePicture: 'ClientApp/assets/images/avatar.png',
            isHasProfile: false
        });
    }
    $scope.leaveInformationData = [];

    $scope.leaveInformationGridOptions = {
        dataSource: {
            serverPaging: false,
            data: $scope.leaveInformationData,
            pageSize: 20,
        },
        sortable: false,
        autoBind: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [{
            field: "status",
            title: $translate.instant('COMMON_STATUS'),
            locked: true,
            lockable: false,
            width: "180px",
            template: function (dataItem) {
                var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                //return `<workflow-status status="${dataItem.status}"></workflow-status>`
            }
        }, {
            field: "referenceNumber",
            title: $translate.instant('COMMON_REFERENCE_NUMBER'),
            width: "150px",
            locked: true,
            lockable: false,
            template: function (dataItem) {
                return `<a ui-sref="home.leavesManagement.item({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "fromDate",
            title: $translate.instant('LEAVE_MANAGEMENT_FROM_DATE'),
            width: "200px",
            template: function (dataItem) {
                return moment(dataItem.fromDate).format('DD/MM/YYYY');
            }
        },
        {
            field: "toDate",
            title: $translate.instant('LEAVE_MANAGEMENT_TO_DATE'),
            width: "200px",
            template: function (dataItem) {
                return moment(dataItem.toDate).format('DD/MM/YYYY');
            }
        },
        //
        {
            field: "leaveName",
            title: $translate.instant('LEAVE_MANAGEMENT_LEAVE_KIND'),
            width: "220px",
            template: function (dataItem) {
                // return `<label>${dataItem.leaveCode} - ${dataItem.leaveName}</label>`
                return `<label>${dataItem.leaveName}</label>`
            }
        },
        {
            field: "quantity",
            //title: "QUANTITY",
            title: $translate.instant('COMMON_QUANTITY'),
            width: "200px",
            template: function (dataItem) {
                return `<label>${dataItem.quantity}</label>`
            }
        },
        {
            field: "reason",
            title: $translate.instant('COMMON_REASON'),
            width: "220px",
        },
        ],
        //page: function (e) {
        //    getLeaveApplications(e.page);
        //}
    };

    $scope.leaveTitleGridOptions = {
        dataSource: {
            data: leaveBalances
        },
        sortable: false,
        autoBind: true,
        valuePrimitive: false,
        columns: [{
            field: "absenceQuotaName",
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_KIND'),
            width: "80px"
        }, {
            field: "currentYearBalance",
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_QUOTA'),
            width: "80px"
        }, {
            field: "remain",
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_REMAIN'),
            width: "80px"
        }]
    }

    //Missing Timeclock
    $scope.missingTimeclockApplicantData = [];
    $scope.stripOptions = {
        animation: false,
    }
    $scope.missingTimeclockApplicantGridOptions = {
        dataSource: {
            serverPaging: false,
            data: $scope.missingTimeclockApplicantData,
            pageSize: 20,
        },
        sortable: false,
        // pageable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [{
            field: "status",
            title: $translate.instant('COMMON_STATUS'),
            locked: true,
            lockable: false,
            width: "150px",
            template: function (dataItem) {
                var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                //return `<workflow-status status="${dataItem.status}"></workflow-status>`
            }
        },
        {
            field: "referenceNumber",
            title: $translate.instant('COMMON_REFERENCE_NUMBER'),
            width: "150px",
            locked: true,
            lockable: false,
            template: function (dataItem) {
                if (
                    dataItem.status === commonData.StatusMissingTimeLock.WaitingCMD ||
                    dataItem.status === commonData.StatusMissingTimeLock.WaitingHOD
                ) {
                    return `<a ui-sref="home.missingTimelock.approve({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                }
                return `<a ui-sref="${viewStateML}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "date",
            title: $translate.instant('MISSING_TIMECLOCK_DATE'),
            width: "220px",
            template: function (dataItem) {
                if (dataItem.date) {
                    return moment(dataItem.date).format('DD/MM/YYYY');
                }
                return '';
            }
        },
        {
            field: "shiftCode",
            title: $translate.instant('MISSING_TIMECLOCK_SHIFT_CODE'),
            width: "220px",
        },
        {
            field: "actualTime",
            title: $translate.instant('MISSING_TIMECLOCK_ACTUAL_TIME'),
            width: "220px",
            template: function (dataItem) {
                if (dataItem.actualTime) {
                    return moment(dataItem.actualTime).format('DD/MM/YYYY');
                }
                return '';
            }
        },
        {
            field: "reasonName",
            title: $translate.instant('COMMON_REASON'),
            width: "220px"
        },
        {
            field: "others",
            title: $translate.instant('COMMON_OTHER_REASON'),
            width: "300px"
        }
        ],
        //page: async function (e) {
        //    console.log(e.page);
        //    await getMissingTimelocks(e.page);
        //}
    };

    $scope.overtimeApplicantData = [];

    $scope.overtimeApplicantGridOptions = {
        dataSource: {
            serverPaging: false,
            data: $scope.overtimeApplicantData,
            pageSize: 20,
        },
        sortable: false,
        // pageable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [{
            field: "status",
            title: $translate.instant('COMMON_STATUS'),
            locked: true,
            lockable: false,
            width: "150px",
            template: function (dataItem) {
                var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                //return `<workflow-status status="${dataItem.status}"></workflow-status>`
            }
        },
        {
            field: "referenceNumber",
            title: $translate.instant('COMMON_REFERENCE_NUMBER'),
            width: "150px",
            locked: true,
            lockable: false,
            template: function (dataItem) {
                return `<a ui-sref="${viewStateOvertime}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})"
                            ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "date",
            title: $translate.instant('OVERTIME_APPLICATION_DATE_OF_OT'),
            width: "250px",
            template: function (dataItem) {
                return moment(dataItem.date).format('DD/MM/YYYY');
            }
        },
        {
            field: "reasonName",
            title: $translate.instant('OVERTIME_APPLICATION_REASON_OF_OT'),
            width: "220px"
        },
        {
            field: "otProposalHours",
            title: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_HOURS'),
            width: "220px"
        },
        {
            field: "proposalHoursFrom",
            title: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_FROM'),
            width: "100px"
        },
        {
            field: "proposalHoursTo",
            title: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_TO'),
            width: "100px"
        },
        {
            field: "actualOtHours",
            title: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_HOURS'),
            width: "220px"
        },
        {
            field: "actualHoursFrom",
            title: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_FROM'),
            width: "130px"
        },
        {
            field: "actualHoursTo",
            title: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_TO'),
            width: "100px"
        },
        {
            field: "dateOffInLieu",
            title: $translate.instant('OVERTIME_APPLICATION_DATE_OFF_IN_LIEU'),
            width: "150px",
            template: "<input name='#:referenceNumber#' id='#:referenceNumber#' class='k-checkbox' type='checkbox' #= (dateOffInLieu == true) ? checked ='checked' : '' # disabled/> <label class='k-checkbox-label' for='#:referenceNumber#'></label>",
        },
        ],
        //page: async function (e) {
        //    await getOvertimeApplications(e.page);
        //}
    };

    $scope.shiftExchangeApplicantApplicantData = [];

    $scope.shiftExchangeApplicantGridOptions = {
        dataSource: {
            serverPaging: false,
            data: $scope.shiftExchangeApplicantApplicantData,
            pageSize: 20,
        },
        sortable: false,
        // pageable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [{
            field: "status",
            title: $translate.instant('COMMON_STATUS'),
            locked: true,
            lockable: false,
            width: "150px",
            template: function (dataItem) {
                var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                //return `<workflow-status status="${dataItem.status}"></workflow-status>`
            }
        },
        {
            field: "referenceNumber",
            title: $translate.instant('COMMON_REFERENCE_NUMBER'),
            width: "220px",
            locked: true,
            lockable: false,
            template: function (dataItem) {
                return `<a ui-sref="home.shiftExchange.item({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" 
                            ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "exchangeShiftDay",
            title: $translate.instant('SHIFT_EXCHANGE_APPLICATION_APPLY_DATE'),
            width: "220px",
            template: function (dataItem) {
                return moment(dataItem.shiftExchangeDate).format('DD/MM/YYYY');
            }
        },
        {
            field: "currentShiftName",
            title: $translate.instant('SHIFT_EXCHANGE_APPLICATION_CURRENT_SHIFT'),
            width: "220px"
        },
        {
            field: "newShiftName",
            title: $translate.instant('SHIFT_EXCHANGE_APPLICATION_NEW_SHIFT'),
            width: "220px"
        },
        {
            field: "reasonName",
            title: $translate.instant('COMMON_REASON'),
            width: "220px"
        },
        ],
        //page: async function (e) {
        //    await getShiftExchangeApplications(e.page);
        //}
    };
    $scope.resetPassWord = function () {
        ssg.dialog = $rootScope.showConfirmDelete("DELETE", "Are you sure to reset your password ?", 'Confirm');
        ssg.dialog.bind("close", confirmResetPassword);
    }
    let confirmResetPassword = function (e) {
        if (e.data && e.data.value) {
            Notification.success("New password sent to your email ");
        }
    }
    $scope.onUpload = function (e) {
        var fileInfo = e.files[0];
        var raw = fileInfo.rawFile;
        var reader = new FileReader();
        if (raw) {
            reader.onloadend = function () {
                $scope.myPhoto = this.result;
                $scope.$apply();
            };

            reader.readAsDataURL(raw);
        }
    }
    $scope.uploadOptions = {
        async: {
            saveUrl: "save",
            removeUrl: "remove",
            autoUpload: false
        },
        multiple: false,
        validation: {
            allowedExtensions: ['.gif', '.jpg', '.png'],
            maxFileSize: 2000000,
        },
        select: function (e) { },
        error: function (e) {

            $scope.myPhoto = 'https://farm9.staticflickr.com/8455/8048926748_1bc624e5c9_d.jpg';
        }
    };

    $scope.dataSource = appSetting.permission;

    function customTemplate(dataItem, name, type) {
        if (type === 'dropdown') {
            if (dataItem.selected || !dataItem.id) {
                return `<select kendo-multi-select style="width: 100%;" name=${name} 
                    data-k-ng-model="dataItem.permissions"
                    k-data-text-field="'name'"
                    k-data-value-field="'code'"
                    filter = "'contains'"
                    k-data-source="dataSource"
                    k-auto-bind="'false'"
                    k-value-primitive="'false'"
                    > </select>`
            } else {
                var perms = [];
                for (var i = 0; i < appSetting.permission.length; i++) {
                    for (var j = 0; j < dataItem.permissions.length; j++) {
                        if (appSetting.permission[i].code == dataItem.permissions[j]) {
                            perms.push(appSetting.permission[i].name);
                        }
                    }
                }
                return `<span>${perms.map(x => x)}</span>`
            }
        }

    }
    $scope.keyword = '';
    async function getUsers(option) {
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Modified desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }

        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        if ($scope.keyword) {
            $scope.currentQuery.predicate = '(LoginName.contains(@0) or SAPCode.contains(@0) or Email.contains(@0) or FullName.contains(@0))';
            $scope.currentQuery.predicateParameters.push($scope.keyword);
            $scope.currentQuery.predicateParameters.push(false);
        }
        else {
            $scope.currentQuery.predicateParameters.push(false);
        }



        var res = await settingService.getInstance().users.getUsers(
            $scope.currentQuery
        ).$promise;
        if (res.isSuccess) {
            $scope.data = [];

            // var n = 1;
            let n = (($scope.currentQuery.page - 1) * appSetting.pageSizeDefault) + 1;
            res.object.data.forEach(element => {
                element.no = n++;
                element.permissions = [];
                element.type = element.type.toString();
                //conver bitwise to role perms
                for (var i = 0; i < appSetting.permission.length; i++) {
                    if ((appSetting.permission[i].code & element.role) > 0) {
                        element.permissions.push(appSetting.permission[i].code);
                    }
                }
                $scope.data.push(element);
            });
            $scope.data.push({ no: '', loginName: '', fullName: '', sapCode: '', mail: '', profilePicture: null, permissions: [], isActivated: true});
            if ($scope.userSelected.length) {
                var currentItem = _.find($scope.data, x => {
                    return x.id == $scope.userSelected[0].item.id;
                });
                if (currentItem) {
                    currentItem.profilePicture = $scope.userSelected[0].profilePicture;
                }
            }

        }
        $scope.total = res.object.count;
        if (option) {
            option.success($scope.data);
        } else {
            var grid = $("#grid").data("kendoGrid");
            // grid.options.dataSource.data = $scope.data;
            grid.dataSource.read();
            grid.dataSource.page(1);
        }
        $scope.userSelected = [];
    }

    //them moi 
    function mapOTHour(from, to) {
        let startTime = moment(from, "HH:mm");
        let endTime = moment(to, "HH:mm");
        let duration = moment.duration(endTime.diff(startTime));
        let hours = duration.asHours();
        //Làm tròn xuống 15 phút
        return result = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
    }

    function initGrid(idGird, dataSource, total, pageIndex) {
        let grid = $(idGird).data("kendoGrid");
        let dataSourceRequests = new kendo.data.DataSource({
            data: dataSource,
            pageSize: 5,
            page: pageIndex,
            schema: {
                total: function () {
                    return total;
                }
            },
        });
        if (grid) {
            grid.setDataSource(dataSourceRequests);
        }
    }

    $scope.export = async function () {
        let model = {
            predicate: '',
            predicateParameters: [],
            order: "Modified desc",
        }

        if ($scope.keyword) {
            model.predicate = 'LoginName.contains(@0) or SAPCode.contains(@0) or Email.contains(@0) or FullName.contains(@0)';
            model.predicateParameters.push($scope.keyword);
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: 19
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }

    $scope.import = async function () {

    }

    $scope.ifEnter = async function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            // Do that thing you finally wanted to do
            await getUsers();
        }
    }

    $scope.search = async function () {
        await getUsers();
    }


    async function uploadAction(item, idUser, profilePictureId) {
        if (profilePictureId == null) {
            profilePictureId = '00000000-0000-0000-0000-000000000000'; // gắn Guid.Empty Id 
        }
        var payload = new FormData();
        payload.append('file', item.rawFile, item.name);
        if (idUser) {
            let resFile = await attachmentService.getInstance().attachmentFile.uploadImage({ userId: idUser, profilePictureId: profilePictureId }, payload).$promise;
            if (resFile.isSuccess) {
                let resUser = await settingService.getInstance().users.updateImageUser({ id: idUser, profilePictureId: resFile.object }).$promise;
                if (resUser.isSuccess && resUser.object) {
                    if (idUser == $rootScope.currentUser.id) {
                        $rootScope.avatar = baseUrlApi.replace('/api', '') + resUser.object.profilePicture.trim();
                    }
                    await getUsers();
                }
            }
            return resFile;
        }
    }

    async function getImageUser() {
        var res = await settingService.getInstance().users.getImageUserById({ userId: $rootScope.currentUser.id }).$promise;
        if (res.isSuccess) {
            if (res.object.data.profilePicture) {
                $timeout(function () {
                    $scope.avatar = baseUrlApi.replace('/api', '') + res.object.data.profilePicture.trim();;
                    $rootScope.avatar = $scope.avatar;
                }, 0);

            } else {
                $timeout(function () {
                    $scope.avatar = $scope.avatarDefault;
                    $rootScope.avatar = $scope.avatar;
                }, 0);

            }
        }
    }

    $scope.changePwdDialog = {};
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
                    var matchedItems = $scope.pwdModel.newPassword.match(/(.{7,})/g);
                    if (!matchedItems) {
                        //$scope.pwdModel.errors.push("Password must greater than 7 characters and at least one special (non-alphanumeric) character");
                        $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_CHARACTER_VALIDATE'))
                    }
                }
                if ($scope.pwdModel.errors.length == 0) {
                    settingService.getInstance().users.changePassword($scope.pwdModel).$promise.then(function (result) {
                        if (result.object) {
                            Notification.success("Your password was changed successfully");
                            $scope.changePwdDialog.close();
                        } else {
                            //$scope.pwdModel.errors.push("Your current password is not correct. Please try again")
                            $scope.pwdModel.errors.push($translate.instant('CHANGE_PASSWORD_CURRENT_CORRECT_VALIDATE'))
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

    // $scope.cancel = function () {
    //     window.history.back();
    // }

    async function findUserByCodeLoginEmail(userId, sapCode, loginName, email) {
        let model = {
            id: userId,
            sapCode: sapCode,
            loginName: loginName,
            email: email
        }
        var result = await settingService.getInstance().users.findUserForDataInvalid(model).$promise;
        if (result.isSuccess) {
            if (result.object.count > 0) {
                result.object.data.forEach(item => {
                    Notification.error(item);
                });
                return false;
            }

        }
        return true;
    }
    $scope.cancel = function () {
        $history.back();
    }

});