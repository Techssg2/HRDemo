var ssgApp = angular.module('ssg.btaPolicySpecialCaseModule', ['kendo.directives']);
ssgApp.controller('btaPolicySpecialCaseController', function ($rootScope, recruitmentService, $scope, $location, appSetting, Notification, commonData, $stateParams, settingService, fileService, $timeout) {
    // create a message to display in our view
    var ssg = this;
    $scope.advancedSearchMode = false;
    $rootScope.isParentMenu = false;
    $scope.title = $stateParams.action.title;

    // general actions
    $scope.actions = {
        save: save,
        cancel: cancel,
        edit: edit,
        deleteRecord: deleteRecord,
        create: create,
        ifEnter: ifEnter
    };

    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Modified desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };
    $scope.numeric = {
        format: '',
        min: 0
    }

    $scope.total = 0;
    $scope.keyword = '';
    $scope.dataBTAPolicySpecialCases = [];

    $scope.jobGradeOptions = {
        dataTextField: "caption",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.jobGradeDataSource
    };
    //API
    async function getListBTAPolicySpecialCases(option) {
        let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;
        var result = await settingService.getInstance().btaPolicy.getListBTAPolicySpecialCases($scope.currentQuery).$promise;

        if (result.isSuccess) {
            $scope.dataBTAPolicySpecialCases = result.object.data;
            var count = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;
            $scope.dataBTAPolicySpecialCases.map(async (x) => {
                x["select"] = false;
                x["no"] = count;
                x['userJobGradeCaption'] = x.jobGrade ? x.jobGrade.caption : null;
                x['userDepartmentName'] = x.department ? x.department.name : null;
                count = count + 1;
            });
            let nValue = {
                id: '',
                sapCode: '',
                budgetTo: '',
                select: true,
                positionName : '',
                fullName : '',
                userJobGradeCaption : '',
                userDepartmentName : ''
            };
            $scope.dataBTAPolicySpecialCases.push(nValue);
            $scope.total = result.object.count;
            if (option) {
                option.success($scope.dataBTAPolicySpecialCases);
            }
            else {
                let grid = $("#grid").data("kendoGrid");
                grid.dataSource.read();
                grid.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    function buildArgs(pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        if ($scope.keyword) {
            predicate.push('(sapCode.contains(@0) || fullName.contains(@0) || user.loginName.contains(@0))');
            predicateParameters.push($scope.keyword);
        }
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }
    //Data SAP CODE
    function showCustomSapCodeTitle(model) {
        if (model.sapCode) {
            return `${model.sapCode} - ${model.fullName}`
        } else {
            return `${model.fullName}`
        }
    }
    $scope.sapCodesDataSource = {
        dataTextField: 'sapCode',
        dataValueField: 'id',
        template: showCustomSapCodeTitle,
        valueTemplate: '#: sapCode #',
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getUsers(e);
                }
            }
        },
        change: async function (e) {
            alert('change');
        }
    };
    async function getDepartment(dataItem) {
        if (dataItem.userDepartmentCode != null) {
            var dept = await settingService.getInstance().departments.getDepartmentByCode({ deptCode: dataItem.userDepartmentCode }).$promise;
            if (dept.isSuccess && dept.object != null) {
                dataItem.userDepartmentId = dept.object.id;
            }
        }
    }
    async function getPosition(dataItem) {
        if (dataItem.userDepartmentId) {
            var position = await recruitmentService.getInstance().position.getPositionByDepartmentId({ deptId: dataItem.userDepartmentId }).$promise;
            if (position.isSuccess && position.object != null) {
                 return position.object.positionName;
            }
        }
    }
    function resetValuesOnChanges(dataItem) {
        dataItem.userDepartmentId = null;
        dataItem.positionName = '';
        dataItem.fullName = '';
        dataItem.userId = null;
        dataItem.userJobGradeCaption = '';
        dataItem.userDepartmentName = '';
        dataItem.jobGradeId = null;
        dataItem.userDepartmentCode = '';
    }
    $scope.changeSAPCode = async function (dataItem) {
        //var currentItem = jQuery.extend({}, dataItem);;
        let employee = _.find($scope.dataUser, function (item) {
            return item.sapCode == dataItem.sapCode;
        });
        
        if (typeof (employee) !== "undefined" && employee != null) {
            dataItem.fullName = employee.fullName;
            dataItem.userId = employee.id;
            dataItem.userJobGradeCaption = employee.userDepartmentMappingsJobGradeCaption;
            dataItem.userDepartmentName = employee.userDepartmentMappingsDepartmentName;
            dataItem.jobGradeId = employee.userDepartmentMappingsJobGradeId;
            dataItem.userDepartmentCode = employee.userDepartmentMappingsDepartmentCode;
            await getDepartment(dataItem);
            dataItem.positionName = await getPosition(dataItem);
        }
        else
            resetValuesOnChanges(dataItem);
        $scope.$apply();
    }
    async function getUsers(option) {
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var arg = {
            predicate: "(sapcode.contains(@0) or fullName.contains(@1)) and IsActivated = @2",
            predicateParameters: [filter, filter,true],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "sapcode asc"
        }
        var res = await settingService.getInstance().users.getUsers(arg).$promise;
        if (res.isSuccess) {
            $scope.dataUser = [];
            res.object.data.forEach(element => {
                if (element.isActivated) {
                    $scope.dataUser.push(element);
                }
            });
            if ($scope.selectedUserId) {
                let index = _.findIndex($scope.dataUser, x => {
                    return x.id == $scope.selectedUserId;
                });
                if (index == -1) {
                    $scope.dataUser.push({ id: $scope.detail.userId, fullName: $scope.detail.fullName, sapCode: $scope.detail.employeeCode });
                }
                $scope.selectedUserId = '';
            }
        }
        if (option) {
            option.success($scope.dataUser);
        }
    }
    // Get list
    $scope.BTAPolicySpecialCaseOption = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getListBTAPolicySpecialCases(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataBTAPolicySpecialCases }
            }
        },
        sortable: false,
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
                title: "NO.",
                width: "30px",
                editor: function (container, options) {
                    $(`<label>${options.model[options.field]}</label>`).appendTo(
                        container
                    );
                }
            },
            {
                field: "sapCode",
                title: "Employee Code",
                width: "140px",
                template: function (dataItem) {
                    if (dataItem.select && dataItem.id === '') {
                        return `<select kendo-drop-down-list='sapCodeDropdown' style="width: 100%;" name="'userSAPCode'" 
                            data-k-ng-model="dataItem.sapCode"
                            k-data-text-field="'sapCode'"
                            k-data-value-field="'sapCode'"                          
                            k-options="sapCodesDataSource" 
                            k-auto-bind="'true'"
                            k-value-primitive="'false'"
                            filter="'contains'",
                            k-on-change="changeSAPCode(dataItem);"
                            > </select>`;
                    } else {
                        return `<label style="margin-left: .25em;">${dataItem.sapCode}</label>`
                    }
                }
            },
            {
                field: "fullName",
                title: "Employee Name",
                width: "100px",
                template: function (dataItem) {
                    return `<label>{{dataItem.fullName}}</label>`
                }
            },
            {
                field: "userDepartmentName",
                title: "Department Name",
                width: "140px",
                template: function (dataItem) {
                    
                    return `<label>{{dataItem.userDepartmentName}}</label>`
                }
            },
            {
                field: "positionName",
                title: "Position",
                width: "100px",
                template: function (dataItem) {
                    return `<label>{{dataItem.positionName}}</label>`
                }
            },
            {
                field: "userJobGradeCaption",
                title: "Job Grade",
                width: "70px",
                template: function (dataItem) {
                    return `<label>{{dataItem.userJobGradeCaption}}</label>`
                }
            },
            {
                field: "budgetTo",
                title: "Budget Limit",
                width: "100px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input kendo-numeric-text-box k-min="0" id="budgetTo" name="budgetTo" k-ng-model="dataItem.budgetTo" style="width: 100%;"/>`
                    }
                    else
                        return "<label>{{ " + dataItem.budgetTo + "| number" + "}}</label>";
                }
            },
            {
                title: "ACTIONS",
                width: "120px",
                template: function (dataItem) {
                    if (!dataItem.id) {
                        return `
                        <a class="btn btn-sm btn-primary " ng-click="actions.create(dataItem)"><i class="fa fa-plus right-5"></i>Create</a>
                        `
                    }
                    if (dataItem.select) {
                        return `
                        <a class="btn btn-sm default green-stripe " ng-click="actions.save(dataItem)">Save</a>
                        <a class="btn btn-sm default " ng-click="actions.cancel(dataItem)">Cancel</a>
                `;
                    } else {
                        return `
                        <a class="btn btn-sm default blue-stripe " ng-click="actions.edit(dataItem)">Edit</a>
                        <a class="btn btn-sm default red-stripe " ng-click="actions.deleteRecord(dataItem)">Delete</a>
                `;
                    }
                }
            }
        ]
    };
    function checkEdit() {
        var result = false;
        $scope.dataBTAPolicySpecialCases.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }
    let requiredFields = [
        {
            fieldName: "sapCode",
            title: "User SAP Code"
        },
        {
            fieldName: "budgetTo",
            title: "Budget To"
        }
    ];
    async function create(model) {
        var editExist = checkEdit();
        if (editExist) {
            Notification.error("Please save selected item before create other item");
        }
        else {
            var hasError = false;
            let errors = $rootScope.validateInRecruitment(requiredFields, model);
            if (errors.length > 0) {
                let errorList = errors.filter(function (x) {
                    if (x.fieldName === "budgetTo" && model.budgetTo === 0) {
                        return false;
                    }
                    return true;
                }).map(x => {
                    return x.controlName + " " + x.errorDetail;
                });
                if (errorList.length > 0) {
                    hasError = true;
                    Notification.error(`Some fields are required: </br>
                    <ul>${errorList.join('<br/>')}</ul>`);
                }
            }
            if (!hasError) {
                var checkCode = await findBTAPolicySpecialCasesByUserSAPCode(model.sapCode);
                if (checkCode) {
                    Notification.error("SAP Code " + model.sapCode + " has existed in BTA Policy Special Cases.");
                }
                else {
                    var item = {
                        Id: model.id,
                        SapCode: model.sapCode,
                        FullName: model.fullName,
                        DepartmentId: model.userDepartmentId,
                        JobGradeId: model.jobGradeId,
                        BudgetFrom: 0,
                        BudgetTo: model.budgetTo,
                        PositionName: model.positionName,
                        UserId: model.userId
                    };
                    settingService.getInstance().btaPolicy.createBTAPolicySpecialCases(item).$promise.then(function (result) {
                        if (result.isSuccess) {
                            Notification.success("Data Successfully Saved");
                            loadPageOne($scope.keyword = '');
                            return true;
                        }
                    });
                }
            }
        }
    };
    async function findBTAPolicySpecialCasesByUserSAPCode(userSAPCode) {
        try {
            var result = await settingService.getInstance().btaPolicy.getBTAPolicySpecialCasesByUserSAPCode({ userSAPCode: userSAPCode }).$promise;
            if (result.object.data !== null && result.object.count >= 1) {
                return true;
            }
            return false;
        } catch (e) {
            Notification.error("Something wrong!");
            return false;
        }
    }
    async function ifEnter($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            loadPageOne();
        }
    }

    $scope.search = async function () {
        loadPageOne();
    }

    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }
    $scope.dataDelete = [];
    function deleteRecord(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            $scope.dataDelete = model;
            $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
            $scope.dialog.bind("close", confirm);
        }
    }
    function validateEdit() {
        var result = false;
        $scope.dataBTAPolicySpecialCases.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    confirm = function (e) {
        let grid = $("#grid").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().btaPolicy.deleteBTAPolicySpecialCases({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Saved");
                    getListBTAPolicySpecialCases();
                    grid.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }
    async function save(model) {
        let errors = $rootScope.validateInRecruitment(requiredFields, model);
        var hasError = false;
        if (errors.length > 0) {
            let errorList = errors.filter(function (x) {
                if (x.fieldName === "budgetTo" && model.budgetTo === 0) {
                    return false;
                }
                return true;
            }).map(x => {
                return x.controlName + " " + x.errorDetail;
            });
            if (errorList.length > 0) {
                hasError = true;
                Notification.error(`Some fields are required: </br>
                    <ul>${errorList.join('<br/>')}</ul>`);
            }

        }
        if (!hasError) {
            var item = {
                Id: model.id,
                SapCode: model.sapCode,
                FullName: model.fullName,
                DepartmentId: model.departmentId,
                JobGradeId: model.jobGradeId,
                BudgetTo: model.budgetTo,
                PositionName: model.positionName
            };
            settingService.getInstance().btaPolicy.updateBTAPolicySpecialCases(item).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Saved");
                    getListBTAPolicySpecialCases();
                    return true;
                }
            });

        }
    }
    function cancel(model) {
        model['select'] = false;
        var item = $scope.dataBTAPolicySpecialCases.find(x => x.id === model.id);
        item.select = false;
        model.sapCode = item.sapCode;
        model.fullName = item.fullName;
        model.departmentId = item.departmentId;
        model.jobGradeId = item.jobGradeId;
        model.budgetTo = item.budgetTo;
        model.positionName = item.positionName;
        let grid = $("#grid").data("kendoGrid");
        grid.refresh();
    }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            model['select'] = true;
            var item = $scope.dataBTAPolicySpecialCases.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#grid").data("kendoGrid");
            grid.refresh();
        }

    }

   $scope.export = async function () {
        let model = {
            predicate: '',
            predicateParameters: [],
            order: "Modified desc",
        }

        if ($scope.keyword) {
            model.predicate = '(sapCode.contains(@0) || fullName.contains(@0) || user.loginName.contains(@0))';
            model.predicateParameters.push($scope.keyword);
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.BTAPOLICYSPECIALCASES
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }
    
    async function ngInit() {
        
    }

    ngInit();
});