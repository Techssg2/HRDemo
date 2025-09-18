var ssgApp = angular.module("ssg.missingTimelockModule", ["kendo.directives"]);
ssgApp.controller("missingTimelockController", function (
    $rootScope,
    $scope,
    $location,
    $stateParams,
    $state,
    appSetting,
    commonData,
    Notification,
    cbService,
    settingService,
    masterDataService,
    workflowService,
    $timeout,
    fileService,
    $translate,
    localStorageService,
    attachmentService,
    attachmentFile
) {
    // create a message to display in our view
    var ssg = this;
    $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
    $scope.advancedSearchMode = false;
    $scope.DateOfBirth = new Date();
    stateItem = "home.missingTimelock.item";
    $scope.titleHeader = 'MISSING TIMECLOCK';
    //title 
    //$scope.title = $stateParams.id ? "MISSING TIMECLOCK: " + $stateParams.referenceValue : $stateParams.action.title;
    $scope.title = $stateParams.id ? /*$translate.instant('MISSING_TIMECLOCK_EDIT_TITLE') +*/ $stateParams.referenceValue : $state.current.name == 'home.missingTimelock.item' ? /*$translate.instant('MISSING_TIMECLOCK_NEW_TITLE')*/'' : $state.current.name == 'home.missingTimelock.myRequests' ? $translate.instant('MISSING_TIMECLOCK_MY_REQUETS').toUpperCase() : $translate.instant('MISSING_TIMECLOCK_ALL_REQUETS').toUpperCase();
    //edit / new
    $scope.id = $stateParams.referenceValue;
    //search
    $scope.toggleFilterPanel = async function (value) {
        $scope.advancedSearchMode = value;
        if (value) {
            if (!$scope.query.departmentCode || $scope.query.departmentCode == '') {
                $timeout(function () {
                    setDataDepartment(allDepartments);
                }, 0);
            }
        }
    };

    $scope.query = {
        keyword: "",
        departmentId: "",
        fromDate: null,
        toDate: null,
        missingDateFrom: null,
        missingDateTo: null
    };
    var mode = $stateParams.id ? 'Edit' : 'New';
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.workflowInstances = [];
    $scope.processingStageRound = 0;

    $scope.onTabChange = function () {
        localStorage.setItem('selectedTab', $scope.selectedTab);
        if ($scope.selectedTab === "1") {
            $state.go('home.missingTimelock.myRequests');
        } else if ($scope.selectedTab === "0") {
            $state.go('home.missingTimelock.allRequests');
        }
    };

    function loadPageOne() {
        let grid = $("#gridRequests").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }

    }


    $scope.search = function (reset) {
        // getAllMissingTimelock();
        loadPageOne();
        $scope.toggleFilterPanel(false);
    }

    $scope.clearSearch = function () {
        $scope.query = {
            keyword: "",
            departmentCode: "",
            fromDate: null,
            toDate: null
        };
        $scope.$broadcast('resetToDate', $scope.query.toDate);
        // getAllMissingTimelock();
        loadPageOne();
    }


    $scope.SAPCode = "";
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

    $scope.statusData = [{
        name: "Completed",
        value: 1
    },
    {
        name: "Waiting for CMD Checker Approval",
        value: 2
    },
    {
        name: "Waiting for HOD Approval",
        value: 3
    },
    {
        name: "Cancelled",
        value: 4
    },
    {
        name: "Draft",
        value: 5
    }
    ];
    $scope.data = [];
    $scope.user = [];

    async function getUserById(userid) {
        var model = {
            userId: userid
        }

        if (userid) {
            var result = await settingService.getInstance().users.getUserById(model).$promise;
            if (result.isSuccess) {
                $scope.user = result.object;
                $scope.missingTimelockForm = {
                    sapCode: $scope.user.sapCode,
                    deptLine: $scope.user.deptName,
                    staffFullName: $scope.user.fullName,
                    divisionGroup: $scope.user.divisionName
                };

            }
        }

    }
    //

    //Department

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
    async function getData(option) {
        await getAllMissingTimelock(option.data.page, '', option.data.take)
    }
    $scope.total = 0;
    //LIST
    //show data in table
    $scope.missingTimelockGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    // await getAllOvertime(e.data.page, '', e.data.take)
                    await getAllMissingTimelock(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.data }
            }
        },
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
            sortable: false,
            width: "350px",
            locked: true,
            template: function (dataItem) {
                var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                // return `<workflow-status status="${dataItem.status}"></workflow-status>`;
            }
        },
        {
            field: "referenceNumber",
            //title: "Reference Number",
            headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
            sortable: false,
            locked: true,
            width: "180px",
            template: function (dataItem) {
                return `<a ui-sref= "${stateItem}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "userSAPCode",
            //title: "SAP Code",
            headerTemplate: $translate.instant('COMMON_SAP_CODE'),
            sortable: false,
            width: "150px",
        },
        {
            field: "createdByFullName",
            //title: "Full Name",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            sortable: false,
            width: "200px"
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
            field: "created",
            //title: "Created Date",
            headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
            sortable: false,
            width: "200px",
            template: function (dataItem) {
                return moment(dataItem.created).format(appSetting.longDateFormat);
            }
        }
        ]
    };

    $scope.missingTimelockId = '';
    //Model
    $scope.model = {
        listReason: []
    };

    $scope.openANewRequest = function () {
        $state.go('home.missingTimelock.item')
    }

    /*function dateToJSONWithLocalTimeZone(date) {
        // Lấy các thành phần của ngày và giờ
        var year = date.getFullYear();
        var month = ('0' + (date.getMonth() + 1)).slice(-2); // Thêm số 0 vào trước nếu cần
        var day = ('0' + date.getDate()).slice(-2); // Thêm số 0 vào trước nếu cần
        var hours = ('0' + date.getHours()).slice(-2); // Thêm số 0 vào trước nếu cần
        var minutes = ('0' + date.getMinutes()).slice(-2); // Thêm số 0 vào trước nếu cần
        var seconds = ('0' + date.getSeconds()).slice(-2); // Thêm số 0 vào trước nếu cần

        // Lấy chênh lệch múi giờ tính bằng phút
        var tzOffsetMinutes = date.getTimezoneOffset();
        var tzOffsetHours = Math.abs(tzOffsetMinutes / 60);
        var tzSign = tzOffsetMinutes > 0 ? '-' : '+';

        // Tạo chuỗi đại diện cho múi giờ
        var formattedOffset = tzSign + ('0' + tzOffsetHours).slice(-2) + ':' + ('0' + (Math.abs(tzOffsetMinutes) % 60)).slice(-2);

        // Tạo chuỗi JSON với ngày và giờ cùng với múi giờ địa phương
        var formattedDate = year + '-' + month + '-' + day + 'T' + hours + ':' + minutes + ':' + seconds + formattedOffset;

        return formattedDate;
    };*/

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
	
	

    async function saveItem(form, perm) {
        $scope.errors = [];
        var result = { isSuccess: false };
        let grid = $("#grid").data("kendoGrid");
        $scope.model.listReason = grid.dataSource._data;

        var timezoneOffset = null;
        $scope.localtimezone = timezoneOffset;
        // var isValidation = false;       
        // if (!$scope.model || perm == 'undefined' || perm != 1) {
        //     isValidation = true;
        // } else {
        //     isValidation = false;
        // }       
        // if (isValidation) {
        //     $scope.errors = validation();
        // }
        $scope.errors = validation();
        if (!$scope.errors.length) {
            $scope.model.listReason.forEach(item => {
                let otherItem = $scope.reasons.find(x => x.name.includes("Others"));
                if (otherItem && otherItem.code == item.reason) {
                    item.reasonName = '';
                } else {
                    let reasonItem = $scope.reasons.find(x => x.code == item.reason);
                    item.reasonName = reasonItem.name;
                }
                item.previous = (_.find($scope.shiftCodes, x => { return x.code == item.shiftCode })).rawData.Previous;

				var isIpad = deviceIsIpad();
				if (!isIpad) {
					item.date = moment(item.date).format('YYYY-MM-DD HH:mm:ss') + ' +07:00';
					item.date = moment(item.date).utc().format('YYYY-MM-DD HH:mm:ss') + ' +00:00';

					item.actualTime = moment(item.actualTime).format('YYYY-MM-DD HH:mm:ss') + ' +07:00';
					item.actualTime = moment(item.actualTime).utc().format('YYYY-MM-DD HH:mm:ss') + ' +00:00';
				}
				
                /*item.date = dateToJSONWithLocalTimeZone(item.date);
                item.actualTime = dateToJSONWithLocalTimeZone(item.actualTime);*/
            });
            $scope.model.listReason = JSON.stringify($scope.model.listReason);
            if ($scope.attachments.length || $scope.removeFiles.length) {
                let attachmentFilesJson = await mergeAttachment();
                $scope.model.documents = attachmentFilesJson;
            }
            var res = await cbService.getInstance().missingTimelocks.saveMissingTimelock($scope.model).$promise;
            if (res.isSuccess) {
                //Notification.success('Data Successfully Saved');
                Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                $scope.model = _.cloneDeep(res.object);
                try {
                    $scope.oldAttachments = JSON.parse(res.object.documents);
                    $timeout(function () {
                        $scope.attachments = [];
                        $(".k-upload-files.k-reset").find("li").remove();
                    }, 0);
                } catch (e) {
                }
                $scope.model.listReason = JSON.parse($scope.model.listReason);
                if (!$stateParams.id) {
                    $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                }
				if (res.object.id) {
					$state.go($state.current.name, { id: res.object.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
				}
                // $scope.title = $scope.model.id ? "MISSING TIMECLOCK: " + $scope.model.referenceNumber : $stateParams.action.title;
                $scope.title = $scope.model.id ? $translate.instant('MISSING_TIMECLOCK_EDIT_TITLE') + $scope.model.referenceNumber : $stateParams.action.title;
                if ($scope.removeFiles) {
                    await attachmentService.getInstance().attachmentFile.deleteMultiFile($scope.removeFiles).$promise;
                }
            } else {
                Notification.error(res?.messages?.[0] || "Error System");
                if (res?.messages?.[0]) {
                    res.messages = null;
                }
                $timeout(function () {
                    location.reload();
                }, 4000);
            }
            return res;
        } else {
            return result;
        }
    }

    $scope.statusCompleted = false;
    valueReasonCode = '';
    async function getMissingTimelockByReferenceNumber(sapCode) {
        let QueryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: '',
            limit: ''
        };
        let model = {
            queryArgs: QueryArgs,
            type: ''
        }
        QueryArgs.predicateParameters.push(sapCode);
        var result = await cbService.getInstance().missingTimelocks.getMissingTimelockByReferenceNumber(model).$promise;
        if (result.isSuccess) {
            if (result.object !== null) {
                $scope.model = _.cloneDeep(result.object);
                $scope.statusTranslate = $rootScope.getStatusTranslate($scope.model.status);
                $scope.model.moduleTitle = 'Missing Time Lock';
                $scope.model.listReason = JSON.parse(result.object.listReason);
                $scope.model.listReason.forEach(item => {
					const specificDate = new Date(moment.utc(item.actualTime, "YYYY-MM-DD HH:mm:ss"));
					const year = specificDate.getUTCFullYear();   // Lấy năm theo UTC
					const month = specificDate.getUTCMonth();     // Lấy tháng theo UTC (0 - 11)
					const day = specificDate.getUTCDate();        // Lấy ngày theo UTC
					const hours = specificDate.getUTCHours();     // Lấy giờ theo UTC
					const minutes = specificDate.getUTCMinutes(); // Lấy phút theo UTC
					const seconds = specificDate.getUTCSeconds(); // Lấy giây theo UTC
					// const specificDate = new Date(Date.UTC(2025, 2, 11, 1, 2, 3));  // Tháng 2 là tháng 3 vì tháng bắt đầu từ 0
                    const newDate = new Date(Date.UTC(year, month, day, hours, minutes, seconds));

                    item.actualTime = newDate;
					
					const specificDate1 = new Date(moment.utc(item.date, "YYYY-MM-DD HH:mm:ss"));
					const year1 = specificDate1.getUTCFullYear();   // Lấy năm theo UTC
					const month1 = specificDate1.getUTCMonth();     // Lấy tháng theo UTC (0 - 11)
					const day1 = specificDate1.getUTCDate();        // Lấy ngày theo UTC
					const hours1 = specificDate1.getUTCHours();     // Lấy giờ theo UTC
					const minutes1 = specificDate1.getUTCMinutes(); // Lấy phút theo UTC
					const seconds1 = specificDate1.getUTCSeconds(); // Lấy giây theo UTC
					// const specificDate = new Date(Date.UTC(2025, 2, 11, 1, 2, 3));  // Tháng 2 là tháng 3 vì tháng bắt đầu từ 0
                    const newDate1 = new Date(Date.UTC(year1, month1, day1, hours1, minutes1, seconds1));
					
					item.date = newDate1;
                    if (item.actualTime.getTimezoneOffset() * -1 != 420) {
                        //item.actualTime.setMinutes(item.actualTime.getMinutes() + 420 - item.actualTime.getTimezoneOffset() * -1);
                        item.date.setMinutes(item.date.getMinutes() + 420 - item.date.getTimezoneOffset() * -1);
                    }
                    /*var timeString = item.actualTime.split("T")[1];
                    var [hours, minutes] = timeString.split(":");
                    item.actualTime = hours + ':' + (minutes < 10 ? '0' : '') + minutes;*/
                });

                //ngan
                if ($scope.model.listReason.length) {
                    for (var i = 0; i < $scope.model.listReason.length; i++) {
                        if ($scope.model.listReason[i].reason) {
                            let index = _.findIndex($scope.reasons, x => {
                                return x.code == $scope.model.listReason[i].reason;
                            });
                            if (index == -1) {
                                $scope.reasons.push({ code: $scope.model.listReason[i].reason, name: $scope.model.listReason[i].reasonName });
                            }
                            valueReasonCode = $scope.model.listReason[i].reason;
                            setDataDropdownList("#reason_Id", $scope.reasons, valueReasonCode);
                        }
                    }
                }
                //end
                initGridLeason($scope.model.listReason);
                if (result.object.status == 'Completed') {
                    $scope.statusCompleted = true;
                }
                try {
                    $scope.oldAttachments = JSON.parse(result.object.documents);
                } catch (e) {
                    console.log(e);
                }
            }
        }
    }

    function initGridLeason(dataSource) {
        let grid = $("#grid").data("kendoGrid");
        let dataSourceMissingTimelock = new kendo.data.DataSource({
            data: dataSource,
        });
        if (grid) {
            grid.setDataSource(dataSourceMissingTimelock);
        }

    }

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
            currentState: $state.current.name,
            myRequestState: 'home.missingTimelock.myRequests',
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
    async function getAllMissingTimelock(option) {
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

        var result = await cbService.getInstance().missingTimelocks.getAllMissingTimelockByUser($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.data = result.object.data;
            $scope.data.forEach(item => {
                item.created = new Date(item.created);
                /*if (item.mapping) {
                    if (item.mapping.userJobGradeGrade >= 5) {
                        item['userDeptName'] = item.mapping.departmentName;
                    } else {
                        item['userDivisionName'] = item.mapping.departmentName;
                    }
                }*/
            });
            $scope.total = result.object.count;
        }
        if (option) {
            option.success($scope.data);
        } else {
            initGridRequests($scope.data, $scope.total, $scope.currentQuery.page, $scope.currentQuery.Limit);
        }
    }



    // function getAllMissingTimelock(pageIndex, sapCode, limit = 20) {

    //     let args = buildArgs(pageIndex, appSetting.pageSizeDefault);
    //     if ($scope.searchMissingTimelock.status && $scope.searchMissingTimelock.status.length) {
    //         generatePredicateWithStatus(args, $scope.searchMissingTimelock.status);
    //     }
    //     cbService.getInstance().missingTimelocks.getAllMissingTimelockByUser(args).$promise.then(function(result) {
    //         if (result.isSuccess) {
    //             $scope.data = result.object.data;
    //             $scope.data.forEach(item => {
    //                 item.created = new Date(item.created);
    //                 if (item.mapping) {
    //                     if (item.mapping.userJobGradeGrade >= 5) {
    //                         item['userDeptName'] = item.mapping.departmentName;
    //                     } else {
    //                         item['userDivisionName'] = item.mapping.departmentName;
    //                     }
    //                 }
    //             })
    //             initGridRequests($scope.data, result.object.count, pageIndex);
    //         }
    //     });

    // }

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




    function goBack() {
        Notification.success("goBack");
    }

    // function requestedToChange() {
    //   Notification.success("requestedToChange");
    // }

    // function requestedToChange() {
    //   Notification.success("requestedToChange");
    // }

    // function approve() {
    //   Notification.success("approve");
    // }

    // function reject() {
    //   Notification.success("reject");
    // }

    function approve() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("Approve missing timeclock");
        return true;
    }

    function requestedToChange() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("RequestedToChange missing timeclock");
        return true;
    }

    function reject() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("Reject missing timeclock");
        return true;
    }

    function isAction(actions) {
        //  user sẽ có một danh sách permission
        // kiểm tra xem permission đó có nằm trong mảng gửi vào hay không
        if (actions.some(x => x === $scope.action)) {
            return true;
        }
        return false;
    }


    $scope.properties = [{
        property: "userSAPCode",
        controlName: "SAP Code",
        required: true
    },
        // {
        //   property: "deptLine",
        //   controlName: "Dept Line",
        //   required: true
        // },
        // {
        //   property: "staffFullName",
        //   controlName: "staff Full Name",
        //   required: true
        // },
        // {
        //   property: "divisionGroup",
        //   controlName: "Division Group",
        //   required: true
        // }
    ];

    requiredFieldsForTable = [
        {
            fieldName: 'date',
            //title: "Date"
            title: $translate.instant('MISSING_TIMECLOCK_DATE_REQUIRED'),
        },
        {
            fieldName: 'typeActualTime',
            // title: "Type"
            title: $translate.instant('MISSING_TIMECLOCK_TYPE_REQUIRED'),
        },
        {
            fieldName: 'shiftCode',
            // title: "Shift Code"
            title: $translate.instant('MISSING_TIMECLOCK_SHIFT_CODE_REQUIRED'),
        },
        {
            fieldName: 'actualTime',
            // title: "Actual Time",
            title: $translate.instant('MISSING_TIMECLOCK_ACTUAL_TIME_REQUIRED'),
        },
        {
            fieldName: 'reason',
            // title: "Reason"
            title: $translate.instant('MISSING_TIMECLOCK_REASON_REQUIRED'),
        }
    ];

    $scope.errors = [];
    $scope.temporyDataGrid = [];

    function validationForTable(model, requiredFields) {
        let errors = [];
        requiredFields.forEach(field => {
            if (!model[field.fieldName]) {
                errors.push({
                    controlName: `${field.title} ${model.id}`
                });
            }
        });
        return errors;
    }

    function validataDuplicateReasons(model) {
        let errors = [];
        model.forEach(x => {
            x['date'] = moment(x.date).format('DD/MM/YYYY');
        });
        let groupbyDate = _.groupBy(model, 'date');
        Object.keys(groupbyDate).forEach(key => {
            let groupbyShiftCode = _.groupBy(groupbyDate[key], function (x) {
                return x.shiftCode + '-' + x.typeActualTime;
            });
            Object.keys(groupbyShiftCode).forEach(x => {
                if (groupbyShiftCode[x].length > 1) {
                    if (key == 'Invalid date') {
                        errors.push({
                            // controlName: ` Shift Code in a day is not duplicate: ${key}`,
                            controlName: $translate.instant('MISSING_TIMECLOCK_DUPLICATE_VALIDATE'),
                            isRule: true
                        });
                    }
                    else {
                        errors.push({
                            // controlName: ` Shift Code in a day is not duplicate: ${key}`,
                            controlName: $translate.instant('MISSING_TIMECLOCK_DUPLICATE_VALIDATE') + `: ${key}`,
                            isRule: true
                        });
                    }
                }
            });
        });
        return errors;
    }



    $scope.listReason = []; // fix ticket 134 
    function validation() {
        CheckIsStore($scope);
        let errors = [];
        if ($scope.model.listReason.length === 0) {
            // fix ticket 134 - start
            // errors.push({ controlName: `Table Missing Timeclock` });
            errors.push({ controlName: $translate.instant('MISSING_TIMECLOCK_TABLE') });
            // $scope.listReason.push(`List reason not invalid`)
            // fix ticket 134 - end
        } else {
            $scope.model.listReason.forEach(item => {
                // kiểm tra xem có đi ền other reason hay không nếu chọn lý do khác
                if (item.reason === commonData.reasons[4].code) {
                    if (!item.others) {
                        errors.push({
                            // controlName: `Other Reason of No ${item.id}`
                            controlName: $translate.instant('MISSING_TIMECLOCK_OTHER_REASON_REQUIRED') + ` ${item.id}`
                        });
                    }
                }
                // kiếm tra xem các field trong bảng đã có dữ liệu chưa
                errors = errors.concat(validationForTable(item, requiredFieldsForTable));
                //let error = validateDateRange(appSetting.rangeDate, item.date, null, item.id, 'Missing Timeclock', $translate);
                let error = validateDateRange($rootScope.salaryDayConfiguration, item.date, null, item.id, 'MISSING_TIMECLOCK_MENU', $translate, 'MISSING_TIMECLOCK_PERIOD_VALIDATE_1', 'MISSING_TIMECLOCK_PERIOD_VALIDATE_2');

                if (error.length) {
                    errors.push({ controlName: error[0].controlName, message: error[0].message, isRule: true });
                }

            });

            let grid = $("#grid").data("kendoGrid");
            $scope.model.listReason = grid.dataSource._data;
            $scope.temporyDataGrid = _.cloneDeep($scope.model.listReason);
            if (!errors.length) {
                errors = validateSalaryRangeDate($translate, $scope.temporyDataGrid, appSetting.salaryRangeDate, 'MISSING_TIMECLOCK_MENU', 'MISSING_TIMECLOCK_PERIOD_VALIDATE_2', false);
                if (errors.length) {
                    errors = errors.map(function (item) { return { ...item, isRule: true } });
                }
            }
            errors = errors.concat(validataDuplicateReasons($scope.temporyDataGrid));
        }
        return errors;
    }

    //quyen sumbit or approval
    $scope.ActionForPromoteAndTransfer = commonData.ActionForPromoteAndTransfer;
    $scope.action = [commonData.ActionForPromoteAndTransfer.Approve];

    //data Init


    $scope.shiftCodes = [];

    async function getShiftCodes() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'ShiftCode',
        }).$promise;
        if (res.isSuccess) {
            let shiftCodes = _.filter(res.object.data, x => {
                return x.code != 'PRD';
            });
            $scope.shiftCodes = shiftCodes.map(function (item) {
                return { ...item, showtext: item.code }
            });
        }
    }

    $scope.reasons = [];
    async function getReasons() {
        var res = await settingService.getInstance().cabs.getAllReason({
            nameType: commonData.reasonType.MISSING_TIMECLOCK_REASON
        }).$promise;
        //ngan
        if (res.isSuccess) {
            res.object.data.forEach(element => {
                $scope.reasons.push(element);
            });
        }
        //end
    }

    $scope.timeEditor = function (container, options) {
        $(`<input required name="${options.field}"/>`)
            .appendTo(container)
            .kendoTimePicker({
                format: "HH:mm",
                // value: kendo.toString(options.model[options.field], "HH:mm"),
                value: new Date().toLocaleTimeString("en-GB").slice(0, -3),
                dateInput: true,
                change: function (e) {
                    var value = this.value();
                    return value.toLocaleTimeString("en-GB").slice(0, -3);
                }
            });
    };

    $scope.dateEditor = function (container, options) {
        $(`<input required name="${options.field}" />`)
            .appendTo(container)
            .kendoDatePicker({
                format: "dd-MM-yyyy",
                value: kendo.toString(options.model[options.field], "dd-MM-yyyy")
            });
    };

    function reasonDropDownEditor(container, options) {
        $(`<input required name="${options.field}" />`)
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "name",
                dataValueField: "code",
                template: '<span><label>#: data.name# </label></span>',
                valueTemplate: '#: name #',
                filter: "contains",
                dataSource: $scope.reasons,
            });
    }

    $scope.templateOther = function (container, options) {
        let result = $scope.reasons.find(x => x.name.includes("Others"));
        if (options.model.reason.code) {
            if (options.model.reason.code === result.code) {
                var input = $(
                    `<input class="k-input" type="text" ng-model="options.model.others" style="width: 100%"/>`
                );
                input.attr("name", options.field);
                input.appendTo(container);
            } else {
                options.model.others = "";
                $(`<label></label>`).appendTo(container);
            }
        } else {
            if (result && options.model.reason === result.code) {
                var input = $(
                    `<input class="k-input" type="text" ng-model="options.model.others" style="width: 100%"/>`
                );
                input.attr("name", options.field);
                input.appendTo(container);
            } else {
                options.model.others = "";
                $(`<label></label>`).appendTo(container);
            }
        }
    };

    function shiftDataDownEditor(container, options) {
        $(`<input id="dropdownlist" required name="${options.field}"/>`)
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "code",
                dataValueField: "code",
                template: '<span><label>#: data.code# - #: data.name# </label></span>',
                valueTemplate: '#: code #',
                filter: "contains",
                dataSource: $scope.shiftCodes,
                customFilterFields: ['code', 'name'],
                filtering: filterMultiField
            });
    }

    function findValue(code, list, property) {
        var result = list.find(x => x.code === code);
        if (result) {
            return result[property];
        }
        return "";
    }

    function addNewRecord() {
        let defaultTime = null;
        defaultTime && defaultTime.setHours(0);
        defaultTime && defaultTime.setMinutes(0);
        let nValue = {
            id: $scope.model.listReason ? $scope.model.listReason.length + 1 : 1,
            date: defaultTime,
            typeActualTime: '',
            shiftCode: '',
            actualTime: defaultTime,
            reason: '',
            others: ""
        };
        let grid = $("#grid").data("kendoGrid");
        $scope.model.listReason = grid.dataSource._data;
        $scope.model.listReason.push(nValue);
        let dataSourceReasonOthers = new kendo.data.DataSource({
            data: $scope.model.listReason,
        });
        grid.setDataSource(dataSourceReasonOthers);
    }

    confirm = function (e) {
        if (e.data && e.data.value) {
            let grid = $("#grid").data("kendoGrid");
            $scope.model.listReason = grid.dataSource._data;
            let resetId = 1;
            $scope.model.listReason.splice($scope.no - 1, 1);
            $scope.model.listReason.forEach(item => {
                item.id = resetId;
                resetId += 1;
            });
            initGridLeason($scope.model.listReason);
            // Notification.success("Data Sucessfully Deleted"); 
            Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
        }
    }

    $scope.deleteRecord = function (no) {
        $scope.no = no;
        $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
        $scope.dialog.bind("close", confirm);
    };

    $scope.onchangeShiftCode = function (e, dataItem) {
        $scope.actualTimeIn = '';
        $scope.actualTimeOut = '';
        let rawData = e.sender.dataItem().rawData;
        //GET TIME FROM SHIFTCODE
        let dateObjIn = new Date();
        let dateObjOut = new Date();
        dateObjIn.setHours(rawData.Sobeg.substring(0, 2), rawData.Sobeg.substring(2, 4));
        dateObjOut.setHours(rawData.Soend.substring(0, 2), rawData.Soend.substring(2, 4));
        //in
        if (dataItem.typeActualTime == $scope.dataTypeActualTime[0].value) {
            dataItem.actualTime = dateObjIn;
        }
        //out
        else {
            dataItem.actualTime = dateObjOut
        }
        refreshGrid();
    }

    $scope.onchangeType = function (e, dataItem) {
        dataItem.shiftCode = '';
        dataItem.actualTime = '';
        refreshGrid();
    }
    $scope.notAcceptInput = function (e) {
        e.preventDefault()
    }

    $scope.checkTypeActualTime = true;
    $scope.dataTypeActualTime = commonData.typeActualTime //in = 1; out =2
    let defaultTime = null;
    defaultTime && defaultTime.setHours(0);
    defaultTime && defaultTime.setMinutes(0);
    $scope.model.listReason = new kendo.data.ObservableArray([
        {
            id: $scope.model.listReason ? $scope.model.listReason.length + 1 : 1,
            date: defaultTime,
            typeActualTime: '',
            shiftCode: '',
            actualTime: defaultTime,
            reason: '',
            others: ""
        }
    ]);

    $scope.reasonDetailGridOptions = {
        dataSource: {
            data: $scope.model.listReason
        },
        sortable: false,
        editable: false,
        scrollable: true,
        columns: [{
            field: "date",
            //title: "Date",
            headerTemplate: $translate.instant('MISSING_TIMECLOCK_DATE'),
            width: "100px",
            //editor: $scope.dateEditor,
            format: "{0: dd/MM/yyyy}",
            template: function (dataItem) {
                return `<input kendo-date-picker
                        k-date-input="true"
                        k-ng-model="dataItem.date"
                        k-format="'dd/MM/yyyy'"
                        style="width: 100%;" />`;
            }
        },
        {
            field: "typeActualTime",
            //title: "Type",
            headerTemplate: $translate.instant('MISSING_TIMECLOCK_TYPE'),
            width: "90px",
            //editor: $scope.dateEditor,
            format: "{0: dd/MM/yyyy}",
            template: function (dataItem) {
                return `<select kendo-drop-down-list style="width: 100%;" id="typeActualTimeId"
                    data-k-ng-model="dataItem.typeActualTime"
                    k-data-text-field="'name'"
                    k-data-value-field="'value'"
                    k-template="'<span><label>#: data.name# </label></span>'",
                    k-valueTemplate="'#: value #'",
                    k-auto-bind="'true'"
                    k-value-primitive="'false'"
                    k-data-source="dataTypeActualTime"
                    k-on-change="onchangeType(kendoEvent, dataItem)"
                    > </select>`;
            }
        },
        {
            field: "shiftCode",
            //title: "Shift Code",
            headerTemplate: $translate.instant('MISSING_TIMECLOCK_SHIFT_CODE'),
            width: "180px",
            //editor: shiftDataDownEditor,
            template: function (dataItem) {
                if (dataItem.typeActualTime) {
                    return `<select kendo-drop-down-list style="width: 100%;"
                        data-k-ng-model="dataItem.shiftCode"
                        k-data-text-field="'showtext'"
                        k-data-value-field="'code'"
                        k-template="'<span><label>#: data.code# - #: data.name# (#:data.nameVN#)</label></span>'",
                        k-valueTemplate="'#: code #'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="shiftCodes"
                        k-filter="'contains'",
                        k-customFilterFields="['code', 'name']",
                        k-filtering="'filterMultiField'"
                        k-on-change="onchangeShiftCode(kendoEvent, dataItem)"
                        > </select>`;
                } else {
                   // return `<input readonly class="k-textbox w100" type="text" ng-model="dataItem.shiftCode" style="width: 100%;" />`;
                    return `<select kendo-drop-down-list style="width: 100%;"
                            data-k-ng-model="dataItem.shiftCode"
                            k-data-text-field="'showtext'"
                            k-data-value-field="'code'"
                            k-template="'<span><label>#: data.code# - #: data.name# (#:data.nameVN#)</label></span>'",
                            k-valueTemplate="'#: code #'",
                            k-auto-bind="'true'"
                            k-value-primitive="'false'"
                            k-data-source="shiftCodes"
                            k-filter="'contains'",
                            k-customFilterFields="['code', 'name']",
                            k-filtering="'filterMultiField'"
                            disabled
                            k-on-change="onchangeShiftCode(kendoEvent, dataItem)"
                            > </select>`;
                }

            }
        },
        {
            field: "actualTime",
            //title: "Actual Time",
            headerTemplate: $translate.instant('MISSING_TIMECLOCK_ACTUAL_TIME'),
            width: "100px",
            format: "{0: yyyy-MM-dd HH:mm:ss}",
            // editor: $scope.timeEditor,
            template: function (dataItem) {

                if (dataItem.typeActualTime) {
                    return `<input kendo-time-picker
                        k-ng-model="dataItem.actualTime"
                        k-date-input="true"
                        k-format="'HH:mm'" k-interval="15"                       
                        
                        style="width: 100%;" />`;
                } else {
                    //return `<input readonly class="k-input w100" type="text" />`;
                    return `<input kendo-time-picker
                        k-ng-model="dataItem.actualTime"
                        k-date-input="true"
                        k-format="'HH:mm'" k-interval="15"                       
                        disabled
                        style="width: 100%;" />`;
                }

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
                    k-data-source="reasons"
                    k-filter="'contains'",
                    > </select>`;
                //return findValue(item.reason, $scope.reasons, "name");
            }
        },
        {
            field: "others",
            //title: "Other Reason",
            headerTemplate: $translate.instant('COMMON_OTHER_REASON'),
            width: "200px",
            //editor: $scope.templateOther,
            template: function (dataItem) {
                return `<input name="others" style="width: 200px;" ng-readonly="!isOther(dataItem)" class="k-textbox w100" type="text" ng-model="dataItem.others"/>`;
            }
        },
        {
            //title: "Actions",
            width: "100px",
            headerTemplate: $translate.instant('COMMON_ACTION'),
            headerAttributes: {
                class: "bg-table-head"
            },
            template: function (dataItem) {
                 return `<a class="btn-border-upgrade btn-delete-upgrade" ng-if="!statusCompleted" ng-click="deleteRecord(dataItem.id)"></a>`;
            }
        }
        ],
        selectable: true,
        save: function (e) {
            if (e.values['date']) {
                e.model.date = e.values['date'];
                mapValueGrid('date', e);
            }
            if (e.values['shiftCode']) {
                e.model.shiftCode = e.values['shiftCode'];
                mapValueGrid('shiftCode', e);
            }
            if (e.values['actualTimeIn']) {
                e.model.actualTimeIn = e.values['actualTimeIn'];
                mapValueGrid('actualTimeIn', e);
            }
            if (e.values['actualTimeOut']) {
                e.model.actualTimeOut = e.values['actualTimeOut'];
                mapValueGrid('actualTimeOut', e);
            }
            if (e.values['reason']) {
                e.model.reason = e.values['reason'];
                mapValueGrid('reason', e);
                let result = $scope.reasons.find(
                    x => x.name.includes("Others"));
                if (e.values['reason'] === result.code) {
                    e.model.others = '';
                }
            }
            if (e.values['others']) {
                e.model.others = e.values['others'];
                mapValueGrid('others', e);
            }
            refreshGrid();
        }
    };

    function refreshGrid() {
        let grid = $("#grid").data("kendoGrid");
        grid.refresh();
    }

    function mapValueGrid(name, e) {
        $scope.model.listReason.forEach(x => {
            if (x.id === e.model.id) {
                x[name] = e.model[name];
            }
        });

    }
    $scope.changeReason = function (item) {
        let result = $scope.reasons.find(x => x.name.includes("Others"));
        if (result && result.code !== item.reason) {
            item.others = "";
            item.reasonName = item.name;
            refreshGrid();
        }
        return;
    }
    $scope.isOther = function (item) {
        let result = $scope.reasons.find(x => x.name.includes("Others"));
        if (result && result.code === item.reason) {
            return true;
        }
        item.others = "";
        return false;
    }
    $scope.reasonDetailApproveGridOptions = {
        dataSource: {
            data: $scope.model.listReason
        },
        sortable: false,
        editable: false,
        columns: [{
            field: "date",
            //title: "DATE",
            headerTemplate: $translate.instant('MISSING_TIMECLOCK_DATE'),
            width: "50px",
            editor: $scope.dateEditor,
            format: "{0: dd-MM-yyyy}",
            template: function () {
                return new Date().toLocaleTimeString("en-GB").slice(0, 10);
            }
        },
        {
            field: "shiftCode",
            //title: "SHIFT CODE",
            headerTemplate: $translate.instant('MISSING_TIMECLOCK_SHIFT_CODE'),
            width: "100px",
            editor: shiftDataDownEditor,
            template: function (item) {
                if (item.shiftCode) {
                    if (item.shiftCode.code) {
                        return findValue(
                            item.shiftCode.code,
                            $scope.shiftCodes,
                            "name"
                        );
                    } else {
                        return findValue(item.shiftCode, $scope.shiftCodes, "name");
                    }
                }
            }
        },
        {
            field: "actualTimeIn",
            title: "ACTUAL TIME IN",
            width: "100px",
            format: "{0: yyyy-MM-dd HH:mm:ss}",
            editor: $scope.timeEditor,
            template: function (item) {
                if (item.actualTimeIn) {
                    return item.actualTimeIn.toLocaleTimeString("en-GB").slice(0, -3);
                } else {
                    return new Date().toLocaleTimeString("en-GB").slice(0, -3);
                }
            }
        },
        {
            field: "actualTimeOut",
            title: "ACTUAL TIME OUT ",
            width: "100px",
            format: "{0: yyyy-MM-dd HH:mm:ss}",
            editor: $scope.timeEditor,
            template: function (item) {
                if (item.actualTimeOut) {
                    return item.actualTimeOut.toLocaleTimeString("en-GB").slice(0, -3);
                } else {
                    return new Date().toLocaleTimeString("en-GB").slice(0, -3);
                }
            }
        },
        {
            field: "reason",
            //title: "REASON",
            headerTemplate: $translate.instant('COMMON_REASON'),
            width: "110px",
            editor: reasonDropDownEditor,
            template: function (item) {
                if (item.reason) {
                    if (item.reason.code) {
                        return findValue(item.reason.code, $scope.reasons, "name");
                    } else {
                        return findValue(item.reason, $scope.reasons, "name");
                    }
                }
            }
        },
        {
            field: "others",
            //title: "Reason other",
            headerTemplate: $translate.instant('COMMON_OTHER_REASON'),
            width: "140px"
        }
        ],
    };

    //item Summit or Approval

    $scope.actions = {

        saveItem: saveItem,
        goBack: goBack,
        isAction: isAction,
        requestedToChange: requestedToChange,
        approve: approve,
        reject: reject,
        addNewRecord: addNewRecord
    };

    //Form User
    $scope.missingTimelockForm = {
        sapCode: '',
        deptLine: '',
        staffFullName: '',
        divisionGroup: ''
    };

    $scope.userId = '';

    async function ngOnInit() {
        $rootScope.showLoading();
        if ($state.current.name === 'home.missingTimelock.myRequests') {
            $scope.selectedTab = "1";
        }
        else if ($state.current.name === 'home.missingTimelock.allRequests') {
            $scope.selectedTab = "0";
        }
        if ($state.current.name === 'home.missingTimelock.item' || $state.current.name == 'home.missingTimelock.approve') {
            await getShiftCodes();
            await getReasons();
            if (mode == 'Edit') {
                await getMissingTimelockByReferenceNumber($stateParams.id);
                await getWorkflowProcessingStage($stateParams.id);
            } else {
                $timeout(function () {
                    $scope.model = _.cloneDeep($rootScope.currentUser);
                    $scope.model.id = '';
                    $scope.model.userSAPCode = $rootScope.currentUser.sapCode;
                    $scope.model.createdByFullName = $rootScope.currentUser.fullName;
                    $scope.model.startingDate = $rootScope.currentUser.startDate ? $rootScope.currentUser.startDate : null;
                    // $scope.model.deptName = $scope.model.deptName && $scope.model.jobGradeCode ? $scope.model.deptName + "(" + $scope.model.jobGradeCode + ")" : $scope.model.deptName ? $scope.model.deptName : '';
                    // $scope.model.divisionName = $scope.model.divisionName && $scope.model.jobGradeCode ? $scope.model.divisionName + "(" + $scope.model.jobGradeCode + ")" : $scope.model.divisionName ? $scope.model.divisionName : '';
                    $scope.model.deptName = $scope.model.deptName && $scope.model.jobGradeCode ? $scope.model.deptName : $scope.model.deptName ? $scope.model.deptName : '';
                    $scope.model.divisionName = $scope.model.divisionName && $scope.model.jobGradeCode ? $scope.model.divisionName : $scope.model.divisionName ? $scope.model.divisionName : '';

                }, 0);
                $scope.missingTimelockForm = {
                    sapCode: $rootScope.currentUser.sapCode,
                    deptLine: $rootScope.currentUser.deptName,
                    staffFullName: $rootScope.currentUser.fullName,
                    divisionGroup: $rootScope.currentUser.divisionName
                };
            }
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
        $rootScope.hideLoading();
    }
    $scope.export = async function () {
        let args = buildArgs(appSetting.numberSheets, appSetting.numberRowPerSheets);
        if ($scope.query.status && $scope.query.status.length) {
            generatePredicateWithStatus(args, $scope.query.status);
        }
        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.MISSINGTIMECLOCK
        }, args).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }

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

    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }
    $scope.showTrackingHistory = function () {
        $rootScope.visibleTrackingHistory($translate, appSetting.TrackingLogDialogDefaultWidth);
    }
    ngOnInit();

    $scope.attachments = [];
    $scope.oldAttachments = [];
    $scope.removeFiles = [];
    $scope.onSelect = function (e) {
        let message = $.map(e.files, function (file) {
            $scope.attachments.push(file);
            return file.name;
        }).join(", ");
    };

    $scope.removeAttach = function (e) {
        let file = e.files[0];
        $scope.attachments = $scope.attachments.filter(item => item.name !== file.name);
    }

    $scope.removeOldAttachment = function (model) {
        $scope.oldAttachments = $scope.oldAttachments.filter(item => item.id !== model.id);
        $scope.removeFiles.push(model.id);
    }

    $scope.downloadAttachment = async function (id) {
        let result = await attachmentFile.downloadAndSaveFile({
            id
        });
    }

    $scope.checkAllowRevoke = async function () {
        let returnValue = true;
        $scope.errors = validation();
        if ($scope.errors.length > 0) {
            returnValue = false;
        }
        return returnValue;
    }

    async function uploadAction() {
        var payload = new FormData();
        $scope.attachments.forEach(item => {
            payload.append('file', item.rawFile, item.name);
        });
        let result = await attachmentService.getInstance().attachmentFile.upload(payload).$promise;
        return result;
    }

    async function mergeAttachment() {
        try {
            // Upload file lên server rồi lấy thông tin lưu thành chuỗi json
            let uploadResult = await uploadAction();
            let attachmentFilesJson = '';
            let allAttachments = $scope.oldAttachments && $scope.oldAttachments.length ? $scope.oldAttachments.map(({
                id,
                fileDisplayName
            }) => ({
                id,
                fileDisplayName
            })) : [];
            if (uploadResult.isSuccess) {
                let attachmentFiles = uploadResult.object.map(({
                    id,
                    fileDisplayName
                }) => ({
                    id,
                    fileDisplayName
                }));
                allAttachments = allAttachments.concat(attachmentFiles);
            }
            attachmentFilesJson = JSON.stringify(allAttachments);
            return attachmentFilesJson;
        } catch (e) {
            console.log(e);
            return '';
        }
    }
    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });
});