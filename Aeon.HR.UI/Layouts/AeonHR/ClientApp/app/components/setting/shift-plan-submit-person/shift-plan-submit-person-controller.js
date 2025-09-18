var ssgApp = angular.module("ssg.shiftPlanSubmitPersonModule", [
    "kendo.directives"
]);
ssgApp.controller("shiftPlanSubmitPersonController", function (
    $rootScope,
    $scope,
    $location,
    $stateParams,
    appSetting,
    commonData,
    Notification,
    settingService,
    $timeout,
    masterDataService,
    fileService,
    $translate
) {
    // create a message to display in our view
    var ssg = this;
    $scope.advancedSearchMode = false;
    $scope.DateOfBirth = new Date();
    $rootScope.isParentMenu = false;
    $scope.title = 'Shift Plan Submit Persons';
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.departmentTreeDataSource = [];
    $scope.departmentTreeDataSourceAdd = [];
    let selectedUsers = [];
    let filterUsers = [];
    $scope.total = 0;
    $scope.shiftPlanGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            schema: {
                total: () => {
                    return $scope.total
                },
                data: () => {
                    return $scope.shiftPlans
                }
            },
            transport: {
                read: async function (e) {
                    await getShiftPlan(e);
                }
            },
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
        columns: [
            {
                field: "no",
                title: "No.",
                width: "20px",
                template: function (dataItem) {
                    return `<label>${dataItem.no ? dataItem.no : ''}</label>`
                }
            },
            {
                field: "departmentId",
                title: "Department",
                width: "150px",
                template: function (dataItem) {
                    if (dataItem.id != -1 && dataItem.edit) {
                        return `<select kendo-drop-down-tree 
                        id="departmentTree${dataItem.no}" 
                        style="width: 100%;" 
                        name="'departmentEditId'"                       
                        k-data-source = "departmentTreeDataSource"
                        k-ng-model="dataItem.departmentId"                   
                        k-options="departmentEditOptions"    
                        k-on-open="openDepartment(dataItem.departmentId ,'departmentTree${dataItem.no}')"                 
                        > </select>`;
                    } else if (dataItem.id === -1) {
                        return `<select kendo-drop-down-tree
                    id="departmentTreeAdd" 
                    style="width: 100%;" 
                    name="'departmentAddId'"                  
                    k-data-source = 'departmentTreeDataSourceAdd'
                    k-ng-model="dataItem.departmentAddId"                       
                    k-options="departmentAddOptions"                                        
                    k-on-change="changeDepartmentAdd(dataItem.departmentAddId)"                     
                    > </select>`;
                    }
                    else {
                        return `<label>${dataItem.departmentName}</label>`
                    }
                }
            },
            {
                field: "userIds",
                title: "User",
                width: "300px",
                template: function (dataItem) {
                    if (dataItem.edit && dataItem.id != -1) {
                        return `<select kendo-multi-select id="userOption${dataItem.no}" k-options="userOptionEdits"  k-ng-model="dataItem.userIds"></select>`
                    } else if (dataItem.id === -1) {
                        return `<select kendo-multi-select id="userOption" k-options="userOptionAdds" k-data-source = "userMultiDataSource" k-ng-model="dataItem.userIds"></select>`
                    } else {
                        return `<label>${dataItem.userNames}</label>`
                    }
                }
            },
            {
                field: "isIncludeChildren",
                title: "Include Children",
                width: "300px", 
                template: function (dataItem) {
                    if (dataItem.edit && dataItem.id != -1) {
                        return genarateCheckbox('Edit');
                    } else if (dataItem.id === -1) {
                        dataItem.isIncludeChildren = true;
                        return genarateCheckbox('Add');
                    } else {
                        return genarateCheckbox('View');
                    }
                }
            },
            {
                width: "140px",
                title: "Actions",
                template: function (dataItem) {
                    if (dataItem.id === -1) {
                        return `<a class="btn btn-sm btn-primary" ng-click="executeAction('create',dataItem)"><i class="fa fa-plus right-5"></i>Create</a>`
                    } else {
                        if (dataItem.edit) {
                            return `<a class='btn btn-sm default green-stripe' ng-click="executeAction('save',dataItem, $event)">Save</a>
                            <a class='btn btn-sm default' ng-click="executeAction('cancel',dataItem)">Cancel</a>`
                        } else {
                            return `<a class='btn btn-sm default blue-stripe' ng-click="executeAction('edit',dataItem)">Edit</a>
                            <a class='btn btn-sm default red-stripe' ng-click="executeAction('remove',dataItem)" >Delete</a>`
                        }
                    }
                },
            }

        ]
    };

    $scope.userIds = '';
    $scope.idUserOption = '';
    valueTemporary = '';
    departmentIdOld = '';
    userEdit = [];
    $scope.executeAction = async function (action, dataItem) {
        var grid = $("#shiftPlanGrid").data("kendoGrid");
        let data = grid.dataSource._data;
        let hasEditing = false;
        switch (action) {
            case commonData.gridActions.Create:
                hasEditing = _.find(grid.dataSource._data, function (item) {
                    return item.edit === true && item.id != -1;
                });
                if (!hasEditing) {
                    if (dataItem.userIds.length == 0) {
                        dataItem.userIds = "";
                    }
                    var requiredValueFieldsAdd = [
                        {
                            fieldName: 'departmentAddId',
                            title: "Department"
                        },
                        {
                            fieldName: 'userIds',
                            title: "User"
                        }
                    ]
                    var isContinue = validateDataInGrid(dataItem, requiredValueFieldsAdd, [], Notification);
                    if (isContinue) {
                        let model = {
                            departmentId: dataItem.departmentAddId,
                            userIds: dataItem.userIds,
                            isIncludeChildren: dataItem.isIncludeChildren
                        };
                        var resValidate = await settingService.getInstance().shiftPlanSubmitPerson.checkDepartmentExist(model).$promise;
                        if (resValidate.object.count == 0) {
                            let result = await settingService.getInstance().shiftPlanSubmitPerson.createShiftPlanSubmitPerson(model).$promise;
                            if (result.isSuccess) {
                                Notification.success($translate.instant('COMMON_CREATE_SUCCESS'));
                                let grid = $("#shiftPlanGrid").data("kendoGrid");
                                if (grid) {
                                    grid.dataSource.fetch(() => grid.dataSource.page(1));
                                }
                            } else {
                                Notification.error(result.messages[0]);
                            }
                        }
                        else {
                            Notification.error("Department is exists");
                        }
                    }
                } else {
                    Notification.error("Please save selected item before create other item");
                }
                await resetDepartmentAdd();
                break;
            case commonData.gridActions.Save:
                debugger;
                var requiredValueFieldsAdd = [
                    {
                        fieldName: 'departmentId',
                        title: "Department"
                    },
                    {
                        fieldName: 'userIds',
                        title: "User"
                    }
                ]
                if (dataItem.userIds.length == 0) {
                    dataItem.userIds = "";
                }
                if (dataItem.departmentId.length == 0) {
                    var dropdowntree = $(`#departmentTree${dataItem.no}`).data("kendoDropDownTree");
                    dataItem.departmentId = dropdowntree.value();
                }
                var isContinue = validateDataInGrid(dataItem, requiredValueFieldsAdd, [], Notification);
                if (isContinue) {
                    let model = {
                        departmentId: dataItem.departmentId,
                        userIds: $scope.userIds,
                        isIncludeChildren: dataItem.isIncludeChildren,
                        departmentIdOld: departmentIdOld
                    };
                    let validateDepartment = 0;
                    if (model.departmentId != model.departmentIdOld) {
                        var resultValidate = await settingService.getInstance().shiftPlanSubmitPerson.checkDepartmentExist(model).$promise;
                        validateDepartment = resultValidate.object.count;
                    }
                    if (validateDepartment == 0) {
                        let result = await settingService.getInstance().shiftPlanSubmitPerson.saveShiftPlanSubmitPerson(model).$promise;
                        if (result.isSuccess) {
                            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                            let grid = $("#shiftPlanGrid").data("kendoGrid");
                            if (grid) {
                                grid.dataSource.fetch(() => grid.dataSource.page(1));
                            }
                        } else {
                            Notification.error(result.messages[0]);
                        }
                    }
                    else {
                        Notification.error("Department is exists");
                    }
                }
                break;
            case commonData.gridActions.Edit:
                hasEditing = grid.dataSource._data.find(item => item.edit === true && item.id != -1);
                if (!hasEditing) {
                    valueTemporary = Object.assign(valueTemporary, dataItem);
                    departmentIdOld = dataItem.departmentId;
                    $scope.userIds = dataItem.userIds;
                    dataItem.edit = true;
                    userEdit = dataItem.userListViews;
                    $timeout(async function () {
                        var departmentDetail = await settingService.getInstance().departments.getDetailDepartment({
                            id: dataItem.departmentId
                        }).$promise;
                        let id = "#userOption" + dataItem.no;
                        $scope.idUserOption = id;
                        setDepartmentSource(departmentDetail.object.object, 'edit');
                        selectFirstItemOnDepartmentDataSource(dataItem.departmentId, dataItem.no);
                        selectFirstItemOnUserDataSource(dataItem.userIds, id);
                    }, 0);
                    grid.refresh();
                }
                else {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                await resetDepartmentAdd();
                break;
            case commonData.gridActions.Remove:
                hasEditing = grid.dataSource._data.find(item => item.edit === true && item.id != -1);
                var result = validateEdit();
                if (result || hasEditing) {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                else {
                    $scope.dataDelete = dataItem.departmentId;
                    $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
                    $scope.dialog.bind("close", confirm);
                }
                break;
            case commonData.gridActions.Cancel:
                dataItem.edit = false;
                hasEditing = false;
                dataItem.departmentId = valueTemporary.departmentId;
                dataItem.isIncludeChildren = valueTemporary.isIncludeChildren;
                dataItem.userIds = valueTemporary.userIds;
                dataItem.userNames = valueTemporary.userNames;
                grid.refresh();
                break;
        }
    }

    function selectFirstItemOnDepartmentDataSource(id, no) {
        $timeout(function () {
            let departmentInstance = $(`#departmentTree${no}`).data("kendoDropDownTree");
            if (departmentInstance) {
                departmentInstance.value(id);
            }
        }, 0);
    }
    async function resetDepartmentAdd() {
        clearSearchTextOnDropdownTree("departmentTreeAdd");
        if ($scope.dapartments && $scope.dapartments.length > 0) {
            setDepartmentSource($scope.dapartments, 'add')
        } else {
            await getDepartmentByFilter(null, 'add');
        }
    }
    function selectFirstItemOnUserDataSource(userIds, id) {
        $timeout(function () {
            let userOption = $(id).data("kendoMultiSelect");
            if (userOption) {
                userOption.value(userIds);
            }
        }, 0);
    }

    $scope.dataDelete = '';
    confirm = function (e) {
        let grid = $("#shiftPlanGrid").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().shiftPlanSubmitPerson.deleteShiftPlanSubmitPerson({ departmentId: $scope.dataDelete }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                    if (grid) {
                        page = grid.pager.dataSource._page;
                        pageSize = grid.pager.dataSource._pageSize;
                        grid.dataSource.fetch(() => grid.dataSource.page(page));
                    }
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
        console.log(filterUsers);
    }
    function genarateCheckbox(type) {
        return `<input type="checkbox" ng-disabled="${type == 'View'}" ng-model="dataItem.isIncludeChildren" name="isIncludeChildren{{dataItem.no}}"
        id="isIncludeChildren{{dataItem.no}}" class="k-checkbox" style="width: 100%;"/>
        <label class="k-checkbox-label cbox" for="isIncludeChildren{{dataItem.no}}"></label>`;
    }
    function validateEdit() {
        var result = false;
        $scope.shiftPlans.forEach(item => {
            if (item.edit === true && item.id != -1) {
                result = true;
                return result;
            }
        });
        return result
    }

    $scope.departmentEditOptions = {
        dataTextField: "name",
        dataValueField: "id",
        checkboxes: false,
        valuePrimitive: true,
        autoBind: true,
        filter: "contains",
        loadOnDemand: true,
        template: '#: item.code # - #: item.name #',
        dataSource: new kendo.data.HierarchicalDataSource({
            data: $scope.departmentTreeDataSource,
            schema: {
                model: {
                    children: "items"
                }
            }
        }),
        filtering: async function (option) {
            await getDepartmentByFilter(option, 'edit');
        },
        select: async function (option) {
            let dataItem = this.dataItem(option.node)
        },
    };

    $scope.departmentAddOptions = {
        dataTextField: "name",
        dataValueField: "id",
        template: showCustomDepartmentTitle,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        loadOnDemand: true,
        template: '#: item.code # - #: item.name #',
        dataSource: new kendo.data.HierarchicalDataSource({
            schema: {
                model: {
                    children: "items"
                }
            }
        }),
        valueTemplate: (e) => showCustomField(e, ['name']),
        filtering: async function (option) {
            await getDepartmentByFilter(option, 'add');
        }
    };

    $scope.openDepartment = function (id, no) {
        let departmentId = $(`#${no}`).data("kendoDropDownTree");
        if (!departmentId.value()) {
            setDataDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")), no);
        }
    }

    function setDataDepartment(dataDepartment, dropdownId) {
        $timeout(function () {
            $scope.departments = dataDepartment;
            var dataSource = new kendo.data.HierarchicalDataSource({
                data: dataDepartment,
                schema: {
                    model: {
                        children: "items"
                    }
                }
            });
            var dropdownlist = $(`#${dropdownId}`).data("kendoDropDownTree");
            if (dropdownlist) {
                dropdownlist.setDataSource(dataSource);
            }
        }, 0);
    }

    function setDepartmentSource(data, actionType) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: data,
            schema: {
                model: {
                    children: "items"
                }
            },
        });
        $timeout(function () {
            if (actionType == 'edit') {
                $scope.departmentTreeDataSource = dataSource;
            }
            else if (actionType == 'add') {
                $scope.departmentTreeDataSourceAdd = dataSource;
            }
        }, 0)
    }
    $scope.userOptionAdds = {
        dataValueField: "id",
        dataTextField: "fullName",
        template: '#: sapCode # - #: fullName #',
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        loadOnDemand: true,
        filter: "contains",
        dataSource: {
            data: filterUsers
        },
        filtering: async function (e) {
            var filter = e.filter ? e.filter.value : "";
            var multiselect = $("#userOption").data("kendoMultiSelect");
            let model = {
                predicate: "IsActivated=@0",
                predicateParameters: [true],
                order: "Modified desc",
                limit: appSetting.pageSizeDefault,
                page: 1
            }
            if (filter) {
                model.predicate = (model.predicate ? model.predicate + ' and ' : model.predicate) + `(SAPCode.contains(@1) or Email.contains(@1) or FullName.contains(@1))`;
                model.predicateParameters.push(filter.trim());
            }
            var res = await settingService.getInstance().users.getUsers(model).$promise;
            if (res.isSuccess) {
                let data = res.object.data;
                filterUsers = res.object.data;
                if (selectedUsers.length > 0) {
                    _.forEach(selectedUsers, function (item) {
                        if (!data.map(i => i.id).includes(item.id)) {
                            data = _.concat(data, item);
                        }
                    });
                }
                $timeout(function () {
                    multiselect.dataSource.data(data);
                }, 0);
            }
            this.dataSource.filter({
                logic: "or",
                filters: [
                    {
                        field: "sapCode",
                        operator: "contains",
                        value: filter
                    },
                    {
                        field: "fullName",
                        operator: "contains",
                        value: filter
                    }
                ]
            });
        },
        change: function (e) {
            console.log(filterUsers);
            var value = this.value();
            selectedUsers = _.filter(filterUsers, function (o) { return value.includes(o.id) });
        },

    }

    $scope.userOptionEdits = {
        dataValueField: "id",
        dataTextField: "fullName",
        template: '#: sapCode # - #: fullName #',
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    let model = {
                        predicate: "IsActivated=@0",
                        predicateParameters: [true],
                        order: "Modified desc",
                        limit: appSetting.pageSizeDefault,
                        page: 1
                    }
                    var filter = e.data.filter && e.data.filter.filters.length ? e.data.filter.filters[0].value : "";
                    if (filter) {
                        model.predicate = (model.predicate ? model.predicate + ' and ' : model.predicate) + `(LoginName.contains(@1) or SAPCode.contains(@1) or Email.contains(@1) or FullName.contains(@1))`;
                        model.predicateParameters.push(filter.trim());
                    }
                    var res = await settingService.getInstance().users.getUsers(model).$promise;
                    let data = res.object.data;
                    if (res.isSuccess) {
                        if (userEdit.length > 0) {
                            userEdit.forEach(item => {
                                if (!_.map(data, "id").includes(item.id)) {
                                    data.push(item);
                                }
                            });
                        }
                        e.success(data);
                    }
                }
            },
        },
        filter: async function (e) {
            let model = {
                predicate: "IsActivated=@0",
                predicateParameters: [true],
                order: "Modified desc",
                limit: appSetting.pageSizeDefault,
                page: 1
            }
            var filter = e ? e : "";
            if (filter) {
                model.predicate = (model.predicate ? model.predicate + ' and ' : model.predicate) + `(SAPCode.contains(@1) or Email.contains(@1) or FullName.contains(@1))`;
                model.predicateParameters.push(filter.trim());
            }
            var res = await settingService.getInstance().users.getUsers(model).$promise;
            if (res.isSuccess) {
                setDataUserOption(res.object.data, $scope.idUserOption);
            }
        },
        change: function (e) {
            var value = this.value();
            $scope.userIds = value;
        }
    }

    $scope.users = [];
    async function getUsers() {
        kendo.ui.progress($("#loading_submit_person"), true);
        let model = {
            predicate: "IsActivated=@0",
            predicateParameters: [true],
            order: "Modified desc",
            limit: 20,
            page: 1
        }
        var res = await settingService.getInstance().users.getUsers(model).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $timeout(function () {
                $scope.userMultiDataSource = res.object.data;
                filterUsers = res.object.data;
                setDataUserOption(res.object.data, "#userOption");
                kendo.ui.progress($("#loading_submit_person"), false);
            }, 0);

        }
    }

    function setDataUserOption(data, id) {
        $timeout(function () {
            var multiselect = $(id).data("kendoMultiSelect");
            multiselect.setDataSource(data);
        }, 0);
    }

    $scope.shiftPlans = [];
    $scope.currentQuery = {
        predicate: "IsSubmitPerson=@0",
        predicateParameters: [true],
        order: "Modified desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }
    async function getShiftPlan(option) {
        let currentQuery = {
            predicate: "IsSubmitPerson=@0",
            predicateParameters: [true],
            order: "Modified desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }

        if (option) {
            currentQuery.limit = option.data.take;
            currentQuery.page = option.data.page;
        }

        if ($scope.searchTextOnClick) {
            currentQuery.predicate = currentQuery.predicate + ' and (Department.Name.contains(@1) or User.FullName.contains(@1))';
            currentQuery.predicateParameters.push($scope.searchTextOnClick);
        }

        var res = await settingService.getInstance().shiftPlanSubmitPerson.getShiftPlanSubmitPersons(currentQuery).$promise;
        if (res.isSuccess) {
            $scope.shiftPlans = res.object.data;
            $scope.total = res.object.count;
            let count = 1;
            if (option && option.data.page > 1) {
                count = option.data.skip + 1;
            }
            $scope.shiftPlans.forEach(item => {
                item['no'] = count;
                item['id'] = count;
                item['edit'] = false;
                count++;
                item.userNames = splitUserNames(item.userNames);
                item.userListViews.forEach(x => {
                    x['value'] = x.id;
                });
            });
            let dataCreateDefault = {
                id: -1,
                no: '',
                departmentAddId: '',
                edit: true,
                userNames: "",
                userIds: "",
            }
            $scope.shiftPlans.push(dataCreateDefault);
        }
        option.success($scope.shiftPlans);
    }

    $scope.searchText = '';
    $scope.search = function () {
        $scope.searchTextOnClick = $scope.searchText;
        loadPageOne();
    }

    $scope.ifEnter = async function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            $scope.searchTextOnClick = $scope.searchText;
            // Do that thing you finally wanted to do
            loadPageOne();
        }

    }

    function loadPageOne() {
        let grid = $("#shiftPlanGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
    }

    function splitUserNames(data) {
        var result = '';
        if (data.length) {
            data.forEach(item => {
                result = (result ? result + ', ' : result) + item;
            });
        }
        return result;
    }

    async function ngInit() {
        //setDepartmentSource(allDepartments, 'add');
        await getDepartmentByFilter(null, 'add');
        await getUsers();
    }

    ngInit();
    async function getDepartmentByFilter(option, actionType) {
        kendo.ui.progress($("#loading_submit_person"), true);
        let args = {
            predicate: "",
            predicateParameters: [],
            page: 1,
            limit: 20,
            order: ""
        }
        if (option && option.filter.value) {
            args.predicate = "name.contains(@0) or code.contains(@0)";
            args.predicateParameters = [option.filter.value];
            args.limit = appSetting.pageSizeDefault;
        }
        res = await settingService.getInstance().departments.getDepartmentByFilter(args).$promise;
        if (res.isSuccess) {
            if (!option || !option.filter.value) {
                $scope.dapartments = res.object.data;
            }
            setDepartmentSource(res.object.data, actionType);
            kendo.ui.progress($("#loading_submit_person"), false);
        }
    }
    $scope.$on('$locationChangeStart', function (event, next, current) {
        $scope.errors = [];
    });

    $scope.$on("$viewContentLoaded", function () {
    });

    $scope.export = async function () {
        let currentQuery = {
            predicate: "IsSubmitPerson=@0",
            predicateParameters: [true],
            order: "Modified desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }


        if ($scope.searchText) {
            currentQuery.predicate = currentQuery.predicate + ' and (Department.Name.contains(@1) or User.FullName.contains(@1))';
            currentQuery.predicateParameters.push($scope.searchText);
        }

        var res = await fileService.getInstance().processingFiles.export({ type: 38 }, currentQuery).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }

});
