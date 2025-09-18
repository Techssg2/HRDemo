var ssgApp = angular.module('ssg.applicantModule', ["kendo.directives"]);
ssgApp.controller('applicantController', function ($rootScope, $scope, $location,$translate, appSetting, $stateParams, $state, moment, commonData, recruitmentService, masterDataService, settingService, Notification, $timeout, fileService, localStorageService) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = '';
    $scope.advancedSearchMode = false;
    isItem = true;
    $scope.title = $stateParams.id ? $translate.instant('APPLICANT_MENU') + $stateParams.referenceValue : $state.current.name == 'home.applicant.item' ? "New item: Applicant" : $stateParams.action.title;
    $scope.titleEdit = '';
    $scope.filter = {
        keyword: '',
    }
    $scope.searchInfo = {
        keyword: '',
        status: '',
        store: '',
        position: '',
    }
    $rootScope.isParentMenu = false;
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }

    $scope.widget = {};
    var dupPositions = [];
    var mode = $stateParams.id ? 'Edit' : 'New';
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    var requiredFields = [
        {
            fieldName: 'fullName',
            title: "Full Name"
        },
        {
            fieldName: 'genderCode',
            title: "Gender"
        },
        {
            fieldName: 'idCard9Number',
            title: "ID Card (9 numbers)"
        },
        {
            fieldName: 'email',
            title: "Email Address"
        },
        {
            fieldName: 'mobile',
            title: "Mobile"
        },
        // {
        //     fieldName: 'idCard12Number',
        //     title: "ID Card (12 numbers)"
        // },
        {
            fieldName: 'positionId',
            title: "Applied Position"
        }
    ]


    function initSearch() {
        $scope.filter = {}
    }
    initSearch();

    $scope.clearSearch = async function () {
        initSearch();
        $scope.$broadcast('resetToDate', $scope.filter.toDate);
        var dropdowntree = $("#departmentsearch").data("kendoDropDownTree");
        dropdowntree.value('');
        $("#storeSearch").val("");
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }
        reloadGrid();
        clearSearchTextOnDropdownTree("departmentsearch");
        setDataDepartment(allDepartments);
    }

    $scope.toggleFilterPanel = async function (value) {
        $scope.advancedSearchMode = !$scope.advancedSearchMode;
        if ($scope.advancedSearchMode) {
            setDataDepartment(allDepartments);
        }
    }

    $scope.model = {
        //applicantRelativeInAeons: [],
        //applicantEducations: [],
        //applicantWorkingProcesses: [],
        knowJobFromWeb: [],
        knowJobFromSchool: [],
        knowJobFromOthers: [],
        // familyMembers: [],
        // employmentHistories: [],
        // characterReferences: [],
        // languageProficiencyEntries: [],
        //activities: [{}],
        type: ''
    };

    //$scope.districtSetItems = [];
    $scope.districtData = {
        permanentDistrictSetItems: new kendo.data.ObservableArray([{ code: '', name: '' }]),
        provisionalDistrictSetItems: new kendo.data.ObservableArray([{ code: '', name: '' }])

    }
    $scope.wardData = {
        permanentWardSetItems: new kendo.data.ObservableArray([{ code: '', name: '' }]),
        provisionalWardSetItems: new kendo.data.ObservableArray([{ code: '', name: '' }])
    }
    // $scope.districtSetItems = new kendo.data.ObservableArray([{ code: '', name: '' }]);
    this.$onInit = async () => {
        if ($state.current.name === 'home.applicant.item') {
            if ($stateParams.referenceValue) {
                await getApplicantsDetail();
            } else {
                $timeout(function () {
                    showConfirm('Create New Applicant');
                }, 0);
            }
            await getEducationList();
            await getSchoolList();
            await getRelations();
            await getWorkingTimes();
            await getListForInterestInWorkPriority();
        }
    };

    async function getDepartmentByReferenceNumber() {
        let referencePrefix = $stateParams.referenceValue.split('-')[0];
        let res = await settingService.getInstance().departments.getDepartmentByReferenceNumber({ prefix: referencePrefix, itemId: $stateParams.id }).$promise;
        if (res.isSuccess) {
            setDataDepartment(res.object.data);
        }
    }

    $scope.save = function () {
        console.log($scope.modal)
    }

    $scope.onChangeGender = function () {
        switch ($scope.model.genderCode) {
            case '1':
                $scope.model.genderName = 'Male';
                break;
            case '4':
                $scope.model.genderName = 'Female';
                break;
            default:
                $scope.model.genderName = null;
                break;
        }
    }
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));


    $scope.applicantGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getApplicants(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                //data: () => { return $scope.data }
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
            width: "40px",
            locked: true,
        },
        {
            field: "applicantStatusName",
            title: "Status",
            width: "300px",
            locked: true,
            lockable: false,
            template: function (dataItem) {
                //return dataItem.applicantStatusName ? `<label>${dataItem.applicantStatusName}</label>` : 'Initial';
                return dataItem.applicantStatusName ? `<label>${dataItem.applicantStatusName}</label>` : '';
            }
        },
        {
            field: "referenceNumber",
            title: "Reference Number",
            width: "180px",
            locked: true,
            template: function (dataItem) {
                //return "<a ui-sref='home.applicant.item({ referenceValue: dataItem.id })' ui-sref-active='active'>" + kendo.htmlEncode(dataItem.referenceNumber) + "</a>";
                return `<a ui-sref="home.applicant.item({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "fullName",
            title: "Full Name",
            width: "200px"
        },
        {
            field: "positionName",
            title: "Position",
            width: "180px"
        },
        {
            field: "idCard9Number",
            title: "ID Card (9 numbers)",
            width: "180px"
        },
        {
            field: "idCard12Number",
            title: "ID Card (12 numbers)",
            width: "180px"
        },
        {
            field: "deptDivision",
            title: 'Department',
            width: "300px"
        },
        {
            field: "dateOfBirth",
            title: 'Date Of Birth',
            width: "200px",
            template: function (dataItem) {
                if (dataItem && dataItem.dateOfBirth !== null) {
                    return moment(dataItem.dateOfBirth).format(appSetting.sortDateFormat);
                }
                return '';
            },
        },
        {
            field: "created",
            title: 'Created Date',
            width: "200px",
            template: function (dataItem) {
                if (dataItem && dataItem.created !== null) {
                    return moment(dataItem.created).format('DD/MM/YYYY HH:mm');
                }
                return '';
            }
        }
        ],
        dataBound: function (e) {
        },
    };
    async function getApplicants(option) {
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        if ($state.current.name == commonData.myRequests.Applicant) {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        }

        var res = await recruitmentService.getInstance().applicants.getApplicantList(
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
            option.success($scope.data);
        } else {
            $scope.widget.applicantList.dataSource.read();
        }
    }

    async function getApplicantsDetail() {
        var res = await recruitmentService.getInstance().applicants.getApplicantList({
            predicate: `Id.ToString() == @0`,
            predicateParameters: [$stateParams.id],
            order: "Created desc",
            limit: 1,
            page: 1
        }).$promise;
        if (res && res.object.data.length === 1) {
            let parentCodePermanentOfDistrict = res.object.data[0].permanentResidentCityCode;
            let parentCodePermanentOfWard = res.object.data[0].permanentResidentDistrictCode;
            let parentCodeProvisionalOfDistrict = res.object.data[0].provisionalResidentCityCode;
            let parentCodeProvisionalOfWard = res.object.data[0].provisionalResidentDistrictCode;

            await getDistrictSet(parentCodePermanentOfDistrict, parentCodeProvisionalOfDistrict);
            await getWardSet(parentCodePermanentOfDistrict, parentCodePermanentOfWard, parentCodeProvisionalOfDistrict, parentCodeProvisionalOfWard);

            $timeout(function () {
                $scope.model = _.cloneDeep(res.object.data[0]);
                $scope.model.applicantEducations = _.orderBy($scope.model.applicantEducations, ['fromDate', 'toDate'], ['asc', 'asc']);
                $scope.model.applicantWorkingProcesses = _.orderBy($scope.model.applicantWorkingProcesses, ['fromDate', 'toDate'], ['asc', 'asc']);
                $scope.model.applicantRelativeInAeons = _.orderBy($scope.model.applicantRelativeInAeons, ['fullName'], ['asc']);
                $scope.model.familyMembers = _.orderBy($scope.model.familyMembers, ['name'], ['asc']);
                $scope.model.languageProficiencyEntries = _.orderBy($scope.model.languageProficiencyEntries, ['language'], ['asc']);
                $scope.model.activities = _.orderBy($scope.model.activities, ['name'], ['asc']);


                $scope.model.skillsCode = $scope.model.skillsCode ? JSON.parse($scope.model.skillsCode) : null;
                $scope.model.abilitiesCode = $scope.model.abilitiesCode ? JSON.parse($scope.model.abilitiesCode) : null;
                $scope.model.languagesCode = $scope.model.languagesCode ? JSON.parse($scope.model.languagesCode) : null;

                $scope.model.knowJobFromWeb = JSON.parse($scope.model.knowJobFromWeb);
                $scope.model.knowJobFromSchool = JSON.parse($scope.model.knowJobFromSchool);
                $scope.model.knowJobFromOthers = JSON.parse($scope.model.knowJobFromOthers);
                $scope.model.possessOwnVehicleType = JSON.parse($scope.model.possessOwnVehicleType);

                let temp = [{ priority: 1, positionId: null }, { priority: 2, positionId: null }, { priority: 3, positionId: null }];
                $scope.model.positionApplicantMappings.forEach(i => {
                    temp[i.priority - 1] = i;
                });
                $scope.model.positionApplicantMappings = temp;
                $timeout(function () {
                    $scope.titleEdit = "APPLICANT:" + $scope.model.referenceNumber;
                }, 10);


            }, 0);
        }
    }

    async function getDistrictSet(parentCodePermanent, parentCodeProvisional) {
        if (parentCodePermanent) {
            var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                name: 'District',
                parentCode: parentCodePermanent,
            }).$promise;
            if (res.isSuccess && res.object.data.length > 0) {
                $scope.districtData.permanentDistrictSetItems = [];
                res.object.data.forEach(item => $scope.districtData.permanentDistrictSetItems.push(item));

            }
        }
        if (parentCodeProvisional) {
            var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                name: 'District',
                parentCode: parentCodeProvisional,
            }).$promise;
            if (res.isSuccess && res.object.data.length > 0) {
                $scope.districtData.provisionalDistrictSetItems = [];
                res.object.data.forEach(item => $scope.districtData.provisionalDistrictSetItems.push(item));
            }
        }

    }
    async function getEducationList() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: "EducationalEst",
            parentCode: '',
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.masterDatas.educationList = res.object.data;
        }
    }
    async function getSchoolList() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: "Institute",
            parentCode: '',
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.masterDatas.schools = res.object.data;
        }
    }
    async function getRelations() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'Relationship',
            parentCode: '',
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.masterDatas.relationships = res.object.data;
        }
    }
    async function getWorkingTimes() {
        var res = await settingService.getInstance().recruitment.getWorkingTime({
            predicate: "",
            predicateParameters: [],
            order: "created desc",
            page: 1,
            limit: 1000,
        }).$promise;
        if (res.isSuccess) {
            $scope.masterDatas.workingTimes = res.object.data;
        }
    }
    async function getListForInterestInWorkPriority() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'ImportantFactor',
            parentCode: '',
        }).$promise;
        if (res.isSuccess && res.object.data.length > 0) {
            $scope.masterDatas.interestInWorkPriorityItems = res.object.data;
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
                $scope.wardData.permanentWardSetItems = [];
                res.object.data.forEach(item => $scope.wardData.permanentWardSetItems.push(item));
            }
        }
        if (parentCodeProvisionalCity && parentCodeProvisionalOfDistrict) {
            var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                name: 'Ward',
                parentCode: `${parentCodeProvisionalCity},${parentCodeProvisionalOfDistrict}`
            }).$promise;
            if (res.isSuccess && res.object.data.length > 0) {
                $scope.wardData.provisionalWardSetItems = [];
                res.object.data.forEach(item => $scope.wardData.provisionalWardSetItems.push(item));
            }
        }
    }
    let args = {
        limit: 1000000,
        order: "Modified desc",
        page: 1,
        predicate: "",
        predicateParameters: [],
    }
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
        },
        positionForListOptions: {
            placeholder: "",
            dataTextField: "positionName",
            dataValueField: "id",
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
        positionForImportApplicant: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: {
                serverPaging: false,
                //pageSize: 10000,
                // transport: {
                //     read: async function (e) {
                //         const res = [
                //             {id: "282fb95f-0c3a-4a1a-827a-853a472c3e94", name:"Support Leader (G2)"},
                //             {id: "2dc6ecff-e25a-46c4-a25e-667fd77a0a8e", name:"Deputy Manager"},
                //         ];
                //         e.success(res);
                //     }
                // },
                // schema: {
                //     model: {
                //         children: "items"
                //     }
                // },
                dataSource: []
            },
        },
        statusForListOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: {
                serverPaging: false,
                //pageSize: 10000,
                transport: {
                    read: async function (e) {
                        // var res = await settingService.getInstance().recruitment.getApplicantStatus(args).$promise;
                        // if (res.isSuccess) {
                        //     e.success(res.object.data);
                        // }
                        const res = commonData.statusImportMass;
                        e.success(res);
                    }
                },
                schema: {
                    model: {
                        children: "items"
                    }
                },
            },
        },
        storeForListOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: {
                serverPaging: false,
                dataSource: []
            },
        },
    }

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
        var dropdownlist = $("#departmentsearch").data("kendoDropDownTree");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
        else {
            $scope.optionGroup.departmentTreeOptions.dataSource = dataSource;
        }

    }

    $scope.collectionIndex = function (list, value) {
        var index = _.findIndex(list, x => {
            return x === 'Part Time';
        });
        return index;
    };

    function validateForm(form, requiredFields, model) {
        var messages = "";
        var errorObject = {
            errorField: { title: 'Some fields are required: ', array: [] },
            errorFormatField: { title: 'Some fields are incorrect format: ', array: [] }
        }
        // Required Fields
        if (requiredFields.length) {
            requiredFields.forEach(x => {
                if (!model[x.fieldName] && (x.fieldName == "mobile" || x.fieldName == "genderCode" || x.fieldName == "idCard9Number" || x.fieldName == "positionId")) {
                    errorObject.errorField.array.push(`<li> - ${x.title}</li>`);
                }
                if (mode == 'New') {
                    if (x.fieldName == "email") {
                        var errorFormats = validateEmail(form[x.fieldName].$viewValue);
                        if (errorFormats.length) {
                            errorFormats.forEach(ele => {
                                errorObject.errorFormatField.array.push(`<li> - ${ele}</li>`)
                            });
                        }
                    }
                    else if (x.fieldName == "idCard9Number") {
                        var errorFormats = validateNumber(form[x.fieldName].$viewValue, 'idCard9Number');
                        if (errorFormats.length) {
                            errorFormats.forEach(ele => {
                                errorObject.errorFormatField.array.push(`<li> - ${ele}</li>`)
                            });
                        }
                    }
                    else if (x.fieldName == "idCard12Number") {
                        var errorFormats = validateNumber(form[x.fieldName].$viewValue, 'idCard12Number');
                        if (errorFormats.length) {
                            errorFormats.forEach(ele => {
                                errorObject.errorFormatField.array.push(`<li> - ${ele}</li>`)
                            });
                        }
                    }
                    else if (x.fieldName == "mobile") {
                        var errorFormats = validateNumber(form['phoneNumber'].$viewValue, 'mobile');
                        if (errorFormats.length) {
                            errorFormats.forEach(ele => {
                                errorObject.errorFormatField.array.push(`<li> - ${ele}</li>`)
                            });
                        }
                    }
                }
            });
        }

        if (errorObject.errorField.array.length || errorObject.errorFormatField.array.length) {
            if (errorObject.errorField.array.length) {
                var title = errorObject.errorField.title + "</br>";
                var subContent = "<ul>" + errorObject.errorField.array.join('') + "</ul>";
                messages += title + subContent;
            }
            if (errorObject.errorFormatField.array.length) {
                var title = errorObject.errorFormatField.title + "</br>";
                var subContent = "<ul>" + errorObject.errorFormatField.array.join('') + "</ul>";
                messages += title + subContent;
            }
            Notification.error(messages);
            return false;
        }
        return true;
    }

    function validateEmail(email) {
        var errorMessages = [];
        if (email) {
            var regrex = /^([a-zA-Z0-9_\.\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/i;
            if (!regrex.test(email)) {
                errorMessages.push('Email');
            }
        }
        return errorMessages;
    }

    function validateNumber(number, title) {
        var errorMessages = [];
        if (title == 'idCard9Number') {
            let regex = new RegExp("^\\d{9}$");
            if (!regex.test(number)) {
                errorMessages.push('ID Card (9 numbers)');
            }
        }
        if (title == 'mobile' && number) {
            let rege = new RegExp("^\\d{10}$");
            if (!rege.test(number)) {
                errorMessages.push('Mobile');
            }
        }
        if (title == 'idCard12Number' && number) {
            let rege = new RegExp("^\\d{12}$");
            if (!rege.test(number)) {
                errorMessages.push('ID Card (12 numbers)');
            }
        }

        return errorMessages;
    }

    $scope.errors = [];
    function beforeSubmit() {
        if (!$scope.model.haveRelativeInAeon) {
            $scope.model.applicantRelativeInAeons = [];
        }
        if (!$scope.model.hasEmailAddress) {
            $scope.model.email = '';
        }
        if (!$scope.model.haveIdentifyCardNumber12) {
            $scope.model.idCard12Number = '';
            $scope.model.idCard12Date = '';
            $scope.model.idCard12PlaceCode = '';
        }
        if (!$scope.model.havePassport) {
            $scope.model.passportNo = '';
            $scope.model.passportType = '';
        }
        if (!$scope.model.hasConvictedForCrime) {
            $scope.model.convictedForCrimeNotes = '';
        }
        if (!$scope.model.hasHealthProblems) {
            $scope.model.healthProblemsNotes = '';
        }
        if (!$scope.model.usedToInterviewInCompany) {
            $scope.model.convictedForCrimeNotes = '';
        }
    }
    $scope.submit = async function (form) {
        $scope.model.applicantRelativeInAeons = removeEmptyRow($scope.model.applicantRelativeInAeons)
        $scope.model.applicantEducations = removeEmptyRow($scope.model.applicantEducations)
        if ($scope.model.type == 1) {
            $scope.model.applicantWorkingProcesses = removeEmptyRow($scope.model.applicantWorkingProcesses)
        }
        if ($scope.model.type == 2) {
            $scope.model.familyMembers = removeEmptyRow($scope.model.familyMembers)
            $scope.model.employmentHistories = removeEmptyRow($scope.model.employmentHistories)
            $scope.model.characterReferences = removeEmptyRow($scope.model.characterReferences)
            $scope.model.languageProficiencyEntries = removeEmptyRow($scope.model.languageProficiencyEntries)
            $scope.model.activities = removeEmptyRow($scope.model.activities)

            $scope.model.knowJobFromWeb = null;
            $scope.model.knowJobFromSchool = null;
            $scope.model.knowJobFromOthers = null;
        }
        let checkForm = validateForm(form, requiredFields, $scope.model);
        if (checkForm) {
            $scope.errors = [];
            if (dupPositions.length > 0) {
                $scope.errors.push({ controlName: dupPositions.join(', '), message: 'Fields are Duplicated' });
            }
            let customDateFields = [
                // {
                //     fieldName: 'dateOfBirth', title: 'Date Of Birth', type: 'date', range: { greaterThan: 14, lessThan: 65, }
                // },
                {
                    fieldName: 'idCardDate9', title: 'ID Card(9) Date', type: 'date'
                },
                {
                    fieldName: 'idCardDate12', title: 'ID Card(12) Date', type: 'date'
                },
            ];
            let dateErrors = $rootScope.dateValidator(customDateFields, $scope.model);
            if (dateErrors) {
                $scope.errors = $scope.errors.concat(dateErrors)
            }
            if ($scope.errors && $scope.errors.length > 0) {
                return;
            }
            let data = angular.copy($scope.model);
            //return;
            beforeSubmit();

            if ($scope.model.type == '1') {
                data.skillsCode = data.skillsCode ? JSON.stringify(data.skillsCode) : null;
                data.skillsName = JSON.stringify($scope.widget.skills.dataItems().map(i => i.name));

                data.abilitiesCode = data.abilitiesCode ? JSON.stringify(data.abilitiesCode) : null;
                data.abilitiesName = JSON.stringify($scope.widget.abilities.dataItems().map(i => i.name));
                data.languagesCode = data.languagesCode ? JSON.stringify(data.languagesCode) : null;
                data.languagesName = JSON.stringify($scope.widget.languages.dataItems().map(i => i.name));

                data.knowJobFromWeb = JSON.stringify(data.knowJobFromWeb);
                data.knowJobFromSchool = JSON.stringify(data.knowJobFromSchool);
                data.knowJobFromOthers = JSON.stringify(data.knowJobFromOthers);
                data.possessOwnVehicleType = JSON.stringify(data.possessOwnVehicleType);
            } else {

            }
            var res;
            if ($scope.model.id) {
                res = await recruitmentService.getInstance().applicants.updateApplicants(data).$promise;
            } else {
                res = await recruitmentService.getInstance().applicants.createApplicants(data).$promise;
            }

            if (res.isSuccess) {
                if (res.object && !$scope.model.id) {
                    $scope.model.id = res.object.id;
                    $scope.model.referenceNumber = res.object.referenceNumber;
                    $state.go('home.applicant.item', { id: res.object.id, referenceValue: res.object.referenceNumber });
                }
                Notification.success("Data successfully saved");
            } else { }

        }

    }

    // $scope.goBack = function () {
    //     $state.go('home.dashboard', {}, { reload: true });
    // }

    $scope.toggleSelection = function (items, value) {
        var idx = items.indexOf(value);

        if (idx > -1) items.splice(idx, 1);
        else items.push(value);
    };

    var columsSearch = ['ReferenceNumber.contains(@0)', 'FullName.contains(@1)', 'Position.DeptDivision.Name.contains(@2)', 'Position.PositionName.contains(@3)', 'IdCard9Number.contains(@4)', 'IdCard12Number.contains(@5)', 'ApplicantStatus.Name.contains(@6)']
    var priorities = ['1st Position', '2nd Position', '3rd Position']

    $scope.search = async function () {
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }
        var option = $scope.currentQuery;
        if ($scope.filter.keyword) {
            option.predicate = `(${columsSearch.join(" or ")})`;
            for (let index = 0; index < columsSearch.length; index++) {
                option.predicateParameters.push($scope.filter.keyword);
            }
            option.predicateParameters.push($scope.filter.keyword);
        }
        if ($scope.filter.department) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(Position.DeptDivisionId = @${option.predicateParameters.length})`;
            option.predicateParameters.push($scope.filter.department);
        }
        if ($scope.filter.position) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(PositionId = @${option.predicateParameters.length})`;
            option.predicateParameters.push($scope.filter.position);
        }
        if ($scope.filter.gender) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `GenderCode = @${option.predicateParameters.length}`;
            option.predicateParameters.push($scope.filter.gender);
        }
        if ($scope.filter.fromDate) {
            var date = moment($scope.filter.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created >= @${option.predicateParameters.length}`;
            option.predicateParameters.push(date);
        }
        if ($scope.filter.toDate) {
            var date = moment($scope.filter.toDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created <= @${option.predicateParameters.length}`;
            option.predicateParameters.push(date);
        }
        $scope.currentQueryExport = option;
        reloadGrid();
    }

    function reloadGrid() {
        let grid = $("#applicantGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
            if ($scope.advancedSearchMode) {
                grid.dataSource.page(1);
            }
        }
    }

    function compare(dateTimeA, dateTimeB) {
        var momentA = moment(dateTimeA, "DD/MM/YYYY");
        var momentB = moment(dateTimeB, "DD/MM/YYYY");
        if (momentA > momentB) return 1;
        else if (momentA < momentB) return -1;
        else return 0;
    }

    $scope.onChangeDate = function (item) {
        var result = compare(item.fromDate, item.toDate);
        if (result == 1) {
            item.toDate = null;
        }
    }

    $scope.currentQueryExport = {
        Predicate: "",
        PredicateParameters: [],
        Order: "Created desc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    }

    $scope.export = async function () {
        let option = $scope.currentQueryExport;
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.APPLICANT }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }
    $scope.isG1 = '1';
    ssg.confirmDialog = {};
    $scope.applicantType = '1';
    $scope.confirm = function () {
        //alert($scope.isG1);
        if ($scope.applicantType == '3') {
            $scope.searchInfo = {};
            openImportApplicant();
        } else {
            $scope.model.type = $scope.applicantType;
        }
        $scope.isConfirmed = true;
        ssg.confirmDialog.close();
    }
    function showConfirm(title) {

        ssg.dialogConfirm = $("#confirmGrade").kendoDialog({
            title: title,
            width: "300px",
            modal: true,
            visible: false,
            // closable: true,
            animation: {
                open: {
                    effects: "fade:in"
                }
            },
            // close: function () {
            //     if (!$scope.isOpenImportApplicant && !$scope.isConfirmed) {
            //         $state.go('home.dashboard');
            //     }
            // }
        });
        ssg.confirmDialog = ssg.dialogConfirm.data("kendoDialog");
        ssg.confirmDialog.open();
        $rootScope.confirmGrade = ssg.confirmDialog;
        return ssg.confirmDialog;
    }
    $scope.removeSubItem = function (list, $index, title) {
        if (title == 'relativeInAeon') {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this applicant Relative In Aeon ?', 'Confirm');
            $scope.dialog.bind("close", confirm = function (e) {
                if (e.data && e.data.value) {
                    $scope.model.applicantRelativeInAeons.splice($scope.index, 1);
                    Notification.success("Data Successfully Deleted");
                }
            });
        }
        else if (title == 'education') {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this applicant Educations ?', 'Confirm');
            $scope.dialog.bind("close", confirm = function (e) {
                if (e.data && e.data.value) {
                    $scope.model.applicantEducations.splice($scope.index, 1);
                    Notification.success("Data Successfully Deleted");
                }
            });
        }
        else if (title == 'workingProcesses') {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this applicant Working Processes ?', 'Confirm');
            $scope.dialog.bind("close", confirm = function (e) {
                if (e.data && e.data.value) {
                    $scope.model.applicantWorkingProcesses.splice($scope.index, 1);
                    Notification.success("Data Successfully Deleted");
                }
            });
        }
        else if (title == 'familyMembers') {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this applicant Particulars of Immediate Family ?', 'Confirm');
            $scope.dialog.bind("close", confirm = function (e) {
                if (e.data && e.data.value) {
                    $scope.model.familyMembers.splice($scope.index, 1);
                    Notification.success("Data Successfully Deleted");
                }
            });
        } else if (title == 'employmentHistories') {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this applicant Employ History?', 'Confirm');
            $scope.dialog.bind("close", confirm = function (e) {
                if (e.data && e.data.value) {
                    $scope.model.employmentHistories.splice($scope.index, 1);
                    Notification.success("Data Successfully Deleted");
                }
            });
        } else if (title == 'characterReferences') {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this applicant Character References?', 'Confirm');
            $scope.dialog.bind("close", confirm = function (e) {
                if (e.data && e.data.value) {
                    $scope.model.characterReferences.splice($scope.index, 1);
                    Notification.success("Data Successfully Deleted");
                }
            });
        }
        else if (title == 'languageProficiencyEntries') {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this applicant Language Proficiency?', 'Confirm');
            $scope.dialog.bind("close", confirm = function (e) {
                if (e.data && e.data.value) {
                    $scope.model.languageProficiencyEntries.splice($scope.index, 1);
                    Notification.success("Data Successfully Deleted");
                }
            });
        }
        else if (title == 'activities') {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this applicant Activities?', 'Confirm');
            $scope.dialog.bind("close", confirm = function (e) {
                if (e.data && e.data.value) {
                    $scope.model.activities.splice($scope.index, 1);
                    Notification.success("Data Successfully Deleted");
                }
            });
        }
        //list.splice($index, 1);
    }
    function removeEmptyRow(arrayData) {
        if (arrayData) {
            let newArray = [];
            arrayData.forEach(function (row) {
                if (!isEmptyObject(row)) {
                    newArray.push(row)
                }
            });
            return newArray;
        }
    }
    function isEmptyObject(obj) {
        for (var key in obj) {
            if (obj.hasOwnProperty(key) && key != "$$hashKey")
                return false;
        }
        return true;
    }
    $scope.dialogImportApplicantFromMassOption = {
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
            text: "Close",
            enable: false,
            action: function (e) {
                $scope.applicantsGrid = [];
                $scope.searchInfo.keyword = '';
                setApplicantGrid($scope.applicantsGrid, 0, 1)
                let dialog = $("#dialogImportApplicantFromMass").data("kendoDialog");
                dialog.close();
                $scope.closeSearchDialog(true);
                return false;
            },
            primary: true
        }]
    };
    $scope.applicantsFromMassOptions = {
        dataSource: {
            serverPaging: true,
            data: $scope.applicantsGrid,
            pageSize: 10,
        },
        sortable: false,
        scrollable: false,
        pageable: {
            alwaysVisible: true,
            responsive: false
        },
        //editable: true,
        columns: [
            {
                field: "no",
                title: "No",
                width: "50px",
            },
            {
                field: "applicantType",
                title: "Type",
                width: "50px",
            },
            {
                field: "name",
                title: "Applicant",
                width: "200px",
            },
            {
                field: "idCardNumber",
                title: "ID Card (9 numbers)",
                width: "200px",
            },
            {
                field: "idCard12Number",
                title: "ID Card (12 numbers)",
                width: "200px",
            },
            {
                field: "phoneNumber",
                title: "Phone Number",
                width: "50px",
            },
            {
                field: "dateOfBirth",
                title: "Birthday",
                width: "100px",
                template: function (dataItem) {
                    if (dataItem && dataItem.dateOfBirth !== null) {
                        return moment(dataItem.dateOfBirth).format(appSetting.sortDateFormat);
                    }
                    return '';
                },
            },
            {
                field: "position1Name",
                title: "Position 1",
                width: "200px",
            },
            {
                field: "position1Name",
                title: "Position 2",
                width: "200px",
            },
            {
                field: "position1Name",
                title: "Position 3",
                width: "200px",
            },
            {
                field: "runningNumber",
                title: "Running Number",
                width: "150px",
            },
            {
                title: "Actions",
                width: "100px",
                template: function (dataItem) {
                    return `<a class="btn btn-sm default green-stripe" ng-click="selectApplicant(dataItem.id)">Select</a>`;
                }
            }

        ],
        page: async function (e) {
            $scope.applicantsGrid = [];
            $scope.allCheck = false;
            await searchApplicant($scope.searchInfo.keyword, e.page, 10);
        }
    };
    async function openImportApplicant() {
        //setDataSapCode($scope.employeesDataSource)
        $scope.userGrid = [];
        let count = 0;
        $scope.isOpenImportApplicant = true;
        let dialog = $("#dialogImportApplicantFromMass").data("kendoDialog");
        dialog.title('Import Applicant From Mass');
        dialog.open();
        await searchApplicant("", 1, 10);
        //get Store
        await getMassStores();
        //get Positions
        await getMassPositions();
    }
    $scope.searchApplicant = function (page = 1, limit = 10) {
        searchApplicant($scope.searchInfo.keyword, page, limit);
    }
    $scope.ifEnter = function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            searchApplicant($scope.searchInfo.keyword, 1, 10);
        }
    }
    async function searchApplicant(searchText, page, limit) {
        var query = {
            predicate: "(PassportNo.contains(@0) || IdCardNumber.contains(@0) || IdCard12Number.contains(@0) || name.contains(@0))",
            predicateParameters: [searchText],
            order: "created desc",
            page: page,
            limit: limit
        }
        let args = GenarateQueryImportApplicant(query);
        let res = await recruitmentService.getInstance().applicants.searchApplicant(args).$promise;
        if (res && res.object) {
            let n = (page - 1) * limit + 1;
            $scope.applicantsGrid = res.object.data.items.map(function (item) {
                return { ...item, no: n++ }
            });
            setApplicantGrid($scope.applicantsGrid, res.object.data.count, page)
        }
        await getAllAppreciationList();

    }

    function GenarateQueryImportApplicant(query) {
        //&& ( interviewResult=="Không đạt")
        if ($scope.searchInfo.position) {
            query.predicate = query.predicate + `&& ( position1Id =@${query.predicateParameters.length} || position2Id = @${query.predicateParameters.length} || position3Id=@${query.predicateParameters.length})`;
            query.predicateParameters.push($scope.searchInfo.position);
        }
        if ($scope.searchInfo.status) {
            if($scope.searchInfo.status != commonData.checkStatusImportMass.All) {
                let result = commonData.statusImportMass.find(x => x.code == $scope.searchInfo.status);
                query.predicate = query.predicate + ` && ` + result.value;
            }
        }
        if ($scope.searchInfo.store) {
            query.predicate = query.predicate + ` && ( Store=@${query.predicateParameters.length} )`;
            var result = $scope.dataMassLocations.find(x => x.code == $scope.searchInfo.store);
            query.predicateParameters.push(result.name);
        }
        return query;

    }

    $scope.reset = function () {
        $scope.searchInfo = {};
        $scope.searchApplicant();
    }
    //API
    async function getAllAppreciationList() {
        if (!$scope.appreciations) {
            let QueryArgs = {
                predicate: '',
                predicateParameters: [],
                order: appSetting.ORDER_GRID_DEFAULT,
                page: 1,
                limit: 1000
            };

            let result = await settingService.getInstance().recruitment.getAppreciationList(QueryArgs).$promise;
            if (result && result.object) {
                $scope.appreciations = result.object.data;
            }
        }
    }
    function setApplicantGrid(data, total, pageIndex) {
        let grid = $('#applicantsFromMass').data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            data: data,
            pageSize: 10,
            page: pageIndex,
            serverPaging: true,
            schema: {
                total: function () {
                    return total;
                }
            },
        });
        grid.setDataSource(dataSource);
    }
    $scope.closeSearchDialog = function (openDialog_Detail) {
        $scope.dialogVisible = false;
        $scope.isOpenImportApplicant = false;
        if (openDialog_Detail && !$scope.reviewOk) {
            let dialog = $("#confirmGrade").data("kendoDialog");
            dialog.title('Create New Applicant');
            dialog.open();
        }
    }
    getApplicantDetailFromId = async function (id) {
        let resultData = null;
        let res = await recruitmentService.getInstance().applicants.getApplicantDetail({ Id: id }, {}).$promise;
        if (res.isSuccess && res.object && res.object.success) {
            resultData = res.object.data;
        }
        return resultData;
    }
    $scope.masterDatas = {
        educationList: [],
        schools: [],
        relationships: [],
        workingTimes: []
    }

    $scope.selectApplicant = async function (id) {
        var dataItem = await getApplicantDetailFromId(id);
        $timeout(async function () {
            $scope.reviewOk = true;
            let dialog1 = $("#confirmGrade").data("kendoDialog");
            dialog1.close();
            let dialog2 = $("#dialogImportApplicantFromMass").data("kendoDialog");
            dialog2.close();
            if (dataItem.applicantType.includes('G1')) {
                $scope.applicantType = '1';
            } else {
                $scope.applicantType = '2';
            }
            $scope.model.type = $scope.applicantType;
            $scope.model.fullName = dataItem.name;
            $scope.model.genderCode = dataItem.gender == 'Male' ? '1' : '4';
            $scope.model.genderName = dataItem.genderTitle;
            $scope.model.dateOfBirth = dataItem.dateOfBirth;
            $scope.model.idCard9Number = dataItem.idCardNumber;
            $scope.model.idCard9Date = new Date(dataItem.idCardIssuedDate);
            $scope.model.idCard9PlaceCode = dataItem.idCardIssuedPlace;
            $scope.model.permanentResidentAddress = dataItem.permanentAddress;
            $scope.model.permanentResidentCityCode = dataItem.permanentCity;
            $scope.model.permanentResidentDistrictCode = dataItem.permanentDistrict;
            $scope.model.permanentResidentWardCode = dataItem.permanentWard;
            $scope.model.provisionalResidentAddress = dataItem.contactAddress;
            $scope.model.provisionalResidentCityCode = dataItem.contactCity;
            $scope.model.provisionalResidentDistrictCode = dataItem.contactDistrict;
            $scope.model.provisionalResidentWardCode = dataItem.contactWard;
            $scope.model.hasConvictedForCrime = dataItem.hasConvictedForCrime;
            $scope.model.convictedForCrimeNotes = dataItem.convictedForCrimeNotes;
            $scope.model.hasHealthProblem = dataItem.hasHealthProblems;
            $scope.model.informationAboutHealthProblem = dataItem.healthProblemsNotes;
            $scope.model.hasPregnant = dataItem.isPregnant;
            $scope.model.haveChildrenUnder12Month = dataItem.hasChildrenUnder12Months;
            $scope.model.hasEmailAddress = dataItem.emailAddress ? 'true' : 'false';
            $scope.model.email = dataItem.emailAddress;
            $scope.model.usedToInterviewInCompany = dataItem.usedToInterviewInCompany;
            $scope.model.emergencyContactNumber = dataItem.emergencyContactNumber;
            $scope.model.emergencyContactPerson = dataItem.emergencyContactPerson;
            $scope.model.birthPlaceCode = dataItem.placeOfBirth;
            $scope.model.positionId = dataItem.position1Id;
            $scope.model.educationLevelCode = searchCodeByName($scope.masterDatas.educationList, dataItem.educationalDegree);
            $scope.model.applicantEducations = convertEducations(dataItem.educationalDetails);
            $scope.model.familyMembers = convertFamilyMembers(dataItem.familyMembers);
            await getDistrictSet($scope.model.permanentResidentCityCode, $scope.model.provisionalResidentCityCode);
            await getWardSet($scope.model.permanentResidentCityCode, $scope.model.permanentResidentDistrictCode, $scope.model.provisionalResidentCityCode, $scope.model.provisionalResidentDistrictCode);
            let importantFactorValues = [];
            if (dataItem.importantFactorValues) {
                importantFactorValues = dataItem.importantFactorValues.split(';');
            }
            if (dataItem.bloodGroup) {
                switch (dataItem.bloodGroup) {
                    case 'A':
                        $scope.model.bloodGroup = 1;
                        break;
                    case 'B':
                        $scope.model.bloodGroup = 2;
                        break;
                    case 'O':
                        $scope.model.bloodGroup = 3;
                        break;
                    case 'AB':
                        $scope.model.bloodGroup = 4;
                        break;
                    case 'NA':
                        $scope.model.bloodGroup = 5;
                        break;
                }
            }
            if (dataItem.marialStatus) {
                if (dataItem.marialStatus == 'Single') {
                    $scope.model.maritalStatus = 1;
                } else if (dataItem.marialStatus == 'Married') {
                    $scope.model.maritalStatus = 2;
                } else {
                    $scope.model.maritalStatus = 3;
                }
            }
            $scope.model.haveRelativeInAeon = dataItem.hasFamilyMembersInCompany;
            $scope.model.haveIdentifyCardNumber12 = false;
            if (dataItem.hasId12Card) {
                $scope.model.haveIdentifyCardNumber12 = true;
                $scope.model.idCard12Date = new Date(dataItem.idCard12IssuedDate);
                $scope.model.iDCard12Date = dataItem.idCard12PlaceCode;
                $scope.model.idCard12Number = dataItem.idCard12Number;
            }
            $scope.model.havePassport = false;
            if (dataItem.passportNo) {
                $scope.model.havePassport = true;
                $scope.model.passportNo = dataItem.passportNo;
                $scope.model.passportType = dataItem.passportType;
            }

            $scope.model.mobile = dataItem.phoneNumber;
            if (dataItem.usedToInterviewInCompany) {
                $scope.model.haveWorkedInAeon = dataItem.usedToInterviewInCompany;
                $scope.model.interviewedForVacancyInCompanyNotes = dataItem.interviewedForVacancyInCompanyNotes;
            }
            if ($scope.applicantType == '1') {
                $scope.model.applicantAppliedPosition1 = dataItem.position1Name;
                $scope.model.applicantAppliedPosition2 = dataItem.position2Name;
                $scope.model.applicantAppliedPosition3 = dataItem.position3Name;
                $scope.model.expectedSalary = dataItem.grossSalary1;
                $scope.model.haveAppliedInAeon = dataItem.usedToWorkInCompany;
                $scope.model.hasExperienceOnWorkingInShift = dataItem.hasExperienceOnWorkingInShift ? true : false;
                $scope.model.hasExperienceOnStandingAndWalking = dataItem.hasExperienceOnStandingAndMoving ? true : false;
                $scope.model.canWorkEarlyOrLateShift = dataItem.canWorkEarlyOrLateShift ? true : false;
                $scope.model.hasExperienceOnRetail = dataItem.hasExperienceOnRetail ? true : false;
                $scope.model.havePension = dataItem.isOnPension ? true : false;
                if ($scope.model.havePension) {
                    $scope.model.applicantRelativeInAeons = convertRelativeInAeons(dataItem.familyMembersInCompany);
                }
                getInterestsInWork1stPriorityByName(importantFactorValues, 1);
            } else {
                $scope.model.applicantAppliedPosition1 = dataItem.position1Name;
                $scope.model.applicantAppliedPosition2 = dataItem.position2Name;
                $scope.model.drivingsLicense = dataItem.hasDrivingLicense ? true : false;
                $scope.model.possessOwnVehicle = true;
                $scope.model.grossSalary1 = dataItem.grossSalary1;
                $scope.model.grossSalary2 = dataItem.grossSalary2;
                $scope.model.terminationNotice1 = dataItem.terminationNotice1;
                $scope.model.terminationNotice2 = dataItem.terminationNotice2;
                $scope.model.carNo = dataItem.carNo;
                $scope.model.hasDrivingLicense = dataItem.hasDrivingLicense;
                $scope.model.hasOwnVehicle = dataItem.hasOwnVehicle;
                $scope.model.hasCar = dataItem.hasCar;
                $scope.model.hasMotorbike = dataItem.hasMotorbike;
                $scope.model.motocycleNo = dataItem.motorcycleNo;
                $scope.model.computerSkills = dataItem.computerSkills;
                $scope.model.typingSpeed = dataItem.typingSpeed;
                $scope.model.otherSkills = dataItem.otherSkills; 1
                $scope.model.employmentHistories = dataItem.employmentHistories;
                $scope.model.languageProficiencyEntries = convertLanguageProficiencyEntries(dataItem.languageProficiencyEntries);
                $scope.model.characterReferences = convertLanguageCharacters(dataItem.characterReferees);

            }
            if (importantFactorValues.length > 0) {
                getInterestsInWork1stPriorityByName(importantFactorValues, $scope.applicantType);
            }
            $scope.model.appreciationId = (_.find($scope.appreciations, x => { return x.code === 'APP1' })).id;
            console.log($scope.masterDatas.educationList);
        }, 0)



    }

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });
    function searchCodeByName(list, name) {
        if (name) {
            let item = _.find(list, x => { return x.name.includes(name) || name.includes(x.name) });
            if (item) {
                return item.code;
            }
        }
        return '';
    }
    function searchNameByCode(list, code) {
        if (code) {
            let item = _.find(list, x => { return x.code.includes(code) || code.includes(x.code) });
            if (item) {
                return item.name;
            }
        }
        return '';
    }
    function convertEducations(educationLists) {
        let result = [];
        educationLists.forEach(x => {
            result.push({
                school: searchCodeByName($scope.masterDatas.schools, x.school),
                fromDate: new Date(x.fromDate),
                toDate: new Date(x.toDate),
                major: x.certificate
            })
        })
        return result;
    }
    function convertFamilyMembers(familyMembers) {
        let result = [];
        familyMembers.forEach(x => {
            result.push({
                name: x.name,
                relationShip: searchCodeByName($scope.masterDatas.relationships, x.relationship),
                occupation: x.occupation,
                placeOfOccupation: x.placeOfOccupation,
                contactNumber: x.certificate
            })
        })
        return result;
    }
    function convertLanguageProficiencyEntries(languageProficiencyEntries) {
        let result = [];
        languageProficiencyEntries.forEach(x => {
            result.push({
                language: x.language,
                spoken: x.spoken,
                writen: x.occupation,
                understand: x.understand,
                writen: x.writen
            })
        })
        return result;
    }
    function convertLanguageCharacters(characters) {
        let result = [];
        characters.forEach(x => {
            result.push({
                name: x.name,
                profession: x.profession,
                relationship: searchCodeByName($scope.masterDatas.relationships, x.relationship),
                telNo: x.telNo,
                yearsKnown: x.yearsKnown
            })
        })
        return result;
    }
    function convertRelativeInAeons(applicantRelativeInAeons) {
        let result = [];
        applicantRelativeInAeons.forEach(x => {
            result.push({
                name: x.name,
                positionCode: x.position,
                relationCode: searchCodeByName($scope.masterDatas.relationships, x.relationship),
                department: '',
                workingPlacesCode: ''
            })
        })
        return result;
    }

    // type để xác định là form của G1 hay là G2 up
    function getInterestsInWork1stPriorityByName(importantFactorValues, type) {
        let result = '';
        if (type == '1') { // có 5 dropdown
            for (let i = 0; i < 5; i++) {
                if (importantFactorValues[i]) {
                    if (i == 0) {
                        $scope.model.interestsInWork1stPriorityCode = searchCodeByName($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork1stPriorityName = importantFactorValues[i].trim();
                    } else if (i == 1) {
                        $scope.model.interestsInWork2ndPriorityCode = searchCodeByName($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork2ndPriorityName = importantFactorValues[i].trim();

                    } else if (i == 2) {
                        $scope.model.interestsInWork3rdPriorityCode = searchCodeByName($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork3rdPriorityName = importantFactorValues[i].trim();
                    }
                    else if (i == 3) {
                        $scope.model.interestsInWork4thPriorityCode = searchCodeByName($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork4thPriorityName = importantFactorValues[i].trim();
                    }
                    else if (i == 4) {
                        $scope.model.interestsInWork5thPriorityCode = searchCodeByName($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork5thPriorityName = importantFactorValues[i].trim();
                    }
                }
            }
        } else {  // có 10 dropdown
            for (let i = 0; i < 10; i++) {
                if (importantFactorValues[i]) {
                    if (i == 0) {
                        $scope.model.interestsInWork1stPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork1stPriorityCode = importantFactorValues[i].trim();
                    } else if (i == 1) {
                        $scope.model.interestsInWork2ndPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork2ndPriorityCode = importantFactorValues[i].trim();

                    } else if (i == 2) {
                        $scope.model.interestsInWork3rdPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork3rdPriorityCode = importantFactorValues[i].trim();
                    }
                    else if (i == 3) {
                        $scope.model.interestsInWork4thPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork4thPriorityCode = importantFactorValues[i].trim();
                    }
                    else if (i == 4) {
                        $scope.model.interestsInWork5thPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork5thPriorityCode = importantFactorValues[i].trim();
                    }
                    else if (i == 5) {
                        $scope.model.interestsInWork6thPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork6thPriorityCode = importantFactorValues[i].trim();
                    }
                    else if (i == 6) {
                        $scope.model.interestsInWork7thPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork7thPriorityCode = importantFactorValues[i].trim();
                    }
                    else if (i == 7) {
                        $scope.model.interestsInWork8thPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork8thPriorityCode = importantFactorValues[i].trim();
                    }
                    else if (i == 8) {
                        $scope.model.interestsInWork9thPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork9thPriorityCode = importantFactorValues[i].trim();
                    }
                    else if (i == 9) {
                        $scope.model.interestsInWork10thPriorityName = searchNameByCode($scope.masterDatas.interestInWorkPriorityItems, importantFactorValues[i].trim());
                        $scope.model.interestsInWork10thPriorityCode = importantFactorValues[i].trim();
                    }
                }
            }
        }
        return result;
    }

    $scope.dataMassLocations = [];
    async function getMassStores() {
        $scope.searchInfo.store = '';
        var res = await settingService.getInstance().recruitment.getMassLocations().$promise;
        if (res.isSuccess) {
            $scope.dataMassLocations = [];
            res.object.forEach(element => {
                let item = element;
                if (item.value) {
                    item.code = item.value;
                }
                $scope.dataMassLocations.push(item);
            });
            var dropdownlist = $("#storeId").data("kendoDropDownList");
            if (dropdownlist) {
                dropdownlist.setDataSource($scope.dataMassLocations);
            }
        }
    }

    $scope.dataMassPositions = [];
    async function getMassPositions() {
        let queryArgs = {
            predicate: 'isActivated=@0',
            predicateParameters: [true],
            order: "Created desc",
            limit: 1000,
            page: 1
        }
        var res = await recruitmentService.getInstance().position.getMassPositions(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataMassPositions = res.object.data.items;
            var dropdownlist = $("#positionId").data("kendoDropDownList");
            if (dropdownlist) {
                dropdownlist.setDataSource($scope.dataMassPositions);
            }
        }
    }
});