var ssgApp = angular.module('ssg.handoverModule', ["kendo.directives"]);
ssgApp.controller('handoverController', function ($rootScope, $scope, $location, appSetting, $stateParams, $state, Notification, commonData, recruitmentService, masterDataService, settingService, $timeout, ssgexService, fileService) {
    // create a message to display in our view
    var ssg = this;
    // $scope.title = '';
    isItem = true;
    $scope.title = $stateParams.id ? /*"HANDOVER" +*/ $stateParams.referenceValue : $state.current.name == 'home.handover.item' ? /*"New item: Handover"*/"" : $stateParams.action.title.toUpperCase();
    $rootScope.isParentMenu = false;
    $scope.filter = {
        fromDate: null,
        toDate: null
    };
    $scope.errors = [];
    $scope.model = {
        applicantId: '',
        receivedDate: '',
        createdByFullName: $rootScope.currentUser.fullName
    };
    var id = $stateParams.id;
    $scope.state = { value: '1' };
    $scope.disabled = false;
    $scope.checkTitleEdit = false;
    $scope.titleEdit = "";
    $scope.titleHeader = "HANDOVER";
    var requiredFields = [{
        fieldName: 'applicantId',
        title: "Applicant Reference Number"
    },
    {
        fieldName: 'receivedDate',
        title: 'Received Date'
    }
    ];

    ssg.requiredValueFields = [
        { fieldName: 'name', title: "Name Item" },
        { fieldName: 'quantity', title: "Quantity" }
    ]

    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        Order: "Created desc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    }

    $scope.Init = function () {
        $scope.filter = {
            keyword: '',
            departmentId: '',
            createdDateObject: '',
            fromDate: null,
            toDate: null
        };
    }
    this.$onInit = async () => {
        if ($state.current.name === 'home.handover.allHandover') {
            $scope.selectedTab = "0";
        }
        else if ($state.current.name === 'home.handover.myHandover') {
            $scope.selectedTab = "1";
        }

        if ($state.current.name === 'home.handover.item') {
            //await getListDetailHandovers();
            await getPosition();
            if (id) {
                await getDetailHandover();
                $scope.disabled = true;
                $('.k-autocomplete, .k-dropdown-wrap.k-state-default, .k-numeric-wrap.k-state-default, .k-picker-wrap.k-state-default').css("background-color", "#eee");
            }
        } else {
            await getListHandovers();
        }
    }

    $scope.onTabChange = function () {
        if ($scope.selectedTab === "1") {
            $state.go('home.handover.myHandover');
        } else if ($scope.selectedTab === "0") {
            $state.go('home.handover.allHandover');
        }
    };

    async function getListHandovers() {
        $scope.data = [];
        $scope.handoverGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getHandovers(e);
                    }
                },
                schema: {
                    total: () => { return $scope.total },
                    data: () => { return $scope.data }
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
                field: "referenceNumber",
                title: "Reference Number",
                width: "200px",
                template: function (dataItem) {
                    return `<a ui-sref="home.handover.item({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }"> ${dataItem.referenceNumber} </a>`
                },
                locked: true,
                lockable: false
            },
            {
                field: "applicantReferenceNumber",
                title: "Applicant ReferenceNumber",
                width: "320px"
            }, {
                field: "applicantFullName",
                title: "Applicant FullName",
                width: "400px"
            },
            {
                field: "userDeptName",
                title: "Department",
                width: "550px", 
            },
            {
                field: "isCancel",
                //title: "Other Reason",
                headerTemplate: "cancel",
                width: "100px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isCancel" name="isCancel{{dataItem.no}}"
                    id="isCancel{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle; opacity: 0.2;"  disabled="disabled"/>
                    <label class="form-check-label" for="isCancel{{dataItem.no}}"></label>`;
                }
            }, 
            {
                field: "created",
                title: 'Created Date',
                width: "380px",
                template: function (dataItem) {
                    if (dataItem && dataItem.created !== null) {
                        return moment(dataItem.created).format('DD/MM/YYYY HH:mm');
                    }
                    return '';
                }
            }
            ]
        };
    }

    $scope.jobGradeId = '';

    $scope.sapCodesDataSource = {
        //optionLabel: "Please Select...",
        dataTextField: 'showData',
        dataValueField: 'id',
        template: '#: referenceNumber # - #: fullName #',
        valueTemplate: '#: referenceNumber #',
        autoBind: true,
        valuePrimitive: false,
        filter: async function (e) {
            await getApplicants(e);
        },
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getApplicants(e);
                }
            }
        },
        change: async function (e) {
            if (e.sender.text()) {
                let dataReferenceNumber = e.sender.text();
                var option = {
                    predicate: "ApplicantStatus.Name == @0 && (fullName.contains(@1) || referenceNumber.contains(@1))",
                    predicateParameters: ["Signed Offer", dataReferenceNumber],
                    order: "Created desc",
                    limit: 1,
                    page: 1
                }
                var res = await recruitmentService.getInstance().applicants.getSimpleApplicantList(
                    option
                ).$promise;
                if (res && res.object.data) {
                    let resultApplicant = res.object.data[0];
                    if (resultApplicant) {
                        $timeout(async function () {
                            $scope.model.userFullName = resultApplicant.fullName;
                            if (resultApplicant.positionName) {
                                $scope.model.positionName = resultApplicant.positionName;
                            } else if (resultApplicant.applicantAppliedPosition1) {
                                $scope.model.positionName = resultApplicant.applicantAppliedPosition1;
                            }
                            $scope.model.jobGradeCaption = resultApplicant.jobGradeCaption;
                            // $scope.model.userDeptName = resultApplicant.jobGradeCaption ? resultApplicant.deptDivision + "(" + resultApplicant.jobGradeCaption + ")" : resultApplicant.deptDivision;
                            $scope.model.userDeptName = resultApplicant.jobGradeCaption ? resultApplicant.deptDivision : resultApplicant.deptDivision;
                            $scope.model.locationName = resultApplicant.locationName;
                            $scope.model.locationCode = resultApplicant.locationCode;
                            $scope.model.departmentType = resultApplicant.departmentType;
                            $scope.model.startDate = resultApplicant.startDate;
                            $scope.model.deptDivisionCode = resultApplicant.deptDivisionCode;
                            $scope.model.deptDivision = resultApplicant.deptDivision;
                            $scope.model.positionId = resultApplicant.positionId;
                            $scope.model.deptDivisionJobGradeId = resultApplicant.deptDivisionJobGradeId;
                        }, 0);

                        await getItemListRecruitments();
                        //tim jobgrade
                        let reusltPosition = $scope.positions.find(x => x.id == resultApplicant.positionId)
                        if (reusltPosition) {
                            $scope.jobGradeId = reusltPosition.deptDivisionJobGradeId;
                            await getItemListRecruitments($scope.jobGradeId);
                            //await getAllItemList();
                            await getItemOfJobGrade(reusltPosition.deptDivisionJobGradeId);
                        }
                        else {
                            let dataEmpty = [];
                            $scope.jobGradeId = ''
                            setDataHandoverDetails(dataEmpty);
                        }

                    }
                }

            }
        }
    };

    async function changeApplicantReferenceNumber(referenceNumber) {
        if (referenceNumber) {
            let resultApplicant = $scope.dataApplicant.find(x => x.referenceNumber == referenceNumber);
            if (resultApplicant) {
                await getItemListRecruitments();
            }
        }
    }

    $scope.dataApplicant = [];
    async function getApplicants(e) {
        var filter = e && e.data.filter && e.data.filter.filters.length ? e.data.filter.filters[0].value : "";
        var option = {
            predicate: "ApplicantStatus.Name == @0 && (fullName.contains(@1) || referenceNumber.contains(@1))",
            predicateParameters: ["Signed Offer", filter],
            order: "Created desc",
            limit: 40,
            page: 1
        }
        var res = await recruitmentService.getInstance().applicants.getSimpleApplicantList(
            option
        ).$promise;
        if (res.isSuccess) {
            if ($scope.model.applicantId && _.findIndex(res.object.data, x => x.id == $scope.model.applicantId) == -1) {
                res.object.data.push({ id: $scope.model.applicantId, referenceNumber: $scope.model.applicantReferenceNumber, fullName: $scope.model.applicantFullName, positionId: $scope.model.positionId });
            }
            res.object.data.forEach(element => {
                element['showData'] = element.referenceNumber + ' - ' + element.fullName;
            });
            $scope.dataApplicant = res.object.data;
            if (e) {
                e.success($scope.dataApplicant);
            } else {
                var dropdownlist = $("#userId").data("kendoDropDownList");
                var dataSource = new kendo.data.HierarchicalDataSource({
                    data: $scope.dataApplicant,
                    serverPaging: true,
                    pageSize: 20,
                });
                if (dropdownlist) {
                    dropdownlist.setDataSource(dataSource);
                    dropdownlist.value($scope.model.applicantId);
                    await changeApplicantReferenceNumber($scope.model.applicantReferenceNumber);
                }
            }
        }
    }

    $scope.dataDetail = new kendo.data.ObservableArray([]);
    $scope.handoverDetailGridOptions = {
        dataSource: $scope.dataItemLists,
        autoBind: true,
        valuePrimitive: false,
        sortable: false,
        editable: {
            mode: "inline",
            confirmation: false,
        },
        pageable: false,
        columns: [{
            field: "no",
            title: "No.",
            width: "50px"
        }, {
            field: "itemListRecruitmentId",
            title: "Item",
            width: "220px",
            template: function (dataItem) {
                // if (dataItem.selected || !dataItem.id) {
                if (!$stateParams.id) {
                    return `<select kendo-drop-down-list name="name" id="dataItemRecruitmentList"
                    k-options="dataItemListRecruitments"
                    k-data-source="dataItemLists"
                    data-k-ng-model="dataItem.itemListRecruitmentId"         
                    k-value-primitive="'false'"                             
                    k-on-change="onchangeItem(kendoEvent, dataItem)"
                    style="width: 100%;">
                    </select>`
                } else {
                    return `<select kendo-drop-down-list name="name" id="dataItemRecruitmentList"
                    k-options="dataItemListRecruitments"
                    k-data-source="dataItemLists"
                    data-k-ng-model="dataItem.itemListRecruitmentId"         
                    k-value-primitive="'false'"                             
                    k-on-change="onchangeItem(kendoEvent, dataItem)"
                    style="width: 100%; background-color: #eee;" readonly>
                    </select>`
                    
                }
            }
        },
        {
            field: "serialNumber",
            title: "Series/ Number",
            width: "150px",
            template: function (dataItem) {
                if (!$stateParams.id) {
                    return `<input class="k-textbox w100" name="serialNumber" ng-model="dataItem.serialNumber"/>`;
                } else {
                    return `<input class="k-textbox w100" name="serialNumber" ng-model="dataItem.serialNumber" readonly style="background-color: #eee;"/>`;
                }
            }
        }, {
            field: "unit",
            title: 'Unit',
            width: "60px",
            template: function (dataItem) {
                if (!$stateParams.id) {
                    return `<input type="text" id="unit" name="unit" ng-model="dataItem.unit" class="k-textbox form-control input-sm" style="width: 100%;" />`
                } else {
                    return `<input type="text" id="unit" name="unit" ng-model="dataItem.unit" class="k-textbox form-control input-sm" style="width: 100%;background-color: #eee;" readonly />`
                }

            }
        }, {
            field: "quantity",
            title: 'Quantity',
            width: "80px",
            template: function (dataItem) {
                if (!$stateParams.id) {
                    return `<input kendo-numeric-text-box id="quantity" k-min="0" name="quantity" k-format="'#,0'" k-ng-model="dataItem.quantity" style="width: 100%;" />`
                } else {
                    return `<input kendo-numeric-text-box id="quantity" k-min="0" name="quantity" k-format="'#,0'" k-ng-model="dataItem.quantity" style="width: 100%;" disabled/>`
                }
            }
        }, {
            field: "notes",
            title: 'Notes',
            width: "100px",
            template: function (dataItem) {
                if (!$stateParams.id) {
                    return `<input class="k-textbox w100" name="notes" ng-model="dataItem.notes"/>`;
                } else {
                    return `<input class="k-textbox w100" name="notes" ng-model="dataItem.notes" style="background-color: #eee;" readonly/>`;
                }
            }
        },
        {
            field: "action",
            title: "Actions",
            width: "150px",
            hidden: $stateParams.id != '',
            template: function (dataItem) {
                if (!$stateParams.id) {
                    return `<a class="btn-border-upgrade btn-delete-upgrade" ng-click="deleteRow(dataItem)"></a >`;
                   /* return `<a class="btn btn-outline-dark rounded-3 btn-sm poppins-regular display-9 m-1" ng-click="deleteRow(dataItem)">
                        <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z"></path>
                        <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z"></path>
                        </svg>Delete</a>`;*/
                } else {
                    return '';
                }
            }
        }
        ],
        selectable: true
    };

    $scope.deleteRow = function (dataItem) {
        if (dataItem.id) {
            $scope.id = dataItem.id;
        }
        else {
            $scope.id = dataItem.uid;
        }
        $scope.dialog = $rootScope.showConfirmDelete("DELETE", 'Are you sure to delete this item', 'Confirm');
        $scope.dialog.bind("close", confirm);
    }

    confirm = function (e) {
        if (e.data && e.data.value) {
            for (var i = 0; i < $scope.dataDetail.length; i++) {
                if ($scope.dataDetail[i].id) {
                    if ($scope.dataDetail[i].id === $scope.id) {
                        $scope.dataDetail.splice(i, 1);
                    }
                }
                else {
                    if ($scope.dataDetail[i].uid === $scope.id) {
                        $scope.dataDetail.splice(i, 1);
                    }
                }
            }
            initGridHandoverDetail($scope.dataDetail);
            Notification.success("Data Sucessfully Deleted");
        }
    }

    function initGridLeason(dataSource) {
        let grid = $("#handoverDetailGrid").data("kendoGrid");
        let dataSourceMissingTimelock = new kendo.data.DataSource({
            data: dataSource,
        });
        grid.setDataSource(dataSourceMissingTimelock);
    }

    function buildArgs(pageIndex, pageSize) {
        let conditions = [
            'ReferenceNumber.contains(@{i})',
            'Applicant.ReferenceNumber.contains(@{i})',
            'Applicant.FullName.contains(@{i})',
            'Applicant.Position.DeptDivision.Name.contains(@{i})'
        ];

        conditionDepartmentId = 'User.userDepartmentMappings.any(departmentId = @{i})';

        //$stateParams.type === commonData.pageStatus.myRequests
        let stateArgs = {
            currentState: $state.current.name,
            myRequestState: 'home.handover.myHandover',
            currentUserId: $rootScope.currentUser.id
        };
        //home.handover.myHandover
        return createQueryArgsForCAB({ pageIndex, pageSize, order: appSetting.ORDER_GRID_DEFAULT }, conditions, $scope.filter, stateArgs);
    }
    function buildSearchInfo(option){
        let args = buildArgs(1, appSetting.pageSizeDefault);
        if (option) {
            // $scope.currentQuery.Limit = option.data.take;
            // $scope.currentQuery.Page = option.data.page;
            args.limit = option.data.take;
            args.page = option.data.page;
        }

        if ($scope.filter.departmentId) {
            args.predicate = (args.predicate ? args.predicate + ' and ' : args.predicate) + `(Applicant.Position.DeptDivisionId = @${args.predicateParameters.length})`;
            args.predicateParameters.push($scope.filter.departmentId);
        }
        if ($scope.state.value == '1') {
            args.predicate = (args.predicate ? args.predicate + ' and ' : args.predicate) + `(isCancel = @${args.predicateParameters.length} || isCancel = @${args.predicateParameters.length + 1})`;
            args.predicateParameters.push(true);
            args.predicateParameters.push(false);
        }
        if ($scope.state.value == '2') {
            args.predicate = (args.predicate ? args.predicate + ' and ' : args.predicate) + `(isCancel = @${args.predicateParameters.length})`;
            args.predicateParameters.push(true);
        }
        if ($scope.state.value == '3') {
            args.predicate = (args.predicate ? args.predicate + ' and ' : args.predicate) + `(isCancel = @${args.predicateParameters.length})`;
            args.predicateParameters.push(false);
        }
        return args;

    }

    async function getHandovers(option) {
        $scope.currentQueryExport = buildSearchInfo(option);
        var res = await recruitmentService.getInstance().handovers.getListHandovers(
            $scope.currentQueryExport
        ).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                // if (element.mapping) {
                //     element['userDeptName'] = element.mapping.departmentName;
                // }
                $scope.data.push(element);
            });
        }
        $scope.total = res.object.count;
        if (option) {
            option.success($scope.data);
        } else {
            var grid = $("#grid").data("kendoGrid");
            grid.dataSource.read();
            if ($scope.advancedSearchMode) {
                grid.dataSource.page(1);
            }
        }
    }

    async function getDetailHandover(option) {
        if (id) { //View Edit
            $scope.checkTitleEdit = true;
            let detailHandover = await recruitmentService.getInstance().handovers.getListDetailHandover({ id }).$promise;
            if (detailHandover.isSuccess && detailHandover.object.object) {
                await changeApplicantReferenceNumber(detailHandover.object.object.applicantReferenceNumber);
                $timeout(async function () {
                    $scope.model = detailHandover.object.object;
                    await getApplicants(option);
                    // $scope.title = $scope.model.referenceNumber;
                    $scope.titleEdit = $scope.model.referenceNumber;
                    if ($scope.model.handoverDetailItems) {
                        var items = $scope.model.handoverDetailItems;
                        $scope.dataDetail = [];
                        var n = 1;
                        items.forEach(element => {
                            element.no = n++;
                            $scope.dataDetail.push(element);
                        });
                        initGridHandoverDetail($scope.dataDetail);

                        // console.log(document.getElementsByClassName("k-dropdown-wrap k-state-default")[1].style);
                        // document.getElementsByClassName("k-dropdown-wrap k-state-default")[1].style.backgroundColor = "red";
                        changeColorBackground();
                    }
                }, 0);
            }
        }
    }

    function changeColorBackground() {
        for(var i = 1; i < document.getElementsByClassName("k-dropdown-wrap k-state-default").length; i++) {
            document.getElementsByClassName("k-dropdown-wrap k-state-default")[i].style.backgroundColor = "#eee";
        }
    }

    function initGridHandoverDetail(dataSource) {
        let grid = $("#handoverDetailGrid").data("kendoGrid");
        let dataSourceHandoverDetail = new kendo.data.DataSource({
            data: dataSource,
        });
        grid.setDataSource(dataSourceHandoverDetail);
    }

    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    //Department 
    $scope.departments = [];
    $scope.departmentOptions = {
        dataTextField: "name",
        dataValueField: "id",
        template: showCustomDepartmentTitle,
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        loadOnDemand: true,
        filtering: async function (e) {
            await getDepartmentByFilter(e);
        },
        dataSource: {
            data: $scope.departments
        },
        valueTemplate: (e) => showCustomField(e, ['name']),
        change: async function (e) {
            if (!e.sender.value()) {
                await setDataDepartment(allDepartments);
            }
        }
    };

    async function getDepartmentByFilter(option) {
        let departmentId = option.sender.value();
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
                await setDataDepartment(res.object.data, departmentId);
            }
        }

    }
    async function setDataDepartment(dataDepartment, oldDepartmentId) {
        //$scope.departments = dataDepartment;
        if(oldDepartmentId){
            var departmentDetail = await settingService.getInstance().departments.getDetailDepartment({
                id: oldDepartmentId
            }).$promise;
            if(departmentDetail.isSuccess){
                let valueSearchDepartment = departmentDetail.object.object;
                dataDepartment = dataDepartment.concat(valueSearchDepartment);
            }
        }

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
            if(dropdownlist._value){
                dropdownlist.options.autoClose = true;
            }
            if(oldDepartmentId){
                dropdownlist.options.autoClose = false;
                dropdownlist.value(oldDepartmentId);
            }
        }
    }

    $scope.isModalVisible = false;
    $scope.toggleFilterPanel = async function (value) {
        $scope.advancedSearchMode = value;
        $scope.isModalVisible = value;
        if (value) {
            if (!$scope.filter.departmentId || $scope.filter.departmentId == '')
                await setDataDepartment(allDepartments);
        }
    }

    $scope.search = async function (reset) {
        await getHandovers();
        $scope.toggleFilterPanel(false);
    }

    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        Order: "Created desc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    };

    $scope.resetSearch = async function () {
        $scope.Init();
        $scope.$broadcast('resetToDate', $scope.filter.toDate);
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            Order: "Created desc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        };
        // var option = $scope.currentQuery
        await getHandovers();
        clearSearchTextOnDropdownTree("departmentId");
        await setDataDepartment(allDepartments);
    }

    async function GetHandoverByFilter(option) {
        recruitmentService.getInstance().handovers.getHandovers(option).$promise.then(function (result) {
            if (result.isSuccess) {
                result.object.data.forEach(item => {
                    if (item.mapping) {
                        item['userDeptName'] = item.mapping.departmentName;
                    }
                })
                initGridRequests(result.object.data, result.object.count, 1);
            }
        })
    }

    function initGridRequests(dataSource, total, pageIndex) {
        let grid = $("#grid").data("kendoGrid");
        let dataSourceRequests = new kendo.data.DataSource({
            data: dataSource,
            pageSize: appSetting.pageSizeDefault,
            page: pageIndex,
            schema: {
                total: function () {
                    return total;
                }
            },
        });
        grid.setDataSource(dataSourceRequests);
    }

    async function getItemOfJobGrade(jobGradeId) {
        $scope.dataDetail = [];
        if (jobGradeId) {
            var result = await settingService.getInstance().jobgrade.getItemRecruitmentsOfJobGrade({ jobGradeId }).$promise;
            if (result.isSuccess && result.object) {
                if (result.object.data) {
                    let items = result.object.data;
                    $scope.dataDetail = [];
                    var n = 1;
                    items.forEach(element => {
                        element.no = n++;
                        element['code'] = element.itemCode;
                        element['name'] = element.itemName;
                        element['unit'] = element.itemUnit;
                        element['quantity'] = 1;
                        element['itemListRecruitmentId'] = element.itemRecruitmentId;
                        $scope.dataDetail.push(element);
                    });
                    setDataHandoverDetails($scope.dataDetail);
                }
            }
        }
    }

    function setDataHandoverDetails(dataHandoverDetails) {
        var dataSource = new kendo.data.DataSource({
            data: dataHandoverDetails
        });
        var grid = $("#handoverDetailGrid").data("kendoGrid");

        grid.setDataSource(dataSource);
    }

    $scope.addRow = function (item) {
        var goalGridCurrent = $("#handoverDetailGrid").data("kendoGrid");
        $scope.dataDetail = goalGridCurrent.dataSource._data;
        let value = {
            no: $scope.dataDetail.length + 1,
            name: '',
            serialNumber: '',
            unit: '',
            quantity: 1,
            notes: '',
            itemListRecruitmentId: '',
        };
        $scope.dataDetail.push(value);
        $scope.dataDetail[$scope.dataDetail.length - 1].code = $scope.dataDetail[$scope.dataDetail.length - 1].uid;
        var grid = $("#handoverDetailGrid").data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            data: $scope.dataDetail,
        });
        grid.setDataSource(dataSource);
    };

    $scope.submit = function (form) {
        $scope.save(form);
    }
    $scope.cancelHandover = async function () {
        $scope.model.isCancel = true;
        let res = await recruitmentService.getInstance().handovers.updateHandover(
            $scope.model
        ).$promise;
        if (res.isSuccess) {
            if (res.object && !$scope.model.id) {
                $scope.model.id = res.object.id;
                $scope.model.referenceNumber = res.object.referenceNumber;
                $state.go('home.handover.item', { id: res.object.id, referenceValue: res.object.referenceNumber });
            }
            Notification.success("Cancelled");
        } else { }
    }
    $scope.save = async function (form) {
        $scope.errors = $rootScope.validateForm(form, requiredFields, $scope.model);
        if (!$scope.model.applicantId) {
            $scope.errors.push({ controlName: "Applicant Reference Number", fieldName: "applicantId" })
        }
        if (!$scope.model.receivedDate) {
            $scope.errors.push({ controlName: "Received Date", fieldName: "applicantId" })
        }
        if ($scope.errors.length > 0) {
            return;
        }
        else {
            $scope.errors = $scope.errors.concat(validateTable());
            if ($scope.errors.length > 0) {
                return;
            }
            let data = Object.assign($scope.model);
            data.handoverDetailItems = $("#handoverDetailGrid").data("kendoGrid").dataSource.data();
            var res;
            if ($scope.model.id) {
                res = await recruitmentService.getInstance().handovers.updateHandover(
                    $scope.model
                ).$promise;
            } else {
                res = await recruitmentService.getInstance().handovers.createHandover(
                    data
                ).$promise;
            }

            if (res.isSuccess) {
                if (res.object && !$scope.model.id) {
                    $scope.model.id = res.object.id;
                    $scope.model.referenceNumber = res.object.referenceNumber;
                    $state.go('home.handover.item', { id: res.object.id, referenceValue: res.object.referenceNumber });
                }
                Notification.success("Data successfully saved");
            } else { }
        }
    }

    $scope.dataItemListRecruitments = {
        dataTextField: 'name',
        //dataValueField: 'id',
        dataValueField: 'id',
        template: '#: itemCode # - #: itemName #',
        valueTemplate: '#: itemCode # - #: itemName #',
        autoBind: true,
        filter: "contains",
        filtering: $rootScope.dropdownFilter
    };

    $scope.onchangeItem = function (e, dataItem) {
        let resultItem = $scope.dataItemLists.find(x => x.id == dataItem.itemListRecruitmentId);
        var data = $("#handoverDetailGrid").data("kendoGrid").dataSource._data.find(x => x.no === dataItem.no);
        data.unit = resultItem.unit;
        data.serialNumber = '';
        data.quantity = 1;
        data.notes = '';
        //data.itemRecruitmentId = resultItem.itemRecruitmentId;
    }

    async function getItemListRecruitments() {
        $scope.dataItemLists = [];
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 100000
        };

        var res = await settingService.getInstance().recruitment.getItemList(queryArgs).$promise;
        if (res.isSuccess) {
            // $scope.dataItemLists = res.object.data;
            res.object.data.forEach(element => {
                element['itemCode'] = element.code;
                element['itemName'] = element.name;
                element['itemUnit'] = element.unit;
                element['quantity'] = 1;
                $scope.dataItemLists.push(element);
            });
        }
        $timeout(function () {
            var dataItemRecruitmentOption = $("#dataItemRecruitmentList").data("kendoDropDownList");
            dataItemRecruitmentOption.setDataSource($scope.dataItemLists);
        }, 1000);
    }

    requiredFieldsForTable = [{
        fieldName: 'itemListRecruitmentId',
        title: "Item"
    },
    {
        fieldName: 'quantity',
        title: "Quantity"
    },
        //{
        //    fieldName: 'notes',
        //    title: "Notes",
        //}
    ];

    function validateTable() {
        var errors = [];
        var dataList = $("#handoverDetailGrid").data("kendoGrid").dataSource.data();
        if (dataList.length > 0) {
            dataList.forEach(item => {
                requiredFieldsForTable.forEach(field => {
                    if (!item[field.fieldName]) {
                        errors.push({
                            controlName: `${field.title} of No ${item.no}`
                        });
                    }
                });
            })
        }
        else {
            errors.push({ controlName: `Table Item List` });
        }
        return errors;
    }

    $scope.query = {
        keyword: '',
        departmentId: '',
        fromDate: '',
        toDate: '',
    }

    $scope.currentQueryExport = {
        predicate: "",
        predicateParameters: [],
        Order: "Created desc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    }

    $scope.export = async function () {
        let option = buildSearchInfo(null);
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.HANDOVER }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.positions = [];
    async function getPosition() {
        var result = await recruitmentService.getInstance().position.getOpenPositions().$promise;
        if (result.isSuccess) {
            $scope.positions = result.object.data;
        }
    }

    $scope.itemLists = [];
    async function getAllItemList() {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 100000
        };

        var result = await settingService.getInstance().recruitment.getItemList(queryArgs).$promise;
        if (result.isSuccess) {
            $scope.itemLists = result.object.data;
            $scope.itemLists.forEach(item => {
                item['itemCode'] = item.code;
                item['itemName'] = item.name;
                item['itemUnit'] = item.unit;
            })
        }
    }

    $scope.jobgradeItems = [];
    $scope.onChangeState = async function () {
        await getHandovers(null);
    }
    $scope.close = function(){
        $rootScope.cancel();
    }

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });
});