var ssgApp = angular.module('ssg.flightNumberModule', ['kendo.directives']);
ssgApp.controller('flightNumberController', function ($rootScope, $scope, $location, appSetting, Notification, commonData, $stateParams, settingService, fileService, $timeout) {
    var ssg = this;
    $scope.title = 'Flight Number';


    $scope.keyword = '';
    $scope.flightNumbers = [];
    $scope.total = 0;
    $scope.flightNumberOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getFlightNumbers(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.flightNumbers }
            }
        },
        sortable: false,
        pageable: true,
        editable: {
            mode: "inline",
            confirmation: false
        },
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray
        },
        columns: [
            {
                field: "no",
                title: "NO.",
                width: "20px",
                template: function (dataItem) {
                    return `<span>{{dataItem.no}}</span>`;
                }
            },
            {
                field: "code",
                title: "Code",
                width: "70px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="code" ng-model="dataItem.code"/>`;
                    } else {
                        return `<span>{{dataItem.code}}</span>`;
                    }
                }
            },
            {
                field: "name",
                title: "Name",
                width: "200px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="name" ng-model="dataItem.name"/>`;
                    } else {
                        return `<span>{{dataItem.name}}</span>`;
                    }
                }
            },
            {
                field: "airlineId",
                title: "Airline",
                width: "200px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<select kendo-drop-down-list style="width: 100%;"
                            id="airlineId"
                            name="airlineId"
                            k-ng-model="dataItem.airlineId"       
                            k-data-source="airlines"        
                            k-options="airlineOptions"
                            ></select>`;
                    } else {
                        return `<span>{{dataItem.airlineName}}</span>`;
                    }
                }
            },
            {
                field: "departureTime",
                title: "Departure Time",
                width: "100px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input kendo-time-picker k-format="'HH:mm'"
                            k-interval="5"
                            k-date-input="true" 
                            ng-model="dataItem.departureTime"
                            style="width: 100%;" 
                            />`;
                    } else {
                        return `<span>{{dataItem.departureTime}}</span>`;
                    }
                }
            },
            {
                field: "isPeakTime",
                title: "Is Peak Time",
                width: "150px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `
                        <input type="checkbox" id="{{dataItem.id}}" ng-model="dataItem.isPeakTime" class="k-checkbox">
                            <label class="k-checkbox-label" for="{{dataItem.id}}"></label>
                        `
                    } else {
                        return `<input type="checkbox" id="{{dataItem.id}}" ng-model="dataItem.isPeakTime" class="k-checkbox" disabled="true">
                        <label class="k-checkbox-label" for="{{dataItem.id}}"></label>`;
                    }
                }
            },
            {
                title: "ACTION",
                width: "150px",
                template: function (dataItem) {
                    if (dataItem.id == 1) {
                        return `<a class="btn btn-sm btn-primary " ng-click="executeAction('Create', dataItem)"><i class="fa fa-plus right-5"></i>Create</a>`;
                    }
                    if (dataItem.select) {
                        return `
                        <a class="btn btn-sm default green-stripe " ng-click="executeAction('Save', dataItem)">Save</a>
                        <a class="btn btn-sm default " ng-click="executeAction('Cancel', dataItem)">Cancel</a>`;
                    } else {
                        return `
                        <a class="btn btn-sm default blue-stripe " ng-click="executeAction('Edit', dataItem)">Edit</a>
                        <a class="btn btn-sm default red-stripe " ng-click="executeAction('Delete', dataItem)">Delete</a>`;
                    }
                }
            }
        ]
    };

    let requiredFields = [
        {
            fieldName: "code",
            title: "Code"
        },
        {
            fieldName: "name",
            title: "Name"
        },
        {
            fieldName: "airlineId",
            title: "Airline"
        },
        {
            fieldName: "departureTime",
            title: "Departure Time"
        }
    ];

    dataTemporary = '';
    $scope.executeAction = async function (typeAction, dataItem) {
        let grid = $("#flightNumberGrid").data("kendoGrid");
        switch (typeAction) {
            case 'Create':
                var checkEdit = validateEdit(grid.dataSource._data);
                if (!checkEdit) {
                    let errors = $rootScope.validateInRecruitment(requiredFields, dataItem);
                    if (errors.length > 0) {
                        let errorList = errors.map(x => {
                            return x.controlName + " " + x.errorDetail;
                        });
                        Notification.error(`Some fields are required: </br><ul>${errorList.join('<br/>')}</ul>`);
                    }
                    else {
                        let model = {
                            code: dataItem.code,
                            name: dataItem.name,
                            airlineId: dataItem.airlineId,
                            isPeakTime: dataItem.isPeakTime,
                            departureTime: dataItem.departureTime
                        }
                        var resultValidateCode = await settingService.getInstance().flightNumber.checkValidateFlightNumberCode(model).$promise;
                        if (resultValidateCode.object.count > 0) {
                            Notification.error("Code " + dataItem.code + " has existed");
                        }
                        else {
                            var result = await settingService.getInstance().flightNumber.saveFlightNumber(model).$promise;
                            if (result.isSuccess) {
                                Notification.success("Data Successfully Saved");
                                loadPageOne();
                            }
                        }
                    }
                }
                else {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                break;
            case 'Save':
                let errors = $rootScope.validateInRecruitment(requiredFields, dataItem);
                if (errors.length > 0) {
                    let errorList = errors.map(x => {
                        return x.controlName + " " + x.errorDetail;
                    });
                    Notification.error(`Some fields are required: </br><ul>${errorList.join('<br/>')}</ul>`);
                }
                else {
                    let flag = false;
                    let model = {
                        id: dataItem.id,
                        code: dataItem.code,
                        name: dataItem.name,
                        airlineId: dataItem.airlineId,
                        isPeakTime: dataItem.isPeakTime,
                        departureTime: dataItem.departureTime
                    }
                    if (dataItem.code != dataTemporary.code) {
                        var resultValidateCode = await settingService.getInstance().flightNumber.checkValidateFlightNumberCode(model).$promise;
                        if (resultValidateCode.object.count > 0) {
                            flag = true;
                        }
                    }
                    if (flag) {
                        Notification.error("Code " + dataItem.code + " has existed");
                    }
                    else {
                        var result = await settingService.getInstance().flightNumber.saveFlightNumber(model).$promise;
                        if (result.isSuccess) {
                            Notification.success("Data Successfully Saved");
                            page = grid.pager.dataSource._page;
                            pageSize = grid.pager.dataSource._pageSize;
                            grid.dataSource.fetch(() => grid.dataSource.page(page), grid.dataSource.take(pageSize));
                        }
                    }
                }
                break;
            case 'Cancel':
                dataItem.select = false;
                dataItem.no = dataTemporary.no;
                dataItem.id = dataTemporary.id;
                dataItem.code = dataTemporary.code;
                dataItem.name = dataTemporary.name;
                dataItem.airlineId = dataTemporary.airlineId;
                dataItem.airlineCode = dataTemporary.airlineCode;
                dataItem.airlineName = dataTemporary.airlineName;
                dataItem.isPeakTime = dataTemporary.isPeakTime;
                dataItem.departureTime = dataTemporary.departureTime;
                grid.refresh();
                break;
            case 'Edit':
                var result = validateEdit(grid.dataSource._data);
                if (result) {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                else {
                    dataTemporary = _.clone(dataItem);
                    dataItem.select = true;
                    grid.refresh();
                }
                break;
            case 'Delete':
                var result = validateEdit(grid.dataSource._data);
                if (result) {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                else {
                    itemDeleteId = dataItem.id;
                    $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
                    $scope.dialog.bind("close", confirm);
                }
                break;
            default:
                break;
        }
        actionSearch = false;
    }

    itemDeleteId = '';
    confirm = async function (e) {
        let grid = $("#flightNumberGrid").data("kendoGrid");
        if (e.data && e.data.value) {
            let model = {
                id: itemDeleteId
            }
            var result = await settingService.getInstance().flightNumber.deleteFlightNumber(model).$promise
            if (result.isSuccess) {
                Notification.success("Data Sucessfully Deleted");
                page = grid.pager.dataSource._page;
                pageSize = grid.pager.dataSource._pageSize;
                grid.dataSource.fetch(() => grid.dataSource.page(page), grid.dataSource.take(pageSize));
            }
        }
    }

    function validateEdit(data) {
        var result = false;
        data.forEach(item => {
            if (item.select === true && item.id !== 1) {
                result = true;
                return result;
            }
        });
        return result
    }

    actionSearch = false;
    keyWordOld = '';
    $scope.search = function () {
        actionSearch = true;
        keyWordOld = $scope.keyword;
        loadPageOne();
    }

    $scope.ifEnter = async function($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            actionSearch = true;
            keyWordOld = $scope.keyword;
            loadPageOne();
        }
    }

    function loadPageOne() {
        let grid = $("#flightNumberGrid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }

    $scope.airlines = [];
    $scope.airlineOptions = {
        dataTextField: "name",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.airlines
    };

    async function getFlightNumbers(option) {
        let currentQuery = {
            predicate: "",
            predicateParameters: [],
            //order: "Modified desc",
            order: "airline.Name, departureTime asc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };

        if (option) {
            currentQuery.limit = option.data.take;
            currentQuery.page = option.data.page;
        }

        if(actionSearch) {
            if ($scope.keyword) {
                currentQuery.predicate = 'Name.contains(@0) or Code.contains(@0) or Airline.Name.contains(@0)';
                currentQuery.predicateParameters.push($scope.keyword);
            }
        }
        else {
            currentQuery.predicate = 'Name.contains(@0) or Code.contains(@0) or Airline.Name.contains(@0)';
            currentQuery.predicateParameters.push(keyWordOld);
        }

        var result = await settingService.getInstance().flightNumber.getFlightNumbers(currentQuery).$promise;
        if (result.isSuccess) {
            $scope.flightNumbers = result.object.data;
            var count = ((currentQuery.page - 1) * currentQuery.limit) + 1;
            $scope.flightNumbers.forEach(x => {
                x["select"] = false;
                x["no"] = count;
                count = count + 1;
            });
            let value = {
                id: 1,
                code: '',
                name: '',
                airlineId: '',
                isPeakTime: false,
                departureTime: '',
                select: true,
            };
            $scope.flightNumbers.push(value);
            $scope.total = result.object.count;
            option.success($scope.flightNumbers);
        }
    }

    async function getAirlines() {
        let model = {
            Predicate: "",
            PredicateParameters: [],
            Order: "Name asc",
            Limit: 10000,
            Page: 1
        }

        let result = await settingService.getInstance().airline.getAirlines(model).$promise;
        if (result.isSuccess) {
            $scope.airlines = result.object.data;
        }
    }

    this.$onInit = async function () {
        await getAirlines();
    }

    $scope.export = async function() {
        let model = {
            predicate: '',
            predicateParameters: [],
            order: "Modified desc",
        }

        if ($scope.keyword) {
            model.predicate = '(Code.contains(@0) or Name.contains(@0) or Airline.Name.contains(@0))';
            model.predicateParameters.push($scope.keyword);
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.FLIGHTNUMBERSETTING
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }
})