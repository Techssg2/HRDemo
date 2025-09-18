var ssgApp = angular.module('ssg.btaPolicyModule', ['kendo.directives']);
ssgApp.controller('btaPolicyController', function ($rootScope, $scope, $location, appSetting, Notification, commonData, $stateParams, settingService, fileService, $timeout) {

    // create a message to display in our view
    var ssg = this;
    $scope.advancedSearchMode = false;
    $rootScope.isParentMenu = false;
    $scope.title = $stateParams.action.title;

    // general values
    $scope.actions = {
        save: save,
        cancel: cancel,
        edit: edit,
        //deleteRecord: deleteRecord,
        //create: create
    };

    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Modified desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };
    $scope.numeric = {
        format: '',
        min: 0
    }

    $scope.total = 0;
    $scope.keyword = '';
    $scope.dataBTAPolicy = [];

    //sort array

    function SortByName(a, b) {
        var aName = a.jobGradeCaption.toLowerCase();
        var bName = b.jobGradeCaption.toLowerCase();
        return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
    }
    var record = 0;
    $scope.TypeDepartment = "Store";
    //API
    async function getListBTAPolicy(option) {
        record = 0;
        var result = await settingService.getInstance().btaPolicy.getBTAPolicyByDepartment({ typeDepartment: $scope.TypeDepartment }).$promise;
        console.log(result);
        if (result.isSuccess) {
            $scope.dataBTAPolicy = result.object.data;
            $scope.dataBTAPolicy.forEach(x => {
                x["select"] = false;
                x['edit'] = false;
                var jobGrade = $scope.jobGradeDataSource.find(y => y.id == x.jobGradeId);
                if (jobGrade != null & typeof (jobGrade) !== "undefined") {
                    x['jobGradeCaption'] = jobGrade.caption;
                }
            });
            $scope.dataBTAPolicy.sort(SortByName);
            $scope.total = result.object.count;
            if (option) {
                option.success($scope.dataBTAPolicy);
            }
            else {
                let grid = $("#grid").data("kendoGrid");
                grid.dataSource.read();
                grid.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    // Get list
    $scope.jobGradeOptions = {
        dataTextField: "caption",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.jobGradeDataSource
    };

    $scope.jobGradeDataSource = [];
    async function getJobGrade() {
        let model = {
            Predicate: "",
            PredicateParameters: [],
            Order: "Grade asc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        }

        let result = await settingService.getInstance().jobgrade.getJobGradeList(model).$promise;
        if (result.isSuccess) {
            $scope.jobGradeDataSource = result.object.data;
        }
    }

    $scope.BTAPolicyOption = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await ngInit();
                    await getListBTAPolicy(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataBTAPolicy }
            }
        },
        sortable: false,
        editable: {
            mode: "inline",
            confirmation: false
        },
        columns: [
            {
                title: "NO.",
                template: function () {
                    record++;
                    return "<label for='" + record + "'>" + record + "</label>";
                },
                width: 50
            },
            {
                field: "jobGrades",
                title: "Job Grade",
                width: "100px",
                template: function (dataItem) {
                    /*if (dataItem.select && dataItem.jobGradeId === '') {
                        return `<select kendo-drop-down-list style="width: 100%;"
                            id="jobGradeId"
                            name="jobGradeId"
                            k-ng-model="dataItem.jobGradeId"       
                            k-data-source="jobGradeDataSource"        
                            k-options="jobGradeOptions"
                            ></select>`;
                    } else {
                        var jobGrade = $scope.jobGradeDataSource.find( x => x.id == dataItem.jobGradeId);
                        if (typeof (jobGrade) !== "undefined") {
                            return `<label>${jobGrade.caption}</label>`;
                        }
                        else
                            return `<label>N/A</label>`;
                        
                    }*/
                    return `<label>${dataItem.jobGradeCaption}</label>`;
                }
            },
            {
                field: "budgetTo",
                title: "Budget Limit",
                width: "100px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input kendo-numeric-text-box k-min="0" id="budgetTo" name="budgetTo" k-ng-model="dataItem.budgetTo" style="width: 100%;"/>`
                    }
                    else
                        return "<label>{{ " + dataItem.budgetTo + "| number" + "}}</label>";
                }
            },
            {
                title: "ACTIONS",
                width: "150px",
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
    $("#budgetTo").kendoNumericTextBox({
        format: "c",
    });
    function checkEdit() {
        var result = false;
        $scope.dataBTAPolicy.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }
    let requiredFields = [
        {
            fieldName: "jobGrade",
            title: "Job Grade"
        },
        {
            fieldName: "budgetTo",
            title: "Budget To"
        }
    ];
    async function findBTAPolicyExist(jobGradeId, isStore) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 0,
            limit: appSetting.pageSizeDefault
        };

        queryArgs.predicateParameters.push(jobGradeId);
        queryArgs.predicateParameters.push(isStore);
        var result = await settingService.getInstance().btaPolicy.getBTAPolicyByJobGradeId(queryArgs).$promise;
        if (result.object.count >= 1) {
            return true;
        }
        return false;
    }
    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }
    $scope.dataDelete = [];
    function deleteRecord(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            $scope.dataDelete = model;
            $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
            $scope.dialog.bind("close", confirm);
        }
    }
    function validateEdit() {
        var result = false;
        $scope.dataBTAPolicy.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    confirm = function (e) {
        let grid = $("#grid").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().btaPolicy.deleteBTAPolicy({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Saved");
                    getListBTAPolicy();
                    grid.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }
    async function save(model) {
        var hasError = false;
        let errors = $rootScope.validateInRecruitment(requiredFields, model);
        if (errors.length > 0) {
            let errorList = errors.filter(function (x) {
                if (x.fieldName === "budgetTo" && model.budgetTo === 0) {
                    return false;
                }
                return true;
            }).map(x => {
                return x.controlName + " " + x.errorDetail;
            });
            if (errorList.length > 0) {
                hasError = true;
                Notification.error(`Some fields are required: </br>
                    <ul>${errorList.join('<br/>')}</ul>`);
            }
        }
        if (!hasError) {
            settingService.getInstance().btaPolicy.updateBTAPolicy({ Id: model.id, BudgetFrom: 0, BudgetTo: model.budgetTo, IsStore: model.isStore }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Saved");
                    getListBTAPolicy();
                    return true;
                }
            });
           //}
        }
    }
    function cancel(model) {
        model['select'] = false;
        var item = $scope.dataBTAPolicy.find(x => x.id === model.id);
        item.select = false;
        model.budgetTo = item.budgetTo;
        model.isStore = item.isStore;
        let grid = $("#grid").data("kendoGrid");
        grid.refresh();
    }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            model['select'] = true;
            var item = $scope.dataBTAPolicy.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#grid").data("kendoGrid");
            grid.refresh();
        }

    }

    async function ngInit() {
        await getJobGrade();
    }

    async function _onChangeType(value) {
        if (value) {
            $scope.TypeDepartment = value;
            await loadPageOne();
        }
    }

    $scope.onChangeType = function (value) {
        _onChangeType(value);
    }
});