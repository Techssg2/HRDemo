var ssgApp = angular.module("ssg.reportTargetPlanModule", [
    "kendo.directives"
]);

ssgApp.controller("reportTargetPlanController", function (
    $rootScope,
    $scope,
    $location,
    appSetting,
    $stateParams,
    $state,
    moment,
    commonData,
    Notification,
    $sce,
    cbService,
    settingService,
    masterDataService,
    $timeout,
    workflowService,
    fileService,
    $translate, localStorageService,
    $compile,
    dataService
) {
    $scope.title = 'TARGET PLAN REPORT';
    $scope.total = 0;
    $scope.data = [];
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    var currentPeriodId = '';
    $scope.model = {
        keyword: '',
        departmentId: '',
        periodId: '',
        fromDate: null,
        toDate: null
    };
    $scope.checkStatus = {
        value: true
    }
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
                $scope.model.periodId = dataItem.id;
                // $scope.model.periodName = dataItem.name;
                // $scope.model.periodFromDate = dataItem.fromDate;
                // $scope.model.periodToDate = dataItem.toDate;                
            }
        }

        var reportTargetPlanListGridColumns = [
            {
                field: "sapCode",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "200px",
            },
            {
                field: "fullName",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "departmentName",
                //title: "Dept/ Line",
                headerTemplate: $translate.instant('COMMON_DEPT_LINE'),
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
            // {
            //     field: "referenceNumber",
            //     headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
            //     width: "200px",
            //     locked: true,
            //     template: function (data) {
            //         return `<a ui-sref="home.targetPlan.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
            //     }
            // },
            {
                field: "created",
                //title: "Created Date",
                headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.created).format(appSetting.longDateFormat);
                }
            },
            // {
            //     field: "modified",
            //     //title: "Created Date",
            //     headerTemplate: $translate.instant('COMMON_MODIFIED_DATE'),
            //     width: "200px",
            //     template: function (dataItem) {
            //         return moment(dataItem.modified).format(appSetting.longDateFormat);
            //     }
            // }
        ]

        $scope.reportTargetPlanGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getReportTargetPLans(e);
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
            columns: reportTargetPlanListGridColumns
        }

        $scope.departmentOptions = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            template: showCustomDepartmentTitle,
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            dataSource: [],
            filter: "contains",
            filtering: async function (option) {
                await getDepartmentByFilter(option);
            },
            loadOnDemand: true,
            valueTemplate: (e) => showCustomField(e, ['name']),
            change: async function (e) {
                var value = this.value();
                $scope.model.departmentId = value;
            }
        };

        checkRoleUserLogin();
    }

    var columsSearch = [
        'FullName.contains(@0)',
        'DepartmentName.contains(@1)'
    ]

    isSAPCodeExistInDepartment = true;
    async function buildArgs(pageIndex, pageSize) {
        var option = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: pageSize,
            page: pageIndex
        }

        if ($scope.model.keyword) {
            //check xem co phai dang tim user ko
            var isUser = await checkIsUser($scope.model.keyword);
            if (isUser) {
                let haveSAPCode = $scope.sapCodes.find(x => x == $scope.model.keyword);
                if (haveSAPCode) {
                    option.predicate = `(SAPCode.contains(@0))`;
                    option.predicateParameters.push($scope.model.keyword);
                }
                else {
                    isSAPCodeExistInDepartment = false;
                }
            }
            else {
                option.predicate = `(${columsSearch.join(" or ")})`;
                for (let index = 0; index < columsSearch.length; index++) {
                    option.predicateParameters.push($scope.model.keyword);
                }
            }
        }
        else {
            let columnSAPCode = [];
            for (let i = 0; i < $scope.sapCodes.length; i++) {
                columnSAPCode.push(`SAPCode.contains(@${i})`);
                option.predicateParameters.push($scope.sapCodes[i]);
            }
            option.predicate = `(${columnSAPCode.join(" or ")})`;
        }

        if ($scope.model.fromDate && isValidDate($scope.model.fromDate)) {
            var date = moment($scope.model.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' && (' : option.predicate) + `Created >= @${option.predicateParameters.length}`;
            option.predicateParameters.push(date);
        }
        if ($scope.model.toDate && isValidDate($scope.model.toDate)) {
            var toDate = moment($scope.model.toDate, 'DD/MM/YYYY').add(1, 'days').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' && ' : option.predicate) + `Created < @${option.predicateParameters.length})`;
            option.predicateParameters.push(toDate);
        }

        return option;
    }

    async function getReportTargetPLans(option) {
        $scope.data = [];
        $scope.total = 0;
        let limit = 0;
        let page = 0;
        // let query = {
        //     predicate: "",
        //     predicateParameters: [],
        //     order: "Modified desc",
        //     limit: appSetting.pageSizeDefault,
        //     page: 1
        // }
        if (option) {
            limit = option.data.take;
            page = option.data.page;
        }
        if (option && $scope.model.departmentId && $scope.model.periodId) {
            // if (currentPeriodId) {
            //     query.predicate = (query.predicate ? query.predicate + ' and ' : query.predicate) + `periodId=@${query.predicateParameters.length}`;
            //     query.predicateParameters.push(currentPeriodId);
            // }
            let result = await getUsersByDeptLine($scope.model.departmentId, limit, $scope.model.keyword, page);
            if (result && result.isSuccess) {
                $scope.data = _.uniqBy(result.object.data, 'sapCode');
                $scope.total = result.object.count;
                option.success($scope.data);
            }
            else {
                option.success($scope.data);
            }
        }
        else {
            option.success($scope.data);
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
            let res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
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
            if (!$scope.model.departmentId || $scope.model.departmentId == '') {
                setDropDownTree('department_id', allDepartments);
            }
        }
    }

    $rootScope.$on("isEnterKeydown", function (result, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });

    $scope.search = async function () {
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
        $scope.toggleFilterPanel(false);
        $scope.$apply();

    }

    $scope.clearSearch = function () {
        $scope.model.keyword = '';
        $scope.checkStatus.value = true;
        currentPeriodId = '';
        var dropdowntree = $("#department_id").data("kendoDropDownTree");
        dropdowntree.value("");
        $scope.model.departmentId = '';
        var periodList = $("#period_source_id").data("kendoDropDownList");
        periodList.value("");
        $scope.model.periodId = '';
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
    }

    async function getPeriods(e) {
        var res = await cbService.getInstance().targetPlan.getTargetPlanPeriods().$promise;
        if (res.isSuccess) {
            targetPlanData = res.object.data;
            currentPeriod = _.orderBy(targetPlanData, ['fromDate'], ['desc'])[0];
            if (currentPeriod) {
                $scope.model.periodId = currentPeriod.id;
                // $scope.model.periodName = currentPeriod.name;
                // $scope.model.periodFromDate = currentPeriod.fromDate;
                // $scope.model.periodToDate = currentPeriod.toDate;
            }
            e.success(targetPlanData);
        }
    }

    $scope.sapCodes = [];
    async function getUsersByDeptLine(deptId, limit, searchText, page = 1) {
        let result = await settingService.getInstance().users.getUsersForReportTargetPlan({ departmentId: deptId, periodId: $scope.model.periodId, limit: limit, page: page, searchText: searchText, isMade: $scope.checkStatus.value }).$promise;
        if (result.isSuccess) {
            if (result.object.data) {
                return result;
            };
        }
    }

    async function checkIsUser(sapCode) {
        let result = false;
        let res = await settingService.getInstance().users.checkUserBySAPCode({ sapCode: sapCode }, null).$promise;
        if (res.isSuccess) {
            if (res.object) {
                result = true;
            }
        }
        return result;
    }

    function setReportTargetPlanGridAPI(total, pageIndex, pageSizes) {
        let grid = $('#grid').data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            transport: {
                read: async function (e) {
                    await getReportTargetPLans(e);
                }
            },
            pageSize: pageSizes,
            page: pageIndex,
            serverPaging: true,
            schema: {
                total: function () {
                    return total;
                }
            },
        });
        if (grid) {
            grid.setDataSource(dataSource);
        }
    }

    function setReportTargetPlanGrid(total, pageIndex, pageSizes) {
        let grid = $('#grid').data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            data: $scope.data,
            pageSize: pageSizes,
            page: pageIndex,
            schema: {
                total: function () {
                    return total;
                }
            },
        });
        if (grid) {
            grid.setDataSource(dataSource);
        }
    }

    permission = [];
    async function checkRoleUserLogin() {
        if ($rootScope.currentUser && $rootScope.currentUser.role) {
            for (var i = 0; i < appSetting.permission.length; i++) {
                if ((appSetting.permission[i].code & $rootScope.currentUser.role) > 0) {
                    permission.push(appSetting.permission[i].code);
                }
            }
            /*Role cũ: SAdmin, HRAdmin, HR thì có thể thấy được nguyên cây của hệ thống
             * Trường hợp còn lại thì chi theo deptline và division
             let result = permission.find(x => x == commonData.role.SAdmin || x == commonData.role.HRAdmin || x == commonData.role.HR);
             if (result) { 
            */


            /*Role mới: SAdmin, HRAdmin và Department có CB thuộc HQ thấy được tất cả
             Còn lại cũng sẽ chia theo Deptline và Division*/
            let result = permission.find(x => x == commonData.role.SAdmin ||  x == commonData.role.HRAdmin ||  x == commonData.role.HR);
            if (result || ($rootScope.currentUser.isCB === true && $rootScope.currentUser.isStore===false)) {
                $timeout(function() {
                    setDataDepartment(allDepartments);
                }, 0);
            }
            else {
                let departmentId = '';
                if($rootScope.currentUser.divisionId) {
                    departmentId = $rootScope.currentUser.divisionId;
                }
                else {
                    departmentId = $rootScope.currentUser.deptId;
                }
                await getDivisionTreeByIds([departmentId])
            }
        }
    }

    $scope.dataTemporaryArrayDivision = [];
    async function getDivisionTreeByIds(ids) {
        kendo.ui.progress($("#loading_bta"), true); 
        let arg = {
            predicate: "",
            predicateParameters: [],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: ""
        }
        for (let i = 0; i < ids.length; i++) {
            arg.predicate = arg.predicate ? arg.predicate + `||id = @${i}` : `id = @${i}`;
            arg.predicateParameters.push(ids[i]);
        }
        var result = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
        if (result.isSuccess) {
            $scope.deptDivisions = result.object.data;
            $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
            $timeout(function() {
                setDataDepartment($scope.dataTemporaryArrayDivision);
                kendo.ui.progress($("#loading_bta"), false); 
            }, 0)
        }
    }
    
    $scope.exportReportTargetPlan = async function() {
        if($scope.model.departmentId) {
            let res = await cbService.getInstance().targetPlan.targetPlanReport({ departmentId: $scope.model.departmentId, periodId: $scope.model.periodId, limit: 100, page: 1, searchText: $scope.model.keyword, isMade: $scope.checkStatus.value }, null).$promise;
            if (res.isSuccess) {
                let fileContext = {
                    content: res.object,
                    fileName: 'Report Target Plan'
                }
                exportToExcelFile(fileContext);
                Notification.success(appSetting.notificationExport.success);
            }
            else {
                Notification.error(appSetting.notificationExport.error);
            }
        }
        else {
            Notification.error(appSetting.notificationExport.error);
        }
    }
});