var ssgApp = angular.module('ssg.probationItemModule', ["kendo.directives"]);
ssgApp.controller('probationItemController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, dataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, $compile, btaService) {
    $scope.title = $stateParams.id ? 'PROBATION EVALUATION' + ': ' + $stateParams.referenceValue : 'NEW ITEM: PROBATION EVALUATION';

    $scope.model = {};
    $scope.typeProposal = commonData.proposalOfHeadOfDepartment;
    $scope.typeApraisee = commonData.apraisee;
    $scope.typeHr = commonData.hr;
    const reducer = (accumulator, currentValue) => accumulator + currentValue;
    this.$onInit = async function () {
        $scope.perm = true;
        $scope.sapCodesDataSource = {
            //optionLabel: "Please Select...",
            dataTextField: 'sapCode',
            dataValueField: 'id',
            template: showCustomSapCodeTitle,
            valueTemplate: '#: sapCode #',
            valuePrimitive: true,
            autoBind: true,
            filter: "contains",
            dataSource: {
                serverFiltering: true,
                transport: {
                    read: async function (e) {
                        await getUsers(e);
                    }
                }
            },
            change: async function (e) {
                if (e.sender.text()) {
                    let SAPCode = e.sender.text();
                    let result = await settingService.getInstance().users.seachEmployee({
                        SAPCode: SAPCode
                    }).$promise;
                    if (result.isSuccess) {
                        console.log(result.object)
                        $scope.model.userId = result.object.id;
                        $scope.model.sapCode = result.object.sapCode;
                        $scope.model.fullName = result.object.fullName;
                        $scope.model.department = result.object.divisionName ? result.object.divisionName : result.object.deptName;
                        $scope.model.position = result.object.jobTitle;
                        $scope.model.dateJoined = moment(result.object.startDate).format(appSetting.sortDateFormat);
                        $scope.model.probationPeriodFrom = new Date(result.object.startDate);
                        $scope.$applyAsync();
                    }
                }
            }
        };

        var probationGridColumns = [
            {
                field: "competency",
                // headerTemplate: $translate.instant('COMMON_NO'),
                headerTemplate: 'Competency',
                width: "150px",
                editable: function (e) {
                    return false;
                },
            },
            {
                headerTemplate: `<div class="text-center">CRITERIA</div>`,
                width: "200px",
                columns: [{
                    field: "importantLevelFixed",
                    headerTemplate: '<div class="text-center">Important Level Fixed (1 - 3)</div>',
                    width: 100,
                    template: function (dataItem) {
                        if(!dataItem.isTotal) {
                            return `<input kendo-numeric-text-box k-min="1" k-max="3" k-format="'#,0'" data-role="numerictextbox" aria-valuemin="1" aria-valuemax="3" class="k-input w100" ng-model="dataItem.importantLevelFixed" readonly/>`
                        }
                        else {
                            return `<span>{{dataItem.importantLevelFixed}}</span>`
                        }
                    }
                }, {
                    field: "requirePerformanceLevel",
                    headerTemplate: '<div class="text-center">Require Performance Level (3 - 5)</div>',
                    width: 100,
                    template: function (dataItem) {
                        if(!dataItem.isTotal) {
                            return `<input kendo-numeric-text-box k-min="3" k-max="5" k-format="'#,0'" data-role="numerictextbox" aria-valuemin="3" aria-valuemax="5" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.requirePerformanceLevel" style="width: 100%"/>`;
                        }
                        else {
                            return `<span>{{dataItem.requirePerformanceLevel}}</span>`
                        }
                    }
                }]
            },
            {
                headerTemplate: `<div class="text-center">RESULT</div>`,
                width: "100px",
                columns: [{
                    field: "achievedPerformance",
                    headerTemplate: '<div class="text-center">Achieved Performance (3 - 5)</div>',
                    width: 100,
                    template: function (dataItem) {
                        if(!dataItem.isTotal) {
                            return `<input  kendo-numeric-text-box k-min="3" k-max="5" k-format="'#,0'" data-role="numerictextbox" aria-valuemin="3" aria-valuemax="5" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.achievedPerformance" style="width: 100%"/>`;
                        }
                        else {
                            return `<span>{{dataItem.achievedPerformance}}</span>`
                        }
                    }
                }]
            },
            {
                headerTemplate: `<div class="text-center">POINT</div>`,
                width: "100px",
                columns: [{
                    field: "requiredPoint",
                    headerTemplate: '<div class="text-center">Required</div>',
                    width: 100,
                    template: function (dataItem) {
                        if (!dataItem.isTotal) {
                            return `<input class="k-input w100" ng-model="dataItem.requiredPoint" readonly/>`
                        }
                        else {
                            return `<span>{{dataItem.requiredPoint}}</span>`
                        }
                    }
                }, {
                    field: "achievedPoint",
                    headerTemplate: '<div class="text-center">Achieved</div>',
                    width: 100,
                    template: function (dataItem) {
                        if (!dataItem.isTotal) {
                            return `<input class="k-input w100" ng-model="dataItem.achievedPoint" readonly/>`
                        }
                        else {
                            return `<span>{{dataItem.achievedPoint}}</span>`
                        }
                    }
                }]
            },
            {
                headerTemplate: $translate.instant('COMMON_ACTION'),
                width: "100px",
                template: function (dataItem) {
                    if (dataItem.isDelete) {
                        return `<a class="btn btn-sm default red-stripe" ng-click="deleteRecord(dataItem)">{{'COMMON_BUTTON_DELETE'|translate}}</a>`;
                    }
                    else {
                        return `<span></span>`
                    }
                }
            },
        ]

        $scope.probationGridOptions = {
            dataSource: {
                data: $scope.probations
            },
            sortable: false,
            pageable: false,
            columns: probationGridColumns
        }
        if ($stateParams.id) {
            await getProbationById();
        }
        else {
            await getPerformanceTemplate();
        }
    }

    totalPoint = {
        competency: 'Total Point',
        importantLevelFixed: 0,
        requirePerformanceLevel: 0,
        achievedPerformance: 0,
        requiredPoint: 0,
        achievedPoint: 0,
        isDelete: false,
        isTotal: true
    }

    $scope.probations = []
    function showCustomSapCodeTitle(model) {
        if (model.sapCode) {
            return `${model.sapCode} - ${model.fullName}`
        } else {
            return `${model.fullName}`
        }
    }

    async function getUsers(option) {
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var arg = {
            predicate: "(sapcode.contains(@0) or fullName.contains(@1)) and IsActivated = @2",
            predicateParameters: [filter, filter, true],
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
                    $scope.dataUser.push({ id: $scope.detail.userId, fullName: $scope.detail.fullName, sapCode: $scope.detail.employeeCode });
                }
                $scope.selectedUserId = '';
            }
        }
        if (option) {
            option.success($scope.dataUser);
        }
    }

    $scope.fromDateChanged = function () {
        if ($scope.model.probationPeriodFrom) {
            $("#toDate_id").data("kendoDatePicker").min(moment($scope.model.probationPeriodFrom, 'DD/MM/YYYY').toDate());
            if ($scope.model.probationPeriodTo && (moment($scope.probationPeriodTo, 'DD/MM/YYYY').dayOfYear()) < (moment($scope.model.probationPeriodFrom, 'DD/MM/YYYY').dayOfYear())) {
                $scope.model.probationPeriodTo = null;
            }
        }
    }

    $scope.deleteRecord = function (dataItem) {
        $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
        $scope.dialog.bind("close", function (e) {
            if (e.data && e.data.value) {
                let currentDataOnGrid = getDataGrid('#grid');
                _.remove(currentDataOnGrid, function (item) {
                    return item.no === dataItem.no;
                });
                currentDataOnGrid.forEach((item, index) => {
                    item.no = index + 1;
                });
                $timeout(function () {
                    setDataSourceForGrid(currentDataOnGrid, '#grid');
                    refreshTotal();
                    Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                }, 0);
            }
        });
    }

    function refreshTotal() {
        arrayTotal.importantLevelFixed = [];
        arrayTotal.requirePerformanceLevel = [];
        arrayTotal.achievedPerformance = [];
        arrayTotal.requiredPoint = [];
        arrayTotal.achievedPoint = [];
        let dataGrid = getDataGrid('#grid');
        dataGrid.forEach(item => {
            if (!item.isTotal) {
                if (item.importantLevelFixed) {
                    arrayTotal.importantLevelFixed.push(parseInt(item.importantLevelFixed));
                }
                if (item.requirePerformanceLevel) {
                    arrayTotal.requirePerformanceLevel.push(parseInt(item.requirePerformanceLevel));
                }
                if (item.achievedPerformance) {
                    arrayTotal.achievedPerformance.push(parseInt(item.achievedPerformance));
                }
                if (item.requiredPoint) {
                    arrayTotal.requiredPoint.push(parseInt(item.requiredPoint));
                }
                if (item.achievedPoint) {
                    arrayTotal.achievedPoint.push(parseInt(item.achievedPoint));
                }
            }
        });
        //
        if (arrayTotal.importantLevelFixed.length) {
            totalPoint.importantLevelFixed = arrayTotal.importantLevelFixed.reduce(reducer);
        }
        if (arrayTotal.requirePerformanceLevel.length) {
            totalPoint.requirePerformanceLevel = arrayTotal.requirePerformanceLevel.reduce(reducer);
        }
        if (arrayTotal.achievedPerformance.length) {
            totalPoint.achievedPerformance = arrayTotal.achievedPerformance.reduce(reducer);
        }
        if (arrayTotal.requiredPoint.length) {
            totalPoint.requiredPoint = arrayTotal.requiredPoint.reduce(reducer);
        }
        if (arrayTotal.achievedPoint.length) {
            totalPoint.achievedPoint = arrayTotal.achievedPoint.reduce(reducer);
        }
        //
        if (dataGrid.length) {
            dataGrid.pop();
            dataGrid.push(totalPoint);
            setDataSourceForGrid(dataGrid, '#grid');
        }
    }

    $scope.disableProposal = true;
    $scope.changeProposalOfHeadOfDepartment = function () {
        if ($scope.model.proposalOfHeadOfDepartment == commonData.proposalOfHeadOfDepartment.OtherInstruction) {
            $scope.disableProposal = false;
        }
        else {
            $scope.disableProposal = true;
            $scope.model.otherInstructionProposal = '';
        }
    }

    $scope.disableAppraisee = true;
    $scope.changeApraisee = function () {
        if ($scope.model.appraiseeCommonent == commonData.apraisee.DisagreeProposal) {
            $scope.disableAppraisee = false;
        }
        else {
            $scope.disableAppraisee = true;
            $scope.model.disagreeAppraisee = '';
        }
    }

    $scope.disableHrToBe = true;
    $scope.disableHrProbation = true;
    $scope.changeHr = function () {
        if ($scope.model.hr == commonData.hr.ToBeConfimed) {
            $scope.disableHrToBe = false;
            $scope.disableHrProbation = true;
            $scope.model.hrProbationPeriod = '';
        }
        else if ($scope.model.hr == commonData.hr.ProbationPeriod) {
            $scope.disableHrToBe = true;
            $scope.disableHrProbation = false;
            $scope.model.hrTobe = '';
        }
    }

    arrayTotal = {
        importantLevelFixed: [],
        requirePerformanceLevel: [],
        achievedPerformance: [],
        requiredPoint: [],
        achievedPoint: []
    }

    async function getPerformanceTemplate() {
        var res = await cbService.getInstance().probationEvaluation.getPerformanceTemplateByDate({ dataCheck: new Date() }, null).$promise;
        if (res.isSuccess) {
            let count = 1;
            $scope.probations = res.object.performanceTemplateDetails;
            $scope.probations.forEach(item => {
                item['competency'] = item.criteria;
                item['isTotal'] = false;
                arrayTotal.importantLevelFixed.push(item.importantLevelFixed);
                item['no'] = count;
                count++;
            });
            //push total vô grid
            totalPoint.importantLevelFixed = arrayTotal.importantLevelFixed.reduce(reducer);
            $scope.probations.push(totalPoint);
            //set dataSource
            setDataSourceForGrid($scope.probations, '#grid');
        }
    }

    function setDataSourceForGrid(data, id) {
        let grid = $(id).data("kendoGrid");
        grid.setDataSource(data);
    }

    function getDataGrid(id) {
        let gridRoom = $(id).data("kendoGrid");
        return gridRoom.dataSource._data.toJSON();
    }

    $scope.change = function (type, dataItem) {
        let value = 0;
        switch (type) {
            case 'requirePerformanceLevel':
                value = parseInt(dataItem.requirePerformanceLevel);
                dataItem.requiredPoint = dataItem.importantLevelFixed * value;
                break;
            case 'achievedPerformance':
                value = parseInt(dataItem.achievedPerformance);
                dataItem.achievedPoint = dataItem.importantLevelFixed * value;
                break;
            default:
                break;
        }
        refreshTotal();
        if (totalPoint.requiredPoint && totalPoint.achievedPoint) {
            $scope.model.rateOfCompetency = totalPoint.requiredPoint / totalPoint.achievedPoint;
        }
    }

    async function getProbationById() {
        var res = await cbService.getInstance().probationEvaluation.getProbationDetailById({ id: $stateParams.id }, null).$promise;
        if (res.isSuccess) {
            $scope.model = res.object;
            $scope.model.evaluations.forEach(item => {
                item['isTotal'] = false;
                arrayTotal.importantLevelFixed.push(parseInt(item.importantLevelFixed));
                arrayTotal.requirePerformanceLevel.push(parseInt(item.requirePerformanceLevel));
                arrayTotal.achievedPerformance.push(parseInt(item.achievedPerformance));
                arrayTotal.requiredPoint.push(parseInt(item.requiredPoint));
                arrayTotal.achievedPoint.push(parseInt(item.achievedPoint));
            });
            //tinh total
            totalPoint.importantLevelFixed = arrayTotal.importantLevelFixed.reduce(reducer);
            totalPoint.requirePerformanceLevel = arrayTotal.requirePerformanceLevel.reduce(reducer);
            totalPoint.achievedPerformance = arrayTotal.achievedPerformance.reduce(reducer);
            totalPoint.requiredPoint = arrayTotal.requiredPoint.reduce(reducer);
            totalPoint.achievedPoint = arrayTotal.achievedPoint.reduce(reducer);
            //
            $scope.changeHr();
            $scope.changeApraisee();
            $scope.changeProposalOfHeadOfDepartment();
            //
            //set dataSource
            $scope.model.evaluations.push(totalPoint);
            setDataSourceForGrid($scope.model.evaluations, '#grid');
            if (!$scope.perm) {
                disableElement(true);
            }


        }
    }

    $rootScope.$watch(function () { return dataService.permission }, function (newValue, oldValue) {

        if ($stateParams.id) {
            $scope.perm = (2 & dataService.permission.right) == 2;
        } else {
            $scope.perm = true;
        }

    }, true);

    $scope.errors = [];
    $scope.save = async function () {
        $scope.errors = [];
        $scope.errors = validate();
        if ($scope.errors.length == 0) {
            $scope.model.evaluations = getDataGrid('#grid');
            $scope.model.evaluations.pop();
            var res = await cbService.getInstance().probationEvaluation.saveProbationEvaluation($scope.model).$promise;
            if (res.isSuccess) {
                $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
                Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
            }
        }
    }

    function validate() {
        let errors = [];
        //
        if (!$scope.model.userId) {
            errors.push({ controlName: "SAP Code", message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
        }
        //
        return errors;
    }
})