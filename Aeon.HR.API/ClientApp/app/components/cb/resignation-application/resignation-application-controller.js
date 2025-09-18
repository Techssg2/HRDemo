var ssgApp = angular.module("ssg.resignationApplicationModule", [
    "kendo.directives"
]);
ssgApp.controller("resignationApplicationController", function (
    $rootScope,
    $scope,
    $location,
    $stateParams,
    appSetting,
    commonData,
    Notification,
    cbService,
    settingService,
    masterDataService,
    $timeout,
    workflowService,
    fileService,
    $state,
    $translate,
    $window
) {
    // create a message to display in our view
    var ssg = this;
    $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
    $scope.isHRAdmin = $rootScope.currentUser && $rootScope.currentUser.isHRAdmin;
    $scope.advancedSearchMode = false;
    $scope.isRequestor = false;
    $scope.DateOfBirth = new Date();
    $rootScope.isParentMenu = false;
    $scope.refValue = $stateParams.referenceValue;
    //$scope.title = $stateParams.id ? "RESIGNATION APPLICATION: " + $stateParams.referenceValue : $stateParams.action.title;
    $scope.titleHeader = 'RESIGNATION MANAGEMENT'
    $scope.title = $stateParams.id ? /*$translate.instant('RESIGNATION_APPLICATION_EDIT_TITLE') + */$stateParams.referenceValue : $state.current.name == 'home.resignationApplication.item' ? /*$translate.instant('RESIGNATION_APPLICATION_NEW_TITLE')*/'' : $stateParams.action.title;



    var mode = $stateParams.id ? 'Edit' : 'New';
    $scope.mode = mode;
    var allDepartments = sessionStorage.getItemWithSafe("departments") ? JSON.parse(sessionStorage.getItemWithSafe("departments")) : '';
    stateItem = "home.resignationApplication.item";
    $scope.workflowInstances = [];
    $scope.trackingHistory = [];
    $scope.selectedTab = 0;
    $scope.processingStageRound = 0;
    $('#mySelect').on('change', function () {
        var selectedValue = $(this).val();
        if (selectedValue === "1") {
            $scope.selectedTab = 1;
            $state.go('home.resignationApplication.myRequests')

        } else if (selectedValue === "0") {
            $state.go('home.resignationApplication.allRequests')
        }
    });

    //search
    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
        if (value) {
            $timeout(function () {
                setDataDepartment(allDepartments);
            }, 0);
        }
    }
    $scope.openSearch = function () {
        $timeout(function () {
            setDataDepartment(allDepartments);
        }, 0);
    }
    $scope.isModalVisible = false;

 
    $scope.toggleModal = function (show) {
        $scope.isModalVisible = show;
    };
    //$scope.openANewRequest = function () {
    //    $state.go('home.resignationApplication.item')
    //}
    $scope.workLocationNameSearch = "";

    function loadPageOne() {
        let grid = $("#gridRequests").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }

    }

    $scope.search = function (value) {
        //$scope.advancedSearchMode = value;
        // getAllResignationApplication();
        loadPageOne();
        $scope.toggleModal(false);
        $scope.$apply();
        /*$('#exampleModal').modal('hide');
        $('.modal-backdrop').remove();*/
    };

    $scope.resetSearch = async function () {
        // $scope.advancedSearchMode = true;
        $scope.query = {
            keyword: "",
            departmentCode: "",
            createdDate: "",
            fromDate: null,
            toDate: null,
            workLocationName: []
        };
        $scope.$broadcast('resetToDate', $scope.query.toDate);

        // getAllResignationApplication();
        loadPageOne();
    };

    $scope.submited = false;

    $scope.query = {
        keyword: "",
        departmentId: "",
        createdDate: "",
        fromDate: null,
        toDate: null,
        //workLocationName: ""
    };


    $scope.statusData = [
        { statusName: "Completed", code: 1 },
        { statusName: "Waiting for CMD Checker Approval", code: 2 },
        { statusName: "Waiting for HOD Approval", code: 3 },
        { statusName: "Rejected", code: 4 },
        { statusName: "Draft", code: 5 }
    ];


    //Status Data
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

    $scope.departmentOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
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
            console.log(res);
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

    //LIST
    $scope.dataGridOptions = [];
    //show data in table
    $scope.resignationApplicationGridOptions = {
        dataSource: {
            serverPaging: true,
            data: $scope.dataGridOptions,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    $scope.optionSave = e;
                    await getAllResignationApplication(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataGridOptions }
            }
        },
        sortable: false,
        // pageable: true,
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
            width: "350px",
            locked: true,
            lockable: false,
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
            sortable: false,
            width: "180px",
            template: function (dataItem) {
                // if (
                //     dataItem.status === commonData.StatusMissingTimeLock.WaitingCMD ||
                //     dataItem.status === commonData.StatusMissingTimeLock.WaitingHOD
                // ) {
                //     return `<a ui-sref= home.resignationApplication.approve({referenceValue:'${dataItem.id}'}) ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                // }
                return `<a ui-sref= "${stateItem}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "userSAPCode",
            //title: "SAP Code",
            headerTemplate: $translate.instant('COMMON_SAP_CODE'),
            width: "150px",
            // template: function(dataItem) {
            //     if (
            //         dataItem.status === commonData.StatusMissingTimeLock.WaitingCMD ||
            //         dataItem.status === commonData.StatusMissingTimeLock.WaitingHOD
            //     ) {
            //         return `<a ui-sref= home.resignationApplication.approve({referenceValue:'${dataItem.id}'}) ui-sref-opts="{ reload: true }">${dataItem.userSAPCode}</a>`;
            //     }
            //     return `<a ui-sref= "${stateItem}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.userSAPCode}</a>`;
            // }
        },
        {
            field: "createdByFullName",
            //title: "FULL NAME",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            width: "180px"
        },
        {
            field: "deptName",
            //title: "DEPT/ LINE",
            headerTemplate: $translate.instant('COMMON_DEPT_LINE'),
            width: "350px"
        },
        {
            field: "divisionName",
            //title: "DIVISION/ GROUP",
            headerTemplate: $translate.instant('COMMON_DIVISION_GROUP'),
            width: "350px"
        },
        {
            field: "workLocationName",
            //title: "WORK LOCATION",
            headerTemplate: $translate.instant('COMMON_WORK_LOCATION'),
            width: "250px"
        },
        {
            field: "created",
            //title: "CREATED DATE",
            headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
            width: "150px",
            template: function (dataItem) {
                return moment(dataItem.created).format(appSetting.longDateFormat);
            }
        }
        ],
        // page: function (e) {
        //     getAllResignationApplication(e.page, $scope.userId);
        // }
    };
    //
    //item
    //
    // 
    function getStatusClass(dataItem) {
        let classStatus = "";
        switch (dataItem.status) {
            case commonData.StatusMissingTimeLock.WaitingCMD:
                classStatus = "fa-circle font-yellow-lemon";
                break;
            case commonData.StatusMissingTimeLock.WaitingHOD:
                classStatus = "fa-circle font-yellow-lemon";
                break;
            case commonData.StatusMissingTimeLock.Completed:
                classStatus = "fa-check-circle font-green-jungle";
                break;
            case commonData.StatusMissingTimeLock.Rejected:
                classStatus = "fa-ban font-red";
                break;
            default:
        }
        return classStatus;
    }

    function approve() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("Approve resignation application");
        return true;
    }

    function requestedToChange() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("RequestedToChange resignation application");
        return true;
    }

    function reject() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("Reject resignation application");
        return true;
    }

    // phần khai báo chung
    $scope.actions = {
        approve: approve,
        requestedToChange: requestedToChange,
        reject: reject,
        saveItem: saveItem
    }

    $scope.checkErorr = false;

    function Validation(model) {
        let errors = [];
        $scope.checkErorr = false;
        $scope.properties.forEach(prop => {
            if (prop.required) {
                if (!model[prop.property]) {
                    errors.push(prop);
                    $scope.checkErorr = true;
                }
            }
        });
        if (!model.isAgree) {
            //$scope.errors.erorrIsAgree = 'You have to agree to compensation fot un-noticed day';
            $scope.errors.erorrIsAgree = $translate.instant('RESIGNATION_APPLICATION_AGREE_CHECK_REQUIRED');
            $scope.checkErorr = true;
        }
        return errors;
    }

    $scope.reasonOfResignationOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: $scope.resignationReasons,
        template: (e) => showCustomCoupleLanguage(e),
        valueTemplate: (e) => showCustomCoupleLanguage(e),
        change: function (e) {
            var value = this.value();
        }
    }

    function setDataResignationReason(data) {
        var dropdownlist = $("#resignationReasons").data("kendoDropDownList");
        if (dropdownlist) {
            dropdownlist.setDataSource(data);
        }
    }

    //Location
    $scope.locationsDataSource = {
        dataTextField: 'name',
        dataValueField: 'name',
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            serverPaging: false,
            pageSize: 100,
            transport: {
                read: async function (e) {
                    await getLocations(e);
                }
            },
            schema: {
                data: () => {
                    return $scope.dataLocations
                }
            }
        },
        dataBound: function (e) {
            if (!e.sender.value()) {
                this.select(-1);
            }
        },
    };

    async function getLocations(option) {
        var resLocations = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            Name: "WorkLocation"
        }).$promise;
        if (resLocations.isSuccess) {
            $scope.dataLocations = [];
            resLocations.object.data.forEach(element => {
                $scope.dataLocations.push(element);
            });
        }
        if (option) {
            option.success($scope.dataLocations);
        }
    }

    //$scope.dataLocations = 
    //getLocations();

    $scope.numeric = {
        format: '',
    }

    function findValue(data, list, propertyGet, propertyCompare) {
        var result = list.find(x => x[propertyCompare] === data);
        if (result) {
            return result[propertyGet];
        }
    }

    function findCode(name, list, property) {
        var result = list.find(x => x.name === name);
        if (result) {
            return result[property];
        }
    }

    function getValue(ReferenceNumber, list, property, propertyCompare) {
        let item = list.find(x => x[propertyCompare] === ReferenceNumber);
        if (item) {
            return item[property];
        }
        return "";
    }
    async function save(form) {
        var result = { isSuccess: false };
        $scope.errors = {
            errorOfficalResignation: [],
            erorrIsAgree: ''
        };
        $scope.errors.errorOfficalResignation = Validation($scope.model);
        if (!$scope.errors.errorOfficalResignation.length && $scope.model.id) {
            await validateResignation($scope.model);
        }
        $scope.$applyAsync()
        if ($scope.errors.errorOfficalResignation.length) {
            return result;
        }
		
		var startingDateTimeZone = new Date($scope.model.startingDate);
        var timezoneOffset = startingDateTimeZone.getTimezoneOffset() * -1;
        if (timezoneOffset != $scope.localtimezone) {
            var diff = $scope.localtimezone - timezoneOffset;
            if ($scope.model.officialResignationDate) {
                var tempDate1 = new Date($scope.model.officialResignationDate);
                $scope.model.officialResignationDate.setMinutes(tempDate1.getMinutes() + diff);
                $scope.model.officialResignationDate = moment($scope.model.officialResignationDate).format('YYYY-MM-DD HH:mm:ss') + ' +07:00';
                $scope.model.officialResignationDate = moment($scope.model.officialResignationDate).utc().format('YYYY-MM-DD HH:mm:ss') + ' +00:00';
            }
            if ($scope.model.suggestionForLastWorkingDay) {
                var tempDate2 = new Date($scope.model.suggestionForLastWorkingDay);
                $scope.model.suggestionForLastWorkingDay.setMinutes(tempDate2.getMinutes() + diff);
                $scope.model.suggestionForLastWorkingDay = moment($scope.model.suggestionForLastWorkingDay).format('YYYY-MM-DD HH:mm:ss') + ' +07:00';
                $scope.model.suggestionForLastWorkingDay = moment($scope.model.suggestionForLastWorkingDay).utc().format('YYYY-MM-DD HH:mm:ss') + ' +00:00';
            }
        }
        $scope.localtimezone = timezoneOffset;

        if (!$scope.checkErorr && !$scope.errors.errorOfficalResignation.length) {
            $scope.model.ReasonForActionName = findReasonName($scope.model.reasonForActionCode);
            // HR-932 save dua vao ngay tao, chi can dua vao save luc dau
            if (!$scope.model.isExpiredLaborContractDate && $scope.model.status != 'Requested To Change' && (!$scope.model.id || $scope.model.status == 'Draft')) {
                $scope.onChange($scope.model.contractTypeCode, 'Contract');
            }
            let res = await cbService.getInstance().resignationApplication.saveResignationApplication($scope.model).$promise;
            if (res.isSuccess) {
                Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                $scope.model = _.cloneDeep(res.object);
                $scope.isRequestor = $scope.model.createdById ? $scope.model.createdById == $rootScope.currentUser.id : false;
                $scope.model.suggestionForLastWorkingDay = new Date($scope.model.suggestionForLastWorkingDay);
                $scope.model.officialResignationDate = new Date($scope.model.officialResignationDate);
                $scope.title = $translate.instant('RESIGNATION_APPLICATION_EDIT_TITLE') + res.object.referenceNumber;
                if (!$stateParams.id) {
                    $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
                }
                if (res.object.id) {
                    $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
                }
            } else {
                Notification.success("Error System");
            }
            return res;
        }
        return result;
    }

    async function saveItem() {
        var result = await save();
        return result;
    }

    // function findReasonName(code) { 
    //     var result;
    //     $scope.resignationReasons.forEach(item => {
    //         if (item.code === code) {
    //             result = item.name;
    //             return result;
    //         }
    //     });
    //     return result;
    // }



    function findReasonName(code) {
        var result;
        $scope.resignationReasons.forEach(item => {
            if (item.code === code) {
                result = item.name + '(' + item.nameVN + ')';
                return result;
            }
        });
        return result;
    }


    $scope.resignationReasons = [];

    async function getResignationReasons() {
        $rootScope.showLoading();
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'ActionTypeResignation',
        }).$promise;
        if (res.isSuccess) {
            $scope.resignationReasons = res.object.data;
            $scope.reasonOfResignationOptions.dataSource = $scope.resignationReasons
            if ($stateParams.type === commonData.pageStatus.item) {
                setDataResignationReason($scope.resignationReasons);
            }
        }
        $rootScope.hideLoading();
    }

    async function getResignationApplicantionById(id) {
        $rootScope.showLoading();
        var result = await cbService.getInstance().resignationApplication.getResignationApplicantionById({ id: id }).$promise;
        if (result && result.isSuccess) {
            $timeout(async function () {
                $scope.model = _.cloneDeep(result.object);
                $scope.statusTranslate = $rootScope.getStatusTranslate($scope.model.status);
                $scope.isRequestor = $scope.model.createdById ? ($rootScope.currentUser && $scope.model.createdById == $rootScope.currentUser.id) : false;
                $scope.model.contractTypeCode = parseInt(result.object.contractTypeCode);
                $scope.model.startingDate = result.object.startingDate ? moment.utc(result.object.startingDate).utcOffset(7).format('YYYY-MM-DD HH:mm:ss') : '';
                $scope.model.suggestionForLastWorkingDay = result.object.suggestionForLastWorkingDay ? moment.utc(result.object.suggestionForLastWorkingDay).utcOffset(7).format('YYYY-MM-DD HH:mm:ss') : '';
                $scope.model.officialResignationDate = result.object.officialResignationDate ? moment.utc(result.object.officialResignationDate).utcOffset(7).format('YYYY-MM-DD HH:mm:ss') : '';
                /*if ($scope.model.createdById == $rootScope.currentUser.id) {
                    if ($scope.model.status == 'Draft') {
                        updateOfficialResignationDateByContractType($scope.model.contractTypeCode);
                    } else if ($scope.model.status.toLowerCase() == 'Requested To Change'.toLowerCase()) {
                        let firstSubmittedItemResult = await cbService.getInstance().resignationApplication.getSubmitedFirstDate({ itemId: $stateParams.id }, null).$promise;
                        if (firstSubmittedItemResult && firstSubmittedItemResult.isSuccess && firstSubmittedItemResult.object) {
                            $scope.submittedFirstDate = firstSubmittedItemResult.object.created;
                            updateOfficialResignationDateByContractType($scope.model.contractTypeCode);
                        }
                        $scope.model.suggestionForLastWorkingDay = new Date($scope.model.suggestionForLastWorkingDay);
                    }
                    else {
                        $scope.model.suggestionForLastWorkingDay = new Date($scope.model.suggestionForLastWorkingDay);
                        $scope.model.officialResignationDate = new Date($scope.model.officialResignationDate);
                    }
                }
                else {
                    $scope.model.suggestionForLastWorkingDay = new Date($scope.model.suggestionForLastWorkingDay);
                    $scope.model.officialResignationDate = new Date($scope.model.officialResignationDate);
                }*/
            }, 0);
        }
        $rootScope.hideLoading();
    }

    $scope.user = [];


    $scope.dataGrid = [];

    function buildArgs(pageIndex, pageSize) {
        let conditions = [
            'ReferenceNumber.contains(@{i})',
            'userSAPCode.contains(@{i})',
            'createdByFullName.contains(@{i})',
            'deptName.contains(@{i})',
            'divisionName.contains(@{i})',
            'status.contains(@{i})',
            //'workLocation.contains(@{i})'
        ];
        let stateArgs = {
            currentState: $stateParams.type,
            myRequestState: commonData.pageStatus.myRequests,
            currentUserId: $rootScope.currentUser.id
        };

        return createQueryArgsForCAB({ pageIndex, pageSize, order: appSetting.ORDER_GRID_DEFAULT }, conditions, $scope.query, stateArgs);
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
    async function getAllResignationApplication(option) {

        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            Order: "Created desc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        }


        let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);
        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status);
        }

        if ($scope.query.workLocationName && $scope.query.workLocationName.length) {
            generatePredicateWithWorkLocationName(args, $scope.query.workLocationName);
        }

        if ($scope.query.jobGrade && $scope.query.jobGrade.length) {
            $scope.currentQuery.Object = $scope.query.jobGrade;
        }

        if (option) {
            $scope.currentQuery.Limit = option.data.take;
            $scope.currentQuery.Page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;

        //get limit in grid 
        let grid = $("#gridRequests").data("kendoGrid");
        $scope.currentQuery.Limit = grid.pager.dataSource._pageSize
        //


        var result = await cbService.getInstance().resignationApplication.getAllResignationApplication($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.dataGridOptions = result.object.data;
            $scope.dataGridOptions.forEach(item => {
                item.created = new Date(item.created);
                if (item.mapping) {
                    if (item.mapping.userJobGradeGrade >= 5) {
                        item['userDeptName'] = item.mapping.departmentName;
                    } else {
                        item['userDivisionName'] = item.mapping.departmentName;
                    }
                }
            });
            $scope.total = result.object.count;
        }
        if (option) {
            option.success($scope.dataGridOptions);
        } else {
            // $scope.model.overtimeApplicationGridOptions.dataSource.read();
            initGridRequests($scope.dataGridOptions, $scope.total, $scope.currentQuery.Page, $scope.currentQuery.Limit);
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


    $scope.onChange = function (data, type) {
        switch (type) {
            case 'Shuibook':
                var code = findValue(data, commonData.shuiBooks, 'code', 'name');
                $scope.model.SHUIBookCode = code;
                break;
            case 'Contract':
                var name = findValue(parseInt(data), commonData.contracts, 'nameVN', 'code');
                var day = findValue(parseInt(data), commonData.contracts, 'day', 'code');
                var result = addDays(new Date(), day);
                if ($scope.submittedFirstDate) {
                    result = addDays($scope.submittedFirstDate, day);
                }
                $scope.model.officialResignationDate = new Date(result);
                $scope.changeOfficialResignationDate();
                $scope.model.ContractTypeName = name;
                let datePicker = $('#official_resignation_date_id').data('kendoDatePicker');
                if ($scope.model.isExpiredLaborContractDate) {
                    if (datePicker) {
                        datePicker.min(new Date(1970, 1, 1));
                    }
                } else {
                    if (datePicker) {
                        datePicker.min(result);
                    }
                }
                break;
            case 'Reason':
                var name = findValue(parseInt(data), $scope.data.reasonOfResignation, 'name', 'code');
                $scope.model.Reason = name;
                break;
            case 'Argee':
                if ($scope.model.isAgree) {
                    $scope.checkisAgree = true;
                } else {
                    $scope.checkisAgree = false;
                }
                break;
            case 'ExpiredLaborContract':
                if ($scope.model.isExpiredLaborContractDate) {
                    let datePicker = $('#official_resignation_date_id').data('kendoDatePicker');
                    if (datePicker) {
                        datePicker.min(new Date(1970, 1, 1));
                    }
                } else {
                    updateOfficialResignationDateByContractType($scope.model.contractTypeCode);
                }
                break;
            default:
                break;
        }
        // $scope.itemForm.officialResignationDate = new Date(moment().add($scope.model.contract, 'days').format('MM/DD/YYYY'));
    }
    $scope.changeOfficialResignationDate = function () {
        var now = new Date();
        if ($scope.model.officialResignationDate) {
            var officialDate = $scope.model.officialResignationDate;
            $scope.model.officialResignationDate = new Date(officialDate.getFullYear(), officialDate.getMonth(), officialDate.getDate(), now.getHours(), now.getMinutes(), now.getSeconds(), now.getMilliseconds());;
            $scope.model.suggestionForLastWorkingDay = dateFns.addDays($scope.model.officialResignationDate, -1);
        }
    }

    function addDays(date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }
    function updateOfficialResignationDateByContractType(data) {
        var name = findValue(parseInt(data), commonData.contracts, 'nameVN', 'code');
        var day = findValue(parseInt(data), commonData.contracts, 'day', 'code');
        var result = addDays(new Date(), day);
        if ($scope.submittedFirstDate) {
            result = addDays($scope.submittedFirstDate, day);
        }
        if (!$scope.model.isExpiredLaborContractDate) {
            if (!dateFns.isAfter(new Date(dateFns.format($scope.model.officialResignationDate, 'MM/DD/YYYY')), new Date(dateFns.format(result, 'MM/DD/YYYY')))) {
                $scope.model.officialResignationDate = new Date(result);
            }
            let datePicker = $('#official_resignation_date_id').data('kendoDatePicker');
            if (datePicker) {
                datePicker.min(result);
            }
        } else {
            $scope.model.officialResignationDate = $scope.model.officialResignationDate ? new Date($scope.model.officialResignationDate) : result;
        }
        $scope.changeOfficialResignationDate();
        $scope.model.ContractTypeName = name;
    }

    //form
    $scope.itemForm = {
        sapCode: '',
        staffFullName: '',
        position: '',
        deptLine: '',
        divisionGroup: '',
        location: '',
        joiningDate: '',
    };

    $scope.userId = '';
    $scope.contracts = commonData.contracts;
    $scope.shuiBooks = commonData.shuiBooks;

    $scope.localtimezone = null;

    async function ngOnit() {
        var currentDate = new Date();
        $scope.localtimezone = -1 * currentDate.getTimezoneOffset();


        $scope.model = {};
        await getJobGradeList();
        if ($rootScope.currentUser) {
            $scope.model = {
                Id: '',
                UserId: $rootScope.currentUser.id,
                officialResignationDate: '',
                UnusedLeaveDate: '',
                SHUIBookCode: 1,
                SHUIBookName: '',
                ReasonForActionCode: '',
                ReasonForActionName: '',
                // isAgree: false,
                SuggestionForLastWorkingDay: '',
                ContractTypeCode: '',
                ContractTypeName: '',
                Status: '',
                referenceNumber: '',
                workLocationCode: $rootScope.currentUser.workLocationCode,
                workLocationName: $rootScope.currentUser.workLocationName,
                departmentCode: $rootScope.currentUser.deptCode ? $rootScope.currentUser.deptCode : $rootScope.currentUser.divisionCode,
                departmentName: $rootScope.currentUser.deptName ? $rootScope.currentUser.deptName : $rootScope.currentUser.divisionName,
                userSAPCode: '',
                createdByFullName: '',
                deptName: '',
                deptCode: '',
                divisionCode: '',
                divisionName: '',
                workLocationCode: '',
                workLocationName: '',
                startingDate: ''
            }
            $scope.checkisAgree = false;
            $scope.model.isAgree = false;
            $scope.errors = {
                errorOfficalResignation: [],
                erorrIsAgree: ''
            };

            $scope.properties = [
                // { property: "officialResignationDate", controlName: "Official Resignation Date", required: true },
                { property: "officialResignationDate", controlName: $translate.instant('RESIGNATION_APPLICATION_OFFICAL_RESINATION_DATE'), required: true },
                // { property: "shuiBookName", controlName: "SHUI Book", required: true },
                { property: "shuiBookName", controlName: $translate.instant('RESIGNATION_APPLICATION_SHUI_BOOK'), required: true },
                // { property: "reasonForActionCode", controlName: "Reason of Resination", required: true },
                { property: "reasonForActionCode", controlName: $translate.instant('RESIGNATION_APPLICATION_REASON'), required: true },
                { property: "contractTypeCode", controlName: $translate.instant('COMMON_CONTRACT'), required: true },
            ]
        }
        if ($state.current.name === commonData.resignationApplication.item || $state.current.name == commonData.resignationApplication.approve) {
            await getResignationReasons();
            if (mode == 'New') {
                $scope.currentInstanceProcessingStage = null;
                $timeout(function () {
                    $scope.model = _.cloneDeep($rootScope.currentUser);
                    $scope.model.id = '';
                    $scope.model.userSAPCode = $rootScope.currentUser.sapCode;
                    $scope.model.createdByFullName = $rootScope.currentUser.fullName;
                    $scope.model.startingDate = $rootScope.currentUser.startDate ? new Date($rootScope.currentUser.startDate) : '';
                    if ($scope.model.startingDate.getTimezoneOffset() * -1 != 420) {
                        $scope.model.startingDate.setMinutes($scope.model.startingDate.getMinutes() + 420 - $scope.model.startingDate.getTimezoneOffset() * -1);
                    }
                    
                    $scope.model.positionName = $rootScope.currentUser.positionName;
                    // $scope.model.deptName = $scope.model.deptName && $scope.model.jobGradeCode ? $scope.model.deptName + "(" + $scope.model.jobGradeCode + ")" : $scope.model.deptName ? $scope.model.deptName : '';
                    // $scope.model.divisionName = $scope.model.divisionName && $scope.model.jobGradeCode ? $scope.model.divisionName + "(" + $scope.model.jobGradeCode + ")" : $scope.model.divisionName ? $scope.model.divisionName : '';
                    $scope.model.deptName = $scope.model.deptName && $scope.model.jobGradeCode ? $scope.model.deptName : $scope.model.deptName ? $scope.model.deptName : '';
                    $scope.model.divisionName = $scope.model.divisionName && $scope.model.jobGradeCode ? $scope.model.divisionName : $scope.model.divisionName ? $scope.model.divisionName : '';
                }, 0);
            } else {
                await getResignationApplicantionById($stateParams.id);
                await getSubmitedFirstDate($stateParams.id);
                await getWorkflowProcessingStage($stateParams.id);
            }
        } else {
            // if ($stateParams.type === commonData.pageStatus.myRequests) {
            //     $scope.userId = $rootScope.currentUser.id;
            //     getAllResignationApplication(1, $scope.userId);
            // }
            // if ($stateParams.type === commonData.pageStatus.allRequests) {
            //     getAllResignationApplication(1, $scope.userId);
            // }
            $timeout(function () {
                setDataDepartment(allDepartments);
            }, 0);
        }
        //Translate cho SHUI book
        $scope.shuiBooks[0].nameValue = $translate.instant('RESIGNATION_APPLICATION_SHUI_EMPLOYEE');
        $scope.shuiBooks[1].nameValue = $translate.instant('RESIGNATION_APPLICATION_SHUI_COMPANY');
        $scope.shuiBooks[2].nameValue = $translate.instant('RESIGNATION_APPLICATION_SHUI_NOTYET');

        setTimeout(function () {
            document.querySelectorAll('.tooltip-custom').forEach((tooltip) => {
                const tooltipText = tooltip.querySelector('.tooltip-text');
                // const parentRect = document.querySelector('.portlet-body').getBoundingClientRect();
				const portletBody = document.querySelector('.portlet-body');
				if (portletBody != null && tooltipText != null) {
					const parentRect = portletBody.getBoundingClientRect();
					tooltip.addEventListener('mouseenter', function () {
						const tooltipRect = tooltipText.getBoundingClientRect();
						if (tooltipRect.right > parentRect.right) {
							tooltip.classList.add('tooltip-left');
						}
					});
				}
            });
        }, 10);
 
    }

    $scope.export = async function () {
        let args = buildArgs(appSetting.numberSheets, appSetting.numberRowPerSheets);
        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status);
        }

        if ($scope.query.workLocationName && $scope.query.workLocationName.length) {
            generatePredicateWithWorkLocationName(args, $scope.query.workLocationName);
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.RESIGNATION
        }, args).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success("Data Successfully Exported");
        } else {
            Notification.error("No Data To Exporting");
        }
    }
    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }
    $scope.showTrackingHistory = function () {
        $rootScope.visibleTrackingHistory($translate, appSetting.TrackingLogDialogDefaultWidth);
    }
    $scope.showHistoriesForRoleHR = function () {
        $rootScope.visibleLogHRAmin($translate, appSetting.TrackingLogDialogDefaultWidth);
    }
    $rootScope.showLoading();
    ngOnit();
    $rootScope.hideLoading();

    $scope.printForm = async function () {
        let res = await cbService.getInstance().resignationApplication.printForm({ id: $stateParams.id }).$promise;
        if (res.isSuccess) {
            exportToPdfFile(res.object);
        }
    }

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search(true);
        }
    });
    $scope.$on("startWorkflow", async function (evt, data) {
        await getResignationApplicantionById(data);
    });

    async function validateResignation(model) {
        let workflowStatusResult = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: model.id }).$promise;
        if (workflowStatusResult.isSuccess && workflowStatusResult && workflowStatusResult.object && workflowStatusResult.object.lastHistory && workflowStatusResult.object.currentStatus) {
            let stepName = Number(workflowStatusResult.object.lastHistory.stepNumber);
            if (stepName == 3 && !model.isNotifiedLastWorkingDate && workflowStatusResult.object.currentStatus != 'Requested To Change') {
                $scope.errors.erorrIsAgree = $translate.instant('HAVE_BEEN_NOTIFIED_REMAIN_LEAVE_OF_EMPLOYEE');
                $scope.checkErorr = true;
            }
        }

        if (!$scope.checkErorr) {
            if ($scope.model.countExitInterview == 0) {
                $scope.errors.erorrIsAgree = $translate.instant('EXIT_INTERVIEW_ERROR_MESSAGE');
                $scope.checkErorr = true;
            }
        }
    }

    $scope.jobGradeOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "value",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: $scope.jobGradeList
    };

    async function getJobGradeList() {
        $scope.jobGradeList = [];
        let result = await settingService.getInstance().headcount.getJobGrades({
            predicate: "",
            predicateParameters: [],
            order: "Grade asc",
            limit: 200,
            page: 1,
        }).$promise;
        if (result.isSuccess) {
            if (result.object.data) {
                result.object.data.forEach(x => {
                    var item = {
                        name: x.caption,
                        value: x.id
                    }
                    $scope.jobGradeList.push(item);
                })
            }
        } else {
            Notification.error(result.messages[0]);
        }
    }

    async function getSubmitedFirstDate(id) {
        let firstSubmittedItemResult = await cbService.getInstance().resignationApplication.getSubmitedFirstDate({ itemId: $stateParams.id }, null).$promise;
        if (firstSubmittedItemResult && firstSubmittedItemResult.isSuccess && firstSubmittedItemResult.object) {
            $scope.submittedFirstDate = firstSubmittedItemResult.object.created;
        }
    }

    // count 
    $scope.openNewWindow = async function (id, itemType) {
        if (itemType) {
            if (itemType === 'ExitInterview') {
                // save thong tin khi click
                let currentRes = await cbService.getInstance().resignationApplication.countExitInterview({ itemId: id }, null).$promise;
                if (currentRes.isSuccess && currentRes.object.countExitInterview) {
                    $scope.model.countExitInterview = currentRes.object.countExitInterview;
                    $window.open(appSetting.ExitInterview, '_blank');
                }
            }
        }
    };

    async function getWorkflowProcessingStage (itemId) {
        var result = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: itemId }).$promise;
        if (result.isSuccess && result.object) {
            $scope.currentInstanceProcessingStage = result.object.workflowInstances[0];
            $scope.processingStageRound = result.object.workflowInstances.length;
            $scope.currentInstanceProcessingStage.workflowData.steps.map((item) => {
                var findHistories = _.find($scope.currentInstanceProcessingStage.histories, x => { return x.stepNumber == item.stepNumber });
                if (findHistories != null) {
                    item.histories = findHistories;
                    return item;
                }
            });
        }
    }
});