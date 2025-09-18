var ssgApp = angular.module('ssg.workflowViewLogDetailModule', ['kendo.directives']);
ssgApp.controller('workFlowViewLogDetailController', function ($rootScope, $scope, appSetting, $stateParams, $state, commonData, Notification, $timeout, settingService, $anchorScroll, trackingHistoryService) {
    // create a message to display in our view
    var ssg = this;
    $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
    $scope.title = '';
    $scope.value = 0;
    var currentAction = null;
    //$scope.title = $stateParams.action.title;
    var Id = $stateParams.referenceValue;
    $scope.filter = {};
    $scope.currentStep = {};
    $scope.searchText = "";
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "WorkflowName DESC",
        limit: appSetting.pageSizeDefault,
        page: 1
    }
    $scope.$watch('currentStep.participantType', function (newValue, oldValue) {
        if (newValue && newValue !== 3) {
            $scope.currentStep.userId = null;
        }
        if (newValue === 6) {
            $scope.currentStep.departmentType = null;
            $scope.currentStep.nextDepartmentType = null;
        }
        if (newValue === 0 || newValue === 1 || newValue === 2 || newValue === 4 || newValue === 5 || newValue === 6 || newValue === 7) {
            $scope.currentStep.jobGrade = null;
            $scope.currentStep.maxJobGrade = null;
        }
        if (newValue === 0 || newValue === 1 || newValue === 2 || newValue === 3 || newValue === 4 || newValue === 6 || newValue === 7 || newValue === 8 || newValue === 9) {
            $scope.currentStep.level = null;
            $scope.currentStep.maxLevel = null;
        }
    });

    $scope.cloneStep = {};
    $scope.model = {
        workflowData: {
            startWorkflowConditions: [],
            steps: [],
        }
    };
    $scope.widget = {};

    $scope.abc = function () {
        $scope.widget.restrictedProperties.dataSource.add({});
    }

    $rootScope.isParentMenu = false;
    isItem = true;

    $scope.widget = {
        confirmWorkFlow: '',
    }

    var requiredFields = [
        {
            fieldName: 'workflowName',
            title: "Workflow Name"
        },
    ]


    $scope.ifEnter = async function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            // Do that thing you finally wanted to do
            await getWorkFlows();
        }
    }
    async function getListWorkFlows() {
        $scope.workFlowGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: appSetting.pageSizeDefault,
                transport: {
                    read: async function (e) {
                        await getWorkFlows(e);
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
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArray
            },
            columns: [{
                field: "no",
                title: "No.",
                width: "50px"
            }, {
                field: "workflowName",
                title: "Flow Name",
                width: "280px"
            },
            {
                field: "step",
                title: "Step",
                width: "180px"
            },
            {
                field: "isActivated",
                title: "Status",
                width: "180px",
                template: function (dataItem) {
                    return `<span>{{dataItem.isActivated==false?'Inactive':'Active'}}</span>`
                }
            },
            {
                //field: "",
                title: "Actions",
                width: "180px",
                template: function (dataItem) {
                    let button = `<a class='btn btn-sm default blue-stripe k-grid-addItems' ui-sref="home.workflowszzyyxx.item({referenceValue: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">Edit</a>
                            <a ng-show="${dataItem.isActivated}"  ng-click="changeStatusItem(dataItem)" class='btn btn-sm default red-stripe' >InActive</a>
                            <a ng-show="${!dataItem.isActivated}"  ng-click="changeStatusItem(dataItem)" class='btn btn-sm default red-stripe' >Active</a>`
                    if ($scope.isITHelpdesk && dataItem.hasTrackingLog) {
                        button += `<a class="btn btn-sm btn-primary" ng-click="showViewLogDetaiLog(dataItem)"><i class="fa fa-book right-5"></i>{{'COMMON_TRACKING_HISTORY' | translate}}</a>`;
                    }
                    return button;
                }
            }
            ]
        };
    }

    $scope.changeStatusItem = function (dataItem) {
        $scope.dataTempory = dataItem;
        ssg.dialog = $rootScope.showConfirmDelete(dataItem['isActivated'] ? 'INACTIVE' : 'ACTIVE', dataItem['isActivated'] ? commonData.confirmContents.inactiveWorkflow : commonData.confirmContents.activeWorkflow, 'Confirm');
        ssg.dialog.bind("close", confirmInactiveWorkflow);
    }

    confirmInactiveWorkflow = async function (e) {
        if (e.data && e.data.value) {
            var grid = $("#grid").data("kendoGrid");
            dataItem = $scope.dataTempory;
            dataItem['isActivated'] = !dataItem['isActivated'];
            var result = await await settingService.getInstance().workflows.changeStatusWorkflow({ Id: dataItem.id, isActivated: dataItem.isActivated }).$promise;
            if (result.isSuccess) {
                Notification.success("The workflow status has been changed");
                $state.reload();
            } else {
                Notification.error(result.messages[0]);
            }
            grid.refresh();
        }
    }

    $scope.search = async function () {
        getWorkFlows()
    }
    //Get data list work flow
    async function getWorkFlows(option) {
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        if ($scope.searchText) {
            $scope.currentQuery.predicate = "WorkflowName.contains(@0)";
            $scope.currentQuery.predicateParameters = [$scope.searchText];
        } else {
            $scope.currentQuery.predicateParameters = [];
            $scope.currentQuery.predicate = "";
        }
        let res = await settingService.getInstance().workflows.getWorkflowTemplates($scope.currentQuery).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = ($scope.currentQuery.page - 1) * $scope.currentQuery.limit + 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });
        }
        $scope.total = res.object.count;
        if (option) {
            option.success($scope.data);
        } else {
            var grid = $("#grid").data("kendoGrid");
            grid.dataSource.read();
            if ($scope.searchText) {
                grid.dataSource.page(1);
            }
        }
    }


    $scope.requestedDepartmentFieldGobalDataSource = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: [
            { code: 'DeptDivisionId', name: 'Department or Division' },
            { code: 'DepartmentId', name: 'Department', },
            { code: 'CurrentDepartmentId', name: 'Current Department' },
            { code: 'NewDeptOrLineId', name: 'New Department' },
            { code: 'MaxDepartmentId', name: 'Max Department' }
        ],
    }

    $scope.requestedDepartmentFieldDataSource = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: [
            { code: 'DeptDivisionId', name: 'Department or Division' },
            { code: 'DepartmentId', name: 'Department', },
            { code: 'CurrentDepartmentId', name: 'Current Department' },
            { code: 'NewDeptOrLineId', name: 'New Department' },
            { code: 'MaxDepartmentId', name: 'Max Department' }
        ],
    }

    //Data Permission for Requestor
    $scope.requestorPermsDataSource = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: [
            {
                code: 1,
                name: 'View'
            },
            {
                code: 2,
                name: 'Edit'
            }
        ]
    }

    //Data Permission for Approver
    $scope.approverPermsDataSource = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: [
            {
                code: 1,
                name: 'View'
            },
            {
                code: 2,
                name: 'Edit'
            }
        ]
    }

    //Data Participant Type
    $scope.participantTypesOptions = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        change: function (e) {
            if (e.sender.value() != 6) {
                $timeout(function () {
                    $scope.currentStep.dataField = null;
                }, 0);
            }
        },
        dataSource: [
            {
                code: 3,
                name: 'Upper Department'
            },
            {
                code: 5,
                name: 'Department Level'
            },
            {
                code: 6,
                name: 'Item User Field'
            },
            {
                code: 8,
                name: 'HR Department'
            },
            {
                code: 9,
                name: 'Performance Department'
            },
            {
                code: 10,
                name: 'CB Department'
            },
            {
                code: 11,
                name: 'Admin Department'
            },
            {
                code: 12,
                name: 'HR Manager Department'
            },
            {
                code: 13,
                name: 'Appraiser 1 Department'
            },
            {
                code: 14,
                name: 'Appraiser 2 Department'
            }
        ]
    }

    //Data Group
    $scope.departmentTypesOptions = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: [{
            code: 1,
            name: 'HOD'
        },
        {
            code: 2,
            name: 'Checker'
        },
        {
            code: 4,
            name: 'Member'
        },
        {
            code: 7,
            name: 'All'
        }
        ]
    }

    $scope.nextDepartmentTypeOption = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: false,
        checkboxes: false,
        autoBind: true,
        dataSource: [{
            code: 1,
            name: 'HOD'
        },
        {
            code: 2,
            name: 'Checker'
        },
        {
            code: 4,
            name: 'Member'
        },
        {
            code: (1 | 2 | 4),
            name: 'All'
        },
        ]
    }

    $scope.jobGradeOption = {
        valuePrimitive: false,
        checkboxes: false,
        autoBind: true,
        dataSource: ['G1', 'G2', 'G3', 'G4', 'G5', 'G6', 'G7', 'G8', 'G9']
    }

    $scope.maxJobGradeOption = {
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        dataSource: ['G1', 'G2', 'G3', 'G4', 'G5', 'G6', 'G7', 'G8', 'G9']
    }
    $scope.participantTypeValueOptions = {
        dataTextField: 'name',
        dataValueField: 'value',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
        filtering: $rootScope.dropdownFilter,
        dataSource: [
            { value: 'FirstAppraiserId', name: '1st Appraiser' },
            { value: 'SecondAppraiserId', name: '2nd Appraiser' },
            { value: 'CreatedById', name: 'Creator' },
            { value: 'UserId', name: 'Requested Employee' }
        ],
    }

    //Department
    $scope.departmentOptions = {
        dataTextField: "name",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            serverPaging: false,
            transport: {
                read: async function (e) {
                    if ($scope.widget.dropdowntree.items().length > 0) {
                        return;
                    }

                    var res = await settingService.getInstance().departments.getDepartmentTree().$promise;
                    if (res.isSuccess) {
                        let dataSource = new kendo.data.HierarchicalDataSource({
                            data: res.object.data
                        });
                        $scope.widget.dropdowntree.setDataSource(dataSource);
                        $scope.widget.dropdowntree.value($scope.currentStep.departmentId);
                        //$timeout(function() {
                        //    $scope.currentStep.departmentId = $scope.cloneStep.departmentId;
                        //}, 0);
                    }
                }
            },
        }
    };

    //Value data default
    $scope.valuesDataSourceDefault = {
        dataTextField: 'name',
        dataValueField: 'code',
        autoBind: true,
        valuePrimitive: false,
        filter: "contains",
        dataSource: {
            serverPaging: false,
            data: []
        }
    };

    $scope.addMore = function () {
        if ($scope.widget.restrictedProperties) {
            $scope.currentStep.restrictedProperties = $scope.widget.restrictedProperties.dataItems();
        }
        $scope.model.workflowData.steps.push({
            stepConditions: [],
            restrictedProperties: [],
            stepNumber: $scope.model.workflowData.steps.length + 1,
        });
        $scope.currentStep = $scope.model.workflowData.steps[$scope.model.workflowData.steps.length - 1];

        if ($scope.widget.restrictedProperties) {
            $scope.widget.restrictedProperties.setDataSource(new kendo.data.DataSource({
                data: $scope.currentStep.restrictedProperties || [],
            }));
        }

        if ($scope.widget.stepConditions) {
            $scope.widget.stepConditions.setDataSource(new kendo.data.DataSource({
                data: $scope.currentStep.stepConditions || [],
            }));
        }
    }

    $scope.addRow = function (kendoGrid) {
        kendoGrid.dataSource.add({});
        //$scope.widget.startWorkflowConditions.dataSource.add({});
    }

    $scope.removeItem = function (dataSource, dataItem) {
        $scope.dialog = $rootScope.showConfirmDelete("DELETE", `Are you sure to delete this items`, 'Confirm');
        $scope.dialog.bind("close", async function (e) {
            if (e.data && e.data.value) {
                dataSource.remove(dataItem);
            }
        });
    }

    $scope.startWorkflowConditionsGridOptions = {
        dataSource: [],
        sortable: false,
        pageable: false,
        columns: [
            {
                field: "fieldName",
                title: 'Field Name',
                template: `<input type="text" class="k-textbox" style="width: 100%;" ng-model="dataItem.fieldName" autocomplete="off" disabled/>`,
            },
            {
                field: "fieldValues",
                title: 'Field Values',
                template: `<input type="text" class="k-textbox" style="width: 100%;" ng-model="dataItem.fieldValues" autocomplete="off" disabled/>`,
            }
        ],
        selectable: true,
    };

    $scope.stepConditionsGridOptions = {
        dataSource: [],
        sortable: false,
        pageable: false,
        columns: [
            {
                field: "fieldName",
                title: 'Field Name',
                //headerTemplate: $translate.instant('COMMON_REASON'),
                template: `<input type="text" class="k-textbox" style="width: 100%;" ng-model="dataItem.fieldName" autocomplete="off"  disabled/>`,
            },
            {
                field: "fieldValues",
                title: 'Field Values',
                //headerTemplate: $translate.instant('COMMON_REASON'),
                template: `<input type="text" class="k-textbox" style="width: 100%;" ng-model="dataItem.fieldValues" autocomplete="off"  disabled/>`,
            }
        ],
        selectable: true,
    };

    $scope.restrictedPropertiesGridOptions = {
        dataSource: [],
        sortable: false,
        pageable: false,
        columns: [
            {
                field: "name",
                title: 'Name',
                template: `<input type="text" class="k-textbox" style="width: 100%;" ng-model="dataItem.name" autocomplete="off" disabled/>`,
            },
            {
                field: "fieldPattern",
                title: 'Field Pattern',
                template: `<input type="text" class="k-textbox" style="width: 100%;" ng-model="dataItem.fieldPattern" autocomplete="off" disabled/>`,
            },
            {
                field: "isRequired",
                title: 'Required',
                template: `<input type="checkbox" name="{{dataItem.uid}}" id="{{dataItem.uid}}" class="k-checkbox" ng-model="dataItem.isRequired" disabled>
                          <label class="k-checkbox-label" for="{{dataItem.uid}}"></label>`,
            }
        ],
        selectable: true,
    };

    $scope.selectStep = function (step) {
        if ($scope.widget.restrictedProperties) {
            $scope.currentStep.restrictedProperties = $scope.widget.restrictedProperties.dataItems();
        }

        if ($scope.widget.stepConditions) {
            $scope.currentStep.stepConditions = $scope.widget.stepConditions.dataItems().map(con => (
                {
                    fieldName: con.fieldName,
                    fieldValues: con.fieldValues ? con.fieldValues.split(',').map(i => i.trim()) : '',
                }
            ));
        }

        $scope.currentStep = step;

        if ($scope.widget.restrictedProperties) {
            $scope.widget.restrictedProperties.setDataSource(new kendo.data.DataSource({
                data: step.restrictedProperties || [],
            }));
        }

        if ($scope.widget.stepConditions) {
            $scope.widget.stepConditions.setDataSource(new kendo.data.DataSource({
                data: step.stepConditions.map(item => ({ ...item, fieldValues: $.type(item.fieldValues) == "array" ? item.fieldValues.join(', ') : item.fieldValues })) || [],
            }));
        }

        $anchorScroll();
    }

    $scope.save = async function (form) {
        $scope.errors = [];
        $scope.errors = $rootScope.validateForm(form, requiredFields, $scope.model);
        if ($scope.errors.length > 0) {
            return;
        }

        $scope.model.workflowData.startWorkflowConditions = $scope.widget.startWorkflowConditions.dataItems().filter(i => i.fieldName || i.fieldValues).map(con => (
            {
                fieldName: con.fieldName,
                fieldValues: con.fieldValues ? con.fieldValues.split(',').map(i => i.trim()) : '',
            }
        ));

        $scope.currentStep.restrictedProperties = $scope.widget.restrictedProperties.dataItems().filter(i => i.name || i.fieldPattern);
        $scope.currentStep.stepConditions = $scope.widget.stepConditions.dataItems().filter(i => i.fieldName);
        $scope.currentStep.stepConditions = $scope.currentStep.stepConditions.map(function (cItem, cIndex) {
            cItem.fieldValues = cItem.fieldValues ? cItem.fieldValues.toString().split(',').map(i => i.trim()) : '';
            return cItem;
        });
        var res;
        if ($scope.model.id)
            res = await settingService.getInstance().workflows.updateWorkflowTemplate($scope.model).$promise;
        else
            res = await settingService.getInstance().workflows.createWorkflowTemplate($scope.model).$promise;
        if (res.isSuccess) {
            if (!$scope.model.id && res.object) {
                $scope.model.id = res.object.id;
            }
            Notification.success("Data successfully saved");
            $state.go('home.workflowszzyyxx.item', { referenceValue: $scope.model.id, itemType: '' }, { reload: true });
        } else { }
    }

    function showConfirm() {
        $scope.widget.confirmWorkFlow.open();
    }

    $scope.onCreateWorkFlowTemplate = function () {
        $timeout(function () {
            showConfirm();
        }, 0);
    }

    $scope.flowTypeOptions = {
        dataTextField: "name",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: {
            serverPaging: false,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    let res = await settingService.getInstance().workflows.getAllItemTypes().$promise;
                    if (res.isSuccess) {
                        var result = res.object.map(i => ({
                            id: i,
                            name: i.replace(/([A-Z])/g, ' $1').replace(/^./, function (str) { return str.toUpperCase(); }),
                        }));
                        result.sort();
                        e.success(result);
                    }
                    else {
                        e.error(res);
                    }
                }
            },
        },
    };

    $scope.stateGo = function (state) {
        $state.go(state);
    }

    this.$onInit = async () => {
        if ($state.current.name === 'home.workflowszzyyxx.viewlog') {
            if (Id) {
                await getWorkFlowById();
            }
        }
    }
    
    async function getWorkFlowById() {
        let res = await await trackingHistoryService.getInstance().trackingHistory.getTrackingHistoryById({ Id }).$promise;
        if (res.isSuccess) {
            $scope.title = 'VIEW LOG DETAIL';
            $scope.workflowData = res.object.workflowData.workflowData;
            $scope.model = res.object.workflowData;
            if ($scope.widget.startWorkflowConditions) {
                $scope.widget.startWorkflowConditions.setDataSource(new kendo.data.DataSource({
                    data: $scope.workflowData.startWorkflowConditions.map(item => ({ ...item, fieldValues: item.fieldValues.join(', ') })) || [],
                }));
            }
            if ($scope.model.workflowData.steps[0]) {
                $timeout(function () {
                    $scope.selectStep($scope.model.workflowData.steps[0]);
                }, 0);
            }
        }
    }
});