angular
    .module('ssg')
    .factory('settingService', function ($resource, $window, appSetting, $rootScope, interceptorService) {
        var _ = $window._;
        var instance = null;

        function createInstance() {
            instance = new createfactoryService();
            return instance;
        }

        function createfactoryService() {
            var url = baseUrlApi + "/:controller/:action/:id";
            var customHeaders = buildCustomHeader();
            return {
                users: $resource(url, null, {
                    createUser: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'createUser' },
                        headers: customHeaders
                    },
                    update: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'update' },
                        headers: customHeaders
                    },
                    changeStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'changeStatus' },
                        headers: customHeaders
                    },
                    changePassword: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'changePassword' },
                        headers: customHeaders
                    },
                    resetPassword: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'resetPassword' },
                        headers: customHeaders
                    },
                    getUsers: {
                        method: 'Post',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsers' },
                        headers: customHeaders
                    },
                    getUsersInAllDivision: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsersInAllDivision' },
                        headers: customHeaders
                    },
                    getChildUsers: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'getChildUsers' },
                        headers: customHeaders
                    },
                    getUserCheckedHeadCount: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUserCheckedHeadCount' },
                        headers: customHeaders
                    },
                    getUsersByOnlyDeptLine: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsersByOnlyDeptLine' },
                        headers: customHeaders
                    },
                    getUsersByDeptLines: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsersByDeptLines' },
                        headers: customHeaders
                    },
                    getUsersInCurrentDivision: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsersInCurrentDivision' },
                        headers: customHeaders
                    },
                    getUsersInCurrentDivisionAndChild: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsersInCurrentDivisionAndChild' },
                        headers: customHeaders
                    },
                    getUserById: {
                        method: 'Post',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUserById' },
                        headers: customHeaders
                    },
                    seachEmployee: {
                        method: 'Get',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Employee", action: 'GetEmployeeInfo' },
                        headers: customHeaders
                    },
                    getSubGroupEmployees: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Employee", action: 'GetMasterDataEmployeeList' },
                        headers: customHeaders
                    },
                    getNewWorkLocation: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Employee", action: 'GetNewWorkLocationList' },
                        headers: customHeaders
                    },
                    getCurrentUser: {
                        method: 'Get',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetCurrentUser' },
                        headers: customHeaders
                    },
                    updateImageUser: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'UpdateImageUser' },
                        headers: customHeaders
                    },
                    updateImageUser: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'UpdateImageUser' },
                        headers: customHeaders
                    },
                    getImageUserById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetImageUserById' },
                        headers: customHeaders
                    },
                    findUserForDataInvalid: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'FindUserForDataInvalid' },
                        headers: customHeaders
                    },
                    getUserProfileCustomById: {
                        method: 'Post',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUserProfileCustomById' },
                        headers: customHeaders
                    },
                    getUserProfileDataById: {
                        method: 'Post',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUserProfileDataById' },
                        headers: customHeaders
                    },
                    forgotPassword: {
                        method: 'Post',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'ForgotPassword' },
                        headers: customHeaders
                    },
                    checkUserIsStore: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'CheckUserIsStore' },
                        headers: customHeaders
                    },
                    getUsersForTargetPlanByDeptId: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsersForTargetPlanByDeptId' },
                        headers: customHeaders
                    },
                    getUsersByList: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsersByList' },
                        headers: customHeaders
                    },
                    getValidUsersForSubmitTargetPlan: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetValidUsersForSubmitTargetPlan' },
                        headers: customHeaders
                    },
                    checkUserBySAPCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'CheckUserBySAPCode' },
                        headers: customHeaders
                    }, 
                    getUsersForReportTargetPlan: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'GetUsersForReportTargetPlan' },
                        headers: customHeaders
                    },
                    lockUserMembership: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'lockUserMembership' },
                        headers: customHeaders
                    },
                    checkLockUser: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "User", action: 'checkLockUser' },
                        headers: customHeaders
                    },
                }),
                referenceNumbers: $resource(url, null, {
                    getReferenceNumber: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ReferenceNumber", action: 'GetReferencyNumberRecruiments' },
                        headers: customHeaders
                    },
                    searchReferenceNumberByName: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ReferenceNumber", action: 'SearchReferencyNumberRecruimentsByName' },
                        headers: customHeaders
                    },
                    editReferenceNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ReferenceNumber", action: 'UpdateReferencyNumberRecruitment' },
                        headers: customHeaders
                    },
                    createReference: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "ReferenceNumber", action: 'CreateReference' },
                        headers: customHeaders
                    }
                }),
                recruitment: $resource(url, null, {
                    getMassLocations:{
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Mass", action: 'GetMassLocations' },
                        headers: customHeaders
                    },
                    //categoriesCheckDepartmentExist
                    getCategories: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetRecruitmentCategories' },
                        headers: customHeaders
                    },
                    editCategory: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdateRecruitmentCategory' },
                        headers: customHeaders
                    },
                    addCategory: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreateRecruitmentCategory' },
                        headers: customHeaders
                    },
                    deleteCategory: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeleteRecruitmentCategory' },
                        headers: customHeaders
                    },
                    searchCategories: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'SearchRecruitmentCategories' },
                        headers: customHeaders
                    },
                    //working Time
                    getWorkingTime: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetWorkingTimeRecruiments' },
                        headers: customHeaders
                    },
                    editWorkingTime: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdateRecruitmentCategory' },
                        headers: customHeaders
                    },
                    addWorkingTime: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreateRecruitmentCategory' },
                        headers: customHeaders
                    },
                    deleteWorkingTime: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeleteRecruitmentCategory' },
                        headers: customHeaders
                    },
                    searchWorkingTime: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'SearchWorkingTimeRecruiment' },
                        headers: customHeaders
                    },
                    getWorkingTimeRecruimentByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetWorkingTimeRecruimentByCode' },
                        headers: customHeaders
                    },
                    //Item List
                    getItemList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetItemListRecruiments' },
                        headers: customHeaders
                    },
                    editItemList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdateItemListRecruitment' },
                        headers: customHeaders
                    },
                    addItemList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreateItemListRecruitment' },
                        headers: customHeaders
                    },
                    deleteItemList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeleteItemListRecruitment' },
                        headers: customHeaders
                    },
                    searchItemListRecruiment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'SearchItemListRecruimentByName' },
                        headers: customHeaders
                    },
                    getItemListByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetItemListByCode' },
                        headers: customHeaders
                    },
                    //Applicant Status
                    getApplicantStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetApplicantStatusRecruiments' },
                        headers: customHeaders
                    },
                    editApplicantStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdateApplicantStatusRecruitment' },
                        headers: customHeaders
                    },
                    addApplicantStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreateApplicantStatusRecruitment' },
                        headers: customHeaders
                    },
                    deleteApplicantStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeleteApplicantStatusRecruitment' },
                        headers: customHeaders
                    },
                    searchApplicantStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'SearchApplicantStatusRecruimentByName' },
                        headers: customHeaders
                    },
                    getAllApplicantStatus: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetAllApplicantStatusRecruiments' },
                        headers: customHeaders
                    },
                    getApplicantStatusRecruitmentByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetApplicantStatusRecruitmentByCode' },
                        headers: customHeaders
                    },
                    //Appreciation List
                    getAppreciationList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetAppreciationListRecruiments' },
                        headers: customHeaders
                    },
                    editAppreciationList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdateAppreciationListRecruitment' },
                        headers: customHeaders
                    },
                    addAppreciationList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreateAppreciationListRecruitment' },
                        headers: customHeaders
                    },
                    deleteAppreciationList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeleteAppreciationListRecruitment' },
                        headers: customHeaders
                    },
                    searchAppreciationList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'SearchAppreciationListRecruimentByName' },
                        headers: customHeaders
                    },
                    getAppreciationListRecruimentByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetAppreciationListRecruimentByCode' },
                        headers: customHeaders
                    },
                    //Position List
                    getPositionLists: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetPositionRecruiments' },
                        headers: customHeaders
                    },
                    editPositionList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdatePositionRecruitment' },
                        headers: customHeaders
                    },
                    addPositionList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreatePositionRecruitment' },
                        headers: customHeaders
                    },
                    deletePositionList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeletePositionRecruitment' },
                        headers: customHeaders
                    },
                    getPositionListRecruimentByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetPositionRecruimentByCode' },
                        headers: customHeaders
                    },
                    //Cost Center
                    getCostCenterRecruiments: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetCostCenterRecruiments' },
                        headers: customHeaders
                    },
                    getCostCenterRecruitmentByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetCostCenterRecruitmentByCode' },
                        headers: customHeaders
                    },
                    updateCostCenterRecruitment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdateCostCenterRecruitment' },
                        headers: customHeaders
                    },
                    createCostCenterRecruitment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreateCostCenterRecruitment' },
                        headers: customHeaders
                    },
                    deleteCostCenterRecruitment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeleteCostCenterRecruitment' },
                        headers: customHeaders
                    },
                    getCostCenterByDepartmentId: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetCostCenterByDepartmentId' },
                        headers: customHeaders
                    },
                    //Working Address
                    getWorkingAddressRecruiments: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetWorkingAddressRecruiments' },
                        headers: customHeaders
                    },
                    getWorkingAddressRecruimentByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetWorkingAddressRecruimentByCode' },
                        headers: customHeaders
                    },
                    updateWorkingAddressRecruiment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdateWorkingAddressRecruiment' },
                        headers: customHeaders
                    },
                    createWorkingAddressRecruiment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreateWorkingAddressRecruiment' },
                        headers: customHeaders
                    },
                    deleteWorkingAddressRecruiment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeleteWorkingAddressRecruiment' },
                        headers: customHeaders
                    },
                    //Promote and tranfer print
                    getPromoteAndTranferPrintValue: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetPromoteAndTranferPrintValue' },
                        headers: customHeaders
                    },
                    getPromoteAndTranferPrintByName: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'GetPromoteAndTranferPrintByName' },
                        headers: customHeaders
                    },
                    createPromoteAndTranferPrint: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'CreatePromoteAndTranferPrint' },
                        headers: customHeaders
                    },
                    deletePromoteAndTranferPrint: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'DeletePromoteAndTranferPrint' },
                        headers: customHeaders
                    },
                    updatePromoteAndTranferPrint: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RecruitmentSetting", action: 'UpdatePromoteAndTranferPrint' },
                        headers: customHeaders
                    }
                }),
                departments: $resource(url, null, {
                    getRegionList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetRegionList' },
                        headers: customHeaders
                    },
                    createDepartment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'CreateDepartment' },
                        headers: customHeaders
                    },
                    updateDepartment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'UpdateDepartment' },
                        headers: customHeaders
                    },
                    getDepartments: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartments' },
                        headers: customHeaders
                    },
                    getDepartmentByCode: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentByCode' },
                        headers: customHeaders
                    },
                    getDetailDepartment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDetailDepartment' },
                        headers: customHeaders
                    },
                    getDepartmentTree: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentTree' },
                        headers: customHeaders
                    },
                    getDepartmentTreeByGrade: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentTreeByGrade' },
                        headers: customHeaders
                    },
                    getDivisionTree: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetOnlyDivisionTree' },
                        headers: customHeaders
                    },
                    getDeptLineTree: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetOnlyDeptLineTree' },
                        headers: customHeaders
                    },
                    getAllChildTree: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetAllChildTree' },
                        headers: customHeaders
                    },
                    deleteDepartment: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'DeleteDepartment' },
                        headers: customHeaders
                    },
                    getUserList: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetEmployeeCodes' },
                        headers: customHeaders
                    },
                    addUserToDepartment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'AddUserToDepartment' },
                        headers: customHeaders
                    },
                    getUserInDepartment: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetUserInDepartment' },
                        headers: customHeaders
                    },
                    updateUserInDepartment: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'UpdateUserInDepartment' },
                        headers: customHeaders
                    },
                    removeUserInDepartment: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'RemoveUserFromDepartment' },
                        headers: customHeaders
                    },
                    moveHeadCountAdd: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'MoveHeadCountAdd' },
                        headers: customHeaders
                    },
                    moveHeadCountUpdate: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'MoveHeadCountUpdate' },
                        headers: customHeaders
                    },
                    getOnlyDivisionTree: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetOnlyDivisionTree' },
                        headers: customHeaders
                    },
                    getOnlyDeptLineTree: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetOnlyDeptLineTree' },
                        headers: customHeaders
                    },
                    getAllDeptLineByGrade: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetAllDeptLineByGrade' },
                        headers: customHeaders
                    },
                    getDepartmentByFilter: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentByFilter' },
                        headers: customHeaders
                    },
                    getDepartmentByReferenceNumber: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentByReferenceNumber' },
                        headers: customHeaders
                    },
                    getDepartmentById: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentById' },
                        headers: customHeaders
                    },
                    getDepartmentsByUserId: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentsByUserId' },
                        headers: customHeaders
                    },
                    getDepartmentUpToG4ByUserId: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentUpToG4ByUserId' },
                        headers: customHeaders
                    },
                    getDivisionByFilter: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDivisionByFilter' },
                        headers: customHeaders
                    },             
                    getDepartmentByArg: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetDepartmentByArg' },
                        headers: customHeaders
                    },
                    getAllDepartmentsByPositonName: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Department", action: 'GetAllDepartmentsByPositonName' },
                        headers: customHeaders
                    }
                }),
                cabs: $resource(url, null, {
                    addReason: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'AddReason' },
                        headers: customHeaders
                    },
                    updateReason: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'UpdateReason' },
                        headers: customHeaders
                    },
                    deleteReason: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'DeleteReason' },
                        headers: customHeaders
                    },
                    getReasons: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'GetReasons' },
                        headers: customHeaders
                    },
                    getAllReason: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'GetAllReason' },
                        headers: customHeaders
                    },
                    // searchReason: {
                    //     method: 'GET', 
                    //     interceptor: interceptorService.getInstance().interceptor,                           
                    //     params: { controller: "CABSetting", action: 'SearchReason' }, headers: customHeaders
                    // }
                    getHolidaySchedules: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'GetHolidaySchedules' },
                        headers: customHeaders
                    },
                    createHolidaySchedule: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'CreateHolidaySchedule' },
                        headers: customHeaders
                    },
                    updateHolidaySchedule: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'UpdateHolidaySchedule' },
                        headers: customHeaders
                    },
                    deleteHolidaySchedule: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'DeleteHolidaySchedule' },
                        headers: customHeaders
                    },
                    getYearHolidays: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'GetYearHolidays' },
                        headers: customHeaders
                    }
                }),
                headcount: $resource(url, null, {
                    createHeadCount: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HeadCountSetting", action: 'CreateHeadCount' },
                        headers: customHeaders
                    },
                    getHeadCountList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HeadCountSetting", action: 'GetHeadCountList' },
                        headers: customHeaders
                    },
                    getHeadCountByDepartmentId: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HeadCountSetting", action: 'GetHeadCountByDepartmentId' },
                        headers: customHeaders
                    },
                    getDepartments: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HeadCountSetting", action: 'GetDepartmentListForHeadCount' },
                        headers: customHeaders
                    },
                    getJobGrades: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HeadCountSetting", action: 'GetJobGradeList' },
                        headers: customHeaders
                    },
                    updateHeadCount: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HeadCountSetting", action: 'UpdateHeadCount' },
                        headers: customHeaders
                    },
                    deleteHeadCount: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HeadCountSetting", action: 'DeleteHeadCount' },
                        headers: customHeaders
                    }
                }),
                jobgrade: $resource(url, null, {
                    getJobGradeList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'GetJobGradeList' },
                        headers: customHeaders
                    },
                    updateJobGrade: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'UpdateJobGrade' },
                        headers: customHeaders
                    },
                    deleteJobGrade: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'DeleteJobGrade' },
                        headers: customHeaders
                    },
                    createJobGrade: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'CreateJobGrade' },
                        headers: customHeaders
                    },
                    getItemRecruitmentsOfJobGrade: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'GetItemRecruitmentsOfJobGrade' },
                        headers: customHeaders
                    },
                    addOrUpdateItemsOfJobGrade: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'AddOrUpdateItemsOfJobGrade' },
                        headers: customHeaders
                    },
                    getAllItemRecruitments: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'GetAllItemRecruitments' },
                        headers: customHeaders
                    },
                    getJobGradeById: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'GetJobGradeById' },
                        headers: customHeaders
                    },
                    getItemRecruitmentsByJobGradeId: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "JobGrade", action: 'GetItemRecruitmentsByJobGradeId' },
                        headers: customHeaders
                    },
                }),
                workflows: $resource(url, null, {
                    getWorkflowTemplates: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'GetWorkflowTemplates' },
                        headers: customHeaders
                    },
                    getWorkflowTemplateById: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'GetWorkflowTemplateById' },
                        headers: customHeaders
                    },
                    getAllItemTypes: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'GetAllItemTypes' },
                        headers: customHeaders
                    },
                    updateWorkflowTemplate: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'UpdateWorkflowTemplate' },
                        headers: customHeaders
                    },
                    createWorkflowTemplate: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'CreateWorkflowTemplate' },
                        headers: customHeaders
                    },
                }),
                masterData: $resource(url, null, {
                    getMasterData: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "MasterDataLocal", action: 'GetMasterDataApplicantList' },
                        headers: customHeaders
                    }
                }),
                trackingRequest: $resource(url, null, {
                    getTrackingRequest: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "TrackingRequestSetting", action: 'GetTrackingRequest' },
                        headers: customHeaders
                    }
                }),
                //shift plan submit person
                shiftPlanSubmitPerson: $resource(url, null, {
                    deleteShiftPlanSubmitPerson: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'DeleteShiftPlanSubmitPerson' },
                        headers: customHeaders
                    },
                    getShiftPlanSubmitPersonById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'GetShiftPlanSubmitPersonById' },
                        headers: customHeaders
                    },
                    getShiftPlanSubmitPersons: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'GetShiftPlanSubmitPersons' },
                        headers: customHeaders
                    },
                    createShiftPlanSubmitPerson: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'CreateShiftPlanSubmitPerson' },
                        headers: customHeaders
                    },
                    checkDepartmentExist: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'CheckDepartmentExist' },
                        headers: customHeaders
                    },
                    saveShiftPlanSubmitPerson: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'SaveShiftPlanSubmitPerson' },
                        headers: customHeaders
                    },
                    getDepartmentTargetPlansByUserId: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'GetDepartmentTargetPlansByUserId' },
                        headers: customHeaders
                    },
                    getFilterDivisionByUserNotSubmit: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'FilterDivisionByUserNotSubmit' },
                        headers: customHeaders
                    },
                    checkSubmitPersonFromDepartmentId: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "CABSetting", action: 'CheckSubmitPersonFromDepartmentId' },
                        headers: customHeaders
                    }
                }),
                airline: $resource(url, null, {
                    getAirlines: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Airline", action: 'GetAirlines' },
                        headers: customHeaders
                    },
                    getAirlineById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Airline", action: 'GetAirlineById' },
                        headers: customHeaders
                    },
                    saveAirline: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Airline", action: 'SaveAirline' },
                        headers: customHeaders
                    },
                    deleteAirline: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Airline", action: 'DeleteAirline' },
                        headers: customHeaders
                    },
                    checkValidateCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Airline", action: 'CheckValidateCode' },
                        headers: customHeaders
                    },
                    validateWhenFlightNumberUsed: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Airline", action: 'ValidateWhenFlightNumberUsed' },
                        headers: customHeaders
                    },
                }),
                flightNumber: $resource(url, null, {
                    getFlightNumbers: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "FlightNumber", action: 'GetFlightNumbers' },
                        headers: customHeaders
                    },
                    getFlightNumberById: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "FlightNumber", action: 'GetFlightNumberById' },
                        headers: customHeaders
                    },
                    saveFlightNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "FlightNumber", action: 'SaveFlightNumber' },
                        headers: customHeaders
                    },
                    deleteFlightNumber: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "FlightNumber", action: 'DeleteFlightNumber' },
                        headers: customHeaders
                    },
                    checkValidateFlightNumberCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "FlightNumber", action: 'CheckValidateFlightNumberCode' },
                        headers: customHeaders
                    },
                }),
                businessTripLocation: $resource(url, null, {
                    createBusinessTripLocation: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BusinessTripLocationSetting", action: 'CreateBusinessTripLocation' },
                        headers: customHeaders
                    },
                    getListBusinessTripLocation: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BusinessTripLocationSetting", action: 'GetListBusinessTripLocation' },
                        headers: customHeaders
                    },
                    updateBusinessTripLocation: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BusinessTripLocationSetting", action: 'UpdateBusinessTripLocation' },
                        headers: customHeaders
                    },
                    deleteBusinessTripLocation: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BusinessTripLocationSetting", action: 'DeleteBusinessTripLocation' },
                        headers: customHeaders
                    },
                    getBusinessTripLocationByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BusinessTripLocationSetting", action: 'GetBusinessTripLocationByCode' },
                        headers: customHeaders
                    },
                }),
                hotel: $resource(url, null, {
                    createHotel: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HotelSetting", action: 'CreateHotel' },
                        headers: customHeaders
                    },
                    getListHotels: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HotelSetting", action: 'GetListHotels' },
                        headers: customHeaders
                    },
                    updateHotel: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HotelSetting", action: 'UpdateHotel' },
                        headers: customHeaders
                    },
                    deleteHotel: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HotelSetting", action: 'DeleteHotel' },
                        headers: customHeaders
                    },
                    getHotelByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "HotelSetting", action: 'GetHotelByCode' },
                        headers: customHeaders
                    }
                }),
                btaPolicy: $resource(url, null, {
                   //BTAPolicy mapping API
                    getBTAPolicyList: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'GetBTAPolicyList' },
                        headers: customHeaders
                    },
                    getBTAPolicyByJobGradeId: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'GetBTAPolicyByJobGradeId' },
                        headers: customHeaders
                    },
                    createBTAPolicy: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'CreateBTAPolicy' },
                        headers: customHeaders
                    },
                    updateBTAPolicy: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'UpdateBTAPolicy' },
                        headers: customHeaders
                    },
                    getBTAPolicyByDepartment: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'GetBTAPolicyByDepartment' },
                        headers: customHeaders
                    },
                    //BTAPolicy Special mapping API
                    getListBTAPolicySpecialCases: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'GetListBTAPolicySpecialCases' },
                        headers: customHeaders
                    },
                    createBTAPolicySpecialCases: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'CreateBTAPolicySpecialCases' },
                        headers: customHeaders
                    },
                    updateBTAPolicySpecialCases: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'UpdateBTAPolicySpecialCases' },
                        headers: customHeaders
                    },
                    deleteBTAPolicySpecialCases: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'DeleteBTAPolicySpecialCases' },
                        headers: customHeaders
                    },
                    getBTAPolicySpecialCasesByUserSAPCode: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BTAPolicy", action: 'GetBTAPolicySpecialCasesByUserSAPCode' },
                        headers: customHeaders
                    },
                }),
                roomType: $resource(url, null, {
                    createRoomType: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RoomTypeSetting", action: 'CreateRoomType' },
                        headers: customHeaders
                    },
                    getListRoomTypes: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RoomTypeSetting", action: 'GetListRoomTypes' },
                        headers: customHeaders
                    },
                    updateRoomType: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RoomTypeSetting", action: 'UpdateRoomType' },
                        headers: customHeaders
                    },
                    deleteRoomType: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RoomTypeSetting", action: 'DeleteRoomType' },
                        headers: customHeaders
                    },
                    getRoomTypeByCode: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "RoomTypeSetting", action: 'GetRoomTypeByCode' },
                        headers: customHeaders
                    },
                }),
                salaryDayConfiguration: $resource(url, null, {
                    getSalaryDayConfigurations: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SalaryDayConfiguration", action: 'GetSalaryDayConfigurations' },
                        headers: customHeaders
                    },
                    saveSalaryDayConfiguration: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "SalaryDayConfiguration", action: 'SaveSalaryDayConfiguration' },
                        headers: customHeaders
                    }                    
                }),
                bookingContract: $resource(url, null, {
                    getBookingContract: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BookingContract", action: 'GetBookingContract' },
                        headers: customHeaders
                    },
                    updateBookingContract: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "BookingContract", action: 'UpdateBookingContract' },
                        headers: customHeaders
                    }
                }),
                timeConfiguration: $resource(url, null, {
                    getTimeConfigurations: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Time", action: 'GetTimeConfigurations' },
                        headers: customHeaders
                    },
                    updateConfiguration: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Time", action: 'UpdateConfiguration' },
                        headers: customHeaders
                    }
                }),
                maintenant: $resource(url, null, {
                    getItemsHasWrongStatus: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'GetItemsHasWrongStatus' },
                        headers: customHeaders
                    },
                    getItemsNotHavePayload: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'GetItemsNotHavePayload' },
                        headers: customHeaders
                    },
                    getItemsHadPending: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'GetItemsHadPending' },
                        headers: customHeaders
                    },
                    getUserLockedStatus: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'GetUserLockedStatus' },
                        headers: customHeaders
                    },
                    unlockedUser: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'UnlockedUser' },
                        headers: customHeaders
                    },
                    getUserSend_OT_Holiday_Failed: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'GetUserSend_OT_Holiday_Failed' },
                        headers: customHeaders
                    },
                    submitPayload: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'SubmitPayload' },
                        headers: customHeaders
                    },
                    updateStatus: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'UpdateStatus' },
                        headers: customHeaders
                    },
                    generateOTPayload: {
                        method: 'GET',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Maintenant", action: 'GenerateOTPayload' },
                        headers: customHeaders
                    },
                }),
            }
        }
        return {
            getInstance() {
                if (!instance) {
                    instance = new createInstance();
                }
                return instance;
            }
        }
    });