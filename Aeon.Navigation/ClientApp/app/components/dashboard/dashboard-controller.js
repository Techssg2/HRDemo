var ssgApp = angular.module('ssg.dashboardModule', ["kendo.directives"]);
ssgApp.controller('dashboardController', function ($rootScope, $scope, $location, appSetting, workflowService, dashboardService, commonData, settingService, $translate, $timeout, $window) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = 'Dashboard';
    $rootScope.isParentMenu = true;
    $scope.currentQuery = {};
    $scope.tasks = [];
    $scope.myItems = [];
    $scope.query = {};
    $scope.isCheckedAll = false;
    $scope.allTask = [];
    var allDepartments = sessionStorage.getItemWithSafe("departments") != null ? JSON.parse(sessionStorage.getItemWithSafe("departments")) : [];
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
                $scope.allTask = allTasks;
                //$scope.statusForm = allTasks.map(item => item.status)
                //    .filter((value, index, self) => self.indexOf(value.toLowerCase()) === index);

                var flags = [], outputData = [];
                for (var i = 0; i < allTasks.length; i++) {
                    if (allTasks[i].status) {
                        if (flags[allTasks[i].status.toLowerCase()]) continue;
                        flags[allTasks[i].status.toLowerCase()] = true;
                        outputData.push(allTasks[i].status);
                    }
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
        if ($scope.query.module && $scope.query.module.length) {
            searchResults = searchResults.filter(el => {
                return $scope.query.module.find(element => {
                    return el.module.toLowerCase() === element.toLowerCase();
                });
            });
        }
        if ($scope.query.status && $scope.query.status.length) {
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
                let edoc1Url = (appSetting.edoc1Url !== null && appSetting.edoc1Url !== '') ? appSetting.edoc1Url : '';
                let edoc2Url = (appSetting.edoc2Url !== null && appSetting.edoc2Url !== '') ? appSetting.edoc2Url : '';
                var redirectUrl = '';
                switch (data.itemType) {
                    case "RequestToHire":
                        redirectUrl = edoc2Url + `/home/requestToHire/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "PromoteAndTransfer":
                        redirectUrl = edoc2Url + `/home/promoteAndTransfer/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "Acting":
                        redirectUrl = edoc2Url + `/home/action/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "LeaveApplication":
                        redirectUrl = edoc2Url + `/home/leaves-management/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "MissingTimeClock":
                        redirectUrl = edoc2Url + `/home/missingTimelock/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "OvertimeApplication":
                        redirectUrl = edoc2Url + `/home/overtimeApplication/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "ShiftExchangeApplication":
                        redirectUrl = edoc2Url + `/home/shift-exchange/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "ResignationApplication":
                        redirectUrl = edoc2Url + `/home/resignationApplication/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "BusinessTripApplication":
                        redirectUrl = edoc2Url + `/home/business-trip-application/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "TargetPlan":
                        redirectUrl = edoc2Url + `/home/target-plan/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "BusinessTripOverBudget":
                        redirectUrl = edoc2Url + `/home/over-budget/item/${data.referenceNumber}?id=${data.itemId}`
                        return `<a target="_blank" href="${redirectUrl}">${data.referenceNumber}</a>`;
                    case "SKU":
                        return `<a target="_blank" href="${data.link}">${data.referenceNumber}</a>`;
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
                return (data.dueDate && data.dueDate != null) ? moment(data.dueDate).format('DD/MM/YYYY') : "";
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

    $scope.currentStatus = [];
    $scope.moduleOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            data: [
                { "code": "Edoc1", "name": "Edoc1" },
                { "code": "Edoc2", "name": "Edoc2" },
                { "code": "TradeContract", "name": "TradeContract" },
                { "code": "Facility", "name": "Facility" }
            ]
        },
        change: function (e) {
            let value = e.sender.value();
            if (!value.length && !$scope.query.module.length) {
                setDataStatus($scope.statusForm);
            } else {
                if ($scope.query.module.length == 1) {
                    $scope.currentStatus = [];
                    $scope.allTask.forEach(el => {
                        if ($scope.query.module[0].toLowerCase() == el.module.toLowerCase() && !$scope.currentStatus.includes(el.status)) {
                            $scope.currentStatus.push(el.status);
                        }
                    });
                    setDataStatus($scope.currentStatus);
                } else {
                    $scope.allTask.forEach(el => {
                        if (value.includes(el.module) && !$scope.currentStatus.includes(el.status)) {
                            $scope.currentStatus.push(el.status);
                        }
                    });
                    setDataStatus($scope.currentStatus);
                }
            }
        }
    };

});