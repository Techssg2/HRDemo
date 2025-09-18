var ssgApp = angular.module('ssg.settingTrackingLogModule', ["kendo.directives"]);
ssgApp.controller('settingTrackingLogController', function ($rootScope, $q, $scope, $timeout, appSetting, $stateParams, $state, moment, commonData, Notification, settingService, $q, attachmentFile, ssgexService, fileService) {
    // create a message to display in our view
    var ssg = this;
    $scope.value = 0;
    var currentAction = null;
    isItem = true;
    $scope.title = $stateParams.action.title;

    STATUS_SUCCESS = 'SUCCESS';
    STATUS_FAIL = 'FAIL';
    ActionExposeAPI = commonData.actionExposeAPI;
    allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.functionTypesOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "value",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        height: "250px",
        filter: "contains",
        dataSource: {
            data: [
                {
                    name: "Leave Balance",
                    value: "LeaveBalanceSet"
                }, {
                    name: "Missing TimeClock",
                    value: "MissingTimeclockSet"
                }, {
                    name: "Overtime",
                    value: "OverTimeSet"
                }, {
                    name: "Shift Exchange",
                    value: "ShiftExchangeDataSet"
                }, {
                    name: "Resignation",
                    value: "ResignationSet"
                }, {
                    name: "Add Employee",
                    value: "EmployeeSet"
                },
                {
                    name: "Target Plan",
                    value: "TargetPlansSet"
                },
                {
                    name: "Promote and Transfer",
                    value: "Promote_TransferSet"
                },
                {
                    name: "Acting",
                    value: "ActingSet"
                },
            ]
        }
    };
    $scope.statusOptions = {
        placeholder: "",
        dataTextField: "statusName",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: {
            data: [{
                statusName: "Success",
                id: STATUS_SUCCESS
            },
            {
                statusName: "Fail",
                id: STATUS_FAIL
            }
            ]
        },
    }

    $scope.actionOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "value",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: {
            data: [{
                name: "Send",
                value: ActionExposeAPI.Send
            },
            {
                name: "Receive",
                value: ActionExposeAPI.Receive
            }
            ]
        },
    }


    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
    };

    TYPE_MASTERDATA = 'MISSING_TIMECLOCK_REASON_TYPE';

    // phần code dành cho myrequest và all request
    $scope.data = [];
    //Search
    $scope.searchInfo = {
        keyword: '',
        statusId: '',
        fromDate: null,
        toDate: null,
        action: null,
        departmentCode: ''
    };

    // code back-up

    $scope.allOrMyRequestGridOptions = {
        dataSource: {
            data: $scope.data,
            pageSize: appSetting.pageSizeDefault,
            serverPaging: true,
            transport: {
                read: async function (e) {
                    await getListTrackingLog(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.data }
            }
        },
        sortable: false,
        // pageable: true,
        autoBind: true,
        valuePrimitive: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray
        },
        columns: [{
            field: "no",
            title: 'No.',
            width: "50px"
        },
        {
            field: "referenceNumber",
            title: "Reference Number",
            width: "200px",
            template: "#= referenceNumber #"
        },
        {
            field: "payload",
            title: 'Payload',
            width: "120px",
            template: function (dataItem) {
                return "<div  style='height:50px;word-wrap: break-word;' kendo-tooltip k-content=\"'{{dataItem.payload}}'\">{{dataItem.payload}}</div>"
            },
        },
        {
            field: "response",
            title: "Response",
            width: "250px",
            template: function (dataItem) {
                return "<div style='height:300px; word-wrap: break-word;'>{{dataItem.response}}</div>"
            }
        },
        {
            field: "userName",
            title: "UserName",
            width: "150px",
            template: function (dataItem) {
                return "<div kendo-tooltip k-content=\"'{{dataItem.userName}}'\">{{dataItem.userName}}</div>"
            }
        },
        {
            field: "status",
            title: "Status",
            width: "120px",
        },
        {
            field: "created",
            title: "Created Date",
            width: "120px",
            template: function (dataItem) {
                return moment(dataItem.created).format('DD/MM/YYYY HH:mm');
            }
        },
        {
            field: "modified",
            title: "Modified Date",
            width: "120px",
            template: function (dataItem) {
                return moment(dataItem.modified).format('DD/MM/YYYY HH:mm');
            }
        },
        {
            field: "httpStatusCode",
            title: "HttpStatusCode",
            width: "120px",
        },
        {
            field: "actionDescription",
            title: "Action Type",
            width: "120px",
        },
        {
            width: "140px",
            title: "Actions",
            template: function (dataItem) {
                if (dataItem.status && dataItem.status !== 'Success' && dataItem.status !== 'Succes' && dataItem.status.toLowerCase() !== 'success') {
                    return `<a class="btn btn-sm btn-primary" ng-click="retry(dataItem.id)"><i class="fa fa-send right-5"></i>Retry</a>`
                } else {
                    if (dataItem.status && (dataItem.status === 'Success' || dataItem.status === 'Succes' || dataItem.status.toLowerCase() === 'success')) {
                        return `<a class="btn btn-sm btn-primary" ng-click="retry(dataItem.id)"><i class="fa fa-send right-5"></i>Resend</a>`
                    }
                    else {
                        return '';
                    }
                }
            },
        }
        ],
        // page: function (e) {
        //     getList(false, e.page);
        // }
    };
    var tempLimit = appSetting.pageSizeDefault;
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        Order: "Created asc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    }

    async function getListTrackingLog(option) {
        let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);

        if (option) {
            $scope.currentQuery.Limit = option.data.take;
            $scope.currentQuery.Page = option.data.page;
            tempLimit = option.data.take;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;

        let res = await settingService.getInstance().trackingRequest.getTrackingRequest($scope.currentQuery).$promise;
        if (res.isSuccess) {
            let items = res.object.data;
            $scope.allTrackingLogs = items;
            $scope.totalItems = res.object.count;
            var count = (($scope.currentQuery.Page - 1) * $scope.currentQuery.Limit) + 1;
            if (option && option.data.page > 1) {
                // $scope.data = res.object.data.map((item, index) => {
                //     return {
                //         ...item,
                //         no: index + option.data.take + 1
                //     }
                // });
                res.object.data.forEach(item => {
                    item['no'] = count;
                    count++;
                });
                $scope.data = res.object.data;
            } else {
                // $scope.data = res.object.data.map((item, index) => {
                //     return {
                //         ...item,
                //         no: index + 1
                //     }
                // });
                res.object.data.forEach(item => {
                    item['no'] = count;
                    count++;
                });
                $scope.data = res.object.data;
            }
            $scope.total = res.object.count;
            if (option) {
                option.success($scope.data);
            } else {
                var grid = $("#allOrMyRequestGrid").data("kendoGrid");
                grid.dataSource.read();
            }
        }

        //end
    }

    function buildArgs(pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        let conditions = ['url.contains(@{i})', 'referenceNumber.contains(@{i})', 'userName.contains(@{i})'];

        if ($scope.searchInfo.statusId) {
            if ($scope.searchInfo.statusId === STATUS_SUCCESS) {
                predicate.push('status = @{i}');
                predicateParameters.push(STATUS_SUCCESS);
            } else {
                predicate.push('status != @{i}');
                predicateParameters.push(STATUS_SUCCESS);
            }
        }
        if ($scope.searchInfo.action || _.isNumber($scope.searchInfo.action)) {
            predicate.push('action = @{i}');
            predicateParameters.push($scope.searchInfo.action);
        }
        createPredicateKeyWord(predicate, predicateParameters, conditions, $scope.searchInfo.keyword);
        createPredicateFromDate(predicate, predicateParameters, $scope.searchInfo.fromDate);
        createPredicateToDate(predicate, predicateParameters, $scope.searchInfo.toDate);
        createPredicateTrackingDate(predicate, predicateParameters, $scope.searchInfo.fromTrackingDate, $scope.searchInfo.toTrackingDate);
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        if ($scope.searchInfo.departmentCode) {
            option.predicate = option.predicate ? option.predicate + ` and (deptCode = @${option.predicateParameters.length} or divisionCode = @${option.predicateParameters.length + 1})` : `(deptCode = @${option.predicateParameters.length} or divisionCode = @${option.predicateParameters.length + 1})`;
            option.predicateParameters.push($scope.searchInfo.departmentCode);
            option.predicateParameters.push($scope.searchInfo.departmentCode);
        }
        let functionPredicate = [];
        let resultPredicate = '';
        if ($scope.searchInfo.functionTypes) {
            $scope.searchInfo.functionTypes.forEach(x => {
                functionPredicate.push(`url.EndsWith(@${option.predicateParameters.length})`);
                option.predicateParameters.push(x);
            });
            resultPredicate = resultPredicate ? (resultPredicate + ' or ' + `(${functionPredicate.join(' or ')})`) : functionPredicate.join(' or ');

            if (option.predicate && resultPredicate) {
                option.predicate = `(${option.predicate}) and (${resultPredicate})`;
            } else if (option.predicate) {
                option.predicate = option.predicate;
            } else if (resultPredicate) {
                option.predicate = resultPredicate;
            }
        }

        if ($scope.searchInfo.payload) {
            option.predicate = option.predicate ? option.predicate + ` and payload.contains(@${option.predicateParameters.length})` : `payload.contains(@${option.predicateParameters.length})`;
            option.predicateParameters.push($scope.searchInfo.payload);
        }
        return option;
    }

    $scope.search = async function (reset) {
        let grid = $("#allOrMyRequestGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
        //await getListTrackingLog();
    }

    $scope.export = async function () {
        let option = $scope.currentQuery;
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.TRACKINGLOG }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.$on('$locationChangeStart', function (event, next, current) {
        $scope.errors = [];
    });
    $scope.retry = async function (id) {
        var res = await ssgexService.getInstance().trackingLogs.retry({ id: id }, null).$promise;
        if (res.isSuccess) {
            Notification.success('Sent successfully');
        }
        //await ssgexService.getInstance().sapData.submitData({ id: '138360eb-d19c-48e4-aa49-78379b62c468' }).$promise;
        await getListTrackingLog(false, 1);
    }
    var isSuccess = false;
    $scope.retryAll = async function () {
        isSuccess = false;
        //Khiem - fix 381
        var pageCount = 0;
        if ($scope.currentQuery.predicate === "" && $scope.currentQuery.predicateParameters.length == 0) {
            $scope.currentQuery.predicate = 'status != @0 and status != @1';
            $scope.currentQuery.predicateParameters.push('Success');
            $scope.currentQuery.predicateParameters.push('Succes');
        }

        let firstRun = await settingService.getInstance().trackingRequest.getTrackingRequest($scope.currentQuery).$promise;
        if (firstRun.isSuccess) {
            pageCount = Math.ceil(firstRun.object.count / 100);
        }

        if (pageCount == 0) {
            Notification.error('Not found item to send');
        }

        for (var i = 0; i < pageCount; i++) {
            let defers = [];
            var resultArray = [];
            $scope.currentQuery.limit = 100;
            $scope.currentQuery.page = i + 1;

            let res = await settingService.getInstance().trackingRequest.getTrackingRequest($scope.currentQuery).$promise;
            if (res.isSuccess) {
                //let items = res.object.data;
                //resultArray = resultArray.concat(items);
                resultArray = res.object.data;
            }

            if (resultArray.length > 0) {
                $scope.allTrackingLogs = resultArray;
            }
            let unSuccessItems = null;
            if ($scope.currentQuery.predicate === "" && $scope.currentQuery.predicateParameters.length == 0) {
                unSuccessItems = $scope.allTrackingLogs;
            }
            else {
                unSuccessItems = _.filter($scope.allTrackingLogs, x => { return x.status != 'Success' && x.status != 'Succes' });
            }
            if (unSuccessItems && unSuccessItems.length) {
                let n = 0;
                unSuccessItems.forEach(item => {
                    defers.push($q.defer());
                    ssgexService.getInstance().trackingLogs.retry({ id: item.id }, null).$promise.then(result => {
                        defers[n].resolve('success');
                        n++;
                    });
                });
                var all = $q.all([defers[0].promise, defers[1].promise]);
                all.then(allSuccess);
            } else {
                Notification.error('Not found item to send');
            }
        }
        if (isSuccess) {
            Notification.success('Sent successfully');
            await getListTrackingLog(false, 1);
        }

    }
    async function allSuccess() {
        isSuccess = true;
        //Notification.success('Sent successfully');
        //await getList(false, 1);
        $scope.currentQuery.limit = tempLimit;
        //await getListTrackingLog(false, 1);
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
        loadOnDemand: true,
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

    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
        if (value) {
            $timeout(function () {
                setDataDepartment(allDepartments);
            }, 100)

        }
    }

    $scope.reset = async function () {
        $scope.searchInfo = {
            keyword: '',
            statusId: '',
            fromDate: null,
            toDate: null,
            action: null,
            fromTrackingDate: null,
            toTrackingDate: null,
            departmentCode: ''
        };
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            Order: "Created asc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        };
        let grid = $("#allOrMyRequestGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
        //await getListTrackingLog();
        clearSearchTextOnDropdownTree('departmentId');
        $timeout(function () {
            setDataDepartment(allDepartments);
        }, 100);
    }

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });
});