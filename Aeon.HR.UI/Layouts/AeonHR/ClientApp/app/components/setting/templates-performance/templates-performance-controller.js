
var ssgApp = angular.module('ssg.templatesPerformanceModule', ['kendo.directives']);
ssgApp.controller('templatesPerformanceController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, masterDataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, settingService) {
    $scope.currentQuery = {
        Predicate: "",
        PredicateParameters: [],
        Order: "EffectivePeriodTo asc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    };

    $scope.typePerformanceTemplate = {
        dataTextField: "name",
        dataValueField: "value",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            data: translateStatus($translate, commonData.typePerformanceTemplate)
        }
    }

    this.$onInit = function () {
        $scope.searchText = "";
        $scope.title = 'Performance Template List';
        $scope.total = 0;
        $scope.templateGrid = [];
        $scope.model = {};
        $scope.data = [];
        $scope.templateData = [];

        $scope.query = {
            type: '',
            effectivePeriodFrom: null,
            effectivePeriodTo: null,
            name: '',
        }
        $scope.requiredFields = [{
            fieldName: 'name',
            title: "Name"
        },
        {
            fieldName: 'effectivePeriodFrom',
            title: "Effective Date From"
        },
        {
            fieldName: 'type',
            title: "Type"
        },
        ];

        $scope.editingTemplatesPerformance = {};
        $scope.templatesPerformanceListGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getPerformanceTemplateList(e);
                    }
                },
                schema: {
                    total: () => {
                        return $scope.total
                    },
                    data: () => {
                        return $scope.data
                    }
                },

            },
            sortable: false,
            autoBind: true,
            valuePrimitive: false,
            editable: {
                mode: "inline",
                confirmation: false
            },
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArray
            },
            columns: [{
                field: "id",
                hidden: true
            },
            {
                field: "name",
                // title: "Name",
                headerTemplate: "Name",
                width: "300px",
            },
            {
                field: "effectiveDate",
                // title: "Effective Date",
                headerTemplate: "Effective Date",
                width: "150px",
                template: function (dataItem) {
                    return `<span>${moment(dataItem.effectivePeriodFrom).format(appSetting.sortDateFormat)} - ${moment(dataItem.effectivePeriodTo).format(appSetting.sortDateFormat)}</span>`
                }

            },
            {
                field: "type",
                // title: "Type",
                headerTemplate: "Type",
                width: "150px",
                template: function (dataItem) {
                    let type = _.find(commonData.typePerformanceTemplate, function (item) {
                        return item.value == dataItem.type;
                    })
                    return `<span>${type.name}</span>`
                }
            },
            {
                width: "140px",
                // title: "Actions",
                headerTemplate: "Actions",
                template: function (dataItem) {
                    return `<a ng-click="edit(dataItem)" class="btn btn-sm default blue-stripe">Edit</a>
                        <a  ng-click="deleteRow(dataItem)" class="btn btn-sm default red-stripe">Delete</a>`
                },
            }
            ]
        };
        $scope.templatePerformanceGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                schema: {
                    total: () => {

                        return $scope.templateData.length
                    },
                    data: () => {
                        return $scope.templateData
                    }
                },
            },
            sortable: false,
            autoBind: true,
            valuePrimitive: false,
            editable: {
                mode: "inline",
                confirmation: false
            },
            // pageable: {
            //     alwaysVisible: true,
            //     pageSizes: appSetting.pageSizesArray
            // },
            columns: [{
                field: "id",
                hidden: true
            },
            {
                field: "criteria",
                // title: "Criteria",
                headerTemplate: "Criteria",
                width: "300px",
                template: function (dataItem) {
                    return `<input class="k-textbox w100" id="criteria${dataItem.no}" name="criteria${dataItem.no}" ng-model="dataItem.criteria"/>`;
                }
            },
            {
                field: "importantLevelFixed",
                // title: "Important Level Fixed",
                headerTemplate: "Important Level Fixed",
                width: "150px",
                template: function (dataItem) {
                    return `<input kendo-numeric-text-box k-min="1" k-max="3" k-format="'#,0'" style="width: 100%;" id="importantLevelFixed${dataItem.no}" name="importantLevelFixed${dataItem.no}" ng-model="dataItem.importantLevelFixed"/>`
                }
            },
            {
                field: "isDeleted",
                // title: "Can Be Delete",
                headerTemplate: "Can Be Delete",
                width: "150px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isDelete" name="isDelete{{dataItem.no}}"
                    id="isDelete{{dataItem.no}}" class="k-checkbox" style="width: 100%;" ng-change='changeDeleteCheck(dataItem)'/>
                    <label class="k-checkbox-label cbox" for="isDelete{{dataItem.no}}"></label>`
                }
            },
            {
                width: "140px",
                title: "Actions",
                template: function (dataItem) {
                    return `<a class='btn btn-sm default red-stripe' ng-click="deleteRow(dataItem)" >Delete</a>`
                },
            }
            ]
        }

    };


    let requiredFieldsGrid = [
        {
            fieldName: "criteria",
            title: "Criteria"
        },
        {
            fieldName: "importantLevelFixed",
            title: "Important Level Fixed"
        }
    ];


    $scope.addRow = async function (item) {
        let currentTemplateGrid = $("#templatePerformanceGrid").data("kendoGrid");
        let nValue = {
            no: currentTemplateGrid._data.length + 1,
            criteria: '',
            importantLevelFixed: '',
            isDelete: false,
        }
        $scope.templateData.push({ nValue })
        let grid = $("#templatePerformanceGrid").data("kendoGrid");
        grid.dataSource.add(nValue);
    };

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
            text: "SAVE",
            action: function (e) {
                var isContinue = validateDataInGrid($scope.model, $scope.requiredFields, [], Notification);
                let dataGrid = getDataGrid('#templatePerformanceGrid');
                if (dataGrid.length == 0) {
                    Notification.error(`Table not invalid`);
                    isContinue = false;
                }
                if (isContinue) {
                    let errors = [];
                    dataGrid.forEach(item => {
                        errors = errors.concat(validationForTable(item, requiredFieldsGrid));
                    });
                    if (errors.length == 0) {
                        save();
                        return true;
                    }
                    else {
                        let errorList = errors.map(x => {
                            return x.controlName + " " + x.errorDetail;
                        });
                        Notification.error(`Some fields are required: </br>
                            <ul>${errorList.join('<br/>')}</ul>`);
                    }
                }

                return false;
            },
            primary: true
        }]
    };

    async function save() {
        $scope.model.performanceTemplateDetails = getDataGrid('#templatePerformanceGrid');
        let res = await settingService.getInstance().performanceTemplate.savePerformanceTemplate($scope.model).$promise;
        if (res.isSuccess) {
            refresh();
            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
        } else {
            Notification.success("Error System");
        }
        return res;
    }

    // async function test() {
    //     let res = await settingService.getInstance().performanceTemplate.getPerformanceTemplates($scope.model).$promise;
    //     if(res.isSuccess) {
    //         let templateList = res.object.data.forEach(item => {
    //             console.log(moment(item.effectivePeriodTo).format(appSetting.sortDateFormat))
    //         });
    //         return templateList;
    //     }
    //     return res;
    // }


    $scope.createTemplate = async function (model) {
        $scope.templateGrid = [];
        $scope.model = {};
        let grid = $('#templatePerformanceGrid').data("kendoGrid");
        grid.setDataSource([]);
        // set title cho cái dialog
        let dialog = $("#dialog_Detail").data("kendoDialog");
        dialog.title("Performance Template");
        dialog.open();
        $rootScope.confirmDialogCreateTemplate = dialog;
    }

    async function getPerformanceTemplateList(option) {


        // let args = buildArgs($scope.currentQuery.page, appSetting.pageSizeDefault);

        // if (option) {
        //     $scope.currentQuery.limit = option.data.take;
        //     $scope.currentQuery.page = option.data.page;
        // }
        // $scope.currentQuery.predicate = args.predicate;
        // $scope.currentQuery.predicateParameters = args.predicateParameters;
        // if ($rootScope.currentUser && $state.current.name == 'home.probationEvalution.myRequests') {
        //     $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
        //     $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        // }

        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        if ($scope.searchText) {
            $scope.currentQuery.Predicate = "Name.contains(@0)";
            $scope.currentQuery.PredicateParameters = [$scope.searchText];
        } else {
            $scope.currentQuery.Predicate = "";
            $scope.currentQuery.PredicateParameters = [];
        }

        if (option) {
            let res = await settingService.getInstance().performanceTemplate.getPerformanceTemplates($scope.currentQuery).$promise;
            if (res && res.isSuccess) {
                $scope.data = res.object.data;
                $scope.total = res.object.count;
                option.success($scope.data);
            }
        }
        else {
            let grid = $("#templatesPerformanceListGrid").data("kendoGrid");
            grid.dataSource.read();
            grid.dataSource.page($scope.currentQuery.page);
        }
    }

    $scope.search = function () {
        getPerformanceTemplateList();
        textTemporary = $scope.searchText;
    }

    $scope.ifEnter = async function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            // Do that thing you finally wanted to do
            getPerformanceTemplateList();
        }

    }

    textTemporary = '';
    $scope.export = async function () {
        let option = {
            predicate: "",
            predicateParameters: [],
            order: "EffectivePeriodTo asc",
            limit: 100000,
            page: 1
        }
        option.predicate = "Name.contains(@0)";
        option.predicateParameters.push(textTemporary);
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.PERFORMANCETEMPLATE }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    function getDataGrid(id) {
        let gridRoom = $(id).data("kendoGrid");
        return gridRoom.dataSource._data.toJSON();
    }

    function validationForTable(model, requiredFields) {
        let errors = [];
        requiredFields.forEach(field => {
            if (!model[field.fieldName]) {
                errors.push({
                    fieldName: field.fieldName,
                    controlName: field.title,
                    errorDetail: 'is required',
                });
            }
        });
        return errors;
    }

    // async function validateDate() {
    //     let res = await settingService.getInstance().performanceTemplate.getPerformanceTemplates($scope.model).$promise
    //     if(res.isSuccess) {
            
    //     }
    // }

    $scope.mDatePicker = {
        fromDateMin: new Date(2000, 0, 1, 0, 0, 0),
        fromDateMax: new Date(2030, 0, 1, 0, 0, 0),
        toDateMin: new Date(2000, 0, 1, 0, 0, 0),
        toDateMax: new Date(2030, 0, 1, 0, 0, 0),
    }

    $scope.fromDateChanged = function () {
        if ($scope.model.effectivePeriodFrom) {
            $("#toDate_id").data("kendoDatePicker").min(moment($scope.model.effectivePeriodFrom, 'DD/MM/YYYY').toDate());
            if ($scope.model.effectivePeriodTo && (moment($scope.model.effectivePeriodTo, 'DD/MM/YYYY').dayOfYear()) < (moment($scope.model.effectivePeriodFrom, 'DD/MM/YYYY').dayOfYear())) {
                $scope.model.effectivePeriodTo = null;
            }
        }
    }

    $scope.toDateChanged = function () {
        if (moment($scope.model.effectivePeriodTo, 'DD/MM/YYYY').isValid()) {
            $scope.mDatePicker.fromDateMax = moment($scope.model.effectivePeriodTo, 'DD/MM/YYYY').format('MM/DD/YYYY');
        } else {
            $scope.mDatePicker.fromDateMax = '12/31/2030';
        }
    }

    function refresh() {
        let grid = $("#templatesPerformanceListGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
    }

    performanceId = '';
    $scope.deleteRow = function (dataItem) {
        performanceId = dataItem.id;
        $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
        $scope.dialog.bind("close", confirm);
    }

    confirm = async function (e) {
        if (e.data && e.data.value) {
            var res = await settingService.getInstance().performanceTemplate.deletePerformanceTemplate({ id: performanceId }, null).$promise;
            let currentDataOnGrid = $("#templatePerformanceGrid").data("kendoGrid").dataSource._data;
            if (res.isSuccess) {
                refresh();
                Notification.success("Data Sucessfully Deleted");
            }
            else if (currentDataOnGrid) {
                let resetId = 1;
                var index = _.findIndex(currentDataOnGrid, x => {
                    if (x.id) {
                        return x.id == $scope.itemDetailId
                    } else if (x.uid) {
                        return x.uid == $scope.itemDetailId
                    }
                });
                currentDataOnGrid.splice(index, 1);
                Notification.success("Data Sucessfully Deleted");
            } else {
                Notification.error('Error');
            }
        }
    }

    $scope.edit = async function (dataItem) {
        // set title cho cái dialog
        let dialog = $("#dialog_Detail").data("kendoDialog");
        dialog.title("Edit Performance Template");
        dialog.open();
        var res = await settingService.getInstance().performanceTemplate.getPerformanceTemplateById({ id: dataItem.id }, null).$promise;
        if (res.isSuccess) {
            $scope.model = res.object;
            let grid = $('#templatePerformanceGrid').data("kendoGrid");
            let count = 0;
            $scope.model.performanceTemplateDetails.forEach(item => {
                item['no'] = count;
                count++;
            });
            grid.setDataSource($scope.model.performanceTemplateDetails);
        }
    }
});

