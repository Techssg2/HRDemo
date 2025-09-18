var ssgApp = angular.module('ssg.requestToHireModule', ["kendo.directives"]);
ssgApp.controller('requestToHireController', function ($rootScope, $scope, workflowService,
    commonData, $timeout, $location, appSetting, $stateParams, Notification, localStorageService,
    $state, moment, recruitmentService, masterDataService, settingService, fileService, attachmentService, cbService, attachmentFile, dataService, $translate, $http) {
    // create a message to display in our view
    var ssg = this;
    $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
    $scope.isAllowImportMaual = $rootScope.currentUser && ($rootScope.currentUser.isITHelpDesk || $rootScope.currentUser.isHRAdmin || $rootScope.currentUser.loginName == 'SAdmin');
    $scope.titleHeader = 'REQUEST TO HIRE';
    $scope.titleEdit = "REQUEST TO HIRE: ";
    $scope.title = $stateParams.id ? /*$translate.instant('REQUEST_TO_HIRE_MENU') + */$stateParams.referenceValue : $state.current.name == 'home.requestToHire.item' ? /*"NEW ITEM: REQUEST TO HIRE"*/'' : $stateParams.action.title.toUpperCase();
    $scope.value = 0;
    var currentAction = null;
    isItem = true;
    var id = $stateParams.id;
    $scope.filter = {};
    $scope.queryImportRTH = {};
    $scope.importDialog = {};
    $scope.model = { replacementFor: commonData.typeOfNeed.NewPosition };
    $scope.typeOfNeed = commonData.typeOfNeed;
    $scope.disabled = true;
    $scope.disableFollowTypeOfNeed = false;
    $scope.disableFollowReason = true;
    $scope.disabledForReplacement = true;
    //$scope.disabledForReplacementDepartmentMass = false;
    $rootScope.isParentMenu = false;
    $scope.maxQuantity = 1000000;
    $scope.dataCategories = [];
    $scope.dataMassLocations = [];
    $scope.checkTitleEdit = false;
    dataService.trackingHistory = [];
    var checkBudgetCollection = {
        all: '1',
        budget: '2',
        non_budget: '3'
    }
    $scope.checkBudget = {
        value: checkBudgetCollection.all
    };
    $scope.removeFiles = [];
    $scope.addNew = [];
    var requiredFields = [{
        fieldName: 'positionName',
        title: "Position"
    },
    {
        fieldName: 'jobGradeId',
        title: 'Job Grade'
    },
    {
        fieldName: 'locationCode',
        title: 'Working Location'
    },
    {
        fieldName: 'expiredDayPosition',
        title: 'Expired Day'
    },
    /*{
        fieldName: 'costCenterRecruitmentId',
        title: 'Cost Center'
    },*/
    {
        fieldName: 'hasBudget',
        title: 'Check Budget'
    },
    {
        fieldName: 'workingAddressRecruitmentId',
        title: 'Working Address'
    },
    {
        fieldName: 'operation',
        title: 'HQ/Operation'
    }
    ];
    $scope.total = 0;
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Status desc, Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };

    $scope.currentQueryImport = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };

    $scope.maxDate = new Date();
    $scope.minDate = new Date(2000, 0, 1, 0, 0, 0);
    $scope.bk_CurrentBalance = 0;
    var currentMaximumGrade = 9;
    $scope.selectedTab = "0";
    $scope.processingStageRound = 0;
    $('#mySelect').on('change', function () {
        var selectedValue = $(this).val();
        if (selectedValue === "1") {
            $scope.selectedTab = 1;
            $state.go('home.requestToHire.myRequests')

        } else if (selectedValue === "0") {
            $state.go('home.requestToHire.allRequests')
        }
        else if (selectedValue === "2") {
            $state.go('home.requestToHire.trackingImport')
        }
    });
    $scope.Init = async function () {
        $scope.filter = {
            keyWord: '',
            departmentId: '',
            departmentCode: '',
            position: '',
            fromStartingDate: '',
            toStartingDate: '',
        };
        $scope.checkBudget.value = '1';
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            Order: "Status desc, Created desc",
            Limit: appSetting.pageSizeDefault,
            Page: 1,
            Object: ""
        };
        await getRequestToHiresByFilter($scope.currentQuery);
    }

    async function ngOnit() {
        $rootScope.showLoading();
        if ($state.current.name === 'home.requestToHire.myRequests') {
            $scope.selectedTab = "1";
            $scope.title = "MY REQUESTS TO HIRE";
        }
        else if ($state.current.name === 'home.requestToHire.allRequests') {
            $scope.selectedTab = "0";
            $scope.title = "ALL REQUESTS TO HIRE";
        }
        else if ($state.current.name === 'home.requestToHire.trackingImport') {
            $scope.selectedTab = "2";
            $scope.title = "TRACKING IMPORT";
        }
        if ($state.current.name === 'home.requestToHire.item') {
            await getJobGrades(true);
            if (id) {
                $scope.checkTitleEdit = true;
                //await getDepartments();      
                await getDepartmentByReferenceNumber();
                await getListDetailRequestToHires();
                await getWorkflowProcessingStage(id);
            } else {
                if (!$scope.isPerAdminHrAdmin()) {
                    $timeout(function () {
                        $scope.$apply();
                        $scope.dialog = $rootScope.showConfirmWarningHRPortal(
                            $translate.instant('COMMON_WARNING'),
                            $translate.instant('REDIRECT_LINK'),
                            $translate.instant('COMMON_BUTTON_CLOSE')
                        );
                        $scope.$apply();
                        $scope.dialog.bind("close", async function (e) {
                            if (e.data && e.data.value) {
                            }
                        });
                    }, 0);
                }
                await getDepartmentByFilter({ filter: { value: '' } });
                $scope.model.createdByFullName = $rootScope.currentUser.fullName;
                $scope.currentInstanceProcessingStage = null;
            }
        } else if ($state.current.name === 'home.requestToHire.trackingImport') {
            await getListTrackingImportRequestToHires();
        }
        else {
            await getListRequestToHires();
        }

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

    $scope.onTabChange = function () {
        if ($scope.selectedTab === "1") {
            $state.go('home.requestToHire.myRequests');
        } else if ($scope.selectedTab === "0") {
            $state.go('home.requestToHire.allRequests');
        }
        else if ($scope.selectedTab === "2") {
            $state.go('home.requestToHire.trackingImport')
        }
    };

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

    $scope.openJobGrade = async function () {
        if (currentMaximumGrade) {
            await getJobGrades();
        }
    }
    $rootScope.showLoading();
    this.$onInit = function () {
        ngOnit();
    }
    $rootScope.hideLoading();
    async function getListRequestToHires() {
        $rootScope.showLoading();
        $scope.data = [];
        $scope.requestToHireGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getRequestToHires(e);
                    }
                },
                schema: {
                    total: () => {
                        return $scope.total
                    },
                    data: () => {
                        return $scope.data
                    }
                }
            },

            sortable: false,
            autoBind: true,
            valuePrimitive: false,
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArray,
            },
            columns: [{
                field: "status",
                title: "Status",
                locked: true,
                lockable: false,
                width: "350px",
                template: function (dataItem) {
                    var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                    return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                }
            }, {
                field: "referenceNumber",
                title: "Reference Number",
                locked: true,
                width: "180px",
                template: function (dataItem) {
                    return `<a ui-sref="home.requestToHire.item({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                }
            },
            {
                field: "replacementFor",
                title: "type",
                width: "280px",
                template: function (dataItem) {
                    if (dataItem.replacementFor == $scope.typeOfNeed.NewPosition) {
                        return 'New Position';
                    } else {
                        return 'Replacement For a Valid Position';
                    }
                }
            },
            {
                field: "deptDivisionName",
                title: "Department",
                width: "350px",
                template: function (dataItem) {
                    if (dataItem.replacementFor == $scope.typeOfNeed.NewPosition) {
                        return `${dataItem.deptDivisionName}`;
                    } else {
                        return `${dataItem.replacementForName}`;
                    }
                }
            },
            {
                field: "regionName",
                title: "Region",
                width: "100px"
            },
            {
                field: "positionName",
                title: "Position",
                width: "150px"
            },
            {
                field: "costCenterRecruitmentCode",
                title: "Cost Center",
                width: "350px",
                template: function (dataItem) {
                    if (dataItem && dataItem.costCenterRecruitmentCode !== null) {
                        return `<label>${dataItem.costCenterRecruitmentCode} - ${dataItem.costCenterRecruitmentDescription}</label>`
                    }
                    return '';
                }
            },
            {
                field: "hasBudget",
                title: 'Budget',
                width: "150px",
                template: function (dataItem) {
                    if (dataItem.replacementFor == $scope.typeOfNeed.NewPosition) {
                        if (dataItem && dataItem.hasBudget == 1) {
                            return "Budget";
                        }
                        return 'Non Budget';
                    } else {
                        return "Budget";
                    }
                }
            }, {
                field: "locationName",
                title: 'Working Location',
                width: "150px"
            }, {
                field: "startingDateRequire",
                title: 'Starting Date',
                width: "150px",
                template: function (dataItem) {
                    if (dataItem && dataItem.startingDateRequire !== null && new Date(dataItem.startingDateRequire).getTime() != new Date("1970-01-01T00:00:00+00:00").getTime()) {
                        return moment(dataItem.startingDateRequire).format('DD/MM/YYYY');
                    }
                    return '';
                }
            }, {
                field: "quantity",
                title: 'Quantity',
                width: "100px"
            }, {
                field: "createdByFullName",
                title: 'Requestor',
                width: "200px"
            }, {
                field: "created",
                title: 'Created Date',
                width: "150px",
                template: function (dataItem) {
                    if (dataItem && dataItem.created !== null) {
                        return moment(dataItem.created).format('DD/MM/YYYY HH:mm');
                    }
                    return '';
                }
            }
            ]
        }
        $rootScope.hideLoading();
    }

    //Get data list request to hire
    async function getRequestToHires(option) {
        $rootScope.showLoading();
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        if ($state.current.name == commonData.myRequests.RequestToHire) {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        }

        if ($scope.model && $scope.model.deptDivisionId) {
            $scope.currentQuery.Object = $scope.model.deptDivisionId;
        }

        var res = await recruitmentService.getInstance().requestToHires.getListRequestToHires($scope.currentQuery).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });
        }
        $scope.total = res.object.count;
        if (option) {
            option.success($scope.data);
        } else {
            var grid = $("#grid").data("kendoGrid");
            grid.dataSource.read();
        }
        $rootScope.hideLoading();
    }

    async function getRequestToHiresByFilter(option) {
        $rootScope.showLoading();
        var res = await recruitmentService.getInstance().requestToHires.getListRequestToHires(option).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });
            $scope.total = res.object.count;
            var grid = $("#grid").data("kendoGrid");
            grid.dataSource.read();
        }
        $rootScope.hideLoading();
    }

    valueAssignToId = '';
    valueCostCenterId = '';
    valueWorkingAddressId = '';
    //Get data detail request to hire
    async function getListDetailRequestToHires() {
        let detailRequestToHire = await recruitmentService.getInstance().requestToHires.getListDetailRequestToHire({
            id
        }).$promise;
        if (detailRequestToHire.isSuccess) {
            $scope.model = _.cloneDeep(detailRequestToHire.object.object[0]);
            $scope.statusTranslate = $rootScope.getStatusTranslate($scope.model.status);
            setDefaultCostCenter();
            $scope.selectedUserId = $scope.model.assignToId;
            $scope.selectedCostCenterId = $scope.model.costCenterRecruitmentId;
            $scope.selectedWorkingAddressId = $scope.model.workingAddressRecruitmentId;
            if ($scope.model.replacementForId) {
                await getAllUserHaveCheckedHeadCounts($scope.model.replacementForId);
            }

            if ($scope.selectedUserId) {
                let index = _.findIndex($scope.dataUser, x => {
                    return x.id == $scope.selectedUserId;
                });
                if (index == -1) {
                    $scope.dataUser.push({ id: $scope.selectedUserId, fullName: $scope.model.assignToFullName, sapCode: $scope.model.assignToSAPCode });
                }
                valueAssignToId = $scope.selectedUserId;
                $scope.selectedUserId = '';
                setDataDropdownList("#assignToId", $scope.dataUser, valueAssignToId);

            }

            $timeout(async function () {
                $scope.model = _.cloneDeep(detailRequestToHire.object.object[0]);
                setDefaultCostCenter();
                if (!$scope.jobGradeForHeadCounts && $scope.model.replacementFor == $scope.typeOfNeed.NewPosition) {
                    await getHeadCount($scope.model.deptDivisionId, $scope.model.jobGradeGrade);
                }
                if ($scope.model.fromDate) {
                    $scope.model.fromDate = new Date($scope.model.fromDate);
                }
                if ($scope.model.toDate) {
                    $scope.model.toDate = new Date($scope.model.toDate);
                }
                if (new Date($scope.model.startingDateRequire).getTime() != new Date("1970-01-01T00:00:00+00:00").getTime()) {
                    $scope.model.startingDateRequire = new Date($scope.model.startingDateRequire);
                } else {
                    $scope.model.startingDateRequire = '';
                }
                $scope.disableFollowTypeOfNeed = ($scope.model.replacementFor !== $scope.typeOfNeed.NewPosition);
                $scope.disableFollowReason = $scope.model.reason == 6 ? false : true;
                $scope.disabledForReplacement = ($scope.model.replacementFor == $scope.typeOfNeed.NewPosition);
                //$scope.disabledForReplacementDepartmentMass = ($scope.model.replacementFor != $scope.typeOfNeed.NewPosition);
                currentMaximumGrade = $scope.model.deptDivisionGrade;
                if ($scope.model.replacementFor == $scope.typeOfNeed.ReplacementFor) {
                    currentMaximumGrade = 9;
                }

                try {
                    $scope.oldAttachments = JSON.parse(detailRequestToHire.object.object[0].documents);
                } catch (e) {
                    console.log(e);
                }
                $scope.titleEdit = $scope.titleEdit + $scope.model.referenceNumber;
                updateQuantity();
                //code fix bug
                $timeout(function () {
                    // if ($scope.model.replacementFor == 2) {
                    //     var dropdownlist = $("#jobGradeId").data("kendoDropDownList");
                    //     dropdownlist.enable(false);
                    // }
                    if ($scope.disableFollowTypeOfNeed) {
                        let numerictextbox = $("#quantity").data("kendoNumericTextBox");
                        if (numerictextbox) {
                            numerictextbox.enable(false);
                        }
                    }

                    if ($scope.disabledForReplacement) {
                        var dropdownListReplacementForUsers = $("#replacementFor_UserId").data("kendoDropDownList");
                        var dropdownListReason = $("#reason_id").data("kendoDropDownList");
                        if (dropdownListReplacementForUsers) {
                            dropdownListReplacementForUsers.enable(false);
                        }
                        if (dropdownListReason) {
                            dropdownListReason.enable(false);
                        }

                    }
                    /*if ($scope.dataPositions) {
                        $scope.model.departmentName = $scope.dataPositions && $scope.dataPositions.find(({ id }) => id === $scope.model.positionId) ? $scope.dataPositions.find(({ id }) => id === $scope.model.positionId).name : '';
                    }*/

                    //code moi
                    var dropdownlist = $("#jobGradeId").data("kendoDropDownList");
                    if (dropdownlist) {
                        dropdownlist.enable(false);
                    }
                    //
                }, 1000);
                //
            }, 0);


        }
    }
    async function getDepartmentByReferenceNumber() {
        let referencePrefix = $stateParams.referenceValue.split('-')[0];
        let res = await settingService.getInstance().departments.getDepartmentByReferenceNumber({ prefix: referencePrefix, itemId: $stateParams.id }).$promise;
        if (res.isSuccess) {
            setDataDeplaceFor(res.object.data);
            setDataDepartment(res.object.data);

        }

    }
    $scope.massLocationsDataSource = {
        dataTextField: 'name',
        //Vì bên Mass lưu Name - value nên value là code.
        dataValueField: 'code',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
        filtering: filterMultiField,
        dataSource: {
            serverFiltering: false,
            transport: {
                read: async function (e) {
                    await getMassLocations(e);
                }
            },
        },
        change: function (e) {
            var i = e.sender;
            $scope.model.massLocationName = i.text();
            $scope.model.massLocationCode = i.value();
        }
    };
    async function getMassLocations(option) {
        var res = await settingService.getInstance().recruitment.getMassLocations().$promise;
        if (res.isSuccess) {
            $scope.dataMassLocations = [];
            res.object.forEach(element => {
                let item = element;
                if (item.value) {
                    item.code = item.value;
                }
                $scope.dataMassLocations.push(item);
            });
        }
        if (option) {
            option.success($scope.dataMassLocations);
        }
    }
    $scope.categoriesDataSource = {
        dataTextField: 'name',
        dataValueField: 'id',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
        filtering: filterMultiField,
        dataSource: {
            serverFiltering: false,
            transport: {
                read: async function (e) {
                    await getCategories(e);
                }
            },
        },
        change: function (e) {
            var i = e.sender;
            $scope.model.categoryName = i.text()
        }
    };
    async function getCategories(option) {
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var arg = {
            predicate: 'name.contains(@0)',
            predicateParameters: [filter],
            page: 1,
            limit: 1000,
            order: "created asc"
        }
        var res = await settingService.getInstance().recruitment.getCategories(arg).$promise;
        if (res.isSuccess) {
            $scope.dataCategories = [];
            res.object.data.forEach(element => {
                $scope.dataCategories.push(element);
            });
        }
        if (option) {
            option.success($scope.dataCategories);
        }
    }
    //reportToDataSource
    $scope.assignToDataSource = {
        dataTextField: 'fullName',
        dataValueField: 'id',
        template: '#: sapCode # - #: fullName #',
        valueTemplate: '#: sapCode # - #: fullName #',
        autoBind: true,
        valuePrimitive: true,
        filter: "contains",
        customFilterFields: ['sapCode', 'fullName'],
        filtering: filterMultiField,
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getUsers(e);
                }
            },
        }
    };
    async function getUsers(option) {
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var arg = {
            predicate: "sapcode.contains(@0) or fullName.contains(@1)",
            predicateParameters: [filter, filter],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "sapcode asc"
        }
        var res = await settingService.getInstance().users.getUsers(arg).$promise;
        if (res.isSuccess) {
            $scope.dataUser = [];
            res.object.data.forEach(element => {
                $scope.dataUser.push(element);
            });
            if ($scope.selectedUserId) {
                let index = _.findIndex($scope.dataUser, x => {
                    return x.id == $scope.selectedUserId;
                });
                if (index == -1) {
                    $scope.dataUser.push({ id: $scope.selectedUserId, fullName: $scope.model.assignToFullName, sapCode: $scope.model.assignToSAPCode });
                }
                $scope.selectedUserId = '';
            }
        }
        if (option) {
            option.success($scope.dataUser);
        }
    }

    $scope.departmentOptions = {
        dataTextField: "name",
        dataValueField: "id",
        template: showCustomDepartmentTitle,
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: 'contains',
        loadOnDemand: true,
        filtering: async function (e) {
            await getDepartmentByFilter(e);
        },
        customFilterFields: ['code', 'name'],
        dataSource: {
            data: $scope.departments
        },
        valueTemplate: (e) => showCustomField(e, ['name']),
        change: async function (option) {
            option.preventDefault()
            await onChangeJobGrade(option.sender.value());
        },
        select: async function (e) {
            let dropdownlist = $("#departmentId").data("kendoDropDownTree");
            let dataItem = dropdownlist.dataItem(e.node);
            if (dataItem) {
                $scope.model.deptDivisionCode = dataItem.code;
            }
        }
    };

    async function findCostCenterByDepartmentId(departmentId) {
        if (departmentId) {
            var res = await settingService.getInstance().recruitment.getCostCenterByDepartmentId({ id: departmentId }).$promise;
            if (res.isSuccess) {
                $timeout(function () {
                    $scope.model.costCenterRecruitmentId = res.object;
                }, 0);
            }
            else {
                $timeout(function () {
                    $scope.model.costCenterRecruitmentId = '';
                }, 0);
            }
        }
    }

    //Replacement for
    $scope.replacementForOptions = {
        dataTextField: "name",
        dataValueField: "id",
        //template: showCustomDepartmentTitle,
        template: function (dataItem) {
            return `<span class="${dataItem.item.enable == false ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
        },
        checkboxes: false,
        autoBind: true,
        valuePrimitive: true,
        filter: "contains",
        filtering: async function (e) {
            await getDepartmentByFilter(e);
        },
        loadOnDemand: true,
        // valueTemplate: (e) => showCustomField(e, ['name', 'userCheckedHeadCount'], 'jobGradeCaption')
        valueTemplate: (e) => showCustomField(e, ['name']),
        select: function (e) {
            let dropdownlist = $("#replacementId").data("kendoDropDownTree");
            let dataItem = dropdownlist.dataItem(e.node)
            if (!dataItem.enable) {
                e.preventDefault();
            }
            if (dataItem) {
                $scope.model.replacementForCode = dataItem.code;
            }
        }
    };

    function setDataDeplaceFor(dataDepartment) {

        $scope.departments = dataDepartment;

        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        var dropdownReplacementFor = $("#replacementId").data("kendoDropDownTree");
        if (dropdownReplacementFor) {
            dropdownReplacementFor.setDataSource(dataSource);
        }
    }

    $scope.statusOptions = {
        placeholder: "",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            // serverFiltering: true,
            data: commonData.itemStatusesHR
        }
    };
    $scope.departments = [];
    // Department data


    $scope.deptLine = [];
    async function getDepartmentByFilter(option) {
        let arg = {};
        if (option) {
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
            }
        } else if ($stateParams.id && ($scope.model.deptDivisionId || $scope.model.replacementForId)) {
            let departmentId = ($scope.model.replacementFor == $scope.typeOfNeed.NewPosition) ? $scope.model.deptDivisionId : $scope.model.replacementForId;
            arg = {
                predicate: "id=@0",
                predicateParameters: [departmentId],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
        }
        if (arg.predicate) {
            let res = {};
            if (!option.filter.value) {
                $timeout(function () {
                    res = { isSuccess: true, object: { data: JSON.parse(sessionStorage.getItemWithSafe("departments")) } }
                    updateDataSourceForDepartment(res.object.data);
                }, 0);
            } else {
                res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
                updateDataSourceForDepartment(res.object.data);
            }
        }

    }

    function updateDataSourceForDepartment(data) {
        if ($scope.model.replacementFor == $scope.typeOfNeed.NewPosition) {
            setDataDepartment(data);
        } else {
            //code moi
            data.forEach(x => {
                x['enable'] = false;
                if (x.items.length > 0) {
                    checkEnable(x.items);
                }
            });
            if ($scope.model.positionId) {
                resultPosition = $scope.dataPositions.find(x => x.id == $scope.model.positionId);
                enableDepartment(resultPosition.jobGradeId, data);
                setDataDeplaceFor(data);
            }

        }
    }
    function setDataDepartment(dataDepartment) {
        $timeout(function () {
            $scope.departments = dataDepartment;
            var dataSource = new kendo.data.HierarchicalDataSource({
                data: dataDepartment,
                schema: {
                    model: {
                        children: "items"
                    }
                }
            });
            var dropdownlist = $("#departmentId").data("kendoDropDownTree");
            if (dropdownlist) {
                dropdownlist.setDataSource(dataSource);
            }
        }, 0);
    }

    //Cost Center
    $scope.costCenterListOptions = {
        dataTextField: 'code',
        dataValueField: 'id',
        filter: "contains",
        template: '#: code # - #: description #',
        valueTemplate: '#: code # - #: description #',
        autoBind: false,
        valuePrimitive: true,
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getCostCenters(e);
                }
            },
            schema: {
                data: () => {
                    return $scope.dataCostCenters
                }
            }
        },
        customFilterFields: ['code', 'description'],
        filtering: filterMultiField
    };

    //Working Address
    $scope.workingAddressListOptions = {
        dataTextField: 'address',
        dataValueField: 'id',
        filter: "contains",
        //template: '#: code # - #: address #',
        //valueTemplate: '#: code # - #: address #',
        autoBind: false,
        valuePrimitive: true,
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getWorkingAddress(e);
                }
            },
            schema: {
                data: () => {
                    return $scope.dataWorkingAddress
                }
            }
        },
        customFilterFields: ['code', 'address'],
        filtering: filterMultiField
    };


    //Location
    $scope.locationsDataSource = {
        dataTextField: 'name',
        dataValueField: 'code',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
        filtering: $rootScope.dropdownFilter,
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
        change: function (e) {
            var i = e.sender;
            $scope.model.locationCode = i.value(),
                $scope.model.locationName = i.text()
        }
    };

    //Position
    $scope.positionsDataSource = {
        dataTextField: 'name',
        dataValueField: 'id',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
        template: showCodeName,
        filtering: $rootScope.dropdownFilter,
        dataSource: {
            serverPaging: false,
            pageSize: 100,
            transport: {
                read: async function (e) {
                    await getPositions(e);
                }
            },
            schema: {
                data: () => {
                    return $scope.dataPositions
                }
            }
        },

        change: async function (e) {
            let dropdownlist = $("#positionId").data("kendoDropDownList");
            let dataItem = dropdownlist.dataItem(e.node);
            $scope.model.positionCode = dataItem.code;
            $scope.model.positionName = dataItem.name;
            //code moi
            let numerictextbox = $("#quantity").data("kendoNumericTextBox");
            if ($scope.model.replacementFor == $scope.typeOfNeed.NewPosition) {
                numerictextbox.enable(true);
                //setDataDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")));
                if ($scope.model.deptDivisionId) {
                    var resultDepartment = findDepartment($scope.model.deptDivisionId, $scope.departments);
                    if (dataItem.jobGradeGrade <= resultDepartment.jobGradeGrade) {
                        var resultJobGrade = $scope.jobGrades.find(x => x.id == dataItem.jobGradeId);
                        await getHeadCount($scope.model.deptDivisionId, resultJobGrade.grade);
                        changeJobGrade(resultJobGrade);
                    }
                    else {
                        $timeout(function () {
                            $scope.model.jobGradeId = '';
                            $scope.model.currentBalance = 0;
                            $scope.jobGradeForHeadCounts = [];
                        }, 0);
                    }
                }
                // $scope.model.departmentName = $scope.dataPositions && $scope.dataPositions.find(({ id }) => id === $scope.model.positionId) ? $scope.dataPositions.find(({ id }) => id === $scope.model.positionId).name : '';
            }
            else if ($scope.model.replacementFor == $scope.typeOfNeed.ReplacementFor) {

                setDataDeplaceFor(JSON.parse(sessionStorage.getItemWithSafe("departments")));
                disableNodeDepartment($scope.departments);
                enableDepartment(dataItem.jobGradeId, $scope.departments);
                setDataDeplaceFor($scope.departments);
                setDataReplacementForUsers([]);
                var resultJobGrade = $scope.jobGrades.find(x => x.id == dataItem.jobGradeId);
                changeJobGrade(resultJobGrade);
                let numerictextbox = $("#quantity").data("kendoNumericTextBox");
                numerictextbox.enable(false);
                $timeout(function () {
                    $scope.model.currentBalance = 1;
                    $scope.jobGradeForHeadCounts = [];
                }, 0);
                // $scope.model.departmentName = "";
            }
            //
        },
        valueTemplate: showCodeName
    };
    //Job grade
    $scope.jobGradesDataSource = {
        dataTextField: 'title',
        dataValueField: 'id',
        filter: "contains",
        autoBind: true,
        valuePrimitive: true,
        template: '#: title #',
        valueTemplate: '#: title #',
        filtering: $rootScope.dropdownFilter,
        dataSource: {
            serverPaging: false,
            pageSize: 100,
        },
        change: function (e) {
            $timeout(function () {
                let dropdownlist = $("#jobGradeId").data("kendoDropDownList");
                let dataItem = dropdownlist.dataItem(e.node)
                if (dataItem) {
                    /*$scope.model.jobgradeCaption = dataItem.caption;*/
                    $scope.model.jobGradeCaption = dataItem.grade;
                    $scope.model.jobGradeGrade = dataItem.grade;
                    $scope.model.jobGradeId = dataItem.id;
                    $scope.model.expiredDayPosition = parseFloat(dataItem.expiredDayPosition);
                    let headCount = _.find($scope.jobGradeForHeadCounts, x => { return x.jobGradeForHeadCountId == dataItem.id });
                    if (headCount) {
                        $scope.model.currentBalance = headCount.quantity;
                    } else {
                        $scope.model.currentBalance = 0;
                    }
                    updateQuantity();
                }
            }, 0)
        }
    };

    //replacement For (User)
    $scope.dataUserCheckedHeadCount = [];
    $scope.replacementForUsersOptions = {
        dataTextField: 'fullName',
        dataValueField: 'id',
        template: '#: sapCode # - #: fullName #',
        valueTemplate: '#: sapCode # - #: fullName #',
        autoBind: true,
        valuePrimitive: true,
        filter: "contains",
        //customFilterFields: ['sapCode', 'fullName'],
        //filtering: filterMultiField,
        dataSource: $scope.dataUserCheckedHeadCount
        , change: async function (e) {
            if (e.sender.dataItem().sapCode) {
                let sapCode = e.sender.dataItem().sapCode;
                await getReferenceNumberOfResignationForUser(sapCode);
            }
        }
    };

    //Working time
    $scope.workingTimesDataSource = {
        dataTextField: 'name',
        dataValueField: 'id',
        template: '#: code # - #: name #',
        valueTemplate: '#: code # - #: name #',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
        filtering: $rootScope.dropdownFilter,
        dataSource: {
            serverPaging: false,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getWorkingTimes(e);
                }
            },
            schema: {
                data: () => {
                    return $scope.dataWorkingTimes
                }
            }
        },
        change: function (e) {
            let dropdownlist = $("#workingTimeId").data("kendoDropDownList");
            let dataItem = dropdownlist.dataItem(e.node);
            $scope.model.workingTimeCode = dataItem.code;
            $scope.model.workingTimeName = dataItem.name;
        }
    };

    async function getAllUserHaveCheckedHeadCounts(deptId, textSearch = "") {
        let result = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: deptId, textSearch: textSearch }).$promise;
        if (result.isSuccess) {
            $scope.dataUserCheckedHeadCount = result.object.data;
            if ($scope.model.replacementForUserId) {
                let isRemoveUserInDepartment = _.find($scope.dataUserCheckedHeadCount, x => {
                    return x.id == $scope.model.replacementForUserId;
                });
                if (!isRemoveUserInDepartment) {
                    $scope.dataUserCheckedHeadCount.push({ id: $scope.model.replacementForUserId, sapCode: $scope.model.replacementForUserSAPCode, fullName: $scope.model.replacementForUserFullName })
                }
                else {
                    $timeout(async function () {
                        await getReferenceNumberOfResignationForUser(isRemoveUserInDepartment.sapCode);
                    }, 0);
                }
            }
            else {
                //$scope.model.replacementForUserId = $scope.dataUserCheckedHeadCount.length? $scope.dataUserCheckedHeadCount[0].id: null;   
                //TICKET 471
                if ($scope.dataUserCheckedHeadCount.length) {
                    $scope.model.replacementForUserId = $scope.dataUserCheckedHeadCount[0].id;
                    await getReferenceNumberOfResignationForUser($scope.dataUserCheckedHeadCount[0].sapCode);
                }
                else {
                    $scope.model.replacementForUserId = null;
                    $scope.model.resignationId = '',
                        $scope.model.resignationNumber = ''
                }
            }
            $timeout(function () {
                setDataReplacementForUsers($scope.dataUserCheckedHeadCount);
            });
        }
    }

    function setDataReplacementForUsers(dataUserCheckedHeadCount) {
        $scope.dataUserCheckedHeadCount = dataUserCheckedHeadCount;
        var dataSource = new kendo.data.DataSource({
            data: dataUserCheckedHeadCount
        });

        var dropdownListReplacementForUsers = $("#replacementFor_UserId").data("kendoDropDownList");
        if (dropdownListReplacementForUsers) {
            dropdownListReplacementForUsers.setDataSource(dataSource);
            dropdownListReplacementForUsers.value($scope.model.replacementForUserId);
        }
    }

    async function getReferenceNumberOfResignationForUser(sapCode) {
        if (sapCode) {
            // ticket 471:
            //let arg = {
            //    predicate: "userSAPCode.contains(@0) && status.contains(@1)",
            //    predicateParameters: [sapCode, 'Completed'],
            //    Order: "Created desc",
            //    Limit: 10000,
            //    Page: 1
            //}
            var res = await recruitmentService.getInstance().requestToHires.getResignationApplicantionCompletedBySapCode({ sapcode: sapCode }).$promise;
            if (res.isSuccess && res.object.data.length) {
                $scope.dataResignation = res.object.data;
                $scope.dataResignation = $scope.dataResignation.sort(function (a, b) {
                    return new Date(b.created) - new Date(a.created);
                });

                if ($scope.model.reason == 1) {
                    $timeout(function () {
                        $scope.model.resignationId = $scope.dataResignation[0].id;
                        $scope.model.resignationNumber = $scope.dataResignation[0].referenceNumber;
                    }, 0);
                }

            }
            else {
                $scope.dataResignation = [];
                $timeout(function () {
                    $scope.model.resignationId = '';
                    $scope.model.resignationNumber = '';
                }, 0);

            }
        }
    }

    async function getCostCenters(option) {
        if (option.data.filter) {
            var filter = option.data.filter.filters.length ? capitalize(option.data.filter.filters[0].value) : "";
        }
        else {
            var filter = '';
        }
        var res = await settingService.getInstance().recruitment.getCostCenterRecruiments({
            predicate: "code.contains(@0) || description.contains(@1)",
            predicateParameters: [filter, filter],
            page: 1,
            limit: 10000,
            order: "Modified desc"
        }).$promise;
        if (res.isSuccess) {
            $scope.dataCostCenters = [];
            res.object.data.forEach(element => {
                $scope.dataCostCenters.push(element);
            });
            if ($scope.selectedCostCenterId) {
                let index = _.findIndex($scope.dataCostCenters, x => {
                    return x.id == $scope.selectedCostCenterId;
                });
                if (index == -1 && $scope.model.costCenterRecruitmentCode && $scope.model.costCenterRecruitmentDescription) {
                    $scope.dataCostCenters.push({ id: $scope.selectedCostCenterId, code: $scope.model.costCenterRecruitmentCode, description: $scope.model.costCenterRecruitmentDescription });
                }
                $scope.selectedCostCenterId = '';
            }
        }

        if (option) {
            option.success($scope.dataCostCenters);
        }
    }

    async function getWorkingAddress(option) {
        if (option.data.filter) {
            var filter = option.data.filter.filters.length ? capitalize(option.data.filter.filters[0].value) : "";
        }
        else {
            var filter = '';
        }
        var res = await settingService.getInstance().recruitment.getWorkingAddressRecruiments({
            predicate: "code.contains(@0) || address.contains(@1)",
            predicateParameters: [filter, filter],
            page: 1,
            limit: 10000,
            order: "Modified desc"
        }).$promise;
        if (res.isSuccess) {
            $scope.dataWorkingAddress = [];
            res.object.data.forEach(element => {
                $scope.dataWorkingAddress.push(element);
            });
            if ($scope.selectedWorkingAddressId) {
                let index = _.findIndex($scope.dataWorkingAddress, x => {
                    return x.id == $scope.selectedWorkingAddressId;
                });
                if (index == -1) {
                    $scope.dataWorkingAddress.push({ id: $scope.selectedWorkingAddressId, code: $scope.model.workingAddressRecruitmentCode, address: $scope.model.workingAddressRecruitmentAddress });
                }
                $scope.selectedWorkingAddressId = '';
            }
        }
        if (option) {
            option.success($scope.dataWorkingAddress);
        }
    }

    async function getLocations(option) {
        var resLocations = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            Name: "WorkLocation"
        }).$promise;
        if (resLocations.isSuccess) {
            $scope.dataLocations = [];
            resLocations.object.data.forEach(element => {
                if (element.code.indexOf("A") == 0 || element.code.indexOf("B") == 0) {
                    element.name = `${element.name} - ${element.rawData.Ptext}`;
                }
                $scope.dataLocations.push(element);
            });
            // cheat location 25-2-2022
            //$scope.dataLocations.push({ code: "B008", name: "THE NINE" });
        }
        if (option) {
            option.success($scope.dataLocations);
        }
    }


    async function onChangeJobGrade(id) {
        var resultPosition = null;
        if ($scope.model.positionId) {
            resultPosition = $scope.dataPositions.find(x => x.id == $scope.model.positionId);
        }
        if (!id) {
            setDataDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")));
            $scope.model.currentBalance = 0;
            $scope.jobGradeForHeadCounts = [];
            $scope.model.jobGradeId = '';
        } else {
            var departmentDetail = await settingService.getInstance().departments.getDetailDepartment({
                id
            }).$promise;
            if (departmentDetail.isSuccess && departmentDetail.object && departmentDetail.object.object && departmentDetail.object.object.length > 0) {
                $timeout(function () {
                    let item = departmentDetail.object.object[0];
                    /// Department
                    $scope.model.expiredDayPosition = item.jobGradeExpiredDayPosition;
                    $scope.model.deptDivisionId = item.id;
                    $scope.model.deptDivisionCode = item.code;
                    $scope.model.deptDivisionName = item.name;
                    $scope.model.deptDivisionGrade = item.jobGradeGrade;
                    $scope.model.isStore = item.isStore;
                    $scope.model.isHr = item.isHr;
                }, 0);
            }
            if (resultPosition) {
                var resultDepartment = findDepartment($scope.model.deptDivisionId, $scope.departments)
                if (resultPosition.jobGradeGrade <= resultDepartment.jobGradeGrade) {
                    var resultJobGrade = $scope.jobGrades.find(x => x.id == resultPosition.jobGradeId);
                    await getHeadCount(id, resultJobGrade.grade);
                    changeJobGrade(resultJobGrade);
                }
                else {
                    $timeout(function () {
                        $scope.model.jobGradeId = '';
                    }, 0);
                }
            }
        }
        //
        $scope.model.costCenterRecruitmentId = await findCostCenterByDepartmentId($scope.model.deptDivisionId, JSON.parse(sessionStorage.getItemWithSafe("departments")));

        //await getJobGrades();
    }
    $scope.onChangeReplacementFor = async function (id) {
        if (!id) {
            $scope.departments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
            disableNodeDepartment($scope.departments);
            if ($scope.model.positionId) {
                resultPosition = $scope.dataPositions.find(x => x.id == $scope.model.positionId);
                enableDepartment(resultPosition.jobGradeId, $scope.departments);
                setDataDeplaceFor($scope.departments);
                setDataReplacementForUsers([]);
                $scope.model.resignationNumber = '';
            }
        } else {
            currentMaximumGrade = 9;
            await getJobGrades(true);
            var res = await settingService.getInstance().departments.getDetailDepartment({
                id
            }).$promise;
            if (res.isSuccess && res.object && res.object.object.length > 0) {
                $timeout(function () {
                    let departmentDetail = res.object.object[0];
                    $scope.model.replacementForId = departmentDetail.id;
                    $scope.model.replacementForCode = departmentDetail.code;
                    $scope.model.replacementForName = departmentDetail.name;
                    $scope.model.replacementForGrade = departmentDetail.jobGradeGrade;
                    $scope.model.jobGradeId = departmentDetail.jobGradeId;
                    $scope.model.isStore = departmentDetail.isStore;
                    $scope.model.isHr = departmentDetail.isHr;
                    $scope.model.expiredDayPosition = departmentDetail.jobGradeExpiredDayPosition;
                    $scope.model.jobGradeGrade = departmentDetail.jobGradeGrade;
                }, 0);
                $scope.model.replacementForUserId = '';
                await findCostCenterByDepartmentId(res.object.object[0].id);
                await getAllUserHaveCheckedHeadCounts(id);
                //await getUserForRepacement();
            }
            //await getHeadCount(departmentId);

        }
    }

    async function getHeadCount(departmentId, jobgradeValue = null) {
        if (departmentId) {
            var res = await settingService.getInstance().headcount.getHeadCountByDepartmentId({
                id: departmentId,
                jobGrade: jobgradeValue
            }, null).$promise;
            if (res.isSuccess && res.object) {
                $timeout(function () {
                    $scope.jobGradeForHeadCounts = res.object.data;
                }, 0);
            } else {
                $timeout(function () {
                    $scope.model.currentBalance = 0;
                    $scope.jobGradeForHeadCounts = [];
                }, 0);
            }
        }

    }

    async function getWorkingTimes(option) {
        let queryArgs = {
            predicate: "",
            predicateParameters: [],
            order: "created desc",
            limit: 1000,
            page: 1
        }
        var res = await settingService.getInstance().recruitment.getWorkingTime(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataWorkingTimes = [];
            res.object.data.forEach(element => {
                $scope.dataWorkingTimes.push(element);
            });
        }
        if (option)
            option.success($scope.dataWorkingTimes);
    }
    async function getPositions(option) {
        var queryArgs = {
            predicate: "MetadataType.Value = @0 & !Name.contains(@1)",
            predicateParameters: ['Position', 'Acting'],
            order: "created desc",
            limit: 1000,
            page: 1
        }
        var res = await settingService.getInstance().recruitment.getPositionLists(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataPositions = [];
            res.object.data.forEach(element => {
                $scope.dataPositions.push(element);
            });
        }
        if (option)
            option.success($scope.dataPositions);
    }
    $scope.jobGrades = [];
    async function getJobGrades(hasMaximumGrade = false) {
        var queryArgs = {
            predicate: "",
            predicateParameters: [],
            order: "grade desc",
            limit: 1000,
            page: 1
        }
        if (!$scope.jobGrades || !$scope.jobGrades.length) {
            var res = await settingService.getInstance().jobgrade.getJobGradeList(queryArgs).$promise;
            if (res.isSuccess) {
                $scope.jobGrades = res.object.data;
            }
        }
        setJobGradeSource($scope.jobGrades, hasMaximumGrade);
    }
    async function setJobGradeSource(data, hasMaximumGrade = false) {
        dropdownSource = $("#jobGradeId").data("kendoDropDownList");
        if (dropdownSource !== undefined) {
            let dataList = data;
            if (!hasMaximumGrade) {
                dataList = _.filter(data, x => { return x.grade < currentMaximumGrade });
            }
            var dataSource = new kendo.data.DataSource({
                data: dataList
            });
            $timeout(function () {
                $scope.dataJobgrades = dataList;
                dropdownSource.setDataSource(dataSource);
            }, 0);
        }
    }

    async function getJobGradesBy(e) {
        var queryArgs = {
            predicate: "",
            predicateParameters: [],
            order: "grade desc",
            limit: 1000,
            page: 1
        }
        var res = await settingService.getInstance().jobgrade.getJobGradeList(queryArgs).$promise;
        if (res.isSuccess) {
            dropdownSource = $("#jobGradeId").data("kendoDropDownList");
            var dataList = _.filter(res.object.data, x => { return x.grade < currentMaximumGrade });
            e.success(dataList);
        }

    }
    $scope.isModalVisible = false;
    $scope.toggleFilterPanel = async function (value) {
        $scope.advancedSearchMode = value;
        $scope.isModalVisible = value;
        if ($scope.advancedSearchMode) {
            if (!$scope.filter.departmentCode || $scope.filter.departmentCode == '') {
                await getDepartmentByFilter({ filter: { value: '' } });
            }
        }
    }

    $scope.changeTypeOfNeed = function () {
        $timeout(function () {
            resetModel();
            let numerictextbox = $("#quantity").data("kendoNumericTextBox");
            if ($scope.model.replacementFor == $scope.typeOfNeed.ReplacementFor) {
                $scope.disableFollowTypeOfNeed = true;
                $scope.disabledForReplacement = false;
                //$scope.disabledForReplacementDepartmentMass = true;
                //$scope.model.hasBudget = null;
                $scope.model.hasBudget = 1;
                $scope.model.quantity = 1;
                $scope.model.currentBalance = 1;
                numerictextbox.enable(false);
                //dropdownSource.setDataSource(dataList);
                $scope.model.quantity = 1;
                // $scope.model.departmentName = "";
                //var dropdownListReplacementForUser = $("#replacementFor_UserId").data("kendoDropDownList");
                // dropdownListReplacementForUser.select(0);
                //$scope.model.replacementForUserId = dropdownListReplacementForUser._old;
            } else if ($scope.model.replacementFor == $scope.typeOfNeed.NewPosition) {
                $scope.disableFollowTypeOfNeed = false;
                $scope.disabledForReplacement = true;
                //$scope.disabledForReplacementDepartmentMass = false;
                var dropdownListReplacementForUser = $("#replacementFor_UserId").data("kendoDropDownList");
                dropdownListReplacementForUser.value("");
                dropdownListReplacementForUser.setDataSource([]);
                $scope.model.reason = '';
                $scope.model.otherReason = '';
                $scope.model.replacementForUserId = '';
                $scope.model.hasBudget = null;
                numerictextbox.enable(true);
                dropdownSource = $("#jobGradeId").data("kendoDropDownList");
                var dataList = []
                $scope.model.quantity = '';
                //$scope.model.resignation = '';
                $scope.model.resignationId = '';
                $scope.model.resignationNumber = '';
                // $scope.model.departmentName = $scope.dataPositions && $scope.dataPositions.find(({ id }) => id === $scope.model.positionId) ? $scope.dataPositions.find(({ id }) => id === $scope.model.positionId).name : '';
            }
            updateDataSourceForDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")));
        }, 0);
    }
    function resetModel() {
        $scope.model.deptDivisionId = '';
        $scope.model.deptDivisionCode = '';
        $scope.model.deptDivisionName = '';
        $scope.model.currentBalance = '';
        $scope.model.jobGradeId = '';
        $scope.model.jobGradeCaption = '';
        $scope.model.jobGradeGrade = '';
        $scope.model.costCenterRecruitmentId = '';
        $scope.model.replacementForId = '';
        $scope.model.replacementForCode = '';
        $scope.model.replacementForName = '';
        jobGradeList = $("#jobGradeId").data("kendoDropDownList");
        jobGradeList.value('');
        costCenterRecruitmentList = $("#costCenterRecruitmentId").data("kendoDropDownList");
        if (costCenterRecruitmentList) {
            costCenterRecruitmentList.value('');
        }
    }

    //Data Check Budget

    $scope.budgetOptions = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: [{
            code: commonData.checkBudgetOption.BUDGET,
            name: 'Budget'
        },
        {
            code: commonData.checkBudgetOption.NOBUDGET,
            name: 'Non-Budget'
        },
        ],
        change: function (e) {
            updateQuantity();
        }
    }

    $scope.reasonOptions = {
        dataTextField: "name",
        dataValueField: "value",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            data: translateStatus($translate, commonData.reasonOptions)
        },
        change: async function (e) {
            if (e.sender.value() == 1) {//resign
                if ($scope.dataResignation.length) {
                    $scope.model.resignationId = $scope.dataResignation[0].id,
                        $scope.model.resignationNumber = $scope.dataResignation[0].referenceNumber
                }
                else {
                    $scope.model.resignationId = '',
                        $scope.model.resignationNumber = ''
                }
            }
            else {
                $scope.model.resignationId = '',
                    $scope.model.resignationNumber = ''
            }

            if (e.sender.value() == 6) {
                $timeout(function () {
                    $scope.disableFollowReason = false;
                }, 0);
            }
            else {
                $timeout(function () {
                    $scope.disableFollowReason = true;
                    $scope.model.otherReason = '';
                }, 0);
            }
        }
    }

    $scope.operationOptions = {
        dataTextField: "name",
        dataValueField: "value",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            data: translateStatus($translate, commonData.operationOptions)
        }
    }

    function updateQuantity() {
        let numerictextbox = $("#quantity").data("kendoNumericTextBox");
        if (numerictextbox) {
            if ($scope.model.deptDivisionId && $scope.model.hasBudget == commonData.checkBudgetOption.BUDGET && !$scope.model.currentBalance) {
                numerictextbox.enable(false);
                numerictextbox.value(null);
                $scope.model.quantity = null;
            } else {
                numerictextbox.max(10000);
                numerictextbox.enable(true);

            }
        }
    }

    //Data Contract type
    $scope.contractTypesOptions = {
        dataTextField: 'name',
        dataValueField: 'code',
        valuePrimitive: true,
        oxes: false,
        autoBind: false,
        dataSource: [{
            code: 'FT',
            name: 'Full time'
        },
        {
            code: 'PT',
            name: 'Part time'
        }
        ],
        change: function (e) {
            var i = e.sender;
            $scope.model.contractTypeName = i.text()
        }
    };

    $scope.resetSearch = async function () {
        $scope.model.deptDivisionId = null;
        await $scope.Init();
        $scope.$broadcast('resetToDate', $scope.filter.toStartingDate);
        clearSearchTextOnDropdownTree("departmentId");
        setDataDepartment(JSON.parse(sessionStorage.getItemWithSafe("departments")));
    }

    $scope.fromDateChanged = function () {
        if ($scope.model.fromDate) {
            $scope.minDate = moment($scope.model.fromDate, 'DD/MM/YYYY').toDate();
        }
        if ($scope.filter.startingDateFrom) {
            $scope.minDate = moment($scope.filter.startingDateFrom, 'DD/MM/YYYY').format('MM/DD/YYYY');
        }
    };

    $scope.toDateChanged = function () {
        if ($scope.model.toDate) {
            $scope.maxDate = moment($scope.model.toDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
        }
        if ($scope.filter.startingDateTo) {
            $scope.maxDate = moment($scope.filter.startingDateTo, 'DD/MM/YYYY').format('MM/DD/YYYY');
        }
    };

    $scope.customLocalization = {
        select: '<i class="k-icon k-i-attachment-45"></i>'
    };
    $scope.onChangeEstimateSalaryTo = function (value) {
        let estimateSalaryStart = value;
        if (estimateSalaryStart) {
            var estimateSalaryEnd = $("#estimateSalaryEnd").data("kendoNumericTextBox");
            estimateSalaryEnd.min(estimateSalaryStart);
            estimateSalaryEnd.focus();
        }
    }

    $scope.errors = [];

    $scope.save = async function (form) {
        return await save(form);
    }
    async function save(form) {
        /*$scope.model.jobGradeCaption = $("#jobGradeId").data("kendoDropDownList").text();*/
        let result = { isSuccess: false };
        $scope.errors = validate($scope.model, requiredFields);
        if ($scope.errors.length > 0) {
            return result;
        } else {
            beforeSave();
            if ($scope.attachments.length || $scope.removeFiles.length) {
                let attachmentFilesJson = await mergeAttachment();
                $scope.model.documents = attachmentFilesJson;
            }
            var res;
            if ($scope.model.id) {
                res = await recruitmentService.getInstance().requestToHires.updateRequestToHire(
                    $scope.model
                ).$promise;
            } else {
                res = await recruitmentService.getInstance().requestToHires.createRequestToHire(
                    $scope.model
                ).$promise;
            }
            if (res.isSuccess && res.object) {
                $scope.model = _.cloneDeep(res.object);
                try {
                    $scope.oldAttachments = JSON.parse(res.object.documents);
                    $timeout(function () {
                        $scope.attachments = [];
                        $(".k-upload-files.k-reset").find("li").remove();
                    }, 0);
                } catch (e) {
                    console.log(e);
                }
                $scope.title = /*"REQUEST TO HIRE: " +*/ res.object.referenceNumber;
                if ($scope.removeFiles) {
                    await attachmentService.getInstance().attachmentFile.deleteMultiFile($scope.removeFiles).$promise;
                }
                $timeout(function () {
                    Notification.success("Data successfully saved");
                    if (!$stateParams.id) {
                        $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
                    }
                    if (res.object.id) {
                        $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
                    }
                }, 0);
            } else {
                Notification.success("Error System");
            }
            return res;
        }

    }

    function beforeSave() {
        if ($scope.model.contractTypeCode == 'FT') {
            if ($scope.model.fromDate) {
                $scope.model.fromDate = null;
            }
            if ($scope.model.toDate) {
                $scope.model.toDate = null;
            }
            if ($scope.model.workingHoursPerWeerk) {
                $scope.model.workingHoursPerWeerk = '';
            }
            if ($scope.model.wagePerHour) {
                $scope.model.wagePerHour = '';
            }
        }
    }
    $scope.export = async function () {
        let option = buildArgs();
        if (parseInt($scope.checkBudget.value) > 1) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `HasBudget = @${option.predicateParameters.length}`;
            if (parseInt($scope.checkBudget.value) > 1) {
                if (parseInt($scope.checkBudget.value) == checkBudgetCollection.budget) {
                    option.predicateParameters.push(commonData.checkBudgetOption.BUDGET);
                } else if (parseInt($scope.checkBudget.value) == checkBudgetCollection.non_budget) {
                    option.predicateParameters.push(commonData.checkBudgetOption.NOBUDGET);
                }
            }
        }
        var res = await fileService.getInstance().processingFiles.export({
            type: 1
        }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.onChangeBudget = async function () {
        if ($scope.isITHelpdesk) return;
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Status desc, Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };

        if (!$scope.advancedSearchMode) {
            var option = $scope.currentQuery;
            if (parseInt($scope.checkBudget.value) > 1) {
                option.predicate = "HasBudget = @0"
                if (parseInt($scope.checkBudget.value) == checkBudgetCollection.budget) {
                    option.predicateParameters.push(commonData.checkBudgetOption.BUDGET);
                } else if (parseInt($scope.checkBudget.value) == checkBudgetCollection.non_budget) {
                    option.predicateParameters.push(commonData.checkBudgetOption.NOBUDGET);
                }
                reloadGrid();
            } else {
                reloadGrid();
            }
        } else {
            await $scope.search();
        }

        if ($scope.model.hasBudget == 1) {
            $scope.model.currentBalance = $scope.bk_CurrentBalance;
        }
        else {
            $scope.model.currentBalance = 0;
        }
        updateQuantity();
    }

    var columsSearch = ['ReferenceNumber.contains(@0)', 'LocationName.contains(@1)', 'PositionName.contains(@2)', 'DeptDivisionName.contains(@3)', 'ReplacementForName.contains(@4)']
    function buildArgs() {
        let option = buildArgForSearch(appSetting.pageSizeDefault, $scope.filter, columsSearch, false);
        if ($state.current.name == commonData.requestToHires.myRequests) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `CreatedById = @${option.predicateParameters.length}`;
            option.predicateParameters.push($rootScope.currentUser.id);
        }
        // TamPV
        // them luong de quy department
        /*if ($scope.filter.departmentCode) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(DeptDivisionId = @${option.predicateParameters.length} or ReplacementForId = @${option.predicateParameters.length + 1})`;
            option.predicateParameters.push($scope.filter.departmentCode);
            option.predicateParameters.push($scope.filter.departmentCode);
        }*/
        return option;
    }
    $scope.search = async function () {
        $scope.currentQuery = buildArgs();
        $scope.currentQuery.order = "Status desc, Created desc";

        if (parseInt($scope.checkBudget.value) > 1) {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `HasBudget = @${$scope.currentQuery.predicateParameters.length}`;
            if (parseInt($scope.checkBudget.value) > 1) {
                if (parseInt($scope.checkBudget.value) == checkBudgetCollection.budget) {
                    $scope.currentQuery.predicateParameters.push(commonData.checkBudgetOption.BUDGET);
                } else if (parseInt($scope.checkBudget.value) == checkBudgetCollection.non_budget) {
                    $scope.currentQuery.predicateParameters.push(commonData.checkBudgetOption.NOBUDGET);
                }
                reloadGrid();
            }
        }
        reloadGrid();
        $scope.toggleFilterPanel(false);
    }

    function reloadGrid() {
        var grid = $("#grid").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
            grid.dataSource.page(1);
        }
    }

    function validate(model, requiredFields) {
        var copyRequiredFields = _.cloneDeep(requiredFields);
        //ngan
        if ($scope.model.contractTypeCode == 'PT') {
            copyRequiredFields.push(
                {
                    fieldName: 'workingHoursPerWeerk',
                    title: 'Working Hour Per Week'
                },
                {
                    fieldName: 'fromDate',
                    title: 'From Date'
                },
                {
                    fieldName: 'toDate',
                    title: 'To Date'
                }
            );
        }

        //end
        if ($scope.model.replacementFor == $scope.typeOfNeed.NewPosition) {
            copyRequiredFields.push({
                fieldName: 'deptDivisionId',
                title: 'Department'
            });
        } else {
            copyRequiredFields.push(
                {
                    fieldName: 'replacementForId',
                    title: 'Department'
                },
                {
                    fieldName: 'reason',
                    title: 'Reason'
                }
            );
        }

        if ($scope.model.reason == 6) {
            copyRequiredFields.push({
                fieldName: 'otherReason',
                title: 'Other Reason'
            });
        }
        var errors = validateForm(model, copyRequiredFields);
        if (model.hasBudget == 0 && $scope.model.replacementFor == $scope.typeOfNeed.NewPosition) {
            errors.push({ controlName: 'Check Budget', message: 'Field is required' });
        }
        if (model.quantity == 'undifined' || model.quantity == 0 || !model.quantity) {
            errors.push({ controlName: 'Quantity', message: 'Field is required' });
        }
        if (model.workingTimeId == 'undifined' || model.workingTimeId == 0 || !model.workingTimeId) {
            errors.push({ controlName: 'Working Time', message: 'Field is required' });
        }
        if (model.contractTypeCode == 'undifined' || model.contractTypeCode == 0 || !model.contractTypeCode) {
            errors.push({ controlName: 'Contract Type', message: 'Field is required' });
        }
        if ((!$scope.oldAttachments || !$scope.oldAttachments.length) && (!$scope.attachments || !$scope.attachments.length)) {
            errors.push({ controlName: 'Attachment File', message: 'Field is required' });
        }
        /*if ((!$scope.oldAttachments || !$scope.oldAttachments.length) && (!$scope.attachments || !$scope.attachments.length) && (!model.jobDescription)) {
            errors.push({ controlName: 'Remark', message: 'You have to attach file or fill remark' });
        }*/
        if (model.hasBudget == 1 && (model.currentBalance || model.currentBalance == 0) && model.quantity && model.quantity > model.currentBalance) {
            errors.push({ controlName: 'Quantity cannot exceed the current Balance when Check budget' });
        }
        if (model.hasBudget == 1 && model.currentBalance == 0) {
            errors.push({ controlName: 'This Position is full Budget or you have not set budget for this Position yet' });
        }
        // if(model.replacementForUserId && model.reason == 1 && ! model.resignationNumber){
        //     errors.push({controlName: 'Resignation Application', message: 'Replacement For User has not applied any Resignation'});
        // }
        return errors;
    }

    $scope.attachments = [];
    $scope.oldAttachments = [];
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
            console.log(e);
            return '';
        }
    }
    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }
    $scope.showTrackingHistory = function () {
        $rootScope.visibleTrackingHistory($translate, appSetting.TrackingLogDialogDefaultWidth);
    }
    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });
    $scope.printForm = async function () {
        let res = await recruitmentService.getInstance().requestToHires.printForm({ id: id }).$promise;
        if (res.isSuccess) {
            exportToPdfFile(res.object);
        }
    }

    function checkEnable(dataList) {
        dataList.forEach(item => {
            item['enable'] = false;
            if (item.items.length > 0) {
                return checkEnable(item.items);
            }
        });
    }

    function findDepartment(id, departments) {
        var node = '';
        for (var i = 0; i < departments.length; i++) {
            if (departments[i].id == id) {
                node = departments[i];
                break;
            }
            else {
                if (departments[i].items.length > 0) {
                    node = findDepartment(id, departments[i].items);
                    if (node) {
                        break;
                    }
                }
            }
        }
        return node;
    }
    function enableDepartment(jobGradeId, departments) {
        var node = '';
        for (var i = 0; i < departments.length; i++) {
            if (departments[i].jobGradeId == jobGradeId) {
                departments[i].enable = true;
            }
            else {
                if (departments[i].items.length > 0) {
                    node = enableDepartment(jobGradeId, departments[i].items);
                }
            }
        }
        return node;
    }
    function changeJobGrade(dataItem) {
        $timeout(function () {
            $scope.model.jobGradeCaption = dataItem.grade;
            $scope.model.jobGradeGrade = dataItem.grade;
            $scope.model.jobGradeId = dataItem.id;
            $scope.model.expiredDayPosition = parseFloat(dataItem.expiredDayPosition);
            let headCount = _.find($scope.jobGradeForHeadCounts, x => { return x.jobGradeForHeadCountId == dataItem.id });
            if (headCount) {
                $scope.model.currentBalance = headCount.quantity;
                $scope.bk_CurrentBalance = headCount.quantity;
            }
            else {
                $scope.model.currentBalance = 0;
                $scope.bk_CurrentBalance = 0;
            }

            $scope.onChangeBudget();
        }, 0);

        updateQuantity();
    }

    function disableNodeDepartment(data) {
        data.forEach(x => {
            x['enable'] = false;
            if (x.items.length > 0) {
                checkEnable(x.items);
            }
        });
    }
    //Khiem - task 512
    $scope.$on('workflowStatus', async function (event, data) {
        if (data.isAttachmentFile) {
            $timeout(function () {
                $("#attachmentDetail").removeAttr("disabled");
            }, 0);
        }

    });


    //QUI - task JIRA 84
    $scope.getFilePath = async function (id) {
        let result = await attachmentFile.getFilePath({
            id
        });

        return result.object;
    }
    $scope.owaClose = function () {
        if (!$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))) {
            attachmentViewDialog = $("#attachmentViewDialog").data("kendoDialog");
            attachmentViewDialog.close();
        }
    }

    $scope.viewFileOnline = async function (id) {
        let filePath = await $scope.getFilePath(id);
        if ($.type(filePath) == "string" && filePath.length > 0) {
            callHandler({
                Action: "viewFileOnWeb",
                filePath: filePath,
                itemID: id
            },
                function (resultData) {
                    if (!isNullOrUndefined(resultData) && resultData.data.length > 0) {
                        let attachmentViewDialog = null;
                        if (!$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))) {
                            attachmentViewDialog = $("#attachmentViewDialog").data("kendoDialog");
                        }
                        else {
                            attachmentViewDialog = $("#attachmentViewDialog").kendoDialog({
                                width: "70%",
                                height: "100%",
                                close: function (e) {
                                    $("#attachmentViewDialog").hide();
                                }
                            }).data("kendoDialog");
                        }
                        $("#attachmentViewDialog").show();
                        $("#attachment_owa")[0].setAttribute("src", resultData.data);
                        attachmentViewDialog.open();

                    }
                    else {
                        Notification.error($translate.instant('NOT_SUPPORT_VIEW_ONLINE'));
                        $scope.$apply();
                    }

                }
            );
        }
    }

    function setDefaultCostCenter() {
        if ($scope.model && !$scope.model.costCenterRecruitmentId) {
            $scope.model.costCenterRecruitmentId = 'None';
        }
    }
    $scope.attachmentRequestToHireDetails = [];
    $scope.oldAttachmentRequestTohireDetails = [];
    $scope.removeRequestToHireFileDetails = [];
    $scope.importDialogOpts = {
        width: "600px",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        multiple: false
    };

    $scope.downloadTemplate = async function () {
        let model = {
            SAPCode: '',
        }
        var res = await recruitmentService.getInstance().requestToHires.downloadTemplateImportRequestToHire(model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.closeImportDialog = function () {
        $scope.attachmentDetails = [];
        var upload = $("#attachmentRequestToHireDetail").data("kendoUpload");
        upload.removeAllFiles();
        setTimeout(function () {
            if ($scope.dialogVisible) {
                let dialog = $("#dialog-import").data("kendoDialog");
                dialog.close();
                $scope.dialogVisible = false;
            }
        }, 0);
        return false;
    }

    $scope.importRequestToHire = function () {
        $scope.importDialog.title('IMPORT REQUEST TO HIRE')
        $scope.importDialog.open();
    }

    $scope.onClickSelectFile = function () {
        $(".attachmentDetail").kendoUpload({
            multiple: false,
            validation: {
                allowedExtensions: ['XLS', 'XLSX']
            }
        }).getKendoUpload();
    }
    $scope.onImportSelect = function (e) {
        $.map(e.files, function (file) {
            $scope.attachmentRequestToHireDetails.push(file);
            return file.name;
        }).join(", ");
    };

    $scope.removeImportAttach = function (e) {
        let file = e.files[0];
        $scope.attachmentRequestToHireDetails = $scope.attachmentRequestToHireDetails.filter(item => item.name !== file.name);
    }

    async function mergeAttachmentRequestToHire() {
        try {
            // Upload file lên server rồi lấy thông tin lưu thành chuỗi json
            let uploadResult = await uploadActionRequestToHire();
            let attachmentFilesJson = '';
            let allattachmentRequestToHireDetails = $scope.oldAttachmentRequestTohireDetails && $scope.oldAttachmentRequestTohireDetails.length ? $scope.oldAttachmentRequestTohireDetails.map(({
                id,
                fileDisplayName
            }) => ({
                id,
                fileDisplayName
            })) : [];
            if (uploadResult.isSuccess) {
                allattachmentRequestToHireDetails = allattachmentRequestToHireDetails.concat(['Import Target Plan.xlsx']);
                //await $scope.onCheckStatus();
                $scope.closeImportDialog();
            }
            allattachmentRequestToHireDetails = JSON.stringify(allattachmentRequestToHireDetails);
            return allattachmentRequestToHireDetails;
        } catch (e) {
            console.log(e);
            return '';
        }
    }


    async function uploadActionRequestToHire() {
        $scope.errors = [];
        var payload = new FormData();
        $scope.attachments.forEach(item => {
            payload.append('file', item.rawFile, item.name);
        });

        $scope.attachmentRequestToHireDetails.forEach(item => {
            payload.append('file', item.rawFile, item.name);
        });
        // upload
        let result = await attachmentService.getInstance().attachmentFile.upload(payload).$promise;
        let argData = {
            module: 'RequestToHire'
        };
        if (result.isSuccess) {
            argData.attachmentFileId = result.object[0].id,
                argData.fileName = result.object[0].name
        }
        var header = buildCustomBlobHeader();
        kendo.ui.progress($("#loading"), true);
        let res = await $http({
            method: 'POST',
            url: baseUrlApi + "/RequestToHire/PostFileWithData",
            headers: header,
            transformRequest: function (data) {
                var formData = new FormData();
                formData.append("argData", angular.toJson(data.model));
                data.files.forEach(item => {
                    formData.append('payload', item.rawFile, item.name);
                });
                return formData;
            },
            data: { model: argData, files: $scope.attachmentRequestToHireDetails }
        }).then(res => {
            let result = res.data;
            if (result.isSuccess && result.object) {
                if (result.object.data.status == 'Error') {
                    Notification.error('Upload Data Failure');
                } else {
                    Notification.success('Upload Data Successfully');
                }
                if (result.object && result.object.data) {
                    var dialog = $("#dialog-import").data("kendoDialog");
                    dialog.close();
                    $state.go($state.current.name, {}, { reload: true });
                    return res;
                }
            } else {
                if (result.errorCodes.length) {
                    for (let i = 0; i < result.errorCodes.length; i++) {
                        Notification.error($translate.instant(result.messages[i]));
                    }
                } else {
                    Notification.error($translate.instant('TARGET_PLAN_NOT_FOUND_DATA_IMPORT'))
                }
                $scope.$applyAsync();
                $state.go($state.current.name, {}, { reload: true });
            }
            return result;
        });
        kendo.ui.progress($("#loading"), false);
        return res;
    }

    $scope.sendData = async function () {
        let attachmentFilesJson = null;
        if ($scope.attachmentRequestToHireDetails.length || $scope.removeRequestToHireFileDetails.length) {
            kendo.ui.progress($("#dialog-import"), true);
            attachmentFilesJson = await mergeAttachmentRequestToHire($scope.oldAttachmentRequestTohireDetails, $scope.attachmentRequestToHireDetails);
            kendo.ui.progress($("#dialog-import"), false);
        }
        if (!attachmentFilesJson) {
            Notification.error($translate.instant('TARGET_PLAN_FILE_FORMAT_NOT_OK'));
        }
    }

    $scope.importRTHStatusOptions = {
        placeholder: "",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            // serverFiltering: true,
            data: ["Success", "Error"]
        }
    };
    $scope.ReferenceStatusOptions = {
        placeholder: "",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            // serverFiltering: true,
            data: ["Success", "Failure"]
        }
    };

    async function getListTrackingImportRequestToHires() {
        $scope.dataImport = [];
        $scope.requestToHireTrackingImportGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getImportTracking(e);
                    }
                },
                schema: {
                    total: () => {
                        return $scope.totalImport
                    },
                    data: () => {
                        return $scope.dataImport
                    }
                }
            },
            sortable: false,
            autoBind: true,
            valuePrimitive: false,
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArray,
            },
            columns: [
                {
                    field: "no",
                    title: 'No.',
                    width: "50px"
                },
                {
                    field: "Field name",
                    title: "fileName",
                    width: "250px",
                    template: function (dataItem) {
                        if (dataItem.fileName) {
                            return dataItem.fileName;
                        } else {
                            var document = JSON.parse(dataItem.documents);
                            if (document) {
                                return `<label>${document.fileDisplayName}</label>`;
                            }
                            // return `<div kendo-tooltip k-content="'${data.requestorUserName}'" >${data.requestorFullName}</div>`;
                        }
                    }
                },
                {
                    field: "attachment",
                    title: "Attachment",
                    width: "300px",
                    template: function (dataItem) {
                        var document = JSON.parse(dataItem.documents);
                        if (document) {
                            dataItem.attachmentFileId = document.id;
                            return `<a href="" ng-click="downloadAttachment(dataItem.attachmentFileId)">${document.fileDisplayName}</a>
                                    <a href="" ng-click="downloadAttachment(dataItem.attachmentFileId)">{{'COMMON_BUTTON_DOWLOAD'| translate}}</a>
                                     <a href="" ng-click="viewFileOnline(dataItem.attachmentFileId)">| {{'COMMON_BUTTON_VIEW'| translate}}</a>`;
                        }
                    }
                },
                {
                    field: "log",
                    title: "Log",
                    width: "200px",
                    template: function (dataItem) {
                        /*if (dataItem.fileName) {
                            return dataItem.fileName;
                        } else {
                            return '';
                        }*/
                        return '';
                    }
                },
                {
                    field: "createdBy",
                    title: 'Upload by',
                    width: "200px"
                },
                {
                    field: "createdByFullName",
                    title: 'Upload by full name',
                    width: "200px"
                },
                {
                    field: "Upload date",
                    title: 'Upload Date',
                    width: "150px",
                    template: function (dataItem) {
                        if (dataItem && dataItem.created !== null) {
                            return moment(dataItem.created).format('DD/MM/YYYY HH:mm:ss');
                        }
                        return '';
                    }
                },
                {
                    field: "status",
                    title: 'Status',
                    width: "100px"
                },
                {
                    width: "250px",
                    title: "Actions",
                    template: function (dataItem) {
                        return `<a class="btn-border-upgrade btn-view-detail-upgrade" ng-click="viewDetail(dataItem.jsonDataStr)"></a>`;
                    },
                }
            ]
        }
    }

    $scope.viewDetail = function (data) {
        $scope.trackingImportDetail = JSON.parse(data);
        let dialogReason = $("#trackingImportDetailDialog").kendoDialog({
            title: $translate.instant('COMMON_VIEW_DETAIL'),
            width: "1100px",
            height: "700px",
            modal: true,
            visible: true,
            animation: {
                open: {
                    effects: "fade:in"
                }
            },
            close: function (e) {
                $scope.trackingImportDetail = {};
            }
        });
        let boxReason = dialogReason.data("kendoDialog");
        boxReason.open();
        $rootScope.confirmProcessing = boxReason;
        return $rootScope.confirmProcessing;
    }

    //Get data list request to hire
    async function getImportTracking(option) {
        if (option && option.data && option.data.take) {
            $scope.currentQueryImport.limit = option.data.take;
            $scope.currentQueryImport.page = option.data.page;
        }

        var res = await recruitmentService.getInstance().requestToHires.getImportTracking($scope.currentQueryImport).$promise;
        if (res.isSuccess) {
            $scope.dataImport = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.dataImport.push(element);
            });
        }
        $scope.totalImport = res.object.count;
        if (option && option.data && option.data.take) {
            option.success($scope.data);
        } else {
            var grid = $("#grid_import_tracking").data("kendoGrid");
            grid.dataSource.read();
        }
    }
    var columsSearchImport = ['FileName.contains(@0)', 'CreatedBy.contains(@1)', 'CreatedByFullName.contains(@2)']
    $scope.importTrackingSearch = async function () {
        $scope.currentQueryImport = buildArgForSearch(appSetting.pageSizeDefault, $scope.queryImportRTH, columsSearchImport, false);;
        $scope.currentQueryImport.order = " Created desc";
        $scope.currentQueryImport.referenceId = $scope.queryImportRTH.referenceId;
        $scope.currentQueryImport.referenceNumber = $scope.queryImportRTH.referenceNumber;
        $scope.currentQueryImport.referenceStatus = $scope.queryImportRTH.referenceStatus;
        reloadGridImportTracking();
    }

    function reloadGridImportTracking() {
        var grid = $("#grid_import_tracking").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
            grid.dataSource.page(1);
        }
    }
    $scope.resetSearchImportTracking = async function () {
        $scope.currentQueryImport = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        $scope.queryImportRTH = {
            keyword: '',
            status: '',
            referenceId: '',
            referenceNumber: '',
            referenceStatus: ''
        };
        await getImportTracking($scope.currentQueryImport);
    }

    async function getImportTrackingByFilter(option) {
        var res = await recruitmentService.getInstance().requestToHires.getImportTracking(option).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });
            $scope.total = res.object.count;
            var grid = $("#grid").data("kendoGrid");
            grid.dataSource.read();
        }
    }

    $scope.isPerAdminHrAdmin = function () {
        var _return = false;
        if ($rootScope.currentUser) {
            _return = ((($rootScope.currentUser.role & appSetting.role.HRAdmin) == appSetting.role.HRAdmin) || (($rootScope.currentUser.role & appSetting.role.Admin) == appSetting.role.Admin));
        }
        return _return;
    }
});