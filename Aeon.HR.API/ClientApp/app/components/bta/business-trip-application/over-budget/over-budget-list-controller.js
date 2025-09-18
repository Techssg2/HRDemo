var ssgApp = angular.module('ssg.overBudgetListModule', ["kendo.directives"]);
ssgApp.controller('overBudgetListController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, masterDataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, btaService) {
    $scope.title = $stateParams.id ? $translate.instant('BUSINESS_TRIP_OVER_BUDGET') + ': ' + $stateParams.referenceValue : $translate.instant('BUSINESS_TRIP_OVER_BUDGET_NEW_TITLE');
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "businessTripFrom desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };
    let functionType = "";
    this.$onInit = function () {
        if ($state.current.name === 'home.over-budget.myRequests') {
            $scope.selectedTab = "1";
        } else if ($state.current.name === 'home.over-budget.allRequests') {
            $scope.selectedTab = "0";
        }

        $scope.title = $translate.instant($stateParams.action.title);
        $scope.advancedSearchMode = false;
        $scope.total = 0;
        $scope.data = [];
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

            }
        };
        async function getDepartmentByFilter(option) {
            if (!option.filter) {
                option.preventDefault();
            } else {
                let filter = option.filter && option.filter.value ? option.filter.value : "";
                arg = {
                    predicate: "name.contains(@0) or code.contains(@1) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
                    predicateParameters: [filter, filter, filter],
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
            var departmentTree = $("#department_id").data("kendoDropDownTree");
            if (departmentTree) {
                departmentTree.setDataSource(dataSource);
            }
        }

        $scope.query = {
            keyword: '',
            departmentCode: '',
            fromDate: null,
            toDate: null,
            status: ''
        };
        const cloneStatus = [
            ...commonData.itemStatuses,
            { name: "STATUS_COMPLETED_CHANGING", code: 'Completed Changing' }
        ];
        $scope.statusOptions = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: false,
            filter: "contains",
            dataSource: {
                data: translateStatus($translate, cloneStatus)
            }
        };
        $scope.btaGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getOverBudgetList(e);
                    }
                },
                schema: {
                    total: () => { return $scope.total },
                    data: () => { return $scope.data }
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
                field: "status",
                //title: "Status",
                headerTemplate: $translate.instant('COMMON_STATUS'),
                width: "300px",
                locked: false,
                template: function (data) {
                    var statusTranslate = $rootScope.getStatusTranslate(data.status);
                    return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                }
            },
            {
                field: "referenceNumber",
                headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
                width: "180px",
                locked: false,
                template: function (data) {
                    return `<a ui-sref="home.over-budget.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                }
            },
            {
                field: "btaReferenceNumber",
                headerTemplate: $translate.instant('BTA_REFERENCE_NUMBER'),
                width: "180px",
                locked: false,
                template: function (data) {
                    return `<a ui-sref="home.business-trip-application.item({referenceValue: '${data.btaReferenceNumber}', id: '${data.businessTripApplicationId}'})" ui-sref-opts="{ reload: true }">${data.btaReferenceNumber}</a>`;
                }
            },
            {
                field: "userSAPCode",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "150px",
            },
            {
                field: "userFullName",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "deptName",
                headerTemplate: $translate.instant('COMMON_DEPT_LINE'),
                sortable: false,
                width: "240px"
            },
            {
                field: "deptDivisionName",
                headerTemplate: $translate.instant('COMMON_DIVISION_GROUP'),
                sortable: false,
                width: "240px"
            },
            {
                field: "created",
                headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.created).format(appSetting.longDateFormat);
                }
            },
            {
                field: "modified",
                headerTemplate: $translate.instant('COMMON_MODIFIED_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.modified).format(appSetting.longDateFormat);
                }
            },
            {
                field: "businessTripFrom",
                headerTemplate: $translate.instant('BTA_FROM_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.businessTripFrom).format(appSetting.longDateFormat);
                }
            },
            {
                field: "businessTripTo",
                headerTemplate: $translate.instant('BTA_TO_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.businessTripTo).format(appSetting.longDateFormat);
                }
            }
            ]
        };
    }

    $scope.isModalVisible = false;
    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
        $scope.isModalVisible = value;
        if (value) {
            if (!$scope.query.departmentCode || $scope.query.departmentCode == '')
                setDropDownTree('department_id', allDepartments);
        }
    }

    $scope.onTabChange = function () {
        if ($scope.selectedTab === "1") {
            $state.go('home.over-budget.myRequests');
        } else if ($scope.selectedTab === "0") {
            $state.go('home.over-budget.allRequests');
        }
    };

    $scope.export = async function () {
        functionType = "export";
        let args = buildArgs(appSetting.numberSheets, appSetting.numberRowPerSheets);
        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status);
        }
        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.OVERBUDGET
        }, args).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.search = function () {
        functionType = "search";
        loadPageOne();
        $scope.toggleFilterPanel(false);
        $scope.$apply();
    };

    $scope.clearSearch = function () {
        $scope.query = {
            keyword: '',
            departmentCode: '',
            fromDate: null,
            toDate: null,
            status: ''
        };
        $scope.$broadcast('resetToDate', $scope.query.toDate);
        clearSearchTextOnDropdownTree("department_id");
        setDropDownTree('department_id', allDepartments);
        loadPageOne();
    };

    var columsSearch = [
        'ReferenceNumber.contains(@0)',
        'CreatedByFullName.contains(@1)',
        'UserSAPCode.contains(@2)'
    ];
    let comlumnSearchForExport = [
        'ReferenceNumber.contains(@0)',
        'CreatedByFullName.contains(@1)',
        'CreatedBySapCode.contains(@2)'
    ]
    function buildArgs(pageIndex, pageSize) {
        let columnType = [];
        var option = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: pageSize,
            page: pageIndex
        }
        switch (functionType) {
            case 'export': { columnType = comlumnSearchForExport; break; }
            case 'search': { columnType = columsSearch; break; }
        }
        if ($scope.query.keyword) {
            option.predicate = `(${columnType.join(" or ")})`;
            for (let index = 0; index < columnType.length; index++) {
                option.predicateParameters.push($scope.query.keyword);
            }
        }
        if ($scope.query.departmentCode) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(DeptCode = @${option.predicateParameters.length} or DeptDivisionCode = @${option.predicateParameters.length + 1})`;
            option.predicateParameters.push($scope.query.departmentCode);
            option.predicateParameters.push($scope.query.departmentCode);
        }
        if ($scope.query.fromDate && isValidDate($scope.query.fromDate)) {
            var date = moment($scope.query.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created > @${option.predicateParameters.length}`;
            option.predicateParameters.push(date);
        }
        if ($scope.query.toDate && isValidDate($scope.query.toDate)) {
            const toDate = moment($scope.query.toDate, 'DD/MM/YYYY').add(1, 'days').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created < @${option.predicateParameters.length}`;
            option.predicateParameters.push(toDate);
        }
        if ($rootScope.currentUser && $state.current.name == 'home.business-trip-application.myRequests') {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `CreatedById = @${option.predicateParameters.length}`;
            option.predicateParameters.push($rootScope.currentUser.id);
        }

        return option;
    }

    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
    }

    $rootScope.$on("isEnterKeydown", function (result, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });
    async function getOverBudgetList(option) {
        $scope.data = [
        ];
        let args = buildArgs($scope.currentQuery.page, appSetting.pageSizeDefault);

        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status, 'status');
        }

        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;
        if ($rootScope.currentUser && $state.current.name == 'home.over-budget.myRequests') {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        }
        if (option) {
            let result = await btaService.getInstance().bussinessTripApps.getListOverBudget($scope.currentQuery).$promise;
            if (result && result.isSuccess) {
                $scope.data = result.object.data;
                $scope.total = result.object.count;
                option.success($scope.data);
            }
        }
        else {
            let grid = $("#grid").data("kendoGrid");
            grid.dataSource.read();
            grid.dataSource.page($scope.currentQuery.page);
        }
    }
    
});
