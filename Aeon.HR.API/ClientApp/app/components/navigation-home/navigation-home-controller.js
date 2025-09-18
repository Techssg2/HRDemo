var ssgApp = angular.module('ssg.navigationHomeModule', ['kendo.directives']);
ssgApp.controller('navigationHomeController', function ($rootScope, $scope, $state, appSetting, $translate, workflowService, $stateParams, $history, commonData, $interval, $window, settingService, navigationService, localStorageService, $timeout, settingService, dashboardService, ssgexService, $sce, $transitions, $window) {
    // create a message to display in our view
    $scope.title = 'Home';
    $scope.currentQuery = {};
    $scope.tasks = [];
    $scope.myItems = [];
    $scope.query = {};
    $rootScope.listNavigation = [];
    $rootScope.navigationRight = [];
    $rootScope.navigationLeft = [];
    $scope.isCheckedAll = false;
    var allTasks = [];
    $rootScope.navigationLeft = [];
    $rootScope.listNavigation = [];
    $rootScope.navigationRight = [];

    $scope.showCheckAll = false;
    var notInSadmin = appSetting.role.Admin | appSetting.role.HR | appSetting.role.CB | appSetting.role.Member;
    this.$onInit = async function () {
        if (appSetting && $rootScope.currentUser) {
            $scope.showCheckAll = appSetting.role.SAdmin == $rootScope.currentUser.role
                || appSetting.role.HRAdmin == $rootScope.currentUser.role
                || !(($rootScope.currentUser.role & notInSadmin) == $rootScope.currentUser.role)
        }
        await addListNavigation();
        getListNavigationRighCentertLeft();
        $scope.$applyAsync();
    }

    function getListNavigationRighCentertLeft() {
        var dashboardItem = _.find(appSetting.menu, x => {
            return x.ref === 'home.navigation-home'
        });
        if (dashboardItem) {
            dashboardItem.selected = true;
            $rootScope.title = dashboardItem.title.toUpperCase();
            // fix sapcode cung
            let scopeChild = _.filter(dashboardItem.childMenus, y => {
                if (y.sapCodes) {
                    if ($rootScope.currentUser && $rootScope.currentUser.sapCode && y.sapCodes.includes($rootScope.currentUser.sapCode)) {
                        if (y.url == 'home.navigation-list') {
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            });
            $rootScope.childMenusLeft = scopeChild;
        }
    }

    $rootScope.openNewWindow = function (item, isRoot = true) {
        $window.open(item.url, '_blank');
    };

    $scope.switchLayout = function () {
        if ($rootScope.currentUser) {
            // 0 AD, 1 MS
            if($rootScope.currentUser.type == 0)
                $window.open("/Home/HR", '_blank');
            else {
                $window.open("/home/Home/Dashboard", '_blank');
            }
        }
    }

    async function addListNavigation() {
        if ($rootScope.currentUser != null && $rootScope.currentUser.id != null) {
            let res;
            try {
                res = await navigationService.getInstance().navigation.getListNavigationByUserIdAndDepartmentId().$promise;
            }
            catch (err) {
                res = await navigationService.getInstance().navigation.getListNavigationByUserIdAndDepartmentIdV2({ UserId: $rootScope.currentUser.id, DepartmentId: $rootScope.currentUser.deptId }).$promise;
            }
            if (res && res.isSuccess) {
                if (res.object.data != null && res.object.count > 0) {
                    let objectData = res.object.data;

                    let navigationLeft = objectData.filter((x) => x.type == 0);
                    if (navigationLeft && navigationLeft.length) {
                        navigationLeft.forEach(x => {
                            var parent = {
                                id: x.id,
                                title: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                                name: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                                url: x.url
                            }
                            let existsChild = _.findIndex($rootScope.navigationLeft, y => {
                                return x.id == y.id;
                            });
                            if (existsChild == -1) {
                                $rootScope.navigationLeft.push(parent);
                            }
                        })
                    }

                    let navigationCenter = objectData.filter((x) => x.type == 1);
                    if (navigationCenter && navigationCenter.length) {
                        navigationCenter.forEach(x => {
                            var el1 = {
                                title: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                                url: x.url
                            }
                            var sections = [];
                            let child = JSON.parse(x.jsonChild);
                            if (child && child.length) {
                                child.forEach(xx => {
                                    var title = $translate.use() == 'en_US' ? xx.Title_EN : xx.Title_VI;

                                    let existsChild = _.findIndex(sections, x => {
                                        return x.id == xx.Id;
                                    });
                                    if (existsChild == -1) {
                                        var title = $translate.use() == 'en_US' ? xx.Title_EN : xx.Title_VI;
                                        var childSection = {
                                            id: xx.Id,
                                            name: title,
                                            nameAdd: title,
                                            title: title,
                                            ref: xx.Url,
                                            url: '',
                                            icon: baseUrlApi.replace('/api', xx.ProfilePicture.trim()),
                                            addToMenu: true,
                                            gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                                            roles: [1, 2, 4, 8, 16, 32],
                                            type: [0],
                                            departments: xx.Departments,
                                            users: xx.Users
                                        };
                                        sections.push(childSection);
                                    }
                                });
                                el1.sections = sections;
                                $rootScope.listNavigation.push(el1);
                            }
                        })
                    }

                    let navigationRight = objectData.filter((x) => x.type == 2);
                    if (navigationRight && navigationRight.length) {
                        navigationRight.forEach(x => {
                            var parent = {
                                title: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                                name: $translate.use() == 'en_US' ? x.title_EN : x.title_VI,
                                url: x.url
                            }
                            let values = [];
                            if (x.jsonChild != null) {
                                let child = JSON.parse(x.jsonChild);
                                child.forEach(x => {
                                    var child = {
                                        id: x.Id,
                                        title: $translate.use() == 'en_US' ? x.Title_EN : x.Title_VI,
                                        name: $translate.use() == 'en_US' ? x.Title_EN : x.Title_ENTitle_VI,
                                        url: x.Url
                                    }

                                    let existsChild = _.findIndex(values, z => {
                                        return x.Id == z.id;
                                    });

                                    if (existsChild == -1) {
                                        values.push(child);
                                    }
                                })
                            }
                            parent.values = values;
                            $rootScope.navigationRight.push(parent);
                        })
                    }
                }
            }
        }
    };

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
                    case "BusinessTripOverBudget":
                        return `<a ui-sref="home.over-budget.item({referenceValue: '${data.referenceNumber}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
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
    function setDataStatus(statusData) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: statusData
        });
        if ($("#dataStatusId") && $("#dataStatusId").length > 0) {
            var dropdownlist = $("#dataStatusId").data("kendoMultiSelect");
            dropdownlist.setDataSource(dataSource);
        }
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
                var statusTranslate = $rootScope.getStatusTranslate(data.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
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
                    case "BusinessTripOverBudget":
                        return `<a ui-sref="home.over-budget.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                    case "SKU":
                        return `<a target="_blank" href="${data.link}">${data.referenceNumber}</a>`;
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

    async function getMyItems(option) {
        var res = await dashboardService.getInstance().dashboard.getMyItems().$promise;
        if (res.isSuccess) {
            $scope.myItems = res.object;
            console.log($scope.myItems);
        }
        $scope.totalItems = $scope.myItems.length;
        option.success($scope.myItems);
    }


});