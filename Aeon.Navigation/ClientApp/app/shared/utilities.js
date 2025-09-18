var underscore = angular.module("underscore", []);
underscore.factory("_", [
    "$window",
    function () {
        return $window._;
    }
]);

moment = angular.module("moment-module", []);
moment.factory("moment", function ($window) {
    return $window.moment;
});
var app = angular.module("constantModule", []);
app.constant("commonData", {
    appName: "SSG",
    ActionForPromoteAndTransfer: {
        New: "New",
        Update: "Update",
        Approve: "Approve"
    },
    Status: {
        Waiting: "Waiting",
        Completed: "Completed",
        Rejected: "Rejected"
    },
    StatusMissingTimeLock: {
        Completed: "Completed",
        WaitingCMD: "Waiting for CMD Checker Approval",
        WaitingHOD: "Waiting for HOD Approval",
        Rejected: "Rejected"
    },
    gridActions: {
        Save: 'save',
        Edit: 'edit',
        Remove: 'remove',
        StatusChange: 'statusChange',
        Cancel: 'cancel',
        Create: 'create'
    },
    RecruitmentSetting: {
        IsWorkingTime: 'workingTime',
        IsItemList: 'itemList'
    },
    confirmContents: {
        resetPassword: 'Are you sure you want to reset password ?',
        remove: 'Are you sure you want to delete this record ?',
        inactive: 'Are you sure you want to Inactive this user ?',
        active: 'Are you sure you want to Active this user ?',
        activeWorkflow: 'Are you sure you want to Active this workflow ?',
        inactiveWorkflow: 'Are you sure you want to Inactive this workflow ?'
    },
    statusApplicant: [
        { name: 'Signed Offer' }
    ],
    typePosition: {
        isThumbNail: 'ThumbNail',
        isList: 'List'
    },
    pageStatus: {
        myRequests: 'myRequests',
        allRequests: 'allRequests',
        item: 'item',
        approve: 'approve',
    },
    statusResignationApp: {
        draft: 'Draft',
        waitingCMD: 'Waiting for CMD Checker Approval'
    },
    statusMissingTimelock: {
        draft: 'Draft',
        waitingCMD: 'Waiting for CMD Checker Approval'
    },
    referenceNumbers: [
        { name: 'Request To Hire', code: '1' },
        { name: 'Position', code: '2' },
        { name: 'Applicant', code: '3' },
        { name: 'Handover', code: '4' },
        { name: 'Promote And Transfer', code: '5' },
        { name: 'Acting', code: '6' },
        { name: 'LeaveApplication', code: '7' },
        { name: 'Missing Timeclock Application', code: '8' },
        { name: 'OvertimeApplication', code: '9' },
        { name: 'Shift Exchange Application', code: '10' },
        { name: 'Resignation Application', code: '11' },
    ],
    stateActing: {
        item: 'home.action.item',
        approve: 'home.action.itemApprove',
        myRequests: 'home.action.myRequests',
        allRequests: 'home.action.allRequests',
        appraise: 'home.action.itemAppraise'
    },
    reasons: [{
        name: "Quên đem thẻ (Forget employee card)",
        code: "MT0001"
    },
    {
        name: "Bị mất thẻ/hư thẻ (Losing employee card/broken)",
        code: "MT0002"
    },
    {
        name: "Quên quẹt thẻ (Forget scan time in/out)",
        code: "MT0003"
    },
    {
        name: "Công tác bên ngoài công ty (Working outsite)",
        code: "MT0004"
    },
    {
        name: "Lý do khác (Others)",
        code: "MT0005"
    }
    ],
    reasonOfResignations: [
        { name: "Nhân viên không ký hợp đồng lao động (Employee not sign labor con)", code: 1 },
        { name: "Lý do gia đình (Family reason)", code: 2 },
        { name: "Có công việc khác (Get another job)", code: 3 },
        { name: "Lý do sức khỏe (Health)", code: 4 },
        { name: "Nghỉ việc trong thời gian thử việc (Leaving during probation)", code: 5 },
        { name: "Lương thấp (Low salary)", code: 6 },
        { name: "Nghĩa vụ quân sự (Military Service)", code: 7 },
        { name: "Chuyển công tác (Move the living)", code: 8 },
        { name: "Không phù hợp với công việc /môi trường (Not suitable w job/environment)", code: 9 },
        { name: "Không phù hợp với người quản lý (Not suitable with manager)", code: 10 },
        { name: "Lý do cá nhân (Personal reason)", code: 11 },
        { name: "Bỏ việc (Quit job)", code: 12 },
        { name: "Đi học (Study)", code: 13 },
        { name: "Công ty không ký hợp đồng lao động (Company not sign labor contract)", code: 14 },
        { name: "Kỷ luật (Discipline)", code: 15 },
        { name: "Không pass thử việc (Not pass probation)", code: 16 },
        { name: "Từ chức trong thời gian thử việc (Resigned During Probation)", code: 17 },
        { name: "Môi trường làm việc (Working Environment)", code: 18 },
        { name: "Thu nhập thấp (Low Income)", code: 19 },
        { name: "Hết hạn hợp đồng - Kỷ luật (Expiry Of Contract-Discipline)", code: 20 },
        { name: "Thỏa thuận chấm dứt hợp đồng lao động (Voluntary Separation Scheme)", code: 21 },
        { name: "Xa nơi làm việc (Far From Workplace) ", code: 22 },
        { name: "Sa thải (Dismissal) ", code: 23 },
        { name: "Đơn phương chấm dứt hợp đồng lao động (Expiry Of Contract-Own Request) ", code: 24 },
    ],
    // shuiBooks: [
    //     { name: "Employee hold it", code: 1, value: false },
    //     { name: "Company hold it", code: 2, value: false },
    //     { name: "Not yet contribute SHUI", code: 3, value: false }
    // ],
    shuiBooks: [
        { nameValue: 'Employee hold it', name: "Employee hold it", code: 1, value: false },
        { nameValue: 'Company hold it', name: "Company hold it", code: 2, value: false },
        { nameValue: 'Not yet contribute SHUI', name: "Not yet contribute SHUI", code: 3, value: false }
    ],
    contracts: [
        { nameVN: "Hợp đồng học việc, thử việc báo trước 01 ngày ", nameEN: "01 day prior notice must be given for definite", code: 1, day: 1, value: false, },
        { nameVN: "Hợp đồng thời vụ báo trước 03 ngày", nameEN: "  03 days prior notice must be given for definite", code: 2, day: 3, value: false },
        { nameVN: "Hợp đồng lao động có thời hạn, báo trước 30 ngày", nameEN: " 30 days prior notice must be given for definite term contract", code: 3, day: 30, value: false },
        { nameVN: "Hợp đồng lao động không thời hạn, báo trước 45 ngày", nameEN: " 45 days prior notice must be given for indefinite term contract", code: 4, day: 45, value: false },
    ],
    reasonOvertime: [
        { name: "Hỗ trợ thời gian đông khách", code: 1 },
        { name: "Bộ phận thiếu nhân sự", code: 2 },
        { name: "Hỗ trợ thời gian nghỉ lễ", code: 3 },
        { name: "Hỗ trợ thời gian khai trương", code: 4 },
        { name: "Khác (ghi rõ lý do)", code: 'OT0005' },
    ],
    stateOvertime: {
        item: 'home.overtimeApplication.item',
        approve: 'home.overtimeApplication.view',
        myRequests: 'home.overtimeApplication.myRequests',
        allRequests: 'home.overtimeApplication.allRequests',
    },
    stateShiftExchange: {
        item: 'home.shiftExchange.item',
        approve: 'home.shiftExchange.itemView',
        myRequests: 'home.shiftExchange.myRequests',
        allRequests: 'home.shiftExchange.allRequests',
    },
    resignationApplication: {
        item: 'home.resignationApplication.item',
        approve: 'home.resignationApplication.approve',
        myRequests: 'home.resignationApplication.myRequests',
        allRequests: 'home.resignationApplication.allRequests',
    },
    leaveManagement: {
        item: 'home.leavesManagement.item',
        myRequests: 'home.leavesManagement.myRequests',
        allRequests: 'home.leavesManagement.allRequests',
    },
    requestToHires: {
        item: 'home.requestToHire.item',
        myRequests: 'home.requestToHire.myRequests',
        allRequests: 'home.requestToHire.allRequests',
    },
    reasonType: {
        MISSING_TIMECLOCK_REASON: 'MISSING_TIME_CLOCK_REASON_TYPE',
        OVERTIME_REASON: 'OVERTIME_REASON_TYPE',
        RESIGNATION_REASON: 'RESIGNATION_REASON_TYPE',
        SHIFT_EXCHANGE_REASON: 'SHIFT_EXCHANGE_REASON_TYPE',
        POSITION_REASON: 'Position',
        TIME_CONFIGURATION: 'TIME_CONFIGURATION',
    },
    jobGrade: 'G1',
    actionExposeAPI: {
        Send: 0,
        Receive: 1
    },
    exportType: {
        REQUESTOHIRE: 1,
        POSITION: 2,
        APPLICANT: 3,
        NEWSTAFFONBOARD: 4,
        HANDOVER: 5,
        PROMOTEANDTRANSFER: 6,
        ACTING: 7,
        LEAVEMANAGEMENT: 8,
        MISSINGTIMECLOCK: 9,
        OVERTIME: 10,
        SHIFTEXCHANGE: 11,
        RESIGNATION: 12,
        DEPARTMENT: 13,
        HEADCOUNT: 14,
        MISSINGTIMECLOCKREASON: 15,
        OVERTIMEREASON: 16,
        SHIFTEXCHANGEREASON: 17,
        RESIGNATIONREASON: 18,
        USER: 19,
        TRACKINGLOG: 20,
        WORKINGTIMERECRUITMENT: 21,
        ITEMLISTRECRUITMENT: 22,
        APPLICANTSTATUSLISTRECRUITMENT: 23,
        APPRECIATIONLISTRECRUITMENT: 24,
        POSITIONLISTRECRUITMENT: 25,
        JOBGRADE: 26,
        POSITIONDETAIL: 27,
        COSTCENTERRECRUITMENT: 28,
        HOTELSETTING: 29,
        BUSINESSTRIPLOCATIONSETTING: 30,
        FLIGHTNUMBERSETTING: 31,
        AIRLINESETTING: 32,
        ROOMTYPESETTING: 33,
        BUSINESSTRIPAPPLICATION: 35,
        WORKINGADDRESSRECRUITMENT: 36,
        HOLIDAYSCHEDULE: 37,
        TARGETPLAN: 40,
        OVERTIME_FILL_ACTUAL_DETAILS: 41,
        MAINTAINPROMOTEANDTRANFERPRINT: 42,
        BTAPOLICYSPECIALCASES: 43,
        FLIGHTSBOOKING: 44,
        OVERBUDGET: 45
    },
    positionStatus: {
        None: 0,
        Opened: 1,
        Closed: 2,
        Draft: 3
    },
    typeOfNeed: {
        NewPosition: 1,
        ReplacementFor: 2,

    },
    myRequests: {
        RequestToHire: 'home.requestToHire.myRequests',
        Position: 'home.position.myRequests',
        Applicant: 'home.applicant.myRequests',
        Handover: 'home.handover.myHandover',
        PromoteAndTranfer: 'home.promoteAndTransfer.myRequests',
        Acting: 'home.action.myRequests',
        LeaveManagement: 'home.leavesManagement.myRequests',
        MissingTimeLock: 'home.missingTimelock.myRequests',
        OvertimeApplication: 'home.overtimeApplication.myRequests',
        ShiftExchange: 'home.shiftExchange.myRequests',
        ResignationApplication: 'home.resignationApplication.myRequests'
    },
    itemStatusesHR: [
        "Draft", "Cancelled", "Completed", "Rejected", "Requested to Change", "In Progress"
    ],
    itemStatuses: [
        { name: "STATUS_DRAFT", code: 'Draft' },
        { name: "STATUS_CANCEL", code: 'Cancelled' },
        { name: "STATUS_COMPLETED", code: 'Completed' },
        { name: "STATUS_REJECT", code: 'Rejected' },
        { name: "STATUS_REQUEST_CHANGE", code: 'Requested to Change' },
        { name: "STATUS_INPROGRESS", code: 'In Progress' },
        { name: "STATUS_PENDING", code: 'Pending' },
    ],
    itemBtaStatuses: [
        { name: "STATUS_DRAFT", code: 'Draft' },
        { name: "STATUS_CANCEL", code: 'Cancelled' },
        { name: "BTA_CHANGING_CACELLING_BUSNESS_TRIP", code: 'Changing/ Cancelling Business Trip' },
        { name: "STATUS_COMPLETED", code: 'Completed' },
        { name: "STATUS_COMPLETED_CHANGING", code: 'Completed Changing' },
        { name: "STATUS_REJECT", code: 'Rejected' },
        { name: "STATUS_REQUEST_CHANGE", code: 'Requested To Change' },
        { name: "STATUS_INPROGRESS", code: 'In Progress' },
        { name: "STATUS_PENDING", code: 'Pending' },
        { name: "STATUS_REVOKE", code: 'Revoking' }
    ],
    itemFlightsBookingStatuses: [
        { name: "STATUS_COMPLETED", code: 'Completed' },
        { name: "STATUS_CANCEL", code: 'Cancelled' },
    ],
    role: { SAdmin: 1, Admin: 2, HR: 4, CB: 8, Member: 16, HRAdmin: 32 },
    typeActualTime: [
        { name: 'In (Vào)', value: 1 },
        { name: 'Out (Ra)', value: 2 },
    ],
    checkBudgetOption: {
        BUDGET: 1,
        NOBUDGET: 2
    },
    OverTimeType: {
        EmployeeSeftService: 1,
        ManagerApplyForEmployee: 2
    },
    reasonOptions: [
        { name: "Resign", value: 1 },
        { name: "Transfer", value: 2 },
        { name: "Promote", value: 3 },
        { name: "Acting", value: 4 },
        { name: "Maternity Leave", value: 5 },
        { name: "Others", value: 6 }
    ],
    operationOptions: [
        { name: "Store", value: 1 },
        { name: "HQ", value: 2 }
    ],
    typeAvailableLeaveBalance: {
        ERD: '12',
        DOFL: '13',
    },
    foreignerOptions: [
        { name: "Not Foreigner", value: 0 },
        { name: "Foreigner", value: 1 },
        { name: "Both", value: 2 }
    ],
    statusImportMass: [
        { code: "Đạt", name: "Pass", value: '( interviewResult=="Đạt" || interviewResult=="Chuyển vị trí khác" )' },
        { code: "Không đạt", name: "Not Pass", value: '( interviewResult=="Không đạt" )' },
        { code: "All", name: "All" }
    ],
    checkStatusImportMass: {
        Pass: "Đạt",
        Fail: "Không đạt",
        All: "All"
    },
    targetPlanType: { target1: 'Target 1', target2: 'Target 2', target3: 'Actual 1', target4: 'Actual 2' },
    color: {
        Active: 'hsl(0, 0%, 93%)', //màu xám default
        Sun: ' #A8D08D',
        LeaveActual: '#02ACE8', //màu cam
        ShiftActual: '#F3A875', //màu xanh dương
        LeaveActualOrShiftActualInProgress: '#F9F1C7', //màu vàng
    },
    validateTargetPlan: {
        PRD: "PRD",
        ERD: "ERD",
        G2: "G2",
        AL: "AL",
        DOFL: "DOFL",
    },
    halfDayCode: {
        ERD1: "ERD1",
        ERD2: "ERD2",
        DOH1: "DOH1",
        DOH2: "DOH2",
        ALH1: "ALH1",
        ALH2: "ALH2",
        NPH1: "NPH1",
        NPH2: "NPH2",
        TRN1: "TRN1",
        TRN2: "TRN2"
    },
    checkTypeTargetPLan: {
        target1: '1',
        target2: '2'
    }
});
// Common function
function validateEmail(email) {
    var errorMessages = [];
    var regrex = /^([a-zA-Z0-9_\.\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/i;
    if (!regrex.test(email)) {
        errorMessages.push('Email');
    }
    return errorMessages;
}

function validateDataInGrid(dataItem, requiredFields, dateFields, Notification, arrayItemsNotNull = []) {
    // requiredFields: [{fieldName: 'abc', title: 'a b c'}]
    // dateFields: [[{fieldName: 'abc', title: 'a b c'},{ fieldName: 'def', title: 'd e f'}]]
    // arrayItemsNotNull: [[{fieldName: 'abc', title: 'a b c'}]]
    var messages = "";
    var errorObject = {
        errorField: { title: 'Some fields are required: ', array: [] },
        errorFormatField: { title: 'Some fields are incorrect format: ', array: [] },
        errorDateField: { title: 'Some fields are incorrect value: ', array: [] }
    }
    // Required Fields
    if (requiredFields.length) {
        requiredFields.forEach(x => {
            if (!dataItem[x.fieldName]) {
                errorObject.errorField.array.push(`<li> - ${x.title}</li>`);
            } else if (x.fieldName === 'email') {
                var errorFormats = validateEmail(dataItem[x.fieldName]);
                if (errorFormats.length) {
                    errorFormats.forEach(ele => {
                        errorObject.errorFormatField.array.push(`<li> - ${ele}</li>`)
                    });
                }
            }
        });
    }
    // Array Items Not Null
    if (arrayItemsNotNull.length > 0) {
        arrayItemsNotNull.forEach(x => {
            if (!dataItem[x.fieldName].length) {
                errorObject.errorField.array.push(`<li> - ${x.title}</li>`);
            }
        })
    }
    // Date Fields
    if (dateFields.length) {
        dateFields.forEach(element => {
            var fromDate = element[0];
            var toDate = element[1];
            if (dataItem[fromDate.fieldName] && dataItem[toDate.fieldName] && dataItem[toDate.fieldName] < dataItem[fromDate.fieldName]) {
                errorObject.errorDateField.array.push(`<li> - ${toDate.title} must be greater ${fromDate.title}</li>`)
            }
        })
    }
    if (errorObject.errorField.array.length || errorObject.errorFormatField.array.length || errorObject.errorDateField.array.length) {
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
        if (errorObject.errorDateField.array.length) {
            var title = errorObject.errorDateField.title + "</br>";
            var subContent = "<ul>" + errorObject.errorDateField.array.join('') + "</ul>";
            messages += title + subContent;
        }
        Notification.error(messages);
        return false;
    }
    return true;
}

function validateForm(model, requiredFields) {
    let errors = [];
    requiredFields.forEach(x => {
        if (!model[x.fieldName]) {
            errors.push({ controlName: x.title, message: 'Field is required' });
        }
    })
    return errors;
}

function buildCustomHeader() {
    return {
        'Content-Type': 'application/json',
        "Accept": "application/json",
        "Access-Control-Allow-Origin": "*",
        //Should be remove if upgrade to prod
        "secret": sr,
        "uxr": uxr
    }
}

function buildCustomBlobHeader() {
    return {
        "Content-Type": undefined,
        "Accept": "application/json",
        "Access-Control-Allow-Origin": "*",
        "secret": sr,
        "uxr": uxr
    }
}

async function getBase64(file) {
    var res = new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
    });
    return await res;
}

function filterMultiField(ev) {
    var filterValue = ev.filter != undefined ? ev.filter.value : "";
    ev.preventDefault();
    let filters = [];
    if (this.options.customFilterFields) {
        this.options.customFilterFields.forEach(x => {
            filters.push({ field: x, operator: this.options.filter, value: filterValue })
        });
    } else {
        filters.push({ field: this.options.dataValueField, operator: this.options.filter, value: filterValue })
        filters.push({ field: this.options.dataTextField, operator: this.options.filter, value: filterValue })
    }
    this.dataSource.filter({
        logic: "or",
        filters: filters
    });
}

function convertByteArrayToUint8Array(content) {
    let byteCharacters = atob(content);
    let byteNumbers = new Array(byteCharacters.length);
    for (var i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    let byteArray = new Uint8Array(byteNumbers);
    return byteArray;
}

function isEmptyString(stringValue) {
    return stringValue == null || stringValue == undefined || stringValue.length == 0;
}

function exportToExcelFile(fileContent) {
    var content = convertByteArrayToUint8Array(fileContent.content);
    var blob = new Blob([content], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
    saveAs(blob, fileContent.fileName);
}

function exportToPdfFile(fileContent) {
    var content = convertByteArrayToUint8Array(fileContent.content);
    var blob = new Blob([content], { type: "Application/pdf" });
    saveAs(blob, fileContent.fileName);
}

function QueryArgs(predicate, predicateParammeter, order, pageIndex, limit) {
    return {
        predicate: genPredicate(predicate),
        predicateParameters: predicateParammeter.length ? predicateParammeter : [],
        order: order ? order : 'created desc',
        page: pageIndex ? pageIndex : 1,
        limit: limit ? limit : 20
    };
}

function genPredicate(predicate) {
    if (predicate && predicate.length > 0) {
        let _predicate = predicate.map((item, index) => {
            let temp = item.split('{i}').join(index);
            if (item.includes('{i+1}') > -1) {
                temp = temp.split('{i+1}').join(index + 1);
            }
            return temp;
        });
        let _resPredicate = _predicate.join(' && ');
        if (_resPredicate.trim().length > 0)
            return _predicate.join(' && ');
        else return '';
    } else return '';
}

function genPredicateKeyWord(conditions) {
    return `(${conditions.join(" || ")})`;
}

function genPredicateStatus(condition, values) {
    let predicate = [];
    for (let i = 0; i < values.length; i++) {
        predicate.push(condition.replace('[value]', `"${values[i]}"`));
    }
    return `(${predicate.join(" || ")})`;
}

function createPredicateFromDate(predicate, predicateParameters, fromDate) {
    if (fromDate) {
        predicate.push('created >= @{i}');
        predicateParameters.push(moment(fromDate, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    }
}

function createPredicateToDate(predicate, predicateParameters, toDate) {
    if (toDate) {
        predicate.push('created < @{i}');
        predicateParameters.push(moment(toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
    }
}

function createPredicateTrackingDate(predicate, predicateParameters, fromTrackingDate, toTrackingDate) {
    let fromDate = null;
    let toDate = null;
    if (fromTrackingDate && fromTrackingDate != "day/month/year") {
        fromDate = moment(fromTrackingDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
    }
    if (toTrackingDate && toTrackingDate != "day/month/year") {
        toDate = moment(toTrackingDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD');
    }
    if (fromDate && toDate && fromTrackingDate != "day/month/year" && toTrackingDate != "day/month/year") {
        predicate.push('trackingLogInitDatas.Any(x=> x.fromDate >= @{i} && x.toDate < @{i+1})');
        predicateParameters.push(fromDate);
        predicateParameters.push(toDate);
    }
    else if (fromDate) {
        predicate.push('trackingLogInitDatas.Any(x=> x.fromDate >= @{i})');
        predicateParameters.push(fromDate);
    }
    else if (toDate) {
        predicate.push('trackingLogInitDatas.Any(x=> x.toDate < @{i})');
        predicateParameters.push(toDate);
    }

}

function createPredicateStatus(predicate, predicateParameters, statuses) {
    if (statuses && statuses.length) {
        predicate.push(genPredicateStatus('status = [value]', statuses));
        predicateParameters.push('');
    }
}

function createPredicateCreateBy(predicate, predicateParameters, currentState, stateMyrequest, userId) {
    if (currentState == stateMyrequest) {
        predicate.push('createdById = @{i}');
        predicateParameters.push(userId);
    }
}

function createPredicateKeyWord(predicate, predicateParameters, conditions, keyWord) {
    if (keyWord) {
        predicate.push(genPredicateKeyWord(conditions));
        predicateParameters.push(keyWord);
    }
}
function setTimeByTimeInRound(date, TimeInRound) {
    if (date && TimeInRound) {
        let time = moment(date, "HH:mm");
        const interval = generateInterval(TimeInRound);
        for (let i = 0; i < interval.length - 1; i++) {
            let currentValue = moment(interval[i], "HH:mm");
            let nextValue = moment(interval[i + 1], "HH:mm");
            if (time > currentValue && time < nextValue) {
                time = nextValue;
                break;
            } else if (time <= currentValue) {
                time = currentValue;
                break;
            }
        }
        return moment(time).format("HH:mm");
    }
}
function generateInterval(interval) {
    let arr = [];
    let start = moment("00:00", "HH:mm");
    let max = moment("23:59", "HH:mm");
    arr.push(start.format("HH:mm"));
    while (start.add(interval, "minutes") < max) {
        arr.push(start.format("HH:mm"));
    }
    return arr;
}
function setTimeByTimeOutRound(date, TimeOutRound) {
    if (date && TimeOutRound) {
        let time = moment(date, "HH:mm");
        const interval = generateInterval(TimeOutRound);
        for (let i = 0; i < interval.length - 1; i++) {
            let currentValue = moment(interval[i], "HH:mm");
            let nextValue = moment(interval[i + 1], "HH:mm");
            if (time > currentValue && time < nextValue || time == currentValue) {
                time = currentValue;
                break;
            }
        }
        return moment(time).format("HH:mm");
    }
}

// hàm sài chung search bên c&b
// stateArgs : {currentState: 'xxx', myRequestState: 'constant lấy trong common data', currentUserId: 'user id của nhân viên đang login }
function createDefaultQueryArgsForCAB(pageIndex, pageSize, order, searchInfo, stateArgs) {
    let predicate = [];
    let predicateParameters = [];
    let conditions = [
        'ReferenceNumber.contains(@{i})',
        'userSAPCode.contains(@{i})',
        'createdByFullName.contains(@{i})',
        'deptName.contains(@{i})',
        'divisionName.contains(@{i})',
        'status.contains(@{i})',
        //'workLocation.contains(@{i})'
    ];

    if (searchInfo.departmentCode) {
        predicate.push('deptCode = @{i} or divisionCode = @{i}');
        predicateParameters.push(searchInfo.departmentCode);
        predicateParameters.push(searchInfo.departmentCode);
    }
    createPredicateKeyWord(predicate, predicateParameters, conditions, searchInfo.keyword);
    createPredicateStatus(predicate, predicateParameters, searchInfo.statusIds);
    createPredicateFromDate(predicate, predicateParameters, searchInfo.fromDate);
    createPredicateToDate(predicate, predicateParameters, searchInfo.toDate);

    // nếu là my request thì get những đơn nào do người đó tạo // hàm này đã check luôn điều kiên này
    createPredicateCreateBy(predicate, predicateParameters, stateArgs.currentState, stateArgs.myRequestState, stateArgs.currentUserId);

    return QueryArgs(predicate, predicateParameters, order, pageIndex, pageSize);
}

// những filter chung nhất thì vào cho vào hàm này
// stateArgs : {currentState: 'xxx', myRequestState: 'constant lấy trong common data', currentUserId: 'user id của nhân viên đang login }
function createQueryArgsForCAB(tableInfo, conditions, searchInfo, stateArgs) {

    let predicate = [];
    let predicateParameters = [];
    // nếu là my request thì get những đơn nào do người đó tạo // hàm này đã check luôn điều kiên này
    createPredicateCreateBy(predicate, predicateParameters, stateArgs.currentState, stateArgs.myRequestState, stateArgs.currentUserId);
    createPredicateKeyWord(predicate, predicateParameters, conditions, searchInfo.keyword);
    createPredicateStatus(predicate, predicateParameters, searchInfo.statusIds);
    createPredicateFromDate(predicate, predicateParameters, searchInfo.fromDate);
    createPredicateToDate(predicate, predicateParameters, searchInfo.toDate);
    createPredicateMissingDate(predicate, predicateParameters, searchInfo.missingDateFrom, searchInfo.missingDateTo);
    createPredicateOTDate(predicate, predicateParameters, searchInfo.otDateFrom, searchInfo.otDateTo);
    createPredicateResignationDate(predicate, predicateParameters, searchInfo.resignationDateFrom, searchInfo.resignationDateTo);
    var simpleArg = QueryArgs(predicate, predicateParameters, tableInfo.order, tableInfo.pageIndex, tableInfo.pageSize);
    if (searchInfo.departmentCode) {
        simpleArg.predicate = simpleArg.predicate ? simpleArg.predicate + ` and (deptCode = @${simpleArg.predicateParameters.length} or divisionCode = @${simpleArg.predicateParameters.length + 1})` : `(deptCode = @${simpleArg.predicateParameters.length} or divisionCode = @${simpleArg.predicateParameters.length + 1})`;
        simpleArg.predicateParameters.push(searchInfo.departmentCode);
        simpleArg.predicateParameters.push(searchInfo.departmentCode);

    }
    return simpleArg;
}
function createPredicateResignationDate(predicate, predicateParameters, fromDate, toDate) {
    let temp = '';
    if (fromDate && isValidDate(fromDate)) {
        temp = 'officialResignationDate >= @{i}';
        predicateParameters.push(moment(fromDate, 'DD/MM/YYYY').format('YYYY-MM-DD'));
        if (toDate && isValidDate(toDate)) {
            temp = 'officialResignationDate >= @{i} && officialResignationDate < @{i+1}';
            predicateParameters.push(moment(toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
        }
        predicate.push(temp);
    } else {
        if (toDate && isValidDate(toDate)) {
            temp = 'officialResignationDate < @{i}';
            predicateParameters.push(moment(toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
            predicate.push(temp);
        }
    }
}
function createPredicateOTDate(predicate, predicateParameters, fromDate, toDate) {
    let temp = '';
    if (fromDate && isValidDate(fromDate)) {
        temp = 'overtimeItems.Any(y => y.date >= @{i})';
        predicateParameters.push(moment(fromDate, 'DD/MM/YYYY').format('YYYY-MM-DD'));
        if (toDate && isValidDate(toDate)) {
            temp = 'overtimeItems.Any(y => y.date >= @{i} && y.date < @{i+1})';
            predicateParameters.push(moment(toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
        }
        predicate.push(temp);
    } else {
        if (toDate && isValidDate(toDate)) {
            temp = 'overtimeItems.Any(y => y.date < @{i})';
            predicateParameters.push(moment(toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
            predicate.push(temp);
        }
    }

}
function createPredicateMissingDate(predicate, predicateParameters, fromDate, toDate) {
    let temp = '';
    if (fromDate && isValidDate(fromDate)) {
        temp = 'missingTimeClockDetails.Any(y => y.date >= @{i})';
        predicateParameters.push(moment(fromDate, 'DD/MM/YYYY').format('YYYY-MM-DD'));
        if (toDate && isValidDate(toDate)) {
            temp = 'missingTimeClockDetails.Any(y => y.date >= @{i} && y.date < @{i+1})';
            predicateParameters.push(moment(toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
        }
        predicate.push(temp);
    } else {
        if (toDate && isValidDate(toDate)) {
            temp = 'missingTimeClockDetails.Any(y => y.date < @{i})';
            predicateParameters.push(moment(toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
            predicate.push(temp);
        }
    }
}
function showCodeName(model) {
    if (model && model.code && model.name) {
        return `${model.code} - ${model.name}`;
    }

}

function enableRestrictedProperties(fields, typeReplacement) {
    if (fields) {
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            if (typeReplacement !== 2) {
                try {
                    let fieldControls = $("input[k-ng-model*='" + field.fieldPattern + "'],select[data-k-ng-model*='dataItem." + field.fieldPattern + "']");
                    if (fieldControls) {
                        fieldControls.map((cIndex, currentFieldControl) => {
                            let treeView = $(currentFieldControl).data("kendoDropDownTree");
                            if (treeView) {
                                treeView.filterInput.removeAttr("disabled").removeAttr("readonly");
                            }

                            let dropDownList = $(currentFieldControl).data("kendoDropDownList");
                            if (dropDownList) {
                                dropDownList.filterInput.removeAttr("disabled").removeAttr("readonly");
                            }
                        });


                    }
                } catch (e) {

                }

                $("input[aria-owns*='" + field.fieldPattern + "'],input[k-ng-model*='" + field.fieldPattern + "'],input[ng-model*='" + field.fieldPattern + "'],textarea[ng-model*='" + field.fieldPattern + "'],select[k-ng-model*='" + field.fieldPattern + "'],select[data-k-ng-model*='" + field.fieldPattern + "'],select[ng-model*='" + field.fieldPattern + "']").removeAttr("disabled").removeAttr("readonly");
            } else if (field.fieldPattern !== "departmentName") {
                $("input[aria-owns*='" + field.fieldPattern + "'],input[k-ng-model*='" + field.fieldPattern + "'],input[ng-model*='" + field.fieldPattern + "'],textarea[ng-model*='" + field.fieldPattern + "'],select[k-ng-model*='" + field.fieldPattern + "'],select[data-k-ng-model*='" + field.fieldPattern + "'],select[ng-model*='" + field.fieldPattern + "']").removeAttr("disabled").removeAttr("readonly");
            }
        }
    }
}

function isRestrictedPropertiesEmpty(fields) {
    var errorFields = [];
    if (fields) {
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            if (field.isRequired) {
                var isChecked = false;
                var eles = $("input[k-ng-model*='" + field.fieldPattern + "'],input[ng-model*='" + field.fieldPattern + "'],select[k-ng-model*='" + field.fieldPattern + "'],select[ng-model*='" + field.fieldPattern + "']");
                for (var j = 0; j < eles.length; j++) {
                    if (!$(eles[j]).val()) {
                        errorFields.push(field.name);
                        break;
                    }
                    if (eles[j].type === "radio") {
                        if (eles[j].checked) {
                            isChecked = true;
                            break;
                        }
                        else {
                            if (j == eles.length - 1 && !isChecked) {
                                errorFields.push(field.name);
                            }
                        }
                    }
                }
            }
        }
    }
    return errorFields;
}


function disableElement(disabled) {
    if (disabled) {
        $("a:contains('Delete'),button:contains('Add more')").hide();
        $("a:contains('Xóa'),button:contains('Thêm mới')").hide();
        $("input:not(.input-default),select:not(.input-default),textarea:not(.input-default),a:contains('Delete'),button:contains('Add more')").attr("disabled", "disabled");
        $("input:not(.input-default),select:not(.input-default),textarea:not(.input-default),a:contains('Xóa'),button:contains('Thêm mới')").attr("disabled", "disabled");
    } else {
        $("a:contains('Delete'),button:contains('Add more')").show();
        $("a:contains('Xóa'),button:contains('Thêm mới')").show();
        $("input:not(.input-default),select:not(.input-default),textarea:not(.input-default),a:contains('Delete'),button:contains('Add more')").removeAttr("disabled");
        $("input:not(.input-default),select:not(.input-default),textarea:not(.input-default),a:contains('Xóa'),button:contains('Thêm mới')").removeAttr("disabled");
    }
    $(".grid-enable-select select").removeAttr("disabled");
}

function enableElementWithRoleITHelpdesk() {
    $("input.it-helpdesk,textarea.it-helpdesk").removeAttr("disabled").removeAttr("readonly");
    $('[aria-owns="newDepartmentId_listbox"]').removeAttr("disabled").removeAttr("readonly");
    $('[aria-owns="newUserId_listbox"]').removeAttr("disabled").removeAttr("readonly");
    $('[aria-owns="newDepartmentGroupId_listbox"]').removeAttr("disabled").removeAttr("readonly");
}

function enableElementToEdit() {
    $('.enable-to-edit').removeAttr("disabled");
}

function showCustomField(model, fields) {
    var items = [];
    fields.forEach(x => {
        if (model[x]) {
            items.push(model[x]);
        }
    });
    // if (endField) {
    //     return items.join(' - ') + `(${model[endField]})`;
    // }
    return items.join(' - ');
}
function showCustomCoupleLanguage(e) {
    if (e.name) {
        let showText = e.name;
        if (e.nameVN) {
            showText = showText + `(${e.nameVN})`;
        }
        return showText;
    }
}
 
function generatePredicateWithWorkLocationName(option, WorkLocationName, workLocationColum = 'WorkLocationName') {
    
    var predicateWorkLocationName = []; 
    var predicateParameters = option.predicateParameters; 

    var indexParameter = option.predicateParameters.length;
     
    for (var wl of WorkLocationName) {
        predicateWorkLocationName.push(`(${workLocationColum} = @${indexParameter})`);
        predicateParameters.push(wl);
        indexParameter ++;
    }

    if (predicateWorkLocationName.length) {
        predicateWorkLocationName = predicateWorkLocationName.join(" or ");
        if (option.predicate != "") {
            option.predicate = `${option.predicate} and (${predicateWorkLocationName})`;
        }
        else {
            option.predicate = `(${predicateWorkLocationName})`; 
        }
        option.predicateParameters = predicateParameters; 
    }  
}

function generatePredicateWithWorkLocationName(option, WorkLocationName, workLocationColum = 'WorkLocationName') {

    var predicateWorkLocationName = [];
    var predicateParameters = option.predicateParameters;

    var indexParameter = option.predicateParameters.length;

    for (var wl of WorkLocationName) {
        predicateWorkLocationName.push(`(${workLocationColum} = @${indexParameter})`);
        predicateParameters.push(wl);
        indexParameter++;
    }

    if (predicateWorkLocationName.length) {
        predicateWorkLocationName = predicateWorkLocationName.join(" or ");
        if (option.predicate != "") {
            option.predicate = `${option.predicate} and (${predicateWorkLocationName})`;
        }
        else {
            option.predicate = `(${predicateWorkLocationName})`;
        }
        option.predicateParameters = predicateParameters;
    }
}

function generatePredicateWithStatus(option, statuses, statusColum = 'Status') {
    let ruleValidateInprogress = ["Draft", "Cancelled", "Rejected", "Completed", "Requested To Change", "Completed Changing", "Pending", "Out Of Period"];

    let predicateStatus = [];
    let predicateForInprogressStatus = [];
    let predicateParameters = option.predicateParameters;
    let resultPredicate = ''

    let notProgressStatuses = _.filter(statuses, x => { return x !== 'In Progress' });
    if (notProgressStatuses.length) {
        notProgressStatuses.forEach(x => {
            predicateStatus.push(`${statusColum} = @${predicateParameters.length}`);
            predicateParameters.push(x);
        });
        resultPredicate += `(${predicateStatus.join(' or ')})`;
    }
    let inprogressIndex = _.findIndex(statuses, x => { return x == 'In Progress' });
    if (inprogressIndex > -1) {
        ruleValidateInprogress.forEach(x => {
            predicateForInprogressStatus.push(`${statusColum}!=@${predicateParameters.length}`);
            predicateParameters.push(x);
        });
        resultPredicate = resultPredicate ? (resultPredicate + ' or ' + `(${predicateForInprogressStatus.join(' and ')})`) : predicateForInprogressStatus.join(' and ');
    }
    if (resultPredicate) {
        option.predicate = option.predicate ? `(${option.predicate}) and (${resultPredicate})` : resultPredicate;
        option.predicateParameters = predicateParameters;
    }
}

function buildArgForSearch(pageSize, query, columsToSearchOnKeyword, checkDepartment = true) {
    let option = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: pageSize,
        page: 1
    };
    if (query.keyword) {
        option.predicate = `(${columsToSearchOnKeyword.join(" or ")})`;
        for (let index = 0; index < columsToSearchOnKeyword.length; index++) {
            option.predicateParameters.push(query.keyword);
        }
    }
    if (query.departmentCode && checkDepartment) {
        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(deptCode = @${option.predicateParameters.length} or divisionCode = @${option.predicateParameters.length + 1})`;
        option.predicateParameters.push(query.departmentCode);
        option.predicateParameters.push(query.departmentCode);
    }
    addDateFieldFilter(query, option);
    if (query.status && query.status.length) {
        generatePredicateWithStatus(option, query.status);
    }
    if (query.leaveKind) {
        generatePredicateWithLeaveKind(option, query.leaveKind);
    }
    return option;
}
function addDateFieldFilter(query, option) {
    if (query.fromDate && isValidDate(query.fromDate)) {
        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `created >= @${option.predicateParameters.length}`;
        option.predicateParameters.push(moment(query.fromDate, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    }
    if (query.toDate && isValidDate(query.toDate)) {
        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `created < @${option.predicateParameters.length}`;
        option.predicateParameters.push(moment(query.toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
    }

    //LEAVE MANAGEMENT
    if (query.leaveDateFrom && isValidDate(query.leaveDateFrom)) {
        if (query.leaveDateTo && isValidDate(query.leaveDateTo)) {
            temp = `leaveApplicationDetails.Any(y =>(y.fromDate >= @${option.predicateParameters.length} && y.fromDate < @${option.predicateParameters.length + 1}) || (y.toDate >= @${option.predicateParameters.length} && y.toDate < @${option.predicateParameters.length + 1}) || (@${option.predicateParameters.length} >= y.fromDate && @${option.predicateParameters.length} < y.toDate) || (@${option.predicateParameters.length + 1} > y.fromDate && @${option.predicateParameters.length + 1} <= y.toDate))`;
            option.predicateParameters.push(moment(query.leaveDateFrom, 'DD/MM/YYYY').format('YYYY-MM-DD'));
            option.predicateParameters.push(moment(query.leaveDateTo, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
        } else {
            temp = `leaveApplicationDetails.Any(y => y.fromDate >= @${option.predicateParameters.length})`;
            option.predicateParameters.push(moment(query.leaveDateFrom, 'DD/MM/YYYY').format('YYYY-MM-DD'));
        }
        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + temp;
    } else {
        if (query.leaveDateTo && isValidDate(query.leaveDateTo)) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `leaveApplicationDetails.Any(y => y.toDate < @${option.predicateParameters.length})`;
            option.predicateParameters.push(moment(query.leaveDateTo, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
        }
    }

    if (query.fromStartingDate && isValidDate(query.fromStartingDate)) {
        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `startingDateRequire >= @${option.predicateParameters.length}`;
        option.predicateParameters.push(moment(query.fromStartingDate, 'DD/MM/YYYY').format('YYYY-MM-DD'));
    }
    if (query.toStartingDate && isValidDate(query.toStartingDate)) {
        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `startingDateRequire < @${option.predicateParameters.length}`;
        option.predicateParameters.push(moment(query.toStartingDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
    }
}
function generatePredicateWithLeaveKind(option, predicateParams) {
    let predicateLeaveKind = [];
    let predicateParameters = option.predicateParameters;
    let resultPredicate = ''
    if (predicateParams.length) {
        predicateParams.forEach(x => {
            predicateLeaveKind.push(`LeaveApplicationDetails.Any(LeaveCode = @${predicateParameters.length})`);
            predicateParameters.push(x);
        });
        resultPredicate += `(${predicateLeaveKind.join(' or ')})`;
    }

    if (resultPredicate) {
        option.predicate = option.predicate ? `(${option.predicate}) and (${resultPredicate})` : resultPredicate;
        option.predicateParameters = predicateParameters;
    }
}

// function visibleProcessingStages($translate) {
//     let dialogReason = $("#processingStates").kendoDialog({
//         // title: "Processing Stages",
//         title: $translate.instant('COMMON_PROCESSING_STAGE'),
//         width: "1000px",
//         modal: true,
//         visible: true,
//         animation: {
//             open: {
//                 effects: "fade:in"
//             }
//         }
//     });
//     let boxReason = dialogReason.data("kendoDialog");
//     boxReason.open();
// }

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}
const capitalize = (s) => {
    if (typeof s !== 'string') return ''
    return s.replace(/(^\w{1})|(\s{1}\w{1})/g, match => match.toUpperCase());
}

//CR321=================
var isStore = null;
function CheckIsStore($scope) {
    try {
        isStore = $scope.$root.currentUser.isStore;
    } catch (e) {

    }
}

function getSubmittedDayByDepartment(setting) {
    var dayOfSubmitted = null;
    if (isStore) {
        dayOfSubmitted = setting["deadlineOfSubmittingCABStore"];
    }
    else if (!isStore) {
        dayOfSubmitted = setting["deadlineOfSubmittingCABHQ"];
    }
    return dayOfSubmitted;
}

function getSubmittedTimeByDepartment(setting) {
    var timeOfSubmitted = null;
    if (isStore) {
        timeOfSubmitted = setting["timeOfSubmittingCABStore"];
    }
    else if (!isStore) {
        timeOfSubmitted = setting["timeOfSubmittingCABHQ"];
    }
    return timeOfSubmitted;
}

//======================
function validateDateRange(setting, fromDate, toDate, rowNumber, subject, $translate, typeToCheckRange1, typeToCheckRange2) {
    let errors = [];
    let range = [setting["salaryPeriodFrom"], setting["salaryPeriodTo"]];
    let current = new Date();
    let submittedDate = new Date();
    var dayOfSubmitted = getSubmittedDayByDepartment(setting);
    var timeOfSubmitted = getSubmittedTimeByDepartment(setting);
    let submittedDay = dayOfSubmitted != null ? dayOfSubmitted : setting["deadlineOfSubmittingCABApplication"];
    var submittedTime = timeOfSubmitted != null ? timeOfSubmitted : 0;
    // validate submited Date 
    var salaryRange = getSalaryDateRange(current, range);
    if (fromDate && dateFns.isAfter(new Date(dateFns.format(salaryRange.start, 'MM/DD/YYYY hh:mm:ss a')), new Date(dateFns.format(fromDate, 'MM/DD/YYYY hh:mm:ss a')))) {
        let fromSalaryDate = getSalaryDateRange(fromDate, range);
        if (dateFns.isAfter(new Date(dateFns.format(salaryRange.start, 'MM/DD/YYYY hh:mm:ss a')), new Date(dateFns.format(fromSalaryDate.start, 'MM/DD/YYYY hh:mm:ss a')))) {
            if (submittedDay >= fromSalaryDate.end.getDate()) {
                submittedDate = dateFns.setHours(dateFns.setDate(fromSalaryDate.end, submittedDay), submittedTime);
            } else {
                submittedDate = dateFns.setHours(dateFns.setDate(dateFns.addMonths(fromSalaryDate.end, 1), submittedDay), submittedTime);
            }
        } else {
            if (submittedDay >= salaryRange.end.getDate()) {
                submittedDate = dateFns.setHours(dateFns.setDate(fromSalaryDate.end, submittedDay), submittedTime);
            } else {
                submittedDate = dateFns.setHours(dateFns.setDate(dateFns.addMonths(salaryRange.end, 1), submittedDay), submittedTime);
            }
        }
        if (dateFns.isAfter(new Date(dateFns.format(current, 'MM/DD/YYYY hh:mm:ss a')), new Date(dateFns.format(submittedDate, 'MM/DD/YYYY hh:mm:ss a')))) {
            /*var datePeriodFrom = dateFns.addMonths(salaryRange.start, -1);
            var datePeriodTo = dateFns.addMonths(salaryRange.end, -1);
            errors.push({ controlName: $translate.instant(subject) + ' ' + $translate.instant('COMMON_ROW_NO') + ` ${rowNumber}`, message: $translate.instant(typeToCheckRange1) + ` ` + moment(datePeriodFrom).format('DD/MM/YYYY') + ` ` + $translate.instant('COMMON_TO') + ` ` + moment(datePeriodTo).format('DD/MM/YYYY') });*/

            if (current.getDate() < submittedDay && current.getMonth() === salaryRange.start.getMonth()) {
                var datePeriodFrom = dateFns.addMonths(salaryRange.start, -1);
                var datePeriodTo = dateFns.addMonths(salaryRange.end, -1);
                errors.push({ controlName: $translate.instant(subject) + ' ' + $translate.instant('COMMON_ROW_NO') + ` ${rowNumber}`, message: $translate.instant(typeToCheckRange1) + ` ` + moment(datePeriodFrom).format('DD/MM/YYYY') + ` ` + $translate.instant('COMMON_TO') + ` ` + moment(datePeriodTo).format('DD/MM/YYYY') });
            }
            else {
                errors.push({ controlName: $translate.instant(subject) + ' ' + $translate.instant('COMMON_ROW_NO') + ` ${rowNumber}`, message: $translate.instant(typeToCheckRange1) + ` ` + moment(salaryRange.start).format('DD/MM/YYYY') + ` ` + $translate.instant('COMMON_TO') + ` ` + moment(salaryRange.end).format('DD/MM/YYYY') });
            }
        }
    }
    if (!errors.length && fromDate && toDate) {
        // fromDate và toDate phải cùng thuộc 1 chu kì
        //* xác định chu kì lương của from       
        // let fromSalaryDate = getSalaryDateRange(fromDate, range);
        // let toSalaryDate = getSalaryDateRange(toDate, range);
        // if (dateFns.format(fromSalaryDate.start, 'MM/DD/YYYY') != dateFns.format(toSalaryDate.start, 'MM/DD/YYYY')
        // ) {
        //     errors.push({ controlName: $translate.instant(subject) + ' ' + $translate.instant('COMMON_ROW_NO') + ` ${rowNumber}`, message: $translate.instant(typeToCheckRange2) });
        // }
        //* xác định chi kì lương của to

    }
    return errors;
}
function validateDateRange_RevokeDeadline(setting) {
    let errors = [];
    let range = [setting["salaryPeriodFrom"], setting["salaryPeriodTo"]];
    let current = new Date();
    let RevokeDate = new Date();
    var dayOfSubmitted = getSubmittedDayByDepartment(setting);
    var timeOfSubmitted = getSubmittedTimeByDepartment(setting);
    let submittedDay = dayOfSubmitted != null ? dayOfSubmitted : setting["deadlineOfSubmittingCABApplication"];
    var submittedTime = timeOfSubmitted != null ? timeOfSubmitted : 0;
    // validate submited Date
    var salaryRange = getSalaryDateRange(current, range);
    if (submittedDay >= salaryRange.end.getDate()) {
        RevokeDate = dateFns.setHours(dateFns.setDate(salaryRange.end, submittedDay), submittedTime);
    } else {
        RevokeDate = dateFns.setHours(dateFns.setDate(dateFns.addMonths(salaryRange.end, 1), submittedDay), submittedTime);
    }
    if (dateFns.isAfter(new Date(dateFns.format(current, 'MM/DD/YYYY hh:mm:ss a')), new Date(dateFns.format(RevokeDate, 'MM/DD/YYYY hh:mm:ss a')))) {
        errors.push({ message: 'Could not revoke when the period is closed.' });
    }

    return errors;
}
function validateDateRange_PeriodClosed(setting, $scope) {
    let errors = [];
    let current = new Date();
    let range = [setting["salaryPeriodFrom"], setting["salaryPeriodTo"]];
    var dayOfSubmitted = getSubmittedDayByDepartment(setting);
    var timeOfSubmitted = getSubmittedTimeByDepartment(setting);
    let submittedDay = dayOfSubmitted != null ? dayOfSubmitted : setting["deadlineOfSubmittingCABApplication"];
    var submittedTime = timeOfSubmitted != null ? timeOfSubmitted : 0;

    //CR- Khiem - Revoke for CB modules
    switch ($scope.model.moduleTitle) {
        case "Shift Exchange":
            if ($scope.model.shiftExchangeItemsData.length) {
                for (var i = 0; i < $scope.model.shiftExchangeItemsData.length; i++) {
                    let applicableDate = new Date(dateFns.format($scope.model.shiftExchangeItemsData[i].shiftExchangeDate, 'MM/DD/YYYY hh:mm:ss a'));
                    var salaryRange_Current = getSalaryDateRange(current, range);
                    var salaryRange_applicableDate = getSalaryDateRange(applicableDate, range);
                    let submittedDate_applicableDatePeriod = dateFns.setHours(new Date(salaryRange_applicableDate.end.getFullYear(), salaryRange_applicableDate.end.getMonth(), submittedDay), submittedTime);

                    if (dateFns.isAfter(new Date(dateFns.format(salaryRange_Current.start, 'MM/DD/YYYY hh:mm:ss a')),
                        new Date(dateFns.format(salaryRange_applicableDate.end, 'MM/DD/YYYY hh:mm:ss a')))) {

                        if (dateFns.isAfter(new Date(dateFns.format(new Date(), 'MM/DD/YYYY hh:mm:ss a')),
                            new Date(dateFns.format(submittedDate_applicableDatePeriod, 'MM/DD/YYYY hh:mm:ss a')))) {
                            errors.push({ message: 'Could not revoke when the period is closed.' });
                            break;
                        }
                    }
                }
            }
            break;

        case "Overtime":
            if ($scope.model.overtimeDetailItems.length) {
                for (var i = 0; i < $scope.model.overtimeDetailItems.length; i++) {
                    let applicableDate = new Date(dateFns.format($scope.model.overtimeDetailItems[i].date, 'MM/DD/YYYY hh:mm:ss a'));
                    var salaryRange_Current = getSalaryDateRange(current, range);
                    var salaryRange_applicableDate = getSalaryDateRange(applicableDate, range);
                    let submittedDate_applicableDatePeriod = dateFns.setHours(new Date(salaryRange_applicableDate.end.getFullYear(), salaryRange_applicableDate.end.getMonth(), submittedDay), submittedTime);

                    if (dateFns.isAfter(new Date(dateFns.format(salaryRange_Current.start, 'MM/DD/YYYY hh:mm:ss a')),
                        new Date(dateFns.format(salaryRange_applicableDate.end, 'MM/DD/YYYY hh:mm:ss a')))) {

                        if (dateFns.isAfter(new Date(dateFns.format(new Date(), 'MM/DD/YYYY hh:mm:ss a')),
                            new Date(dateFns.format(submittedDate_applicableDatePeriod, 'MM/DD/YYYY hh:mm:ss a')))) {
                            errors.push({ message: 'Could not revoke when the period is closed.' });
                            break;
                        }
                    }
                }
            }
            break;

        case "Leave Management":
            if ($scope.model.leaveApplicantDetails.length) {
                for (var i = 0; i < $scope.model.leaveApplicantDetails.length; i++) {
                    let applicableDate = new Date(dateFns.format($scope.model.leaveApplicantDetails[i].toDate, 'MM/DD/YYYY hh:mm:ss a'));
                    var salaryRange_Current = getSalaryDateRange(current, range);
                    var salaryRange_applicableDate = getSalaryDateRange(applicableDate, range);
                    let submittedDate_applicableDatePeriod = dateFns.setHours(new Date(salaryRange_applicableDate.end.getFullYear(), salaryRange_applicableDate.end.getMonth(), submittedDay), submittedTime);

                    if (dateFns.isAfter(new Date(dateFns.format(salaryRange_Current.start, 'MM/DD/YYYY hh:mm:ss a')),
                        new Date(dateFns.format(salaryRange_applicableDate.end, 'MM/DD/YYYY hh:mm:ss a')))) {

                        if (dateFns.isAfter(new Date(dateFns.format(new Date(), 'MM/DD/YYYY hh:mm:ss a')),
                            new Date(dateFns.format(submittedDate_applicableDatePeriod, 'MM/DD/YYYY hh:mm:ss a')))) {
                            errors.push({ message: 'Could not revoke when the period is closed.' });
                            break;
                        }
                    }
                }
            }
            break;

        case "Missing Time Lock":
            if ($scope.model.listReason.length) {
                for (var i = 0; i < $scope.model.listReason.length; i++) {
                    let missingTime = new Date(dateFns.format($scope.model.listReason[i].date, 'MM/DD/YYYY hh:mm:ss a'));
                    var salaryRange_Current = getSalaryDateRange(current, range);
                    var salaryRange_applicableDate = getSalaryDateRange(missingTime, range);
                    let submittedDate_applicableDatePeriod = dateFns.setHours(new Date(salaryRange_applicableDate.end.getFullYear(), salaryRange_applicableDate.end.getMonth(), submittedDay), submittedTime);

                    if (dateFns.isAfter(new Date(dateFns.format(salaryRange_Current.start, 'MM/DD/YYYY hh:mm:ss a')),
                        new Date(dateFns.format(salaryRange_applicableDate.end, 'MM/DD/YYYY hh:mm:ss a')))) {

                        if (dateFns.isAfter(new Date(dateFns.format(new Date(), 'MM/DD/YYYY hh:mm:ss a')),
                            new Date(dateFns.format(submittedDate_applicableDatePeriod, 'MM/DD/YYYY hh:mm:ss a')))) {
                            errors.push({ message: 'Could not revoke when the period is closed.' });
                            break;
                        }
                    }
                }
            }
            break;
        default:
            let createdDate = new Date(dateFns.format($scope.model.created, 'MM/DD/YYYY hh:mm:ss a'));
            var salaryRange_Current = getSalaryDateRange(current, range);
            var salaryRange_Created = getSalaryDateRange(createdDate, range);
            let submittedDate_CreatedDatePeriod = dateFns.setHours(new Date(salaryRange_Created.end.getFullYear(), salaryRange_Created.end.getMonth(), submittedDay), submittedTime);

            if (dateFns.isAfter(new Date(dateFns.format(salaryRange_Current.start, 'MM/DD/YYYY hh:mm:ss a')),
                new Date(dateFns.format(salaryRange_Created.end, 'MM/DD/YYYY hh:mm:ss a')))) {

                if (dateFns.isAfter(new Date(dateFns.format(new Date(), 'MM/DD/YYYY hh:mm:ss a')),
                    new Date(dateFns.format(submittedDate_CreatedDatePeriod, 'MM/DD/YYYY hh:mm:ss a')))) {
                    errors.push({ message: 'Could not revoke when the period is closed.' });
                }
            }
            break;
    }
    return errors;
}
function getSalaryDateRange(date, range) {
    // [1,25] -> [1/9/2020 - 25/9/2020] 
    // [2/25] -> [2/8/2020 - 25/9/2020]

    // now = 26/9/2020 => 
    let startDateRange = new Date();
    let toDateRange = new Date();
    // Create Period
    if (date && date.getDate() > range[1]) {
        if (range[0] == 1) {
            startDateRange = dateFns.setDate(dateFns.addMonths(date, 1), range[0]);
        } else {
            startDateRange = dateFns.setDate(date, range[0]);
        }
        toDateRange = dateFns.setDate(dateFns.addMonths(date, 1), range[1]);
    } else {
        // startDateRange = dateFns.setDate(dateFns.addMonths(date, -1), range[0]);
        if (range[0] == 1) {
            startDateRange = dateFns.setDate(date, range[0]);
        } else {
            startDateRange = dateFns.setDate(dateFns.addMonths(date, -1), range[0]);
        }
        toDateRange = dateFns.setDate(date, range[1])
    }
    console.log({ start: startDateRange, end: toDateRange })
    return { start: startDateRange, end: toDateRange };
}
function validateSalaryRangeDate($translate, items, range, subject, type, isHasToDate) {
    let errors = [];
    let minDate = new Date();
    let maxDate = new Date();
    let minDateItem = {};
    let maxDateItem = {};
    if (isHasToDate) {
        minDateItem = _.minBy(items, x => { return x.fromDate });
        maxDateItem = _.maxBy(items, x => { return x.toDate });
        minDate = minDateItem.fromDate;
        maxDate = maxDateItem.toDate;

    } else {
        minDateItem = _.minBy(items, x => { return x.date });
        maxDateItem = _.maxBy(items, x => { return x.date });
        minDate = minDateItem.date;
        maxDate = maxDateItem.date;
    }

    // let fromSalaryDate = getSalaryDateRange(minDate, range);
    // let toSalaryDate = getSalaryDateRange(maxDate, range);
    // if (dateFns.format(fromSalaryDate.start, 'MM/DD/YYYY') != dateFns.format(toSalaryDate.start, 'MM/DD/YYYY')
    // ) {
    //     errors.push({ controlName: $translate.instant(subject) + ': ', message: $translate.instant(type) });
    // }
    return errors;
}

function showCustomDepartmentTitle(e) {
    let model = e.item;
    if (model.userCheckedHeadCount) {
        //return `${model.code} - ${model.name} - ${model.userCheckedHeadCount}(${model.jobGradeCaption})`
        return `${model.code} - ${model.name} - ${model.userCheckedHeadCount}`
    } else {
        // return `${model.code} - ${model.name} (${model.jobGradeCaption})`;
        return `${model.code} - ${model.name}`;
    }
}

function translateStatus($translate, array) {
    let result = [];
    for (var i = 0; i < array.length; i++) {
        array[i].name = $translate.instant(array[i].name);
        result.push(array[i]);
    }
    return result;
}
function clearSearchTextOnDropdownTree(id) {
    var dropdownlist = $(`#${id}`).data("kendoDropDownTree");
    dropdownlist.filterInput[0].value = '';
}
function clearSearchTextOnDropdownList(id) {
    var dropdownlist = $(`#${id}`).data("kendoDropDownList");
    dropdownlist.filterInput[0].value = '';
}
function clearGrid(id) {
    $(`#${id}`).data('kendoGrid').dataSource.data([]);
}
function setDropDownTree(id, data, value = null) {
    var dataSource = new kendo.data.HierarchicalDataSource({
        data: data,
        schema: {
            model: {
                children: "items"
            }
        }
    });
    var dropdownTree = $(`#${id}`).data("kendoDropDownTree");
    if (dropdownTree) {
        dropdownTree.setDataSource(dataSource);
        if (value) {
            dropdownTree.value(value);
        }
    }
}
function setDataSourceForGrid(id, data) {
    let grid = $(id).data("kendoGrid");
    if (grid) {
        let dataSource = new kendo.data.DataSource({
            data: data
        });
        grid.setDataSource(dataSource);
    }
}
function setDataSourceForDropdownList(id, data) {
    let ctr = $(id).data("kendoDropDownList");
    ctr.setDataSource(data);
}
function readDataSource(id) {
    let grid = $(id).data("kendoGrid");
    grid.dataSource.read();
}
function refreshGrid(id) {
    let grid = $(`#${id}`).data("kendoGrid");
    if (grid) {
        grid.refresh();
    }
}
function setDataDropdownList(id, data, value) {
    var dataSource = new kendo.data.DataSource({
        data: data
    });
    var dropdownlist = $(id).data("kendoDropDownList");
    if (dropdownlist) {
        dropdownlist.setDataSource(dataSource);
        dropdownlist.value(value);
    }
}
function phoneNumberInput(keyCode) {
    if (keyCode != 67 && keyCode != 80 && keyCode != 88 && keyCode != 65 && keyCode != 32 && keyCode != 37 && keyCode != 39 &&
        ((keyCode != 8 && 48 > keyCode) || (96 > keyCode && keyCode > 57) || keyCode > 105)) {
        return false
    } else {
        return true;
    }
}
function isValidDate(date) {
    return date.includes('day') == false && date.includes('month') == false && date.includes('year') == false;
}
function clearDropDownList(ids) {
    if (ids && ids.length) {
        _.forEach(ids, x => {
            var dropdownlistItems = $(`${x}`).data("kendoDropDownList");
            if (dropdownlistItems) {
                dropdownlistItems.value("");
            }
        })
    }
}
function checkIsAfterDate(date1, date2) {
    return dateFns.isAfter(new Date(dateFns.format(date1, 'MM/DD/YYYY')), new Date(dateFns.format(date2, 'MM/DD/YYYY')));
}
function checkIsBeforeDate(date1, date2) {
    return !dateFns.isAfter(new Date(dateFns.format(date1, 'MM/DD/YYYY')), new Date(dateFns.format(date2, 'MM/DD/YYYY')));
}
Array.prototype.insert = function (index) {
    this.splice.apply(this, [index, 0].concat(
        Array.prototype.slice.call(arguments, 1)));
    return this;
};

async function mergeAttachmentV2(attachmentService, oldAttachments, attachments) {
    try {
        let uploadResult = await uploadAction(attachmentService, attachments);
        let attachmentFilesJson = '';
        let allAttachments = oldAttachments && oldAttachments.length ? oldAttachments.map(({
            id,
            fileDisplayName
        }) => ({
            id,
            fileDisplayName
        })) : [];
        if (uploadResult.isSuccess) {
            let attachmentFiles = uploadResult.object.map(({
                id,
                fileDisplayName
            }) => ({
                id,
                fileDisplayName
            }));
            allAttachments = allAttachments.concat(attachmentFiles);
        }
        attachmentFilesJson = JSON.stringify(allAttachments);
        return attachmentFilesJson;
    } catch (e) {
        console.log(e);
        return '';
    }

    async function uploadAction(attachmentService, attachments) {
        var payload = new FormData();
        attachments.forEach(item => {
            payload.append('file', item.rawFile, item.name);
        });
        let result = await attachmentService.getInstance().attachmentFile.upload(payload).$promise;
        return result;
    }

    function getDates(startDate, stopDate) {
        var dateArray = [];
        var currentDate = moment(startDate);
        var stopDate = moment(stopDate);
        while (currentDate <= stopDate) {
            dateArray.push(moment(currentDate).format('YYYYMMDD'))
            currentDate = moment(currentDate).add(1, 'days');
        }
        return dateArray;
    }
}
