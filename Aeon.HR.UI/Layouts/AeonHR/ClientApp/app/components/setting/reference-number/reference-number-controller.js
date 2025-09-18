
var ssgApp = angular.module("ssg.settingReferenceNumberModule", [
    "kendo.directives"
]);
ssgApp.controller("settingReferenceNumberController", function (
    $rootScope,
    $scope,
    $location,
    $stateParams,
    appSetting,
    commonData,
    Notification,
    settingService
) {
    // create a message to display in our view
    var ssg = this;
    $scope.advancedSearchMode = false;
    $scope.DateOfBirth = new Date();
    $rootScope.isParentMenu = false;
    $scope.title = 'Reference Number';

    // phần khai báo chung
    $scope.actions = {
        save: save,
        cancel: cancel,
        edit: edit,
        deleteRecord: deleteRecord,
        create: create,
        ifEnter: ifEnter 
    };

    $scope.numeric =  {
        format: '',
        min: 0
    }

    $scope.keyword = '';
    $scope.dataReferenceNumber = [];
    //API
    function GetAllReferenceNumber() {
        settingService.getInstance().referenceNumbers.getReferenceNumber({}).$promise.then(function(result) {
            $scope.dataReferenceNumber = result.object.data;
            initGrid();
        });
    }

    function ifEnter($event) {
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
        
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            QueryArgs.predicateParameters.push($scope.keyword);
            // Do that thing you finally wanted to do
            settingService.getInstance().referenceNumbers.searchReferenceNumberByName(model).$promise.then(function(result) {
                $scope.dataReferenceNumber = result.object.data;
                initGrid();
            });
        }
    }
    //

    
    $scope.ReferenceNumberOptions = {
        dataSource: {
            data: $scope.dataReferenceNumber,
            // pageSize: 5,
            schema: {
                model: { id: "Id" }
            }
        },
        sortable: false,
        editable: {
            mode: "inline",
        },
        // pageable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
        },
        columns: [
            {
                field: "no",
                title: "NO.",
                width: "50px",
                editor: function (container, options) {
                    $(`<label>${options.model[options.field]}</label>`).appendTo(
                        container
                    );
                }
            },
            {
                field: "moduleType",
                title: "MODULE",
                width: "200px",
                template: function (dataItem) {
                    return `<span>{{dataItem.moduleType}}</span>`;
                }
            },
            {
                field: "currentNumber",
                title: "CURRENT NUMBER",
                width: "150px",
                template: function (dataItem) {
                    
                    if (dataItem.select) {
                        return `
                        <input kendo-numeric-text-box ng-model="dataItem.currentNumber" k-options="numeric" style="width: 100%;" />
                        `
                    } else {
                        return `<span>{{dataItem.currentNumber}}</span>`;
                    }
                }
            },
            {
                field: "isNewYearReset",
                title: "is New Year Reset",
                width: "150px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `
                        <input type="checkbox" id="{{dataItem.id}}" ng-model="dataItem.isNewYearReset" ng-click="change(dataItem)" class="k-checkbox">
                            <label class="k-checkbox-label" for="{{dataItem.id}}"></label>
                        `
                    } else {
                        return `<input type="checkbox" id="{{dataItem.id}}" ng-model="dataItem.isNewYearReset" class="k-checkbox" disabled="true">
                        <label class="k-checkbox-label" for="{{dataItem.id}}"></label>`;
                    }
                }
            },
            {
                field: "formula",
                title: "Formula",
                width: "200px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" placeholder="Ex:FirstCode-{AutoNumber:}\\{Year}" name="formula" ng-model="dataItem.formula"/>`;
                    } else {
                        return `<span>{{dataItem.formula}}</span>`;
                    }
                }
            },
            {
                title: "ACTIONS",
                width: "90px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `
                        <a class="btn btn-sm default green-stripe " ng-click="actions.save(dataItem)">Save</a>
                        <a class="btn btn-sm default " ng-click="actions.cancel(dataItem)">Cancel</a>
                `;
                    } else {
                        return `
                        <a class="btn btn-sm default blue-stripe " ng-click="actions.edit(dataItem)">Edit</a>
                `;
                    }
                }
            }
        ]
    };

    function create(model) {
        save(model);
    };

    $scope.errors = [];

    let requiredFields = [
        {
            fieldName: "currentNumber",
            title: "Current Number"
        },
        {
            fieldName: "formula",
            title: "Formula"
        },
    ];

    function save(model) {
        var isContinue = validateDataInGrid(model, requiredFields, [], Notification);
        if(isContinue) {
            settingService.getInstance().referenceNumbers.editReferenceNumber(model).$promise.then(function(result) {
                if(result.isSuccess) {
                    Notification.success("Save success");
                    GetAllReferenceNumber();
                    return true;
                }
            });
            //
        }
    }

    $scope.change = function(data) {
        if(!data.isNewYearReset) {
            data.currentNumber = 1;
        }
    }

    // function cancel(model) {
    //     changeSelectById(model.id);
    // }

    function cancel(model) {
        model['select'] = false
        var item = $scope.dataReferenceNumber.find(x => x.id === model.id);
        item.select = false;
        model.currentNumber = item.currentNumber;
        model.isNewYearReset = item.isNewYearReset;
        model.formula = item.formula;
        let grid = $("#grid").data("kendoGrid");
        grid.refresh();
    }

    // function edit(model) {
    //     changeSelectById(model.id, true);
    // }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            model['select'] = true
            var item = $scope.dataReferenceNumber.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#grid").data("kendoGrid");
            grid.refresh();
        }
    }

    function validateEdit() {
        var result = false;
        $scope.dataReferenceNumber.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    function deleteRecord(model) {
        Notification.success("Your record is deleted");
        refreshGrid("#grid", true);
    }

    function refreshGrid(idGrid, isCallApi = false) {
        let grid = $(idGrid).data("kendoGrid");
        if (isCallApi) {
            initGrid();
        }
        grid.refresh();
    }

    function changeSelectById(id, valueSelect = false) {
        let grid = $("#grid").data("kendoGrid");
        $scope.dataReferenceNumber.forEach(item => {
            if(item.id === id) {
                item.select = valueSelect;
            }
        });
        let dataSourceReferenceNumber = new kendo.data.DataSource({
            data: $scope.dataReferenceNumber,
            // pageSize: appSetting.pageSizeDefault,
            pageSize: $scope.dataReferenceNumber.length
        });
        grid.setDataSource(dataSourceReferenceNumber);
    }
    $scope.count = 0;
    $scope.referenceNumbers = commonData.referenceNumbers;
    function initGrid() {
        var count = 0;
        let grid = $("#grid").data("kendoGrid");
        if($scope.dataReferenceNumber.length !== 0) {
            $scope.dataReferenceNumber.forEach(x => {
                x["select"] = false;
                x["no"] = count + 1;
                count = x["no"];
            });
            mapValue();
            let dataSourceReferenceNumber = new kendo.data.DataSource({
                data: $scope.dataReferenceNumber,
                // pageSize: $rootScope.pageSizeDefault
                pageSize: 20
            });
            grid.setDataSource(dataSourceReferenceNumber);
        }
    }

    function mapValue() {
        $scope.dataReferenceNumber.forEach(item => {
            commonData.referenceNumbers.forEach(x => {
                if(item.moduleType === parseInt(x.code)) {
                    item.moduleType = x.name;
                }
            });
        });
    }

    function ngInit(){
        GetAllReferenceNumber();
    }

    ngInit();

    $scope.$on('$locationChangeStart', function (event, next, current) {
        $scope.errors = [];
    });

    $scope.$on("$viewContentLoaded", function () {
    });

});
