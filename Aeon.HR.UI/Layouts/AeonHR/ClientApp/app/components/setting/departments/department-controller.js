var ssgApp = angular.module('ssg.DepartmentModule', ['kendo.directives']);
ssgApp.controller('departmentController', function ($rootScope, $scope, $location, appSetting, localStorageService, Notification, commonData, $stateParams,
    settingService, fileService, $timeout, dashboardService) {
    var ssg = this;
    // $scope.title = 'Department Management';
    $scope.title = 'Department'; // ticket 144
    $scope.dialogActions = [
        "close"
    ]
    ssg.requiredValueFields = [{
        fieldName: 'userSAPCode',
        title: "Employee Code"
    },
    {
        fieldName: 'userFullName',
        title: "Employee Name"
    },
    ];
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Modified DESC",
        limit: appSetting.pageSizeDefault,
        page: 1
    }
    $scope.currentParentId = null;
    $scope.tempEditingUser = {};
    $scope.currentUserDepartmentId = -1;
    $scope.currentUserDepartmentGrade = -1;
    $scope.loadColor = false;
    $scope.editingUser = {}
    allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.model = {
        typeList: [{
            id: 2,
            title: 'Department',
        },
        {
            id: 1,
            title: 'Division',
        }
        ],
        jobGradeList: [],
    }
    $scope.departmentTree = [];
    $scope.departmentTreeByGrade = [];

    $scope.groupOptions = {
        hrTreeAddOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            valuePrimitive: false,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: [],
            template: '<span>#:data.item.code# - #:data.item.name#</span>',
            //disable những item ko phai HR, ko cho select
            template: `<span class="#: data.item.isHr != true ? 'k-state-disabled': ''#">#:data.item.code# - #:data.item.name#</span>`,
            filtering: async function (option) {
                let filter = option.filter && option.filter.value ? option.filter.value : "";
                arg = {
                    predicate: "name.contains(@0) or code.contains(@1)",
                    predicateParameters: [filter, filter],
                    page: 1,
                    limit: appSetting.pageSizeDefault,
                    order: ""
                }
                await filterHrDepartment(arg, $scope.modeDeparment);
            },
            select: function (e) {
                let dropdownlist = $("#hrDepartmentTreeAdd").data("kendoDropDownTree");
                let dataItem = dropdownlist.dataItem(e.node)
                if (!dataItem.isHr) {
                    e.preventDefault();
                }
            },
            loadOnDemand: true
        },
        hrTreeEditOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: [],
            //template: '<span>#:data.item.code# - #:data.item.name#</span>',
            //disable những item ko phai HR, ko cho select
            //template: `<span class="#: data.item.isHr != true ? 'k-state-disabled': ''#">#:data.item.code# - #:data.item.name#</span>`,
            template: function (dataItem) {
                return `<span class="${dataItem.item.enable == false ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
            },
            filtering: async function (option) {
                let filter = option.filter && option.filter.value ? option.filter.value : "";
                arg = {
                    predicate: "name.contains(@0) or code.contains(@1)",
                    predicateParameters: [filter, filter],
                    page: 1,
                    limit: appSetting.pageSizeDefault,
                    order: ""
                }
                await filterHrDepartment(arg, $scope.modeDeparment);
            },
            select: function (e) {
                let dropdownlist = $("#hrDepartmentTree").data("kendoDropDownTree");
                let dataItem = dropdownlist.dataItem(e.node)
                if (!dataItem.isHr) {
                    e.preventDefault();
                }
            },
            loadOnDemand: true,
        },
        departmentListOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            template: showCustomDepartmentTitle,
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: [],
            loadOnDemand: true,
            valueTemplate: (e) => showCustomField(e, ['name']),
            filtering: async function (option) {
                if ($scope.departmentModel.jobGradeId) {
                    let filter = option.filter && option.filter.value ? option.filter.value : "";
                    arg = {
                        predicate: "name.contains(@0) or code.contains(@1)",
                        predicateParameters: [filter, filter],
                        page: 1,
                        limit: appSetting.pageSizeDefault,
                        order: ""
                    }
                    let selectedGrade = _.find($scope.model.jobGradeList, x => { return x.id == $scope.departmentModel.jobGradeId })
                    arg.predicate = `(${arg.predicate}) and JobGrade.Grade > @2`;
                    arg.predicateParameters.push(selectedGrade.grade);
                    await filterDepartment(arg, $scope.modeDeparment);
                }

                //code moi
                // let filter = option.filter && option.filter.value ? option.filter.value : "";
                // if(filter) {
                //     arg = {
                //         predicate: "(name.contains(@0) or code.contains(@1)) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
                //         predicateParameters: [filter.trim(), filter.trim(), filter.trim()],
                //         page: 1,
                //         limit: appSetting.pageSizeDefault,
                //         order: ""
                //     }
                //     await filterDepartment(arg, $scope.modeDeparment);
                // }
                // else {
                //     setDataDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")), $scope.modeDeparment);
                // }
                //

            },
            //template: '<span>#:data.item.code# - #:data.item.name#</span>'
        },
        jobGradeListOptions: {
            placeholder: "",
            dataTextField: "caption",
            dataValueField: "id",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: $scope.model.jobGradeList
        },
        costCenterListOptions: {
            dataTextField: 'code',
            dataValueField: 'id',
            filter: "contains",
            template: '#: code # - #: description #',
            valueTemplate: '#: code # - #: description #',
            autoBind: false,
            valuePrimitive: false,
            dataSource: {
                serverFiltering: true,
                transport: {
                    read: async function (e) {
                        await getCostCenters(e);
                    }
                },
                schema: {
                    data: () => {
                        return $scope.dataCostCenters
                    }
                }
            },
            customFilterFields: ['code', 'description'],
            filtering: filterMultiField
        },
        positionListOptions: {
            // dataTextField: 'showCodeName',
            dataTextField: 'name',
            dataValueField: 'code',
            autoBind: true,
            valuePrimitive: false,
            filter: "contains",
            template: showCodeName,
            //template: '#: code # - #: name #',
            //valueTemplate: '#: code # - #: name #',
            filtering: $rootScope.dropdownFilter,
            dataSource: {
                serverPaging: false,
                pageSize: 100,
                transport: {
                    read: async function (e) {
                        await getPositions(e);
                    }
                },
                schema: {
                    data: () => {
                        return $scope.dataPositions
                    }
                }
            },
            change: function (e) {
                $scope.departmentModel.positionName = e.sender.dataItem().name;
            }
        },
        regionListOptions: {
            placeholder: "",
            dataTextField: 'regionName',
            dataValueField: 'id',
            filter: "contains",
            autoBind: false,
            valuePrimitive: false,
            checkboxes: false,
            dataSource: {
                serverFiltering: false,
                transport: {
                    read: async function (e) {
                        await getRegionList(e);
                    }
                },
                schema: {
                    data: () => {
                        return $scope.regionDataSource;
                    }
                }
            },
            change: function (e) {
                $scope.departmentModel.regionid = e.sender.value();
            }
        }
    }
    $scope.changeDepartment = async function (mode) {
        $timeout(async function () {
            let departmentSelectedId = mode == 'add' ? $scope.departmentModel.parentIdCreate : $scope.departmentModel.parentId;
            if (!departmentSelectedId) {
                if ($scope.departmentModel && $scope.departmentModel.jobGradeId) {
                    let result = await settingService.getInstance().departments.getDepartmentTreeByGrade({
                        jobGradeId: $scope.departmentModel.jobGradeId
                    }).$promise;
                    if (result.isSuccess) {
                        if (result.object.data.length > 0) {
                            $scope.departmentTreeByGrade = result.object.data;
                        } else {
                            $scope.departmentTreeByGrade = [];
                        }
                        setDataDepartment($scope.departmentTreeByGrade, mode);
                    }
                }

            }
        }, 1000)


    }

    $scope.searchText = "";
    $scope.total = 0;
    $scope.data = [];
    $scope.regionDataSource = [];
    $scope.currentColor = '#b60081';
    $scope.errors = [];
    $scope.validateFields = [{
        fieldName: 'code',
        title: 'Department Code',
    },
    {
        fieldName: 'name',
        title: 'Department Name',
    },
    {
        fieldName: 'positionCode',
        title: 'Department Position',
    },
    {
        fieldName: 'type',
        title: 'Department Type',
    },
    {
        fieldName: 'jobGradeId',
        title: 'Department Job Grade',
    },
    {
        fieldName: 'regionId',
        title: 'Region',
    },
        // {
        //     fieldName: 'parentId',
        //     title: 'Department Parent',
        // },
        // {
        //     fieldName: 'sapCode',
        //     title: 'Department SAP Code',
        // },
        // {
        //     fieldName: 'color',
        //     title: 'Department Color',
        // },

    ];
    $scope.currentDepartment = {};
    $scope.departmentModel = {
        id: '',
        code: '',
        name: '',
        positionCode: '',
        positionName: '',
        parentId: '',
        parentIdCreate: '',
        type: 1,
        jobGradeId: '',
        sapCode: '',
        color: '#b60081',
        isStore: false,
        isHr: false,
        isCB: false,
        isPerfomance: false,
        hrDepartmentId: '',
        hrDepartmentIdCreate: '',
        isFacility: false,
        enableForPromoteActing: false
    }
    $scope.userInDepartmentList = [];
    $scope.allUserList = [];
    $scope.groupDataSource = [{
        id: 1,
        title: "HOD"
    },
    {
        id: 2,
        title: "Checker"
    },
    {
        id: 4,
        title: "Member"
    },
    {
        id: 8,
        title: "Assistant"
    }
    ];
    $scope.tempGroupDataSource = [{
        id: 1,
        title: "HOD"
    },
    {
        id: 2,
        title: "Checker"
    },
    {
        id: 4,
        title: "Member"
    },
    {
        id: 8,
        title: "Assistant"
    }
    ];
    $scope.availableEmployees = [];
    $scope.hasAnyUserChange = false;
    $scope.changeRole = function (dataItem, grade) {
        if (dataItem.role === 1) {
            $scope.availableUser = $scope.allUserList.filter(x => x.jobGradeGrade >= grade);
        } else if (dataItem.role === 2) {
            $scope.availableUser = $scope.allUserList.filter(x => x.jobGradeGrade <= grade);
        } else {
            $scope.availableUser = $scope.allUserList;
        }
        // update user by role
        let selectedData = $("#userInDepartmentGrid").data("kendoGrid")
            .dataSource.data().filter(function (item) {
                return item.userId !== dataItem.userId;
            });
        $scope.availableEmployees = $scope.availableUser.filter(function (item) {
            return !(selectedData.some(function (n2) {
                return item.sapCode === n2.userSAPCode;
            }))
        });
        let dataSource = new kendo.data.DataSource({
            data: $scope.availableUser
        });
        if ($scope.sapCodeDropdown) {
            $scope.sapCodeDropdown.setDataSource(dataSource);
        }
    }
    $scope.changeSAPCode = function (dataItem) {
        let employee = _.find($scope.dataUser, function (item) {
            return item.sapCode == dataItem.userSAPCode;
        });
        dataItem.userFullName = employee.fullName;
        dataItem.userId = employee.id;
        dataItem.userJobGradeCaption = employee.jobGradeCaption;
        // update Role
        // if (dataItem.userSAPCode) {
        //     let curreentUser = _.find($scope.allUserList, function (item) {
        //         return item.sapCode == dataItem.userSAPCode;
        //     });
        //     if (curreentUser.jobGradeGrade > $scope.currentUserDepartmentGrade) {
        //         $scope.tempGroupDataSource = [{
        //             id: 1,
        //             title: "HOD"
        //         }];
        //     } else if (curreentUser.jobGradeGrade < $scope.currentUserDepartmentGrade) {
        //         $scope.tempGroupDataSource = [{
        //             id: 2,
        //             title: "Checker"
        //         }];
        //     } else {
        //         $scope.tempGroupDataSource = $scope.groupDataSource;
        //     }
        // }
        // let dataSource = new kendo.data.DataSource({
        //     data: $scope.tempGroupDataSource
        // });
        //dataItem.role = null;
        // if ($scope.roleUserCbb) {
        //     $scope.roleUserCbb.setDataSource(dataSource);
        // }
    }
    $scope.test = "######";
    $scope.sapCodesDataSource = {
        //optionLabel: "Please Select...",
        dataTextField: 'sapCode',
        dataValueField: 'id',
        template: '#: sapCode #',
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
    $scope.userInDepartmentGridOptions = {
        dataSource: {
            serverPaging: true,
            data: $scope.userInDepartmentList,
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
            width: "20px",
        },
        {
            field: "userId",
            hidden: true,
        },
        {
            field: "id",
            hidden: true,
        },
        {
            field: "userFullName",
            title: "Employee Name",
            width: "45px",
            template: function (dataItem) {
                return `<label>{{dataItem.userFullName}}</label>`
            }
        },
        {
            field: "userSAPCode",
            title: "Employee Code",
            width: "55px",
            template: function (dataItem) {
                if (dataItem.selected || dataItem.id === -1) {
                    // let selectedData = $("#userInDepartmentGrid").data("kendoGrid")
                    //     .dataSource.data().filter(function(item) {
                    //         return item.userId !== dataItem.userId;
                    //     });
                    // $scope.availableEmployees = $scope.allUserList.filter(function(item) {
                    //     return !(selectedData.some(function(n2) {
                    //         return item.sapCode === n2.userSAPCode;
                    //     }))
                    // });
                    return `<select kendo-drop-down-list='sapCodeDropdown' style="width: 100%;" name="'userSAPCode'" 
                            data-k-ng-model="dataItem.userSAPCode"
                            k-data-text-field="'sapCode'"
                            k-data-value-field="'sapCode'"                          
                            k-options="sapCodesDataSource" 
                            k-auto-bind="'true'"
                            k-value-primitive="'false'"
                            filter="'contains'",
                            k-on-change="changeSAPCode(dataItem);"
                            > </select>`;
                } else {
                    return `<label style="margin-left: .25em;">${dataItem.userSAPCode}</label>`
                }
            }
        },
        {
            field: "role",
            title: "Group",
            width: "45px",
            template: function (dataItem) {
                if (dataItem.selected || dataItem.id === -1) {
                    if (dataItem.role !== 1 && dataItem.role !== 2 && dataItem.role !== 8) {
                        dataItem.role = 4;
                    }
                    // if (dataItem.userSAPCode) {
                    //     let curreentUser = _.find($scope.allUserList, function (item) {
                    //         return item.sapCode == dataItem.userSAPCode;
                    //     });
                    //     if (curreentUser.jobGradeGrade > $scope.currentUserDepartmentGrade) {
                    //         $scope.tempGroupDataSource = [{
                    //             id: 1,
                    //             title: "HOD"
                    //         }];
                    //     } else if (curreentUser.jobGradeGrade < $scope.currentUserDepartmentGrade) {
                    //         $scope.tempGroupDataSource = [{
                    //             id: 2,
                    //             title: "Checker"
                    //         }];
                    //     } else {
                    //         $scope.tempGroupDataSource = $scope.groupDataSource;
                    //     }
                    // }
                    //k-on-change="changeRole(dataItem,currentUserDepartmentGrade)"
                    return `<select kendo-combo-box='roleUserCbb' style="width: 100%;" name='role' 
                            data-k-ng-model="dataItem.role"
                            k-data-text-field="'title'"
                            k-data-value-field="'id'"
                            k-data-source="groupDataSource"
                            k-auto-bind="'true'"
                            k-value-primitive="'true'"
                            > </select>`;
                } else {
                    let currentRole = "";
                    if (dataItem.role) {
                        if (dataItem.role.id === 1 || dataItem.role === 1) {
                            currentRole = "HOD";
                        } else if (dataItem.role.id === 2 || dataItem.role === 2) {
                            currentRole = "Checker"
                        } else if (dataItem.role.id === 4 || dataItem.role === 4) {
                            currentRole = "Member"
                        } else if (dataItem.role.id === 8 || dataItem.role === 8) {
                            currentRole = "Assistant"
                        }
                    }
                    return `<label style="margin-left: .25em;">${currentRole}</label>`
                }
            },
        },
        {
            field: "isHeadCount",
            title: "Is HeadCount",
            width: "35px",
            template: (dataItem) => {
                let checked = dataItem.isHeadCount ? 'checked' : 'false';
                if (!dataItem.selected) {
                    return `<input ng-model="dataItem.isHeadCount" name='${dataItem.id}' id='${dataItem.id}' class='k-checkbox' checked="${checked}" disabled  type='checkbox'/>
                        <label class='k-checkbox-label' for='${dataItem.id}'></label>`
                } else {
                    return `<input ng-model="dataItem.isHeadCount" name="isHeadCount" id="${dataItem.id}" checked="${checked}" class="k-checkbox" type="checkbox"/>
                        <label class="k-checkbox-label" for="${dataItem.id}"></label>`;
                }
            }
        },
        {
            width: "50px",
            title: "Actions",
            template: function (dataItem) {
                if (dataItem.selected || dataItem.id === -1) {
                    return `<a ng-click="executeActionUser('save',dataItem)" class='btn btn-sm default green-stripe' >Save</a> 
                        <a  ng-click="executeActionUser('cancel',dataItem)"  class='btn btn-sm default' >Cancel</a>`
                }
                if (!dataItem.selected && dataItem.id !== -1) {
                    return `<a ng-click="executeActionUser('edit',dataItem)" class='btn btn-sm default blue-stripe'>Edit</a>
                            <a  ng-click="executeActionUser('remove',dataItem)" class='btn btn-sm default red-stripe' >Delete</a>`
                }

            },
        }
        ]
    }
    $scope.hasEditingUser = function () {
        //let hasEditing = $('#userInDepartmentGrid').find('.k-grid-edit-row');
        let grid = $('#userInDepartmentGrid').data("kendoGrid");
        let hasEditing = _.find(grid.dataSource._data, function (item) {
            return item.selected === true;
        })
        if (hasEditing) {
            return true;
        }
        return false;
    }
    async function getData(option) {
        $scope.hasAnyUserChange = false;
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        if ($scope.searchText) {
            if ($scope.searchText.trim().toLowerCase() === 'division') {
                $scope.currentQuery.predicate = "@0 == Type";
                $scope.currentQuery.predicateParameters = [1];
            } else if ($scope.searchText.trim().toLowerCase() === 'department') {
                $scope.currentQuery.predicateParameters = [2];
                $scope.currentQuery.predicate = "@0 == Type";
            } else {
                $scope.currentQuery.predicate = "(SAPCode.contains(@0) || Code.contains(@0) || Name.contains(@0) || JobGrade.Caption.contains(@0))";
                $scope.currentQuery.predicateParameters = [$scope.searchText];
            }

        } else {
            $scope.currentQuery.predicateParameters = [];
            $scope.currentQuery.predicate = "";
        }
        if ($scope.selectedItem && $scope.selectedItem.id != '0') {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' && ' : $scope.currentQuery.predicate) + `(ParentId=@${$scope.currentQuery.predicateParameters.length})`;
            $scope.currentQuery.predicateParameters.push($scope.selectedItem.id);
        }
        let departRes = await settingService.getInstance().departments.getDepartments($scope.currentQuery).$promise;
        if (departRes.isSuccess) {
            if (departRes.object.data && departRes.object.data.length > 1 && $scope.selectedItem) {
                departRes.object.data = _.filter(departRes.object.data, x => {
                    return x.id != $scope.selectedItem.id;
                })
            }
            if (option && option.data.page > 1) {
                $scope.data = departRes.object.data.map((item, index) => {
                    return {
                        ...item,
                        no: index + (option.data.take * option.data.page - option.data.take) + 1
                    }
                });
            } else {
                $scope.data = departRes.object.data.map((item, index) => {
                    return {
                        ...item,
                        no: index + 1
                    }
                });
            }
            $scope.total = departRes.object.count;
        }
        if (option) {
            option.success($scope.data);
        } else {
            
            let grid = $("#departmentGrid").data("kendoGrid");
            grid.dataSource.read();
            grid.dataSource.page(1);
        }
    }
    async function getDepartmentTree() {
        //let treeRes = await settingService.getInstance().departments.getDepartmentTree().$promise;
        if (sessionStorage.getItemWithSafe("departments")) {
            $timeout(function () {
                $scope.departmentTree = JSON.parse(sessionStorage.getItemWithSafe("departments"));
                let dataSource = new kendo.data.HierarchicalDataSource({
                    data: $scope.departmentTree
                });
                $scope.departmentTreeAdd.setDataSource(dataSource);
                $scope.hrDepartmentTreeAdd.setDataSource(dataSource);
                // Ngăn việc mất selected id của dropdowntree
                //let tempDepartmentId = $scope.departmentModel.parentId;
                //$scope.departmentTreeEdit.setDataSource(dataSource);
                $scope.departmentModel.parentId = $scope.currentParentId;
            }, 0)

        } else {
            // Notification.error(treeRes.messages[0]);
        }

    }
    async function getHrDepartmentTree(mode) {
        let tempHrDepartmentId = $scope.departmentModel.hrDepartmentId;
        // let treeRes = await settingService.getInstance().departments.getDepartmentTree().$promise;
        $scope.departmentTree = JSON.parse(sessionStorage.getItemWithSafe("departments"));
        if (sessionStorage.getItemWithSafe("departments")) {
            $scope.departmentTree = $scope.departmentTree;
            let dataSource = new kendo.data.HierarchicalDataSource({
                data: $scope.departmentTree
            });
            if (mode == 'edit') {
                $scope.hrDepartmentTree.setDataSource(dataSource);
                $scope.departmentModel.hrDepartmentId = tempHrDepartmentId;
            } else {
                $scope.hrDepartmentTreeAdd.setDataSource(dataSource);
            }
        } else {
            //Notification.error(treeRes.messages[0]);
        }
    }
    function setDataDepartment(dataDepartment, mode) {
        if (mode === 'add') {
            let dataSource = new kendo.data.HierarchicalDataSource({
                data: dataDepartment,
                schema: {
                    model: {
                        children: "items"
                    }
                }
            });
            $scope.departmentTreeAdd.setDataSource(dataSource);
        } else if (mode === 'edit') {
            let dataSource = new kendo.data.HierarchicalDataSource({
                data: dataDepartment,
                schema: {
                    model: {
                        children: "items"
                    }
                }
            });
            $scope.departmentTreeEdit.setDataSource(dataSource);
        }
    }

    function setHRDepartment(data, mode) {
        let dataSource = new kendo.data.HierarchicalDataSource({
            data: data,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        if (mode == 'edit') {
            $scope.hrDepartmentTree.setDataSource(dataSource);
        } else {
            $scope.hrDepartmentTreeAdd.setDataSource(dataSource);
        }
    }
    async function filterDepartment(arg, mode) {
        res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
        setDataDepartment(res.object.data, mode);
    }
    async function filterHrDepartment(arg, mode) {
        res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
        setHRDepartment(res.object.data, mode);
    }
    async function beforeEditDepartment(id, hrId, mode = 'add') {
        if (id) {
            let arg = {
                predicate: "id=@0",
                predicateParameters: [id],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
            res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
            setDataDepartment(res.object.data, 'edit');
            //$scope.departmentModel.parentId = id;        
        } else {
            setDataDepartment(allDepartments, mode);
        }
        if (hrId) {
            let arg = {
                predicate: "id=@0",
                predicateParameters: [hrId],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
            res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
            if (res.isSuccess) {
                setHRDepartment(res.object.data, 'edit');
            }
        } else {
            setHRDepartment(allDepartments, mode);
        }
        if ($scope.model.jobGradeList.length == 0) {
            await getJobGradeList();
        }
    }
    async function beforeEditUserMapping() {
        if ($scope.allUserList.length == 0) {
            await getUserList();
        }
    }
    $scope.search = async function () {
        await getData();
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
            $scope.model.jobGradeList = result.object.data;
            let dataSource = new kendo.data.DataSource({
                data: $scope.model.jobGradeList
            });
        } else {
            Notification.error(result.messages[0]);
        }
    }

    function setJobGrade(type) {

    }
    async function getUserList() {
        // let result = await settingService.getInstance().departments.getUserList().$promise;
        // if (result.isSuccess) {
        //     $scope.allUserList = result.object.data.map(function(item) {
        //         return {
        //             ...item,
        //             showValue: item.sapCode
        //         }
        //     });
        //     $scope.availableUser = $scope.allUserList;
        // } else {
        //     Notification.error(result.messages[0]);
        // }
    }
    async function getUsers(option) {
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var arg = {
            predicate: "sapcode.contains(@0)",
            predicateParameters: [filter, filter],
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
            // if ($scope.selectedUser) {
            //     $scope.dataUser.push({ id: $scope.selectedUser.id, sapCode: $scope.selectedUser.userSAPCode, fullName: $scope.selectedUser.userFullName });
            //     selectedUser = null;
            // }
        }
        if (option) {
            option.success($scope.dataUser);
        }
    }

    $scope.onClose = function () {
        $scope.dialogVisible = false;
        $scope.departmentModel.parentId = $scope.currentParentId;
    }
    $scope.onOpen = async function () {
    }

    $scope.saveDepartment = async function () {
        let errors = validateDepartment($scope.departmentModel);
        if (errors.length > 0) {
            let errorList = errors.map(x => {
                return x.controlName + " " + x.errorDetail;
            })
            Notification.error(`Some fields are required: </br>
            <ul>${errorList.join('<br/>')}</ul>`);
        } else {
            let result = await settingService.getInstance().departments.updateDepartment(
                $scope.departmentModel
            ).$promise;
            if (result.isSuccess) {
                // $scope.departmentEditWin.close();
                // $scope.currentColor = '#b60081';
                //if ($scope.currentParentId !== $scope.departmentModel.parentId) {
                //    $scope.currentParentId = $scope.departmentModel.parentId;
                //    getDepartmentTree();
                //}
                getData();
                $scope.departmentEditWin.close();
                watch(true);
                await reloadDepartmentTreeList();
                Notification.success("Data Successfully Saved");
                //code them moi
                $scope.temporaryDepartmentAdd = result.object;
                $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
                    data: [
                        {
                            id: '0',
                            name: 'Aeon',
                            jobGradeCaption: '',
                            isRoot: true,
                            expanded: true,
                            items: JSON.parse(sessionStorage.getItemWithSafe("departments")),
                        }
                    ]
                });
                var treeview = $("#treeview").data("kendoTreeView");
                treeview.setDataSource($scope.dataDepartment)
                if ($scope.selectedItem && $scope.selectedItem.id != '0') {
                    $scope.path.push($scope.selectedItem.id);
                    findPathNode($scope.selectedItem, JSON.parse(sessionStorage.getItemWithSafe("departments")))
                    treeview.expandPath($scope.path.reverse());
                    $scope.path = [];
                    setDataDepartmentSearch(JSON.parse(sessionStorage.getItemWithSafe("departments")));
                }
                else {
                    treeview.expand(".k-item");
                }
                //
            } else {
                Notification.error(result.messages[0]);
            }
        }
    }

    function validateDepartment(data) {
        let errors = [];
        Object.keys(data).forEach(function (fieldName) {
            //có trong mảng cần validate hay k
            let requiredFieldIndex = _.findIndex($scope.validateFields, x => {
                return x.fieldName === fieldName
            });
            if (requiredFieldIndex != -1 && !data[fieldName]) {
                errors.push({
                    fieldName: fieldName,
                    controlName: $scope.validateFields[requiredFieldIndex].title,
                    errorDetail: 'is required',
                });
            }
        });
        if (errors.length === 0) {
            if (!$rootScope.validateSapCode(data.sapCode)) {
                errors.push({
                    fieldName: 'SAPCode',
                    controlName: 'SAPCode',
                    errorDetail: 'has wrong format. (A-Z,0-9, - and _ )',
                });
            }
            if (!$rootScope.validateSapCode(data.code)) {
                errors.push({
                    fieldName: 'Code',
                    controlName: 'Code',
                    errorDetail: 'has wrong format. (A-Z,0-9, - and _ )',
                });
            }
        }
        return errors;
    }
    async function getUserDepartment(id) {
        let grid = $("#userInDepartmentGrid").data("kendoGrid");
        grid.setDataSource(new kendo.data.DataSource());
        let result = await settingService.getInstance().departments.getUserInDepartment({
            id: id
        }).$promise;
        if (result.isSuccess) {
            $scope.userInDepartmentList = result.object.data;
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
    $scope.userManagement = function (id, code, grade) {
        $scope.currentUserDepartmentId = id;
        $scope.currentUserDepartmentGrade = grade;
        $scope.userInDepartmentList = [];
        beforeEditUserMapping();
        getUserDepartment(id);
        $scope.userWin.title("ADD USER TO DEPARTMENT " + code);
        $scope.userWin.options.draggable = false;
        $scope.userWin.center();
        // $scope.userWin.setOptions({
        //     position: {
        //         top: "25%"
        //     }
        // });
        $scope.userWin.open();
    }
    $scope.addDepartment = async function () {
        //$scope.departmentModel.positionName = findPositioName($scope.departmentModel.positionCode);
        let objDepartment = Object.assign({}, $scope.departmentModel);
        objDepartment.id = null;
        objDepartment.parentId = $scope.departmentModel.parentIdCreate;
        objDepartment.hrDepartmentId = $scope.departmentModel.hrDepartmentIdCreate;
        let errors = validateDepartment(objDepartment);
        if (errors.length > 0) {
            let errorList = errors.map(x => {
                return x.controlName + " " + x.errorDetail;
            })
            Notification.error(`Some fields are required: </br>
            <ul>${errorList.join('<br/>')}</ul>`);
        } else {
            let result = await settingService.getInstance().departments.createDepartment({
                ...objDepartment
            }).$promise;
            if (result.isSuccess) {
                $scope.departmentCreateWin.close();
                getData();
                watch(true);
                await reloadDepartmentTreeList();
                Notification.success("Data Successfully Saved");
                //$scope.departmentGrid.dataSource.refresh();
                //$scope.departmentGrid.dataSource.read();
                //getDepartmentTree();

                //code them moi
                $scope.temporaryDepartmentAdd = result.object;
                $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
                    data: [
                        {
                            id: '0',
                            name: 'Aeon',
                            jobGradeCaption: '',
                            isRoot: true,
                            expanded: true,
                            items: JSON.parse(sessionStorage.getItemWithSafe("departments")),
                        }
                    ]
                });
                var treeview = $("#treeview").data("kendoTreeView");
                treeview.setDataSource($scope.dataDepartment)
                if ($scope.selectedItem && $scope.selectedItem.id != '0') {
                    $scope.path.push($scope.selectedItem.id);
                    findPathNode($scope.selectedItem, JSON.parse(sessionStorage.getItemWithSafe("departments")))
                    treeview.expandPath($scope.path.reverse());
                    $scope.path = [];
                    setDataDepartmentSearch(JSON.parse(sessionStorage.getItemWithSafe("departments")));
                }
                else {
                    treeview.expand(".k-item");
                }
                //
            } else {
                Notification.error(result.messages[0]);
            }
        }
    };
    $scope.executeActionUser = function (action, dataItem, $event = null) {
        let grid = $('#userInDepartmentGrid').data("kendoGrid");
        let data = grid.dataSource._data;
        let isEditing = false;
        switch (action) {
            case commonData.gridActions.Save:
                var isContinue = validateDataInGrid(dataItem, ssg.requiredValueFields, [], Notification);
                if (isContinue) {
                    $scope.tempEditingUser = Object.assign($scope.tempEditingUser, dataItem);
                    if (dataItem.id === -1) // Add new
                    {
                        let model = {
                            userId: dataItem.userId,
                            departmentId: $scope.currentUserDepartmentId,
                            role: dataItem.role,
                            isHeadCount: dataItem.isHeadCount
                        };
                        settingService.getInstance().departments.addUserToDepartment({
                            ...model
                        }).$promise.then(function (result) {
                            if (result.isSuccess) {
                                Notification.success("Data Sucessfully Saved");
                                getUserDepartment(model.departmentId);
                                $scope.hasAnyUserChange = true; // update user count status to reload
                            } else {
                                if (result.errorCodes[0] === 505) {
                                    // check headcount in another department
                                    ssg.dialogYN = $rootScope.showConfirmYN("WARNING", result.messages[0]);
                                    ssg.dialogYN.bind("close", confirmMoveHeadCount);
                                } else {
                                    Notification.error(result.messages[0]);
                                }

                            }
                        });
                    } else { // update
                        let model = {
                            id: dataItem.id,
                            userId: dataItem.userId,
                            departmentId: $scope.currentUserDepartmentId,
                            role: dataItem.role,
                            isHeadCount: dataItem.isHeadCount
                        };
                        settingService.getInstance().departments.updateUserInDepartment({
                            ...model
                        }).$promise.then(function (result) {
                            if (result.isSuccess) {
                                Notification.success("Data Sucessfully Saved");
                                getUserDepartment(model.departmentId);
                            } else {
                                if (result.errorCodes[0] === 505) {
                                    // check headcount in another department
                                    ssg.dialogYN = $rootScope.showConfirmYN("WARNING", result.messages[0]);
                                    ssg.dialogYN.bind("close", confirmMoveHeadCount);
                                } else {
                                    Notification.error(result.messages[0]);
                                }

                            }
                        });
                    }
                }
                break;
            case commonData.gridActions.Edit:
                data.forEach((item) => {
                    if (item) {
                        // Trường thêm mới mà chưa lưu thì warning
                        if (item.id === -1) {
                            Notification.error("Please save selected item before edit/delete other item");
                            isEditing = true;
                            //grid.dataSource.remove(item)
                        } else {
                            item.selected = false;
                            //Reset giá trị trường chưa lưu
                            if ($scope.editingUser.uid && item.uid === $scope.editingUser.uid) {
                                Notification.error("Please save selected item before edit/delete other item");
                                isEditing = true;
                                // item.role = $scope.editingUser.role;
                                // item.userSAPCode = $scope.editingUser.userSAPCode;
                                // item.userFullName = $scope.editingUser.userFullName;
                                // item.isHeadCount = $scope.editingUser.isHeadCount;
                                // item.dirtyFields = {};
                            }
                        }
                    }
                })
                if (isEditing === false) {
                    $scope.editingUser = Object.assign($scope.editingUser, dataItem);
                    dataItem.selected = true;
                    grid.refresh();
                    // $scope.selectedUser = dataItem;
                }
                break;
            case commonData.gridActions.Cancel:
                grid.dataSource.read();
                break;
            case commonData.gridActions.Remove:
                data.forEach((item) => {
                    if (item) {
                        // Trường thêm mới mà chưa lưu thì warning
                        if (item.id === -1) {
                            Notification.error("Please save selected item before edit/delete other item");
                            isEditing = true;
                            //grid.dataSource.remove(item)
                        } else {
                            item.selected = false;
                            //Reset giá trị trường chưa lưu
                            if ($scope.editingUser.uid && item.uid === $scope.editingUser.uid) {
                                Notification.error("Please save selected item before edit/delete other item");
                                isEditing = true;
                                // item.role = $scope.editingUser.role;
                                // item.userSAPCode = $scope.editingUser.userSAPCode;
                                // item.userFullName = $scope.editingUser.userFullName;
                                // item.isHeadCount = $scope.editingUser.isHeadCount;
                                // item.dirtyFields = {};
                            }
                        }
                    }
                })
                if (isEditing === false) {
                    $scope.editingUser.id = dataItem.id;
                    ssg.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
                    ssg.dialog.bind("close", confirmDeleteUser);
                }

                break;
        }
    }
    $scope.executeActionDepartment = async function (action, dataItem, $event = null) {
        switch (action) {
            case commonData.gridActions.Edit:
                watch(true);
                $scope.modeDeparment = 'edit';
                await beforeEditDepartment(dataItem.parentId, dataItem.hrDepartmentId, 'edit');
                $scope.departmentModel = _.cloneDeep(dataItem);
                console.log($scope.departmentModel);
                $scope.currentParentId = dataItem.parentId;
                $scope.updateJobGradeList('edit');
                watch(false);
                $scope.departmentEditWin.title("EDIT DEPARTMENT " + dataItem.code);
                $scope.departmentEditWin.center();
                $scope.departmentEditWin.open();

                //await getHrDepartmentTree('edit');
                //await $scope.updateTreeList('edit', true);
                break;
            case commonData.gridActions.Remove:
                $scope.departmentModel.id = dataItem.id;
                let removeContent = commonData.confirmContents.remove;
                if (dataItem.requestToHireId) {
                    removeContent = `This department is created by request to hire ${dataItem.rthReferenceNumber}. Are you sure you want to delete?`
                }
                ssg.dialog = $rootScope.showConfirmDelete("DELETE", removeContent, 'Confirm');
                ssg.dialog.bind("close", confirmDeleteDepartment);
                break;
        }

    }
    $scope.openCreateWindow = async function () {
        $scope.resetDepartmentModel();
        $scope.currentParentId = null;
        //beforeEditDepartment();
        $scope.departmentCreateWin.center();
        $scope.departmentCreateWin.open();
        $scope.modeDeparment = 'add';

        let dataSource = new kendo.data.DataSource({
            data: []
        });
        $scope.jobGradeDropdownAdd.setDataSource(dataSource);
        //code them vao
        if ($scope.selectedItem && $scope.selectedItem.id != '0') {
            // var deparmentTreeAdd = $("#departmentAdd").data("kendoDropDownTree");
            // deparmentTreeAdd.readonly(true);
            // let arg = {
            //     predicate: "Id =@0",
            //     predicateParameters: [$scope.selectedItem.id],
            //     page: 1,
            //     limit: appSetting.pageSizeDefault,
            //     order: ""
            // }
            // res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
            // setDataDepartment(res.object.data, 'add');
            $scope.updateJobGradeListFormAdd($scope.selectedItem.jobGradeGrade);
            $scope.departmentModel.jobGradeId = $scope.selectedItem.jobGradeId;
            $scope.updateTypeFormAdd();
            $timeout(function () {
                $scope.departmentModel.parentIdCreate = $scope.selectedItem.id;
            }, 10)

        }
        else {
            $timeout(function () {
                setDataDepartment(allDepartments, 'add');
            }, 0);
        }
        //type
        document.getElementsByClassName("k-dropdown-wrap k-state-default")[11].style.backgroundColor = "#eee";
        //
    }

    $scope.resetDepartmentModel = function () {
        $scope.departmentModel = {
            id: '',
            code: '',
            name: '',
            positionCode: '',
            positionName: '',
            caption: '',
            parentId: '',
            parentIdCreate: '',
            type: null,
            jobGradeId: null,
            userSAPCode: '',
            color: '#b60081',
            isStore: false,
            isHr: false,
            isCB: false,
            isPerfomance: false,
            hrDepartmentIdCreate: '',
            hrDepartmentId: '',
            enableForPromoteActing: false
        }
    }
    $scope.dialogVisible = false;
    $scope.userInDepartment = function (data) {
        $scope.dialogVisible = true;
    }
    $scope.departmentGridOptions = {
        dataSource: {
            pageSize: appSetting.pageSizeDefault,
            serverPaging: true,
            transport: {
                read: async function (e) {
                    await getData(e);
                }
            },
            schema: {
                total: () => {
                    return $scope.total
                },
                data: () => {
                    return $scope.data
                }
            }
        },
        autoBind: true,
        sortable: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
            //refresh: true,
        },
        columns: [{
            field: "no",
            title: "No.",
            width: "50px",
            editor: function (container, options) {
                $(`<label style="padding-top:.4em">${options.model[options.field]}</label>`).appendTo(container);
            }
        },
        {
            field: "isStore",
            hidden: true
        },
        {
            field: "isHr",
            hidden: true
        },
        {
            field: "hrDepartmentId",
            hidden: true
        },
        {
            field: "code",
            title: "Code",
            width: "120px"
        },
        {
            field: "name",
            title: "Name",
            width: "150px"
        },
        {
            field: "positionCode",
            title: "Position",
            width: "150px",
            template: function (dataItem) {
                return `<label>${dataItem.positionCode} - ${dataItem.positionName}</label>`
            }
        },
        {
            field: "costCenterRecruitmentCode",
            title: "Cost Center",
            width: "150px",
            template: function (dataItem) {
                if (dataItem && dataItem.costCenterRecruitmentCode !== null) {
                    return `<label>${dataItem.costCenterRecruitmentCode} - ${dataItem.costCenterRecruitmentDescription}</label>`
                }
                return '';
            }
        },
        {
            field: "parentName",
            title: 'Department Parent',
            width: "200px",
        },
        {
            field: "isStore",
            title: 'Store/HQ',
            width: "100px",
            template: function (dataItem) {
                return dataItem.isStore ? 'Store' : 'HQ';
            }
        },
        {
            field: "type",
            title: "Type",
            template: function (dataItem) {
                let currentType = _.find($scope.model.typeList, function (item) {
                    return item.id === dataItem.type;
                });
                return `<p>${currentType ? currentType.title : ''}</p>`
            },
            width: "100px",
        }, {
            field: "jobGradeId",
            title: "Job Grade",
            width: "100px",
            template: function (dataItem) {
                // let currentGrade = _.find($scope.model.jobGradeList, function (item) {
                //     return item.id === dataItem.jobGradeId;
                // });
                return `<p>${dataItem.jobGradeCaption ? dataItem.jobGradeCaption : ''}</p>`
            },
        },
        {
            field: "sapCode",
            title: "SAP Code",
            width: "100px"
        },
        {
            field: "regionName",
            title: "Region",
            width: "100px"
        },
        {
            field: "rthReferenceNumber",
            title: "Reference Number",
            width: "170px"
        },
        {
            field: "color",
            title: "Color",
            hidden: true,
            width: "100px"
        },
        {
            field: "userCount",
            title: "User",
            locked: true,
            width: "80px",
            attributes: {
                style: "text-align: left"
            },
            template: function (dataItem) {
                return `<a ng-click="userManagement('${dataItem.id}','${dataItem.code}',${dataItem.jobGradeGrade});">${dataItem.userCount}</a>`
            }
        },
        {
            title: "Actions",
            locked: true,
            attributes: {

            },
            template: function (dataItem) {
                return `<a ng-click="executeActionDepartment('edit',dataItem)" class="btn btn-sm default blue-stripe">Edit</a>
                    <a  ng-click="executeActionDepartment('remove',dataItem)" class="btn btn-sm default red-stripe">Delete</a>`
            },
            width: "150px",
        }
        ],
        editable: false,
    };

    $scope.addUserInDepartment = function () {
        let grid = $("#userInDepartmentGrid").data("kendoGrid");
        let countEmp = grid.dataSource.data();
        let isEditing = _.find(countEmp, function (item) {
            return !item.userSAPCode;
        });
        if (!isEditing) {
            let newRow = {
                id: -1,
                no: grid.dataSource._data.length + 1,
                userSAPCode: '',
                userFullName: '',
                isHeadCount: false,
                role: '',
                selected: true
            };
            let source = grid.dataSource;
            // if (source._data.length >= 4) {
            //     source._data.splice(4, 0, newRow)
            //     $scope.editingUser.uid = source._data[3].uid;
            // } else {
            //     source._data.push(newRow);
            //     $scope.editingUser.uid = source._data[source._data.length - 1].uid;
            // }
            var length = source._data.length;
            var currentPage = source.page();
            var currentPageSize = source.pageSize();
            var insertIndex = currentPage * currentPageSize <= length ? (currentPage * currentPageSize - 1) : (length + 1);
            source.insert(insertIndex, newRow);
            grid.refresh();
        }
    }

    let confirmDeleteDepartment = function (e) {
        let removeId = $scope.departmentModel.id;
        if (e.data && e.data.value) {
            settingService.getInstance().departments.deleteDepartment({
                Id: removeId
            }).$promise.then(async function (result) {
                if (result.isSuccess) {
                    watch(true);
                    await reloadDepartmentTreeList();
                    Notification.success("Data Successfully Deleted");
                    //code them moi
                    $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
                        data: [
                            {
                                id: '0',
                                name: 'Aeon',
                                jobGradeCaption: '',
                                isRoot: true,
                                expanded: true,
                                items: JSON.parse(sessionStorage.getItemWithSafe("departments")),
                            }
                        ]
                    });
                    var treeview = $("#treeview").data("kendoTreeView");
                    treeview.setDataSource($scope.dataDepartment)
                    if ($scope.selectedItem && $scope.selectedItem.id != '0') {
                        $scope.path.push($scope.selectedItem.id);
                        findPathNode($scope.selectedItem, JSON.parse(sessionStorage.getItemWithSafe("departments")))
                        treeview.expandPath($scope.path.reverse());
                        $scope.path = [];
                        setDataDepartmentSearch(JSON.parse(sessionStorage.getItemWithSafe("departments")));
                    }
                    else {
                        treeview.expand(".k-item");
                    }

                    //
                    getData();
                } else {
                    Notification.error(result.messages[0]);
                }
            });
        }
        $scope.departmentModel = {
            id: '',
            name: '',
            caption: '',
            parentId: '',
            parentIdCreate: '',
            type: 1,
            jobGradeId: '',
            sapCode: '',
            color: '#b60081',
            isStore: false,
            isHr: false,
            hrDepartmentId: '',
            hrDepartmentIdCreate: '',
            enableForPromoteActing: false
        }
    }
    let confirmDeleteUser = function (e) {
        let removeId = $scope.editingUser.id;
        if (e.data && e.data.value) {
            settingService.getInstance().departments.removeUserInDepartment({
                Id: removeId
            }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Deleted");
                    getUserDepartment($scope.currentUserDepartmentId);
                    $scope.hasAnyUserChange = true; // update user count status to reload
                } else {
                    Notification.error(result.messages[0]);
                }
            });
        }
        $scope.editingUser = {}
    }
    let confirmMoveHeadCount = async function (e) {
        if (e.data.value) {
            let model = {
                id: $scope.tempEditingUser.id,
                userId: $scope.tempEditingUser.userId,
                departmentId: $scope.currentUserDepartmentId,
                role: $scope.tempEditingUser.role,
                isHeadCount: $scope.tempEditingUser.isHeadCount
            };
            /// move headcount
            if ($scope.tempEditingUser.id === -1) //add
            {
                let result = await settingService.getInstance().departments.moveHeadCountAdd({
                    ...model
                }).$promise;
                if (result.isSuccess) {
                    Notification.success("Data Sucessfully Saved");
                    getUserDepartment($scope.currentUserDepartmentId);
                    $scope.hasAnyUserChange = true;
                } else {
                    Notification.error(result.messages[0]);
                }
            } else { //update
                let result = await settingService.getInstance().departments.moveHeadCountUpdate({
                    ...model
                }).$promise;
                if (result.isSuccess) {
                    Notification.success("Data Sucessfully Saved");
                    getUserDepartment($scope.currentUserDepartmentId);
                    $scope.hasAnyUserChange = true;
                } else {
                    Notification.error(result.messages[0]);
                }
            }
        }
    }

    $scope.ifEnter = async function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            $scope.selectedItem = undefined;
            // Do that thing you finally wanted to do
            await getData();
        }
    }

    $scope.updateJobGradeList = function (action) {
        if ($scope.departmentModel.type == 1) {
            var dataSource = new kendo.data.DataSource({
                data: $scope.model.jobGradeList.filter(item => item.grade <= 4)
            });
        } else if ($scope.departmentModel.type == 2) {
            var dataSource = new kendo.data.DataSource({
                data: $scope.model.jobGradeList.filter(item => item.grade >= 5)
            });
        }
        if (action === 'add') {
            $scope.jobGradeDropdownAdd.setDataSource(dataSource);
        } else if (action === 'edit') {
            // prevent loss selected data after set datasource
            let tempJobgrade = $scope.departmentModel.jobGradeId;
            $scope.jobGradeDropdownEdit.setDataSource(dataSource);
            $scope.departmentModel.jobGradeId = tempJobgrade;
        }
    }
    $scope.updateTreeList = async function (action, reselect) {
        let tempId = $scope.departmentModel.parentId;
        if (!tempId) {
            tempId = $scope.currentParentId;
        }
        if ($scope.departmentModel.jobGradeId) {
            let result = await settingService.getInstance().departments.getDepartmentTreeByGrade({
                jobGradeId: $scope.departmentModel.jobGradeId
            }).$promise;
            if (result.isSuccess) {
                if (result.object.data.length > 0) {
                    $scope.departmentTreeByGrade = result.object.data;
                } else {
                    $scope.departmentTreeByGrade = [];
                }
                if (action === 'add') {
                    let dataSource = new kendo.data.HierarchicalDataSource({
                        data: $scope.departmentTreeByGrade
                    });
                    $scope.departmentTreeAdd.setDataSource(dataSource);
                    if (reselect) {
                        $scope.departmentTreeAdd.value(tempId);
                        $scope.departmentModel.parentId = tempId;
                    }
                } else if (action === 'edit') {
                    let dataSource = new kendo.data.HierarchicalDataSource({
                        data: $scope.departmentTreeByGrade
                    });
                    $scope.departmentTreeEdit.setDataSource(dataSource);
                    if (reselect) {
                        $scope.departmentTreeEdit.value(tempId);
                        $scope.departmentModel.parentId = tempId;
                        $scope.departmentTreeEdit.refresh();
                    }
                }
            } else {
                Notification.error(result.messages[0]);
            }
        }
    }
    $scope.onCloseUserWindow = function () {
        if ($scope.hasAnyUserChange) {
            getData();
        }
    }

    $scope.export = async function () {
        let model = {
            predicate: '',
            predicateParameters: [],
            order: "Modified desc",
        }
        // if ($scope.searchText) {
        //     if ($scope.searchText.trim().toLowerCase() === 'division') {
        //         model.predicate = "@0 == Type";
        //         model.predicateParameters = [1];
        //     } else if ($scope.searchText.trim().toLowerCase() === 'department') {
        //         model.predicateParameters = [2];
        //         model.predicate = "@0 == Type";
        //     } else {
        //         model.predicate = "SAPCode.contains(@0) || Code.contains(@0) || Name.contains(@0) || JobGrade.Caption.contains(@0)";
        //         model.predicateParameters = [$scope.searchText];
        //     }

        // } else {
        //     model.predicateParameters = [];
        //     model.predicate = "";
        // }

        var res = await fileService.getInstance().processingFiles.export({
            type: 13
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }

    $scope.ifEnter = async function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            // Do that thing you finally wanted to do
            await getData();
        }
    }
    //$scope.dataPositions = [];
    async function getPositions(option) {
        var queryArgs = {
            predicate: "MetadataType.Value = @0",
            predicateParameters: ['Position'],
            order: "created desc",
            limit: 1000,
            page: 1
        }
        var res = await settingService.getInstance().recruitment.getPositionLists(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataPositions = [];
            res.object.data.forEach(element => {
                $scope.dataPositions.push(element);
            });
        }
        if (option)
            option.success($scope.dataPositions);
    }

    async function getRegionList(option) {
        let result = await settingService.getInstance().departments.getRegionList({
            predicate: "",
            predicateParameters: [],
            order: "RegionName asc",
            limit: 200,
            page: 1
        }).$promise;
        $scope.regionDataSource = [];
        if (result.isSuccess) {
            $scope.regionDataSource = result.object.data;
        }
        if (option) {
            option.success($scope.regionDataSource);
        }
    }

    async function getCostCenters(option) {
        if (option.data.filter) {
            var filter = option.data.filter.filters.length ? capitalize(option.data.filter.filters[0].value) : "";
        }
        else {
            var filter = '';
        }
        var res = await settingService.getInstance().recruitment.getCostCenterRecruiments({
            predicate: "code.contains(@0) || description.contains(@1)",
            predicateParameters: [filter, filter],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "Modified desc"
        }).$promise;
        if (res.isSuccess) {
            $scope.dataCostCenters = [];
            res.object.data.forEach(element => {
                $scope.dataCostCenters.push(element);
            });
        }
        if (option) {
            option.success($scope.dataCostCenters);
        }
    }

    // function findPositioName(positionCode) {
    //     var result;
    //     if (positionCode) {
    //         result = $scope.dataPositions.find(x => x.code == positionCode);
    //     }
    //     return result.name;
    // }
    async function reloadDepartmentTreeList() {
        let resultRefresh = await dashboardService.getInstance().dashboard.refreshDepartmentNodes().$promise;

        if (resultRefresh.isSuccess) {
            console.log('department already refresh');
        }
        var arg = {
            predicate: "",
            predicateParameters: [],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: ""
        }
        let result = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;

        if (result.isSuccess) {
            //sessionStorage.setItem("departments",JSON.stringify(result.object.data));
            sessionStorage.setItemWithSafe("departments", JSON.stringify(result.object.data), 1000000);
            $timeout(function () {
                watch(false);
            }, 0)
        }
    }
    function watch(value) {
        $rootScope.$watch('isLoading', function (newValue, oldValue) {
            kendo.ui.progress($("#loading"), value);
        });
    }

    $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
        data: [
            {
                id: '0',
                name: 'Aeon',
                jobGradeCaption: '',
                isRoot: true,
                expanded: true,
                items: allDepartments,
            }
        ]
    });

    $scope.treeViewDepartment = {
        loadOnDemand: true,
        dataSource: $scope.dataDepartment,
    }

    $scope.temporaryDepartmentAdd;
    $scope.selectedItem = undefined;

    $scope.createAdd = false;
    $scope.isAeon = true;

    $scope.onchangeItemTreeView = async function (dataItem) {
        $scope.selectedItem = dataItem;
        $scope.searchText = '';
        var treeView = $("#treeview").data("kendoTreeView");
        treeView.expand(treeView.findByUid($scope.selectedItem.uid));
        $scope.createAdd = true;
        if ($scope.selectedItem.id != '0') {
            await getData();
            $scope.isAeon = false;
        } else {
            // $scope.currentQuery = {
            //     page: 1,
            //     limit: 10,
            //     predicate: "JobGrade.Grade = @0",
            //     predicateParameters: [9]
            // }
            // let departRes = await settingService.getInstance().departments.getDepartments($scope.currentQuery).$promise;
            // if (departRes.isSuccess) {
            //     if (departRes.object.data && departRes.object.data.length > 1 && $scope.selectedItem) {
            //         departRes.object.data = _.filter(departRes.object.data, x => {
            //             return x.id != $scope.selectedItem.id;
            //         })
            //     }
            //     $scope.data = departRes.object.data.map((item, index) => {
            //         return {
            //             ...item,
            //             no: index + 1
            //         }
            //     });
            //     $scope.total = departRes.object.count;
            //     let grid = $("#departmentGrid").data("kendoGrid");
            //     grid.dataSource.read();
            //     if ($scope.searchText) {
            //         grid.dataSource.page(1);
            //     }
            // }
            $scope.data = _.filter(JSON.parse(sessionStorage.getItem('departments')), x => {
                return x.jobGradeGrade == 9;
            });
            $scope.total = $scope.data.length;
            let grid = $("#departmentGrid").data("kendoGrid");
            grid.dataSource.read();
            if ($scope.searchText) {
                grid.dataSource.page(1);
            }
            $scope.isAeon = true;
        }
    }


    async function ngOnit() {
        await getJobGradeList();
        await getRegionList();
        var treeview = $("#treeview").data("kendoTreeView");
        if (treeview) {
            treeview.expand(".k-item");
        }
        var firstNode = $("#treeview").find('.k-first');
        treeview.select(firstNode);
        setDataDepartmentSearch(allDepartments);

    }
    ngOnit();
    $scope.arraySearchDepartment = [];
    function findDepartment(departmentId, list) {
        var result;
        for (var i = 0; i < list.length; i++) {
            $scope.arraySearchDepartment.push(list[i].id);
            if (list[i].id == departmentId) {
                result = list[i];
                return result;
            }
            else {
                if (list[i].items.length > 0) {
                    result = findDepartment(departmentId, list[i].items);
                    if (result) {
                        return result;
                    }
                }
            }
        }
        return result;
    }

    function setDataDepartmentSearch(dataDepartment) {
        let dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        var dropdowntree = $("#departmentId").data("kendoDropDownTree");
        if (dropdowntree) {
            dropdowntree.setDataSource(dataSource);
        }
    }

    $scope.path = [];
    function findPathNode(data, departments) {
        var node = '';
        for (var i = 0; i < departments.length; i++) {
            if (departments[i].id == data.parentId) {
                node = departments[i].id;
                $scope.path.push(node);
                if (departments[i].parentId) {
                    node = findPathNode(departments[i], JSON.parse(sessionStorage.getItemWithSafe("departments")));
                }
                else {
                    $scope.path.push('0');
                }
            }
            else {
                if (departments[i].items.length > 0 && data.parentId != allDepartments[0].id) {
                    node = findPathNode(data, departments[i].items);
                    if (node) {
                        $scope.path.push(node);
                    }
                }
            }
        }
        return node;
    }
    $scope.departmentTreeId = '';
    $scope.departmentOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "id",
        template: showCustomDepartmentTitle,
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        filtering: async function (option) {
            await getDepartmentByFilter(option);
        },
        loadOnDemand: true,
        // valueTemplate: (e) => showCustomField(e, ['name']),
        change: function (e) {
            if (!e.sender.value()) {
                clearSearchTextOnDropdownTree('departmentId');
                setDataDepartmentSearch(allDepartments);
            }
            else {
                $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
                    data: [
                        {
                            id: '0',
                            name: 'Aeon',
                            jobGradeCaption: '',
                            isRoot: true,
                            expanded: true,
                            items: JSON.parse(sessionStorage.getItemWithSafe("departments")),
                        }
                    ]
                });
                var treeview = $("#treeview").data("kendoTreeView");
                treeview.setDataSource($scope.dataDepartment)
                var result = findDepartment($scope.departmentTreeId, JSON.parse(sessionStorage.getItemWithSafe("departments")));
                $scope.path.push($scope.departmentTreeId)
                findPathNode(result, JSON.parse(sessionStorage.getItemWithSafe("departments")));
                treeview.expandPath($scope.path.reverse());
                //trỏ vào node đó
                var nodeTree = findNodeInTree(result, treeview.dataSource._data);
                treeview.select(treeview.findByUid(nodeTree.uid));
                //$timeout(function() {
                // var test = $(`span[id=${nodeTree.uid}]`)[0].offsetTop;
                // var test = $(`span[id=${nodeTree.uid}]`)[0].offsetTop;
                // $("div.col-md-4").scrollTop(test);
                //}, 300);
                $scope.path = [];
            }

        }
    };

    function findNodeInTree(data, departments) {
        var node = '';
        for (var i = 0; i < departments.length; i++) {
            if (departments[i].id == data.id) {
                node = departments[i];
                break;
            }
            else {
                if (departments[i].items.length > 0) {
                    node = findNodeInTree(data, departments[i].items);
                    if (node) {
                        break;
                    }
                }
            }
        }
        return node;
    }

    async function getDepartmentByFilter(option) {
        if (!option.filter) {
            option.preventDefault();
        } else {
            let filter = option.filter && option.filter.value ? option.filter.value : "";
            if (filter) {
                arg = {
                    predicate: "(name.contains(@0) or code.contains(@1)) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
                    predicateParameters: [filter.trim(), filter.trim(), filter.trim()],
                    page: 1,
                    limit: appSetting.pageSizeDefault,
                    order: ""
                }
                res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
                if (res.isSuccess) {
                    setDataDepartmentSearch(res.object.data);
                }
            }
            else {
                setDataDepartmentSearch(JSON.parse(sessionStorage.getItemWithSafe("departments")));
            }
        }
    }

    $scope.updateJobGradeListFormAdd = function (data) {
        var dataSource = new kendo.data.DataSource({
            data: $scope.model.jobGradeList.filter(item => item.grade <= data)
        });
        $scope.jobGradeDropdownAdd.setDataSource(dataSource);
    }

    $scope.updateTypeFormAdd = function () {
        var result = $scope.model.jobGradeList.find(x => x.id == $scope.departmentModel.jobGradeId);
        if (result) {
            if (result.grade >= 5) {
                $scope.departmentModel.type = 2;
            }
            else {
                $scope.departmentModel.type = 1;
            }
        }
    }

    $scope.changeDepartmentAdd = function () {
        if ($scope.selectedItem && $scope.selectedItem.id == '0') {
            var result = findDepartment($scope.departmentModel.parentIdCreate, JSON.parse(sessionStorage.getItemWithSafe("departments")));
            $scope.updateJobGradeListFormAdd(result.jobGradeGrade)
        }
    }

    $scope.departmentListOptionsAdd = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "id",
        template: showCustomDepartmentTitle,
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: [],
        loadOnDemand: true,
        valueTemplate: (e) => showCustomField(e, ['name']),
        filtering: async function (option) {

            //code moi
            let filter = option.filter && option.filter.value ? option.filter.value : "";
            if (filter) {
                arg = {
                    predicate: "(name.contains(@0) or code.contains(@1)) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
                    predicateParameters: [filter.trim(), filter.trim(), filter.trim()],
                    page: 1,
                    limit: appSetting.pageSizeDefault,
                    order: ""
                }
                await filterDepartment(arg, $scope.modeDeparment);
            }
            else {
                setDataDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")), $scope.modeDeparment);
            }
            //

        },
        //template: '<span>#:data.item.code# - #:data.item.name#</span>'
    }

});