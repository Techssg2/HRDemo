var ssgApp = angular.module('ssg.targetPlanListModule', ["kendo.directives"]);
ssgApp.controller('targetPlanListController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, masterDataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, btaService) {

    $scope.title = $translate.instant($stateParams.action.title);
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Modified desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };
    var currentPeriodId = '';
    this.$onInit = function () {
        $scope.advancedSearchMode = false;
        $scope.targetPlans = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            autoBind: true,
            valuePrimitive: true,
            filter: "contains",
            dataSource: {
                serverPaging: true,
                pageSize: 100,
                transport: {
                    read: async function (e) {
                        await getPeriods(e);
                    }
                }
            },
            change: async function (e) {
                $scope.errors = [];
                let dropdownlist = $("#period_source_id").data("kendoDropDownList");
                let dataItem = dropdownlist.dataItem(e.node);
                currentPeriodId = dataItem.id;
                // $scope.model.periodId = dataItem.id;
                // $scope.model.periodName = dataItem.name;
                // $scope.model.periodFromDate = dataItem.fromDate;
                // $scope.model.periodToDate = dataItem.toDate;                
            }
        }
        var targetPlanListGridColumns = [
            {
                field: "status",
                headerTemplate: $translate.instant('COMMON_STATUS'),
                width: "350px",
                locked: true,
                template: function (data) {
                    var statusTranslate = $rootScope.getStatusTranslate(data.status);
                    return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                }
            },
            {
                field: "referenceNumber",
                headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
                width: "180px",
                locked: true,
                template: function (data) {
                    return `<a ui-sref="home.targetPlan.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
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
                //title: "Dept/ Line",
                headerTemplate: $translate.instant('COMMON_DEPT_LINE'),
                sortable: false,
                width: "350px"
            },
            {
                field: "divisionName",
                //title: "Division/ Group",
                headerTemplate: $translate.instant('COMMON_DIVISION_GROUP'),
                sortable: false,
                width: "350px"
            },
            {
                field: "periodName",
                title: "Period",
                //headerTemplate: $translate.instant('COMMON_DIVISION_GROUP'),
                sortable: false,
                width: "240px"
            },
            {
                field: "created",
                //title: "Created Date",
                headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.created).format(appSetting.longDateFormat);
                }
            },
            {
                field: "modified",
                //title: "Created Date",
                headerTemplate: $translate.instant('COMMON_MODIFIED_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.modified).format(appSetting.longDateFormat);
                }
            }
        ]

        $scope.targetPlanGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getTargetPLans(e);
                    }
                },
                schema: {
                    total: () => { return $scope.total },
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
            columns: targetPlanListGridColumns
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

            }
        };

        $scope.statusOptions = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: false,
            filter: "contains",
            dataSource: {
                data: translateStatus($translate, commonData.itemStatuses)
            }
        };

        $scope.query = {
            keyword: '',
            departmentCode: '',
            fromDate: null,
            toDate: null,
            status: ''
        };

    }
    async function getPeriods(e) {
        var res = await cbService.getInstance().targetPlan.getTargetPlanPeriods().$promise;
        if (res.isSuccess) {
            targetPlanData = res.object.data;
            let currentPeriod = _.head(targetPlanData);
            // if (currentPeriod) {
            //     $scope.model.periodId = currentPeriod.id;
            //     $scope.model.periodName = currentPeriod.name;
            //     $scope.model.periodFromDate = currentPeriod.fromDate;
            //     $scope.model.periodToDate = currentPeriod.toDate;
            // }
            e.success(targetPlanData);
        }
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

    $scope.targetPlanGridData = [
        {
            status: 'Completed',
            referenceNumber: 'TGL-000000001-2020',
            userSAPCode: '00311018',
            userFullName: 'Phạm Hoàng Hiệu',
            deptName: 'IT (G5)',
            divisionName: 'SYSTEM INTEGRATION (G4)',
            period: '10/2020 - 11/2020',
            modified: new Date(),
            created: new Date(),
        },
        {
            status: ' Waiting For Manager (G5) Approval',
            referenceNumber: 'TGL-000000002-2020',
            userSAPCode: '00403080',
            userFullName: 'Nguyễn Thúy Hạ',
            deptName: 'CELADON-GMS (G5)',
            divisionName: 'HUMAN RESOURCE CELADON (G2)',
            period: '10/2020 - 11/2020',
            modified: new Date(),
            created: new Date(),
        }
    ];

    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
        if (value) {
            setDropDownTree('department_id', allDepartments);
        }
    }

    $rootScope.$on("isEnterKeydown", function (result, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });

    var columsSearch = [
        'referenceNumber.contains(@0)',
        'userSAPCode.contains(@1)',
        'userFullName.contains(@2)'
    ]

    function buildArgs(pageIndex, pageSize) {
        var option = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: pageSize,
            page: pageIndex
        }

        if ($scope.query.keyword) {
            option.predicate = `(${columsSearch.join(" or ")})`;
            for (let index = 0; index < columsSearch.length; index++) {
                option.predicateParameters.push($scope.query.keyword);
            }
        }
        if ($scope.query.departmentCode) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(DeptCode = @${option.predicateParameters.length} or DivisionCode = @${option.predicateParameters.length + 1})`;
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

        return option;
    }

    $scope.total = 0;
    async function getTargetPLans(option) {

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
        if ($rootScope.currentUser && $rootScope.currentUser.id && $state.current.name == 'home.targetPlan.myRequests') {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        }
        if (option) {
            if (currentPeriodId) {
                $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `periodId=@${$scope.currentQuery.predicateParameters.length}`;
                $scope.currentQuery.predicateParameters.push(currentPeriodId);
            }
            let result = await cbService.getInstance().targetPlan.getList($scope.currentQuery).$promise;
            if (result && result.isSuccess) {
                $scope.data = result.object.data;
                $scope.total = result.object.count;
                option.success($scope.data);
            }
            else {
                let grid = $("#targetPlanGrid").data("kendoGrid");
                grid.dataSource.read();
                grid.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    $scope.search = function () {
        let grid = $("#targetPlanGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
    }

    $scope.clearSearch = function () {
        $scope.query = {};
        currentPeriodId = '';
        var dropdowntree = $("#department_id").data("kendoDropDownTree");
        dropdowntree.value("");
        var periodList = $("#period_source_id").data("kendoDropDownList");
        periodList.value("");
        let grid = $("#targetPlanGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
    }

    $scope.export = async function () {
        let args = buildArgs($scope.currentQuery.page, appSetting.pageSizeDefault);
        args.limit = 100000000;
        args.page = 1;
        //Khiem - fix 414
        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status, 'status');
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;
        if ($rootScope.currentUser && $rootScope.currentUser.id && $state.current.name == 'home.targetPlan.myRequests') {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        }
        if (currentPeriodId) {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `periodId=@${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push(currentPeriodId);
        }
        args.predicate = $scope.currentQuery.predicate;
        args.predicateParameters = $scope.currentQuery.predicateParameters;
        //
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.TARGETPLAN }, args).$promise;
        if (res.isSuccess) {
            exportToPdfFile(res.object);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }
});

