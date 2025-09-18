var ssgApp = angular.module('ssg.dashboardModule', ["kendo.directives"]);
ssgApp.controller('dashboardController', function ($rootScope, $scope, $location, appSetting, workflowService, dashboardService, commonData, settingService, $translate, $timeout) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = 'Dashboard';
    $rootScope.isParentMenu = true;
    $scope.currentQuery = {};
    $scope.tasks = [];
    $scope.myItems = [];
    $scope.query = {};
    $scope.isCheckedAll = false;
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    var allTasks = [];
    $scope.showCheckAll = false;
    var notInSadmin = appSetting.role.Admin | appSetting.role.HR | appSetting.role.CB | appSetting.role.Member;
    this.$onInit = async function () {
        if (appSetting && $rootScope.currentUser) {
            $scope.showCheckAll = appSetting.role.SAdmin == $rootScope.currentUser.role
                || appSetting.role.HRAdmin == $rootScope.currentUser.role
                || !(($rootScope.currentUser.role & notInSadmin) == $rootScope.currentUser.role)
        }
    }

    $scope.statusForm = [];
    $scope.statusOptions = {
        placeholder: "",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            data: $scope.statusForm
        }
    };

    function setDataStatus(statusData) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: statusData
        });
        if ($("#dataStatusId") && $("#dataStatusId").length > 0) {
            var dropdownlist = $("#dataStatusId").data("kendoMultiSelect");
            dropdownlist.setDataSource(dataSource);
        }
    }

    async function getTasks(option) {
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.Order = 'Status desc, Created';
        // build here
        $scope.currentQuery.Predicate = 'isCompleted != @0';
        $scope.currentQuery.PredicateParameters = [true];
        if (!allTasks.length) {
            var res = await workflowService.getInstance().workflows.getTasks({ isSuperAdmin: $scope.isCheckedAll }, $scope.currentQuery).$promise;
            if (res.isSuccess) {
                allTasks = res.object.items;
                //$scope.statusForm = allTasks.map(item => item.status)
                //    .filter((value, index, self) => self.indexOf(value.toLowerCase()) === index);

                var flags = [], outputData = [];
                for (var i = 0; i < allTasks.length; i++) {
                    if (flags[allTasks[i].status.toLowerCase()]) continue;
                    flags[allTasks[i].status.toLowerCase()] = true;
                    outputData.push(allTasks[i].status);
                }
                $scope.statusForm = outputData;
                setDataStatus($scope.statusForm);
            }
        }
        $scope.tasks = _.take(_.drop(allTasks, ($scope.currentQuery.page - 1) * $scope.currentQuery.limit), $scope.currentQuery.limit);
        $scope.totalTask = allTasks.length;
        if (option) {
            option.success($scope.tasks);
        }

    }



    $scope.exportToDoList = function () {
        $scope.toDoListGrid.saveAsExcel();
    }
    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
        if (value) {
            setDataDepartment(allDepartments);
        }
    }
    let columsSearch = [
        'ReferenceNumber',
        'CreatedByFullName',
        'RequestorFullName',
        'RequestedDepartmentName',
        'RequestorUserName',
        'RegionName',
        'Status',
    ];

    $scope.search = async function () {
        let searchResults = allTasks;
        if ($scope.query.keyword) {
            searchResults = _.filter(allTasks, x => {
                return (x.requestedDepartmentName && x.requestedDepartmentName.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.toLowerCase().includes(x.requestedDepartmentName.toLowerCase()))
                    || (x.referenceNumber && x.referenceNumber.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.toLowerCase().includes(x.referenceNumber))
                    || (x.requestorFullName && x.requestorFullName.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.includes(x.requestorFullName.toLowerCase()))
                    || (x.requestorUserName && x.requestorUserName.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.includes(x.requestorUserName.toLowerCase()))
                    || (x.status && x.status.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.toLowerCase().includes(x.status.toLowerCase()))

            });
        }
        if ($scope.query.status) {
            searchResults = searchResults.filter(el => {
                return $scope.query.status.find(element => {
                    return el.status.toLowerCase() === element.toLowerCase();
                });
            });
        }
        if ($scope.query.fromDate) {
            searchResults = _.filter(searchResults, x => {
                return new Date(x.created) > new Date($scope.query.fromDate);
            })
        }
        if ($scope.query.toDate) {
            searchResults = _.filter(searchResults, x => {
                return new Date(x.created) < new Date($scope.query.toDate);
            })
        }

        if ($scope.query.fromDueDate) {
            searchResults = _.filter(searchResults, x => {
                return new Date(x.dueDate) > new Date($scope.query.fromDueDate);
            })
        }
        if ($scope.query.toDueDate) {
            searchResults = _.filter(searchResults, x => {
                return new Date(x.dueDate) < new Date($scope.query.toDueDate);
            })
        }
        $scope.tasks = searchResults;
        initGridRequests($scope.tasks, searchResults.length, 1, true);
    }
    $scope.clearSearch = async function () {
        $scope.query = {
            keyword: '',
            departmentCode: '',
            fromDate: '',
            toDate: '',
            dueDate: '',
            status: [],
        }
        $scope.currentQuery.limit = appSetting.pageSizeDefault / 2;
        $scope.currentQuery.page = 1;
        $scope.currentQuery.Order = 'Status desc, Created desc';
        // build here
        $scope.currentQuery.Predicate = 'isCompleted != @0';
        $scope.currentQuery.PredicateParameters = [true];

        var res = await workflowService.getInstance().workflows.getTasks({ isSuperAdmin: $scope.isCheckedAll }, $scope.currentQuery).$promise;
        if (res.isSuccess) {
            $scope.tasks = res.object.items;
        }
        $scope.totalTask = res.object.count;
        initGridRequests($scope.tasks, $scope.totalTask, $scope.currentQuery.page);
    }
    async function GetTaskFromOption(option) {
        var res = await workflowService.getInstance().workflows.getTasks(option).$promise;
        if (res.isSuccess) {
            $scope.tasks = res.object.items;
        }
        $scope.totalTask = res.object.count;
        initGridRequests($scope.tasks, $scope.totalTask, option.page);
    }

    function initGridRequests(data, total, pageIndex, isSearch = false) {
        let grid = $("#toDoListGridId").data("kendoGrid");
        let dataSourceRequests = new kendo.data.DataSource({
            serverPaging: true,
            transport: {
                read: async function (e) {
                    if (isSearch) {
                        e.success(data);
                    } else {
                        await getTasks(e);
                    }
                }
            },
            schema: {
                total: () => { return total },
                data: () => { return $scope.tasks }
            },
            pageSize: isSearch ? false : appSetting.pageSizeDefault / 2,
            page: pageIndex
        });
        if (grid) {
            grid.setDataSource(dataSourceRequests);
        }
    }
    $scope.departmentOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        template: showCustomDepartmentTitle,
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        filtering: async function (option) {
            await getDepartmentByFilter(option);
        },
        loadOnDemand: true,
        valueTemplate: (e) => showCustomField(e, ['name']),
        change: function (e) {
            if (!e.sender.value()) {
                setDataDepartment(allDepartments);
            }
        }
    };

    async function getDepartmentByFilter(option) {
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
                setDataDepartment(res.object.data);
            }
        }
    }
    function setDataDepartment(dataDepartment) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        var departmentTree = $("#departmentId").data("kendoDropDownTree");
        if (departmentTree) {
            departmentTree.setDataSource(dataSource);
        }
    }

    $scope.toDoListGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: appSetting.pageSizeDefault / 2,
            transport: {
                read: async function (e) {
                    await getTasks(e);
                }
            },
            schema: {
                total: () => { return $scope.totalTask },
                data: () => { return $scope.tasks }
            }
        },
        sortable: false,
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
            title: "",
            width: "250px",
            template: function (data) {
                return `<item-icon type="${data.itemType}"></item-icon>`;
            }
        },
        {
            field: "referenceNumber",
            //title: "Reference Number",
            headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
            width: "180px",
            template: function (data) {
                switch (data.itemType) {
                    case "RequestToHire":
                        return `<a ui-sref="home.requestToHire.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "PromoteAndTransfer":
                        return `<a ui-sref="home.promoteAndTransfer.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "Acting":
                        return `<a ui-sref="home.action.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "LeaveApplication":
                        return `<a ui-sref="home.leavesManagement.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "MissingTimeClock":
                        return `<a ui-sref="home.missingTimelock.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "OvertimeApplication":
                        return `<a ui-sref="home.overtimeApplication.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "ShiftExchangeApplication":
                        return `<a ui-sref="home.shiftExchange.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "ResignationApplication":
                        return `<a ui-sref="home.resignationApplication.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "BusinessTripApplication":
                        return `<a ui-sref="home.business-trip-application.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "TargetPlan":
                        return `<a ui-sref="home.targetPlan.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    default:
                        return `<a href=${data.link} target="_blank">${data.referenceNumber}</a>`;
                        break;
                }
            }
        },
        {
            field: "status",
            //title: "Requestor",
            headerTemplate: $translate.instant('COMMON_STATUS'),
            width: "350px",
            template: function (data) {
                return `${data.status}`;
            }
        },
        {
            field: "requestorFullName",
            //title: "Requestor",
            headerTemplate: $translate.instant('COMMON_REFERENCE_REQUESTOR'),
            width: "200px",
            template: function (data) {
                return `<div kendo-tooltip k-content="'${data.requestorUserName}'" >${data.requestorFullName}</div>`;
            }
        },
        {
            field: "dueDate",
            //title: "Due Date",
            headerTemplate: $translate.instant('COMMON_DUE_DATE'),
            width: "150px",
            template: function (data) {
                return moment(data.dueDate).format('DD/MM/YYYY');
            }
        },
        {
            field: "requestedDepartmentName",
            //title: "Requested Department Name",
            headerTemplate: $translate.instant('COMMON_REQUESTED_DEPARTMENT'),
            width: "350px",
            template: function (data) {
                return `<div kendo-tooltip k-content="'${data.requestedDepartmentCode}'" >${data.requestedDepartmentName}</div>`;
            }
        },
        {
            field: "regionName",
            headerTemplate: $translate.instant('COMMON_REGION'),
            width: "100px",
            template: function (data) {
                return `${data.regionName}`;
            }
        },
        {
            field: "created",
            //title: "Created Date",
            headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
            width: "200px",
            template: function (data) {
                return moment(data.created).format(appSetting.longDateFormat);
            }
        }
        ]
    };


    async function getMyItems(option) {
        var res = await dashboardService.getInstance().dashboard.getMyItems().$promise;
        if (res.isSuccess) {
            $scope.myItems = res.object;
            console.log($scope.myItems);
        }
        $scope.totalItems = $scope.myItems.length;
        option.success($scope.myItems);
    }

    $scope.myItemsGridOptions = {
        dataSource: {
            serverPaging: false,
            pageSize: 10,
            transport: {
                read: async function (e) {
                    await getMyItems(e);
                }
            },
            schema: {
                total: () => { return $scope.totalItems },
                data: () => { return $scope.myItems }
            }
        },
        sortable: false,
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
            title: "",
            width: "150px",
            template: function (data) {
                return `<item-icon type="${data.itemType}"></item-icon>`;
            }
        },
        {
            field: "status",
            //title: "Status",
            headerTemplate: $translate.instant('COMMON_STATUS'),
            width: "250px",
            template: function (data) {
                // switch(data.status){
                //     case "Draft":
                //         return `<workflow-status status="${$translate.instant('STATUS_DRAFT')}"></workflow-status>`;
                //     case "Waiting For":
                //         return `<workflow-status status="${$translate.instant('STATUS_WAITING')}"></workflow-status>`;
                //     case "Approval":
                //         return `<workflow-status status="${$translate.instant('STATUS_APPROVAL')}"></workflow-status>`;
                //     case "Submit":
                //         return `<workflow-status status="${$translate.instant('STATUS_SUBMIT')}"></workflow-status>`;
                //     case "Fill Actual Hour":
                //         return `<workflow-status status="${$translate.instant('STATUS_FILL_ACTUAL')}"></workflow-status>`;
                //     case "Rejected":
                //         return `<workflow-status status="${$translate.instant('STATUS_REJECT')}"></workflow-status>`;
                //     case "Acknowledgement":
                //         return `<workflow-status status="${$translate.instant('STATUS_ACKNOWLEDGEMENT')}"></workflow-status>`;
                //     case "Budget Checker":
                //         return `<workflow-status status="${$translate.instant('STATUS_BUDGET_CHECKER')}"></workflow-status>`;
                //     case "Pending":
                //         return `<workflow-status status="${$translate.instant('STATUS_PENDING')}"></workflow-status>`;
                //     case "Completed":
                //         return `<workflow-status status="${$translate.instant('STATUS_COMPLETED')}"></workflow-status>`;
                //     case "Canceled":
                //         return `<workflow-status status="${$translate.instant('STATUS_CANCEL')}"></workflow-status>`;
                //     case "Requested to change":
                //         return `<workflow-status status="${$translate.instant('STATUS_REQUEST_CHANGE')}"></workflow-status>`;
                //     default:
                //         return `<workflow-status status=""></workflow-status>`;
                // }
                var statusTranslate = $rootScope.getStatusTranslate(data.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                //return `<workflow-status status="${data.status}"></workflow-status>`;
            }
        },
        {
            field: "referenceNumber",
            //title: "Reference Number",
            headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
            width: "180px",
            template: function (data) {
                switch (data.itemType) {
                    case "RequestToHire":
                        return `<a ui-sref="home.requestToHire.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "PromoteAndTransfer":
                        return `<a ui-sref="home.promoteAndTransfer.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "Acting":
                        return `<a ui-sref="home.action.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "LeaveApplication":
                        return `<a ui-sref="home.leavesManagement.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "MissingTimeClock":
                        return `<a ui-sref="home.missingTimelock.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "OvertimeApplication":
                        return `<a ui-sref="home.overtimeApplication.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "ShiftExchangeApplication":
                        return `<a ui-sref="home.shiftExchange.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "ResignationApplication":
                        return `<a ui-sref="home.resignationApplication.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "BusinessTripApplication":
                        return `<a ui-sref="home.business-trip-application.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "TargetPlan":
                        return `<a ui-sref="home.targetPlan.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    default:
                        break;
                }
            }
        },
        {
            field: "created",
            //title: "Created Date",
            headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
            width: "200px",
            template: function (data) {
                return moment(data.created).format(appSetting.longDateFormat);
            }
        },
        {
            field: "modified",
            //title: "Modified Date",
            headerTemplate: $translate.instant('COMMON_MODIFIED_DATE'),
            width: "200px",
            template: function (data) {
                return moment(data.modified).format(appSetting.longDateFormat);
            }
        }
        ]
    };
    $scope.$watch('isCheckedAll', function (newValue, oldValue) {
        $scope.isCheckedAll = newValue;
        allTasks = [];
        $timeout(async function () {
            if (newValue && !oldValue) {
                if ($scope.advancedSearchMode) {
                    await getTasks(null);
                    await $scope.search();
                } else {
                    await getTasks(null);
                    initGridRequests($scope.tasks, $scope.totalTask, 1);
                }
            } else {
                if ($scope.advancedSearchMode) {
                    await getTasks(null);
                    await $scope.search();
                } else if (newValue !== oldValue) {
                    await getTasks(null);
                    initGridRequests($scope.tasks, $scope.totalTask, 1);
                }
            }
        }, 0);
    });

});