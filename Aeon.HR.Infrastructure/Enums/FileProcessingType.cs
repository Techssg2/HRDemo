using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum FileProcessingType
    {
        [Description("Request To Hire")]
        REQUESTOHIRE = 1,
        [Description("Position")]
        POSITION = 2,
        [Description("Applicant")]
        APPLICANT = 3,
        [Description("New Staff Onboard")]
        NEWSTAFFONBOARD = 4,
        [Description("Handover")]
        HANDOVER = 5,
        [Description("Promote And Transfer")]
        PROMOTEANDTRANSFER = 6,
        [Description("Acting")]
        ACTING = 7,
        [Description("Leave Application")]
        LEAVEMANAGEMENT = 8,
        [Description("Missing Time Clock")]
        MISSINGTIMECLOCK = 9,
        [Description("Overtime")]
        OVERTIME = 10,
        [Description("Shift Exchange")]
        SHIFTEXCHANGE = 11,
        [Description("Resignation")]
        RESIGNATION = 12,
        [Description("Department")]
        DEPARTMENT = 13,
        [Description("Headcount")]
        HEADCOUNT = 14,
        [Description("Missing time clock reason")]
        MISSINGTIMECLOCKREASON = 15,
        [Description("Overtime Reason")]
        OVERTIMEREASON = 16,
        [Description("ShiftExchange Reason")]
        SHIFTEXCHANGEREASON = 17,
        [Description("Resignation Reason")]
        RESIGNATIONREASON = 18,
        [Description("Users")]
        USER = 19,
        [Description("Tracking Log")]
        TRACKINGLOG = 20,
        [Description("Working time Recruitment")]
        WORKINGTIMERECRUITMENT = 21,
        [Description("Item List Recruitment")]
        ITEMLISTRECRUITMENT = 22,
        [Description("Applicant Status List Recruitment")]
        APPLICANTSTATUSLISTRECRUITMENT = 23,
        [Description("Appreciation List Recruitment")]
        APPRECIATIONLISTRECRUITMENT = 24,
        [Description("Position List Recruitment")]
        POSITIONLISTRECRUITMENT = 25,
        [Description("JobGrade List")]
        JOBGRADE = 26,
        [Description("Position Detail")]
        POSITIONDETAIL = 27,
        [Description("Cost Center Recruitment")]
        COSTCENTERRECRUITMENT = 28,           
        [Description("Hotel Setting")]
        HOTELSETTING = 29,
        [Description("Business Trip Location")]
        BUSINESSTRIPLOCATIONSETTING = 30,
        [Description("Flight Number Setting")]
        FLIGHTNUMBERSETTING = 31,
        [Description("Air Line Setting")]
        AIRLINESETTING = 32,
        [Description("Room Type")]
        ROOMTYPESETTING = 33,
        [Description("Business Trip Application Report")]
        BUSINESSTRIPAPPLICATIONREPORT = 34,
        [Description("Business Trip Application")]
        BUSINESSTRIPAPPLICATIONEXPORT = 35,
        [Description("Working Address")]
        WORKINGADDRESSRECRUITMENT = 36,
        [Description("Holiday Schedule")]
        HOLIDAYSCHEDULE = 37,
        [Description("Shift Plan Submit Person")]
        SHIFTPLANSUBMITPERSON = 38,
        [Description("Target Plan")]
        TARGETPLAN = 40,
        [Description("OVERTIME FILL ACTUAL DETAILS")]
        OVERTIME_FILL_ACTUAL_DETAILS = 41,
        [Description("MAINTAIN PROMOTE AND TRANFER PRINT")]
        MAINTAINPROMOTEANDTRANFERPRINT = 42,
        [Description("BTAPOLICYSPECIALCASES")]
        BTAPOLICYSPECIALCASES = 43,
        [Description("FLIGHTSBOOKING")]
        FLIGHTSBOOKING = 44,
        [Description("OVERBUDGET")]
        OVERBUDGET = 45,
        [Description("BUSINESSMODEL")]
        BUSINESSMODEL = 46,
        [Description("BUSINESSMODELUNITMAPPING")]
        BUSINESSMODELUNITMAPPING = 47,
        [Description("SHIFTCODE")]
        SHIFTCODE = 48,
        [Description("TARGETPLANSPECIALCASE")]
        TARGETPLANSPECIALCASE = 49,
        [Description("BTAERRORMESSAGE")]
        BTAERRORMESSAGE = 50
    }
}