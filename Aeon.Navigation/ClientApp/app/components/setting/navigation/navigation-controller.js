var ssgApp = angular.module('ssg.navigationModule', ['kendo.directives']);
ssgApp.controller('navigationController', function ($rootScope, $scope, $translate, appSetting, Notification, commonData, $stateParams, $state, navigationService, settingService, fileService, attachmentService, $timeout, ssgexService, $history) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = $translate.instant('COMMON_NAVIGATION_MANAGEMENT');
    $scope.files = [];
    $scope.test = "1";
    $scope.total = 0;
    $scope.userSelected = [];
    $scope.dialogActions = [
        "close"
    ]
    $scope.testBase64 = '';
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }
    $scope.avatar = '';
    $scope.navigationParentId = -1;
    $scope.navigationType = '';
    $scope.avatarDefault = 'ClientApp/assets/images/avatar.png';
    $scope.baseUrlApi_test = 'abc/api';
    $scope.editingChild = {};
    $scope.isSAdmin = false;
    $scope.numeric = {
        format: '',
    }
    $scope.edocModuleOptions = {
        dataTextField: 'name',
        dataValueField: 'name',
        autoBind: false,
        valuePrimitive: true,
        dataSource: [
            { "id": 'EdocHR', "name": 'EdocHR' },
            { "id": 'EdocFinance', "name": 'EdocFinance' },
            { "id": 'EdocTrade', "name": 'EdocTrade' },
        ]
    };

    this.$onInit = async () => {
        // hard code 
        $scope.isSAdmin = ($scope.currentUser && $scope.currentUser.sapCode && appSetting.personalSapCode.includes($scope.currentUser.sapCode)) ? true : false;
        if ($scope.isSAdmin) {
            $scope.data = [];
            await getListDepartment();
            await getListUser();
            await getListUserGroups();
            await getJobGradeList();
            $scope.navigationGroupUsersGridOptions = {
                dataSource: {
                    serverPaging: true,
                    pageSize: 20,
                    transport: {
                        read: async function (e) {
                            await getListNavigation(e);
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
                    title: $translate.instant('COMMOM_NO'),
                    width: "40px"
                },
                {
                    field: "GroupName",
                    title: $translate.instant('COMMON_GROUP_NAME'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="name" ng-model="dataItem.name"/>`;
                        } else {
                            return `<span>{{dataItem.name}}</span>`
                        }
                    },
                    width: "500px"
                },
                {
                    field: "User_AD",
                    title: 'AD',
                    template: function (dataItem) {
                        let checked = dataItem.isAD ? 'checked' : 'false';
                        if (!dataItem.selected && dataItem.id) {
                            return `<input ng-model="dataItem.isAD" name='${dataItem.id}' id='isAD${dataItem.id}' class='k-checkbox' checked="${checked}" disabled type='checkbox'/>
                        <label class='k-checkbox-label' for='isAD${dataItem.id}'></label>`
                        } else {
                            return `<input ng-model="dataItem.isAD" name="isAD" id="isAD${dataItem.id}" checked="${checked}" class="k-checkbox" type="checkbox"/>
                        <label class="k-checkbox-label" for="isAD${dataItem.id}"></label>`;
                        }
                    },
                    width: "40px"
                },
                {
                    field: "User_MS",
                    title: 'MS',
                    template: function (dataItem) {
                        let checked = dataItem.isMS ? 'checked' : 'false';
                        if (!dataItem.selected && dataItem.id) {
                            return `<input ng-model="dataItem.isMS" name='isMS${dataItem.id}' id='${dataItem.id}' class='k-checkbox' checked="${checked}" disabled type='checkbox'/>
                        <label class='k-checkbox-label' for='isMS${dataItem.id}'></label>`
                        }
                        else {
                            return `<input ng-model="dataItem.isMS" name="isMS" id="isMS${dataItem.id}" checked="${checked}" class="k-checkbox" type="checkbox"/>
                        <label class="k-checkbox-label" for="isMS${dataItem.id}"></label>`;
                        }
                    },
                    width: "40px"
                },
                {
                    field: "JobGrade",
                    title: $translate.instant('JOBGRADE_MENU'),
                    template: (dataItem) => customTemplate(dataItem, 'jobgrades', 'dropdown', 'dataSourceJobGrades', 'dataItem.jobGrades'),
                    width: "150px"
                },
                {
                    field: "Roles",
                    title: $translate.instant('COMMON_ROLES'),
                    template: (dataItem) => customTemplate(dataItem, 'permissions', 'dropdown', 'dataSourcePermissions', 'dataItem.permissions'),
                    width: "200px"
                },
                {
                    field: "Departments",
                    title: $translate.instant('COMMON_DEPARTMENTS'),
                    template: (dataItem) => customTemplate(dataItem, 'departments', 'dropdown', 'dataSourceDepartments', 'dataItem.departments'),
                    width: "375px"
                },
                {
                    field: "Users",
                    title: $translate.instant('COMMON_USERS'),
                    template: (dataItem) => customTemplate(dataItem, 'users', 'dropdown', 'dataSourceUsers', 'dataItem.users'),
                    width: "375px"
                },
                {
                    field: "Actions",
                    title: $translate.instant('COMMOM_ACTION'),
                    template: function (dataItem) {
                        if (!dataItem.no) {
                            return `<a class="btn btn-sm btn-primary" ng-click="excuteAction('create',dataItem)"><i class="fa fa-plus right-5"></i>Create</a>`
                        } else {
                            if (dataItem.selected) {
                                return `<a ng-click="excuteAction('save',dataItem, $event)"  class='btn btn-sm default green-stripe' >Save</a>
                                    <a ng-click="excuteAction('cancel',dataItem)"  class='btn btn-sm default' >Cancel</a>`
                            }
                            if (!dataItem.selected) {
                                return `<a ng-click="excuteAction('edit',dataItem)" class='btn btn-sm default blue-stripe'>Edit</a>
                                    <a ng-click="excuteAction('remove',dataItem)" class='btn btn-sm default blue-stripe'>Delete</a>`;
                            }
                        }
                    },
                    width: "200px"
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
                    }
                },
                selectable: false
            }

            $scope.navigationTopLeftGridOptions = {
                dataSource: {
                    serverPaging: true,
                    pageSize: 20,
                    transport: {
                        read: async function (e) {
                            await getListNavigation(e);
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
                    title: $translate.instant('COMMOM_NO'),
                    width: "40px"
                },
                {
                    field: "Title_VI",
                    title: $translate.instant('COMMOM_TITLE_VI'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="title_VI1" ng-model="dataItem.title_VI"/>`;
                        } else {
                            return `<span>{{dataItem.title_VI}}</span>`
                        }
                    }
                },
                {
                    field: "Title_EN",
                    title: $translate.instant('COMMOM_TITLE_EN'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="title_EN" ng-model="dataItem.title_EN"/>`;
                        } else {
                            if (dataItem.id) {
                                return `<span>{{dataItem.title_EN}}</span>`
                            }
                        }
                    }
                },
                {
                    field: "Module",
                    title: "MODULE",
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<select kendo-drop-down-list
                                ng-model="dataItem.module"
                                k-options="edocModuleOptions"
                                style="width: 100%">
                                </select>`;
                        } else {
                            if (dataItem.id) {
                                return `<span>{{dataItem.module}}</span>`
                            }
                        }
                    }
                },
                {
                    field: "url",
                    title: "Url",
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="url" ng-model="dataItem.url"/>`;
                        } else {
                            return `<span>{{dataItem.url}}</span>`;
                        }
                    }
                },
                {
                    field: "priority",
                    title: $translate.instant('COMMOM_PRIORITY'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input kendo-numeric-text-box k-min="0" name="priority" ng-model="dataItem.priority" style="width: 100%;" />`;
                        } else {
                            return `<span>{{dataItem.priority}}</span>`;
                        }
                    },
                    width: "90px"
                },
                {
                    field: "UserGroup",
                    title: $translate.instant('COMMON_NAVIGATION_GROUP_USER'),
                    template: (dataItem) => customTemplate(dataItem, 'UserGroups', 'dropdown', 'dataSourceUserGroups', 'dataItem.userGroups'),
                    width: "250px"
                },
                {
                    field: "NonUserGroup",
                    title: $translate.instant('COMMON_NAVIGATION_NON_GROUP_USER'),
                    template: (dataItem) => customTemplate(dataItem, 'NonUserGroups', 'dropdown', 'dataSourceNonUserGroups', 'dataItem.nonUserGroups'),
                    width: "250px"
                },
                {
                    field: "Actions",
                    title: $translate.instant('COMMOM_ACTION'),
                    width: "260px",
                    template: function (dataItem) {
                        if (!dataItem.no) {
                            return `<a class="btn btn-sm btn-primary" ng-click="excuteAction('create',dataItem)"><i class="fa fa-plus right-5"></i>Create</a>`
                        } else {
                            if (dataItem.selected) {
                                return `<a ng-click="excuteAction('save',dataItem, $event)"  class='btn btn-sm default green-stripe' >Save</a>
                                    <a ng-click="excuteAction('cancel',dataItem)"  class='btn btn-sm default' >Cancel</a>`
                            }
                            if (!dataItem.selected) {
                                return `<a ng-click="excuteAction('edit',dataItem)" class='btn btn-sm default blue-stripe'>Edit</a>
                                    <a ng-click="excuteAction('remove',dataItem)" class='btn btn-sm default blue-stripe'>Delete</a>`;
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
                    }
                },
                selectable: false
            }

            $scope.userGridOptions = {
                dataSource: {
                    serverPaging: true,
                    pageSize: 20,
                    transport: {
                        read: async function (e) {
                            await getListNavigation(e);
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
                    title: $translate.instant('COMMOM_NO'),
                    width: "40px"
                },
                {
                    field: "Title_VI",
                    title: $translate.instant('COMMOM_TITLE_VI'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="title_VI1" ng-model="dataItem.title_VI"/>`;
                        } else {
                            return `<span>{{dataItem.title_VI}}</span>`
                        }
                    }
                },
                {
                    field: "Title_EN",
                    title: $translate.instant('COMMOM_TITLE_EN'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="title_EN" ng-model="dataItem.title_EN"/>`;
                        } else {
                            if (dataItem.id) {
                                return `<span>{{dataItem.title_EN}}</span>`
                            }
                        }
                    }
                },
                {
                    field: "Module",
                    title: "MODULE",
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<select kendo-drop-down-list
                                ng-model="dataItem.module"
                                k-options="edocModuleOptions"
                                style="width: 100%">
                                </select>`;
                        } else {
                            if (dataItem.id) {
                                return `<span>{{dataItem.module}}</span>`
                            }
                        }
                    }
                },
                {
                    field: "url",
                    title: "Url",
                    width: "350px",
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="url" ng-model="dataItem.url"/>`;
                        } else {
                            return `<span>{{dataItem.url}}</span>`;
                        }
                    }
                },
                {
                    field: "priority",
                    title: $translate.instant('COMMOM_PRIORITY'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input kendo-numeric-text-box k-min="0" name="priority" ng-model="dataItem.priority"  style="width: 100%;" />`;
                        } else {
                            return `<span>{{dataItem.priority}}</span>`;
                        }
                    },
                    width: "90px"
                },
                {
                    field: "child",
                    title: "Child",
                    width: "80px",
                    template: function (dataItem) {
                        if (dataItem.id) {
                            return `<a ng-click="childManagement(dataItem.id, dataItem.type, dataItem.module)">{{dataItem.countChild}}</a>`
                        } else {
                            return `<a></a>`
                        }
                    }
                },
                {
                    field: "Actions",
                    title: $translate.instant('COMMOM_ACTION'),
                    width: "260px",
                    template: function (dataItem) {
                        if (!dataItem.no) {
                            return `<a class="btn btn-sm btn-primary" ng-click="excuteAction('create',dataItem)"><i class="fa fa-plus right-5"></i>Create</a>`
                        } else {
                            if (dataItem.selected) {
                                return `<a ng-click="excuteAction('save',dataItem, $event)"  class='btn btn-sm default green-stripe' >Save</a>
                                    <a ng-click="excuteAction('cancel',dataItem)"  class='btn btn-sm default' >Cancel</a>`
                            }
                            if (!dataItem.selected) {
                                return `<a ng-click="excuteAction('edit',dataItem)" class='btn btn-sm default blue-stripe'>Edit</a>
                                    <a ng-click="excuteAction('remove',dataItem)" class='btn btn-sm default blue-stripe'>Delete</a>`;
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
                    }
                },
                selectable: false
            }
            // right
            $scope.navigationTopRightGridOptions = {
                dataSource: {
                    serverPaging: true,
                    pageSize: 20,
                    transport: {
                        read: async function (e) {
                            await getListNavigation(e);
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
                    title: $translate.instant('COMMOM_NO'),
                    width: "40px"
                },
                {
                    field: "Title_VI",
                    title: $translate.instant('COMMOM_TITLE_VI'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="title_VI1" ng-model="dataItem.title_VI"/>`;
                        } else {
                            return `<span>{{dataItem.title_VI}}</span>`
                        }
                    }
                },
                {
                    field: "Title_EN",
                    title: $translate.instant('COMMOM_TITLE_EN'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input class="k-textbox w100" name="title_EN" ng-model="dataItem.title_EN"/>`;
                        } else {
                            if (dataItem.id) {
                                return `<span>{{dataItem.title_EN}}</span>`
                            }
                        }
                    }
                },
                {
                    field: "Module",
                    title: "MODULE",
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<select kendo-drop-down-list
                                ng-model="dataItem.module"
                                k-options="edocModuleOptions"
                                style="width: 100%">
                                </select>`;
                        } else {
                            if (dataItem.id) {
                                return `<span>{{dataItem.module}}</span>`
                            }
                        }
                    }
                },
                {
                    field: "priority",
                    title: $translate.instant('COMMOM_PRIORITY'),
                    template: function (dataItem) {
                        if (dataItem.selected || !dataItem.id) {
                            return `<input kendo-numeric-text-box k-min="0" name="priority" ng-model="dataItem.priority"  style="width: 100%;" />`;
                        } else {
                            return `<span>{{dataItem.priority}}</span>`;
                        }
                    },
                    width: "90px"
                },
                {
                    field: "child",
                    title: "Child",
                    width: "80px",
                    template: function (dataItem) {
                        if (dataItem.id) {
                            return `<a ng-click="childManagement(dataItem.id, dataItem.type)" >{{dataItem.countChild}}</a>`
                        } else {
                            return `<a></a>`
                        }
                    }
                },
                {
                    field: "Actions",
                    title: $translate.instant('COMMOM_ACTION'),
                    width: "260px",
                    template: function (dataItem) {
                        if (!dataItem.no) {
                            return `<a class="btn btn-sm btn-primary" ng-click="excuteAction('create',dataItem)"><i class="fa fa-plus right-5"></i>Create</a>`
                        } else {
                            if (dataItem.selected) {
                                return `<a ng-click="excuteAction('save',dataItem, $event)"  class='btn btn-sm default green-stripe' >Save</a>
                                    <a ng-click="excuteAction('cancel',dataItem)"  class='btn btn-sm default' >Cancel</a>`
                            }
                            if (!dataItem.selected) {
                                return `<a ng-click="excuteAction('edit',dataItem)" class='btn btn-sm default blue-stripe'>Edit</a>
                                    <a ng-click="excuteAction('remove',dataItem)" class='btn btn-sm default blue-stripe'>Delete</a>`;
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
                    }
                },
                selectable: false
            }
            await initData();
            await initUserAndDepartment();
            $scope.tabSeleted = 3; // default = 1 (center)
            $scope.$apply();
        } else {
            $rootScope.navigationLeft = [];
            $rootScope.listNavigation = [];
            $rootScope.navigationRight = [];
        }
    }

    // init user & department
    async function initUserAndDepartment() {
        $scope.dataSourceUsers = $scope.list_users;
        $scope.dataSourceDepartments = $scope.list_departments;
        $scope.dataSourcePermissions = appSetting.permission;
        $scope.dataSourceJobGrades = $scope.jobGradeList;
    }

    async function initData() {
        if ($stateParams.action) {
            $scope.title = 'Users';
        }
        ssg.requiredValueFields = [
            { fieldName: 'title_VI', title: "Title VI" },
            { fieldName: 'title_EN', title: "Title EN" }
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

    $scope.uploadImages = function () {
        alert('uploadImages');
    }
    $scope.onChange = function (data) {
        console.log(data);
    }
    $scope.dataTempory = [];
    $scope.excuteAction = async function (action, dataItem, $event = null) {
        var grid = $("#grid" + $scope.tabSeleted).data("kendoGrid");
        switch (action) {
            case commonData.gridActions.Create:
                {
                    if ($scope.userSelected.length > 0 && $scope.userSelected[0].item.id) {
                        Notification.error("Please save selected item before create other item");
                        break;
                    }
                    else {
                        dataItem.type = $scope.tabSeleted;
                        var isContinue = dataItem.type != 3 ? validateDataInGrid(dataItem, ssg.requiredValueFields, [], Notification, ssg.requiredArrayFields) : true;
                        if (isContinue) {
                            var result = await navigationService.getInstance().navigation.create(dataItem).$promise;
                            if (result.isSuccess) {
                                await getListNavigation();
                                Notification.success($translate.instant('COMMON_CREATE_SUCCESS'));
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
                dataItem.type = $scope.tabSeleted;
                var isContinue = dataItem.type != 3 ? validateDataInGrid(dataItem, ssg.requiredValueFields, [], Notification, ssg.requiredArrayFields) : true;
                if (isContinue) {
                    var result = await navigationService.getInstance().navigation.update(dataItem).$promise;
                    if (result.isSuccess) {
                        await getListNavigation();
                        Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                        grid.refresh();
                    } else {
                        Notification.error(result.messages[0]);
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
                    if (dataItem.departments) {
                        dataItem.departments = JSON.parse("[" + dataItem.departments + "]");
                    }
                    if (dataItem.users) {
                        dataItem.users = JSON.parse("[" + dataItem.users + "]");
                    }
                    if (dataItem.permissions) {
                        let permission = JSON.parse(dataItem.permissions);
                        permission.forEach(x => {
                            let per = _.find(appSetting.permission, y => {
                                return y.code == x.code;
                            });
                            if (per) {
                                x.name = per.name;
                            }
                        });
                        dataItem.permissions = permission;
                    }

                    if (dataItem.jobGrades) {
                        let jobGrade = JSON.parse(dataItem.jobGrades);
                        jobGrade.forEach(x => {
                            let grade = _.find($scope.jobGradeList, y => {
                                return y.code == x.code;
                            });
                            if (grade) {
                                x.name = grade.name;
                            }
                        });
                        dataItem.jobGrades = jobGrade;
                    }

                    if (dataItem.userGroups) {
                        let userGroups = JSON.parse(dataItem.userGroups);
                        userGroups.forEach(x => {
                            let per = _.find($scope.dataSourceUserGroups, y => {
                                return y.id == x.id;
                            });
                            if (per) {
                                x.name = per.name;
                            }
                        });
                        dataItem.userGroups = userGroups;
                    }

                    if (dataItem.nonUserGroups) {
                        let nonUserGroups = JSON.parse(dataItem.nonUserGroups);
                        nonUserGroups.forEach(x => {
                            let per = _.find($scope.dataSourceUserGroups, y => {
                                return y.id == x.id;
                            });
                            if (per) {
                                x.name = per.name;
                            }
                        });
                        dataItem.nonUserGroups = nonUserGroups;
                    }
                    $scope.userSelected.push({ item: dataItem })
                }
                grid.refresh();
                break;
            case commonData.gridActions.Remove:
                if (dataItem == null || dataItem.id == null) {
                    Notification.success("Navigation Not Found");
                } else {
                    ssg.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
                    ssg.dialog.bind("close", async function (e) {
                        if (e && e.data && e.data.value) {
                            var result = await navigationService.getInstance().navigation.delete({ Id: dataItem.id }).$promise;
                            if (result.isSuccess) {
                                await getListNavigation();
                                Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                                grid.refresh();
                            } else {
                                Notification.error(result.messages[0]);
                            }
                        }
                    });
                    $scope.userSelected = [];
                }
                break;
            case commonData.gridActions.Cancel:
                $scope.userSelected = [];
                // dataItem = tempDataItem;
                dataItem.selected = false;
                grid.dataSource.read();
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

    $scope.stripOptions = {
        animation: false,
    }

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

    function customTemplate(dataItem, name, type, dataSource, dataNgModel) {
        if (type === 'dropdown') {
            if (dataItem.selected || !dataItem.id) {
                var isKendoType = '';
                var returnHtml = '';
                switch (dataSource) {
                    case 'dataSourceDepartments':
                    case 'dataSourceUsers':
                    case 'dataSourceNonUserGroups':
                    case 'dataSourceUserGroups':
                        if (dataSource == 'dataSourceDepartments') {
                            if (dataItem.departments != null && dataItem.departments[0] != null) {
                                dataItem.departments = dataItem.departments[0];
                            }
                        } else if (dataSource == 'dataSourceUsers') {
                            if (dataItem.users != null && dataItem.users[0] != null) {
                                dataItem.users = dataItem.users[0];
                            }
                        }
                        isKendoType = 'kendo-multi-select';
                        returnHtml = `<select ${isKendoType} style="width: 100%;" name=${name}
                            data-k-ng-model=${dataNgModel}
                            k-data-text-field="'name'"
                            k-data-value-field="'id'"
                            filter = "'contains'"
                            k-data-source=${dataSource}
                            k-auto-bind="'false'"
                            k-value-primitive="'false'"
                            ></select>`;
                        break;
                    case "dataSourceJobGrades":
                    case 'dataSourcePermissions':
                        valueField = 'code';
                        isKendoType = 'kendo-multi-select';
                        returnHtml = `<select ${isKendoType} style="width: 100%;" name=${name}
                            data-k-ng-model=${dataNgModel}
                            k-data-text-field="'name'"
                            k-data-value-field="'code'"
                            filter = "'contains'"
                            k-data-source=${dataSource}
                            k-auto-bind="'false'"
                            k-value-primitive="'false'"
                            ></select>`;
                        break;
                }
                return returnHtml
            } else {
                if (dataSource != null) {
                    var perms = [];

                    var user = $scope.list_users;
                    if (dataSource == 'dataSourceUsers') {
                        for (var i = 0; i < user.length; i++) {
                            if (dataItem.usersList != null) {
                                for (var j = 0; j < dataItem.usersList.length; j++) {
                                    if (user[i].id.toLowerCase() == dataItem.usersList[j].id.toLowerCase()) {
                                        perms.push(user[i].name);
                                    }
                                }
                            }
                        }
                    }

                    var department = $scope.list_departments;
                    if (dataSource == 'dataSourceDepartments') {
                        for (var i = 0; i < department.length; i++) {
                            if (dataItem.departmentList != null) {
                                for (var j = 0; j < dataItem.departmentList.length; j++) {
                                    if (department[i].id.toLowerCase() == dataItem.departmentList[j].id.toLowerCase()) {
                                        perms.push(department[i].name);
                                    }
                                }
                            }
                        }
                    }

                    if (dataSource == 'dataSourcePermissions') {
                        let permissions = JSON.parse(dataItem.permissions);
                        if (permissions != null) {
                            for (var i = 0; i < appSetting.permission.length; i++) {
                                for (var j = 0; j < permissions.length; j++) {
                                    if (appSetting.permission[i].code == permissions[j].code) {
                                        perms.push(appSetting.permission[i].name);
                                    }
                                }
                            }
                        }
                    }

                    if (dataSource == 'dataSourceUserGroups') {
                        var listUserGroup = $scope.dataSourceUserGroups;
                        let userGroups = JSON.parse(dataItem.userGroups);
                        if (userGroups != null) {
                            for (var i = 0; i < listUserGroup.length; i++) {
                                for (var j = 0; j < userGroups.length; j++) {
                                    if (listUserGroup[i].id.toLowerCase() == userGroups[j].id.toLowerCase()) {
                                        perms.push(listUserGroup[i].name);
                                    }
                                }
                            }
                        }
                    }
                    if (dataSource == 'dataSourceNonUserGroups') {
                        var listNonUserGroup = $scope.dataSourceNonUserGroups;
                        let nonUserGroups = JSON.parse(dataItem.nonUserGroups);
                        if (nonUserGroups != null) {
                            for (var i = 0; i < listNonUserGroup.length; i++) {
                                for (var j = 0; j < nonUserGroups.length; j++) {
                                    if (listNonUserGroup[i].id.toLowerCase() == nonUserGroups[j].id.toLowerCase()) {
                                        perms.push(listNonUserGroup[i].name);
                                    }
                                }
                            }
                        }
                    }
                    if (dataSource == 'dataSourceJobGrades') {
                        let jobgrade = JSON.parse(dataItem.jobGrades);
                        if (jobgrade != null) {
                            for (var i = 0; i < $scope.jobGradeList.length; i++) {
                                for (var j = 0; j < jobgrade.length; j++) {
                                    if ($scope.jobGradeList[i].code == jobgrade[j].code) {
                                        perms.push($scope.jobGradeList[i].name);
                                    }
                                }
                            }
                        }
                    }

                    return `<span>${perms.map(x => x)}</span>`
                }
            }
        }
    }
    $scope.keyword = '';
    async function getListNavigation(option) {

        // cap nhat lien tuc user group
        await getListUserGroups();
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
            $scope.currentQuery.predicate = '(Name.contains(@0) or Description.contains(@0) or Url.contains(@0) or Title_VI.contains(@0) or Title_EN.contains(@0))';
            $scope.currentQuery.predicateParameters.push($scope.keyword);
        }

        if ($scope.tabSeleted == null) {
            $scope.tabSeleted = 3; // default = 1 (center)
        }

        $scope.currentQuery.predicate = (($scope.currentQuery.predicate) ? ($scope.currentQuery.predicate + ' and ') : ' ') + '(Type == (@' + $scope.currentQuery.predicateParameters.length + ') and ParentId == null)';
        $scope.currentQuery.predicateParameters.push($scope.tabSeleted);

        var res = await navigationService.getInstance().navigation.getListNavigation($scope.currentQuery).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            let n = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;

            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });
            $scope.data.push({ no: '', name: '', description: '', url: '', mail: '', departments: [], users: [], permissions: [] });
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
            var grid = $("#grid" + $scope.tabSeleted).data("kendoGrid");
            grid.dataSource.read();
            grid.dataSource.page(1);
        }
        $scope.userSelected = [];
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

    async function uploadAction(item, navigationId, profilePictureId) {
        if (profilePictureId == null) {
            profilePictureId = '00000000-0000-0000-0000-000000000000'; // gắn Guid.Empty Id
        }
        var payload = new FormData();
        payload.append('file', item.rawFile, item.name);
        if (navigationId && navigationId != -1) {
            let resFile = await attachmentService.getInstance().attachmentFile.uploadImageByType({ type: "navigation", id: navigationId, profilePictureId: profilePictureId }, payload).$promise;
            if (resFile.isSuccess) {
                let resUser = await navigationService.getInstance().navigation.updateImageNavigation({ id: navigationId, profilePictureId: resFile.object }).$promise;
                if (resUser.isSuccess && resUser.object) {
                    await getUsers();
                    if ($scope.navigationParentId != -1) {
                        await getChildNavigationByParentId($scope.navigationParentId);
                    }
                }
            }
            return resFile;
        }
    }

    $scope.changePassword = function () {
        $scope.changePwdDialog.title($translate.instant('USER_PROFILE_CHANGE_PASS'));
        $scope.changePwdDialog.open();
        $rootScope.confirmDialogChangePassWord = $scope.changePwdDialog;
    }

    $scope.cancel = function () {
        $history.back();
    }

    $scope.newUserOptions = {
        dataTextField: 'fullName',
        dataValueField: 'id',
        template: '#: sapCode # - #: fullName #',
        valueTemplate: '#: sapCode # - #: fullName #',
        autoBind: true,
        valuePrimitive: true,
        filter: "contains",
        customFilterFields: ['sapCode', 'fullName'],
        filtering: filterMultiField,
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getUsers(e);
                }
            },
        },
        change: function (e) {
            if (e.sender.dataItem()) {
                /* $timeout(function () {
                     $scope.assigneeDetail.newAssigneeObject = {
                         id: e.sender.dataItem().id,
                         fullName: e.sender.dataItem().fullName,
                         sapCode: e.sender.dataItem().sapCode
                     };
                 }, 0);*/
            }
        }
    };



    async function getUsers(option) {
        $scope.dataUser = [];
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var arg = {
            predicate: "sapcode.contains(@0) or fullName.contains(@1) or loginName.contains(@2) or email.contains(@3)",
            predicateParameters: [filter, filter, filter, filter],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "sapcode asc"
        }
        var res = await settingService.getInstance().users.getUsers(arg).$promise;
        if (res.isSuccess && res.object.data) {
            res.object.data.forEach(element => {
                $scope.dataUser.push(element);
            });
        }
        if (option) {
            option.success($scope.dataUser);
        }
    }

    async function getListDepartment() {
        let res = await settingService.getInstance().departments.getAllListDepartments().$promise;
        if (res.isSuccess) {
            $scope.list_departments = [];
            res.object.data.forEach(element => {
                let e1 = element;
                e1.id = element.id.toUpperCase();
                e1.name = e1.name + ' - [' + e1.code + ']';
                $scope.list_departments.push(e1);
            });
        }
    }

    async function getListUserGroups() {
        let res = await navigationService.getInstance().navigation.getChildNavigationByType({ type: 3 }).$promise;
        if (res.isSuccess) {
            $scope.dataSourceUserGroups = [];
            $scope.dataSourceNonUserGroups = [];
            res.object.data.forEach(element => {
                let e1 = {
                    id: element.id.toUpperCase(),
                    name: element.name
                };
                $scope.dataSourceUserGroups.push(e1);
                $scope.dataSourceNonUserGroups.push(e1);
            });
        }
    }

    $scope.type = '';
    $scope.typeTemporary = '';
    $scope.changeTab = async function (type) {
        $scope.tabSeleted = type;
        await getListNavigation();
    }

    $scope.childInNavigationGridOptions = {
        dataSource: {
            serverPaging: true,
            data: $scope.childInNavigationList,
            schema: {
                model: {
                    id: "id"
                }
            }
        },
        editable: false,
        sortable: false,
        pageable: {
            pageSize: appSetting.pageSizeWindow,
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesWindowArray,
            responsive: false,
        },
        columns: [{
            field: "no",
            title: "No.",
            width: "50px",
        },
        {
            title: $translate.instant('COMMOM_TITLE_VI'),
            width: "220px",
            field: "Title_VI",
            template: function (dataItem) {
                if (dataItem.selected || !dataItem.id) {
                    return `<input class="k-textbox w100" name="title_VI" ng-model="dataItem.title_VI"/>`;
                } else {
                    return `<span>{{dataItem.title_VI}}</span>`
                }
            }
        },
        {
            field: "Title_EN",
            title: $translate.instant('COMMOM_TITLE_EN'),
            width: "220px",
            template: function (dataItem) {
                if (dataItem.selected || !dataItem.id) {
                    return `<input class="k-textbox w100" name="title_EN" ng-model="dataItem.title_EN"/>`;
                } else {
                    if (dataItem.id) {
                        return `<span>{{dataItem.title_EN}}</span>`
                    }
                }
            }
        },
        {
            field: "url",
            title: "Url",
            width: "270px",
            template: function (dataItem) {
                if (dataItem.selected || !dataItem.id) {
                    return `<input class="k-textbox w100" name="url" ng-model="dataItem.url"/>`;
                } else {
                    return `<span>{{dataItem.url}}</span>`;
                }
            }
        },
        {
            field: "priority",
            title: $translate.instant('COMMOM_PRIORITY'),
            template: function (dataItem) {
                if (dataItem.selected || !dataItem.id) {
                    return `<input kendo-numeric-text-box k-min="0" name="priority" ng-model="dataItem.priority"  style="width: 100%;" />`;
                } else {
                    return `<span>{{dataItem.priority}}</span>`;
                }
            },
            width: "90px"
        },
        {
            field: "UserGroup",
            title: $translate.instant('COMMON_NAVIGATION_GROUP_USER'),
            template: (dataItem) => customTemplate(dataItem, 'UserGroups', 'dropdown', 'dataSourceUserGroups', 'dataItem.userGroups'),
            width: "250px"
        },
        {
            field: "NonUserGroup",
            title: $translate.instant('COMMON_NAVIGATION_NON_GROUP_USER'),
            template: (dataItem) => customTemplate(dataItem, 'NonUserGroups', 'dropdown', 'dataSourceNonUserGroups', 'dataItem.nonUserGroups'),
            width: "250px"
        },
        {
            field: "Images",
            title: "Icon",
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
            width: "50px"
        },
        {
            width: "150px",
            title: "Actions",
            template: function (dataItem) {
                if (dataItem.selected || dataItem.id === -1) {
                    return `<a ng-click="executeActionChildNavigation('save',dataItem)" class='btn btn-sm default green-stripe' >Save</a>
                        <a  ng-click="executeActionChildNavigation('cancel',dataItem)"  class='btn btn-sm default' >Cancel</a>`
                }
                if (!dataItem.selected && dataItem.id !== -1) {
                    return `<a ng-click="executeActionChildNavigation('edit',dataItem)" class='btn btn-sm default blue-stripe'>Edit</a>
                            <a  ng-click="executeActionChildNavigation('remove',dataItem)" class='btn btn-sm default red-stripe'>Delete</a>`
                }
            },
        }
        ]
    }

    $scope.hasEditingChildNav = function () {
        let grid = $('#childInNavigationGrid').data("kendoGrid");
        let hasEditing = _.find(grid.dataSource._data, function (item) {
            return item.selected === true;
        })
        return hasEditing ? true : false;
    }

    $scope.addChildInNavigation = function () {
        let grid = $("#childInNavigationGrid").data("kendoGrid");
        let countEmp = grid.dataSource.data();
        let isEditing = _.find(countEmp, function (item) {
            return !item.id;
        });
        if (!isEditing) {
            let newRow = {
                id: -1,
                no: grid.dataSource._data.length + 1,
                title_VI: '',
                title_EN: '',
                url: '',
                parentId: $scope.navigationParentId,
                type: $scope.navigationType,
                module: $scope.navigationModule,
                departments: [],
                users: [],
                selected: true
            };
            let source = grid.dataSource;
            var length = source._data.length;
            var currentPage = source.page();
            var currentPageSize = source.pageSize();
            var insertIndex = currentPage * currentPageSize <= length ? (currentPage * currentPageSize - 1) : (length + 1);
            source.insert(insertIndex, newRow);
            grid.refresh();
        }
    }

    $scope.childManagement = function (id, type, module) {
        $scope.navigationParentId = id;
        $scope.navigationType = type;
        $scope.navigationModule = module;
        $scope.childInNavigationList = [];
        getChildNavigationByParentId(id);
        $scope.childWin.title("ADD CHILD TO NAVIGATION");
        $scope.childWin.options.draggable = false;
        $scope.childWin.center();
        $scope.childWin.open();
    }

    async function getChildNavigationByParentId(id) {
        // cap nhat lien tuc user group
        await getListUserGroups();

        let grid = $("#childInNavigationGrid").data("kendoGrid");
        grid.setDataSource(new kendo.data.DataSource());
        let result = await navigationService.getInstance().navigation.getChildNavigationByParentId({ parentId: id }).$promise;
        if (result.isSuccess) {
            $scope.childInNavigationList = result.object.data;
            let dataSource = new kendo.data.DataSource({
                data: result.object.data.map((item, index) => {
                    return {
                        ...item,
                        no: index + 1
                    }
                }),
                pageSize: appSetting.pageSizeWindow,
                schema: {
                    model: {
                        id: "id"
                    }
                }
            });
            grid.setDataSource(dataSource);
        } else {
            Notification.error(result.messages[0]);
        }
    }

    async function getListUser() {
        let res = await settingService.getInstance().users.getAllUsers().$promise;
        if (res.isSuccess) {
            $scope.list_users = [];
            res.object.data.forEach(element => {
                let e1 = element;
                e1.id = element.id.toUpperCase();
                $scope.list_users.push(e1);
            });
        }
    }

    async function getListDepartment() {
        let res = await settingService.getInstance().departments.getAllListDepartments().$promise;
        if (res.isSuccess) {
            $scope.list_departments = [];
            res.object.data.forEach(element => {
                let e1 = element;
                e1.id = element.id.toUpperCase();
                e1.name = e1.name + ' - [' + e1.code + ']';
                $scope.list_departments.push(e1);
            });
        }
    }

    async function getListUserGroups() {
        let res = await navigationService.getInstance().navigation.getChildNavigationByType({ type: 3 }).$promise;
        if (res.isSuccess) {
            $scope.dataSourceUserGroups = [];
            $scope.dataSourceNonUserGroups = [];
            res.object.data.forEach(element => {
                let e1 = {
                    id: element.id.toUpperCase(),
                    name: element.name
                };
                $scope.dataSourceUserGroups.push(e1);
                $scope.dataSourceNonUserGroups.push(e1);
            });
        }
    }

    $scope.executeActionChildNavigation = async function (action, dataItem, $event = null) {
        let grid = $('#childInNavigationGrid').data("kendoGrid");
        var gridTabSelected = $("#grid" + $scope.tabSeleted).data("kendoGrid");
        let data = grid.dataSource._data;
        let isEditing = false;
        switch (action) {
            case commonData.gridActions.Save:
                var isContinue = validateDataInGrid(dataItem, ssg.requiredValueFields, [], Notification);
                if (isContinue) {
                    var result = null;
                    if (dataItem.id === -1) {
                        result = await navigationService.getInstance().navigation.create(dataItem).$promise;
                        if (result.isSuccess) {
                            Notification.success($translate.instant($translate.instant('COMMON_CREATE_SUCCESS')));
                            await getChildNavigationByParentId(dataItem.parentId);
                            await getListNavigation();
                        } else {
                            Notification.error(result.messages[0]);
                        }
                    } else {
                        result = await navigationService.getInstance().navigation.update(dataItem).$promise;
                        if (result.isSuccess) {
                            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                            getChildNavigationByParentId(dataItem.parentId);
                        } else {
                            Notification.error(result.messages[0]);
                        }
                    }
                }
                grid.refresh();
                gridTabSelected.refresh();
                break;
            case commonData.gridActions.Edit:
                data.forEach((item) => {
                    if (item) {
                        if (item.id === -1) {
                            Notification.error("Please save selected item before edit/delete other item");
                            isEditing = true;
                        } else {
                            item.selected = false;
                            if ($scope.editingChild.uid && item.uid === $scope.editingChild.uid) {
                                Notification.error("Please save selected item before edit/delete other item");
                                isEditing = true;
                            }
                        }
                    }
                })
                if (isEditing === false) {
                    if (dataItem.departments) {
                        dataItem.departments = JSON.parse("[" + dataItem.departments + "]");
                    }
                    if (dataItem.users) {
                        dataItem.users = JSON.parse("[" + dataItem.users + "]");
                    }
                    if (dataItem.userGroups) {
                        let userGroups = JSON.parse(dataItem.userGroups);
                        userGroups.forEach(x => {
                            let per = _.find($scope.dataSourceUserGroups, y => {
                                return y.id == x.id;
                            });
                            if (per) {
                                x.name = per.name;
                            }
                        });
                        dataItem.userGroups = userGroups;
                    }

                    if (dataItem.nonUserGroups) {
                        let nonUserGroups = JSON.parse(dataItem.nonUserGroups);
                        nonUserGroups.forEach(x => {
                            let per = _.find($scope.dataSourceUserGroups, y => {
                                return y.id == x.id;
                            });
                            if (per) {
                                x.name = per.name;
                            }
                        });
                        dataItem.nonUserGroups = nonUserGroups;
                    }

                    $scope.editingChild = Object.assign($scope.editingChild, dataItem);
                    dataItem.selected = true;
                    grid.refresh();
                    gridTabSelected.refresh();
                }
                break;
            case commonData.gridActions.Cancel:
                grid.dataSource.read();
                break;
            case commonData.gridActions.Remove:
                data.forEach((item) => {
                    if (item) {
                        if (item.id === -1) {
                            Notification.error("Please save selected item before edit/delete other item");
                            isEditing = true;
                        } else {
                            item.selected = false;
                            if ($scope.editingChild.uid && item.uid === $scope.editingChild.uid) {
                                Notification.error("Please save selected item before edit/delete other item");
                                isEditing = true;
                            }
                        }
                    }
                })
                if (isEditing === false) {
                    $scope.editingChild.id = dataItem.id;
                    ssg.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
                    ssg.dialog.bind("close", confirmDeleteNavigationChild);
                }
                break;
        }
    }

    let confirmDeleteNavigationChild = function (e) {
        let removeId = $scope.editingChild.id;
        if (e.data && e.data.value) {
            navigationService.getInstance().navigation.delete({
                Id: removeId
            }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                    getChildNavigationByParentId($scope.navigationParentId);
                    getUsers();
                    $scope.hasAnyUserChange = true; // update user count status to reload
                } else {
                    Notification.error(result.messages[0]);
                }
            });
        }
        $scope.editingChild = {}
    }

    async function getJobGradeList() {
        let result = await settingService.getInstance().headcount.getJobGrades({
            predicate: "",
            predicateParameters: [],
            order: "Grade asc",
            limit: 200,
            page: 1
        }).$promise;
        if (result.isSuccess) {
            if (result.object.data) {
                $scope.jobGradeList = [];
                result.object.data.forEach(x => {
                    var jgrade = {
                        code: x.grade,
                        name: x.caption
                    }
                    $scope.jobGradeList.push(jgrade);
                });
            }
        } else {
            Notification.error(result.messages[0]);
        }
    }

});