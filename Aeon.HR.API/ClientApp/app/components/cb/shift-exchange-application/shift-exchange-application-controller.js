var ssgApp = angular.module("ssg.shiftExchangeApplicationModule", [
    "kendo.directives"
]);
ssgApp.controller("shiftExchangeApplicationController", function (
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
    ssgexService,
    settingService,
    masterDataService,
    $timeout,
    workflowService,
    fileService,
    $translate, localStorageService,
    $compile,
    dataService
) {
    // create a message to display in our view
    var ssg = this;
    $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
    $scope.value = 0;
    var currentAction = null;
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    isItem = true;
    $scope.titleHeader = 'SHIFT EXCHANGE APPLICATION';
    //$scope.title = $stateParams.id ? "SHIFT EXCHANGE APPLICATION: " + $stateParams.referenceValue : $stateParams.action.title;
    $scope.title = $stateParams.id ? /*$translate.instant('SHIFT_EXCHANGE_APPLICATION_EDIT_TITLE') +*/ $stateParams.referenceValue : $state.current.name == 'home.shiftExchange.item' ? /*$translate.instant('SHIFT_EXCHANGE_APPLICATION_NEW_TITLE')*/'' : $state.current.name == 'home.shiftExchange.myRequests' ? $translate.instant('SHIFT_EXCHANGE_MY_REQUETS').toUpperCase() : $translate.instant('SHIFT_EXCHANGE_ALL_REQUETS').toUpperCase();
    stateItem = "home.shiftExchange.item";
    stateItemView = "home.shiftExchange.itemView";
    $scope.isShow = false;
    $scope.oldExchangingShiftItems = [];
    dataService.trackingHistory = [];
    // phần code dành cho myrequest và all request
    //Status Data
    $scope.query = {
        keyword: '',
        departmentId: null,
        fromDate: null,
        toDate: null,
        status: '',
        userSAPCode: ''
    };
    $scope.review = {
        errorMessage: ''
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
            // serverFiltering: true,
            //data: commonData.itemStatuses
            data: translateStatus($translate, commonData.itemStatuses)
        }
    };

    var checkStatusCollection = {
        notErd: false,
        erd: true,
        all: 'all',
    }
    $scope.checkStatus = {
        value: checkStatusCollection.all
    };

    //Search
    $scope.query = {
        keyword: "",
        departmentId: null,
        createdDate: new Date(),
        fromDate: null,
        toDate: null
    };

    //API
    $scope.model = {};
    $scope.workflowInstances = [];
    $scope.deptLine = [];
    $scope.dataShiftCode = [];
    $scope.processingStageRound = 0;

    async function getDeptLine() {
        // var result = await settingService.getInstance().departments.getDepartmentTree().$promise;
        // if (result.isSuccess) {
        //     $scope.deptLine = result.object.data;
        //     if ($state.current.name === commonData.stateShiftExchange.item) {
        //         setDataDeptLine($scope.deptLine);
        //     }
        // }

        $scope.deptLine = allDepartments;
        if ($state.current.name === commonData.stateShiftExchange.item) {
            setDataDeptLine($scope.deptLine);
        }
    }
    function setDataDeptLine(dataDepartment) {
        // var dataSource = new kendo.data.HierarchicalDataSource({
        //     data: dataDepartment
        // });
        // var dropdownlist = $("#deptLine").data("kendoDropDownTree");
        // dropdownlist.setDataSource(dataSource);
    }

    //hàm này sau này sẽ đổi get data theo deptLine
    $scope.deptDivision = [];
    async function getDivisionTreeByIds(ids) {
        arg = {
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
        result = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
        if (result.isSuccess) {
            $scope.deptDivision = result.object.data;
            //$scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
            $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.type == 1 });
            setDataDeptDivision($scope.dataTemporaryArrayDivision);
            if (_.findIndex(ids, x => { return x == $rootScope.currentUser.divisionId }) > -1) {
                var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
                if (!$stateParams.id) {
                    if (dropdownlist) {
                        dropdownlist.value($rootScope.currentUser.divisionId);
                    }
                    $scope.form.dept_Division = $rootScope.currentUser.divisionId;
                }
                //await getSapCodeByDivison(1, $scope.limitDefaultGrid, '');
            }

        } else {
            Notification.error(result.messages[0]);
        }
    }

    function setDataDeptDivision(dataDepartment) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment
        });
        var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }

    //get SapCode ( hàm này sau này sẽ đổi sẽ get theo deptLine và deptDivision)
    $scope.users = [];

    async function getUsers() {
        var res = await settingService.getInstance().users.getAllUser().$promise;
        if (res.isSuccess) {
            $scope.employeesDataSource = res.object.data;
        }
    }

    var columsSearch = [
        'referenceNumber.contains(@0)',
        'deptLine.Name.contains(@1)',
        'DeptDivision.Name.contains(@2)',
        'CreatedByFullName.contains(@3)',
        'UserCreatedBy.SAPCode.contains(@4)'
    ]

    function buildArgs(pageIndex, pageSize) {
        var option = {
            predicate: "",
            predicateParameters: [],
            Order: "Created desc",
            Limit: pageSize,
            Page: pageIndex
        }

        if ($scope.query.keyword) {
            option.predicate = `(${columsSearch.join(" or ")})`;
            for (let index = 0; index < columsSearch.length; index++) {
                option.predicateParameters.push($scope.query.keyword);
            }
        }
        //code moi
        if ($scope.checkStatus.value != 'all') {
            //check erd
            if ($scope.checkStatus.value) {
                option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `ExchangingShiftItems.Any(x => x.IsERD == @${option.predicateParameters.length})`;
                option.predicateParameters.push($scope.checkStatus.value);
            }
            //check not erd
            if (!$scope.checkStatus.value) {
                option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `ExchangingShiftItems.All(x => x.IsERD == @${option.predicateParameters.length})`;
                option.predicateParameters.push($scope.checkStatus.value);
            }
        }
        //
        if ($scope.query.departmentId) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(DeptLineId = @${option.predicateParameters.length} or DeptDivisionId = @${option.predicateParameters.length + 1})`;
            option.predicateParameters.push($scope.query.departmentId);
            option.predicateParameters.push($scope.query.departmentId);
        }
        if ($scope.query.fromDate) {
            var date = moment($scope.query.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created > @${option.predicateParameters.length}`;
            option.predicateParameters.push(date);
        }
        if ($scope.query.toDate) {
            var date = moment($scope.query.toDate, 'DD/MM/YYYY').add(1, 'days').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created < @${option.predicateParameters.length}`;
            option.predicateParameters.push(date);
        }

        if ($scope.query.shiftDateFrom) {
            if ($scope.query.shiftDateTo) {
                temp = `exchangingShiftItems.Any(y => y.shiftExchangeDate >= @${option.predicateParameters.length} && y.shiftExchangeDate < @${option.predicateParameters.length + 1})`;
                option.predicateParameters.push(moment($scope.query.shiftDateFrom, 'DD/MM/YYYY').format('YYYY-MM-DD'));
                option.predicateParameters.push(moment($scope.query.shiftDateTo, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
            } else {
                temp = `exchangingShiftItems.Any(y => y.shiftExchangeDate >= @${option.predicateParameters.length})`;
                option.predicateParameters.push(moment($scope.query.shiftDateFrom, 'DD/MM/YYYY').format('YYYY-MM-DD'));
            }
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + temp;
        } else {
            if ($scope.query.shiftDateTo) {
                option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `exchangingShiftItems.Any(y => y.shiftExchangeDate < @${option.predicateParameters.length})`;
                option.predicateParameters.push(moment($scope.query.shiftDateTo, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
            }
        }

        if ($scope.query.userSAPCode) {
            if (option.predicateParameters.length) {
                option.predicate += ` and `;
            }
            option.predicate += `ExchangingShiftItems.any(x => x.User.SAPCode == @` + option.predicateParameters.length + `)`;
            option.predicateParameters.push($scope.query.userSAPCode);
        }

        if ($state.current.name == 'home.shiftExchange.myRequests') {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `createdById = @${option.predicateParameters.length}`;
            option.predicateParameters.push($rootScope.currentUser.id);
        }

        return option;
    }

    $scope.onCheckStatus = async function () {
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        if (!$scope.advancedSearchMode) {
            var option = $scope.currentQuery;
            if ($scope.checkStatus.value != 'all') {
                option.predicate = "ExchangingShiftItems.IsERD = @0";
                option.predicateParameters.push($scope.checkStatus.value);
            }
            reloadGrid();
        } else {
            $scope.applySearch();
        }
    }

    function reloadGrid() {
        let grid = $("#gridRequests").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
            if ($scope.advancedSearchMode) {
                grid.dataSource.page(1);
            }
        }
    }

    $scope.total = 0;
    $scope.data = [];
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        Order: "Created desc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    }
    async function getAllShiftExchange(option) {
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            Order: "Created desc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        }

        let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);
        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status, 'status');
        }

        if (option) {
            $scope.currentQuery.Limit = option.data.take;
            $scope.currentQuery.Page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;

        //get limit in grid 
        let grid = $("#gridRequests").data("kendoGrid");
        $scope.currentQuery.Limit = grid.pager.dataSource._pageSize;
        //

        var result = await cbService.getInstance().shiftExchange.getAllShiftExchange($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.data = result.object.data;
            $scope.data.forEach(item => {
                item.created = new Date(item.created);
                if (item.mapping) {
                    if (item.mapping.userJobGradeGrade >= 5) {
                        item['userDeptName'] = item.mapping.departmentName;
                    } else {
                        item['userDivisionName'] = item.mapping.departmentName;
                    }
                }
            })
            $scope.total = result.object.count;
        }
        if (option) {
            option.success($scope.data);
            setTimeout(function () {
                grid.resize();
            }, 100);
        } else {
            // $scope.model.overtimeApplicationGridOptions.dataSource.read();
            initGridRequests($scope.data, $scope.total, $scope.currentQuery.Page, $scope.currentQuery.Limit);
            setTimeout(function () {
                grid.resize();
            }, 100);
        }
    }

    function initGridRequests(dataSource, total, pageIndex, pageSize) {
        let grid = $("#gridRequests").data("kendoGrid");
        let dataSourceRequests = new kendo.data.DataSource({
            data: dataSource,
            // pageSize: appSetting.pageSizeDefault,
            pageSize: pageSize,
            page: pageIndex,
            schema: {
                total: function () {
                    return total;
                }
            },
        });
        grid.setDataSource(dataSourceRequests);
    }

    $scope.shiftCodes = [];

    async function getShiftCodes() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'CurrentShift',
        }).$promise;
        if (res.isSuccess) {

            res.object.data = $.grep(res.object.data, function (v) {
                return v.code != "OT";
            });


            $scope.shiftExchangeDataSource = res.object.data.map(function (item) {

                return { ...item, showtextCode: item.code }
            });
        }
    }
    async function getNewShiftCodes() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'ShiftCode',
            //name: 'CurrentShift',
        }).$promise;
        if (res.isSuccess)

            res.object.data = $.grep(res.object.data, function (v) {
                return v.code != "OT";
            });
        
        await getDataShiftCodes();

        var ShiftCodeInEdoc = res.object.data.filter(function (item) {
            return $scope.dataShiftCode.find(function (dataItem) {
                return dataItem.code === item.code;
            });
        });

        $scope.newShiftExchangeDataSource = ShiftCodeInEdoc
            .filter(function (i) { return this.indexOf(i.code) < 0; }, ['FWFH', 'FWH1', 'FWH2', 'SOP', 'NPA', 'NPA1', 'NPA2', 'PLD'])
            .map(function (item) {
                return { ...item, showtextCode: item.code }
            });
    }

    async function getDataShiftCodes() {
        var query = {
            predicate: `IsActive.equals(@${0})`,
            predicateParameters: [true],
            Order: "Created desc",
            Limit: 10000,
            Page: 1
        }
        var result = await settingService.getInstance().cabs.getDataShiftCode(query).$promise;
        if (result.isSuccess) {
            $scope.dataShiftCode = result.object.data;
        }
        else $scope.dataShiftCode = [];
    }

    $scope.searchOptions = {
        //Department
        departmentOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            //template: '#: item.code # - #: item.name #',
            template: showCustomDepartmentTitle,
            //valueTemplate: '#: item.id #',
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
                    clearSearchTextOnDropdownTree('departmentId');
                    setDataDepartment(allDepartments);
                }
            }
        },
        divisionOptions: {
            dataValueField: "id",
            dataTextField: "name",
            dataSource: [],
            autoClose: false,
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            //disable những item là deptline, có grade > 4, ko cho select
            // filtering: async function (option) {
            //     await getDivisionByFilter(option);
            // },
            customFilterFields: ['code', 'name', 'userCheckedHeadCount'],
            filtering: filterMultiField,
            template: function (dataItem) {
                //return `<span class="${dataItem.item.jobGradeGrade > 4 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
                return `<span class="${dataItem.item.type == 2 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
            },
            loadOnDemand: false,
            valueTemplate: (e) => showCustomField(e, ['name']),
            select: async function (e) {
                let dropdownlist = $("#deptDivision").data("kendoDropDownTree");
                let dataItem = dropdownlist.dataItem(e.node)

                // if (dataItem.jobGradeGrade > 4) {
                if (dataItem.type == 4) {
                    e.preventDefault();
                } else {
                    $scope.form.dept_Division = dataItem.id;
                }
                dropdownlist.close();
                if ($scope.form.exchangingShiftItems.length > 0 && $scope.perm) {
                    $timeout(function () {
                        $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('SHIFT_EXCHANGE_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                        $scope.dialog.bind("close", confirm);
                    }, 0);
                    // console.log($scope.currentDivision);
                }
                //await getSapCodeByDivison(1, $scope.limitDefaultGrid, '');

            }
        }
    };
    $scope.shiftExchangeId = '';
    $scope.referenceNumber = '';
    $scope.test = '';
    $scope.onChangeDivision = async function (value) {
        //if (e.sender.value() && $scope.form.exchangingShiftItems.length > 0) {
        if ($scope.form.exchangingShiftItems.length > 0 && $scope.perm && !value) {
            $timeout(function () {
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('SHIFT_EXCHANGE_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", confirm);
            }, 0);
        }
        else {

            //$scope.form.exchangingShiftItems = new kendo.data.ObservableArray([]);
            // let isAll = false;
            // if ((!$stateParams.id || $scope.form && $scope.form.status == 'Draft') && value) {
            //     isAll = value == $rootScope.currentUser.divisionId;
            // }
            if (!value) {
                kendo.ui.progress($("#loading"), true);
                clearSearchTextOnDropdownTree('deptDivision');
                setDataDeptDivision($scope.dataTemporaryArrayDivision);
                await getUserCheckedHeadCount($scope.form.dept_Line);
                if ((!value || value == '') && $scope.shiftExchangeId && $scope.referenceNumber) {
                    /*$state.go($state.current.name, { id: $scope.shiftExchangeId, referenceValue: $scope.referenceNumber }, { reload: true });*/

                    if ($scope.oldExchangingShiftItems) {
                        $scope.form.exchangingShiftItems = $scope.oldExchangingShiftItems[0];
                        initShiftExchangeEditListGrid($scope.form.exchangingShiftItems);
                    }
                    /*$state.go($state.current.name);*/
                }
            } else {
                //await getSapCodeByDivison(1, $scope.limitDefaultGrid, '');
            }
            kendo.ui.progress($("#loading"), false);
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
        var departmentTree = $("#departmentId").data("kendoDropDownTree");
        if (departmentTree) {
            departmentTree.setDataSource(dataSource);
        }
    }

    async function getDeptDivisionByFilter(option) {
        if (!option.filter) {
            option.preventDefault();
        } else {
            let filter = option.filter && option.filter.value ? option.filter.value : "";
            if (filter) {
                arg = {
                    predicate: "name.contains(@0) or code.contains(@1)",
                    predicateParameters: [filter, filter],
                    page: 1,
                    limit: appSetting.pageSizeDefault,
                    order: ""
                }
                res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
                if (res.isSuccess) {
                    setDataDeptDivision(res.object.data);
                }
            } else {
                setDataDeptDivision($scope.dataTemporaryArrayDivision);
            }
        }
    }
    async function getDivisionByFilter(option) {
        if (!option.filter) {
            option.preventDefault();
        } else {
            let filter = option.filter && option.filter.value ? option.filter.value : "";
            if (filter) {
                let currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == $scope.dept_Line });
                let ids = [];
                if (currentItem.divisions.length)
                    ids = _.map(currentItem.divisions, 'id');
                arg = {
                    deptId: $scope.dept_Line,
                    divisionIds: ids,
                    filter: filter
                }
                res = await settingService.getInstance().departments.getDivisionByFilter(arg).$promise;
                if (res.isSuccess) {
                    setDataDeptDivision(res.object.data);
                }
            } else {
                setDataDeptDivision($scope.dataTemporaryArrayDivision);
            }
        }
    }
    valueReasonCode = '';

    async function getShiftSetByDate(userSapCode, date) {
        let returnValue = new Array();
        var d = moment(date).format('YYYY-MM-DD HH:mm:ss');
        let paramObj = {
            "SAPCode": userSapCode,
            "date": d
        };
        let shiftSetResult = await ssgexService.getInstance().remoteDatas.getShiftSetByDate(paramObj).$promise;
        if (shiftSetResult.isSuccess) {

            let shiftSet = new Array();
            let shiftSetOfCurrentDate = shiftSetResult.object;
            if (!$.isEmptyObject(shiftSetOfCurrentDate.shift1)) {
                shiftSet.push(getShiftDetailsByCode(shiftSetOfCurrentDate.shift1));
            }
            if (!$.isEmptyObject(shiftSetOfCurrentDate.shift2)) {
                shiftSet.push(getShiftDetailsByCode(shiftSetOfCurrentDate.shift2));
            }
            returnValue = shiftSet;
        }

        return returnValue;
    }
	
	
	function parseCustomDate(dateString) {
	  const parts = dateString.split(/[-\/ :]/);
	  // Giả sử định dạng là DD-MM-YYYY HH:mm:ss hoặc DD/MM/YYYY HH:mm:ss
	  return new Date(`${parts[2]}-${parts[1]}-${parts[0]}T${parts[3] || '00'}:${parts[4] || '00'}:${parts[5] || '00'}Z`);
	}

	function formatDateWithOffset(dateString, offsetHours) {
	  const date = new Date(dateString);
	  date.setHours(date.getHours() + offsetHours);
	  return date.toISOString().replace('T', ' ').substring(0, 19);
	}
	
	function formatShiftDate(item, timezoneOffset = 7) {
    if (typeof moment === 'function') {
        return moment.utc(item.shiftExchangeDate)
            .utcOffset(timezoneOffset * 60)
            .format('YYYY-MM-DD HH:mm:ss');
    }
    
    // Fallback
    const date = new Date(item.shiftExchangeDate);
    const offsetMs = timezoneOffset * 60 * 60 * 1000;
    const adjustedDate = new Date(date.getTime() + offsetMs);
    return adjustedDate.toISOString().replace('T', ' ').slice(0, 19);
}

	function deviceIsIpad() {
		const userAgent = navigator.userAgent;
		// Kiểm tra iPadOS (iPad)
		if (/iPad/.test(userAgent)) {
			return true;
		}
		// Với iOS 13 trở lên, iPad có thể có userAgent tương tự như iPhone, vì vậy ta cũng cần kiểm tra dấu hiệu này
		else if (/Macintosh/.test(userAgent) && navigator.maxTouchPoints > 1) {
			return true;  // Đây là cách phát hiện iPad trên iPadOS 13 trở lên
		}
		else if (/Android/.test(userAgent)) {
			return true;
		}
    return false;
}

    async function getShiftExchangeById(id) {
        let model = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: '',
            limit: ''
        }
        model.predicateParameters.push(id);
        var result = await cbService.getInstance().shiftExchange.getShiftExchange(model).$promise;
        if (result.isSuccess) {
            //Khiem added new value of $scope
            $scope.model.moduleTitle = "Shift Exchange";
            $scope.model.referenceNumber = result.object.referenceNumber;
            $scope.statusTranslate = $rootScope.getStatusTranslate(result.object.status);
            $scope.model.shiftExchangeItemsData = result.object.exchangingShiftItems;
            $scope.model.id = result.object.id;
            $scope.model.applyDate = new Date(result.object.applyDate);
            $scope.model.deptLine = result.object.deptLineId;
            $scope.model.deptDivision = result.object.deptDivisionId;
            $scope.model.createdById = result.object.createdById;
            $scope.form.id = result.object.id;
            $scope.form.applyDate = new Date(result.object.applyDate);
            $scope.form.dept_Line = result.object.deptLineId;
            $scope.form.dept_Division = result.object.deptDivisionId;
            $scope.shiftExchangeId = result.object.id;
            $scope.referenceNumber = result.object.referenceNumber;
            $scope.form.dept_LineName = result.object.deptLineName;
            $scope.form.dept_DivisionName = result.object.deptDivisionName;
            $scope.model.created = result.object.created;
            $scope.model.createdByFullName = result.object.fullName;
            await getDepartmentByUserId(result.object.createdById, result.object.deptLineId, result.object.deptDivisionId);
            //get quota ERD
            let args = result.object.exchangingShiftItems.map(function (item) {
                return { "sapCode": item.employeeCode, "exchangeDate": item.shiftExchangeDate.getFullYear() };
            })
            // userSapCodes = removeDuplicates(userSapCodes);
            let resultQuota = await cbService.getInstance().shiftExchange.getAvailableLeaveBalance(args).$promise;
            if (resultQuota.isSuccess) {
                absenceQuotas = resultQuota.object;
            }
            $scope.shiftExchangeItemsData = result.object.exchangingShiftItems;
            //ngan
            if ($scope.shiftExchangeItemsData.length) {
                for (var i = 0; i < $scope.shiftExchangeItemsData.length; i++) {
                    let currentItem = $scope.shiftExchangeItemsData[i];
					var ipad = deviceIsIpad();
					if (!ipad) {
						currentItem.shiftExchangeDate = moment.utc(currentItem.shiftExchangeDate).utcOffset(7).format('YYYY-MM-DD HH:mm:ss');
					}
					const userAgent = navigator.userAgent;
                    if (currentItem.reasonCode) {
                        let index = _.findIndex($scope.reasonsDataSource, x => {
                            return x.code == currentItem.reasonCode;
                        });
                        if (index == -1) {
                            $scope.reasonsDataSource.push({ code: currentItem.reasonCode, name: currentItem.reasonName });
                        }
                        valueReasonCode = currentItem.reasonCode;
                        setDataDropdownList("#reason_Id", $scope.reasonsDataSource, valueReasonCode);
                    }

                    let shiftSet = await getShiftSetByDate(currentItem.employeeCode, currentItem.shiftExchangeDate);
                    if (!$.isEmptyObject(currentItem) && !$.isEmptyObject(currentItem.currentShiftCode)) {
                        let currentShiftCodeValue = {
                            "code": currentItem.currentShiftCode,
                            "name": currentItem.currentShiftName,
                            "nameVN": currentItem.currentShiftName,
                            "startTime": null,
                            "endTime": null,
                            "showtextCode": currentItem.currentShiftCode
                        };
                        shiftSet.push(currentShiftCodeValue);
                    }

                    currentItem.shiftSet = shiftSet;
                }
            }
            //end
        }
        $timeout(function () {
            $scope.title = /*$translate.instant('SHIFT_EXCHANGE_APPLICATION_EDIT_TITLE') +*/ result.object.referenceNumber;
            $scope.exchangingShiftItems = result.object.exchangingShiftItems;
            mapGridForItem($scope.exchangingShiftItems);
        }, 0)
    }
    
    $scope.itemInprocess = function () {
        let ignoreStatus = ['Draft', 'Request To Change'];  
        return ignoreStatus.includes($scope.model.status) || !$scope.model.id;
    }

    $scope.sapCodeDataSource = [];
    function mapGridForItem(dataSource) {
        var count = 0;
        //$scope.form.exchangingShiftItems = new kendo.data.ObservableArray([]);
        let arrayTemporary = []
        dataSource.forEach(item => {
            var simpleItem = _.find($scope.shiftExchangeSimpleDatas, x => { return x.id == item.id });
            let sapCode = simpleItem ? simpleItem.sapCode : '';
            let fullName = simpleItem ? simpleItem.fullName : '';
            item['no'] = count + 1;
            count = item['no'];
            item['fullName'] = fullName ? fullName : item.userFullName;
            // item['employeeCode'] = sapCode ? sapCode : item.userSAPCode;
            item['exchangeDate'] = new Date(item.shiftExchangeDate);
            item['currentShift'] = item.currentShiftCode;
            item['newShift'] = item.newShiftCode;
            item['reason'] = item.reasonCode;
            item['otherReason'] = item.otherReason;
            //item
            //$scope.form.exchangingShiftItems.push(item);
            // if ($scope.sapCodeDataSource && $scope.sapCodeDataSource.length && $scope.sapCodeDataSource[1].length == 0) {
            //     if (_.findIndex($scope.sapCodeDataSource[item.no], x => {
            //         return x.sapCode == sapCode;
            //     }) == -1) {
            //         $scope.sapCodeDataSource[item.no].push({ id: simpleItem.userId, fullName: fullName, sapCode: sapCode, showtextCode: sapCode });
            //     } else {
            //         $scope.sapCodeDataSource[item.no].push(item);
            //     }
            // }
            if (absenceQuotas) {
                var resultQuotaERDSapCode = absenceQuotas.find(x => x.employeeCode == item.employeeCode && x.year == item.exchangeDate.getFullYear() && x.absenceQuotaType == commonData.typeAvailableLeaveBalance.ERD);
                //var resultQuotaDOFLSapCode = absenceQuotas.find(x => x.employeeCode == item.employeeCode && x.absenceQuotaType == commonData.typeAvailableLeaveBalance.DOFL);
                if (resultQuotaERDSapCode) {
                    item['quotaERD'] = resultQuotaERDSapCode.remain;
                    //item['quotaDOFL'] = resultQuotaDOFLSapCode.remain;
                }
                else {
                    item['quotaERD'] = 0;
                }
            }
            else {
                item['quotaERD'] = 0;
            }
            arrayTemporary.push(item);
        });
        $scope.form.exchangingShiftItems = new kendo.data.ObservableArray(arrayTemporary);
        //$scope.form.exchangingShiftItems = _.orderBy($scope.form.exchangingShiftItems, ['shiftExchangeDate'], ['asc']);
        initShiftExchangeEditListGrid($scope.form.exchangingShiftItems);
    }

    function mapGridForApprove(dataSource) {
        var count = 0;
        dataSource.forEach(item => {
            var simpleItem = _.find($scope.shiftExchangeSimpleDatas, x => { return x.id == item.id });
            let sapCode = simpleItem.sapCode;
            let fullName = simpleItem.fullName;
            item['no'] = count + 1;
            count = item['no'];
            item['fullName'] = fullName ? fullName : item.userFullName;
            item['employeeCode'] = sapCode ? sapCode : item.userSAPCode;
            item['exchangeDate'] = new Date(item.shiftExchangeDate);
            item['currentShift'] = item.currentShiftCode;
            item['newShift'] = item.newShiftCode;
            item['reason'] = findValueByField($scope.reasonsDataSource, 'code', 'name', item.reasonCode);
            item['otherReason'] = item.otherReason;
            //approve
            $scope.modelForView.exchangingShiftItems.push(item);

            // if (_.findIndex($scope.employeesDataSource, x => {
            //         return x.sapCode == sapCode;
            //     }) == -1) {
            //     $scope.employeesDataSource.push({ id: simpleItem.userId, fullName: fullName, sapCode: sapCode, showtextCode: sapCode, isRemoved: true });
            // }

        });
        initShiftExchangeEditListGridApprove($scope.modelForView.exchangingShiftItems);

    }

    function initShiftExchangeEditListGrid(dataSource) {
        let grid = $("#shiftExchangeEditListGrid").data("kendoGrid");
        let dataSourceShiftExchange = new kendo.data.DataSource({
            data: dataSource,
        });
        if (grid) {
            grid.setDataSource(dataSourceShiftExchange);
        }
        if (!$scope.perm) {
            disableElement(true);
        }

    }

    function initShiftExchangeEditListGridApprove(dataSource) {
        let grid = $("#shiftExchangeEditListGridApprove").data("kendoGrid");
        let dataSourceShiftExchange = new kendo.data.DataSource({
            data: dataSource,
        });

    }

    $scope.allOrMyRequestGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    if ($rootScope.currentUser) {
                        $scope.optionSave = e;
                        await getAllShiftExchange(e);
                    }
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.data }
            }
        },
        resizable: false,
        sortable: false,
        autoBind: true,
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
            locked: true,
            lockable: false,
            width: "350px",
            template: function (dataItem) {
                var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                //return `<workflow-status status="${dataItem.status}"></workflow-status>`;
            }
        },
        {
            field: "referenceNumber",
            //title: "Reference Number",
            headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
            locked: true,
            lockable: false,
            width: "180px",
            template: function (dataItem) {
                return `<a ui-sref= "${stateItem}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "userSAPCode",
            //title: "SAP Code",
            headerTemplate: $translate.instant('COMMON_SAP_CODE'),
            width: "150px",
            lockable: false
        },
        {
            field: "userFullName",
            //title: "Full Name",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            width: "200px"
        },
        {
            field: "deptName",
            //title: "Dept/ Line",
            headerTemplate: $translate.instant('COMMON_DEPT_LINE'),
            width: "350px"
        },
        {
            field: "divisionName",
            //title: "Division/ Group",
            headerTemplate: $translate.instant('COMMON_DIVISION_GROUP'),
            width: "350px"
        },
        {
            field: "created",
            //title: "Created Date",
            headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
            width: "200px",
            lockable: false,
            template: function (dataItem) {
                return moment(dataItem.created).format(appSetting.longDateFormat);
            }
        }
        ]
    };
    $scope.isModalVisible = false;
    $scope.toggleFilterPanel = async function (value) {
        $scope.advancedSearchMode = value;
        $scope.isModalVisible = value;
        if (value) {
            //await GetDepartment();
            if (!$scope.query.departmentId || $scope.query.departmentId == '') {
                $timeout(function () {
                    setDataDepartment(allDepartments);
                }, 0);
            }
        }

    };
    // Event
    function loadPageOne() {
        let grid = $("#gridRequests").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
    }

    // Event
    $scope.applySearch = function () {
        // if ($scope.query.departmentId) {

        // }
        // getAllShiftExchange();
        loadPageOne();
        $scope.toggleFilterPanel(false);
    };

    $scope.clearSearch = function (query) {
        $scope.query = {
            keyword: '',
            userSAPCode: '',
            departmentId: '',
            fromDate: null,
            toDate: null,
            status: ''
        };
        $scope.$broadcast('resetToDate', $scope.query.toDate);
        // getAllShiftExchange();
        loadPageOne();
    };

    // phần dành cho trang add/edit
    let requiredFields = [{
        fieldName: "applyDate",
        title: $translate.instant('SHIFT_EXCHANGE_APPLICATION_APPLY_DATE')
    },
    {
        fieldName: "dept_Line",
        title: "Dept/Line"
    },
        // {
        //     fieldName: "dept_Division",
        //     title: "Dept Division"
        // }
    ];

    $scope.form = {
        applyDate: null,
        exchangingShiftItems: new kendo.data.ObservableArray([
            //   {
            //   no: 1,
            //   fullName: "Châu Ngọc Trâm Anh",
            //   employeeCode: "SAP-437312218",
            //   exchangeDate: new Date(),
            //   currentShift: 1,
            //   newShift: 2,
            //   reason: 3
            // },
            // {
            //   no: 2,
            //   fullName: "Nguyễn Thị Hoài Thương",
            //   employeeCode: "SAP-527312218",
            //   exchangeDate: new Date(),
            //   currentShift: 1,
            //   newShift: 2,
            //   reason: 3
            // }
        ]),
        dept_Line: '',
        dept_LineName: '',
        dept_Division: '',
        id: '',
    };

    $scope.deptLineDataSource = [{
        name: "IT",
        code: 1
    },
    {
        name: "Stationery -Sports- Bike - Division",
        code: 2
    },
    {
        name: "Perishable – Fish & Meat Division",
        code: 3
    },
    {
        name: "Daily - Dairy - Group",
        code: 4
    }
    ];

    $scope.deptDivisionDataSource = [{
        name: "IT",
        code: 1
    },
    {
        name: "Stationery -Sports- Bike - Division",
        code: 2
    },
    {
        name: "Perishable – Fish & Meat Division",
        code: 3
    },
    {
        name: "Daily - Dairy - Group",
        code: 4
    }
    ];

    $scope.reasonsDataSource = [
        //   {
        //   name: "Bộ phận thiếu nhân sự",
        //   code: "1"
        // },
        // {
        //   name: "Yêu cầu công việc",
        //   code: "2"
        // },
        // {
        //   name: "Lý do khác (ghi rõ lý do)",
        //   code: "3"
        // }
    ];


    async function getReasons() {
        var res = await settingService.getInstance().cabs.getAllReason({
            nameType: commonData.reasonType.SHIFT_EXCHANGE_REASON
        }).$promise;
        //ngan
        if (res.isSuccess) {
            res.object.data.forEach(element => {
                $scope.reasonsDataSource.push(element);
            });
        }
        //end
    }

    $scope.employeesDataSource = [];
    $scope.shiftExchangeDataSource = [];
    $scope.newShiftExchangeDataSource = [];

    $scope.settingOptions = {
        deptLineOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: $scope.deptLineDataSource,
        },
        deptDivisionOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: $rootScope.dropdownFilter,
            dataSource: $scope.deptDivisionDataSource,
            previous: ""
        },
    }

    $scope.idEditing = 0;
    $scope.errors = [];
    $scope.errorsTwo = {};


    requiredFieldsForTable = [{
        fieldName: "employeeCode",
        // title: "SAP Code"
        title: $translate.instant('SHIFT_EXCHANGE_SAPCODE_REQUIRED')
    },
    {
        fieldName: "exchangeDate",
        title: $translate.instant('SHIFT_EXCHANGE_APPLICATION_DATE_REQUIRED')
        // title: $translate.instant('COMMON_FIELD_IS_REQUIRED')
    },/*
    {
        fieldName: "currentShift",
        // title: "Current Shift"
        title: $translate.instant('SHIFT_EXCHANGE_CURRENT_SHIFT_REQUIRED')
    },*/
    {
        fieldName: "newShift",
        // title: "New Shift"
        title: $translate.instant('SHIFT_EXCHANGE_NEW_SHIFT_REQUIRED')
    },
    {
        fieldName: "reason",
        // title: "Reason"
        title: $translate.instant('SHIFT_EXCHANGE_REASON_REQUIRED')
    }
    ];

    // Phần code dùng chung
    function setDataSourceDropdown(idDropdown, items) {
        let dropdownlist = $(idDropdown).data("kendoDropDownList");
        dropdownlist.setDataSource(items);
    }

    $scope.renderHtml = function (htmlCode) {
        return $sce.trustAsHtml(htmlCode);
    };

    $scope.getLengthObjectKeys = function (obj) {
        return Object.keys(obj).length;
    }

    function employeeDropDownEditor(container, options) {
        $scope.idEditing = options.model.no;
        $(`<input required name="${options.field}" id="${options.field + options.model.uid}"/>`)
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "sapCode",
                dataValueField: "sapCode",
                dataSource: $scope.employeesDataSource,
                filter: "contains",
                customFilterFields: ['sapCode'],
                filtering: filterMultiField
            });
    }

    function shiftExchangeDropDownEditor(container, options) {
        $(`<input required name="${options.field}" id="${options.field + options.model.uid}"/>`)
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "name",
                dataValueField: "code",
                template: '<span><label>#: data.code# - #: data.name# </label></span>',
                valueTemplate: '#: name #',
                dataSource: $scope.shiftExchangeDataSource,
                filter: "contains",
                customFilterFields: ['code', 'name'],
                filtering: filterMultiField
            });
    }

    // $scope.isOther = true;

    $scope.changeReason = function (item) {
        var result = findReasonName(item.reason);
        if (result.includes("Lý do khác")) {
            item.otherReason = "";
            refreshGrid();
            // let rowEditing = $scope.form.exchangingShiftItems.find(
            //     x => x.no === $scope.idEditing
            // );
            // if (rowEditing) {
            //     rowEditing.otherReason = "";
            //     refreshGrid();
            // }
        }
    }

    $scope.isOther = function (item) {
        let result = $scope.reasonsDataSource.find(x => x.name.includes("Lý do khác"));
        if (result && result.code === item.reason) {
            return true;
        }
        item.otherReason = "";
        return false;
    }

    // function onChangeReason(e) {
    //     let control = e.sender;
    //     var result = findReasonName(control.value());
    //     if (result.includes("Lý do khác")) {
    //         let rowEditing = $scope.form.exchangingShiftItems.find(
    //             x => x.no === $scope.idEditing
    //         );
    //         if (rowEditing) {
    //             rowEditing.otherReason = "";
    //         }
    //     }
    // }

    function findReasonName(code) {
        var result;
        $scope.reasonsDataSource.forEach(item => {
            if (item.code === code) {
                result = item.name;
                return result;
            }
        });
        return result;
    }

    function reasonDropDownEditor(container, options) {
        $scope.idEditing = options.model.no;
        $(`<input required name="${options.field}"/>`)
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "name",
                dataValueField: "code",
                dataSource: $scope.reasonsDataSource,
                change: onChangeReason,
                filter: "contains"
            });
    }

    function otherReasonDropDownEditor(container, options) {
        var result = findReasonName(options.model.reason);
        if (result.includes("Lý do khác")) {
            $(`<input class="k-textbox" name="${options.field}"/>`).appendTo(
                container
            );
        }
        // if (options.model.reason === $scope.reasonsDataSource[2].code) {

        // }
    }

    function shiftExchangeDateEditor(container, options) {
        $(`<input kendo-date-picker remove-placeholder-date required name="${options.field}" k-format="'dd/MM/yyyy'" class="w100" k-date-input="true"/>`)
            .appendTo(container);
    }

    function getValueByCode(code, list, property) {
        if (list.length > 0) {
            let item = list.find(x => x.code === code);
            if (item) {
                return item[property];
            }
        }
        return "";
    }

    function addNewRecord() {
        let nValue = {
            no: $scope.form.exchangingShiftItems.length + 1,
            fullName: '',
            employeeCode: '',
            exchangeDate: null,
            currentShift: '',
            newShift: '',
            reason: '',
        };
        $scope.form.exchangingShiftItems.push(nValue);
    }

    $scope.deleteRecord = function (no) {
        $scope.no = no;
        $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
        $scope.dialog.bind("close", function (e) {
            if (e.data && e.data.value) {
                let currentDataOnGrid = $("#shiftExchangeEditListGrid").data("kendoGrid").dataSource._data;
                var index = _.findIndex(currentDataOnGrid, x => {
                    if (x.id) {
                        return x.id == $scope.form.exchangingShiftItems[$scope.no - 1].id;
                    } else if (x.uid) {
                        return x.uid == $scope.form.exchangingShiftItems[$scope.no - 1].uid;
                    }
                });
                currentDataOnGrid.splice(index, 1);
                $scope.form.exchangingShiftItems = currentDataOnGrid;
                //thêm code
                let count = 1;
                $scope.form.exchangingShiftItems.forEach(item => {
                    item.no = count;
                    count++;
                });
                $scope.exchangingShiftItems = $scope.form.exchangingShiftItems;
                Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
            }
        });
    }

    $scope.customvalue = "['code', 'name']"

    $scope.shiftExchangeEditListGridOptions = {
        dataSource: {
            data: $scope.form.exchangingShiftItems,
            pageSize: 100
        },
        sortable: false,
        pageable: false,
        // editable: true,
        columns: [{
            field: "no",
            //title: "No.",
            headerTemplate: $translate.instant('COMMON_NO'),
            width: "80px",
            editable: function (e) {
                return false;
            }
        },
        {
            field: "employeeCode",
            //title: "SAP Code",
            headerTemplate: $translate.instant('COMMON_SAP_CODE'),
            width: "200px",
            // editor: employeeDropDownEditor,
            template: function (dataItem) {
                return `<input class="k-textbox w100" name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.employeeCode" style="width: 150px"/>`;
            }
        },
        {
            field: "fullName",
            //title: "Full Name",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            width: "200px",
            // editable: function(e) {
            //     return false;
            // }
            template: function (dataItem) {
                return `<input class="k-textbox w100" name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.fullName" style="width: 150px"/>`;
            }
        },
        {
            field: "quotaERD",
            title: "Quota ERD",
            width: "100px",
            // editable: function(e) {
            //     return false;
            // }
            template: function (dataItem) {
                return `<input class="k-textbox w100" name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.quotaERD" style="width: 100%"/>`;
            }
        },
        // {
        //     field: "quotaDOFL",
        //     title: "Quota DOFL",
        //     width: "100px",
        //     // editable: function(e) {
        //     //     return false;
        //     // }
        //     template: function (dataItem) {
        //         return `<input name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.quotaDOFL" style="width: 100%"/>`;
        //     }
        // },
        {
            field: "exchangeDate",
            //title: "Shift Exchange Date",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_DATE'),
            width: "200px",
            format: "{0:dd/MM/yyyy}",
            // editor: shiftExchangeDateEditor
            template: function (dataItem) {
                return `<input kendo-date-picker
                    k-ng-model="dataItem.exchangeDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy'"
                    k-on-change="onChangeExchangeDate(dataItem)"
                    style="width: 100%;"/>`;
            }
        },
        {
            field: "currentShift",
            //title: "Current Shift",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_CURRENT_SHIFT'),
            width: "200px",
            // editor: shiftExchangeDropDownEditor,
            template: function (dataItem) {
                return `<select kendo-drop-down-list style="width: 100%;"
                    id="currentShiftSelect${dataItem.no}"
                    data-k-ng-model="dataItem.currentShift"
                    k-data-text-field="'showtextCode'"
                    k-data-value-field="'code'"
                    k-template="'<span><label>#: data.code# - #: data.name# (#:data.nameVN#)</label></span>'",
                    valueTemplate="'#: code #'",
                    k-auto-bind="'false'"
                    k-value-primitive="'false'"
                    k-data-source="dataItem.shiftSet"
                    k-filter="'contains'",
                    k-customFilterFields="['code', 'name']",
                    k-filtering="'filterMultiField'",
                    > </select>`;
            }
        },
        {
            field: "newShift",
            //title: "New Shift",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_NEW_SHIFT'),
            width: "200px",
            // editor: shiftExchangeDropDownEditor,
            template: function (dataItem) {
                //return findValue(item.shiftCode, $scope.shiftCodes, "code");
                return `<select kendo-drop-down-list style="width: 100%;"
                    data-k-ng-model="dataItem.newShift"
                    k-data-text-field="'showtextCode'"
                    k-data-value-field="'code'"
                    k-template="'<span><label>#: data.code# - #: data.name# (#:data.nameVN#)</label></span>'",
                    k-valueTemplate="'#: code #'",
                    k-auto-bind="'false'"
                    k-value-primitive="'false'"
                    k-data-source="newShiftExchangeDataSource"
                    k-filter="'contains'",
                    k-customFilterFields="['code', 'name']",
                    k-filtering="'filterMultiField'",
                    k-on-change="onChangeNewShift()"
                    > </select>`;
            }
        },
        {
            field: "reason",
            //title: "Reason",
            headerTemplate: $translate.instant('COMMON_REASON'),
            width: "200px",
            //editor: reasonDropDownEditor,
            template: function (dataItem) {
                return `<select kendo-drop-down-list id="reason_Id" style="width: 100%;"
                    data-k-ng-model="dataItem.reason"
                    k-data-text-field="'name'"
                    k-data-value-field="'code'"
                    k-template="'<span><label>#: data.name# </label></span>'",
                    k-valueTemplate="'#: name #'",
                    k-auto-bind="'true'"
                    k-on-change="changeReason(dataItem)"
                    k-value-primitive="'false'"
                    k-data-source="reasonsDataSource"
                    k-filter="'contains'",
                    > </select>`;
            }
        },
        {
            field: "otherReason",
            //title: "Other Reason",
            headerTemplate: $translate.instant('COMMON_OTHER_REASON'),
            width: "200px",
            template: function (dataItem) {
                return `<input name="otherReason" ng-readonly="!isOther(dataItem)" class="k-textbox w100" type="text" ng-model="dataItem.otherReason" style="width: 190px"/>`;
            }
        },
        {
            field: "isERD",
            //title: "Other Reason",
            headerTemplate: "ERD",
            width: "100px",
            template: function (dataItem) {
                return `<input type="checkbox" ng-model="dataItem.isERD" name="isERD{{dataItem.no}}"
                id="isERD{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;"/>
                <label class="form-check-label" for="isERD{{dataItem.no}}"></label>`
                    
            }
        },
        {
            title: "Actions",
            width: "100px",
            template: function (dataItem) {
                return `<a class="btn-border-upgrade btn-delete-upgrade" ng-click="deleteRecord(dataItem.no)"></a>`;
                
            }
        }
        ]
    };
    $scope.onChangeNewShift = function () {
        $timeout(function () {
            $scope.warning = warningERD();
        }, 0);
    }
    $scope.onChangeExchangeDate = async function (dataItem) {
        let args = [{ "sapCode": dataItem.employeeCode, "exchangeDate": dataItem.exchangeDate.getFullYear() }]
        let resultQuota = await cbService.getInstance().shiftExchange.getAvailableLeaveBalance(args).$promise;
        if (resultQuota.isSuccess) {
            let currentQuota = resultQuota.object.find(x => x.absenceQuotaType == commonData.typeAvailableLeaveBalance.ERD);
            dataItem.quotaERD = currentQuota ? currentQuota.remain : 0;
        }

        let arg = [{
            deptId: $scope.form.dept_Line,
            divisionId: $scope.form.dept_Division,
            sapCode: dataItem.employeeCode,
            //shiftExchangeDate: (new Date(dataItem.exchangeDate.setTime(dataItem.exchangeDate.getTime() + 7 * 60 * 60 * 1000)))
			shiftExchangeDate: dataItem.exchangeDate
        }];
        let resCurrentShift = await cbService.getInstance().shiftExchange.getCurrentShiftCodeFromShiftPlan(arg).$promise;
        if (resCurrentShift.isSuccess && resCurrentShift.object && resCurrentShift.object.data) {
            let data = resCurrentShift.object.data[0];
            if (data) {
                dataItem.currentShift = data.currentCode;
            }
        } else {
            dataItem.currentShift = '';
        }


        let shiftSet = await getShiftSetByDate(dataItem.employeeCode, dataItem.exchangeDate);
        dataItem.shiftSet = shiftSet;
        if ($.isEmptyObject(shiftSet) || shiftSet.length == 0) {
            dataItem.currentShift = "";
        } else if (dataItem.shiftSet.lengh == 1) {
            dataItem.currentShift = dataItem.shiftSet[0].code;
        }
        $scope.$applyAsync();

    }
    $scope.openDropdown = async function (dataItem) {
        //await $scope.getSapCodeByDivison();
        $scope.sapCodeDataSource[dataItem.no] = $scope.employeesDataSource;
    }
    $scope.closeDropdown = function (dataItem) {
        $scope.sapCodeDataSource[dataItem.no] = [];
        // var simpleItem = _.find($scope.shiftExchangeSimpleDatas, x => { return x.id == dataItem.id });
        // if (_.findIndex($scope.employeesDataSource, x => {
        //         return x.sapCode == dataItem.employeeCode;
        //     }) == -1 && dataItem.id) {
        //     $scope.sapCodeDataSource[dataItem.no].push({ id: simpleItem.userId, fullName: simpleItem.fullName, employeeCode: simpleItem.sapCode, showtextCode: simpleItem.sapCode });
        // }
        if ($scope.exchangingShiftItems) {
            mapGridForItem($scope.exchangingShiftItems);
        }

    }

    function refreshGrid() {
        let grid = $("#shiftExchangeEditListGrid").data("kendoGrid");
        grid.refresh();
    }

    function readDataSource(id) {
        let grid = $(id).data("kendoGrid");
        grid.dataSource.read();
    }

    function validationRequiredForTable(model, requiredFields) {
        let errors = [];
        requiredFields.forEach(field => {
            if (!model[field.fieldName]) {
                if (field.fieldName == 'currentShift') {
                    /*if (!$.isEmptyObject(model.shiftSet) && model.shiftSet.length > 0) {
                        errors.push({
                            message: `<span class="bold">${field.title} ${model.no}</span>: ` + $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                            groupName: $translate.instant('COMMON_ENTER_FIELD')
                        });
                    }*/
                }
                else {
                    errors.push({
                        message: `<span class="bold">${field.title} ${model.no}</span>: ` + $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                        groupName: $translate.instant('COMMON_ENTER_FIELD')
                    });
                }
            }
        });
        return errors;
    }

    // items ở đây là array table shift exchanges
    function validationDuplicateValue(items) {
        let errors = [];
        let itemsClone = _.cloneDeep(items);
        items.forEach(item => {
            if (item.exchangeDate) {
                let duplicates = itemsClone.filter(x => x.employeeCode === item.employeeCode && moment(x.exchangeDate.toDateString()).isSame(item.exchangeDate.toDateString()));
                itemsClone = itemsClone.filter(x => !(x.employeeCode === item.employeeCode && moment(x.exchangeDate.toDateString()).isSame(item.exchangeDate.toDateString())));

                if (duplicates.length > 1) {
                    errors.push({
                        // message: `<span class="bold">Row of No ${duplicates.map(x => x.no).join(', Row of No ')}</span><b>: SAP Code and Exchange Date are not duplicated </b>`,
                        // groupName: 'Duplicate values'
                        message: `<span class="bold">` + $translate.instant('COMMON_ROW_NO') + ` ${duplicates.map(x => x.no).join(', ' + $translate.instant('COMMON_ROW_NO') + ' ')}</span><b>: ` + $translate.instant('SHIFT_EXCHANGE_DUPLICATE_VALIDATE') + `</b>`,
                        groupName: $translate.instant('COMMON_VALIDATE_VALUE')
                    });
                }
            }

        });
        return errors;
    }

    function validationDuplicateCurrentShiftNewShift(items) {
        let errors = [];
        items.forEach(item => {
            if (item.currentShift == item.newShift) {
                errors.push({
                    message: `<span class="bold">` + $translate.instant('COMMON_ROW_NO') + ` ${item.no}</span>: ` + $translate.instant('SHIFT_EXCHANGE_DUPLICATE_SHIFT_VALIDATE'),
                    groupName: $translate.instant('COMMON_VALIDATE_VALUE')
                });
            }
        });
        return errors;
    }

    function Validation(form) {
        let errors = [];
        // kiểm tra xem các prop của form đã chọn chưa
        errors = validateForm($scope.form, requiredFields);

        errors = errors.map(({
            controlName
        }) => ({
            message: `<span class="bold">${controlName}</span>: ` + $translate.instant('COMMON_FIELD_IS_REQUIRED'),
            // groupName: 'Please enter all required fields'
            groupName: $translate.instant('COMMON_ENTER_FIELD')
        }));

        // kiểm tra xem danh sách shift exchange có dòng dữ liệu nào hay không, nếu có thì mới validation còn không thì show một cái message yêu cầu nhập vào
        if (!$scope.form.exchangingShiftItems.length) {
            errors.push({
                // message: `<span class="bold">Table shift exchange</span>: Field is required`,
                message: `<span class="bold">` + $translate.instant('SHIFT_EXCHANGE_APPLICATION_TABLE') + `</span>: ` + $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                //groupName: 'Please enter all required fields'
                groupName: $translate.instant('COMMON_ENTER_FIELD')
            });
        } else {
            // kiểm tra danh sách shiftExchange có chọn lí do khác hay không, nếu có thì add vào messsage lỗi
            $scope.form.exchangingShiftItems.forEach(item => {
                // kiểm tra xem có điền other reason hay không nếu chọn lý do khác
                var result = $scope.reasonsDataSource.find(x => x.name.includes("Lý do khác"))
                if (result && item.reason === result.code) {
                    /*if (!item.currentShift) {
                        errors.push({
                            //message: `<span class="bold">Other Reason of No ${item.no}</span>: ` + $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                            message: `<span class="bold">` + $translate.instant('SHIFT_EXCHANGE_CURRENT_SHIFT_REQUIRED') + ` ${item.no}</span>: ` + $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                            //groupName: 'Please enter all required fields'
                            groupName: $translate.instant('COMMON_ENTER_FIELD')
                        });
                    }*/
                    if (!item.otherReason) {
                        errors.push({
                            //message: `<span class="bold">Other Reason of No ${item.no}</span>: ` + $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                            message: `<span class="bold">` + $translate.instant('SHIFT_EXCHANGE_OTHER_REASON_REQUIRED') + ` ${item.no}</span>: ` + $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                            //groupName: 'Please enter all required fields'
                            groupName: $translate.instant('COMMON_ENTER_FIELD')
                        });
                    } 
                    
                }
                // kiếm tra xem các field trong bảng đã có dữ liệu chưa
                errors = errors.concat(validationRequiredForTable(item, requiredFieldsForTable));
                //HR 1077
                /*if (!item.currentShift.startsWith('V') && !item.newShift.startsWith('V')) {
                    errors.push({
                        message: `<span class="bold">${item.no} </span>: ` + $translate.instant('SHIFT_EXCHANGE_EROR_SHIFT_V') + ` ${item.newShift}`,
                    });
                }*/
                if (!errors.length) {
                    CheckIsStore($scope);
                    // let errorDateRange = validateDateRange(appSetting.rangeDate, item.exchangeDate, null, item.no, 'Shift Exchange');
                    let errorDateRange = validateDateRange($rootScope.salaryDayConfiguration, item.exchangeDate, null, item.no, 'SHIFT_EXCHANGE_MENU', $translate, 'SHIFT_EXCHANGE_PERIOD_VALIDATE_1', 'SHIFT_EXCHANGE_PERIOD_VALIDATE_2');
                    if (errorDateRange.length) {
                        errors.push({ message: `<span class="bold"> ${errorDateRange[0].controlName}</span>: ${errorDateRange[0].message}`, groupName: $translate.instant('SHIFT_EXCHANGE_PERIOD') });
                    }
                }
                //end
            });
            if (!errors.length) {
                $scope.form.exchangingShiftItems = $scope.form.exchangingShiftItems.map(function (item) {
                    return { ...item, date: item.exchangeDate }
                });
                errors = validateSalaryRangeDate($translate, $scope.form.exchangingShiftItems, appSetting.salaryRangeDate, 'SHIFT_EXCHANGE_MENU', 'SHIFT_EXCHANGE_PERIOD_VALIDATE_2', false);
                if (errors.length) {
                    errors = errors.map(function (item) {
                        return { groupName: $translate.instant('SHIFT_EXCHANGE_PERIOD'), message: `<span class="bold"> ${item.controlName}</span>: ${item.message}` }
                    })
                }
            }
            if (!errors.length) {
                // // kiểm tra xem có dòng nào bị trùng employee code và exchange date hay không
                errors = errors.concat(validationDuplicateValue($scope.form.exchangingShiftItems));

                //code moi
                errors = errors.concat(validationDuplicateCurrentShiftNewShift($scope.form.exchangingShiftItems));
                //
            }

        }

        // biến đổi các message cùng thuộc một group
        if (errors.length) {
            errors = _.groupBy(errors, 'groupName');
        }
        return errors;
    }

    function validateOther() {
        let errors = [];
        $scope.form.exchangingShiftItems.forEach(item => {
            // let errorDateRange = validateDateRange(appSetting.rangeDate, item.exchangeDate, null, item.no, 'Shift Exchange');
            //ngan them
            if (item.currentShift == 'ERD' && (item.newShift == 'ERD1' || item.newShift == 'ERD2')) {
                errors.push({
                    message: `<span class="bold">` + $translate.instant('COMMON_ROW_NO') + ` ${item.no}</span>: ` + $translate.instant('SHIFT_EXCHANGE_HALFDAY_VALIDATE'),
                    groupName: ''
                });
            }
            if (item.currentShift == 'TRN' && (item.newShift == 'TRN1' || item.newShift == 'TRN2')) {
                errors.push({
                    message: `<span class="bold">` + $translate.instant('COMMON_ROW_NO') + ` ${item.no}</span>: ` + $translate.instant('SHIFT_EXCHANGE_HALFDAY_VALIDATE'),
                    groupName: ''
                });
            }
            if (item.currentShift == 'CE' && (item.newShift == 'CE1' || item.newShift == 'CE2')) {
                errors.push({
                    message: `<span class="bold">` + $translate.instant('COMMON_ROW_NO') + ` ${item.no}</span>: ` + $translate.instant('SHIFT_EXCHANGE_HALFDAY_VALIDATE'),
                    groupName: ''
                });
            }
        });

        if (errors.length) {
            errors = _.groupBy(errors, 'groupName');
        }
        return errors;
    }

    $scope.changeSapCode = function (dataItem) {
        let propertiesNeedCheckChange = [{
            propCheck: 'employeeCode',
            propCompare: 'sapCode',
            propNeedChange: 'fullName',
            propGetValue: 'fullName',
            nameDataSource: 'employeesDataSource'
        }];

        propertiesNeedCheckChange.forEach(prop => {
            if (dataItem[prop.propCheck]) {
                let item = $scope[prop.nameDataSource].find(x => x[prop.propCompare] === dataItem[prop.propCheck]);
                if (item) {
                    dataItem[prop.propNeedChange] = item[prop.propGetValue];
                    refreshGrid();
                }
            }
        });
        return dataItem.employeeCode;
    }


    function findUserId(sapCode) {
        var result = $scope.employeesDataSource.find(x => x.sapCode === sapCode);
        if (result) {
            return result.id;
        } else {
            let item = _.find($scope.shiftExchangeSimpleDatas, x => {
                return x.sapCode == sapCode;
            });
            if (item) {
                return item.userId;
            }
            return '';
        }
    }
	
	
    async function save(form, perm, actionButtonName) {
        $scope.errorsTwo = {};
        $scope.errorsTwo = [];
        $scope.errors = [];
        var result = { isSuccess: false };
        var isValidation = false;
        // if (perm == 'undefined' || perm != 1) {
        //     isValidation = true;
        // } else {
        //     isValidation = false;
        // }
        // if (isValidation) {
        //     $scope.form.exchangingShiftItems = [];
        //     var gridData = $("#shiftExchangeEditListGrid").data("kendoGrid").dataSource._data;
        //     if (gridData.length > 0) {
        //         gridData.forEach(x => {
        //             $scope.form.exchangingShiftItems.push(x);
        //         });
        //     }
        //     $scope.errorsTwo = Validation(form);
        //     //code moi
        //     if ($scope.getLengthObjectKeys($scope.errorsTwo) == 0) {
        //         $scope.errorsTwo = validateOther(form);
        //     }
        //     //
        // }
        var timezoneOffset = null;
        let allowSendShiftExchange = await $scope.checkAllowSendShiftExchange();
        if (allowSendShiftExchange) {
            $scope.form.exchangingShiftItems = [];
            var gridData = $("#shiftExchangeEditListGrid").data("kendoGrid").dataSource._data;
            if (gridData.length > 0) {
                gridData.forEach(x => {
                    timezoneOffset = x.exchangeDate.getTimezoneOffset() * -1;
                    var convertDate = x.exchangeDate;
                    if (timezoneOffset != $scope.localtimezone) {
                        var diff = $scope.localtimezone - timezoneOffset;
                        convertDate.setMinutes(convertDate.getMinutes() + diff);
                    }

                    $scope.form.exchangingShiftItems.push({
                        ...x,
                        exchangeDate: convertDate
                    });

                });
                $scope.localtimezone = timezoneOffset;
            }
            $scope.errorsTwo = Validation(form);
            //code moi
            if ($scope.getLengthObjectKeys($scope.errorsTwo) == 0) {
                $scope.errorsTwo = validateOther(form);
            }
            $scope.$applyAsync();
            if (!$scope.getLengthObjectKeys($scope.errorsTwo)) {
                // $timeout(function () {
                //     $scope.warning = validateQuantityNewShift();
                // }, 0);

                if (!$scope.getLengthObjectKeys($scope.errorsTwo)) {
                    // var arraySAPCode = await validateQuantityNewShiftInServer();
                    // $scope.errorsTwo = translateValidateERD(arraySAPCode);
                    $scope.$applyAsync();
                    if (!$scope.getLengthObjectKeys($scope.errorsTwo)) {
                        let grid = $('#shiftExchangeEditListGrid').data("kendoGrid");
                        $scope.form.exchangingShiftItems.map(item => {
                            item["userId"] = item.userId;
                            item["shiftExchangeDate"] = moment(item.exchangeDate).format(
                                "MM/DD/YYYY"
                            );
                            item["currentShiftCode"] = item.currentShift;
                            item["currentShiftName"] = findValueByField(
                                $scope.shiftExchangeDataSource,
                                "code",
                                "name",
                                item.currentShift
                            );
                            item["newShiftCode"] = item.newShift;

                            item["newShiftName"] = findValueByField(
                                $scope.newShiftExchangeDataSource,
                                "code",
                                "name",
                                item.newShift
                            );
                            item["reasonCode"] = item.reason.toString();
                            item["reasonName"] = findValueByField(
                                $scope.reasonsDataSource,
                                "code",
                                "name",
                                item.reason
                            );
                        });
                        // lay cac dep da luu truoc do
                        if ($scope.oldExchangingShiftItems[0]) {
                            $scope.oldExchangingShiftItems[0].forEach(x => {
                                $scope.form.exchangingShiftItems.push(x);
                            });
                        }
						
					const userAgent = navigator.userAgent;
                        var model = {
                            id: $scope.shiftExchangeId,
                            // status: 'Draf',
                            applyDate: $scope.form.applyDate ? $scope.form.applyDate.toLocaleDateString("en-US") : "",
                            deptLineId: $scope.form.dept_Line,
                            deptDivisionId: $scope.form.dept_Division,
                            exchangingShiftItems: $scope.form.exchangingShiftItems,
                            currentUserId: $rootScope.currentUser.id,
                            status: $scope.model.status,
                            isVNMessage: $translate.preferredLanguage() == "vi_VN"
                        };
                        //AEON_658
                        if (actionButtonName == "Send Request") {
                            var res = await cbService.getInstance().shiftExchange.checkShiftExchangeComplete(model).$promise;
                            if (!res.isSuccess) {
                                var errorInfos = res.object;
                                if (!$.isEmptyObject(errorInfos) && $.type(errorInfos) == "array") {
                                    let errors = [];
                                    $(errorInfos).map(function (index, currentErrorInfo) {
                                        if (!$.isEmptyObject(currentErrorInfo) && !currentErrorInfo.success) {
                                            errors.push(
                                                {
                                                    errorDetail: currentErrorInfo.message + ': ' + $translate.instant(currentErrorInfo.errorCode),
                                                    isRule: false
                                                }
                                            );
                                        }
                                    });
                                    $scope.errors = errors;
                                    $scope.$apply();
                                    return res;
                                }
                            }
                        }
						
                    
                        var res = await cbService.getInstance().shiftExchange.saveShiftExchange(model).$promise;
                        if (res.isSuccess) {
                            // Notification.success("Data Successfully Save");
                            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                            $scope.model = _.cloneDeep(res.object);
                            $scope.form.applyDate = new Date(res.object.applyDate);
                            $scope.form.dept_Line = res.object.deptLineId;
                            $scope.form.dept_Division = res.object.deptDivisionId;
                            $scope.temporaryDivision = $scope.form.dept_Division;
                            $scope.shiftExchangeId = res.object.id;
                            $scope.referenceNumber = res.object.referenceNumber;
                            $timeout(function () {
                                $scope.form.id = res.object.id;
                                $scope.title = $translate.instant('SHIFT_EXCHANGE_APPLICATION_EDIT_TITLE') + res.object.referenceNumber;
                            }, 10);
                            // await getShiftExchangeDetailSimpleById($scope.model.id);
                            // mapGridForItem(res.object.exchangingShiftItems);
                            let arg = {
                                predicate: '',
                                predicateParameters: [],
                                order: appSetting.ORDER_GRID_DEFAULT,
                                page: '',
                                limit: ''
                            }
                            arg.predicateParameters.push($scope.model.id);
                            var result = await cbService.getInstance().shiftExchange.getShiftExchange(arg).$promise;
                            if (result.isSuccess) {
                                mapGridForItem(result.object.exchangingShiftItems);
                            }
                            $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });

                        } else {
                            //Notification.success("Error System");
                            var errorInfos = res.object;
                            if (!$.isEmptyObject(errorInfos) && $.type(errorInfos) == "array") {
                                let errors = [];
                                $(errorInfos).map(function (index, currentErrorInfo) {
                                    if (!$.isEmptyObject(currentErrorInfo) && !currentErrorInfo.success) {
                                        errors.push(
                                            {
                                                errorDetail: currentErrorInfo.message + ': ' + $translate.instant(currentErrorInfo.errorCode) + ' ' + (currentErrorInfo.description ? currentErrorInfo.description : ""),
                                                isRule: false
                                            }
                                        );
                                    }
                                });
                                $scope.errors = errors;
                                $scope.$apply();
                            }
                        }
                        $scope.checkDeptNameUser = true
                        $rootScope.loadingDialog(null, false);
                        return res;
                    }
                }
            } else {
                $rootScope.loadingDialog(null, false);
                return result;
            }
        }
    }

    async function saveItem(form, perm, actionButtonName) {
        var res = await save(form, perm, actionButtonName);
        return res;
    }

    function findValueByField(dataSource, nameFieldCompare, nameFieldGetValue, valueCheck) {
        var result = dataSource.find(x => x[nameFieldCompare] === valueCheck);
        if (result) {
            return result[nameFieldGetValue];
        } else {
            return "";
        }
    }

    // phần dành cho trang view để approve or request to change ....
    $scope.modelForView = {
        id: '213123123',
        applyDate: '',
        exchangingShiftItems: [
            //   {
            //   no: 1,
            //   fullName: "Châu Ngọc Trâm Anh",
            //   employeeCode: "SAP-437312218",
            //   exchangeDate: new Date(),
            //   currentShift: "V601",
            //   newShift: "BTD",
            //   reason: "Bộ phận thiếu nhân sự"
            // },
            // {
            //   no: 2,
            //   fullName: "Nguyễn Thị Hoài Thương",
            //   employeeCode: "SAP-527312218",
            //   exchangeDate: new Date(),
            //   currentShift: "V602",
            //   newShift: "BTD",
            //   reason: "Đi support AEON Canary Bình Dương"
            // },
            // {
            //   no: 3,
            //   fullName: "Nguyễn Thị Bảo Khánh",
            //   employeeCode: "SAP-167312218",
            //   exchangeDate: new Date(),
            //   currentShift: "V603",
            //   newShift: "BTD",
            //   reason: "Lý do công việc"
            // }
        ],
        dept_Line: '',
        dept_Division: ''
    };

    $scope.listOfShiftExchangingOptions = {
        dataSource: {
            data: $scope.modelForView.exchangingShiftItems,
            pageSize: $rootScope.pageSizeDefault
        },
        sortable: false,
        // pageable: true,
        editable: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
        },
        columns: [{
            field: "no",
            //title: "No",
            headerTemplate: $translate.instant('COMMON_NO'),
            width: "100px"
        },
        {
            field: "fullName",
            //title: "Full Name",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            width: "200px"
        },
        {
            field: "employeeCode",
            //title: "SAP Code",
            headerTemplate: $translate.instant('COMMON_SAP_CODE'),
            width: "100px"
        },
        {
            field: "exchangeDate",
            //title: "Shift Exchange Date",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_DATE'),
            width: "100px",
            format: "{0:dd/MM/yyyy}"
        },
        {
            field: "currentShift",
            //title: "Current Shift",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_CURRENT_SHIFT'),
            width: "100px"
        },
        {
            field: "newShift",
            //title: "New Shift",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_NEW_SHIFT'),
            width: "100px"
        },
        {
            field: "reason",
            //title: "Reason",
            headerTemplate: $translate.instant('COMMON_REASON'),
            width: "200px"
        },
        {
            field: "otherReason",
            //title: "Other Reason",
            headerTemplate: $translate.instant('COMMON_OTHER_REASON'),
            width: "200px"
        }
        ]
    };

    function approve() {
        Notification.success("Approve Shift Exchange");
        return true;
    }

    function requestedToChange() {
        Notification.success("RequestedToChange Shift Exchange");
        return true;
    }

    function reject() {
        Notification.success("Reject Shift Exchange");
        return true;
    }

    $scope.dataTemporaryArrayDivision = [];
    $scope.getChildDivison = async function () {
        if ($scope.form.dept_Line) {
            var result = await settingService.getInstance().departments.getDivisionTree({
                departmentId: $scope.form.dept_Line
            }).$promise;
            if (result.isSuccess) {
                //$scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
                $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.type == 1 });
                setDataDeptDivision($scope.dataTemporaryArrayDivision)
                if (!result.object.data.length) {
                    await getUsersByDeptLine($scope.form.dept_Line, 5, '');
                }
            } else {
                Notification.error(result.messages[0]);
            }
        }
    }

    $scope.temporaryDivision = '';
    confirm = async function (e) {
        let dropdownlist = $("#deptDivision").data("kendoDropDownTree");
        //alert(dropdownlist.dataSource._data);
        if (e.data && e.data.value) {
            $scope.oldExchangingShiftItems.push($scope.form.exchangingShiftItems);
            if (!$scope.form.dept_Division) {
                clearSearchTextOnDropdownTree('deptDivision');
                /*$scope.form.exchangingShiftItems = new kendo.data.ObservableArray([]);
                initShiftExchangeEditListGrid($scope.form.exchangingShiftItems);*/
                if ($scope.dataTemporaryArrayDivision.length) {
                    setDataDeptDivision($scope.dataTemporaryArrayDivision);
                } else {
                    await getDivisionsByDeptLine($scope.form.dept_Line);
                    setDataDeptDivision($scope.dataTemporaryArrayDivision);
                }
                $scope.employeesDataSource = [];

            } else {
                if (dropdownlist) {
                    dropdownlist.value($scope.form.dept_Division);
                }
                $scope.form.exchangingShiftItems = new kendo.data.ObservableArray([]);
                initShiftExchangeEditListGrid($scope.form.exchangingShiftItems);
                //await $scope.getSapCodeByDivison()
                //await getSapCodeByDivison(1, $scope.limitDefaultGrid);
            }
        } else {
            if (dropdownlist) {
                dropdownlist.value($scope.temporaryDivision);
            }
            $scope.form.dept_Division = $scope.temporaryDivision;
            //await getSapCodeByDivison(1, $scope.limitDefaultGrid);
        }
    }


    async function getSapCodeByDivison(page, limit, searchText = "", isAll = false) {
        if ($scope.form.dept_Division) {
            let result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.form.dept_Division, limit: limit, page: page, searchText: searchText, isAll: isAll }).$promise;
            if (result.isSuccess) {
                $scope.employeesDataSource = result.object.data.map(function (item) {
                    return { ...item, showtextCode: item.sapCode }
                });
                $scope.total = result.object.count;
            }
        }
    }

    async function getUsersByDeptLine(deptId, limit, searchText, page = 1) {
        let result = await settingService.getInstance().users.getChildUsers({ departmentId: deptId, limit: limit, page: page, searchText: searchText }).$promise;
        if (result.isSuccess) {
            $scope.employeesDataSource = result.object.data.map(function (item) {
                return { ...item, showtextCode: item.sapCode }
            });
            $scope.total = result.object.count;
        }
    }
    async function getUserCheckedHeadCount(deptId, textSearch = "") {
        // check null division
        if ($("#deptDivision").val() == '' && $rootScope.currentUser.divisionId != null) {
            deptId = $rootScope.currentUser.divisionId;
        }

        let result = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: deptId, textSearch: textSearch }).$promise;
        if (result.isSuccess) {
            $scope.employeesDataSource = result.object.data.map(function (item) {
                return { ...item, showtextCode: item.sapCode }
            });
            $scope.total = result.object.count;
        }
    }

    // phần khai báo chung
    $scope.actions = {
        saveItem: saveItem,
        addNewRecord: addNewRecord
    };

    $scope.$on("$locationChangeStart", function (event, next, current) {
        // $scope.errors = [];
    });

    // hàm này sẽ chạy sau khi view được render lên
    $scope.$on('$viewContentLoaded', function () {
        if (!$stateParams.referenceValue && $state.current.name === 'home.shiftExchange.item') {
            $scope.form = {
                applyDate: null,
                exchangingShiftItems: new kendo.data.ObservableArray([
                    // {
                    //     no: $scope.form.exchangingShiftItems.length,
                    //     fullName: '',
                    //     employeeCode: '',
                    //     exchangeDate: new Date(),
                    //     currentShift: '',
                    //     newShift: '',
                    //     reason: '',
                    // }
                ]),
                dept_Line: '',
                dept_Division: ''
            };

            let grid = $('#shiftExchangeEditListGrid').data("kendoGrid");
            if (grid) {
                grid.setDataSource($scope.form.exchangingShiftItems);
            }
        }
    });

    async function getDeptLineByJobGradeId(jogGradeId) {
        var result = await settingService.getInstance().departments.getAllDeptLineByGrade({ jobGradeId: jogGradeId }).$promise;
        if (result.isSuccess) {
            $scope.deptLine = result.object.data;
            // if ($state.current.name === commonData.stateShiftExchange.item) {
            setDataDeptLine($scope.deptLine);
            // }
        }
    }

    async function getDeptLineByDeptName(name) {
        let queryArgs = {
            predicate: 'Name.contains(@0)',
            predicateParameters: [name],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000
        };

        var result = await settingService.getInstance().departments.getDepartments(queryArgs).$promise;
        if (result.isSuccess) {
            $scope.form.dept_Line = result.object.data[0].id;
            // $scope.form.dept_LineName = result.object.data[0].name && result.object.data[0].jobGradeCaption ? result.object.data[0].name + "(" + result.object.data[0].jobGradeCaption + ")" : result.object.data[0].name ? result.object.data[0].name : '';
            $scope.form.dept_LineName = result.object.data[0].name && result.object.data[0].jobGradeCaption ? result.object.data[0].name : '';
            $scope.$applyAsync()
        }

    }
    $scope.jobGrade = '';
    async function getJobGradeG5() {
        let queryArgs = {
            predicate: 'Grade = 5',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000
        };
        //
        var result = await settingService.getInstance().jobgrade.getJobGradeList(queryArgs).$promise;
        if (result.isSuccess) {
            $scope.jobGrade = result.object.data[0];
        }
    }

    async function getDepartmentByReferenceNumber(referenceNumber, id) {
        let referencePrefix = referenceNumber.split('-')[0];
        let res = await settingService.getInstance().departments.getDepartmentByReferenceNumber({ prefix: referencePrefix, itemId: id }).$promise;
        $scope.currentDivision = res.object.data[0];
        if (res.isSuccess) {
            setDataDeptDivision(res.object.data);
        }
    }
    $scope.limitDefaultGrid = 20;
    $scope.checkDeptNameUser = true;
    this.$onInit = function () {

        $scope.deptLineOption = {
            dataTextField: 'name',
            dataValueField: 'id',
            autoBind: false,
            valuePrimitive: true,
            filter: "contains",
            dataSource: [],
            change: async function (e) {
                $scope.total = 0;
                let i = e.sender;
                let deptId = i.value();
                $scope.errorsTwo = {};
                if (deptId) {
                    $scope.dept_Line = deptId;
                    $scope.previousDeptId = deptId;
                    $scope.form.exchangingShiftItems = [];
                    $scope.employeesDataSource = [];
                    clearGrid('shiftExchangeEditListGrid');
                    clearSearchTextOnDropdownTree('deptDivision');

                    var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
                    if (!currentItem.divisions.length) { // User là member của DeptLine                      
                        await $scope.getChildDivison();
                        await getUserCheckedHeadCount($scope.dept_Line);
                    } else {
                        var ids = _.map(currentItem.divisions, 'id');
                        await getDivisionTreeByIds(ids);
                    }
                }
            }
        };

        if ($state.current.name === 'home.shiftExchange.myRequests') {
            $scope.selectedTab = "1";
        }
        else if ($state.current.name === 'home.shiftExchange.allRequests') {
            $scope.selectedTab = "0";
        }
    }

    $scope.openANewRequest = function () {
        $state.go('home.shiftExchange.item');
    }
    $scope.onTabChange = function () {
        localStorage.setItem('selectedTab', $scope.selectedTab);
        if ($scope.selectedTab === "1") {
            $state.go('home.shiftExchange.myRequests');
        } else if ($scope.selectedTab === "0") {
            $state.go('home.shiftExchange.allRequests');
        }
    };
    async function ngOnInit() {
        if ($state.current.name === commonData.stateShiftExchange.item || $state.current.name === commonData.stateShiftExchange.approve) {
            //check xem user có dept/line ko    
            await getShiftCodes();
            await getNewShiftCodes();
            await getReasons();
            if ($stateParams.id) {
                await getDepartmentByReferenceNumber($stateParams.referenceValue, $stateParams.id);
                await getShiftExchangeById($stateParams.id);
                await getWorkflowProcessingStage($stateParams.id);
            } else {
                $scope.currentInstanceProcessingStage = null;
                $timeout(async function () {
                    if ($rootScope.currentUser) {
                        $scope.form.dept_LineName = $rootScope.currentUser.deptName ? $rootScope.currentUser.deptName : '';
                        $scope.form.dept_Line = $rootScope.currentUser.deptId;
                        $scope.form.dept_Division = $rootScope.currentUser.divisionId;
                        $scope.form.applyDate = new Date();
                        $scope.model.createdByFullName = $rootScope.currentUser.fullName;
                        await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId, '');

                    }
                }, 0);
            }
            $timeout(function () {
                $scope.warning = warningERD();
            }, 0);
        }
        var currentDate = new Date();
        $scope.localtimezone = -1 * currentDate.getTimezoneOffset();

        setTimeout(function () {
            document.querySelectorAll('.tooltip-custom').forEach((tooltip) => {
                const tooltipText = tooltip.querySelector('.tooltip-text');
                const parentRect = document.querySelector('.portlet-body').getBoundingClientRect();
                tooltip.addEventListener('mouseenter', function () {
                    const tooltipRect = tooltipText.getBoundingClientRect();
                    if (tooltipRect.right > parentRect.right) {
                        tooltip.classList.add('tooltip-left');
                    }
                });
            });
        }, 10);
    }

    async function getWorkflowProcessingStage(itemId) {
        var result = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: itemId }).$promise;
        if (result.isSuccess && result.object) {
            $scope.currentInstanceProcessingStage = result.object.workflowInstances[0];
            $scope.processingStageRound = result.object.workflowInstances.length;
            if (!$scope.currentInstanceProcessingStage) return;
            $scope.currentInstanceProcessingStage.workflowData.steps.map((item) => {
                var findHistories = _.find($scope.currentInstanceProcessingStage.histories, x => { return x.stepNumber == item.stepNumber });
                if (findHistories != null) {
                    item.histories = findHistories;
                    return item;
                }
            });
        }
    }
    function setDefaultDateItem() {
        $scope.date = {
            fromDate: null,
            toDate: null
        }
        let fromDatePicker = $('#toDate').data('kendoDatePicker');
        let toDatePicker = $('#fromDate').data('kendoDatePicker');
        fromDatePicker.value(null);
        toDatePicker.value(null);
    }

    async function getDepartmentByUserId(userId, deptId, divisionId) {
        $scope.deptLineList = [];
        $scope.dept_Line = deptId;
        let res = await settingService.getInstance().departments.getDepartmentsByUserId({ id: userId }, null).$promise;
        if (res && res.isSuccess) {
            $scope.deptLineList = res.object.data;
            var deptLines = _.map($scope.deptLineList, x => { return x['deptLine'] });
            var dropdownlist = $("#deptLineOptionId").data("kendoDropDownList");
            if (dropdownlist)
                dropdownlist.setDataSource(deptLines);
            if (deptId) {
                if (dropdownlist) {
                    dropdownlist.value(deptId);
                }
            }
            if (!$stateParams.id) {
                if (!divisionId) {
                    await getDivisionsByDeptLine(deptId);
                }
                else {
                    await getDivisionsByDeptLine(deptId, divisionId);
                }
            } else {
                if (!divisionId) {
                    await getDivisionsByDeptLine(deptId);
                }
            }
        }
    }

    async function getDivisionsByDeptLine(deptId, divisionId = '') {
        var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
        var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
        if (dropdownlist) {
            if (currentItem && currentItem.divisions && currentItem.divisions.length) {
                var ids = _.map(currentItem.divisions, 'id');
                await getDivisionTreeByIds(ids);
            } else {
                await getDivisionTreeByIds([deptId]);
            }
            if (divisionId) {
                dropdownlist.value(divisionId);
                $scope.form.dept_Division = divisionId;
            }
        }

    }

    async function getShiftExchangeDetailSimpleById(id) {
        var result = await cbService.getInstance().shiftExchange.getShiftExchangeDetailById({ id: id }, null).$promise;
        if (result.isSuccess) {
            $scope.shiftExchangeSimpleDatas = result.object.data;
            $scope.sapCodeDataSource = [];
            for (let i = 0; i < $scope.shiftExchangeSimpleDatas.length; i++) {
                $scope.sapCodeDataSource[i + 1] = [];
            }
        }
    }
    $scope.export = async function () {
        let args = buildArgs(appSetting.numberSheets, appSetting.numberRowPerSheets);
        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status, 'status');
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.SHIFTEXCHANGE
        }, args).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }
    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }
    $scope.showTrackingHistory = function () {
        $rootScope.visibleTrackingHistory($translate, appSetting.TrackingLogDialogDefaultWidth);
    }
    ngOnInit();

    $scope.gridUser = {
        keyword: '',
    }

    $scope.gridUserReview = {
        keyword: '',
    }

    $scope.dialogDetailOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_REVIEWUSER'),
            action: function (e) {
                $scope.reviewUser();
                $scope.reviewOk = false;
                return false;
            },
            primary: true
        }]
    };
    $scope.fromDateChanged = function () {
        if ($scope.date.fromDate) {
            $('#toDate').data('kendoDatePicker').min(new Date($scope.date.fromDate));
            if ($scope.date.toDate && $scope.date.toDate.getDate() < $scope.date.fromDate.getDate()) {
                $scope.date.toDate = null;
            }
        }
    };
    $scope.dialogDetailReviewOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                if (!$scope.date.fromDate) {
                    $scope.review.errorMessage = `From Date: ${$translate.instant('COMMON_FIELD_IS_REQUIRED')}`;
                    $scope.$apply();
                    return false;
                } else {
                    $scope.reviewOk = true;
                    $scope.isShow = false;
                    $scope.originalDivision = $scope.form.dept_Division;
                    $scope.closeReviewDialog(false);
                    $scope.allCheck = false;
                    $scope.arrayCheck = [];
                    $scope.form.exchangingShiftItems = new kendo.data.ObservableArray($scope.form.exchangingShiftItems);
                    //code moi
                    let arrayDay = [];
                    arrayDay.push($scope.date.fromDate);
                    if ($scope.date.toDate) {
                        let tempDate = $scope.date.fromDate;
                        while (tempDate < $scope.date.toDate) {
                            tempDate = addDays(tempDate, 1);
                            arrayDay.push(tempDate);
                        }
                    }
                    //

                    addReviewedUser(arrayDay);
                    let dialog = $("#dialog_Detail").data("kendoDialog");
                    dialog.close();
                    // kendo.ui.progress($("#loading"), false);
                    return true;
                }

            },
            primary: true
        }]
    };
    function generateQuery(sapCodes, arrDay) {

        let args = [];
        _.forEach(arrDay, function (date) {
            _.forEach(sapCodes, function (sap) {
                args.push(
                    {
                        "sapCode": sap
                        , "exchangeYear": date.getFullYear()
                        , "exchangeDate": moment(date).format('YYYYMMDD')
                    })
            });
        });
        return args;

    }
    absenceQuotas = []
    shiftCodeSets = []

    let getShiftDetailsByCode = function (shiftCode) {
        let cArray = $($scope.shiftExchangeDataSource).filter(function (index, cShiftDetails) {
            return cShiftDetails.code == shiftCode;
        })

        if (cArray.length > 0) {
            return cArray[0];
        }
        else {
            return {
                "code": shiftCode,
                "name": shiftCode,
                "nameVN": "",
                "startTime": "",
                "endTime": "",
                "showtextCode": shiftCode
            };
        }
    }

    async function addReviewedUser(arrayDay) {
        let count = 1;
        let tempItems = $scope.form.exchangingShiftItems.toJSON();
        let userSapCodes = $scope.userGridReview.map(x => x.sapCode);

        let getShiftSet = function (sapcode, date) {
            let array = $(shiftCodeSets).filter(function (index, cItem) {
                return cItem.employeeCode == sapcode && cItem.exchangeDT == date;
            });

            if (!$.isEmptyObject(array) && array.length > 0) {
                let shiftSet = new Array();
                let shiftSetOfCurrentDate = array[0];
                if (!$.isEmptyObject(shiftSetOfCurrentDate.shift1)) {
                    shiftSet.push(getShiftDetailsByCode(shiftSetOfCurrentDate.shift1));
                }
                if (!$.isEmptyObject(shiftSetOfCurrentDate.shift2)) {
                    shiftSet.push(getShiftDetailsByCode(shiftSetOfCurrentDate.shift2));
                }
                return shiftSet;
            }
            else {
                return null;
            }
        };


        tempItems.forEach(x => {
            let result = userSapCodes.find(y => y == x.employeeCode);
            if (!result) {
                userSapCodes.push(x.employeeCode);
            }
        });
        const args = generateQuery(userSapCodes, arrayDay);
        // userSapCodes = removeDuplicates(userSapCodes);
        let resultQuota = await cbService.getInstance().shiftExchange.getAvailableLeaveBalance(args).$promise;
        if (resultQuota.isSuccess) {
            absenceQuotas = resultQuota.object;
        }
        let shiftSetResult = await ssgexService.getInstance().remoteDatas.getShiftSetArrayByDate(args).$promise;
        if (shiftSetResult.isSuccess) {
            shiftCodeSets = shiftCodeSets.concat(shiftSetResult.object);
        }
        $scope.userGridReview.forEach(item => {
            let value = {
                no: count,
                userId: item.id,
                fullName: item.fullName,
                employeeCode: item.sapCode,
                exchangeDate: new Date(),
                currentShift: '',
                newShift: '',
                reason: '',
                quotaERD: '',
                quotaDOFL: ''
            }

            if (arrayDay.length > 0) {
                arrayDay.forEach(x => {
                    temp = _.clone(value);
                    temp.exchangeDate = x;
                    tempItems.push(temp);
                    count++;
                });
            }
        });
        $scope.form.exchangingShiftItems = new kendo.data.ObservableArray(tempItems);
        let currentShiftArgs = [];
        _.forEach(tempItems, x => {
            if (x.exchangeDate) {
                currentShiftArgs.push({ deptId: $scope.form.dept_Line, divisionId: $scope.form.dept_Division, sapCode: x.employeeCode, shiftExchangeDate: x.exchangeDate })
            }
        });
        if (currentShiftArgs.length) {
            let resCurrentShifts = await cbService.getInstance().shiftExchange.getCurrentShiftCodeFromShiftPlan(currentShiftArgs).$promise;
            let data = new Array();
            if (resCurrentShifts.isSuccess) {
                data = resCurrentShifts.object.data;
            }
            if (true) {
                let itemCount = $scope.form.exchangingShiftItems.length;
                for (var i = 0; i < itemCount; i++) {
                    let currentItem = $scope.form.exchangingShiftItems[i];
                    if (arrayDay.indexOf(currentItem.exchangeDate) != -1) {
                        let currentActual = _.find(data, y => {
                            return currentItem.employeeCode == y.sapCode && dateFns.isEqual(y.shiftExchangeDate, currentItem.exchangeDate);
                        });

                        if (currentActual) {
                            currentItem.currentShift = currentActual.currentCode;
                        }
                        let exchangeDate = moment(currentItem.exchangeDate).format('YYYYMMDD');

                        let shiftSet = await getShiftSetByDate(currentItem.employeeCode, currentItem.exchangeDate);
                        currentItem.shiftSet = shiftSet;
                        if ($.isEmptyObject(shiftSet) || shiftSet.length == 0) {
                            currentItem.currentShift = "";
                        }
                    }
                }
            }
        }

        let grid = $('#shiftExchangeEditListGrid').data("kendoGrid");
        if (grid) {
            let i = 1;
            $scope.form.exchangingShiftItems.forEach(item => {
                item.no = i;
                i++;
                if (absenceQuotas) {
                    var resultQuotaERDSapCode = absenceQuotas.find(x => x.employeeCode == item.employeeCode && x.year == item.exchangeDate.getFullYear() && x.absenceQuotaType == commonData.typeAvailableLeaveBalance.ERD);
                    //var resultQuotaDOFLSapCode = absenceQuotas.find(x => x.employeeCode == item.employeeCode && x.absenceQuotaType == commonData.typeAvailableLeaveBalance.DOFL);
                    if (resultQuotaERDSapCode) {
                        item.quotaERD = resultQuotaERDSapCode.remain;
                        //item.quotaDOFL = resultQuotaDOFLSapCode.remain;
                    }
                    else {
                        item.quotaERD = 0;
                    }
                }
                else {
                    item.quotaERD = 0;
                }
            });
            grid.setDataSource($scope.form.exchangingShiftItems);
        }
        $timeout(function () {
            $scope.warning = warningERD();
        }, 0);


    }
    $scope.closeReviewDialog = function (openDialog_Detail) {
        $scope.dialogVisible = false;
        if (openDialog_Detail && !$scope.reviewOk) {
            let dialog = $("#dialog_Detail").data("kendoDialog");
            dialog.title($translate.instant('COMMON_BUTTON_ADDUSER'));
            dialog.open();
        }
    }

    $scope.userGrid = [];
    $scope.userListGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 10,
            transport: {
                read: async function (e) {
                    //kendo.ui.progress($("#loading"), false);
                    await getSAPCode(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.userGrid }
            }
        },
        sortable: false,
        autoBind: true,
        resizable: true,
        pageable: {
            // pageSize: appSetting.pageSizeWindow,
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 30, 40],
            responsive: false,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [
            {
                field: "isCheck",
                title: "<label class='form-check-label' for='allCheck' style='padding-left: 30px; padding-bottom: 10px;'></label><input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='form-check-input' ng-change='onChange(allCheck)'/>",
                width: "50px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;" ng-change='changeCheck(dataItem)'/>
                    <label class="form-check-label" for="isCheck{{dataItem.no}}"></label>`
                }
            },
            {
                field: "sapCode",
                // title: "SAP Code",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "150px",
            },
            {
                field: "fullName",
                // title: "Full Name",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "department",
                // title: "Department",
                headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                width: "200px",
            },
            {
                field: "position",
                // title: "Position",
                headerTemplate: $translate.instant('COMMON_POSITION'),
                width: "200px",
            },
            {
                //field: "jobGrade",
                field: "jobGradeTitle",
                // title: "Job Grade",
                headerTemplate: $translate.instant('JOBGRADE_MENU'),
                width: "100px",
            },
        ]
    }

    $scope.changeCheck = async function (dataItem) {
        if ($scope.allCheck) {
            $scope.allCheck = false;
        }
        $scope.userGrid.forEach(item => {
            if (item.id == dataItem.id) {
                if (item.isCheck) {
                    item.isCheck = false;
                    var arrayTemporary = $scope.arrayCheck.filter(x => x.id != item.id);
                    $scope.arrayCheck = arrayTemporary;
                }
                else {
                    item.isCheck = true;
                    $scope.arrayCheck.push(item);
                }
            }
        });
        // if ($scope.arrayCheck.length == $scope.total) {
        //     $scope.allCheck = true;
        // }

        if ($scope.keyWorkTemporary) {
            let arrayCheckTemporary = [];
            let grid = $("#userGrid").data("kendoGrid");
            pageSize = grid.pager.dataSource._pageSize;
            if (pageSize < $scope.total) {
                await getSapCodeByDivison(1, 10000, $scope.keyWorkTemporary);
            }
            $scope.employeesDataSource.forEach(x => {
                let result = $scope.arrayCheck.find(y => y.id == x.id);
                if (result) {
                    arrayCheckTemporary.push(result);
                }
            });

            if (arrayCheckTemporary.length == $scope.total) {
                $timeout(function () {
                    $scope.allCheck = true;
                }, 0);
            }
        }
        else {
            if ($scope.arrayCheck.length == $scope.total) {
                $scope.allCheck = true;
            }
        }

    }

    $scope.userGridReview = [];

    $scope.userListGridReviewOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 10,
            schema: {
                total: () => { return $scope.userGridReview.length },
                data: () => { return $scope.userGridReview }
            }
        },
        sortable: false,
        autoBind: true,
        resizable: true,
        pageable: {
            // pageSize: appSetting.pageSizeWindow,
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 30, 40],
            responsive: false,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [
            {
                field: "sapCode",
                // title: "SAP Code",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "100px",
            },
            {
                field: "fullName",
                // title: "Full Name",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "department",
                // title: "Department",
                headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                width: "200px",
            },
            {
                field: "position",
                // title: "Position",
                headerTemplate: $translate.instant('COMMON_POSITION'),
                width: "200px",
            },
            {
                field: "jobGrade",
                // title: "Job Grade",
                headerTemplate: $translate.instant('JOBGRADE_MENU'),
                width: "100px",
            },
        ],
    }

    $scope.keyWorkTemporary = '';
    $scope.allCheck = false;
    $scope.searchGridUser = async function () {
        $scope.userGrid = [];
        //$scope.allCheck = false;
        $scope.keyWorkTemporary = $scope.gridUser.keyword;
        let grid = $("#userGrid").data("kendoGrid");
        page = grid.pager.dataSource._page;
        pageSize = grid.pager.dataSource._pageSize;
        if ($scope.gridUser.keyword != null) {
            let result = {};
            if ($scope.form.dept_Division) {
                result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.form.dept_Division, limit: pageSize, page: 1, searchText: $scope.gridUser.keyword }).$promise;
            } else {
                result = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: $scope.form.dept_Line, textSearch: $scope.gridUser.keyword ? $scope.gridUser.keyword.trim() : "" }).$promise;
            }
            if (result.isSuccess) {
                let dataFilter = result.object.data.map(function (item) {
                    return { ...item, showtextCode: item.sapCode }
                });
                $scope.total = result.object.count;
                let count = 0;
                let countCheck = 0;
                dataFilter.forEach(item => {
                    item['no'] = count;
                    item['isCheck'] = false;
                    if ($scope.arrayCheck.length > 0) {
                        var result = $scope.arrayCheck.find(x => x.id == item.id);
                        if (result) {
                            item.isCheck = true;
                            countCheck++;
                        }
                    }
                    $scope.userGrid.push(item);
                    count++;
                });
                if ($scope.total) {
                    if (countCheck == $scope.total) {
                        $scope.allCheck = true;
                    }
                    else {
                        if ($scope.arrayCheck.length == $scope.total && countCheck == $scope.userGrid.length) {
                            $scope.allCheck = true;
                        }
                        else {
                            $scope.allCheck = false;
                        }
                    }
                } else {
                    $scope.allCheck = false;
                }
                setGridUser(dataFilter, '#userGrid', $scope.total, 1, $scope.limitDefaultGrid);
            }
        }
        else {
            let count = 0;
            let countCheck = 0;
            $scope.userGrid.forEach(item => {
                item['no'] = count;
                item['isCheck'] = false;
                if ($scope.arrayCheck.length > 0) {
                    var result = $scope.arrayCheck.find(x => x.id == item.id);
                    if (result) {
                        item.isCheck = true;
                        countCheck++;
                    }
                }
                count++;
            });
            if (countCheck == $scope.limitDefaultGrid) {
                $scope.allCheck = true;
            }
            setGridUser($scope.userGrid, '#userGrid', $scope.total, 1, $scope.limitDefaultGrid);
        }
        //code moi
        // let grid = $('#userGrid').data("kendoGrid");
        // grid.dataSource.fetch(() => grid.dataSource.page(1));
        //
        // let grid = $('#userGrid').data("kendoGrid");
        // grid.dataSource.fetch(() => grid.dataSource.page(1));
        // grid.dataSource.total($scope.total);
    }

    $scope.searchGridUserReview = function () {
        let grid = $("#userGridReview").data("kendoGrid");
        pageSize = grid.pager.dataSource._pageSize;
        if ($scope.gridUserReview.keyword) {
            let dataTemporary = _.cloneDeep($scope.userGridReview);
            let dataFilter = dataTemporary.filter(item => {
                if (item.fullName.toLowerCase().includes($scope.gridUserReview.keyword.toLowerCase())) {
                    return item;
                }
            });
            setGridUser(dataFilter, '#userGridReview', dataFilter.length, 1, pageSize);
        }
        else {
            setGridUser($scope.userGridReview, '#userGridReview', $scope.userGridReview.length, 1, pageSize)
        }
    }
    $scope.ifEnter = function ($event, dialog) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            if (dialog == 'review') {
                $scope.searchGridUserReview();
            } else {
                $scope.searchGridUser();
            }
        }
    }

    function setGridUser(data, idGrid, total, pageIndex, pageSizes) {
        let grid = $(idGrid).data("kendoGrid");
        if (idGrid == '#userGrid') {
            let dataSource = new kendo.data.DataSource({
                transport: {
                    read: async function (e) {
                        await getSAPCode(e);
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
        if (idGrid == '#userGridReview') {
            let dataSource = new kendo.data.DataSource({
                data: data,
                pageSize: pageSizes,
                page: 1,
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
    }

    $scope.onChange = async function (isCheckAll) {
        let count = 1;
        // await getSapCodeByDivison(1, 10000, $scope.gridUser.keyword);
        await getSapCodeByDivison(1, 10000, $scope.keyWorkTemporary);
        if (isCheckAll) {
            $scope.employeesDataSource.forEach(item => {
                item['no'] = count;
                item['isCheck'] = true;
                count++;
                //add vô arrayCheck
                var result = $scope.arrayCheck.find(x => x.id == item.id);
                if (!result) {
                    $scope.arrayCheck.push(item);
                }
            });

            $scope.userGrid.forEach(item => {
                item.isCheck = true;
            });
            $scope.allCheck = true;
        }
        else {
            $scope.employeesDataSource.forEach(item => {
                item.isCheck = false;
            });
            $scope.userGrid.forEach(item => {
                item.isCheck = false;
            });
            let arrayTemporary = [];
            $scope.arrayCheck.forEach(item => {
                var result = $scope.employeesDataSource.find(x => x.id == item.id);
                if (!result) {
                    arrayTemporary.push(item);
                }
            });
            $scope.allCheck = false;
            $scope.arrayCheck = arrayTemporary;

        }

        let grid = $("#userGrid").data("kendoGrid");
        page = grid.pager.dataSource._page;
        pageSize = grid.pager.dataSource._pageSize;
        //grid.dataSource.fetch(() => grid.dataSource.page(page));
        setGridUser($scope.userGrid, '#userGrid', $scope.total, page, pageSizeChange);
    }

    $scope.arrayCheck = [];
    $scope.addItemsUser = async function (model) {
        var currentDate = new Date();
        $scope.localtimezone = -1 * currentDate.getTimezoneOffset();

        $scope.keyWorkTemporary = '';
        $scope.review.errorMessage = '';
        $scope.limitDefaultGrid = 20;
        $scope.userGrid = [];
        $scope.arrayCheck = [];
        $scope.userGridReview = [];
        $scope.allCheck = false;
        $scope.isShow = true;
        let count = 0;
        $scope.gridUser.keyword = '';
        $scope.employeesDataSource = [];
        let grid = $('#userGrid').data("kendoGrid");
        grid.dataSource.data($scope.employeesDataSource);
        setDefaultDateItem();
        // grid.dataSource.page(1);
        $scope.searchGridUser();
        // set title cho cái dialog
        let dialog = $("#dialog_Detail").data("kendoDialog");
        dialog.title($translate.instant('COMMON_BUTTON_ADDUSER'));
        dialog.open();
        $rootScope.confirmDialogAddItemsUser = dialog;
    }

    $scope.backPopupReview = function () {
        let dialog = $("#dialog_Detail_Review").data("kendoDialog");
        dialog.close();
        $scope.closeReviewDialog(true);
        // let grid = $("#userGrid").data("kendoGrid");
        // page = grid.pager.dataSource._page;
        // pageSize = grid.pager.dataSource._pageSize;
        // grid.dataSource.fetch(() => grid.dataSource.page(1));
    }
    $scope.reviewUser = function () {
        $scope.userGridReview = [];
        let dialog_Detail = $("#dialog_Detail").data("kendoDialog");
        dialog_Detail.close();
        $scope.arrayCheck.forEach(item => {
            if (item.isCheck) {
                $scope.userGridReview.push(item);
            }
        });
        setGridUser($scope.userGridReview, '#userGridReview', $scope.userGridReview.length, 1, $scope.limitDefaultGrid);
        // set title cho cái dialog
        let dialog = $("#dialog_Detail_Review").data("kendoDialog");
        //dialog.title($translate.instant('REVIEW_USER_TITLE_DIALOG'));
        //reset title 
        dialog.title("");
        //set title mới
        angular.element('.k-dialog-title').append($compile(`
        <div class = "row">
            <div class="d-flex align-items-center">
                <div class="col-md-4" style="margin-top: -25px;">
                     <i class="k-icon k-i-arrow-chevron-left" style="margin-left: -10px;" ng-click="backPopupReview()"></i>
                     <span style="color: #006EFA !important;font-family: Nova-Regular !important;font-size: 18px !important;font-weight: 700; margin: 10px 0;margin: 0;=line-height: 1.42857143;=text-transform: uppercase !important;">${$translate.instant('COMMON_BUTTON_REVIEWUSER')}</span>
                </div>
            </div>
        </div>
        `)($scope)
        )
        dialog.open();
    }

    $scope.arrayERD = [];
    $scope.arrayDOFL = [];
    $scope.warning = [];
    $scope.checkAllowSendShiftExchange = async function () {
        let returnValue = true;
        let shiftExchangeItemsData = $("#shiftExchangeEditListGrid").data("kendoGrid").dataSource.data();
        if (!$.isEmptyObject($scope.model) && !$.isEmptyObject(shiftExchangeItemsData)) {
            var totalCount = shiftExchangeItemsData.length;
            let checkingResults = new Array();
            for (var i = 0; i < totalCount; i++) {
                let currentShiftExchangeItem = shiftExchangeItemsData[i];
                var userSAPCode = currentShiftExchangeItem.employeeCode;

                let shiftCode = currentShiftExchangeItem.currentShift;
                let cCheckingResults = await checkShiftSetByDate(userSAPCode, shiftCode, currentShiftExchangeItem.exchangeDate);
                if (!$.isEmptyObject(cCheckingResults)) {
                    checkingResults = checkingResults.concat(cCheckingResults);
                }
            }

            let errors = [];
            $(checkingResults).filter(function (index, cItem) {
                if (!$.isEmptyObject(cItem) && !cItem.status) {
                    /*returnValue = false;
                    //Show error message
                    errors.push({
                        controlName: "",
                        errorDetail: $translate.instant('SHIFT_EXCHANGE_NOT_ABLE_SEND_SHIFT_CODE_CHANGED', { ExchangeDate: moment(cItem.date).format('DD/MMM/YYYY'), ShiftCode: cItem.currentShiftCode, SAPCode: cItem.userSapCode })

                    });*/
                }
            });

            $scope.errors = errors;
            $scope.$apply();
        }
        return returnValue;
    }
    $scope.checkAllowRevoke = async function () {
        let returnValue = true;
        let error = [];
        $scope.form.exchangingShiftItems.forEach(item => {
            error = error.concat(validationRequiredForTable(item, requiredFieldsForTable));
            let errorDateRange = validateDateRange($rootScope.salaryDayConfiguration, item.exchangeDate, null, item.no, 'SHIFT_EXCHANGE_MENU', $translate, 'SHIFT_EXCHANGE_PERIOD_VALIDATE_1', 'SHIFT_EXCHANGE_PERIOD_VALIDATE_2');
            if (errorDateRange.length) {
                error.push({
                    controlName: $translate.instant('COMMON_ROW_NO') + ' ' + item.no,
                    errorDetail: errorDateRange[0].message

                });
            }

        });
        if (error.length > 0) {
            returnValue = false;
            $scope.errors = error;
            return returnValue;
        }
        else {
            if (!$.isEmptyObject($scope.model) && !$.isEmptyObject($scope.model.shiftExchangeItemsData)) {

                var totalCount = $scope.model.shiftExchangeItemsData.length;
                let checkingResults = new Array();
                for (var i = 0; i < totalCount; i++) {
                    let currentShiftExchangeItem = $scope.model.shiftExchangeItemsData[i];
                    var userSAPCode = currentShiftExchangeItem.employeeCode;

                    let shiftCode = currentShiftExchangeItem.newShiftCode;
                    let cCheckingResults = await checkShiftSetByDate(userSAPCode, shiftCode, currentShiftExchangeItem.exchangeDate);
                    if (!$.isEmptyObject(cCheckingResults)) {
                        checkingResults = checkingResults.concat(cCheckingResults);
                    }
                }

                let errors = [];
                $(checkingResults).filter(function (index, cItem) {
                    if (!$.isEmptyObject(cItem) && !cItem.status) {
                        returnValue = false;
                        //Show error message
                        errors.push({
                            controlName: "",
                            errorDetail: $translate.instant('SHIFT_EXCHANGE_NOT_ABLE_REVOKE_SHIFT_CODE_CHANGED', { ExchangeDate: moment(cItem.date).format('DD/MMM/YYYY'), ShiftCode: cItem.currentShiftCode })

                        });
                    }
                });

                $scope.errors = errors;
                $scope.$apply();
            }

            return returnValue;
        }
    }

    async function checkShiftSetByDate(userSapCode, shiftCode, checkDate) {
        let returnValue = new Array();
        try {
            let currentShiftSet = await getShiftSetByDate(userSapCode, checkDate);
            //In case, current Shift set is null and shiftCode also empty
            // No need to check, return true

            if (currentShiftSet.length == 0 && shiftCode == '') {
                return returnValue;
            }
            else {
                if (currentShiftSet.filter(x => x.code == shiftCode).length == 0) {
                    returnValue.push({
                        "status": false,
                        "shiftSet": currentShiftSet,
                        "currentShiftCode": shiftCode,
                        "date": checkDate,
                        "userSapCode": userSapCode
                    });
                }
            }
        } catch (e) {

        }
        return returnValue;
    }
    // function validateQuantityNewShift() {
    //     let errors = [];
    //     var groupBySAPCode = _.groupBy($scope.form.exchangingShiftItems, 'employeeCode');
    //     Object.keys(groupBySAPCode).forEach(key => {
    //         let arrayERD = [];
    //         let arrayDOFL = [];
    //         var groupByNewShift = _.groupBy(groupBySAPCode[key], 'newShift');
    //         Object.keys(groupByNewShift).forEach(x => {
    //             if (x.includes("ERD")) {
    //                 groupByNewShift[x].forEach(item => {
    //                     arrayERD.push(item);
    //                 });
    //             }
    //         });
    //         var resultERD = checkERDNewShift(arrayERD, groupBySAPCode[key][0].quotaERD);
    //         if (resultERD) {
    //             let text = key + ' ' + $translate.instant('SHIFT_EXCHANGE_MANAGEMENT_SHIFT_QUOTA_VALIDATE');
    //             let textError = {
    //                 groupName: $translate.instant('COMMON_ENTER_FIELD'),
    //                 message: `<span class="bold">${text}</span>`
    //             }
    //             errors.push(textError);
    //         }
    //     });
    //     if (errors.length) {
    //         errors = _.groupBy(errors, 'groupName');
    //     }
    //     return errors;

    // }
    function warningERD() {
        let warningMessages = [];
        var groupBySAPCode = _.groupBy($scope.form.exchangingShiftItems, 'employeeCode');
        Object.keys(groupBySAPCode).forEach(key => {
            var group = groupBySAPCode[key][0];
            if (group.quotaERD < 0) {
                warningMessages.push(`${key}: ${$translate.instant('SHIFT_EXCHANGE_QUOTA_ERD_IS_NAGATIVE')}`);
            } else {
                // let erd1Quality = _.filter(groupBySAPCode[key], x => { return x.newShift == 'ERD1' });
                // let erd2Quality = _.filter(groupBySAPCode[key], x => { return x.newShift == 'ERD2' });
                // let erdQuality = _.filter(groupBySAPCode[key], x => { return x.newShift == 'ERD' });
                // let sumERD = erd1Quality.length / 2 + erd2Quality.length / 2 + erdQuality.length;
                // if (group.quotaERD < sumERD) {
                //     warningMessages.push(`${key}: ${$translate.instant('SHIFT_EXCHANGE_QUOTA_ERD_IS_NAGATIVE')}`);
                // }
            }
            //console.log(groupByNewShift);
            // Object.keys(groupByNewShift).forEach(x => {
            //     array.push(item);
            // });
            // var resultERD = checkERDNewShift(array, groupBySAPCode[key][0].quotaERD);
            // if (resultERD) {
            //     let text = key + ' ' + $translate.instant('SHIFT_EXCHANGE_MANAGEMENT_SHIFT_QUOTA_VALIDATE');
            //     let textError = {
            //         groupName: $translate.instant('COMMON_ENTER_FIELD'),
            //         message: `<span class="bold">${text}</span>`
            //     }
            //     errors.push(textError);
            // }
            //console.log(key);
        });
        return warningMessages;
    }

    function checkERDNewShift(array, maxQuotaERD) {
        let result = false;
        //let total = 0;
        // if (array && array.length) {
        //     // array.forEach(item => {
        //     //     let value = parseValue(item.newShift);
        //     //     total += value;
        //     // });
        //     // if (total > maxQuotaERD) {
        //     //     result = true;
        //     // }
        //     if (maxQuotaERD < 1) {
        //         return true;
        //     }
        // }
        if (maxQuotaERD < 1) {
            return true;
        }
        return result;
    }

    function checkDOFLNewShift(array) {
        let result = false;
        let total = 0;
        array.forEach(item => {
            let value = parseValue(item.newShift);
            total += value;
        });
        if (total > $scope.quotaDOFL) {
            result = true;
        }
        return result;
    }

    function parseValue(item) {
        let result = 0;
        switch (item) {
            case 'ERD':
                result = 1;
                break;
            case 'ERD1':
                result = 0.5;
                break;
            case 'ERD2':
                result = 0.5;
                break;
            // case 'DOFL':
            //     result = 1;
            //     break;
            // case 'DOH1':
            //     result = 0.5;
            //     break;
            // case 'DOH2':
            //     result = 0.5;
            //     break;
            default:
                break;
        }
        return result;
    }


    pageSizeChange = 20;
    async function getSAPCode(option) {
        $scope.userGrid = [];
        $scope.limitDefaultGrid = option.data.take;
        let countCheck = 0;
        // await getSapCodeByDivison(option.data.page, option.data.take, $scope.gridUser.keyword);
        //code moi 
        if ($scope.keyWorkTemporary) {
            await getSapCodeByDivison(1, 10000, $scope.keyWorkTemporary);
            $scope.employeesDataSource.forEach(item => {
                if ($scope.arrayCheck.length > 0) {
                    var result = $scope.arrayCheck.find(x => x.id == item.id);
                    if (result) {
                        item.isCheck = true;
                        countCheck++;
                    }
                }
            });
        }
        //
		var dropDownTree = $("#deptDivision").data("kendoDropDownTree");
		if(dropDownTree){
			var divisionValue = dropDownTree.value();
			if(divisionValue && divisionValue != "" && $scope.form.dept_Division == ""){
				$scope.form.dept_Division = divisionValue;
			}
		}
        if ($scope.form.dept_Division) {
            await getSapCodeByDivison(option.data.page, option.data.take, $scope.keyWorkTemporary);
        } else if ($scope.form.dept_Line && !$scope.form.dept_Division) {
            await getUserCheckedHeadCount($scope.form.dept_Line, $scope.keyWorkTemporary);
        }
        let grid = $("#userGrid").data("kendoGrid");
        pageSizeChange = grid.pager.dataSource._pageSize;
        if (grid) {
            let count = 1;
            //let countCheck = 0;
            // if ($scope.allCheck) {
            if ($scope.allCheck && $scope.arrayCheck.length === $scope.total) {
                $scope.employeesDataSource.forEach(item => {
                    item['no'] = count;
                    item['isCheck'] = true;
                    $scope.userGrid.push(item);
                    count++;
                    //add vô arrayCheck
                    var result = $scope.arrayCheck.find(x => x.id == item.id);
                    if (!result) {
                        $scope.arrayCheck.push(item);
                    }
                });
                countCheck = $scope.arrayCheck.length;
            }
            else {

                $scope.employeesDataSource.forEach(item => {
                    item['no'] = count;
                    item['isCheck'] = false;
                    if ($scope.arrayCheck.length > 0) {
                        var result = $scope.arrayCheck.find(x => x.id == item.id);
                        if (result) {
                            item.isCheck = true;
                            //countCheck++;
                        }
                    }
                    $scope.userGrid.push(item);
                    count++;
                });
            }
            //code moi
            if ($scope.total <= $scope.arrayCheck.length) {
                if (countCheck == $scope.total && countCheck > 0) {
                    $scope.allCheck = true;
                }
                else {
                    if ($scope.arrayCheck.length == $scope.total && countCheck == $scope.arrayCheck.length && $scope.arrayCheck.length > 0) {
                        $scope.allCheck = true;
                    }
                    else {
                        $scope.allCheck = false;
                    }
                }
            } else {
                $scope.allCheck = false;
            }
            //
        }

        option.success($scope.userGrid);
        grid.refresh();

    }

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.applySearch();
        }
    });
    $rootScope.$watch(function () { return dataService.permission }, function (newValue, oldValue) {

        if ($stateParams.id) {
            $scope.perm = (2 & dataService.permission.right) == 2;
        } else {
            $scope.perm = true;
        }

    }, true);
    $scope.$watch('form.dept_Division', function (newValue, oldValue) {
        const firstValue = $scope.originalDivision;
        if (firstValue) {
            $scope.temporaryDivision = firstValue;
        }
        else if (oldValue) {
            $scope.temporaryDivision = oldValue;
        }
    });

    function removeDuplicates(array) {
        let uniq = {};
        return array.filter(obj => !uniq[obj] && (uniq[obj] = true))
    }

    async function validateQuantityNewShiftInServer() {
        let array = []
        listModel = [];
        //
        var groupBySAPCode = _.groupBy($scope.form.exchangingShiftItems, 'employeeCode');
        Object.keys(groupBySAPCode).forEach(key => {
            let model = {
                sapCode: key,
                quotaERD: groupBySAPCode[key][0].quotaERD,
                sumNewShiftERD: 0
            };
            //
            let arrayERD = [];
            var groupByNewShift = _.groupBy(groupBySAPCode[key], 'newShift');
            Object.keys(groupByNewShift).forEach(x => {
                if (x.includes("ERD")) {
                    groupByNewShift[x].forEach(item => {
                        arrayERD.push(item);
                    });
                }
            });
            if (arrayERD.length) {
                arrayERD.forEach(item => {
                    let value = parseValue(item.newShift);
                    model.sumNewShiftERD += value;
                });
                listModel.push(model);
            }
        });
        //
        var result = await cbService.getInstance().shiftExchange.validateERDShiftExchangeDetail(listModel).$promise;
        if (result.isSuccess) {
            if (result.object && result.object.data) {
                //server trả về List<string> SAPCode
                array = result.object.data
            }
        }
        return array;
    }

    function translateValidateERD(arraySAPCode) {
        let errors = [];
        arraySAPCode.forEach(item => {
            let text = item + ' ' + $translate.instant('SHIFT_EXCHANGE_MANAGEMENT_SHIFT_QUOTA_VALIDATE');
            let textError = {
                groupName: $translate.instant('COMMON_ENTER_FIELD'),
                message: `<span class="bold">${text}</span>`
            }
            errors.push(textError);
        });


        if (errors.length) {
            errors = _.groupBy(errors, 'groupName');
        }
        return errors;
    }
});