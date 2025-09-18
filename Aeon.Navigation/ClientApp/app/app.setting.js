angular.module("appModule").constant("appSetting", {
    excludeKeys: ["mobile", "idCard9Number", "idCard12Number", "email", "userSAPCode", "createdByFullName", "userFullName", "requestorFullName", "requestorUserName"],
    sortDateFormat: "DD/MM/YYYY",
    longDateFormat: "DD/MM/YYYY HH:mm",
    longDateFormatAMPM: "DD/MM/YYYY hh:mm A",
    pageSizeDefault: 20,
    pageSizesArray: [10, 20, 30, 40, 50],
    pageSizesArrayMax: [10, 20, 30, 40, 50, 100, 200],
    pageSizeWindow: 5,
    pageSizesWindowArray: [5, 10, 15],
    personalSapCode: ['00311018', '00313739'],
    ORDER_GRID_DEFAULT: 'created desc',
    parentMenus: ['home.recruitment', 'home.cb', 'home.setting', 'home.bta', 'home.facility', 'home.maintenant', 'home.navigation-home'],
    role: { SAdmin: 1, Admin: 2, HR: 4, CB: 8, Member: 16, HRAdmin: 32, Accounting: 64, ITHelpdesk: 128 },
    edoc1Url: '',
    edoc2Url: 'http://edoc.aeon.com.vn/HR/Default.aspx#!',
    targets: {
        Target1: 1,
        Target2: 2
    },
    workflowItemStatus: {
        Approved: "Approved",
        Inprogress: "In Progress"
    },
    salaryRangeDate: [26, 25],
    btaReportType: {
        HOTEL: 1,
        FLIGHTNUMBER: 2,
        STATUS: 3,
        FLIGHTSBOOKING: 4,
        OVERBUDGET: 5
    },
    notificationExport: {
        error: "No Data To Export",
        success: "Data Successfully Exported"
    },
    menu: [
        {
        index: 1,
        url: ".navigation-home",
        selected: false,
        name: "NAVIGATION",
        title: "NAVIGATION",
        ref: "home.navigation-home",
        isParentMenu: true,
        gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
        roles: [1, 2, 4, 8, 16, 32],
        type: [0, 1],
        childMenus: [{
            name: "COMMON_TO_DO_LIST",
            title: "To-Do List",
            selected: false,
            ref: [{ state: "home.todo", isItem: true }],
            url: "home.todo",
            index: 2,
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            isParentMenu: false
        }, 
        {
            name: 'Navigation Management',
            ref: [],
            selected: false,
            url: 'home.navigation-list',
            index: 2,
            isParentMenu: false,
            gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1],
            type: [0, 1],
            sapCode: ["00311018", "00313739"]
        }]
    }
    ],
    mappingStates: [
        { source: 'home', destination: 'home.navigation-home' }
    ],
    permission: [{
        code: 1,
        name: 'SAdmin'
    },
    {
        code: 2,
        name: 'Admin'
    },
    {
        code: 4,
        name: 'HR'
    },
    {
        code: 8,
        name: 'CB'
    },
    {
        code: 16,
        name: 'Member'
    },
    {
        code: 32,
        name: 'HR Admin'
    },
    {
        code: 64,
        name: 'Accounting'
    },
    {
        code: 128,
        name: 'Administrator'
    }
    ],
    departmentGroup: [{
        id: 1,
        title: "HOD"
    },
    {
        id: 2,
        title: "Checker"
    },
    {
        id: 4,
        title: "Member"
    },
    {
        id: 8,
        title: "Assistant"
    }],
    activeStates: [
        { state: 'home', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.dashboard', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.todo', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.navigation-home', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.navigation-list', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
    ]
});