var ssgApp = angular.module('ssg.budgetModule', ['kendo.directives']);
ssgApp.controller('budgetController', function ($rootScope, $scope, $location, appSetting, Notification, commonData, $stateParams, settingService, fileService, $timeout) {
    var ssg = this;
    ssg.requiredValueFields = [{
        fieldName: 'departmentId',
        title: "Department"
    },
    {
        fieldName: 'quantity',
        title: "Quantity"
    },
    {
        fieldName: 'jobGradeForHeadCountId',
        title: "Job Grade"
    },
    ]
    $scope.currentQuery = {
        Predicate: "",
        PredicateParameters: [],
        Order: "Department.Name asc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    }
    $scope.searchText = "";
    $scope.total = 0;
    $scope.data = [];
    $scope.currentBudgetId = '';
    $scope.title = 'Headcount';
    $scope.jobGradeList = [];
    $scope.jobGradeDataSource = [];
    $scope.jobGradeDataSourceAdd = [];
    $scope.departmentList = [];
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.validateFields = [{
        fieldName: 'department',
        title: 'Department',
    },
    {
        fieldName: 'jobGradeForHeadCountId',
        title: 'Job Grade',
    },
    {
        fieldName: 'quantity',
        title: 'Quantity',
    }
    ];

    $scope.editingBudget = {};

    async function getDepartmentList() {
        let result = await settingService.getInstance().headcount.getDepartments().$promise;
        if (result.isSuccess) {
            $scope.departmentList = result.object.data;
        } else {
            Notification.error(result.messages[0]);
        }
    }

    $scope.departmentTree = [];
    $scope.departmentTreeDataSource = [];
    $scope.departmentTreeDataSourceAdd = [];


    async function getJobGradeList() {
        let result = await settingService.getInstance().headcount.getJobGrades({
            Predicate: "",
            PredicateParameters: [],
            Order: "Grade asc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        }).$promise;
        if (result.isSuccess) {
            $scope.jobGradeList = result.object.data;
        } else {
            Notification.error(result.messages[0]);
        }
    }
    $scope.executeAction = async function (action, dataItem, $event = null) {
        var grid = $("#budgetGrid").data("kendoGrid");
        let data = grid.dataSource._data;
        let hasEditing = false;
        switch (action) {
            case commonData.gridActions.Create:
                $scope.mode = 'create';
                hasEditing = _.find(grid.dataSource._data, function (item) {
                    return item.edit === true && item.id != -1;
                });
                if (!hasEditing) {
                    var requiredValueFieldsAdd = [{
                        fieldName: 'departmentAddId',
                        title: "Department"
                    },
                    {
                        fieldName: 'quantity',
                        title: "Quantity"
                    },
                    {
                        fieldName: 'jobGradeForAddHeadCountId',
                        title: "Job Grade"
                    },
                    ]
                    var isContinue = validateDataInGrid(dataItem, requiredValueFieldsAdd, [], Notification);
                    if (isContinue) {
                        let model = {
                            departmentId: dataItem.departmentAddId,
                            quantity: dataItem.quantity,
                            jobGradeForHeadCountId: dataItem.jobGradeForAddHeadCountId
                        };
                        let result = await settingService.getInstance().headcount.createHeadCount({
                            ...model
                        }).$promise;
                        if (result.isSuccess) {
                            Notification.success("Data Successfully Created");
                            await getBudgets();
                            grid.dataSource.page(1);
                        } else {
                            Notification.error(result.messages[0]);
                        }
                    }
                } else {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                break;
            case commonData.gridActions.Save:
                if (dataItem.departmentId.length == 0) {
                    var dropdowntree = $(`#departmentTree${dataItem.no}`).data("kendoDropDownTree");
                    dataItem.departmentId = dropdowntree.value();
                }
                var isContinue = validateDataInGrid(dataItem, ssg.requiredValueFields, [], Notification);
                $scope.mode = 'save';
                if (isContinue) {
                    // update
                    let model = {
                        id: dataItem.id,
                        departmentId: dataItem.departmentId,
                        quantity: dataItem.quantity,
                        jobGradeForHeadCountId: dataItem.jobGradeForHeadCountId
                    };
                    let result = await settingService.getInstance().headcount.updateHeadCount({
                        ...model
                    }).$promise;
                    if (result.isSuccess) {
                        Notification.success("Data Successfully Saved");
                        dataItem.edit = false;
                        await getBudgets();
                        grid.refresh();
                    } else {
                        Notification.error(result.messages[0]);
                    }
                }
                break;
            case commonData.gridActions.Edit:
                //setDepartmentSource(allDepartments, true);               
                hasEditing = false;
                $scope.mode = 'edit';

                data.forEach((item) => {
                    if (item.uid === $scope.editingBudget.uid) {
                        hasEditing = true;
                        Notification.error("Please save selected item before edit/delete other item");
                    }
                })
                if (hasEditing === false) {
                    $scope.editingBudget = Object.assign($scope.editingBudget, dataItem);
                    dataItem.edit = true;
                    $timeout(async function () {
                        var departmentDetail = await settingService.getInstance().departments.getDetailDepartment({
                            id: dataItem.departmentId
                        }).$promise;
                        setDepartmentSource(departmentDetail.object.object, true);
                        selectFirstItemOnDepartmentDataSource(dataItem.departmentId, dataItem.no);
                    }, 0);
                    let currentDepartmentJobGrade = _.find($scope.jobGradeList, x => { return x.id == dataItem.departmentJobGradeId });
                    $scope.jobGradeDataSource = _.filter($scope.jobGradeList, x => {
                        return x.grade <= currentDepartmentJobGrade.grade;
                    });

                    grid.refresh();
                }
                break;
            case commonData.gridActions.Remove:
                hasEditing = false;
                $scope.mode = 'remove';
                data.forEach((item) => {
                    //item.edit = false;
                    //Reset giá trị trường chưa lưu
                    if (item.uid === $scope.editingBudget.uid) {
                        hasEditing = true;
                        Notification.error("Please save selected item before edit/delete other item");
                        // item.quantity = $scope.editingBudget.quantity;
                        // item.id = $scope.editingBudget.id;
                        // item.departmentId = $scope.editingBudget.departmentId;
                        // item.jobGradeId = $scope.editingBudget.jobGradeId;
                        // item.dirtyFields = {};
                    }
                })
                if (hasEditing === false) {
                    $scope.currentBudgetId = dataItem.id;
                    ssg.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
                    ssg.dialog.bind("close", confirmRemoveBudget);
                }
                break;
            case commonData.gridActions.Cancel:
                $scope.mode = 'cancel';
                dataItem.quantity = $scope.editingBudget.quantity;
                dataItem.departmentId = $scope.editingBudget.departmentId;
                dataItem.jobGradeId = $scope.editingBudget.jobGradeId;
                dataItem.edit = false;
                hasEditing = false;
                $scope.editingBudget = {};
                grid.refresh();
                break;
        }
    }
    $scope.quantityOption = {
        decimals: 0,
        min: 1,
        format: "n0"
    };
    $scope.budgetGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            schema: {
                total: () => {
                    return $scope.total
                },
                data: () => {
                    return $scope.data
                }
            },
            transport: {
                read: async function (e) {
                    await getBudgets(e);
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
        columns: [{
            field: "id",
            hidden: true
        },
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
            field: "jobGradeForHeadCountId",
            title: "Job Grade",
            width: "100px",
            template: function (dataItem) {
                if (dataItem.id != -1 && dataItem.edit) {
                    return `<select kendo-drop-down-list 
                style="width: 100%;"
                k-ng-model="dataItem.jobGradeForHeadCountId"       
                k-data-source="jobGradeDataSource"        
                k-options="jobGradeOptions"
                > </select>`;
                } else if (dataItem.id === -1) {
                    return `<select kendo-drop-down-list 
                    style="width: 100%;"
                    k-ng-model="dataItem.jobGradeForAddHeadCountId"       
                    k-data-source="jobGradeDataSourceAdd"        
                    k-options="jobGradeOptions"
                    > </select>`;
                } else {
                    return `<label>${dataItem.jobGradeCaption}</label>`
                }
            }
        },
        {
            field: "quantity",
            title: "Quantity",
            width: "100px",
            template: function (dataItem) {
                if (dataItem.edit || dataItem.id === -1) {
                    return `<input kendo-numeric-text-box
                        k-options="quantityOption"
                        data-k-ng-model="dataItem.quantity"
                        />`
                } else {
                    return `<label>${dataItem.quantity}</label>`
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
    function showCustomDepartmentTitle(e) {
        let model = e.item;
        if (model.jobGradeGrade < 5) {
            if (model.userCheckedHeadCount) {
                return `${model.code} - ${model.name} - ${model.userCheckedHeadCount}(${model.jobGradeCaption})`
            } else {
                return `${model.code} - ${model.name} (${model.jobGradeCaption})`;
            }
        } else {
            return `${model.code} - ${model.name} (${model.jobGradeCaption})`;
        }
    }
    $scope.departmentEditOptions = {
        dataTextField: "name",
        dataValueField: "id",
        template: showCustomDepartmentTitle,
        checkboxes: false,
        valuePrimitive: true,
        autoBind: true,
        filter: "contains",
        loadOnDemand: true,
        dataSource: new kendo.data.HierarchicalDataSource({
            data: $scope.departmentTreeDataSource,
            schema: {
                model: {
                    children: "items"
                }
            }
        }),
        valueTemplate: (e) => showCustomField(e, ['name']),
        filtering: async function (option) {
            await getDepartmentByFilter(option, true);
        },
        select: async function (option) {
            // let dropdownlist = $("#hrDepartmentTreeAdd").data("kendoDropDownTree");
            let dataItem = this.dataItem(option.node)
            await changeDepartmentEdit(dataItem.id);
        },
        // change: async function (option) {
        //     option.preventDefault();
        //     let id = option.sender.value();
        //     if (!id) { 
        //         setDataDepartment(JSON.parse(localStorageService.get("departments")), option.sender.element[0].id);
        //     }
        // }
    };
    $scope.openDepartment = function (id, no) {
        let departmentId = $(`#${no}`).data("kendoDropDownTree");
        if (!departmentId.value()) {
            setDataDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")), no);
        }
        // if (!id) {
        //     setDataDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")), no);
        // }
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
            data: allDepartments,
            schema: {
                model: {
                    children: "items"
                }
            }
        }),
        valueTemplate: (e) => showCustomField(e, ['name']),
        filtering: async function (option) {
            await getDepartmentByFilter(option, false);
        }
    };


    $scope.jobGradeOptions = {
        dataTextField: "caption",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains"
    };

    $scope.onOpenAdd = async function (dataItem) {
        $scope.mode = 'add';
        // setDepartmentSource(allDepartments);
    }
    $scope.onOpenEdit = async function (dataItem) {
        $scope.mode = 'edit';
        // setDepartmentSource(allDepartments);
    }
    function setDepartmentSource(data, isEdit) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: data,
            schema: {
                model: {
                    children: "items"
                }
            },
        });
        $timeout(function () {
            if ($scope.mode == 'edit') {
                $scope.departmentTreeDataSource = dataSource;
                //$(`#departmentTree${data[0].id}`).data("kendoDropDownTree").select(0); 
            }
            else if ($scope.mode == 'add') {
                $scope.departmentTreeDataSourceAdd = dataSource;
            } else {
                if (isEdit) {
                    $scope.departmentTreeDataSource = dataSource;
                }
                else {
                    $scope.departmentTreeDataSourceAdd = dataSource;
                }
            }
        }, 0)
    }
    function selectFirstItemOnDepartmentDataSource(id, no) {
        $timeout(function () {
            let departmentInstance = $(`#departmentTree${no}`).data("kendoDropDownTree");
            if (departmentInstance) {
                departmentInstance.value(id);
            }
        }, 0);

        //$(`#departmentTree${data[0].id}`).data("kendoDropDownTree").value(data[0].id);
    }

    async function changeDepartmentEdit(id) {
        if (id) {
            var departmentDetail = await settingService.getInstance().departments.getDetailDepartment({
                id
            }).$promise;

            if (departmentDetail.isSuccess && departmentDetail.object && departmentDetail.object.object && departmentDetail.object.object.length > 0) {
                let item = departmentDetail.object.object[0];
                $timeout(function () {
                    $scope.jobGradeDataSource = _.filter($scope.jobGradeList, x => {
                        return x.grade <= item.jobGradeGrade;
                    })
                }, 0)
            }
        } else {
            $scope.mode = 'edit';
            setDepartmentSource(allDepartments, false);
            $timeout(function () {
                $scope.jobGradeDataSource = [];
            }, 0)

        }

    }
    $scope.changeDepartmentAdd = async function (id) {
        if (id) {
            var departmentDetail = await settingService.getInstance().departments.getDetailDepartment({
                id
            }).$promise;

            if (departmentDetail.isSuccess && departmentDetail.object && departmentDetail.object.object && departmentDetail.object.object.length > 0) {
                let item = departmentDetail.object.object[0];
                $timeout(function () {
                    $scope.jobGradeDataSourceAdd = _.filter($scope.jobGradeList, x => {
                        return x.grade <= item.jobGradeGrade;
                    })
                }, 0)
            }
        } else {
            $scope.mode = 'add';
            setDepartmentSource(allDepartments);
            $timeout(function () {
                $scope.jobGradeDataSourceAdd = [];
            }, 0)

        }
    }
    async function getDepartmentByFilter(option, isEdit) {
        if (!option.filter) {
            option.preventDefault();
        } else {
            let filter = option.filter && option.filter.value ? option.filter.value : "";
            arg = {
                predicate: "name.contains(@0) or code.contains(@1)",
                predicateParameters: [filter, filter],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
            res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
            if (res.isSuccess) {
                setDepartmentSource(res.object.data, isEdit);
            }
        }
    }
    let confirmRemoveBudget = async function (e) {
        let removeId = $scope.currentBudgetId;
        let grid = $("#budgetGrid").data("kendoGrid");
        if (e.data && e.data.value) {
            var result = await settingService.getInstance().headcount.deleteHeadCount({ Id: removeId }).$promise;
            if (result.isSuccess) {
                Notification.success("Data Successfully Deleted");
                await getBudgets();
                grid.refresh();
            } else {
                Notification.error(result.messages[0]);
            }
        }
    }
    async function getBudgets(option) {
        if ($scope.departmentList.length < 1) {
            //await getDepartmentList();
            await getJobGradeList();
            //await getDepartmentTree();
        }
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        if ($scope.searchText) {
            $scope.currentQuery.Predicate = "Department.Name.contains(@0) || JobGradeForHeadCount.Caption.contains(@0)";
            $scope.currentQuery.PredicateParameters = [$scope.searchText];
        } else {
            $scope.currentQuery.Predicate = "";
            $scope.currentQuery.PredicateParameters = [];
        }
        var res = await settingService.getInstance().headcount.getHeadCountList(
            $scope.currentQuery
        ).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            if (option && option.data.page > 1) {
                n = option.data.skip  + 1;
            }
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });;
            $scope.data.push({
                id: -1,
                no: '',
                departmentAddId: '',
                // quantity: 1,
                edit: true,
                jobGradeCaption: "",
            });
        }
        $scope.total = res.object.count;
        //
        setDepartmentSource(allDepartments, false);
        //setDepartmentSource(allDepartments);
        if (option) {
            option.success($scope.data);
        } else {
            var grid = $("#budgetGrid").data("kendoGrid");
            grid.dataSource.read();
            grid.dataSource.page($scope.currentQuery.page);

        }
    }
    $scope.search = function () {
        getBudgets();
    }

    $scope.ifEnter = async function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            // Do that thing you finally wanted to do
            getBudgets();
        }

    }

    $scope.export = async function () {
        let option = $scope.currentQuery;
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.HEADCOUNT }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.import = async function () {

    }


});