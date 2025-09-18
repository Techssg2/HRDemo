var ssgApp = angular.module('ssg.probationListModule', ["kendo.directives"]);
ssgApp.controller('probationListController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, dataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, $compile, btaService) {
    
    $scope.title = '';
    var allDepartments = JSON.parse(sessionStorage.getItem("departments"));
    $scope.query = {
        predicate: "",
        predicateParameters: [],
        order: "Modified desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };

    this.$onInit = function () {
        $scope.title = $translate.instant($stateParams.action.title);
        $scope.advancedSearchMode = false;

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
        const cloneStatus = [
            ...commonData.itemStatuses
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

        $scope.probationGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getProbations(e);
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
                    return `<a ui-sref="home.probation-evaluation.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
                }
            },
            {
                field: "sapCode",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "150px",
            },
            {
                field: "fullName",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "department",
                headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                sortable: false,
                width: "240px"
            },
            {
                field: "position",
                headerTemplate: 'Position',
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
            }
            ]
        };
    }

    $scope.total = 0;
    $scope.data = [];
    async function getProbations(option) {
        let args = buildArgs($scope.query.page, appSetting.pageSizeDefault);

        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status, 'status');
        }

        if (option) {
            $scope.query.limit = option.data.take;
            $scope.query.page = option.data.page;
        }
        $scope.query.predicate = args.predicate;
        $scope.query.predicateParameters = args.predicateParameters;
        if ($rootScope.currentUser && $state.current.name == 'home.probation-evaluation.myRequests') {
            $scope.query.predicate = ($scope.query.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.query.predicate) + `CreatedById = @${$scope.query.predicateParameters.length}`;
            $scope.query.predicateParameters.push($rootScope.currentUser.id);
        }
        if (option) {
            let result = await cbService.getInstance().probationEvaluation.getProbationEvaluations($scope.query).$promise;
            if (result && result.isSuccess) {
                $scope.data = result.object.data;
                $scope.total = result.object.count;
                option.success($scope.data);
            }
            $scope.total = $scope.data.length;
            option.success($scope.data);
        }
        else {
            let grid = $("#grid").data("kendoGrid");
            grid.dataSource.read();
            grid.dataSource.page($scope.query.page);
        }
    }

    var columsSearch = [
        'ReferenceNumber.contains(@0)',
        'FullName.contains(@1)',
        'Position.contains(@2)'
    ];
    function buildArgs(pageIndex, pageSize) {
        let columnType = [];
        var option = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: pageSize,
            page: pageIndex
        }
        columnType = columsSearch
        if ($scope.query.keyword) {
            option.predicate = `(${columnType.join(" or ")})`;
            for (let index = 0; index < columnType.length; index++) {
                option.predicateParameters.push($scope.query.keyword);
            }
        }
        if ($scope.query.departmentCode) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(Department = @${option.predicateParameters.length})`;
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

    $scope.toggleFilterPanel = function (value) { 
        $scope.advancedSearchMode = value;
        if (value) {
            setDropDownTree('department_id', allDepartments);
        }
    }

    $scope.export = async function () {
        alert('export');
    }

    $scope.search = function () {
        alert('search');
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
    };
})