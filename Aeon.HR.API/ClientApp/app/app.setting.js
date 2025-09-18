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
    parentMenus: ['home.dashboard', 'home.recruitment', 'home.cb', 'home.setting', 'home.bta', 'home.facility', 'home.maintenant'],
    role: { SAdmin: 1, Admin: 2, HR: 4, CB: 8, Member: 16, HRAdmin: 32, Accounting: 64, ITHelpdesk: 128 },
    numberSheets: 1,
    numberRowPerSheets: 10000,
    expiredTimeOut: 9000000,
    notifyTimeOut: 8400000,
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
            selected: true,
            url: ".recruitment",
            name: "COMMON_RECRUIT",
            title: "COMMON_RECRUIT",
            icon: "ClientApp/assets/images/icon/item_people.png",
            isParentMenu: true,
            gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1,2, 4, 8, 16, 32],
            type: [0, 1],
            childMenus: [
                {
                    name: "REQUEST_TO_HIRE_MENU",
                    selected: false,
                    url: [],
                    urlV2: "home.requestToHire.allRequests",
                    index: 1,
                    icon: "ClientApp/assets/images/icon/request-to-hire.jpg",
                    isParentMenu: false,
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0, 1],
         
                    ref: [{
                        state: "home.requestToHire.item",
                        isItem: true,
                        title: "REQUEST TO HIRE",
                        prefix: "REQ-"
                    },
                    { state: "home.requestToHire.myRequests", isItem: false },
                    { state: "home.requestToHire.allRequests", isItem: false },
                    { state: "home.requestToHire.trackingImport", isItem: false }
                    ]
                   
                },
                {
                    name: "POSITION_MENU",
                    selected: false,
                    url: [],
                    index: 2,
                    icon: "ClientApp/assets/images/icon/request-to-hire.jpg",
                    urlV2: "home.position.allRequests",
                    isParentMenu: false,
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 4, 32],
                    type: [0, 1],
                    // ref: [{ state: "home.position.allRequests", isItem: false }],
                    url: "home.position.allRequests",
                    ref: [
                        { state: "home.position.item", isItem: true, title: "POSITION" },
                        {
                            state: "home.position.detail",
                            isItem: true,
                            title: "POSITION",
                            prefix: "POSITION-"
                        },
                        //{ state: "home.position.myRequests", isItem: true },
                        { state: "home.position.allRequests", isItem: true }
                    ],
                },
                {
                    name: "APPLICANT_MENU",
                    selected: false,
                    url: [],
                    index: 3,
                    icon: "ClientApp/assets/images/icon/Application.jpg",
                    urlV2: "home.applicant.allRequests",
                    isParentMenu: false,
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 4, 32],
                    type: [0, 1],
                    ref: [{
                        state: "home.applicant.item",
                        isItem: true,
                        title: "APPLICANT",
                        prefix: "APP-"
                    },
                    { state: "home.applicant.myRequests", isItem: false },
                    { state: "home.applicant.allRequests", isItem: false }
                    ],
                    actions: [{
                        name: "My Applicants",
                        title: "My Applicants",
                        state: "home.applicant.myRequests",
                        roles: [1, 4, 32],
                        type: 1
                    },
                    {
                        name: "All Applicants",
                        title: "All Applicants",
                        state: "home.applicant.allRequests",
                        roles: [1, 4, 32],
                        type: 2
                    }
                    ]
                },
                {
                    name: "NEW_STAFF_MENU",
                    selected: false,
                    isParentMenu: false,
                    urlV2: "home.newStaffOnboard",
                    icon: "ClientApp/assets/images/icon/Application.jpg",
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 32],
                    type: [0, 1],
                    ref: [{ state: "home.newStaffOnboard", isItem: false }],
                    url: "home.newStaffOnboard",
                    index: 4
                },
                {
                    name: "PROMOTE_TRANSFER_MENU",
                    selected: false,
                    url: [],
                    index: 5,
                    urlV2: "home.promoteAndTransfer.allRequests",
                    icon: "ClientApp/assets/images/icon/promote.png",
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0, 1],
                    isParentMenu: false,
                    ref: [{
                        state: "home.promoteAndTransfer.item",
                        isItem: true,
                        title: "PROMOTION & TRANSFER RECOMMENDATION",
                        prefix: "PAT-"
                    },
                    {
                        state: "home.promoteAndTransfer.approve",
                        isItem: true,
                        title: "PROMOTION & TRANSFER RECOMMENDATION",
                        prefix: "PAT-"
                    },
                    { state: "home.promoteAndTransfer.myRequests", isItem: false },
                    { state: "home.promoteAndTransfer.allRequests", isItem: false }
                    ],
                    actions: [{
                        name: "My Requests",
                        title: "My Promote And Transfer Requests",
                        state: "home.promoteAndTransfer.myRequests",
                        roles: [1, 2, 4, 8, 16, 32],
                        type: 1
                    },
                    {
                        name: "All Requests",
                        title: "All Promote And Transfer Requests",
                        state: "home.promoteAndTransfer.allRequests",
                        roles: [1, 2, 4, 8, 16, 32],
                        type: 2
                    }
                    ]
                },
                {
                    name: "ACTING_MENU",
                    selected: false,
                    url: [],
                    index: 6,
                    urlV2: "home.action.allRequests",
                    icon: "ClientApp/assets/images/icon/Acting.jpg",
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0, 1],
                    isParentMenu: false,
                    ref: [{
                        state: "home.action.item",
                        isItem: true,
                        title: "ACTING",
                        prefix: "ACT-"
                    },
                    {
                        state: "home.action.itemApprove",
                        isItem: true,
                        title: "ACTING",
                        prefix: "ACT-"
                    },
                    {
                        state: "home.action.itemAppraise",
                        isItem: true,
                        title: "ACTING APPRAISING",
                    },
                    { state: "home.action.myRequests", isItem: true },
                    { state: "home.action.allRequests", isItem: true }
                    ],
                    actions: [{
                        name: "My Requests",
                        title: "My Acting Requests",
                        state: "home.action.myRequests",
                        gradeUsers: [4, 5, 6, 7, 8, 9],
                        roles: [1, 2, 4, 8, 16, 32],
                        type: [0, 1]
                    },
                    {
                        name: "All Requests",
                        title: "All Acting Requests",
                        state: "home.action.allRequests",
                        gradeUsers: [4, 5, 6, 7, 8, 9],
                        roles: [1, 2, 4, 8, 16, 32],
                        type: [0, 1]
                    }
                    ]
                }
            ]
        },
        {
            index: 2,
            url: ".cb",
            selected: false,
            name: "COMMON_CB",
            title: "COMMON_CB",
            icon: "ClientApp/assets/images/icon/persion-add.png",
            isParentMenu: true,
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            childMenus: [{
                name: "LEAVE_MANAGEMENT_MENU",
                selected: false,
                url: [],
                urlV2: "home.leavesManagement.allRequests",
                icon: "ClientApp/assets/images/icon/leave-management.jpg",
                index: 1,
                roles: [1, 2, 4, 8, 16, 32],
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                type: [0, 1],
                isParentMenu: false,
                isNotTargetPlan: true,
                ref: [{
                    state: "home.leavesManagement.item",
                    isItem: true,
                    title: "LEAVE MANAGEMENT",
                    prefix: "LM-"
                },
                { state: "home.leavesManagement.myRequests", isItem: false },
                { state: "home.leavesManagement.allRequests", isItem: false }
                ],
                actions: [{
                    name: "COMMON_MY_REQUEST",
                    title: "My Leave Requests",
                    state: "home.leavesManagement.myRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 1
                },
                {
                    name: "COMMON_ALL_REQUEST",
                    title: "All Leave Requests",
                    state: "home.leavesManagement.allRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 2
                }
                ]
            },
            {
                name: "MISSING_TIMECLOCK_MENU",
                selected: false,
                url: "",
                index: 2,
                urlV2: "home.missingTimelock.allRequests",
                isParentMenu: false,
                icon: "ClientApp/assets/images/icon/missing-Timelock.png",
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isNotTargetPlan: true,
                ref: [{
                    state: "home.missingTimelock.item",
                    isItem: true,
                    title: "MISSING TIMECLOCK",
                    prefix: "MT-"
                },
                {
                    state: "home.missingTimelock.approve",
                    isItem: true,
                    title: "MISSING TIMECLOCK",
                    prefix: "MT-"
                },
                { state: "home.missingTimelock.myRequests", isItem: false },
                { state: "home.missingTimelock.allRequests", isItem: false }
                ],
                actions: [{
                    name: "COMMON_MY_REQUEST",
                    title: "My Missing TimeClock Requests",
                    state: "home.missingTimelock.myRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 1
                },
                {
                    name: "COMMON_ALL_REQUEST",
                    title: "All Missing TimeClock Requests",
                    state: "home.missingTimelock.allRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 2
                }
                ]
            },
            {
                name: "OVERTIME_TIMECLOCK_MENU",
                selected: false,
                url: [],
                index: 3,
                urlV2: "home.overtimeApplication.allRequests",
                icon: "ClientApp/assets/images/icon/overTime.jpg",
                isParentMenu: false,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isNotTargetPlan: true,
                ref: [{
                    state: "home.overtimeApplication.item",
                    isItem: true,
                    title: "Overtime Application"
                },
                {
                    state: "home.overtimeApplication.view",
                    isItem: true,
                    title: "OA",
                    prefix: ""
                },
                { state: "home.overtimeApplication.myRequests", isItem: false },
                { state: "home.overtimeApplication.allRequests", isItem: false }
                ],
                actions: [{
                    name: "COMMON_MY_REQUEST",
                    title: "My Overtime Requests",
                    state: "home.overtimeApplication.myRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 1
                },
                {
                    name: "COMMON_ALL_REQUEST",
                    title: "All Overtime Requests",
                    state: "home.overtimeApplication.allRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 2
                }
                ]
            },
            {
                name: "SHIFT_PLAN_MENU",
                selected: false,
                url: "",
                subMenu: true,
                icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
                index: 4,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isHQ: true,
                isParentMenu: false,
                isNotTargetPlan: true,
                ref: [
                    { state: "home.pendingTargetPlan.item", isItem: true },
                    { state: 'home.targetPlan.myRequests', isItem: false },
                    { state: 'home.targetPlan.allRequests', isItem: false },
                    { state: 'home.targetPlan.shiftPlan', isItem: false },
                    { state: 'home.targetPlan.item', isItem: false },
                    { state: 'home.targetPlan.reports', isItem: false }
                ],
                actions: [
                    // {
                    //     name: "ADD_TARGET_PLAN",
                    //     title: "Add Target Plan",
                    //     state: "home.addTargetPlan.item",
                    //     type: 1
                    // },
                    {
                        name: "TARGET_PLAN",
                        title: "Target Plan",
                        urlV2: "home.targetPlan.allRequests",
                        children: [
                            { state: 'home.targetPlan.myRequests', name: 'COMMON_MY_REQUEST', isItem: false },
                            { state: 'home.targetPlan.allRequests', name: 'COMMON_ALL_REQUEST', isItem: false },
                        ],
                        roles: [1, 2, 4, 8, 16, 32],
                        type: 1
                    },
                    {
                        name: "SHIFT_PLAN_MENU",
                        title: "Shift Plan",
                        urlV2: "home.targetPlan.shiftPlan",
                        state: "home.targetPlan.shiftPlan",
                        roles: [1, 2, 4, 8, 16, 32],
                        children: [
                            { state: '', name: `DOWNLOAD_SHIFT_CODE`, url: 'ClientApp/assets/templates/Shift Codes.xlsx', item: false },
                        ],
                        type: 2
                    },
                    {
                        name: "REPORT_TARGET_PLAN",
                        title: "REPORT_TARGET_PLAN",
                        urlV2: "home.targetPlan.reports",
                        state: "home.targetPlan.reports",
                        gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                        roles: [1, 2, 4, 8, 16, 32],
                        type: [0, 1]
                    }]
            },
            {
                name: "SHIFT_EXCHANGE_MENU",
                selected: false,
                url: "",
                icon: "ClientApp/assets/images/icon/Shift-Exchange.jpg",
                index: 5,
                urlV2: "home.shiftExchange.allRequests",
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isParentMenu: false,
                isNotTargetPlan: true,
                ref: [{
                    state: "home.shiftExchange.item",
                    isItem: true,
                    title: "SHIFT EXCHANGE APPLICATION",
                    prefix: "SEA-"
                },
                {
                    state: "home.shiftExchange.itemView",
                    isItem: true,
                    title: "SHIFT EXCHANGE APPLICATION",
                    prefix: "SEA-"
                },
                { state: "home.shiftExchange.myRequests", isItem: false },
                { state: "home.shiftExchange.allRequests", isItem: false }
                ],
                actions: [{
                    name: "COMMON_MY_REQUEST",
                    title: "My Shift Exchange Requests",
                    state: "home.shiftExchange.myRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 1
                },
                {
                    name: "COMMON_ALL_REQUEST",
                    title: "All Shift Exchange Requests",
                    state: "home.shiftExchange.allRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 2
                }
                ]
            },
            {
                name: "RESIGNATION_APPLICATION_MENU",
                selected: false,
                url: "",
                index: 6,
                urlV2: "home.resignationApplication.allRequests",
                icon: "ClientApp/assets/images/icon/Resignation.jpg",
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isParentMenu: false,
                ref: [{
                    state: "home.resignationApplication.item",
                    isItem: true,
                    title: "RESIGNATION APPLICATION",
                    prefix: "PAT-"
                },
                {
                    state: "home.resignationApplication.approve",
                    isItem: true,
                    title: "RESIGNATION APPLICATION",
                    prefix: "PAT-"
                },
                { state: "home.resignationApplication.myRequests", isItem: false },
                { state: "home.resignationApplication.allRequests", isItem: false }
                ],
                actions: [{
                    name: "RESIGNATION_APPLICATION_MY_REQUETS",
                    title: "My Resignation Requests",
                    state: "home.resignationApplication.myRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 1
                },
                {
                    name: "RESIGNATION_APPLICATION_ALL_REQUETS",
                    title: "All Resignation Requests",
                    state: "home.resignationApplication.allRequests",
                    roles: [1, 2, 4, 8, 16, 32],
                    type: 2
                }
                ]
            }
            ]
        },
        {
            index: 3,
            url: ".admin",
            selected: false,
            name: "COMMON_ADMIN",
            icon: "ClientApp/assets/images/icon/icon_user.png",
            title: "COMMON_ADMIN",
            isParentMenu: true,
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            childMenus: [{
                name: "HANDOVER_MENU",
                selected: false,
                url: [],
                index: 1,
                urlV2: "home.handover.allHandover",
                icon: "ClientApp/assets/images/icon/handover.jpg",
                isParentMenu: false,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 32],
                type: [0, 1],
                isFacility: true,
                ref: [{
                    state: "home.handover.item",
                    isItem: true,
                    title: "HANDOVER",
                    prefix: "HAND-"
                },
                { state: "home.handover.myHandover", isItem: false },
                { state: "home.handover.allHandover", isItem: false }
                ],
                actions: [{
                    name: "My HandOvers",
                    title: "My Handovers",
                    state: "home.handover.myHandover",
                    roles: [1, 32],
                    type: 1,
                    isFacility: true
                },
                {
                    name: "All HandOvers",
                    title: "All Handovers",
                    state: "home.handover.allHandover",
                    roles: [1, 32],
                    type: 2,
                    isFacility: true
                }
                ]
            },
            {
                name: "BUSINESS_TRIP_APPLICATION",
                selected: false,
                url: [],
                index: 2,
                urlV2: "home.business-trip-application.allRequests",
                icon: "ClientApp/assets/images/icon/business_trip.png",
                roles: [1, 2, 4, 8, 16, 32, 64],
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                type: [0, 1],
                isParentMenu: false,
                ref: [{
                    state: "home.business-trip-application.item",
                    isItem: true,
                    title: "BUSINESS TRIP APPLICATION",
                    prefix: "BTA-"
                },
                { state: "home.business-trip-application.myRequests", isItem: false },
                { state: "home.business-trip-application.allRequests", isItem: false }
                ],
                actions: [{
                    name: "COMMON_MY_REQUEST",
                    title: "BTA_MY_REQUESTS",
                    state: "home.business-trip-application.myRequests",
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: 1
                },
                {
                    name: "COMMON_ALL_REQUEST",
                    title: "BTA_ALL_REQUESTS",
                    state: "home.business-trip-application.allRequests",
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: 2
                }
                ]
            },
            {
                name: "BTA_REPORT",
                selected: false,
                url: [],
                urlV2: "home.business-trip-application-report",
                icon: "ClientApp/assets/images/icon/business_trip_application.jpg",
                index: 3,
                isParentMenu: false,
                roles: [1, 2, 64],
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                isAdmin: true,
                isAccounting: true,
                type: [0, 1],
                url: "home.business-trip-application-report",
                ref: [
                    { state: "home.business-trip-application-report", isItem: false, title: "Business Trip Application Report" }

                ]
            },
            {
                name: "BTA_OVER_BUDGET",
                selected: false,
                url: [],
                index: 4,
                urlV2: "home.over-budget.allRequests",
                icon: "ClientApp/assets/images/icon/over_budget.png",
                roles: [1, 2, 4, 8, 16, 32, 64],
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                type: [0, 1],
                isParentMenu: false,
                ref: [{
                    state: "home.over-budget.item",
                    isItem: true,
                    title: "BUSINESS TRIP OVER BUDGET",
                    prefix: "BTA-"
                },
                { state: "home.over-budget.myRequests", isItem: false },
                { state: "home.over-budget.allRequests", isItem: false }
                ],
                actions: [{
                    name: "COMMON_MY_REQUEST",
                    title: "BTA_OVER_MY_REQUESTS",
                    state: "home.over-budget.myRequests",
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: 1
                },
                {
                    name: "COMMON_ALL_REQUEST",
                    title: "BTA_OVER_ALL_REQUESTS",
                    state: "home.over-budget.allRequests",
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: 2
                }
                ]

            }]
        },
        {
            index: 4,
            name: "User",
            title: "User",
            selected: false,
            icon: "ClientApp/assets/images/icon/admin.png",
            isParentMenu: false,
            ref: [{ state: "home.user-setting.user-profile", isItem: true },{ state: "home.user-setting.user-list", isItem: true }],
            url: "home.user-setting.user-list",
            index: 4,
            urlV2: "home.user-setting.user-list",
            gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 32],
            type: [0, 1],
        },
        {
            index: 5,
            name: "COMMON_DEPARTMENT",
            title: "COMMON_DEPARTMENT",
            icon: "ClientApp/assets/images/icon/icon_depart.png",
            selected: false,
            ref: [{ state: "home.departments", isItem: false }],
            url: "home.departments",
            index: 5,
            urlV2: "home.departments",
            isParentMenu: false,
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 32],
            type: [0, 1],
        },
        {
            // name: "Budget",
            index: 6,
            name: "HEADCOUNT_MENU", // ticket 145
            title: "HEADCOUNT_MENU", // ticket 145
            icon: "ClientApp/assets/images/icon/icon_headcount.png",
            selected: false,
            isParentMenu: false,
            ref: [{ state: "home.budgets", isItem: false }],
            url: "home.budgets",
            urlV2: "home.budgets",
            gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 32],
            type: [0, 1],
        },
        {
            index: 8,
            url: ".academy",
            selected: false,
            name: "Academy",
            title: "Academy",
            icon: "ClientApp/assets/images/icon/icon_aca.png",
            isParentMenu: true,
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [0],
            type: [0, 1],
            childMenus: [
                {
                    name: "TRAINING_REQUEST",
                    title: "TRAINING_REQUEST",
                    icon: "ClientApp/assets/images/icon/icon_item.png",
                    selected: false,
                    url: [],
                    index: "aca1",
                    urlV2: "home.academy-allRequests",
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0, 1],
                    isParentMenu: false,
                    ref: [
                        { state: "home.academy-myRequests", isItem: false },
                        { state: "home.academy-allRequests", isItem: false },
                        { state: "home.academy-newRequest", isItem: false },
                        { state: "home.academy-editRequest", isItem: false },
                    ],
                    actions: [
                        {
                            name: "TRAINING_REQUEST_COMMON_MY_REQUEST",
                            title: "My Academy Requests",
                            state: "home.academy-myRequests",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 1,
                        },
                        {
                            name: "TRAINING_REQUEST_COMMON_ALL_REQUEST",
                            title: "All Academy Requests",
                            state: "home.academy-myRequests",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 2,
                        },
                    ],
                },
                {
                    name: "TRAINING_INVITATION",
                    title: "TRAINING_INVITATION",
                    icon: "ClientApp/assets/images/icon/icon_item.png",
                    selected: false,
                    urlV2: "home.academy-allInvitation",
                    url: [],
                    index: "aca2",
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0, 1],
                    isParentMenu: false,
                    ref: [
                        {
                            state: "home.academy",
                            isItem: true,
                            title: "TRAINING_REQUEST",
                        },
                        { state: "home.academy-myInvitation", isItem: false },
                        { state: "home.academy-allInvitation", isItem: false },
                        { state: "home.academy-manageInvitatio", isItem: false },
                    ],
                    actions: [
                        {
                            name: "TRAINING_INVITATION_COMMON_MY_REQUEST",
                            title: "My Invitation",
                            state: "home.academy-myInvitation",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 1,
                        },
                        {
                            name: "TRAINING_INVITATION_COMMON_ALL_REQUEST",
                            title: "All Invitation",
                            state: "home.academy-allInvitation",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 2,
                        },
                        {
                            name: "MANAGE_INVITATION",
                            title: "Manage Invitation",
                            state: "home.academy-manageInvitatio.",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 2,
                        },
                    ],
                },
                {
                    name: "TRAINING_REPORT",
                    title: "TRAINING_REPORT",
                    icon: "ClientApp/assets/images/icon/icon_item.png",
                    selected: false,
                    urlV2: "home.academy-allReport",
                    url: [],
                    index: "aca3",
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0, 1],
                    isParentMenu: false,
                    ref: [
                        {
                            state: "home.academy",
                            isItem: true,
                            title: "Training Request",
                        },
                        { state: "home.academy-allReport", isItem: false },
                        { state: "home.academy-myReport", isItem: false },
                    ],
                    actions: [
                        {
                            name: "MY_TRAINING_REPORT",
                            title: "My Report",
                            state: "home.academy-myReport",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 1,
                        },
                        {
                            name: "ALL_TRAINING_REPORT",
                            title: "All Training Report",
                            state: "home.academy-allReport",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 2,
                        },
                    ],
                },
                {
                    name: "Report",
                    title: "Report",
                    selected: false,
                    icon: "ClientApp/assets/images/icon/icon_item.png",
                    url: [],
                    index: "aca4",
                    subMenu:true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0, 1],
                    isParentMenu: false,
                    ref: [
                        {
                            state: "home.academy",
                            isItem: true,
                            title: "ACADEMY_REPORT",
                        },
                        { state: "home.academy-trainingTrackerReport", isItem: false },
                    ],
                    actions: [
                        {
                            name: "TRAINING_TRACKER_REPORT",
                            title: "Training Tracker Report",
                            urlV2: "home.academy-trainingTrackerReport",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 1,
                        },
                        {
                            name: "TRAINING_SURVEY_REPORT",
                            title: "Training Survey Report",
                            urlV2: "home.academy-trainingSurveyReport",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 1,
                        },
                        {
                            name: "TRAINING_BUDGET_BALANCE_REPORT",
                            title: "Training Budget Balance Report",
                            urlV2: "home.training-budget-balance-report",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 1,
                        },
                        {
                            name: "TRAINER_CONTRIBUTION_REPORT",
                            title: "Trainer Contribution Report",
                            urlV2: "home.academy-trainerContributionReport",
                            roles: [1, 2, 4, 8, 16, 32],
                            type: 1,
                        },
                    ],
                },
               
            ],
        }
          ,{
            index: 7,
            url: ".setting",
            selected: false,
            name: "COMMON_SETTING",
            title: "COMMON_SETTING",
            isParentMenu: true,
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32, 64, 128],
            type: [0, 1],
            childMenus: [
                {
                    name: "COMMON_RECRUIT",
                    selected: false,
                    url: '',
                    icon: "ClientApp/assets/images/icon/item_people.png",
                    index: 11,
                    isParentMenu: false,
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 32],
                    type: [0, 1],
                    ref: [
                        { state: "home.settingRecruitmentAppreciationList", isItem: false },
                        { state: "home.settingRecruitmentApplicantStatus", isItem: false },
                        { state: "home.settingRecruitmentCostCenter", isItem: false },
                        { state: "home.settingRecruitmenItemList", isItem: false },
                        { state: "home.settingRecruitmentPosition", isItem: false },
                        { state: "home.settingRecruitmentWorkingAddress", isItem: false },
                        { state: "home.settingRecruitmentWorkingTime", isItem: false },
                        { state: "home.settingRecruitmentCategories", isItem: false }
                    ],
                    actions: [
                        {
                            name: "Applicant Status List",
                            title: "Applicant Status List",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.settingRecruitmentApplicantStatus",
                            state: "home.settingRecruitmentApplicantStatus",
                            roles: [1, 32],
                            type: 1
                        },
                        {
                            name: "Appreciation List",
                            title: "Appreciation List",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.settingRecruitmentAppreciationList",
                            state: "home.settingRecruitmentAppreciationList",
                            roles: [1, 32],
                            type: 2
                        },
                        {
                            name: "Cost Center",
                            title: "Cost Center",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.settingRecruitmentCostCenter",
                            state: "home.settingRecruitmentCostCenter",
                            roles: [1, 32],
                            type: 3
                        },
                        {
                            name: "Item List",
                            title: "Item List",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.settingRecruitmenItemList",
                            state: "home.settingRecruitmenItemList",
                            roles: [1, 32],
                            type: 4
                        },
                        {
                            name: "Position List",
                            title: "Position List",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.settingRecruitmentPosition",
                            state: "home.settingRecruitmentPosition",
                            roles: [1, 32],
                            type: 5
                        },
                        {
                            name: "Working Address",
                            title: "Working Address",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.settingRecruitmentWorkingAddress",
                            state: "home.settingRecruitmentWorkingAddress",
                            roles: [1, 32],
                            type: 6
                        },
                        {
                            name: "Working Time",
                            title: "Working Time",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.settingRecruitmentWorkingTime",
                            state: "home.settingRecruitmentWorkingTime",
                            roles: [1, 32],
                            type: 7
                        },
                        {
                            name: "Categories",
                            title: "Categories",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.settingRecruitmentCategories",
                            state: "home.settingRecruitmentCategories",
                            roles: [1, 32],
                            type: 8
                        }
                    ]
                },
                {
                    name: 'COMMON_CB',

                    icon: "ClientApp/assets/images/icon/persion-add.png",
                    ref: [
                        { state: 'home.missingTimeclockReasons', isItem: false },
                        { state: 'home.overtimeReasons', isItem: false },
                        { state: 'home.resignationReasons', isItem: false },
                        { state: 'home.shiftExchangeReasons', isItem: false },
                        { state: 'home.holidaySchedule', isItem: false },
                        { state: 'home.shiftPlanSubmitPerson', isItem: false },
                        { state: 'home.shiftCode', isItem: false },
                        { state: 'home.targetPlanSpecial', isItem: false },
                    ],
                    selected: false,
                    subMenu: true,
                    url: '',
                    index: 22,
                    isParentMenu: false,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 32],
                    type: [0, 1],
                    actions: [{
                        name: 'Missing Timeclock Reasons',
                        title: 'Missing Timeclock Reasons',
                        icon: "ClientApp/assets/images/icon/icon_item.png",
                        urlV2: "home.missingTimeclockReasons",
                        state: 'home.missingTimeclockReasons',
                        roles: [1, 32],
                        isNotTargetPlan: true,
                        type: 1
                    }, {
                        name: 'Overtime Reasons',
                        title: 'Overtime Reasons',
                        icon: "ClientApp/assets/images/icon/icon_item.png",
                        urlV2: "home.overtimeReasons",
                        state: 'home.overtimeReasons',
                        roles: [1, 32],
                        isNotTargetPlan: true,
                        type: 2
                    }, {
                        name: 'Shift Exchange Reasons',
                        title: 'Shift Exchange Reasons',
                        icon: "ClientApp/assets/images/icon/icon_item.png",
                        urlV2: "home.shiftExchangeReasons",
                        state: 'home.shiftExchangeReasons',
                        roles: [1, 32],
                        isNotTargetPlan: true,
                        type: 3
                    }, {
                        name: 'Resignation Reasons',
                        title: 'Resignation Reasons',
                        icon: "ClientApp/assets/images/icon/icon_item.png",
                        urlV2: "home.resignationReasons",
                        state: 'home.resignationReasons',
                        roles: [1, 32],
                        isNotTargetPlan: true,
                        type: 4
                    },
                    // {
                    //     name: 'Shift Plan Submit Persons',
                    //     title: 'Shift Plan Submit Persons',
                    //     state: 'home.shiftPlanSubmitPerson',
                    //     roles: [1, 32],
                    //     type: 5
                    // }, {
                    //     name: 'Holiday Schedule',
                    //     title: 'Holiday Schedule',
                    //     state: 'home.holidaySchedule',
                    //     roles: [1, 32],
                    //     type: 6
                    // }
                    {
                        name: 'Shift Plan',
                        title: 'Shift Plan',
                        icon: "ClientApp/assets/images/icon/icon_item.png",
                        index: "sf",
                        children: [
                            {
                                name: 'Shift Plan Submit Persons',
                                title: 'Shift Plan Submit Persons',
                                urlV2: "home.shiftPlanSubmitPerson",
                                state: 'home.shiftPlanSubmitPerson',
                                type: 5
                            },
                            {
                                name: 'Holiday Schedule',
                                title: 'Holiday Schedule',
                                urlV2: "home.holidaySchedule",
                                state: 'home.holidaySchedule',
                                type: 6
                            },
                            {
                                name: 'Shift Code',
                                title: 'Shift Code',
                                urlV2: "home.shiftCode",
                                state: 'home.shiftCode',
                                type: 7
                            },
                            {
                                name: 'Target Plan Special',
                                title: 'Target Plan Special',
                                urlV2: "home.targetPlanSpecial",
                                state: 'home.targetPlanSpecial',
                                type: 8
                            }

                        ],
                        roles: [1, 32],
                    }]
                },
                {
                    name: 'More_MENU',
                    icon: "ClientApp/assets/images/icon/icon_more.png",
                    subMenu: true,
                    ref: [
                        { state: 'home.workflowszzyyxx', isItem: false },
                        { state: 'home.workflowszzyyxx.viewlog', isItem: false },
                        { state: 'home.referenceNumbers', isItem: false },
                        { state: 'home.jobGrades', isItem: false },
                        { state: 'home.trackingLogs', isItem: false },
                        { state: 'home.trackingSyncOrgcharts', isItem: false },
                        { state: 'home.airline', isItem: false },
                        { state: 'home.partition', isItem: false },
                        //{ state: 'home.globalLocation', isItem: false },
                        { state: 'home.hotel', isItem: false },
                        { state: 'home.flightNumber', isItem: false },
                        { state: 'home.location', isItem: false },
                        { state: 'home.roomType', isItem: false },
                        { state: 'home.btaPolicy', isItem: false },
                        { state: 'home.btaPolicySpecial', isItem: false },
                        { state: 'home.reason', isItem: false },
                        { state: 'home.workflows', isItem: false },
                        //{ state: 'home.user-setting.user-list', isItem: true },
                        { state: 'home.workflows.item', isItem: false },
                        { state: 'home.workflows.myRequests', isItem: false },
                        { state: 'home.workflowszzyyxx.item', isItem: false },
                        { state: 'home.workflowszzyyxx.myRequests', isItem: false },
                        { state: 'home.daysConfiguration', isItem: false },
                        { state: 'home.businessModel', isItem: false },
                        { state: 'home.businessModelUnitMapping', isItem: false },
                    ],
                    selected: false,
                    url: '',
                    index: 33,
                    isParentMenu: false,
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 8, 32],
                    type: [0, 1],
                    actions: [
                        {
                            name: 'Workflow',
                            title: 'Workflow',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.workflowszzyyxx",
                            ref: [{ state: 'home.workflowszzyyxx', isItem: false },
                                { state: 'home.workflowszzyyxx.viewlog', isItem: false },
                                { state: 'home.workflows', isItem: false },
                                //{ state: 'home.user-setting.user-list', isItem: true },
                                { state: 'home.workflows.item', isItem: false },
                                { state: 'home.workflows.myRequests', isItem: false },
                                { state: 'home.workflowszzyyxx.item', isItem: false },
                                { state: 'home.workflowszzyyxx.myRequests', isItem: false },
                            ],
                            roles: [1, 32],
                            type: 1
                        },
                        {
                            name: 'Reference Number',
                            title: 'Reference Number',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.referenceNumbers",
                            state: 'home.referenceNumbers',
                            roles: [1, 32],
                            type: 1
                        },
                        {
                            name: 'Job Grade',
                            title: 'Job Grade',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            state: 'home.jobGrades',
                            urlV2: "home.jobGrades",
                            roles: [1, 32],
                            type: 1
                        },
                        {
                            name: 'Tracking Log',
                            title: 'Tracking Log',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.trackingLogs",
                            state: 'home.trackingLogs',
                            roles: [1, 8, 32],
                            type: 1
                        },
                        {
                            name: 'Tracking Sync Orgchart',
                            title: 'Tracking Sync Orgchart',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.trackingSyncOrgcharts",
                            state: 'home.trackingSyncOrgcharts',
                            roles: [1, 8, 32],
                            type: 1
                        },
                        {
                            name: 'Business Trip Application',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            title: 'BTA',
                            children: [
                                { state: 'home.airline', urlV2: "home.airline", name: 'Airline', isItem: true },
                                { state: 'home.partition', urlV2: "home.partition", name: 'Partition', isItem: false },
                                //{ state: 'home.globalLocation', name: 'Global Location', isItem: false },
                                { state: 'home.hotel', urlV2: "home.hotel", name: 'Hotel', isItem: false },
                                { state: 'home.flightNumber', urlV2: "home.flightNumber", name: 'Flight Number', isItem: false },
                                { state: 'home.location', urlV2: "home.location", name: 'Business Trip Location', isItem: false },
                                { state: 'home.roomType', urlV2: "home.roomType", name: 'Room Type', isItem: false },
                                { state: 'home.btaPolicy', urlV2: "home.btaPolicy", name: 'Budget Limit: HQ & Store', isItem: false },
                                { state: 'home.btaPolicySpecial', urlV2: "home.btaPolicySpecial", name: 'Budget Limit: Special Case', isItem: false },
                                { state: 'home.reason', urlV2: "home.reason", name: 'Reason', isItem: true },
                                { state: 'home.bta-error-message', urlV2: "home.bta-error-message", name: 'BTA Error Message', isItem: true },
                            ],
                            roles: [1, 2],
                            type: 1
                        },
                        {
                            name: 'Days Configuration',
                            title: 'Days Configuration',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.daysConfiguration",
                            state: 'home.daysConfiguration',
                            roles: [1, 32],
                            type: 1
                        },
                        {
                            name: 'Business Model',
                            title: 'Business Model',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.businessModel",
                            state: 'home.businessModel',
                            roles: [1, 128],
                            type: 1
                        },
                        {
                            name: 'Business Model Unit Mapping',
                            title: 'Business Model Unit Mapping',
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.businessModelUnitMapping",
                            state: 'home.businessModelUnitMapping',
                            roles: [1, 32, 128],
                            type: 1
                        }
                    ]
                },
                {
                    name: "Academy",
                    title: "Academy",
                    selected: false,
                    url: '',
                    icon: "ClientApp/assets/images/icon/icon_aca.png",
                    index: 44,
                    isParentMenu: false,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0, 1],
                    actions: [
                        {
                            name: "Reason of Training Request",
                            title: "Reason of Training Request",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.academy-trainingReason",
                            state: "home.academy-trainingReason",
                            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                            roles: [1, 2, 4, 8, 16, 32],
                            type: [0, 1],
                        },
                        {
                            name: "Course management",
                            title: "Course management",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.academy-categoryManagement",
                            state: "home.academy-categoryManagement",
                            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                            roles: [1, 2, 4, 8, 16, 32],
                            type: [0, 1],
                        },
                        {
                            name: "Training Request Management",
                            title: "Training Request Management",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.academy-trainingRequestManagement",
                            state: "home.academy-trainingRequestManagement",
                            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                            roles: [1, 2, 4, 8, 16, 32],
                            type: [0, 1],
                        },
                        {
                            name: "Training Invitation Management",
                            title: "Training Invitation Management",
                            icon: "ClientApp/assets/images/icon/icon_item.png",
                            urlV2: "home.academy-manageInvitation",
                            state: "home.academy-manageInvitation",
                            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                            roles: [1, 2, 4, 8, 16, 32],
                            type: [0, 1],
                        }
                    ]
                },
            ]
        }
        
    ],




















    //old
 //   menu: [
        
 //       {
 //       index: 1,
 //       url: ".dashboard",
 //       selected: false,
 //       name: "COMMON_DASHBOARD",
 //       title: "COMMON_DASHBOARD",
 //       ref: "home.dashboard",
 //       isParentMenu: true,
 //       gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1],
 //       childMenus: [{
 //           name: "COMMON_TO_DO_LIST",
 //           title: "To-Do List",
 //           selected: false,
 //           ref: [{ state: "home.todo", isItem: true }],
 //           url: "home.todo",
 //           index: 2,
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           roles: [1, 2, 4, 8, 16, 32],
 //           type: [0, 1],
 //           isParentMenu: false
 //       }, {
 //           name: "COMMON_ORGANIZATION_CHART",
 //           title: "Organization Chart",
 //           selected: false,
 //           ref: [{ state: "home.orgchart", isItem: true }],
 //           url: "home.orgchart",
 //           index: 2,
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           roles: [1, 2, 4, 8, 16, 32],
 //           type: [0, 1],
 //           isParentMenu: false
 //       }]
 //   },
 //   {
 //       name: "COMMON_PURCHASING",
 //       title: "COMMON_PURCHASING",
 //       url: edoc1Url + '/default.aspx#/all-purchases',
 //       isPhrase1: true,
 //       gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1]
 //   },
 //   {
 //       name: "COMMON_CONTRACT",
 //       title: "COMMON_CONTRACT",
 //       url: edoc1Url + '/default.aspx#/all-contractrequests',
 //       isPhrase1: true,
 //       gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1]
 //   },
 //   {
 //       name: "COMMON_TRADE_CONTRACT",
 //       title: "COMMON_TRADE_CONTRACT",
 //       url: 'http://edoc_l_trade.aeon.com.vn/',
 //       isPhrase1: true,
 //       gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1]
 //   },
 //   {
 //       name: "COMMON_BUDGET",
 //       title: "COMMON_BUDGET",
 //       url: edoc1Url + '/default.aspx#/budget-limit',
 //       isPhrase1: true,
 //       gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1]
 //   },
 //   {
 //       name: "COMMON_REPORT",
 //       title: "COMMON_REPORT",
 //       url: edoc1Url + '/default.aspx#/expenditures-report',
 //       isPhrase1: true,
 //       gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1]
 //   },
 //   {
 //       index: 2,
 //       selected: true,
 //       url: ".recruitment",
 //       name: "COMMON_RECRUIT",
 //       title: "COMMON_RECRUIT",
 //       ref: "home.recruitment",
 //       isParentMenu: true,
 //       gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1],
 //       childMenus: [
 //           {
 //               name: "REQUEST_TO_HIRE_MENU",
 //               selected: false,
 //               url: [],
 //               index: 1,
 //               isParentMenu: false,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: [0, 1],
 //               ref: [{
 //                   state: "home.requestToHire.item",
 //                   isItem: true,
 //                   title: "REQUEST TO HIRE",
 //                   prefix: "REQ-"
 //               },
 //               { state: "home.requestToHire.myRequests", isItem: false },
 //               { state: "home.requestToHire.allRequests", isItem: false },
 //               { state: "home.requestToHire.trackingImport", isItem: false }
 //               ],
 //               actions: [{
 //                   name: "My Requests",
 //                   title: "My Requests To Hire",
 //                   state: "home.requestToHire.myRequests",
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   type: 1
 //               },
 //               {
 //                   name: "All Requests",
 //                   title: "All Requests To Hire",
 //                   state: "home.requestToHire.allRequests",
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   type: 2
 //               },
 //               {
 //                   name: "Tracking Import",
 //                   title: "Tracking Import",
 //                   state: "home.requestToHire.trackingImport",
 //                   roles: [1, 32, 128],
 //                   type: 2
 //               }
 //               ]
 //           },
 //           {
 //               name: "POSITION_MENU",
 //               selected: false,
 //               url: [],
 //               index: 2,
 //               isParentMenu: false,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 4, 32],
 //               type: [0, 1],
 //               // ref: [{ state: "home.position.allRequests", isItem: false }],
 //               url: "home.position.allRequests",
 //               ref: [
 //                   { state: "home.position.item", isItem: true, title: "POSITION" },
 //                   {
 //                       state: "home.position.detail",
 //                       isItem: true,
 //                       title: "POSITION",
 //                       prefix: "POSITION-"
 //                   },
 //                   //{ state: "home.position.myRequests", isItem: true },
 //                   { state: "home.position.allRequests", isItem: true }
 //               ],
 //               // actions: [
 //               //     // {
 //               //     //    name: "My Positions",
 //               //     //    title: "My Positions",
 //               //     //    state: "home.position.myRequests",
 //               //     //    type: 1
 //               //     // },
 //               //    {
 //               //        name: "All Positions",
 //               //        title: "All Positions",
 //               //        state: "home.position.allRequests",
 //               //        type: 2
 //               //    }
 //               // ]
 //           },
 //           {
 //               name: "APPLICANT_MENU",
 //               selected: false,
 //               url: [],
 //               index: 3,
 //               isParentMenu: false,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 4, 32],
 //               type: [0, 1],
 //               ref: [{
 //                   state: "home.applicant.item",
 //                   isItem: true,
 //                   title: "APPLICANT",
 //                   prefix: "APP-"
 //               },
 //               { state: "home.applicant.myRequests", isItem: false },
 //               { state: "home.applicant.allRequests", isItem: false }
 //               ],
 //               actions: [{
 //                   name: "My Applicants",
 //                   title: "My Applicants",
 //                   state: "home.applicant.myRequests",
 //                   roles: [1, 4, 32],
 //                   type: 1
 //               },
 //               {
 //                   name: "All Applicants",
 //                   title: "All Applicants",
 //                   state: "home.applicant.allRequests",
 //                   roles: [1, 4, 32],
 //                   type: 2
 //               }
 //               ]
 //           },
 //           {
 //               name: "NEW_STAFF_MENU",
 //               selected: false,
 //               isParentMenu: false,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 2, 4, 32],
 //               type: [0, 1],
 //               ref: [{ state: "home.newStaffOnboard", isItem: false }],
 //               url: "home.newStaffOnboard",
 //               index: 4
 //           },
 //           {
 //               name: "PROMOTE_TRANSFER_MENU",
 //               selected: false,
 //               url: [],
 //               index: 5,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: [0, 1],
 //               isParentMenu: false,
 //               ref: [{
 //                   state: "home.promoteAndTransfer.item",
 //                   isItem: true,
 //                   title: "PROMOTION & TRANSFER RECOMMENDATION",
 //                   prefix: "PAT-"
 //               },
 //               {
 //                   state: "home.promoteAndTransfer.approve",
 //                   isItem: true,
 //                   title: "PROMOTION & TRANSFER RECOMMENDATION",
 //                   prefix: "PAT-"
 //               },
 //               { state: "home.promoteAndTransfer.myRequests", isItem: false },
 //               { state: "home.promoteAndTransfer.allRequests", isItem: false }
 //               ],
 //               actions: [{
 //                   name: "My Requests",
 //                   title: "My Promote And Transfer Requests",
 //                   state: "home.promoteAndTransfer.myRequests",
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   type: 1
 //               },
 //               {
 //                   name: "All Requests",
 //                   title: "All Promote And Transfer Requests",
 //                   state: "home.promoteAndTransfer.allRequests",
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   type: 2
 //               }
 //               ]
 //           },
 //           {
 //               name: "ACTING_MENU",
 //               selected: false,
 //               url: [],
 //               index: 3,
 //               gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: [0, 1],
 //               isParentMenu: false,
 //               ref: [{
 //                   state: "home.action.item",
 //                   isItem: true,
 //                   title: "ACTING",
 //                   prefix: "ACT-"
 //               },
 //               {
 //                   state: "home.action.itemApprove",
 //                   isItem: true,
 //                   title: "ACTING",
 //                   prefix: "ACT-"
 //               },
 //               {
 //                   state: "home.action.itemAppraise",
 //                   isItem: true,
 //                   title: "ACTING APPRAISING",
 //               },
 //               { state: "home.action.myRequests", isItem: true },
 //               { state: "home.action.allRequests", isItem: true }
 //               ],
 //               actions: [{
 //                   name: "My Requests",
 //                   title: "My Acting Requests",
 //                   state: "home.action.myRequests",
 //                   gradeUsers: [4, 5, 6, 7, 8, 9],
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   type: [0, 1]
 //               },
 //               {
 //                   name: "All Requests",
 //                   title: "All Acting Requests",
 //                   state: "home.action.allRequests",
 //                   gradeUsers: [4, 5, 6, 7, 8, 9],
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   type: [0, 1]
 //               }
 //               ]
 //           }
 //       ]
 //   },
 //   {
 //       index: 3,
 //       url: ".cb",
 //       selected: false,
 //       name: "COMMON_CB",
 //       title: "COMMON_CB",
 //       ref: "home.cb",
 //       isParentMenu: true,
 //       gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1],
 //       childMenus: [{
 //           name: "LEAVE_MANAGEMENT_MENU",
 //           selected: false,
 //           url: [],
 //           index: 1,
 //           roles: [1, 2, 4, 8, 16, 32],
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           type: [0, 1],
 //           isParentMenu: false,
 //           isNotTargetPlan: true,
 //           ref: [{
 //               state: "home.leavesManagement.item",
 //               isItem: true,
 //               title: "LEAVE MANAGEMENT",
 //               prefix: "LM-"
 //           },
 //           { state: "home.leavesManagement.myRequests", isItem: false },
 //           { state: "home.leavesManagement.allRequests", isItem: false }
 //           ],
 //           actions: [{
 //               name: "COMMON_MY_REQUEST",
 //               title: "My Leave Requests",
 //               state: "home.leavesManagement.myRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 1
 //           },
 //           {
 //               name: "COMMON_ALL_REQUEST",
 //               title: "All Leave Requests",
 //               state: "home.leavesManagement.allRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 2
 //           }
 //           ]
 //       },
 //       {
 //           name: "MISSING_TIMECLOCK_MENU",
 //           selected: false,
 //           url: "",
 //           index: 2,
 //           isParentMenu: false,
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           roles: [1, 2, 4, 8, 16, 32],
 //           type: [0, 1],
 //           isNotTargetPlan: true,
 //           ref: [{
 //               state: "home.missingTimelock.item",
 //               isItem: true,
 //               title: "MISSING TIMECLOCK",
 //               prefix: "MT-"
 //           },
 //           {
 //               state: "home.missingTimelock.approve",
 //               isItem: true,
 //               title: "MISSING TIMECLOCK",
 //               prefix: "MT-"
 //           },
 //           { state: "home.missingTimelock.myRequests", isItem: false },
 //           { state: "home.missingTimelock.allRequests", isItem: false }
 //           ],
 //           actions: [{
 //               name: "COMMON_MY_REQUEST",
 //               title: "My Missing TimeClock Requests",
 //               state: "home.missingTimelock.myRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 1
 //           },
 //           {
 //               name: "COMMON_ALL_REQUEST",
 //               title: "All Missing TimeClock Requests",
 //               state: "home.missingTimelock.allRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 2
 //           }
 //           ]
 //       },
 //       {
 //           name: "OVERTIME_TIMECLOCK_MENU",
 //           selected: false,
 //           url: [],
 //           index: 1,
 //           isParentMenu: false,
 //           gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //           roles: [1, 2, 4, 8, 16, 32],
 //           type: [0, 1],
 //           isNotTargetPlan: true,
 //           ref: [{
 //               state: "home.overtimeApplication.item",
 //               isItem: true,
 //               title: "Overtime Application"
 //           },
 //           {
 //               state: "home.overtimeApplication.view",
 //               isItem: true,
 //               title: "OA",
 //               prefix: ""
 //           },
 //           { state: "home.overtimeApplication.myRequests", isItem: false },
 //           { state: "home.overtimeApplication.allRequests", isItem: false }
 //           ],
 //           actions: [{
 //               name: "COMMON_MY_REQUEST",
 //               title: "My Overtime Requests",
 //               state: "home.overtimeApplication.myRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 1
 //           },
 //           {
 //               name: "COMMON_ALL_REQUEST",
 //               title: "All Overtime Requests",
 //               state: "home.overtimeApplication.allRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 2
 //           }
 //           ]
 //       },
 //       {
 //           name: "SHIFT_PLAN_MENU",
 //           selected: false,
 //           url: "",
 //           index: 1,
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           roles: [1, 2, 4, 8, 16, 32],
 //           type: [0, 1],
 //           isHQ: true,
 //           isParentMenu: false,
 //           isNotTargetPlan: true,
 //           ref: [
 //               { state: "home.pendingTargetPlan.item", isItem: true },
 //               { state: 'home.targetPlan.myRequests', isItem: false },
 //               { state: 'home.targetPlan.allRequests', isItem: false },
 //               { state: 'home.targetPlan.shiftPlan', isItem: false },
 //               { state: 'home.targetPlan.item', isItem: false },
 //               { state: 'home.targetPlan.reports', isItem: false }
 //           ],
 //           actions: [
 //               // {
 //               //     name: "ADD_TARGET_PLAN",
 //               //     title: "Add Target Plan",
 //               //     state: "home.addTargetPlan.item",
 //               //     type: 1
 //               // },
 //               {
 //                   name: "TARGET_PLAN",
 //                   title: "Target Plan",
 //                   children: [
 //                       { state: 'home.targetPlan.myRequests', name: 'COMMON_MY_REQUEST', isItem: false },
 //                       { state: 'home.targetPlan.allRequests', name: 'COMMON_ALL_REQUEST', isItem: false },
 //                   ],
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   type: 1
 //               },
 //               {
 //                   name: "SHIFT_PLAN_MENU",
 //                   title: "Shift Plan",
 //                   state: "home.targetPlan.shiftPlan",
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   children: [
 //                       { state: '', name: `DOWNLOAD_SHIFT_CODE`, url: 'ClientApp/assets/templates/Shift Codes.xlsx', item: false },
 //                   ],
 //                   type: 2
 //               },
 //               {
 //                   name: "REPORT_TARGET_PLAN",
 //                   title: "Target Plan",
 //                   state: "home.targetPlan.reports",
 //                   gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //                   roles: [1, 2, 4, 8, 16, 32],
 //                   type: [0, 1]
 //               }]
 //       },
 //       {
 //           name: "SHIFT_EXCHANGE_MENU",
 //           selected: false,
 //           url: "",
 //           index: 1,
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           roles: [1, 2, 4, 8, 16, 32],
 //           type: [0, 1],
 //           isParentMenu: false,
 //           isNotTargetPlan: true,
 //           ref: [{
 //               state: "home.shiftExchange.item",
 //               isItem: true,
 //               title: "SHIFT EXCHANGE APPLICATION",
 //               prefix: "SEA-"
 //           },
 //           {
 //               state: "home.shiftExchange.itemView",
 //               isItem: true,
 //               title: "SHIFT EXCHANGE APPLICATION",
 //               prefix: "SEA-"
 //           },
 //           { state: "home.shiftExchange.myRequests", isItem: false },
 //           { state: "home.shiftExchange.allRequests", isItem: false }
 //           ],
 //           actions: [{
 //               name: "COMMON_MY_REQUEST",
 //               title: "My Shift Exchange Requests",
 //               state: "home.shiftExchange.myRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 1
 //           },
 //           {
 //               name: "COMMON_ALL_REQUEST",
 //               title: "All Shift Exchange Requests",
 //               state: "home.shiftExchange.allRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 2
 //           }
 //           ]
 //       },
 //       {
 //           name: "RESIGNATION_APPLICATION_MENU",
 //           selected: false,
 //           url: "",
 //           index: 5,
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           roles: [1, 2, 4, 8, 16, 32],
 //           type: [0, 1],
 //           isParentMenu: false,
 //           ref: [{
 //               state: "home.resignationApplication.item",
 //               isItem: true,
 //               title: "RESIGNATION APPLICATION",
 //               prefix: "PAT-"
 //           },
 //           {
 //               state: "home.resignationApplication.approve",
 //               isItem: true,
 //               title: "RESIGNATION APPLICATION",
 //               prefix: "PAT-"
 //           },
 //           { state: "home.resignationApplication.myRequests", isItem: false },
 //           { state: "home.resignationApplication.allRequests", isItem: false }
 //           ],
 //           actions: [{
 //               name: "RESIGNATION_APPLICATION_MY_REQUETS",
 //               title: "My Resignation Requests",
 //               state: "home.resignationApplication.myRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 1
 //           },
 //           {
 //               name: "RESIGNATION_APPLICATION_ALL_REQUETS",
 //               title: "All Resignation Requests",
 //               state: "home.resignationApplication.allRequests",
 //               roles: [1, 2, 4, 8, 16, 32],
 //               type: 2
 //           }
 //           ]
 //       }
 //       ]
 //   },
 //   {
 //       index: 4,
 //       url: ".admin",
 //       selected: false,
 //       name: "COMMON_ADMIN",
 //       title: "COMMON_ADMIN",
 //       ref: "home.admin",
 //       isParentMenu: true,
 //       gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1],
 //       childMenus: [{
 //           name: "HANDOVER_MENU",
 //           selected: false,
 //           url: [],
 //           index: 3,
 //           isParentMenu: false,
 //           gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //           roles: [1, 32],
 //           type: [0, 1],
 //           isFacility: true,
 //           ref: [{
 //               state: "home.handover.item",
 //               isItem: true,
 //               title: "HANDOVER",
 //               prefix: "HAND-"
 //           },
 //           { state: "home.handover.myHandover", isItem: false },
 //           { state: "home.handover.allHandover", isItem: false }
 //           ],
 //           actions: [{
 //               name: "My HandOvers",
 //               title: "My Handovers",
 //               state: "home.handover.myHandover",
 //               roles: [1, 32],
 //               type: 1,
 //               isFacility: true
 //           },
 //           {
 //               name: "All HandOvers",
 //               title: "All Handovers",
 //               state: "home.handover.allHandover",
 //               roles: [1, 32],
 //               type: 2,
 //               isFacility: true
 //           }
 //           ]
 //       },
 //       {
 //           name: "BUSINESS_TRIP_APPLICATION",
 //           selected: false,
 //           url: [],
 //           index: 1,
 //           roles: [1, 2, 4, 8, 16, 32, 64],
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           type: [0, 1],
 //           isParentMenu: false,
 //           ref: [{
 //               state: "home.business-trip-application.item",
 //               isItem: true,
 //               title: "BUSINESS TRIP APPLICATION",
 //               prefix: "BTA-"
 //           },
 //           { state: "home.business-trip-application.myRequests", isItem: false },
 //           { state: "home.business-trip-application.allRequests", isItem: false }
 //           ],
 //           actions: [{
 //               name: "COMMON_MY_REQUEST",
 //               title: "BTA_MY_REQUESTS",
 //               state: "home.business-trip-application.myRequests",
 //               roles: [1, 2, 4, 8, 16, 32, 64],
 //               type: 1
 //           },
 //           {
 //               name: "COMMON_ALL_REQUEST",
 //               title: "BTA_ALL_REQUESTS",
 //               state: "home.business-trip-application.allRequests",
 //               roles: [1, 2, 4, 8, 16, 32, 64],
 //               type: 2
 //           }
 //           ]
 //       },
 //       {
 //           name: "BTA_REPORT",
 //           selected: false,
 //           url: [],
 //           index: 2,
 //           isParentMenu: false,
 //           roles: [1, 2, 64],
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           isAdmin: true,
 //           isAccounting: true,
 //           type: [0, 1],
 //           url: "home.business-trip-application-report",
 //           ref: [
 //               { state: "home.business-trip-application-report", isItem: false, title: "Business Trip Application Report" }

 //           ]
 //       },
 //       {
 //           name: "BTA_OVER_BUDGET",
 //           selected: false,
 //           url: [],
 //           index: 1,
 //           roles: [1, 2, 4, 8, 16, 32, 64],
 //           gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //           type: [0, 1],
 //           isParentMenu: false,
 //           ref: [{
 //               state: "home.over-budget.item",
 //               isItem: true,
 //               title: "BUSINESS TRIP OVER BUDGET",
 //               prefix: "BTA-"
 //           },
 //           { state: "home.over-budget.myRequests", isItem: false },
 //           { state: "home.over-budget.allRequests", isItem: false }
 //           ],
 //           actions: [{
 //               name: "COMMON_MY_REQUEST",
 //               title: "BTA_OVER_MY_REQUESTS",
 //               state: "home.over-budget.myRequests",
 //               roles: [1, 2, 4, 8, 16, 32, 64],
 //               type: 1
 //           },
 //           {
 //               name: "COMMON_ALL_REQUEST",
 //               title: "BTA_OVER_ALL_REQUESTS",
 //               state: "home.over-budget.allRequests",
 //               roles: [1, 2, 4, 8, 16, 32, 64],
 //               type: 2
 //           }
 //           ]

 //       }]
 //   },
 //   {
 //       index: 5,
 //       url: ".facility",
 //       selected: false,
 //       name: "COMMON_FACILITY",
 //       title: "COMMON_FACILITY",
 //       ref: "home.facility",
 //       isParentMenu: true,
 //       gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1]
 //   },
 //   {
 //       index: 7,
 //       name: "SUPPLIER_MANAGEMENT",
 //       title: "SUPPLIER_MANAGEMENT",
 //       url: '/SKU/home/dashboard', /*edoc.aeon.com.vn*/
 //       isPhrase1: true,
 //       gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1]
 //   },
	//{
 //       index: 8,
 //       name: "Edoc IT",
 //       title: "Edoc IT",
 //       url: '/IT',
 //       isPhrase1: true,
 //       gradeUsers: [],
 //       roles: [128],
 //       type: [0,1]
 //   },
 //   {
 //       index: 8,
 //       name: "LIQUOR LICENSE",
 //       title: "LICENSE",
 //       url: '/Liquor',
 //       isPhrase1: true,
 //       gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 4, 8, 16, 32],
 //       type: [0, 1]
 //   },
 //   {
 //       index: 9,
 //       url: ".setting",
 //       selected: false,
 //       name: "COMMON_SETTING",
 //       title: "COMMON_SETTING",
 //       ref: "home.setting",
 //       isParentMenu: true,
 //       gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //       roles: [1, 2, 8, 32],
 //       type: [0, 1],
 //       childMenus: [
 //           //     {
 //           //     name: "COMMON_REFERENCE_NUMBER",
 //           //     selected: false,
 //           //     isParentMenu: false,
 //           //     ref: [{ state: "home.referenceNumbers", isItem: false }],
 //           //     url: "home.referenceNumbers",
 //           //     index: 9,
 //           //     gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //           //     roles: [1, 32],
 //           //     type: [0, 1],
 //           // },
 //           // {
 //           //     name: "JOBGRADE_MENU",
 //           //     selected: false,
 //           //     isParentMenu: false,
 //           //     ref: [{ state: "home.jobGrades", isItem: false }],
 //           //     url: "home.jobGrades",
 //           //     index: 6,
 //           //     gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //           //     roles: [1, 32],
 //           //     type: [0, 1],
 //           // },
 //           {
 //               name: "User",
 //               selected: false,
 //               isParentMenu: false,
 //               ref: [{ state: "home.user-setting.user-list", isItem: true }],
 //               url: "home.user-setting.user-list",
 //               index: 1,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 32],
 //               type: [0, 1],
 //           },
 //           {
 //               name: "COMMON_DEPARTMENT",
 //               selected: false,
 //               ref: [{ state: "home.departments", isItem: false }],
 //               url: "home.departments",
 //               index: 2,
 //               isParentMenu: false,
 //               gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 32],
 //               type: [0, 1],
 //           },
 //           {
 //               // name: "Budget",
 //               name: "HEADCOUNT_MENU", // ticket 145
 //               selected: false,
 //               isParentMenu: false,
 //               ref: [{ state: "home.budgets", isItem: false }],
 //               url: "home.budgets",
 //               index: 7,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 32],
 //               type: [0, 1],
 //           },
 //           {
 //               name: "COMMON_RECRUIT",
 //               selected: false,
 //               url: '',
 //               index: 3,
 //               isParentMenu: false,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 32],
 //               type: [0, 1],
 //               ref: [
 //                   { state: "home.settingRecruitmentAppreciationList", isItem: false },
 //                   { state: "home.settingRecruitmentApplicantStatus", isItem: false },
 //                   { state: "home.settingRecruitmentCostCenter", isItem: false },
 //                   { state: "home.settingRecruitmenItemList", isItem: false },
 //                   { state: "home.settingRecruitmentPosition", isItem: false },
 //                   { state: "home.settingRecruitmentWorkingAddress", isItem: false },
 //                   { state: "home.settingRecruitmentWorkingTime", isItem: false },
 //                   { state: "home.settingRecruitmentCategories", isItem: false }
 //               ],
 //               actions: [
 //                   {
 //                       name: "Applicant Status List",
 //                       title: "Applicant Status List",
 //                       state: "home.settingRecruitmentApplicantStatus",
 //                       roles: [1, 32],
 //                       type: 1
 //                   },
 //                   {
 //                       name: "Appreciation List",
 //                       title: "Appreciation List",
 //                       state: "home.settingRecruitmentAppreciationList",
 //                       roles: [1, 32],
 //                       type: 2
 //                   },
 //                   {
 //                       name: "Cost Center",
 //                       title: "Cost Center",
 //                       state: "home.settingRecruitmentCostCenter",
 //                       roles: [1, 32],
 //                       type: 3
 //                   },
 //                   {
 //                       name: "Item List",
 //                       title: "Item List",
 //                       state: "home.settingRecruitmenItemList",
 //                       roles: [1, 32],
 //                       type: 4
 //                   },
 //                   {
 //                       name: "Position List",
 //                       title: "Position List",
 //                       state: "home.settingRecruitmentPosition",
 //                       roles: [1, 32],
 //                       type: 5
 //                   },
 //                   {
 //                       name: "Working Address",
 //                       title: "Working Address",
 //                       state: "home.settingRecruitmentWorkingAddress",
 //                       roles: [1, 32],
 //                       type: 6
 //                   },
 //                   {
 //                       name: "Working Time",
 //                       title: "Working Time",
 //                       state: "home.settingRecruitmentWorkingTime",
 //                       roles: [1, 32],
 //                       type: 7
 //                   },
 //                   {
 //                       name: "Categories",
 //                       title: "Categories",
 //                       state: "home.settingRecruitmentCategories",
 //                       roles: [1, 32],
 //                       type: 8
 //                   }
 //               ]
 //           },
 //           {
 //               name: 'COMMON_CB',
 //               ref: [
 //                   { state: 'home.missingTimeclockReasons', isItem: false },
 //                   { state: 'home.overtimeReasons', isItem: false },
 //                   { state: 'home.resignationReasons', isItem: false },
 //                   { state: 'home.shiftExchangeReasons', isItem: false },
 //                   { state: 'home.holidaySchedule', isItem: false },
 //                   { state: 'home.shiftPlanSubmitPerson', isItem: false },
 //                   { state: 'home.shiftCode', isItem: false },
 //                   { state: 'home.targetPlanSpecial', isItem: false },
 //               ],
 //               selected: false,
 //               url: '',
 //               index: 4,
 //               isParentMenu: false,
 //               gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 32],
 //               type: [0, 1],
 //               actions: [{
 //                   name: 'Missing Timeclock Reasons',
 //                   title: 'Missing Timeclock Reasons',
 //                   state: 'home.missingTimeclockReasons',
 //                   roles: [1, 32],
 //                   isNotTargetPlan: true,
 //                   type: 1
 //               }, {
 //                   name: 'Overtime Reasons',
 //                   title: 'Overtime Reasons',
 //                   state: 'home.overtimeReasons',
 //                   roles: [1, 32],
 //                   isNotTargetPlan: true,
 //                   type: 2
 //               }, {
 //                   name: 'Shift Exchange Reasons',
 //                   title: 'Shift Exchange Reasons',
 //                   state: 'home.shiftExchangeReasons',
 //                   roles: [1, 32],
 //                   isNotTargetPlan: true,
 //                   type: 3
 //               }, {
 //                   name: 'Resignation Reasons',
 //                   title: 'Resignation Reasons',
 //                   state: 'home.resignationReasons',
 //                   roles: [1, 32],
 //                   isNotTargetPlan: true,
 //                   type: 4
 //               },
 //               // {
 //               //     name: 'Shift Plan Submit Persons',
 //               //     title: 'Shift Plan Submit Persons',
 //               //     state: 'home.shiftPlanSubmitPerson',
 //               //     roles: [1, 32],
 //               //     type: 5
 //               // }, {
 //               //     name: 'Holiday Schedule',
 //               //     title: 'Holiday Schedule',
 //               //     state: 'home.holidaySchedule',
 //               //     roles: [1, 32],
 //               //     type: 6
 //               // }
 //               {
 //                   name: 'Shift Plan',
 //                   title: 'Shift Plan',
 //                   children: [
 //                       {
 //                           name: 'Shift Plan Submit Persons',
 //                           title: 'Shift Plan Submit Persons',
 //                           state: 'home.shiftPlanSubmitPerson',
 //                           type: 5
 //                       },
 //                       {
 //                           name: 'Holiday Schedule',
 //                           title: 'Holiday Schedule',
 //                           state: 'home.holidaySchedule',
 //                           type: 6
 //                       },
 //                       {
 //                           name: 'Shift Code',
 //                           title: 'Shift Code',
 //                           state: 'home.shiftCode',
 //                           type: 7
	//					},
 //                       {
 //                           name: 'Target Plan Special',
 //                           title: 'Target Plan Special',
 //                           state: 'home.targetPlanSpecial',
 //                           type: 8
 //                       }

 //                   ],
 //                   roles: [1, 32],
 //               }]
 //           },
 //           {
 //               name: 'More_MENU',
 //               ref: [
 //                   { state: 'home.referenceNumbers', isItem: false },
 //                   { state: 'home.jobGrades', isItem: false },
 //                   { state: 'home.trackingLogs', isItem: false },
 //                   { state: 'home.airline', isItem: false },
 //                   { state: 'home.partition', isItem: false },
 //                   //{ state: 'home.globalLocation', isItem: false },
 //                   { state: 'home.hotel', isItem: false },
 //                   { state: 'home.flightNumber', isItem: false },
 //                   { state: 'home.location', isItem: false },
 //                   { state: 'home.roomType', isItem: false },
 //                   { state: 'home.btaPolicy', isItem: false },
 //                   { state: 'home.btaPolicySpecial', isItem: false },
 //                   { state: 'home.reason', isItem: false },
 //                   { state: 'home.workflows', isItem: false },
 //                   //{ state: 'home.user-setting.user-list', isItem: true },
 //                   { state: 'home.workflows.item', isItem: false },
 //                   { state: 'home.workflows.myRequests', isItem: false },
 //                   { state: 'home.daysConfiguration', isItem: false },
 //                   { state: 'home.businessModel', isItem: false },
 //                   { state: 'home.businessModelUnitMapping', isItem: false },
 //               ],
 //               selected: false,
 //               url: '',
 //               index: 7,
 //               isParentMenu: false,
 //               gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
 //               roles: [1, 2, 8, 32],
 //               type: [0, 1],
 //               actions: [
 //                   {
 //                       name: 'Workflow',
 //                       title: 'Workflow',
 //                       state: 'home.workflowszzyyxx',
 //                       roles: [1, 32],
 //                       type: 1
 //                   },
 //                   // {
 //                   //     name: 'User',
 //                   //     title: 'User',
 //                   //     state: 'home.user-setting.user-list',
 //                   //     type: 1
 //                   // },
 //                   {
 //                       name: 'Reference Number',
 //                       title: 'Reference Number',
 //                       state: 'home.referenceNumbers',
 //                       roles: [1, 32],
 //                       type: 1
 //                   },
 //                   {
 //                       name: 'Job Grade',
 //                       title: 'Job Grade',
 //                       state: 'home.jobGrades',
 //                       roles: [1, 32],
 //                       type: 1
 //                   },
 //                   {
 //                       name: 'Tracking Log',
 //                       title: 'Tracking Log',
 //                       state: 'home.trackingLogs',
 //                       roles: [1, 8, 32],
 //                       type: 1
 //                   },
 //                   {
 //                       name: 'Business Trip Application',
 //                       title: 'BTA',
 //                       children: [
 //                           { state: 'home.airline', name: 'Airline', isItem: true },
 //                           { state: 'home.partition', name: 'Partition', isItem: false },
 //                           //{ state: 'home.globalLocation', name: 'Global Location', isItem: false },
 //                           { state: 'home.hotel', name: 'Hotel', isItem: false },
 //                           { state: 'home.flightNumber', name: 'Flight Number', isItem: false },
 //                           { state: 'home.location', name: 'Business Trip Location', isItem: false },
 //                           { state: 'home.roomType', name: 'Room Type', isItem: false },
 //                           { state: 'home.btaPolicy', name: 'Budget Limit: HQ & Store', isItem: false },
 //                           { state: 'home.btaPolicySpecial', name: 'Budget Limit: Special Case', isItem: false },
 //                           { state: 'home.reason', name: 'Reason', isItem: true },
 //                       ],
 //                       roles: [1, 2],
 //                       type: 1
 //                   },
 //                   {
 //                       name: 'Days Configuration',
 //                       title: 'Days Configuration',
 //                       state: 'home.daysConfiguration',
 //                       roles: [1, 32],
 //                       type: 1
 //                   },
 //                   {
 //                       name: 'Business Model',
 //                       title: 'Business Model',
 //                       state: 'home.businessModel',
 //                       roles: [1, 128],
 //                       type: 1
 //                   },
 //                   {
 //                       name: 'Business Model Unit Mapping',
 //                       title: 'Business Model Unit Mapping',
 //                       state: 'home.businessModelUnitMapping',
 //                       roles: [1, 32, 128],
 //                       type: 1
 //                   }
 //               ]
 //           }
 //       ]
 //   }
 //   ],

    subSections: [
        {
            title: "COMMON_FIN",
            ref: "home.finance",
            gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0],
            sections: [{
                name: "COMMON_ADD_PURCHANSING",
                nameAdd: "COMMON_PURCHANSING",
                title: "New item: purchasing request",
                //url: "home.purchasing.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-purchase?source=dashboard',
                icon: "ClientApp/assets/images/icon/proposal1.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            },
            {
                name: "COMMON_ADD_PURCHANSING_FOR_MULTI",
                nameAdd: "COMMON_PURCHANSING_FOR_MULTI",
                title: "New item: purchasing request",
                //url: "home.purchasing.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-purchase-multiItem',
                icon: "ClientApp/assets/images/icon/proposal1.png",
                addToMenu: false,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            },
            {
                name: "COMMON_ADD_PAYMENT",
                nameAdd: "COMMON_PAYMENT",
                title: "New item: payment request",
                //url: "home.payment.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-payment?source=dashboard',
                icon: "ClientApp/assets/images/icon/requisition1.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            },
            {
                name: "COMMON_ADD_ADVANCE_REQUEST",
                nameAdd: "COMMON_ADVANCE_REQUEST",
                title: "New item: advance request",
                //url: "home.advance.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-advance?source=dashboard',
                icon: "ClientApp/assets/images/icon/advance.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            },
            {
                name: "COMMON_ADD_REIMBURSEMENT_PAYMENT",
                nameAdd: "COMMON_REIMBURSEMENT_PAYMENT",
                title: "New item: reimbursement payment request",
                //url: "home.reimbursementPayment.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-reimbursement-payment?source=dashboard',
                icon: "ClientApp/assets/images/icon/icon-NewReimbursementModels.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            },
            {
                name: "COMMON_ADD_BUSINESS_TRIP",
                nameAdd: "COMMON_BUSINESS_TRIP",
                title: "New item: business trip reimbursement request",
                //url: "home.businessTripReimbursement.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-reimbursement?source=dashboard',
                icon: "ClientApp/assets/images/icon/reimbursement.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            },
            {
                name: "COMMON_ADD_CREDIT_NOTE",
                nameAdd: "COMMON_CREDIT_NOTE",
                title: "New item: credit note request",
                //url: "home.creditNote.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-credit-note?source=dashboard',
                icon: "ClientApp/assets/images/icon/creditnote.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0]
            },
            {
                name: "COMMON_ADD_CONTRACT_APPROVAL",
                nameAdd: "COMMON_CONTRACT_APPROVAL",
                title: "New item: contract approval",
                //url: "home.contractApproval.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-contract-multiF2',
                icon: "ClientApp/assets/images/icon/request1.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            },
            {
                name: "COMMON_ADD_CONTRACT_APPROVAL_MANUAL",
                nameAdd: "COMMON_CONTRACT_APPROVAL_MANUAL",
                title: "New item: contract approval",
                //url: "home.contractApproval.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-contract-custom',
                icon: "ClientApp/assets/images/icon/request1.png",
                addToMenu: false,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            },
            {
                name: "COMMON_ADD_NON_EXPENSE_CONTRACT",
                nameAdd: "COMMON_NON_EXPENSE_CONTRACT",
                title: "New item: non-expense contract request",
                //url: "home.non-expense.item",
                ref: '',
                url: edoc1Url + '/default.aspx#/new-non-expense-contract?source=dashboard',
                icon: "ClientApp/assets/images/icon/non-expense.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0],
            }
            ]
        },
        {
            title: "COMMON_RECRUIT",
            ref: "home.recruitment",
            gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            sections: [{
                name: "COMMON_ADD_REQUEST_TO_HIRE",
                title: "New item: request to hire",
                ref: "home.requestToHire.item",
                url: '',
                icon: "ClientApp/assets/images/icon/request-to-hire.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            },
            // {
            //     name: "Add A Position",
            //     title: "New item: position",
            //     url: "home.position.detail",
            //     icon: "ClientApp/assets/images/icon/Position.png",
            //     addToMenu: false,
            //     roles: [1, 4]
            // },
            {
                name: "COMMON_ADD_APPLICANT",
                title: "New item: Applicant",
                ref: "home.applicant.item",
                url: '',
                icon: "ClientApp/assets/images/icon/Application.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 4, 32],
                type: [0, 1],
            },
            // {
            //     name: "COMMON_ADD_HANDOVER",
            //     title: "New item: Handover",
            //     ref: "home.handover.item",
            //     url: '',
            //     icon: "ClientApp/assets/images/icon/handover.jpg",
            //     addToMenu: true,
            //     gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
            //     roles: [1, 2, 4, 32],
            //     type: [0, 1],
            // },
            {
                name: "COMMON_ADD_PROMOTE_TRANSFER",
                title: "New item: PROMOTION & TRANSFER RECOMMENDATION",
                ref: "home.promoteAndTransfer.item",
                url: '',
                icon: "ClientApp/assets/images/icon/promote.png",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            },
            {
                name: "COMMON_ADD_ACTING",
                title: "New item: ACTING",
                ref: "home.action.item",
                url: '',
                icon: "ClientApp/assets/images/icon/Acting.jpg",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            }
                //{
                //    name: "Recruitment Dashboard",
                //    url: "",
                //    icon: "ClientApp/assets/images/icon/Recruiment-dashboard.jpg",
                //    addToMenu: false,
                //    roles: [1, 2, 4, 8, 16]
                //}
            ]
        },
        {
            title: "COMMON_CB",
            ref: "home.cb",
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            sections: [{
                name: "COMMON_ADD_LEAVE_MANAGEMENT",
                title: "NEW ITEM: LEAVE MANAGEMENT",
                ref: "home.leavesManagement.item",
                url: '',
                icon: "ClientApp/assets/images/icon/Leave-management.jpg",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                isNotTargetPlan: true,
                type: [0, 1],
            },
            {
                name: "COMMON_ADD_MISSING_TIMECLOCK",
                title: "NEW ITEM: MISSING TIMECLOCK",
                ref: "home.missingTimelock.item",
                url: '',
                icon: "ClientApp/assets/images/icon/Missing-Timelock.png",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                isNotTargetPlan: true,
                type: [0, 1],
            },
            {
                name: "COMMON_ADD_OVERTIME",
                title: "NEW ITEM: OVERTIME APPLICATION",
                ref: "home.overtimeApplication.item",
                url: '',
                icon: "ClientApp/assets/images/icon/OverTime.jpg",
                addToMenu: true,
                gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                isNotTargetPlan: true,
                type: [0, 1],
            },
            {
                name: "ADD_TARGET_PLAN",
                title: "Add Target Plan",
                ref: "home.pendingTargetPlan.item",
                url: '',
                icon: "ClientApp/assets/images/icon/SHIFTPLAN.png",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isNotTargetPlan: true,
                isHQ: true
            },
            {
                name: "COMMON_ADD_SHIFT_EXHCNAGE",
                title: "NEW ITEM: SHIFT EXCHANGE APPLICATION",
                ref: "home.shiftExchange.item",
                url: '',
                icon: "ClientApp/assets/images/icon/Shift-Exchange.jpg",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
                isNotTargetPlan: true,
                isHQ: true
            },
            {
                name: "COMMON_ADD_RESIGNATION",
                title: "New item: Resignation Application",
                ref: "home.resignationApplication.item",
                url: '',
                icon: "ClientApp/assets/images/icon/Resignation.jpg",
                addToMenu: true,
                gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                roles: [1, 2, 4, 8, 16, 32],
                type: [0, 1],
            }
            ]
        },
        {
            title: "COMMON_ADMIN",
            ref: "home.admin",
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            sections: [
                {
                    name: "COMMON_ADD_HANDOVER",
                    title: "New item: Handover",
                    ref: "home.handover.item",
                    url: '',
                    icon: "ClientApp/assets/images/icon/handover.jpg",
                    addToMenu: true,
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 32],
                    type: [0, 1],
                    isFacility: true
                },
                {
                    name: "BUSINESS_ADD_TRIP_APPLICATION",
                    title: "NEW ITEM: BUSINESS TRIP APPLICATION",
                    ref: "home.business-trip-application.item",
                    url: '',
                    icon: "ClientApp/assets/images/icon/business_trip.png",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: [0, 1],
                },
                {
                    name: "BTA_OVER_BUDGET",
                    title: "OVER BUDGET",
                    ref: "home.over-budget.allRequests",
                    url: '',
                    icon: "ClientApp/assets/images/icon/over_budget.png",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: [0, 1],
                },
                {
                    name: "In_Out_Document",
                    nameAdd: "In_Out_Document",
                    title: "In/Out Document",
                    ref: '',
                    url: 'http://eoffice.aeon.com.vn',
                    icon: "ClientApp/assets/images/icon/non-expense.png",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: [0, 1],
                },
                {
                    name: "CANTEEN",
                    nameAdd: "CANTEEN",
                    title: "CANTEEN",
                    ref: '',
                    url: 'http://canteen.aeon.com.vn/loginess',
                    icon: "ClientApp/assets/images/icon/canteen.png",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: [0, 1],
                }
            ]
        },
        {
            title: "COMMON_FACILITY",
            ref: "home.facility",
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0],
            sections: [
                {
                    name: "COMMON_ADD_A_STATIONERY",
                    nameAdd: "COMMON_ADD_A_STATIONERY",
                    title: "COMMON_ADD_A_STATIONERY",
                    ref: "",
                    url: '../AEONFacilities/#!/home/dashboard/stationery',
                    icon: "ClientApp/assets/images/icon/icon-stationery.png",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0],
                },
                {
                    name: "COMMON_ADD_A_MATERIAL",
                    nameAdd: "COMMON_ADD_A_MATERIAL",
                    title: "COMMON_ADD_A_MATERIAL",
                    ref: "",
                    url: '../AEONFacilities/#!/home/dashboard/material',
                    icon: "ClientApp/assets/images/icon/icon-mateial.png",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32],
                    type: [0],
                },
                {
                    name: "COMMON_ADD_HANDOVER",
                    title: "New item: Handover",
                    ref: "home.handover.item",
                    url: '',
                    icon: "ClientApp/assets/images/icon/handover.jpg",
                    addToMenu: true,
                    gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 32],
                    type: [0],
                    isFacility: true
                }
            ]
        },
        {
            title: "COMMON_TRADE_CONTRACT",
            ref: "",
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            sections: [
                {
                    name: "COMMON_TRADE_CONTRACT",
                    nameAdd: "COMMON_TRADE_CONTRACT",
                    title: "Trade Contract",
                    ref: '',
                    url: 'http://edoc_l_trade.aeon.com.vn/',
                    icon: "ClientApp/assets/images/icon/TradeContract.png",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: [0, 1],
                }
            ]
        },
		{
            title: "SUPPLIER_MANAGEMENT",
			index: 6,
            ref: "",
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            sections: [
                {
                    name: "SUPPLIER_MANAGEMENT",
                    nameAdd: "SUPPLIER_MANAGEMENT",
                    title: "SUPPLIER_MANAGEMENT",
                    ref: '',
                    url: '/SKU/home/dashboard',
                    icon: "ClientApp/assets/images/icon/sku.jpg",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: [0, 1],
                }
            ]
        },
        {
            title: "LIQUOR LICENSE",
            index: 6,
            ref: "",
            gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
            roles: [1, 2, 4, 8, 16, 32],
            type: [0, 1],
            sections: [
                {
                    name: "LIQUOR LICENSE",
                    nameAdd: "LIQUOR LICENSE",
                    title: "LIQUOR LICENSE",
                    ref: '',
                    url: '/SKU/home/dashboard',
                    icon: "ClientApp/assets/images/icon/liquor_menu.jpg",
                    addToMenu: true,
                    gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9],
                    roles: [1, 2, 4, 8, 16, 32, 64],
                    type: [0, 1],
                }
            ]
        }
    ],
    mappingStates: [
        { source: 'home.user-setting', destination: 'home.user-setting.user-list', params: { action: { title: 'Users' } } },
        { source: 'home.workflowszzyyxx', destination: 'home.workflowszzyyxx.myRequests', params: { action: { title: 'Users' } } },
        { source: 'home.maintenant', destination: 'home.maintenant.dashboard', params: { action: { title: 'Users' } } },
        { source: 'home', destination: 'home.dashboard' }
    ],
    actionNeedShowPopup: [{
        title: 'Approve',
        btnName: 'Approve',
        iconClass: 'fa fa-check font-green-jungle right-5'
    },
    {
        title: 'Requested To Change',
        btnName: 'Requested To Change',
        iconClass: 'fa fa-minus-circle font-red right-5'
    },
    {
        title: 'Reject',
        btnName: 'Reject',
        iconClass: 'fa fa-minus-circle font-red right-5'
    }
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
        { state: 'home.orgchart', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.recruitment', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.requestToHire.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.requestToHire.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.requestToHire.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.requestToHire.trackingImport', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.position', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.position.item', roles: [1, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.position.myRequests', roles: [1, 4, 32], type: [0, 1] },
        { state: 'home.position.allRequests', roles: [1, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.applicant.item', roles: [1, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.applicant.myRequests', roles: [1, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.applicant.allRequests', roles: [1, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.newStaffOnboard', roles: [1, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.handover.item', roles: [1, 2, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isFacility: true },
        { state: 'home.handover.myHandover', roles: [1, 2, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isFacility: true },
        { state: 'home.handover.allHandover', roles: [1, 2, 4, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isFacility: true },
        { state: 'home.promoteAndTransfer.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.promoteAndTransfer.approve', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.promoteAndTransfer.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.promoteAndTransfer.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.action.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.action.itemApprove', roles: [1, 4, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.action.itemAppraise', roles: [1, 4, 32], gradeUsers: [4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.action.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.action.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.cb', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.leavesManagement.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.leavesManagement.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.leavesManagement.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.leavesManagement.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.missingTimelock.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.missingTimelock.approve', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.missingTimelock.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.missingTimelock.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.overtimeApplication.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.overtimeApplication.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.overtimeApplication.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.pendingTargetPlan.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.targetPlan.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.targetPlan.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.targetPlan.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.targetPlan.shiftPlan', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.targetPlan.reports', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.shiftExchange.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.shiftExchange.itemView', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.shiftExchange.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.shiftExchange.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isHQ: true },
        { state: 'home.resignationApplication.item', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.resignationApplication.myRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.resignationApplication.allRequests', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.setting', roles: [1, 2, 8, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.referenceNumbers', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.jobGrades', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.departments', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.budgets', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.airline', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.partition', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        //{ state: 'home.globalLocation', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.flightNumber', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.hotel', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.location', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.roomType', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.btaPolicy', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.btaPolicySpecial', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.reason', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentCostCenter', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentWorkingAddress', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentCategories', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.missingTimeclockReasons', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.overtimeReasons', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.resignationReasons', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.shiftExchangeReasons', roles: [1, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.missingTimeclockReasons', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.overtimeReasons', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.shiftExchangeReasons', roles: [1, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.resignationReasons', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.holidaySchedule', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.trackingLogs', roles: [1, 8, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.workflowszzyyxx.myRequests', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.workflowszzyyxx.item', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.workflowszzyyxx', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.user-setting.user-list', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.user-setting.user-profile', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.shiftPlanSubmitPerson', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.shiftCode', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.targetPlanSpecial', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.bta', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.business-trip-application.myRequests', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.business-trip-application.allRequests', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.business-trip-application.item', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.business-trip-application-report', roles: [1, 2, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1], isAdmin: true, Accounting: true },
        { state: 'home.daysConfiguration', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.navigation-home', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.navigation-list', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.admin', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.facility', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.daysConfiguration', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.over-budget', roles: [1, 2, 4, 8, 16, 32], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.over-budget.myRequests', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.over-budget.allRequests', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.over-budget.item', roles: [1, 2, 4, 8, 16, 32, 64], gradeUsers: [1, 2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
		{ state: 'home.maintenant', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.maintenant.dashboard', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.workflowszzyyxx.viewlog', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.shiftExchange.viewlog', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.targetPlan.viewlog', roles: [1, 32], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.businessModel', roles: [1, 128], gradeUsers: [], type: [0, 1] },
        { state: 'home.businessModelUnitMapping', roles: [1, 32, 128], gradeUsers: [], type: [0, 1] },
        { state: 'home.settingRecruitmentPosition', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },

        { state: 'home.settingRecruitmentApplicantStatus', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.budgets', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentCostCenter', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmenItemList', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentPosition', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentWorkingAddress', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentWorkingTime', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentCategories', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.missingTimeclockReasons', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.overtimeReasons', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.shiftExchangeReasons', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.resignationReasons', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.jobGrades', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.applicant.myRequests', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.reason', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.holidaySchedule', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.referenceNumbers', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.targetPlanSpecial', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.hotel', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.location', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.btaPolicy', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.roomType', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.flightNumber', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.airline', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.businessModelUnitMapping', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.businessModel', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.btaPolicySpecial', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.partition', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.settingRecruitmentAppreciationList', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.targetPlan.reports', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.trackingLogs', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.trackingSyncOrgcharts', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.shiftPlanSubmitPerson', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.daysConfiguration', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.requestToHire.trackingImport', roles: [1, 32, 128], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
        { state: 'home.bta-error-message', roles: [1, 2], gradeUsers: [2, 3, 4, 5, 6, 7, 8, 9], type: [0, 1] },
    ],
    linkActions: [
        {
            index: 1,
            group: 'QA',
            link: 'http://kb.aeon.com.vn/index.php/vi/edocument'
        },
        {
            index: 2,
            group: 'COMMON_FIN',
            link: 'http://kb.aeon.com.vn/index.php/vi/edocument/taichinhketoan'
        },
        {
            index: 3,
            group: 'COMMON_RECRUIT',
            link: 'http://kb.aeon.com.vn/index.php/vi/edocument/nhansu/tuyen-dung'
        },
        {
            index: 4,
            group: 'COMMON_CB',
            link: 'http://kb.aeon.com.vn/index.php/vi/edocument/nhansu/c-b'
        }
    ],
    ExitInterview: 'https://bit.ly/2UlwJlE',
    TrackingType: {
        ChangeStepWorkflow: "ChangeStepWorkflow",
        SyncWorkflow: "SyncWorkflow",
        UpdateApproval: "UpdateApproval",
        UpdateStatus: "UpdateStatus",
        UpdateInformation: "UpdateInformation",
        UpdatePayload: "UpdatePayload",
        Update: "Update",
        Delete: "Delete",
        DeleteUser: "DeleteUser",
        UpdateUser: "UpdateUser",
        AddUser: "AddUser",
        Create: "Create",
        UpdateInformation: "UpdateInformation",
        SyncBusinessUnit: "SyncBusinessUnit",
        SendData: "SendData"
    },
    TrackingHistoryItemType: {
        Workflow: "Workflow",
        Department: "Department"
    },
    TrackingRequestUrl: {
        SAPCreateUser: "/api/User/SAPCreateUser"
    },
    TrackingLogDialogDefaultWidth: "1117px",
    DepartmentGroup: [{
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
    }
    ],
    NavgationUrl: "https://edoc.aeon.com.vn/Home"
});