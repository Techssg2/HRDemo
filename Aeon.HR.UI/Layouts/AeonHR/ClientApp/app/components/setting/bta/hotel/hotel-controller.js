var ssgApp = angular.module('ssg.hotelModule', ['kendo.directives']);
ssgApp.controller('hotelController', function ($rootScope, $scope, $translate, $location, appSetting, Notification, commonData, $stateParams, settingService, fileService, $timeout) {
    var ssg = this;
    $scope.title = 'Hotel';
    var checkForeignerCollection = {
        all: 'all',
        foreigner: 'true',
        notForeigner: 'false',
        bold: 'both'
    }
    $scope.checkForeigner = {
        value: checkForeignerCollection.all
    };

    // phần khai báo chung
    $scope.errors = [];
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
            fieldName: "businessTripLocationId",
            title: "Business trip location"
        }
    ];

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

    this.$onInit = async () => {
        await getBusinessTripLocations();
        $scope.foreigners = commonData.foreignerOptions;
    }

    // Get list
    $scope.hotelSettingOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getHotelSettings(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataHotel }
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
                title: "NO.",
                width: "50px",
                template: function (dataItem) {
                    return `<span>{{dataItem.no}}</span>`;
                }
            },
            {
                field: "code",
                title: "Code",
                width: "100px",
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
                width: "300px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="name" ng-model="dataItem.name"/>`;
                    } else {
                        return `<span>{{dataItem.name}}</span>`;
                    }
                }
            },
            {
                field: "address",
                title: "Address",
                width: "300px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="address" ng-model="dataItem.address"/>`;
                    } else {
                        return `<span>{{dataItem.address}}</span>`;
                    }
                }
            },
            {
                field: "telephone",
                title: "Telephone",
                width: "300px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input name="telephone" class="k-input" type="text" ng-model="dataItem.telephone" style="width: 100%"
                        onKeyDown="return phoneNumberInput(event.keyCode)"/>`;
                    } else {
                        return `<span>{{dataItem.telephone}}</span>`;
                    }
                }
            },
            {
                field: "businessTripLocationId",
                title: "business Trip Location",
                width: "150px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<select kendo-drop-down-list name="name" id="businessTripLocationId"
                            k-options="dataBusinessTripLocationOptions"
                            k-data-source="dataBusinessTripLocations"
                            data-k-ng-model="dataItem.businessTripLocationId"         
                            k-value-primitive="'false'"
                            style="width: 100%;">
                            </select>`
                    }
                    else {
                        return `<label>${dataItem.businessTripLocationName}</label>`
                    }
                }
            },
            // {
            //     field: "isForeigner",
            //     title: "Is Foreigner",
            //     width: "150px",
            //     template: function (dataItem) {
            //         if (dataItem.select) {
            //             return `
            //             <input type="checkbox" ng-model="dataItem.isForeigner" id="{{dataItem.uid}}" class="k-checkbox" style="width: 100%;" />
            //                 <label class="k-checkbox-label cbox" for="{{dataItem.uid}}"></label>`
            //         } else {
            //             return `<input type="checkbox" ng-model="dataItem.isForeigner" id="{{dataItem.uid}}" class="k-checkbox" style="width: 100%;" disabled="true" />
            //                 <label class="k-checkbox-label cbox" for="{{dataItem.uid}}"></label>`;
            //         }
            //     }
            // },
            //ngan
            {
                field: "isForeigner",
                title: "Is Foreigner",
                width: "150px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<select kendo-drop-down-list 
                            style="width: 100%;"
                            k-ng-model="dataItem.isForeigner"       
                            k-options="foreignerOptions"
                            k-data-source="foreigners"
                            > </select>`;
                    } else {
                        return `<span>{{dataItem.isForeignerName}}</span>`;
                    }
                }
            },
            //end
            {
                title: "ACTIONS",
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

    $scope.dataBusinessTripLocations = [];
    $scope.dataBusinessTripLocationOptions = {
        dataTextField: 'name',
        dataValueField: 'id',
        autoBind: true,
        filter: "contains",
        filtering: $rootScope.dropdownFilter
    };

    $scope.foreigners = [];
    $scope.foreignerOptions = {
        dataTextField: "name",
        dataValueField: "value",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.foreigners
        // dataSource: {
        //     data: translateStatus($translate, commonData.foreignerOptions)
        // }
    }

    async function getBusinessTripLocations() {
        $scope.dataBusinessTripLocations = [];
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000000
        };

        var res = await settingService.getInstance().businessTripLocation.getListBusinessTripLocation(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataBusinessTripLocations = res.object.data;
        }
        $timeout(function () {
            var dataBusinessTripLocationOption = $("#businessTripLocationId").data("kendoDropDownList");
            if(dataBusinessTripLocationOption){
                dataBusinessTripLocationOption.setDataSource($scope.dataBusinessTripLocations);
            }
        }, 1000);
    }

    $scope.onCheckForeigner = async function () {
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        var option = $scope.currentQuery;
        if ($scope.checkForeigner.value != 'all') {
            var isForeigner = $scope.checkForeigner.value;
            option.predicate = "IsForeigner == @0";
            option.predicateParameters.push(isForeigner);
        }
        reloadGrid();
        //loadPageOne();
    }

    function reloadGrid() {
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
            grid.dataSource.page(1);
        }
    }

    //API
    async function getHotelSettings(option) {
        let args = buildArgs($scope.currentQuery.page, appSetting.pageSizeDefault);
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;
        var result = await settingService.getInstance().hotel.getListHotels($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.dataHotel = result.object.data;
            var count = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;
            $scope.dataHotel.forEach(x => {
                x["select"] = false;
                x["no"] = count;
                x["businessTripLocationName"] = findBusinessTripLocationName(x.businessTripLocationId);
                x["isForeignerName"] = findForeignerName(x.isForeigner);
                count = count + 1;
            });
            let nValue = {
                id: '',
                code: "",
                name: "",
                address: "",
                telephone: "",
                businessTripLocationId: "",
                isForeigner: "",
                select: true,
            };
            $scope.dataHotel.push(nValue);
            $scope.total = result.object.count;
            if (option) {
                option.success($scope.dataHotel);
            }
            else {
                let grid = $("#grid").data("kendoGrid");
                grid.dataSource.read();
                grid.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    function findBusinessTripLocationName(id){
        var businessTripLocationName;
        $scope.dataBusinessTripLocations.forEach(x => {
            if(x.id == id){
                businessTripLocationName = x.name;
            }
        });
        return businessTripLocationName;
    }

    function findForeignerName(value){
        var isForeignerName;
        $scope.foreigners.forEach(x =>{
            if(x.value == value){
                isForeignerName = x.name;
            }
        });
        return isForeignerName;
    }

    function buildArgs(pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        // if ($scope.keyword) {
        //     predicate.push('(Code.contains(@0) or Name.contains(@0) or BusinessTripLocation.Code.contains(@0) or BusinessTripLocation.Name.contains(@0))');
        //     predicateParameters.push($scope.keyword);  
        // }
        
        if($scope.checkForeigner.value != 'all'){
            predicate.push('IsForeigner == @0');
            var isForeigner = $scope.checkForeigner.value;
            predicateParameters.push(isForeigner);
        }

        if(actionSearch) {
            if ($scope.keyword) {
                predicate.push(`(Code.contains(@${predicateParameters.length}) or Name.contains(@${predicateParameters.length}) or BusinessTripLocation.Name.contains(@${predicateParameters.length}))`);
                predicateParameters.push($scope.keyword);  
            }
        }
        else {
            predicate.push(`(Code.contains(@${predicateParameters.length}) or Name.contains(@${predicateParameters.length}) or BusinessTripLocation.Name.contains(@${predicateParameters.length}))`);
            predicateParameters.push(keyWordOld);  
        }
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }

    async function save(model) {
        let errors = $rootScope.validateInRecruitment(requiredFields, model);
        if (errors.length > 0) {
            let errorList = errors.map(x => {
                return x.controlName + " " + x.errorDetail;
            });
            Notification.error(`Some fields are required: </br>
            <ul>${errorList.join('<br/>')}</ul>`);
            
        }
        else if(errors.length == 0){
            var checkCode = await findHotelByCode(model.code, model.id);
            if (checkCode) {
                Notification.error("Code " + model.code + " has existed in Hotel Setting");
            }
            else {
                settingService.getInstance().hotel.updateHotel(
                    { 
                        Id: model.id, 
                        Code: model.code, 
                        Name: model.name, 
                        Address: model.address, 
                        Telephone: model.telephone, 
                        IsForeigner: model.isForeigner, 
                        BusinessTripLocationId: model.businessTripLocationId
                    }).$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                        getHotelSettings();
                        return true;
                    }
                });
            }
        }
    }

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
            else if(errors.length == 0){
                var checkCode = await findHotelByCode(model.code);
                if (checkCode) {
                    Notification.error("Code " + model.code + " has existed in Hotel Setting");
                }
                else{
                    settingService.getInstance().hotel.createHotel(
                        { 
                            Id: model.id, 
                            Code: model.code, 
                            Name: model.name,
                            Address: model.address, 
                            Telephone: model.telephone, 
                            IsForeigner: model.isForeigner, 
                            BusinessTripLocationId: model.businessTripLocationId
                        }).$promise.then(function (result) {
                        if (result.isSuccess) {
                            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                            loadPageOne($scope.keyword = '');
                            return true;
                        }
                    });
                }
            }
        }
    };

    async function findHotelByCode(code, id) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 0,
            limit: appSetting.pageSizeDefault
        };

        queryArgs.predicateParameters.push(code);
        queryArgs.predicateParameters.push(id);
        var result = await settingService.getInstance().hotel.getHotelByCode(queryArgs).$promise;
        if (result.object.count >= 1) {
            return true;
        }
        return false;
    }

    function checkEdit() {
        var result = false;
        $scope.dataHotel.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    function cancel(model) {
        model['select'] = false;
        var item = $scope.dataHotel.find(x => x.id === model.id);
        item.select = false;
        model.code = item.code;
        model.name = item.name;
        model.address = item.address;
        model.telephone = item.telephone;
        model.isForeigner = item.isForeigner;
        model.businessTripLocationId = item.businessTripLocationId;
        let grid = $("#grid").data("kendoGrid");
        grid.refresh();
    }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            actionSearch = false;
            model['select'] = true;
            var item = $scope.dataHotel.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#grid").data("kendoGrid");
            grid.refresh();
        }

    }

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
            settingService.getInstance().hotel.deleteHotel({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    actionSearch = false;
                    Notification.success('Data Sucessfully Deleted');
                    getHotelSettings();
                    grid.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }

    function validateEdit() {
        var result = false;
        $scope.dataHotel.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    actionSearch = false;
    keyWordOld = '';
    async function ifEnter($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            actionSearch = true;
            keyWordOld = $scope.keyword;
            loadPageOne();
        }
    }

    $scope.search = async function () {
        actionSearch = true;
        keyWordOld = $scope.keyword;
        loadPageOne();
    }

    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }

    $scope.export = async function () {
        let args = buildArgs($scope.currentQuery.page, appSetting.pageSizeDefault);
        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.HOTELSETTING
        }, args).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }
})