var ssgApp = angular.module("ssg.settingHolidayScheduleControllerModule", [
    "kendo.directives"
]);
ssgApp.controller("holidayScheduleController", function (
    $rootScope,
    $scope,
    $location,
    $stateParams,
    appSetting,
    commonData,
    Notification,
    settingService,
    fileService
) {
    // create a message to display in our view
    var ssg = this;
    $scope.advancedSearchMode = false;
    $scope.DateOfBirth = new Date();
    $rootScope.isParentMenu = false;
    $scope.title = $stateParams.action.title;
    $scope.keyword = '',
        $scope.year = '';

    // phần khai báo chung
    $scope.actions = {
        save: save,
        cancel: cancel,
        edit: edit,
        deleteRecord: deleteRecord,
        create: create,
        ifEnter: ifEnter
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

    $scope.Init = async function () {
        $scope.keyword = '';
        // $scope.year = '';
        var dropdownlist = $("#year").data("kendoDropDownList");
        dropdownlist.value("");

        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };


    }

    $scope.total = 0;
    $scope.dataHolidaySchedule = [];
    //API
    async function getListHolidaySchedules(option) {
        let args = buildArgs($scope.currentQuery.page, appSetting.pageSizeDefault);
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;
        var result = await settingService.getInstance().cabs.getHolidaySchedules($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.dataHolidaySchedule = result.object.data;
            var count = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;
            $scope.dataHolidaySchedule.forEach(x => {
                x["select"] = false;
                x["no"] = count;
                count = count + 1;
            });
            let nValue = {
                no: count,
                id: '',
                fromDate: null,
                toDate: null,
                title: '',
                select: true,
            };
            $scope.dataHolidaySchedule.push(nValue);
            $scope.total = result.object.count;
            if (option) {
                option.success($scope.dataHolidaySchedule);
            }
            else {
                let grid = $("#grid").data("kendoGrid");
                grid.dataSource.read();
                grid.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    function buildArgs(pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        if ($scope.keyword) {
            predicate.push('Title.contains(@0)');
            predicateParameters.push($scope.keyword);
        }
        if ($scope.year) {
            predicate.push('fromDate.Year == @{i} || toDate.Year == @{i}');
            predicateParameters.push($scope.year);

        }
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }

    // Get list
    $scope.holidayScheduleOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getListHolidaySchedules(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataHolidaySchedule }
            }
        },
        sortable: false,
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
                title: "No.",
                width: "50px",
                template: function (dataItem) {
                    if (dataItem.id) {
                        return `<span>{{dataItem.no}}</span>`;
                    }
                    else {
                        return `<span></span>`;
                    }
                }
            },
            {
                field: "fromDate",
                title: "From Date",
                width: "200px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input style="width: 100%;" kendo-date-picker id="fromDate" name="fromDate" k-format="'dd/MM/yyyy'" k-on-change="changeFromDate(dataItem)" k-date-input="true" k-ng-model="dataItem.fromDate"/>`;
                    } else {
                        return moment(dataItem.fromDate).format('DD/MM/YYYY');
                    }
                }
            },
            {
                field: "toDate",
                title: "To Date",
                width: "200px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input style="width: 100%;" id="toDate${dataItem.no}" kendo-date-picker id="toDate" name="toDate" k-format="'dd/MM/yyyy'" k-date-input="true" k-ng-model="dataItem.toDate"/>`;
                    } else {
                        return moment(dataItem.toDate).format('DD/MM/YYYY');
                    }
                }
            },
            {
                field: "title",
                title: "Title",
                width: "300px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="title" ng-model="dataItem.title"/>`;
                    } else {
                        return `<span>{{dataItem.title}}</span>`;
                    }
                }
            },
            {
                title: "actions",
                width: "150px",
                template: function (dataItem) {
                    if (!dataItem.id) {
                        return `
                        <a class="btn btn-sm btn-primary " ng-click="actions.create(dataItem)"><i class="fa fa-plus right-5"></i>Create</a>
                        `
                    }
                    if (dataItem.select) {
                        return `
                        <a class="btn btn-sm default green-stripe " ng-click="actions.save(dataItem)">Save</a>
                        <a class="btn btn-sm default " ng-click="actions.cancel(dataItem)">Cancel</a>
                `;
                    } else {
                        return `
                        <a class="btn btn-sm default blue-stripe " ng-click="actions.edit(dataItem)">Edit</a>
                        <a class="btn btn-sm default red-stripe " ng-click="actions.deleteRecord(dataItem)">Delete</a>
                `;
                    }
                }
            }
        ]
    };

    async function create(model) {
        var editExist = checkEdit();
        if (editExist) {
            Notification.error("Please save selected item before create other item");
        }
        else {
            let errors = $rootScope.validateInRecruitment(requiredFields, model);
            if (errors.length > 0) {
                let errorList = errors.map(x => {
                    return x.controlName + " " + x.errorDetail;
                });
                Notification.error(`Some fields are required: </br>
                <ul>${errorList.join('<br/>')}</ul>`);

            }
            else {
                let errors = checkFromToDateDuplicateWithRowOther(model);
                if (errors.length) {
                    Notification.error(`<ul>${errors.join('<br/>')}</ul>`);
                }
                else {
                    settingService.getInstance().cabs.createHolidaySchedule({ Id: model.id, FromDate: model.fromDate, ToDate: model.toDate, Title: model.title }).$promise.then(function (result) {
                        if (result.isSuccess) {
                            Notification.success("Data Successfully Created");
                            clearData();
                            loadPageOne($scope.keyword = '');
                            return true;
                        }
                    });
                }
            }
        }
    };

    $scope.yearHolidayDataSource = {
        dataTextField: 'value',
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
                    await getYearHolidays(e);
                }
            },
            schema: {
                data: () => {
                    return $scope.dataYearHolidays
                }
            }
        }
    };

    $scope.resetSearch = async function () {
        await $scope.Init();
        clearData();
        loadPageOne();
    }

    async function getYearHolidays(option) {
        var resultYearHoliday = await settingService.getInstance().cabs.getYearHolidays().$promise;
        if (resultYearHoliday.isSuccess) {
            $scope.dataYearHolidays = [];
            resultYearHoliday.object.data.forEach(element => {
                let value = {
                    code: element.code,
                    value: element.code
                }
                $scope.dataYearHolidays.push(value);
            });
        }
        if (option) {
            option.success($scope.dataYearHolidays);
        }
    }

    function checkEdit() {
        var result = false;
        $scope.dataHolidaySchedule.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    $scope.errors = [];

    let requiredFields = [
        {
            fieldName: "fromDate",
            title: "From Date"
        },
        {
            fieldName: "toDate",
            title: "To Date"
        },
        {
            fieldName: "title",
            title: "Title"
        }
    ];

    async function save(model) {
        let errors = $rootScope.validateInRecruitment(requiredFields, model);
        if (errors.length > 0) {
            let errorList = errors.map(x => {
                return x.controlName + " " + x.errorDetail;
            });
            Notification.error(`Some fields are required: </br>
            <ul>${errorList.join('<br/>')}</ul>`);

        }
        else {
            let errors = checkFromToDateDuplicateWithRowOther(model, false);
            if (errors.length) {
                Notification.error(`<ul>${errors.join('<br/>')}</ul>`);
            }
            else {
                settingService.getInstance().cabs.updateHolidaySchedule({ Id: model.id, FromDate: model.fromDate, ToDate: model.toDate, Title: model.title }).$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data Successfully Saved");
                        clearData();
                        getListHolidaySchedules();
                        return true;
                    }
                });
            }
        }
    }

    function checkFromToDateDuplicateWithRowOther(dataItem, isCreate = true) {
        let errors = [];
        let holidays = getDataGrid('#grid');
        if (holidays.length) {
            holidays.forEach(item => {
                if (isCreate) {
                    if (item.id) {
                        if(item.fromDate <= dataItem.fromDate && dataItem.fromDate <= item.toDate) {
                            errors.push('From Date ' + moment(dataItem.fromDate).format(appSetting.sortDateFormat) + ' has existed in Holiday Schedule');
                        }
                        if(item.fromDate <= dataItem.toDate && dataItem.toDate <= item.toDate) {
                            errors.push('To Date ' + moment(dataItem.toDate).format(appSetting.sortDateFormat) + ' has existed in Holiday Schedule');
                        }
                    }
                }
                else {
                    if (item.id != dataItem.id) {
                        if(item.fromDate <= dataItem.fromDate && dataItem.fromDate <= item.toDate) {
                            errors.push('From Date ' + moment(dataItem.fromDate).format(appSetting.sortDateFormat) + ' has existed in Holiday Schedule');
                        }
                        if(item.fromDate <= dataItem.toDate && dataItem.toDate <= item.toDate) {
                            errors.push('To Date ' + moment(dataItem.toDate).format(appSetting.sortDateFormat) + ' has existed in Holiday Schedule');
                        }
                    }
                }
            });
        }
        return errors;
    }

    function getDataGrid(id) {
        let gridRoom = $(id).data("kendoGrid");
        return gridRoom.dataSource._data.toJSON();
    }

    async function ifEnter($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            loadPageOne();
        }
    }

    $scope.search = async function () {
        loadPageOne();
    }

    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }

    function cancel(model) {
        model['select'] = false;
        var item = $scope.dataHolidaySchedule.find(x => x.id === model.id);
        item.select = false;
        model.fromDate = item.fromDate;
        model.toDate = item.toDate;
        model.title = item.title;
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
            var item = $scope.dataHolidaySchedule.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#grid").data("kendoGrid");
            grid.refresh();
            //set min cho toDate
            let id = "#toDate" + item.no;
            $(id).data('kendoDatePicker').min(new Date(item.fromDate));
            //
        }

    }

    function validateEdit() {
        var result = false;
        $scope.dataHolidaySchedule.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
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

    confirm = function (e) {
        let grid = $("#grid").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().cabs.deleteHolidaySchedule({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Deleted");
                    getListHolidaySchedules();
                    grid.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }

    $scope.export = async function () {
        let option = buildArgs();
        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.HOLIDAYSCHEDULE
        }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }

    $scope.$on('$locationChangeStart', function (event, next, current) {
        $scope.errors = [];
    });

    $scope.$on("$viewContentLoaded", function () {
    });

    $scope.changeFromDate = function (dataItem) {
        let id = "#toDate" + dataItem.no;
        $(id).data('kendoDatePicker').min(new Date(dataItem.fromDate));
        if (dataItem.fromDate > dataItem.toDate) {
            dataItem.toDate = dataItem.fromDate;
        }
    }

    function clearData() {
        var dropdownlist = $("#year").data("kendoDropDownList");
        dropdownlist.value("");
        $scope.keyword = '';
        $scope.year = '';
    }
});
