var ssgApp = angular.module('ssg.positionModule', ["kendo.directives"]);
ssgApp.controller('positionController', function ($rootScope, $scope, $location, appSetting, $stateParams, $state, commonData, Notification, $timeout, recruitmentService, settingService, masterDataService, ssgexService, fileService, $translate) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = '';
    $scope.hasListView = true;
    $scope.hasGridView = false;
    $scope.showSearch = false;
    $scope.maxDate = new Date();
    $scope.minDate = new Date(2000, 0, 1, 0, 0, 0);
    $scope.isList = commonData.typePosition.isList;
    $scope.isThumbNail = commonData.typePosition.isThumbNail;
    $scope.additionalPositionCode = null;
    $scope.additionalPositionName = null;
    $scope.dataTrackingLog = [];
    $scope.titleModal = 'Title modal';
    var notInSadmin = appSetting.role.Admin | appSetting.role.HR | appSetting.role.CB | appSetting.role.Member;
    $scope.total = 0;
    $scope.data = [];

    $scope.assigneeDetail = {
        newAssigneeId: "",
        newAssigneeObject: {
            id: "",
            fullName: "",
            sapCode: ""
        },
        assignToFullName: "",
    }

    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };

    $scope.currentQueryReassignee = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };

    $scope.quantityOptions = {
        min: 0,
        max: 99999,
        decimals: 0,
        restrictDecimals: true
    }
    let requiredFields = [
        {
            fieldName: "newAssigneeId",
            title: "New Assignee"
        }
    ];
    $scope.initFilter = function () {
        $scope.filter = {
            keyword: '',
            //assignment: '',
            department: '',
            hasApplicant: false,
            position: '',
            statusDetail: '',
            fromDateObject: null,
            toDateObject: null,
        };
    };
    $scope.initFilter();
    var checkStatusCollection = {
        all: '0',
        open: '1',
        closed: '2'
    }
    $scope.checkStatus = {
        value: checkStatusCollection.all
    };
    $scope.widget = {
        dialogDetail: {}
    };

    $scope.quantityMax = 99999;
    $scope.isVisibleRe_AssignBtn = false;
    // $scope.assignOptions = {
    //     placeholder: "",
    //     dataTextField: "fullName",
    //     dataValueField: "id",
    //     template: '#: sapCode # - #: fullName #',
    //     valueTemplate: '#: sapCode # - #: fullName #',
    //     valuePrimitive: false,
    //     checkboxes: false,
    //     autoBind: true,
    //     filter: "contains",
    //     dataSource: {
    //         serverPaging: false,
    //         pageSize: 100,
    //         transport: {
    //             read: async function (e) {
    //                 var res = await settingService.getInstance().users.getUsers({
    //                     predicate: "",
    //                     predicateParameters: [],
    //                     order: "Created desc",
    //                     limit: 100,
    //                     page: 1
    //                 }).$promise;
    //                 if (res.isSuccess) {
    //                     e.success(res.object.data);
    //                 }
    //             }
    //         },
    //     },
    //     dataBound: function () {
    //         this.select(-1);
    //         this.trigger("change");
    //     },
    // };

    $scope.selectedDepartment = {};

    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
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
        $scope.departments = dataDepartment;
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        var dropdownlist = $("#departmentSearch").data("kendoDropDownTree");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }

    async function getExpiredDay(jobGradeId) {
        var res = await settingService.getInstance().jobgrade.getJobGradeById({
            id: jobGradeId
        }).$promise;
        if (res.isSuccess && res.object) {
            $timeout(function () {
                $scope.form.expiredDay = res.object.expiredDayPosition + "";
            }, 0);

        }
    }

    $scope.listPositionDetail = [];
    async function getPositionDetail() {
        let positionId = '';
        var position = await recruitmentService.getInstance().position.getPositionById({ id: $stateParams.id }).$promise;
        if (position.isSuccess) {
            positionId = position.object.id;
        }
        var res = await recruitmentService.getInstance().position.getListPosition({
            predicate: "Id == @0",
            predicateParameters: [positionId],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $timeout(function (e) {
                $scope.form = res.object.data[0];
                // var grid = $("#grid").data("kendoGrid");
                // if(grid){
                //     setDataSourceForGrid('#grid',res.object.data);
                // }
            }, 0);
        }
    };

    $scope.listDepartment = [];
    async function getListDepartment() {
        var res = await settingService.getInstance().departments.getDepartments({
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: 10000,
            page: 1
        }).$promise;
        if (res.isSuccess) {
            $scope.listDepartment = res.object.data;
        }
    }

    $scope.listApplicantStatus = new kendo.data.ObservableArray([{
        id: '',
        name: ''
    }]);
    async function getAllApplicantStatus() {
        var res = await settingService.getInstance().recruitment.getAllApplicantStatus().$promise;
        if (res.isSuccess) {
            $scope.listApplicantStatus.pop();
            res.object.data.forEach(item => $scope.listApplicantStatus.push(item));
            $scope.statusesDataSource = res.object.data;
            var multiselect = $("#multiselect").data("kendoMultiSelect");
            if (multiselect) {
                multiselect.setDataSource($scope.statusesDataSource);
            }

        }
    }
    $scope.listAppreciationStatus = new kendo.data.ObservableArray([{
        id: '',
        name: ''
    }]);

    async function getAllAppreciationStatus() {
        let QueryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 500
        };

        // let model = {
        //     queryArgs: QueryArgs,
        //     type: ''
        // }
        var res = await settingService.getInstance().recruitment.getAppreciationList(QueryArgs).$promise;
        if (res.isSuccess) {
            $scope.listAppreciationStatus.pop();
            res.object.data.forEach(item => $scope.listAppreciationStatus.push(item));
        }
    }
    async function getAllListPositionForFilter() {
        var res = await recruitmentService.getInstance().position.getAllListPositionForFilter().$promise;
        if (res.isSuccess) {
            let positionForFilter = _.chain(res.object.data).groupBy("positionName")
                //.map((value, key) => ({ positionName: key, positions: value }))
                .map((value, key) => ({ positionName: key }))
                .value();
            $scope.widget.positionListForFilter.setDataSource(new kendo.data.DataSource({
                //data: res.object.data,
                data: positionForFilter,
            }));
        }
    }

    this.$onInit = async () => {
        if ($state.current.name === 'home.position.detail') {
            await getListDepartment();
            if ($stateParams.referenceValue) {
                await getPositionDetail();
            } else {

            }
        } else if ($state.current.name === 'home.position.item') {
            $rootScope.showLoading();
            await getAllApplicantStatus();
            await getAllAppreciationStatus();
            await getPositionDetail();
            $rootScope.hideLoading();
        } else {
            await getAllListPositionForFilter();
            await getListDepartment();
            await getAllApplicantStatus();
            await getAllAppreciationStatus();
        }
    };

    $scope.model = {};
    $scope.form = {};
    $scope.formValidate = {};

    $scope.errors = [];

    $scope.title = $stateParams.action.title.toUpperCase();

    if ($stateParams.statusId) {
        $scope.filter.statusDetail = $stateParams.statusId;
    }

    $rootScope.positionStatus = [{
        code: 0,
        name: 'ALL',
    },
    {
        code: 1,
        name: 'Opened',
        temp: `<a class="btn btn-sm default red-stripe" ng-click="actionPosition('close',dataItem)">Close</a>`
    },
    {
        code: 2,
        name: 'Closed',
        temp: `<a class="btn btn-sm default red-stripe" ng-click="actionPosition('open',dataItem)">Open</a>`
    },
    {
        code: 3,
        name: 'Draft',
        temp: `<a class="btn btn-sm default red-stripe" ng-click="actionPosition('delete',dataItem)">Delete</a>`
    },
    ];

    // Position
    $scope.dataPositions = {
        autoBind: true,
        placeholder: "",
        dataTextField: "positionName",
        //dataValueField: "id",
        dataValueField: "positionName",
        filter: "contains",
        dataSource: []
    }

    $scope.locationOptions = {
        autoBind: true,
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        filter: "contains",
        dataSource: {
            serverPaging: false,
            pageSize: 100,
            transport: {
                read: async function (e) {
                    var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                        name: 'WorkLocation',
                        parentCode: '',
                    }).$promise;
                    if (res.isSuccess && res.object.count > 0) {
                        e.success(res.object.data);
                    }
                }
            },
        },
        change: function (e) {
            $scope.form.locationName = e.sender.text();
        }
    }

    async function getPositions(option) {
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        if ($state.current.name == commonData.myRequests.Position) {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        }

        var res = await recruitmentService.getInstance().position.getListPosition(
            $scope.currentQuery
        ).$promise;
        if (res.isSuccess) {
            if (option && option.data.page > 1) {
                $scope.data = res.object.data.map((item, index) => {
                    return {
                        ...item,
                        no: index + option.data.take + 1
                    }
                });
            } else {
                $scope.data = res.object.data.map((item, index) => {
                    return {
                        ...item,
                        no: index + 1
                    }
                });
            }
            $scope.total = res.object.count;
        }

        if (option) {
            option.success($scope.data);
        } else {
            $scope.widget.positionList.dataSource.read();
        }
    }

    async function getPositionsThumbnail(option) {
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        // build here
        // $scope.currentQuery.predicate = '';
        // $scope.currentQuery.predicateParameters = [];

        var res = await recruitmentService.getInstance().position.getListPosition(
            $scope.currentQuery
        ).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });;
        }
        $scope.total = res.object.count;
        if (option) {
            option.success([$scope.data]);
        } else {
            $scope.widget.positionThumbnail.dataSource.read();
        }
    }

    $scope.positionGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getPositions(e);
                }
            },
            schema: {
                total: () => {
                    return $scope.total
                },
                data: () => { return $scope.data }
            }
        },
        sortable: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
        },
        columns: [{
            field: "no",
            title: "No.",
            width: "50px",
            locked: true,
        },
        {
            field: "statusName",
            title: "Status",
            width: "100px",
            locked: true,
            lockable: false,
            template: function (dataItem) {
                return $scope.positionStatus.find(i => i.code === dataItem.status).name;
            }
        },
        {
            field: "requestToHireId",
            title: "Request To Hire",
            width: "200px",
            locked: true,
            template: function (dataItem) {
                return `<a ui-sref="home.requestToHire.item({referenceValue: '${dataItem.requestToHireReferenceNumber}', id: '${dataItem.requestToHireId}'})" ui-sref-opts="{ reload: true }">${dataItem.requestToHireReferenceNumber}</a>`;
            }
        },
        {
            field: "referenceNumber",
            title: "Reference Number",
            width: "200px",
            locked: true,
            template: function (dataItem) {
                return "<a ui-sref='home.position.item({ id: dataItem.id })' ui-sref-active='active'>" + kendo.htmlEncode(dataItem.referenceNumber) + "</a>";
            }
        },
        {
            field: "positionName",
            title: "Position",
            width: "180px",
            locked: true,
        },
        // {
        //     field: "created",
        //     title: "Update Date",
        //     width: "120px",
        //     template: function (dataItem) {
        //         return moment(dataItem.created).format('DD/MM/YYYY HH:mm');
        //     }
        // },
        {
            field: "deptDivisionName",
            title: 'Department',
            width: "150px"
        },
        {
            field: "locationName",
            title: 'Location',
            width: "140px"
        },
        {
            field: "expiredDate",
            title: 'Expired Date',
            width: "150px",
            template: function (dataItem) {
                return moment(dataItem.created).add(dataItem.expiredDay, 'days').format('DD/MM/YYYY');
            }
        },
        {
            field: "applicantsCount",
            title: 'Applicants',
            width: "100px",
            template: function (dataItem) {
                return "<a ui-sref='home.position.item({ id: dataItem.id})' ui-sref-active='active'>" + kendo.htmlEncode(dataItem.applicantsCount) + "</a>";
            }
        },
        {
            field: "hiredApplicantsCount",
            title: 'Hired',
            width: "90px",
            template: function (dataItem) {
                return "<a ui-sref='home.position.item({ id: dataItem.id })' ui-sref-active='active'>" + kendo.htmlEncode(dataItem.hiredApplicantsCount) + "</a>";
            }
        },
        {
            field: "quantity",
            title: 'Required',
            width: "90px",
            template: function (dataItem) {
                //fix tam hien lan la 1 thao y/c chi Vy
                return "<span class='ng-binding'>1</span>"
            }
        },
        {
            field: "assignToFullName",
            title: 'Assignee',
            width: "220px"
        },
        {
            field: "actions",
            title: 'Actions',
            width: "200px",
            template: function (dataItem) {
                $scope.isVisibleRe_AssignBtn = $rootScope.currentUser ? (appSetting.role.SAdmin == $rootScope.currentUser.role
                    || appSetting.role.HRAdmin == $rootScope.currentUser.role
                    || !(($rootScope.currentUser.role & notInSadmin) == $rootScope.currentUser.role)
                    || (!$rootScope.currentUser.isStore && $rootScope.currentUser.isHR && $rootScope.currentUser.jobGradeValue == 4)) : false;
                if (dataItem.status != commonData.positionStatus.Closed) {
                    return `<div style="display: flex; gap: 10px;">
                                <button type="button"
                                    class="btn btn-primary rounded-3 btn-outline-blue poppins-regular display-9 py-1" ng-show="isVisibleRe_AssignBtn" ng-click="actionPosition('reassign',dataItem)">
                                    Re-Assign
                                </button>
                                <button type="button"
                                    class="btn btn-primary rounded-3 btn-outline-blue poppins-regular display-9 py-1" ng-click="actionPosition('close',dataItem)">
                                    Close
                                </button></div>`
                } else if (dataItem.status == commonData.positionStatus.Closed) {
                    return ` <button type="button"
                                    class="btn btn-primary rounded-3 btn-outline-blue poppins-regular display-9 py-1" ng-click="actionPosition('open',dataItem)">
                                    Open
                                </button>`
                } else {
                    return '';
                }
            }
        }
        ],
        dataBound: function (e) {
            for (var i = 0; i < e.sender._data.length; i++) {
                var item = e.sender._data[i];

                if (item.status === 2 || (item.applicants / item.required) <= (10 / 100)) {
                    e.sender.tbody.children().eq(i).css('background-color', '#F1DADA');
                    e.sender.lockedTable.children().eq(1).children().eq(i).css('background-color', '#F1DADA');
                } else {
                    if ((item.applicants / item.required) < (30 / 100)) {
                        e.sender.tbody.children().eq(i).css('background-color', '#F3ECC7');
                        e.sender.lockedTable.children().eq(1).children().eq(i).css('background-color', '#F3ECC7');
                    }
                }
            }
        },
    };

    $scope.actionPosition = async function (action, dataItem, $event = null) {
        if (action === 'edit') {
            $state.go('home.position.detail', ({
                referenceValue: dataItem.id
            }));
        } else if (action === 'close') {
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", `Do you want to close this Position ${dataItem.positionName}?`, 'Confirm');
            $scope.dialog.bind("close", async function (e) {
                if (e.data && e.data.value) {
                    var res = await recruitmentService.getInstance().position.changeStatus({
                        positionId: dataItem.id,
                        status: 2,
                    }).$promise;
                    if (res.isSuccess) {
                        getPositions();
                        Notification.success("Position is closed successfully.");
                    }
                }
            });

        } else if (action === 'open') {
            $scope.dialog = $rootScope.showConfirmDelete("OPEN", `Do you want to open this Position ${dataItem.positionName}?`, 'Confirm');
            $scope.dialog.bind("close", async function (e) {
                if (e.data && e.data.value) {
                    var res = await recruitmentService.getInstance().position.changeStatus({
                        positionId: dataItem.id,
                        status: 1,
                    }).$promise;
                    if (res.isSuccess) {
                        getPositions();
                        Notification.success("Position is opened successfully");
                    }
                }
            });

        } else if (action === 'delete') {
            $scope.dialog = $rootScope.showConfirm("DELETE", `Do you want to delete this Position ${dataItem.positionName}?`, 'Confirm');
            $scope.dialog.bind("close", async function (e) {
                if (e.data && e.data.value) {
                    var res = await recruitmentService.getInstance().position.changeStatus({
                        positionId: dataItem.id,
                        status: 3,
                    }).$promise;
                    if (res.isSuccess) {
                        getPositions();
                        Notification.success("Data Successfully Deleted.");
                    }
                }
            });
        }
        else if (action === 'reassign') {
            $scope.assigneeDetail.newAssigneeId = "";
            $scope.assigneeDetail.newAssigneeObject = {};
            $scope.assigneeDetail = Object.assign($scope.assigneeDetail, dataItem);
            $scope.positionId = dataItem.id;
            $scope.errorSameAssignee = '';
            $('#newAssignee_Id').data('kendoDropDownList').select("");
            let grid = $("#itemsTrackingLogOnGrid").data("kendoGrid");
            pageSize = grid.pager.dataSource._pageSize;
            setGridTrackingLog($scope.dataTrackingLog, '#itemsTrackingLogOnGrid', $scope.dataTrackingLog.length, 1, pageSize);
            let dialog = $("#dialogReassign").data("kendoDialog");
            dialog.title(`RE-ASSIGNEE: ${dataItem.referenceNumber}`);
            dialog.open();
            $rootScope.confirmDialogReassignee = dialog;
        }
    }

    $scope.saveReAssignee = async function () {
        let model = Object.assign($scope.assigneeDetail);
        let errors = $rootScope.validateInRecruitment(requiredFields, model);
        if (errors.length > 0) {
            let errorList = errors.map(x => {
                return x.controlName + " " + x.errorDetail;
            });
            Notification.error(`Some fields are required: </br>
            <ul>${errorList.join('<br/>')}</ul>`);

        }
        else if (!errors.length) {
            if (model.newAssigneeObject.sapCode == model.assignToSAPCode) {
                $scope.errorSameAssignee = 'The New Assignee is the same with the Current Assignee. You cannot save';
            } else {
                $scope.errorSameAssignee = '';
                let dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), `The Assignee of this Position is change to ${model.newAssigneeObject.sapCode}- ${model.newAssigneeObject.fullName}. Are you sure you want to change? `, $translate.instant('COMMON_BUTTON_CONFIRM'));
                dialog.bind("close", function (e) {
                    if (e.data && e.data.value) {
                        model.newAssignee = JSON.stringify(model.newAssigneeObject);
                        model.oldAssignee = {
                            id: model.assignToId ? model.assignToId : '',
                            fullName: model.assignToFullName ? model.assignToFullName : '',
                            sAPCode: model.assignToSAPCode ? model.assignToSAPCode : '',
                            email: model.assignToEmail ? model.assignToEmail : '',
                            departmentCode: model.assignToDepartmentCode ? model.assignToDepartmentCode : '',
                            departmentName: model.assignToDepartmentName ? model.assignToDepartmentName : '',
                            jobGradeId: model.assignToJobGradeId ? model.assignToJobGradeId : '',
                            jobGradeGrade: model.assignToJobGradeGrade ? model.assignToJobGrade : '',
                            jobGradeCaption: model.assignToJobGradeCaption ? model.assignToJobGradeCaption : ''
                        };
                        model.oldAssignee = model.oldAssignee ? JSON.stringify(model.oldAssignee) : '';
                        model.itemId = model.id;
                        let arg = {
                            userId: model.newAssigneeObject.id,
                            positionId: model.id,
                            trackingLogArgs: {
                                oldAssignee: model.oldAssignee,
                                newAssignee: model.newAssignee,
                                itemId: model.itemId
                            }
                        }
                        recruitmentService.getInstance().position.reAssign(arg).$promise.then(async function (result) {
                            if (result.isSuccess) {
                                Notification.success("Data Successfully Saved");
                                $scope.errorSameAssignee = '';
                                let grid = $("#itemsTrackingLogOnGrid").data("kendoGrid");
                                pageSize = grid.pager.dataSource._pageSize;
                                setGridTrackingLog($scope.dataTrackingLog, '#itemsTrackingLogOnGrid', $scope.dataTrackingLog.length, 1, pageSize);
                                $scope.assigneeDetail.assignToFullName = model.newAssigneeObject.fullName;
                                $scope.assigneeDetail.assignToSAPCode = model.newAssigneeObject.sapCode;
                                $scope.assigneeDetail.newAssigneeObject = {};
                                $scope.assigneeDetail.newAssigneeId = "";
                                $('#newAssignee_Id').data('kendoDropDownList').select("");
                                reloadPagePosition();
                                recruitmentService.getInstance().position.sendEmailToAssignee({ assigneeId: result.object.assigneeId, positionId: result.object.positionId }, null).$promise;
                            }
                        });
                    } else {

                    }
                });
            }
        }

    }

    $scope.itemsTrackingLogListGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getTrackingLogs(e);
                }
            },
            schema: {
                total: () => {
                    return $scope.totalTrackingLog
                },
                data: () => { return $scope.dataTrackingLog }
            }
        },
        sortable: false,
        pageable: {
            alwaysVisible: true,
            responsive: false,
            pageSizes: appSetting.pageSizesArray,
        },

        columns: [
            {
                field: "oldAssignee",
                title: "Old Assignee",
                width: "200px",
                template: function (dataItem) {
                    return `<span>{{dataItem.oldAssignee}}</span>`;
                }
            },
            {
                field: "newAssignee",
                title: "New Assignee",
                width: "200px",
                template: function (dataItem) {
                    return `<span>{{dataItem.newAssignee}}</span>`;
                }
            },
            {
                field: "modifiedBy",
                title: "Modified By",
                width: "150px",
                template: function (dataItem) {
                    return `<span>{{dataItem.modifiedBy}}</span>`;
                }
            },
            {
                field: "modified",
                title: "Modified Date",
                width: "100px",
                template: function (dataItem) {
                    if (dataItem && dataItem.modified !== null) {
                        return moment(dataItem.modified).format('DD/MM/YYYY HH:mm');
                    }
                    return '';
                }
            }
        ]
    };

    function setGridTrackingLog(data, idGrid, total, pageIndex, pageSizes) {
        let grid = $(idGrid).data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            transport: {
                read: async function (e) {
                    await getTrackingLogs(e);
                }
            },
            pageSize: pageSizes,
            page: pageIndex,
            serverPaging: true,
            schema: {
                total: function () {
                    return $scope.totalTrackingLog;
                }
            },
        });
        if (grid) {
            grid.setDataSource(dataSource);
        }
    }

    async function getTrackingLogs(option) {
        let grid = $("#itemsTrackingLogOnGrid").data("kendoGrid");
        if ($scope.positionId) {
            let args = buildArgs($scope.positionId, $scope.currentQueryReassignee.page, appSetting.pageSizeDefault);
            if (option) {
                $scope.currentQueryReassignee.limit = option.data.take;
                $scope.currentQueryReassignee.page = option.data.page;
            }
            $scope.currentQueryReassignee.predicate = args.predicate;
            $scope.currentQueryReassignee.predicateParameters = args.predicateParameters;

            var res = await recruitmentService.getInstance().trackingLogs.getListTrackingLog(
                _.clone($scope.currentQueryReassignee)
            ).$promise;
            if (res.isSuccess) {
                $scope.dataTrackingLog = [];
                res.object.data.forEach(x => {
                    x.newAssignee = `${(JSON.parse(x.newAssignee)).sapCode} - ${(JSON.parse(x.newAssignee)).fullName}`;
                    x.oldAssignee = `${(JSON.parse(x.oldAssignee)).sAPCode} - ${(JSON.parse(x.oldAssignee)).fullName}`;
                    $scope.dataTrackingLog.push(x);
                });
            }
            $scope.totalTrackingLog = res.object.count;
            option.success($scope.dataTrackingLog);
            grid.refresh();
        }
    }

    function buildArgs(itemId, pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        predicate.push('ItemId == @0');
        predicateParameters.push(itemId);
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }

    $scope.newAssigneeOptions = {
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
        },
        change: function (e) {
            if (e.sender.dataItem()) {
                $timeout(function () {
                    $scope.assigneeDetail.newAssigneeObject = {
                        id: e.sender.dataItem().id,
                        fullName: e.sender.dataItem().fullName,
                        sapCode: e.sender.dataItem().sapCode
                    };
                }, 0);
            }
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
        }
        if (option) {
            option.success($scope.dataUser);
        }
    }

    $scope.onOpenedDialog = function (event) {
        console.log(event);
    }

    $scope.onClosedDialog = function (event) {
        console.log(event);
    }


    $scope.dialogOptions = {
        width: "450px",
        title: "Cập nhật trạng thái",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "<p>Bạn có muốn đổi trạng thái không?<p>",
        actions: [{
            text: 'Đồng ý',
            primary: true,
            action: function (e) {
                return true;
            },
        },
        {
            text: 'Bỏ qua',
            action: function (e) {
                return true;
            },
        }
        ],
        close: function (e) {
        }
    };

    $scope.positionGridThumbnailOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getPositionsThumbnail(e);
                }
            },
            schema: {
                total: () => {
                    return $scope.total
                },
                // data: () => { return $scope.data }
            }
        },
        sortable: false,
        //pageable: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
        },
        columns: [{
            field: "thumbnail",
            title: "Thumbnail",
            width: "100px",
            template: function (dataItem) {
                return `
                            <div style="display: flex; flex-wrap: wrap;">
                                <div ng-repeat="position in dataItem" style="flex: 0 0 20%; margin-bottom: 20px; display: flex; justify-content: center;">
                                    <div class="k-card" style="width: 96%; border-color: #38b6ff;">
                                        <div class="k-card-body text-right" style="background-color: #fff;">
                                            <h5 class="k-card-title" style="color: black; margin-bottom: 0px">{{position.deptDivisionName}}</h5>
                                            <p class="mrb10" style="color: black;">{{position.locationName}}</p>
                                            <a style="color: black; text-decoration: underline; display: block;" ui-sref="home.position.item({ id: position.id })" ui-sref-active="active">Applicant ({{position.applicantsCount}})</a>
                                            <a style="color: black; text-decoration: underline; display: block" ui-sref="home.position.item({ id: position.id })" ui-sref-active="active">Hired ({{position.hiredApplicantsCount}})</a>
                                            <p style="color: black;">Required ({{position.quantity}})</p>
                                        </div>
                                        <div class="k-card-actions" style="background-color: #66a8fc; display: flex; justify-content: space-between; align-items: center;">
                                            <span ng-click="gotoListPosition(position.id)" class="k-button k-flat k-primary col-auto mr-auto" style="color: white;">View More</span>
                                            <span ng-click="gotoListPosition(position.id)" style="color: white;" class="k-icon k-i-hyperlink-open-sm"></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;
            }
        },],
    };

    async function getPositionsDetail(option) {
        if (option) {
            $scope.currentQueryPositionDetail.limit = option.data.take;
            $scope.currentQueryPositionDetail.page = option.data.page;
        }
        buildArg();
        let positionId = '';
        var position = await recruitmentService.getInstance().position.getPositionById({ id: $stateParams.id }).$promise;
        if (position.isSuccess) {
            positionId = position.object.id;
        }
        $scope.currentQueryPositionDetail.predicate = ($scope.currentQueryPositionDetail.predicate ? $scope.currentQueryPositionDetail.predicate + ' and ' : $scope.currentQueryPositionDetail.predicate) + `(PositionId = @${$scope.currentQueryPositionDetail.predicateParameters.length})`;
        $scope.currentQueryPositionDetail.predicateParameters.push(positionId);

        var res = await recruitmentService.getInstance().position.getListPositionDetail(
            $scope.currentQueryPositionDetail
        ).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                element.selected = false;
                $scope.data.push(element);
            });;
        }
        $scope.total = res.object.count;
        if (option) {
            option.success($scope.data);
        } else {
            $scope.widget.positionDetailList.dataSource.read();
        }
    }

    $scope.applicantStatusOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "id",
        //template: '#: code # - #: name #',
        //valueTemplate: '#: code #',
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.listApplicantStatus,
    };
    $scope.appreciationStatusOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.listAppreciationStatus,
    }

    $scope.onchangeStatus = function (e, item) {
        item.applicantStatusId = e.sender.value();
        item.applicantStatusName = e.sender.text();
    }

    $scope.onDataBound = function (e, item) {
        e.sender.value(item.applicantStatusId);
    }
    $scope.onDataBoundAppreciation = function (e, item) {
        e.sender.value(item.appreciationId);
    }
    $scope.positionDetailGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getPositionsDetail(e);
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
        // pageable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
        },
        //scrollable: true,
        columns: [{
            field: "fullName",
            title: "Applicant Name",
            width: "150px",
            template: function (dataItem) {
                return "<a ui-sref='home.applicant.item({ referenceValue: dataItem.referenceNumber, id: dataItem.id })' ui-sref-active='active' ui-sref-opts='{ reload: true }'>" + kendo.htmlEncode(dataItem.fullName) + "</a>";
            }
        },
        {
            field: "email",
            title: "Email",
            width: "150px"
        },
        {
            field: "mobile",
            title: 'Phone',
            width: "110px"
        },
        {
            field: "applicantStatusName",
            title: 'Status',
            width: "140px",
            template: function (dataItem) {
                if (dataItem.selected || !dataItem.id) {
                    return `
                        <select id="status" name="status" style="width: 90%;" kendo-drop-down-list k-options="applicantStatusOptions" 
                        k-on-change="onchangeStatus(kendoEvent, dataItem)" k-on-data-bound="onDataBound(kendoEvent, dataItem)"></select>
                        `;
                } else {
                    if (dataItem.id) {
                        if (dataItem.applicantStatusName) {
                            return `${dataItem.applicantStatusName}`;
                        } else {
                            return `Initial`;
                        }

                    } else {
                        return '';
                    }
                }
            },

        },
        {
            field: "appreciationName",
            title: 'Appreciation',
            width: "140px",
            template: function (dataItem) {
                if (dataItem.selected || !dataItem.id) {
                    return `<select id="appreciation" name="appreciation" style="width: 90%;" kendo-drop-down-list k-options="appreciationStatusOptions" 
                        k-on-change="onchangeAppreciation(kendoEvent, dataItem)" k-on-data-bound="onDataBoundAppreciation(kendoEvent, dataItem)"></select>`;
                } else {
                    if (dataItem.appreciationName) {
                        return `${dataItem.appreciationName}`;
                    }
                    return '';
                }
            },
        },
        {
            field: "positionAssignToFullName",
            title: 'In Charge Person',
            width: "150px"
        },
        {
            field: "sapReviewStatus",
            title: 'SAP Review Status',
            width: "100px"
        },
        {
            field: "created",
            title: 'Created Date',
            width: "100px",
            template: function (dataItem) {
                return moment(dataItem.created).format('DD/MM/YYYY');
            }
        },
        {
            title: 'Actions',
            width: "130px",
            template: function (dataItem) {
                if (dataItem.id) {
                    if (dataItem.selected) {
                        return `
                            <button type="button" title = "Save" class="btn-border-upgrade btn-save-upgrade" ng-click="excuteAction('save', dataItem, $event)"></button>
                            <button type="button" title = "Cancel" class="btn-border-upgrade btn-cancel-upgrade" ng-click="excuteAction('cancel', dataItem)"></button>
                        `;
                    } else {
                        return `
                            <button type="button" title = "Edit" class="btn-border-upgrade btn-edit-upgrade" ng-if="form.status != 2" ng-click="excuteAction('edit', dataItem)"></button>
                        `;
                    }
                }
            }
        }],
        dataBound: function (e) {
            var rows = e.sender.tbody.children();
            for (var j = 0; j < rows.length; j++) {
                var row = $(rows[j]);
                var dataItem = e.sender.dataItem(row);

                var inactive = dataItem.get("inactive");
                if (inactive) {
                    row.addClass("inactive-row");
                }
            }
        },
    };

    $scope.excuteAction = async function (action, dataItem, $event = null) {
        $scope.currentDataItem = dataItem;
        var grid = $("#grid").data("kendoGrid");
        switch (action) {
            case commonData.gridActions.Create:
                break;
            case commonData.gridActions.Save:
                if (dataItem.applicantStatusName === 'Signed Offer') {
                    dataItem.isSignedOffer = true;
                    dataItem.appreciationId = dataItem.appreciationId ? dataItem.appreciationId : null;
                    Object.assign($scope.model, dataItem);
                    await getEmpSubGroupByEmpGroupCode($scope.model.employeeGroupCode);
                    if ($scope.model.additionalPositionCode) {
                        if (_.findIndex($scope.positionItemsDisplay, x => { return x.code == $scope.model.additionalPositionCode }) == -1) {
                            $scope.positionItemsDisplay.push({ code: $scope.model.additionalPositionCode, name: $scope.model.additionalPositionName });
                        }
                    }
                    let positionList = $('#positionId').data("kendoDropDownList");
                    if (positionList) {
                        positionList.setDataSource($scope.positionItemsDisplay);
                        positionList.value($scope.model.additionalPositionCode);
                    }
                    additionalApplicantDialogClick();
                } else {
                    dataItem.isSignedOffer = false;
                    dataItem.startDate = null;
                    dataItem.amployeeGroupCode = null;
                    dataItem.amployeeSybGroupCode = null;
                    dataItem.reasonHiringCode = null;
                    dataItem.additionalPositionCode = null;
                    dataItem.additionalPositionName = null;
                    dataItem.applicantId = $scope.model.id;
                    var res = await recruitmentService.getInstance().applicants.updatePositionDetail(
                        dataItem
                    ).$promise;
                    if (res.isSuccess) {
                        $scope.widget.positionDetailList.dataSource.read();
                        // Notification here.
                        Notification.success("Data successfully saved.");

                    }
                }
                break;
            case commonData.gridActions.Edit:
                // dataItem['selected'] = true;
                //grid.refresh();

                var result = validateEdit();
                if (result) {
                    Notification.error("Please save selected item before edit other item");
                }
                else {
                    dataItem.selected = true;
                    var item = $scope.data.find(x => x.id === dataItem.id);
                    item.selected = true;
                    grid.refresh();
                }
                break;
            case commonData.gridActions.Remove:
                break;
            case commonData.gridActions.Cancel:
                grid.dataSource.read();
                break;
        }
    }

    function validateEdit() {
        var result = false;
        $scope.data.forEach(item => {
            if (item.selected === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    $scope.changeToListView = function () {
        if ($stateParams.isMyRequest) {
            $state.go('home.position.myRequests', ({
                type: $scope.isList
            }));
        } else {
            $state.go('home.position.allRequests', ({
                type: $scope.isList
            }));
        }

        // $scope.hasListView = true;
        // $scope.hasGridView = false;
        // $scope.showSearch = false;
    };

    $scope.changeToGridView = function () {
        if ($stateParams.isMyRequest) {
            $state.go('home.position.myRequests', ({
                type: $scope.isThumbNail
            }));
        } else {
            $state.go('home.position.allRequests', ({
                type: $scope.isThumbNail
            }));
        }
        // $scope.hasListView = false;
        // $scope.hasGridView = true;
        // $scope.showSearch = false;
    };

    $scope.deptDivisionDataSource = [{
        name: 'IT',
        code: 1
    },
    {
        name: 'Stationery -Sports- Bike - Division',
        code: 2
    },
    {
        name: 'Perishable – Fish & Meat Division',
        code: 3
    },
    {
        name: 'Daily - Dairy - Group',
        code: 4
    }
    ];

    $scope.statusDetailOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.statusesDataSource,
    }

    $scope.quantityOptions = {
        min: 0,
        max: 99999,
        decimals: 0,
        restrictDecimals: true,
        format: '#',
    }

    $scope.budgetOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "id",
        valuePrimitive: false,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: [{
            id: 'true',
            name: 'Budget'
        },
        {
            id: 'false',
            name: 'Not Budget'
        },
        ],
        change: function (e) {
            //getQuantity();
            let numerictextbox = $("#quantity").data("kendoNumericTextBox");
            if ($scope.form.deptDivisionId && e.sender.value() == 'true') {
                if ($scope.form.currentQuantity) {
                    numerictextbox.max($scope.form.currentQuantity);
                    numerictextbox.value($scope.form.currentQuantity);
                }
                else {
                    numerictextbox.enable(false);
                    numerictextbox.value(null);
                }

            } else {
                numerictextbox.max(10000);
                numerictextbox.enable(true);
            }
        }
    }

    $scope.departmentDataSource = [{
        id: '123',
        name: 'Hữu Lộc',
    },
    {
        id: '456',
        name: 'Hữu Phát',
    },
    ];

    $scope.hasApplicantDataSource = [{
        id: '123',
        name: 'Has Applicant',
    },
    {
        id: '456',
        name: 'No Applicant',
    },
    ];

    $scope.toggleSearch = function () {
        $scope.showSearch = !$scope.showSearch;
        if ($scope.showSearch) {
            setDataDepartment(allDepartments);
        }
    }

    $scope.onClickSearch = function () {
        //let filter = [];
        //if ($scope.filter.keyword) {
        //    filter.push(`PositionName.Contains("$scope.filter.keyword")`);
        //}
        //if ($scope.filter.department) {
        //    filter.push(`DeptDivision.Name.Contains("$scope.filter.department")`);
        //}
    }

    $scope.onClickReset = async function () {
        $scope.initFilter();
        var dropdownlist = $("#positionDropdownlist").data("kendoDropDownList");
        dropdownlist.select(-1);
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }
        if ($scope.hasListView) {
            await getPositions();
        }
        else {
            await getPositionsThumbnail();
        }
        clearSearchTextOnDropdownTree("departmentSearch");
        setDataDepartment(allDepartments);
        //await getPositions();
    }

    $scope.save = async function (status, formItem) {
        if (event)
            event.preventDefault();

        if ($scope.formValidate.validator.validate()) {
            $scope.errors = [];

            if (status === 'Draft')
                $scope.form.status = 3;
            else if (status === 'Opened')
                $scope.form.status = 1;

            var res;
            if ($scope.form.id) {
                res = await recruitmentService.getInstance().position.updatePosition(
                    $scope.form
                ).$promise;
            } else {
                res = await recruitmentService.getInstance().position.createNewPosition(
                    $scope.form
                ).$promise;
            }

            if (res.isSuccess) {
                if (!$scope.form.id && res.object) {
                    $scope.form.id = res.object.id;
                    $scope.form.referenceNumber = res.object.referenceNumber;
                }
                Notification.success("Data successfully saved.");
                $state.go('home.position.myRequests', {}, {
                    reload: true
                });
            } else {
                Notification.errors(res.messages[0]);
            }
        } else {
            //Ngan

            $scope.errors = customValidateForm(formItem, $scope.form);
            //end
            $scope.errors = [...$scope.errors, ...Object.entries($scope.formValidate.validator._errors)];
            if ($scope.errors.length > 0) {
                return;
            }
        }
    }

    function customValidateForm(formItem, form) {
        var errors = [];
        angular.forEach(formItem.$$controls, function (control) {
            if (!form[control.$name]) {
                if (control.$name == 'Quantity') {
                    if (form.hasBudget == 'true') {
                        errors.push(
                            ["Quantity", "is block. You have to settup Headcount for Budget Request"]
                        );
                    }
                }

            }
        });
        return errors;
    }

    $scope.cancel = function (state) {
        $state.go(state);
    }

    $scope.onCancel = function () {
        $state.go('home.position.item', ({
            id: 1
        }));
    }

    $scope.onClose = function () {
        $state.go('home.position.item', ({
            id: 1
        }));
    }

    $scope.gotoListPosition = function (id) {
        $state.go('home.position.item', ({
            id: id,
        }));
    }

    $scope.onClickEdit = function () {
        $state.go('home.position.detail', ({
            id: 1
        }));
    }

    $scope.onchangeAppreciation = function (e, item) {
        item.appreciationId = e.sender.value();
        item.appreciationName = e.sender.text();
    }
    $scope.onchangeApplicantPosition = function (e) {
        $scope.additionalPositionCode = e.sender.value();
        $scope.additionalPositionName = e.sender.text();
    }
    $scope.fromDateChanged = function () {
        $scope.minDate = new Date($scope.filter.fromDateString);
    };

    $scope.toDateChanged = function () {
        $scope.maxDate = new Date($scope.filter.toDateString);
    };

    $scope.cancelDetail = function () {
        $state.go('home.position.myRequests', {
            isThumbNail: $stateParams.isThumbNail
        }, {
            reload: true
        });
    }

    $scope.actions = {
        additionalApplicantDialogClick: additionalApplicantDialogClick
    }

    $scope.showMessages = function (value) {
    }

    $scope.additionalApplicantDialogOptions = {
        width: "600px",
        title: "EMPLOYEE INFO",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        actions: [{
            text: "Save",
            action: function (e) {
                $scope.widget.dialogDetail.close();
                return false;
            },
            primary: true
        }],
        close: async function (e) {
            if (e.userTriggered) {
                // do nothing
            } else {
                $scope.model.additionalPositionCode = $scope.additionalPositionCode;
                $scope.model.additionalPositionName = $scope.additionalPositionName;
                $scope.model.applicantId = $scope.model.id;
                var res = await recruitmentService.getInstance().applicants.updatePositionDetail(
                    $scope.model
                ).$promise;
                if (res.isSuccess) {
                    $scope.widget.positionDetailList.dataSource.read();
                    Notification.success("Data successfully submited.");
                }
                if ($scope.model.f2Form) {
                    // send request to edoc01 to create f2 form
                    let result = await ssgexService.getInstance().edoc1.createF2Form().$promise;
                    if (result && result.object && result.isSuccess) {
                        if (!result.object.isSuccess) {
                            Notification.error('Create F2 Form: ' + result.object.messages);
                        } else {
                            window.open(
                                result.object.itemLink,
                                '_blank'
                            );
                        }

                    } else {
                        Notification.error(result.Messages);
                    }
                }
            }
        }
    };

    $scope.setDefaultValueDate = function () {
        $scope.default = {
            startDateObject: null
        };
    };

    $scope.empGroupSetItems = new kendo.data.ObservableArray([{
        code: '',
        name: ''
    }]);
    $scope.empSubGroupSetItems = new kendo.data.ObservableArray([{
        code: '',
        name: ''
    }]);
    $scope.reasonHireSetItems = new kendo.data.ObservableArray([{
        code: '',
        name: ''
    }]);

    async function getEmpGroupSet() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'EmployeeGroupCode',
            parentCode: '',
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.empGroupSetItems.pop();
            res.object.data.forEach(item => $scope.empGroupSetItems.push(item));
        }
    }

    async function getEmpSubGroupSet(e) {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'EmployeeSubgroup',
            parentCode: e.sender.value(),
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.empSubGroupSetItems.pop();
            res.object.data.forEach(item => $scope.empSubGroupSetItems.push(item));
        }
    }
    async function getEmpSubGroupByEmpGroupCode(code) {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'EmployeeSubgroup',
            parentCode: code
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.empSubGroupSetItems.pop();
            res.object.data.forEach(item => $scope.empSubGroupSetItems.push(item));
            let empSubGroupInstance = $('#empSubGroup').data('kendoDropDownList');
            empSubGroupInstance.setDataSource($scope.empSubGroupSetItems);
            empSubGroupInstance.value($scope.model.employeeSybGroupCode);

        }
    }

    async function getreasonHireSet(e) {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'ReasonHiring',
            parentCode: '',
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.reasonHireSetItems.pop();
            res.object.data.forEach(item => $scope.reasonHireSetItems.push(item));
        }
    }
    $scope.positionItems = new kendo.data.ObservableArray([{
        code: '',
        name: ''
    }]);
    async function getPosition() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'JobTitle',
            parentCode: '',
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.positionItems.pop();
            $scope.positionItemsDisplay = [];
            res.object.data.forEach(item => { if ($scope.positionItemsDisplay.length < 100) { $scope.positionItemsDisplay.push(item) } $scope.positionItems.push(item) });
        }
    }


    $scope.isModalVisible = false;

    $scope.toggleModal =  async function (show) {
        $scope.isModalVisible = show;
        if ($scope.isModalVisible) {
            if (!$scope.filter.department || $scope.filter.department == '')
                await getDepartmentByFilter({ filter: { value: '' } });
        }
    };

    $scope.optionGroup = {
        positionOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            template: '#: code # - #: name #',
            valueTemplate: '#: name #',
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: $scope.positionItemsDisplay,
            filtering: function (e) {
                let result = _.filter($scope.positionItems, x => {
                    if (e.filter && e.filter.value) {
                        return x.name.toLowerCase().includes(e.filter.value.toLowerCase())
                            || x.code.toLowerCase().includes(e.filter.value.toLowerCase())
                            || e.filter.value.toLowerCase().includes(x.code.toLowerCase())
                            || e.filter.value.toLowerCase().includes(x.name.toLowerCase())
                    }
                });
                if (result.length) {
                    $scope.positionItemsDisplay = [];
                    result.forEach(item => { if ($scope.positionItemsDisplay.length < 100) { $scope.positionItemsDisplay.push(item) } });
                    let positionList = $('#positionId').data("kendoDropDownList");
                    if (positionList) {
                        positionList.setDataSource($scope.positionItemsDisplay);
                    }
                }

            },
        },
        empGroupOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            template: '#: code # - #: name #',
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: $scope.empGroupSetItems,
            change: async function (e) {
                //$scope.model.empGroupCode = e.sender.value();
                await getEmpSubGroupSet(e);
            },
            filtering: $rootScope.dropdownFilter,
        },
        empSubGroupOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            template: '#: code # - #: name #',
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: $scope.empSubGroupSetItems,
            change: async function (e) {
                $scope.model.empSubGroupCode = e.sender.value();
            },
            filtering: $rootScope.dropdownFilter,
        },
        reasonHireOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            template: '#: code # - #: name #',
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: $scope.reasonHireSetItems,
            //change: async function (e) {
            //    $scope.model.reasonHireCode = e.sender.value();
            //},
            filtering: $rootScope.dropdownFilter,
        },
    }

    function additionalApplicantDialogClick() {
        if ($scope.widget.dialogDetail) {
            $scope.widget.dialogDetail.open();
            $rootScope.confirmDialogEmployeeInfo = $scope.widget.dialogDetail;
            $scope.setDefaultValueDate();
        }
    }

    async function ngOnit() {
        if ($stateParams.type === commonData.typePosition.isThumbNail) {
            $scope.hasListView = false;
            $scope.hasGridView = true;
        }
        await getEmpGroupSet();
        await getreasonHireSet();
        await getPosition();
    }
    var _columsSearch = ['ReferenceNumber.contains(@0)', 'LocationName.contains(@1)', 'AssignTo.FullName.contains(@2)', 'PositionName.contains(@3)', 'DeptDivision.Name.contains(@4)', 'RequestToHire.ReferenceNumber.contains(@5)']

    $scope.search = async function () {
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        var option = $scope.currentQuery;
        if ($scope.filter.keyword) {
            option.predicate = `(${_columsSearch.join(" or ")})`;
            for (let index = 0; index < _columsSearch.length; index++) {
                option.predicateParameters.push($scope.filter.keyword);
            }
            option.predicateParameters.push($scope.filter.keyword);
        }
        if (parseInt($scope.checkStatus.value) > 0) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Status = @${option.predicateParameters.length}`;
            var stt = parseInt($scope.checkStatus.value)
            option.predicateParameters.push(stt);
        }
        if ($scope.filter.department) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `DeptDivision.Id = @${option.predicateParameters.length}`;
            option.predicateParameters.push($scope.filter.department);
        }
        if ($scope.filter.position) {
            //option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Id = @${option.predicateParameters.length}`;
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `PositionName = @${option.predicateParameters.length}`;
            option.predicateParameters.push($scope.filter.position);
        }
        if ($scope.filter.hasApplicant) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `!Applicants.Any() `;
        }
        // if ($scope.filter.assignment === true) {
        //     option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `AssignToId != null`;
        //     option.predicateParameters.push($scope.filter.assignment);
        // }
        // else if ($scope.filter.assignment === false) {
        //     option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `AssignToId == null`;
        //     option.predicateParameters.push($scope.filter.assignment);
        // }
        $scope.isSearch = true;
        $scope.currentQueryExport = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }
        $scope.currentQueryExport = option;

        reloadGrid();
        $scope.toggleModal(false);
    }
    function reloadGrid() {
        if ($scope.hasListView) {
            let grid = $("#positionGrid").data("kendoGrid");
            if (grid) {
                grid.dataSource.read();
                if ($scope.showSearch) {
                    grid.dataSource.page(1);
                }
            }

        }
        else {
            let grid = $("#positionThumbGrid").data("kendoGrid");
            grid.dataSource.read();
        }
    }
    $scope.onCheckStatus = async function () {
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        if (!$scope.showSearch) {
            var option = $scope.currentQuery;
            if (parseInt($scope.checkStatus.value) > 0) {
                var stt = parseInt($scope.checkStatus.value);
                option.predicate = "Status = @0";
                option.predicateParameters.push(stt);
            }
            reloadGrid();
        } else {
            await $scope.search();
        }
    }
    ngOnit();

    $scope.currentQueryExport = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }

    $scope.export = async function () {
        //let pagesize = $("#positionGrid").data("kendoGrid").dataSource.pageSize();

        $scope.currentQueryExport = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: $scope.total ? $scope.total : 100000,
            page: 1
        }
        var option = $scope.currentQueryExport;
        if ($scope.filter.keyword) {
            option.predicate = `(${_columsSearch.join(" or ")})`;
            for (let index = 0; index < _columsSearch.length; index++) {
                option.predicateParameters.push($scope.filter.keyword);
            }
            option.predicateParameters.push($scope.filter.keyword);
        }
        if (parseInt($scope.checkStatus.value) > 0) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Status = @${option.predicateParameters.length}`;
            var stt = parseInt($scope.checkStatus.value)
            option.predicateParameters.push(stt);
        }
        if ($scope.filter.department) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `DeptDivision.Id = @${option.predicateParameters.length}`;
            option.predicateParameters.push($scope.filter.department);
        }
        if ($scope.filter.position) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `PositionName = @${option.predicateParameters.length}`;
            option.predicateParameters.push($scope.filter.position);
        }
        if ($scope.filter.hasApplicant) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Applicants.Any() `;
        }

        //let option = $scope.currentQueryExport;
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.POSITION }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.filterPosition = {
        keyword: '',
        fromDate: null,
        toDate: null,
        statusDetail: ''
    }

    $scope.currentQueryPositionDetail = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }

    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }

    function reloadPagePosition() {
        $scope.currentQuery.predicate = '';
        $scope.currentQuery.predicateParameters = [];
        let grid = $("#positionGrid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }

    $scope.exportPositionDetail = async function () {

        $scope.currentQueryPositionDetail = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }
        var option = $scope.currentQueryPositionDetail;
        if ($scope.filterPosition.keyword) {
            option.predicate = `(${columsSearch.join(" or ")})`;
            for (let index = 0; index < columsSearch.length; index++) {
                option.predicateParameters.push($scope.filterPosition.keyword);
            }
        }

        if ($scope.filterPosition.statusDetail) {
            let predicateStatusArray = [];
            for (let i = 0; i < $scope.filterPosition.statusDetail.length; i++) {
                predicateStatusArray.push(`${applicantStatus} = @${option.predicateParameters.length}`);
                option.predicateParameters.push($scope.filterPosition.statusDetail[i]);
            }
            predicateStatus = `(${predicateStatusArray.join(" or ")})`;
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + predicateStatus;
        }

        if ($scope.filterPosition.fromDate) {
            //var date = moment($scope.filter.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created >= @${option.predicateParameters.length}`;
            option.predicateParameters.push(moment($scope.filterPosition.fromDate).format('YYYY-MM-DD'));
        }
        if ($scope.filterPosition.toDate) {
            //var date = moment($scope.filter.toDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created < @${option.predicateParameters.length}`;
            option.predicateParameters.push(moment($scope.filterPosition.toDate).add(1, 'days').format('YYYY-MM-DD'));
        }

        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(PositionId = @${option.predicateParameters.length})`;
        option.predicateParameters.push($stateParams.id);

        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.POSITIONDETAIL }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    var columsSearch = ['FullName.contains(@0)', 'Email.contains(@1)', 'Appreciation.Name.contains(@2)']
    var applicantStatus = 'ApplicantStatusId'
    $scope.onClickSearchPositionDetails = function () {
        loadPageOne();
    }

    function buildArg() {
        $scope.currentQueryPositionDetail = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }
        var option = $scope.currentQueryPositionDetail;
        if ($scope.filterPosition.keyword) {
            option.predicate = `(${columsSearch.join(" or ")})`;
            for (let index = 0; index < columsSearch.length; index++) {
                option.predicateParameters.push($scope.filterPosition.keyword);
            }
        }

        if ($scope.filterPosition.statusDetail.length > 0) {
            let predicateStatusArray = [];
            for (let i = 0; i < $scope.filterPosition.statusDetail.length; i++) {
                predicateStatusArray.push(`${applicantStatus} = @${option.predicateParameters.length}`);
                //option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(ApplicantStatusId = @${option.predicateParameters.length})`;
                option.predicateParameters.push($scope.filterPosition.statusDetail[i]);
            }
            predicateStatus = `(${predicateStatusArray.join(" or ")})`;
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + predicateStatus;
        }

        if ($scope.filterPosition.fromDate) {
            //var date = moment($scope.filter.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created >= @${option.predicateParameters.length}`;
            option.predicateParameters.push(moment($scope.filterPosition.fromDate).format('YYYY-MM-DD'));
        }
        if ($scope.filterPosition.toDate) {
            //var date = moment($scope.filter.toDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created < @${option.predicateParameters.length}`;
            option.predicateParameters.push(moment($scope.filterPosition.toDate).add(1, 'days').format('YYYY-MM-DD'));
        }

        $scope.currentQueryPositionDetail = option;
    }

    $scope.onClickResetPositionDetails = function () {
        $scope.currentQueryPositionDetail = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };

        $scope.filterPosition = {
            keyword: '',
            fromDateObject: null,
            toDateObject: null,
            statusDetail: ''
        }

        loadPageOne();
    }

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.showSearch && data.state == $state.current.name) {
            $scope.onClickSearchPositionDetails();
        }
    });
});