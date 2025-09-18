var ssgApp = angular.module('ssg.directiveApplicantItemG1', []);
ssgApp.directive("applicantItemG1", [
    function ($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/components/recruitment/applicant/applicant-directives/applicant-item-g1.html",
            scope: {
                widget: "=",
                model: "=",
                form: "=",
                removeSubItem: "=bind",
                districtData: "=",
                wardData: "=",
                masterDatas: "="
            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope", "settingService", "localStorageService", "masterDataService", "recruitmentService", "appSetting", "$timeout", "Notification",
                function ($rootScope, $scope, settingService, localStorageService, masterDataService, recruitmentService, appSetting, $timeout, Notification) {
                    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
                    $scope.changeDepartment = function (elementId, id) {
                        if (!id) {
                            setDataDepartmentFromId(allDepartments, elementId);
                        }
                    };

                    this.$onInit = async () => {
                        await getCountrySet();
                        await getProvinceSet();
                        await getAllListPositionForFilter();
                        //await getListForInterestInWorkPriority();
                        await getPosition();
                        await getRelation();                        
                        await getReligion();
                        await getOpenPositions();
                        await getWorkingPlaces();
                        $timeout(function () {
                            $scope.optionGroup = {
                                departmentTreeOptions: {
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
                                    valueTemplate: (e) => showCustomField(e, ['name']),
                                    dataSource: allDepartments
                                    // change: function (e) {
                                    //     // khi click dấu x no se set lai all department
                                    //     if (!e.sender.value()) {
                                    //         //setDataDepartment(allDepartments);
                                    //         setDataDepartmentFromId(allDepartments, 'departmentId0');
                                    //     }
                                    //     else {
                                    //         $scope.model.applicantRelativeInAeons.departmentId = e.sender.value();
                                    //     }
                                    // }
                                },
                                positionForListOptions: {
                                    placeholder: "",
                                    dataTextField: "positionName",
                                    dataValueField: "id",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: item.id #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: {
                                        serverPaging: false,
                                        //pageSize: 10000,
                                        transport: {
                                            read: async function (e) {
                                                var res = await recruitmentService.getInstance().position.getAllListPositionForFilter().$promise;
                                                if (res.isSuccess) {
                                                    e.success(res.object.data);
                                                }
                                            }
                                        },
                                        schema: {
                                            model: {
                                                children: "items"
                                            }
                                        },
                                    },
                                },
                                nationalityOption: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: code #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.countrySetItems,
                                    change: function (e) {
                                        $scope.model.nationalityName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                nativeOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: code #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.provinceSetItems,
                                    change: function (e) {
                                        $scope.model.nativeName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                birthPlaceOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.provinceSetItems,
                                    change: function (e) {
                                        $scope.model.birthPlaceName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                idCard9PlaceOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.provinceSetItems,
                                    change: function (e) {
                                        $scope.model.idCard9PlaceName = e.sender.text();
                                    },
                                    //dataBound: function () {
                                    //    //this.select(-1);
                                    //    //this.trigger("change");
                                    //},
                                    filtering: $rootScope.dropdownFilter,
                                },
                                idCard12PlaceOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.provinceSetItems,
                                    change: function (e) {
                                        $scope.model.idCard12PlaceName = e.sender.text();
                                    },
                                    //dataBound: function () {
                                    //    //this.select(-1);
                                    //    //this.trigger("change");
                                    //},
                                    filtering: $rootScope.dropdownFilter,
                                },
                                permanentResidentCityOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: code #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.provinceSetItems,
                                    change: async function (e) {
                                        $scope.model.permanentResidentCityName = e.sender.text();
                                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                            name: "District",
                                            parentCode: e.sender.value(),
                                        }).$promise;
                                        if (res.isSuccess && res.object.data.length > 0) {
                                            $scope.widget.permanentResidentDistrict.setDataSource(new kendo.data.DataSource({
                                                data: res.object.data,
                                            }));
                                            $scope.widget.permanentResidentWard.setDataSource(new kendo.data.DataSource({
                                                data: []
                                            }));
                                        }
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                permanentResidentDistrictOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: code #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    //dataSource: new kendo.data.DataSource({
                                    //    data: []
                                    //}),
                                    dataSource: $scope.districtData.permanentDistrictSetItems,
                                    change: async function (e) {
                                        $scope.model.permanentResidentDistrictName = e.sender.text();

                                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                            name: 'Ward',
                                            parentCode: `${$scope.model.permanentResidentCityCode},${e.sender.value()}`,
                                        }).$promise;
                                        if (res.isSuccess && res.object.data.length > 0) {
                                            $scope.widget.permanentResidentWard.setDataSource(new kendo.data.DataSource({
                                                data: res.object.data,
                                            }));
                                        }
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                permanentResidentWardOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: code #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    //dataSource: new kendo.data.DataSource({
                                    //    data: []
                                    //}),
                                    dataSource: $scope.wardData.permanentWardSetItems,
                                    change: function (e) {
                                        $scope.model.permanentResidentWardName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                provisionalResidentCityOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.provinceSetItems,
                                    change: async function (e) {
                                        $scope.model.provisionalResidentCityName = e.sender.text();

                                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                            name: "District",
                                            parentCode: e.sender.value(),
                                        }).$promise;
                                        if (res.isSuccess && res.object.data.length > 0) {
                                            $scope.widget.provisionalResidentDistrict.setDataSource(new kendo.data.DataSource({
                                                data: res.object.data,
                                            }));
                                            $scope.widget.provisionalResidentWard.setDataSource(new kendo.data.DataSource({
                                                data: []
                                            }));
                                        }
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                provisionalResidentDistrictOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    //dataSource: new kendo.data.DataSource({
                                    //    data: []
                                    //}),
                                    dataSource: $scope.districtData.provisionalDistrictSetItems,
                                    change: async function (e) {
                                        $scope.model.provisionalResidentDistrictName = e.sender.text();

                                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                            name: "Ward",
                                            parentCode: `${$scope.model.provisionalResidentCityCode},${e.sender.value()}`,
                                        }).$promise;
                                        if (res.isSuccess && res.object.data.length > 0) {
                                            $scope.widget.provisionalResidentWard.setDataSource(new kendo.data.DataSource({
                                                data: res.object.data,
                                            }));
                                        }
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                provisionalResidentWardOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    //dataSource: new kendo.data.DataSource({
                                    //    data: []
                                    //}),
                                    dataSource: $scope.wardData.provisionalWardSetItems,
                                    change: function (e) {
                                        $scope.model.provisionalResidentWardName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                positionOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.positionItems,
                                    filtering: $rootScope.dropdownFilter,
                                },
                                positionChange: function (e, index) {
                                    $scope.model.applicantRelativeInAeons[index].positionName = e.sender.text();
                                },

                                relationOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.relationItems,
                                    filtering: $rootScope.dropdownFilter,
                                },
                                relationChange: function (e, index) {
                                    $scope.model.applicantRelativeInAeons[index].relationName = e.sender.text();
                                },

                                workingPlacesOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.workingPlacesItems,
                                    filtering: $rootScope.dropdownFilter,
                                },
                                workingPlacesChange: function (e, index) {
                                    $scope.model.applicantRelativeInAeons[index].workingPlacesName = e.sender.text();
                                },

                                educationLevelOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    // valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.masterDatas.educationList,
                                    //dataBound: function () {
                                    //    this.select(-1);
                                    //    this.trigger("change");
                                    //},
                                    filtering: $rootScope.dropdownFilter,
                                    change: function (e) {
                                        $scope.model.educationLevelName = e.sender.text();
                                    }
                                },

                                schoolOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.masterDatas.schools,
                                    filtering: $rootScope.dropdownFilter,
                                    change: function (e) {
                                        $scope.model.schoolName = e.sender.text();
                                    }
                                },

                                myPositionOptions: {
                                    placeholder: "",
                                    dataTextField: "positionName",
                                    dataValueField: "id",
                                    template: '#: positionName # - #: deptDivisionName # - #: locationName #',
                                    valueTemplate: '#: positionName # - #: deptDivisionName # - #: locationName #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: true,
                                    filter: "contains",
                                    filtering: async function (option) {
                                        let filter = option.filter && option.filter.value ? option.filter.value : "";
                                        arg = {
                                            predicate: "positionName.contains(@0) or locationName.contains(@1) or DeptDivision.Name.Contains(@2) or RequestToHire.PositionCode.Contains(@3) or RequestToHire.ReferenceNumber.Contains(@4)",
                                            predicateParameters: [filter, filter, filter, filter, filter],
                                            page: 1,
                                            limit: 100,
                                            order: "created desc"
                                        }
                                        var res = await recruitmentService.getInstance().position.getListPosition(arg).$promise;
                                        if (res.isSuccess) {
                                            $timeout(function () {
                                                var positionList = $("#appliedPositionId").data("kendoDropDownList");
                                                if (positionList) {
                                                    positionList.setDataSource(res.object.data);
                                                }
                                            }, 0)

                                        }
                                    },
                                    dataSource: $scope.openPositions
                                },
                                workingTimeSelectOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "id",
                                    template: '#: code # - #: name #',
                                    //valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.masterDatas.workingTimes,
                                    filtering: $rootScope.dropdownFilter,
                                },
                                appropriateTimeOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: {
                                        serverPaging: false,
                                        pageSize: 100,
                                        transport: {
                                            read: async function (e) {
                                                var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                                    name: 'PreferedTime',
                                                    parentCode: '',
                                                }).$promise;
                                                if (res.isSuccess && res.object.data.length > 0) {
                                                    e.success(res.object.data);
                                                }
                                            }
                                        },
                                    },
                                    change: function (e) {
                                        $scope.model.appropriateTimeName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                expectedStartingDateOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: {
                                        serverPaging: false,
                                        pageSize: 100,
                                        transport: {
                                            read: async function (e) {
                                                var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                                    name: 'StartWorkingFrom',
                                                    parentCode: '',
                                                }).$promise;
                                                if (res.isSuccess && res.object.data.length > 0) {
                                                    e.success(res.object.data);
                                                }
                                            }
                                        },
                                    },
                                    //dataBound: function () {
                                    //    this.select(-1);
                                    //    //this.value(null);
                                    //    //this.text(null);
                                    //    this.trigger("change");
                                    //},
                                    change: function (e) {
                                        $scope.model.expectedStartingDateName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                fullTimeWorkingExprerimentOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    //template: '#: code # - #: name #',
                                    //valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: {
                                        serverPaging: false,
                                        pageSize: 100,
                                        transport: {
                                            read: async function (e) {
                                                var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                                    name: 'Experience',
                                                    parentCode: '',
                                                }).$promise;
                                                if (res.isSuccess && res.object.data.length > 0) {
                                                    e.success(res.object.data);
                                                }
                                            }
                                        },
                                    },
                                    //dataBound: function () {
                                    //    this.select(-1);
                                    //    this.trigger("change");
                                    //},
                                    change: function (e) {
                                        $scope.model.fullTimeWorkingExprerimentName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                skillsOptions: {
                                    placeholder: "Select skill...",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    // template: '#: code # - #: name #',
                                    // valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: {
                                        serverPaging: false,
                                        pageSize: 100,
                                        transport: {
                                            read: async function (e) {
                                                var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                                    name: 'Skill',
                                                    parentCode: '',
                                                }).$promise;
                                                if (res.isSuccess && res.object.data.length > 0) {
                                                    e.success(res.object.data);
                                                }
                                            }
                                        },
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                abilitiesOptions: {
                                    placeholder: "Select ability...",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    // template: '#: code # - #: name #',
                                    // valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: {
                                        serverPaging: false,
                                        pageSize: 100,
                                        transport: {
                                            read: async function (e) {
                                                var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                                    name: 'Certificate',
                                                    parentCode: '',
                                                }).$promise;
                                                if (res.isSuccess && res.object.data.length > 0) {
                                                    e.success(res.object.data);
                                                }
                                            }
                                        },
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                                languagesOptions: {
                                    placeholder: "Select language...",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    template: '#: code # - #: name #',
                                    valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: {
                                        serverPaging: false,
                                        pageSize: 100,
                                        transport: {
                                            read: async function (e) {
                                                var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                                    name: 'Language',
                                                    parentCode: '',
                                                }).$promise;
                                                if (res.isSuccess && res.object.data.length > 0) {
                                                    e.success(res.object.data);
                                                }
                                            }
                                        },
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },

                                interestInWorkPriorityOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    template: '#: code # - #: name #',
                                    //valueTemplate: '#: name #',
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.masterDatas.interestInWorkPriorityItems,
                                    //dataBound: function () {
                                    //    this.select(-1);
                                    //    //this.text(null);
                                    //    //this.value(null);
                                    //    this.trigger("change");
                                    //},
                                    filtering: $rootScope.dropdownFilter,
                                },
                                onchangeInterestsInWork: function (e, property) {
                                    $scope.model[property] = e.sender.text() || null;
                                },

                                educationalEstOptions: {
                                    placeholder: "",
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.positionItems,
                                    filtering: $rootScope.dropdownFilter,
                                },

                                passportPlaceOptions: {
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.provinceSetItems,
                                    filtering: $rootScope.dropdownFilter,
                                },

                                religionOptions: {
                                    dataTextField: "name",
                                    dataValueField: "code",
                                    valuePrimitive: true,
                                    checkboxes: false,
                                    autoBind: false,
                                    filter: "contains",
                                    dataSource: $scope.religionItems,
                                    change: function (e) {
                                        $scope.model.religionName = e.sender.text();
                                    },
                                    filtering: $rootScope.dropdownFilter,
                                },
                            }
                        }, 0);
                        if ($scope.model.applicantRelativeInAeons && $scope.model.applicantRelativeInAeons.length) {
                            for (let i = 0; i < $scope.model.applicantRelativeInAeons.length; i++) {
                                if ($scope.model.applicantRelativeInAeons[i].departmentId) {
                                    var res = await settingService.getInstance().departments.getDepartmentById({ id: $scope.model.applicantRelativeInAeons[i].departmentId }).$promise;
                                    if (res && res.isSuccess) {
                                        setDataDepartmentFromId([res.object], `departmentId${i}`);
                                        var dropdownlist = $(`#departmentId${i}`).data("kendoDropDownTree");
                                        dropdownlist.value(res.object.id);
                                    }
                                }
                            }
                        }


                    }
                    function setDataDepartmentFromId(dataDepartment, id) {
                        $scope.departments = dataDepartment;
                        var dataSource = new kendo.data.HierarchicalDataSource({
                            data: dataDepartment,
                            schema: {
                                model: {
                                    children: "items"
                                }
                            }
                        });
                        var dropdownlist = $(`#${id}`).data("kendoDropDownTree");
                        if (dropdownlist) {
                            dropdownlist.setDataSource(dataSource);
                        }


                    }

                    function showCustomDepartmentTitle(e) {
                        let model = e.item;
                        if (model.jobGradeGrade < 5) {
                            if (model.userCheckedHeadCount) {
                                return `${model.code} - ${model.name} - ${model.userCheckedHeadCount}(${model.jobGradeCaption})`
                            } else {
                                return `${model.code} - ${model.name} (${model.jobGradeCaption})`;
                            }
                        } else {
                            return `${model.code} - ${model.name} (${model.jobGradeCaption})`;
                        }
                    }
                    $scope.total = 0;
                    $scope.data = [];
                    $scope.currentQuery = {
                        predicate: "",
                        predicateParameters: [],
                        predicate: "Created desc",
                        limit: appSetting.pageSizeDefault,
                        page: 1
                    }

                    $scope.toggleSelection = function (items, value) {
                        var idx = items.indexOf(value);

                        if (idx > -1) items.splice(idx, 1);
                        else items.push(value);
                    };

                    $scope.formWeb = ['Jobstreet', 'Vieclam24h', 'Vietnamwork', 'Mywork', 'Ybox'];
                    $scope.possessOwnVehicleType = ['Car', 'Motobike'];
                    $scope.fromSchool = ['Website/ Email trường', 'Standee tại trường'];
                    $scope.fromOthers = ['Nộp trực tiếp tại siêu thị', 'Người quen giới thiệu', 'Hội thảo việc làm', 'Facebook', 'Băng-rôn(phướn)', 'Loa phường', 'Tờ rơi', 'Thông báo của phường, xã', 'Quảng cáo từ banner của các websites', 'Website Aeon', 'Chương trình đạp xe quảng cáo'];

                    var dupPositions = [];
                    $scope.countrySetItems = new kendo.data.ObservableArray([{ code: '', name: '' }]);
                    async function getCountrySet() {
                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                            name: 'Nationality',
                            parentCode: '',
                        }).$promise;
                        if (res.isSuccess && res.object.data.length > 0) {
                            $scope.countrySetItems.pop();
                            res.object.data.forEach(item => $scope.countrySetItems.push(item));
                        }
                    }

                    $scope.provinceSetItems = new kendo.data.ObservableArray([{ code: '', name: '' }]);
                    async function getProvinceSet() {
                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                            name: 'Province',
                            parentCode: '',
                        }).$promise;
                        if (res.isSuccess && res.object.data.length > 0) {
                            $scope.provinceSetItems.pop();
                            res.object.data.forEach(item => $scope.provinceSetItems.push(item));
                        }
                    }

                    $scope.districtSetItems = new kendo.data.ObservableArray([{ code: '', name: '' }]);
                    async function getDistrictSet(parentCodePermanent, parentCodeProvisional) {
                        if (parentCodePermanent) {
                            var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                name: 'District',
                                parentCode: parentCodePermanent,
                            }).$promise;
                            if (res.isSuccess && res.object.data.length > 0) {
                                $scope.districtSetItems.pop();
                                res.object.data.forEach(item => $scope.districtSetItems.push(item));
                            }
                        }
                        if (parentCodeProvisional) {
                            var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                name: 'District',
                                parentCode: parentCodeProvisional,
                            }).$promise;
                            if (res.isSuccess && res.object.data.length > 0) {
                                $scope.districtSetItems.pop();
                                res.object.data.forEach(item => $scope.districtSetItems.push(item));
                            }
                        }

                    }

                    $scope.wardSetItems = new kendo.data.ObservableArray([{ code: '', name: '' }]);
                    async function getWardSet(parentCodePermanentCity, parentCodePermanentOfDistrict, parentCodeProvisionalCity, parentCodeProvisionalOfDistrict) {
                        if (parentCodePermanentCity && parentCodePermanentOfDistrict) {
                            var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                name: 'Ward',
                                parentCode: `${parentCodePermanentCity},${parentCodePermanentOfDistrict}`
                            }).$promise;
                            if (res.isSuccess && res.object.data.length > 0) {
                                $scope.wardSetItems.pop();
                                res.object.data.forEach(item => $scope.wardSetItems.push(item));
                            }
                        }
                        if (parentCodeProvisionalCity && parentCodeProvisionalOfDistrict) {
                            var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                                name: 'Ward',
                                parentCode: `${parentCodeProvisionalCity},${parentCodeProvisionalOfDistrict}`
                            }).$promise;
                            if (res.isSuccess && res.object.data.length > 0) {
                                $scope.wardSetItems.pop();
                                res.object.data.forEach(item => $scope.wardSetItems.push(item));
                            }
                        }
                    }

                    $scope.religionItems = new kendo.data.ObservableArray([{ code: '', name: '' }]);
                    async function getReligion() {
                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                            name: "Religion",
                            parentCode: '',
                        }).$promise;
                        if (res.isSuccess && res.object.data.length > 0) {
                            $scope.religionItems.pop();
                            res.object.data.forEach(item => $scope.religionItems.push(item));
                        }
                    }

                    $scope.interestInWorkPriorityItems = new kendo.data.ObservableArray([{ code: '', name: '' }]);
                    // async function getListForInterestInWorkPriority() {
                    //     var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                    //         name: 'ImportantFactor',
                    //         parentCode: '',
                    //     }).$promise;
                    //     if (res.isSuccess && res.object.data.length > 0) {
                    //         $scope.interestInWorkPriorityItems.pop();
                    //         res.object.data.forEach(item => $scope.interestInWorkPriorityItems.push(item));
                    //     }
                    // }

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
                            res.object.data.forEach(item => $scope.positionItems.push(item));
                        }
                    }

                    $scope.relationItems = new kendo.data.ObservableArray([{
                        code: '',
                        name: ''
                    }]);
                    async function getRelation() {
                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                            name: 'Relationship',
                            parentCode: '',
                        }).$promise;
                        if (res.isSuccess && res.object.data.length > 0) {
                            $scope.relationItems.pop();
                            res.object.data.forEach(item => $scope.relationItems.push(item));
                        }
                    }

                    $scope.workingPlacesItems = new kendo.data.ObservableArray([{
                        code: '',
                        name: ''
                    }]);
                    async function getWorkingPlaces() {
                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                            name: 'WorkLocation',
                            parentCode: '',
                        }).$promise;
                        if (res.isSuccess && res.object.data.length > 0) {
                            $scope.workingPlacesItems.pop();
                            res.object.data.forEach(item => $scope.workingPlacesItems.push(item));
                        }
                    }
                    $scope.openPositions = new kendo.data.ObservableArray([{}]);
                    async function getOpenPositions() {
                        var res = await recruitmentService.getInstance().position.getOpenPositions().$promise;
                        if (res.isSuccess) {
                            $scope.openPositions.pop();
                            res.object.data.forEach(item => $scope.openPositions.push(item));
                        }
                    }

                    async function getAllListPositionForFilter() {
                        var res = await recruitmentService.getInstance().position.getAllListPositionForFilter().$promise;
                        if (res.isSuccess) {
                            $scope.myPositionItems = res.object.data;
                        }
                    }
                    async function getDepartmentByFilter(option) {
                        if (!option.filter) {
                            option.preventDefault();
                        } else {
                            let filter = option.filter && option.filter.value ? option.filter.value : "";
                            arg = {
                                predicate: "name.contains(@0) or code.contains(@1)",
                                predicateParameters: [filter, filter],
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

                    function showCustomDepartmentTitle(e) {
                        let model = e.item;
                        if (model.jobGradeGrade < 5) {
                            if (model.userCheckedHeadCount) {
                                return `${model.code} - ${model.name} - ${model.userCheckedHeadCount}(${model.jobGradeCaption})`
                            } else {
                                return `${model.code} - ${model.name} (${model.jobGradeCaption})`;
                            }
                        } else {
                            return `${model.code} - ${model.name} (${model.jobGradeCaption})`;
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
                        var dropdownlist = $("#departmentsearch").data("kendoDropDownTree");
                        if (dropdownlist) {
                            dropdownlist.setDataSource(dataSource);
                        }
                        else {
                            $scope.optionGroup.departmentTreeOptions.dataSource = dataSource;
                        }

                    }
                    $scope.addSubItem = function (list) {
                        if (!$scope.model[list])
                            $scope.model[list] = [];
                        $scope.model[list].push({})
                        // if(list == 'applicantRelativeInAeons'){
                        //     setDataDepartmentFromId(allDepartments, `departmentId${$scope.model.applicantRelativeInAeons.length -1}`);
                        // }
                    }
                    $scope.onChangeDate = function (item) {
                        var result = compare(item.fromDate, item.toDate);
                        if (result == 1) {
                            item.toDate = null;
                        }
                    };
                    $scope.onChangeGrossSalary = function (value, id) {
                        if (value) {
                            var estimateSalaryEnd = $(`#${id}`).data("kendoNumericTextBox");
                            estimateSalaryEnd.min(0);
                            estimateSalaryEnd.focus();
                        }
                    }
                    function compare(dateTimeA, dateTimeB) {
                        var momentA = moment(dateTimeA, "DD/MM/YYYY");
                        var momentB = moment(dateTimeB, "DD/MM/YYYY");
                        if (momentA > momentB) return 1;
                        else if (momentA < momentB) return -1;
                        else return 0;
                    }
                },
            ],
        };
    },
]);