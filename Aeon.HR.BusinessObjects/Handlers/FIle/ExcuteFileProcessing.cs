using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.BusinessObjects.Interfaces;
using System;
using System.Threading.Tasks;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Handlers.FIle;
using System.IO;

namespace Aeon.HR.BusinessObjects.Handlers.File
{
    public class ExcuteFileProcessing : IExcuteFileProcessing
    {     
        private readonly IUnitOfWork _uow;
        public ExcuteFileProcessing(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IFileProcessing CreatedFileProcessing(FileProcessingType type)
        {
            switch (type)
            {
                case FileProcessingType.REQUESTOHIRE: // đây là BO mẫu, sau sử dụng thì xóa
                    return new RequestToHireProcessingBO(_uow);
                //setting - recruitment 
                case FileProcessingType.WORKINGTIMERECRUITMENT:
                    return new WorkingTimeRecruitmentProcessingBO(_uow);
                case FileProcessingType.ITEMLISTRECRUITMENT:
                    return new ItemListRecruitmentProcessingBO(_uow);
                case FileProcessingType.APPLICANTSTATUSLISTRECRUITMENT:
                    return new ApplicantStatusListRecruitmentProcessingBO(_uow);
                case FileProcessingType.APPRECIATIONLISTRECRUITMENT:
                    return new AppreciaListRecruitmentProcessingBO(_uow);
                case FileProcessingType.POSITIONLISTRECRUITMENT:
                    return new PositionListRecruitmentProcessingBO(_uow);
                case FileProcessingType.HEADCOUNT:
                    return new HeadcountProcessingBO(_uow);
                case FileProcessingType.DEPARTMENT:
                    return new DepartmentProcessingBO(_uow);
                case FileProcessingType.JOBGRADE:
                    return new JobGradeProcessingBO(_uow);
                case FileProcessingType.MISSINGTIMECLOCKREASON:
                    return new CAndBProcessingBO(_uow);
                case FileProcessingType.OVERTIMEREASON:
                    return new CAndBProcessingBO(_uow);
                case FileProcessingType.SHIFTEXCHANGEREASON:
                    return new CAndBProcessingBO(_uow);
                case FileProcessingType.RESIGNATIONREASON:
                    return new CAndBProcessingBO(_uow);
                case FileProcessingType.HOLIDAYSCHEDULE:
                    return new HolidayScheduleProcessingBO(_uow);
                case FileProcessingType.TRACKINGLOG:
                    return new TrackingLogProcessingBO(_uow);
                case FileProcessingType.USER:
                    return new UserProcessingBO(_uow); 
                case FileProcessingType.MISSINGTIMECLOCK:
                    return new MissingTimeClockProcessingBO(_uow);
                case FileProcessingType.OVERTIME:
                    return new OverTimeProcessingBO(_uow);
                case FileProcessingType.SHIFTEXCHANGE:
                    return new ShiftExchangeApplicationProcessingBO(_uow);
                case FileProcessingType.RESIGNATION:
                    return new ResignationApplicationProcessingBO(_uow);
                case FileProcessingType.LEAVEMANAGEMENT:
                    return new LeaveApplicationProcessingBO(_uow);
                //recruitment
                case FileProcessingType.HANDOVER:
                    return new HandoverProcessingBO(_uow);
                case FileProcessingType.PROMOTEANDTRANSFER:
                    return new PromoteAndTransferProcessingBO(_uow);
                case FileProcessingType.ACTING:
                    return new ActingProcessingBO(_uow);
                case FileProcessingType.POSITION:
                    return new PositionProcessingBO(_uow);
                case FileProcessingType.APPLICANT:
                    return new ApplicantProcessingBO(_uow);
                case FileProcessingType.NEWSTAFFONBOARD:
                    return new NewStaffOnBoardProcessingBO(_uow);
                case FileProcessingType.POSITIONDETAIL:
                    return new NewStaffOnBoardProcessingBO(_uow);
                case FileProcessingType.COSTCENTERRECRUITMENT:
                    return new CostCenterRecruitmentProcessingBO(_uow);
                case FileProcessingType.HOTELSETTING:
                    return new HotelSettingProcessingBO(_uow);
                case FileProcessingType.BUSINESSTRIPLOCATIONSETTING:
                    return new BusinessTripLocationProcessingBO(_uow);
                case FileProcessingType.AIRLINESETTING:
                    return new AirlineSettingProcessingBO(_uow);
                case FileProcessingType.FLIGHTNUMBERSETTING:
                    return new FlightNumberSettingProcessingBO(_uow);
                case FileProcessingType.ROOMTYPESETTING:
                    return new RoomTypeProcessingBO(_uow);
                case FileProcessingType.BUSINESSTRIPAPPLICATIONEXPORT:
                    return new BusinessTripApplicationProcessingBO(_uow);
                case FileProcessingType.WORKINGADDRESSRECRUITMENT:
                    return new WorkingAddressRecruitmentProcessingBO(_uow);
                case FileProcessingType.MAINTAINPROMOTEANDTRANFERPRINT:
                    return new MaintainPromoteAndTranferPrintProcessingBO(_uow);
                case FileProcessingType.SHIFTPLANSUBMITPERSON:
                    return new ShiftPlanSubmitPersonProcessingBO(_uow);
                case FileProcessingType.TARGETPLAN:
                    return new TargetPlanProcessingBO(_uow);
                case FileProcessingType.OVERTIME_FILL_ACTUAL_DETAILS:
                    return new OverTimeFillActualProcessingBO(_uow);
                case FileProcessingType.BTAPOLICYSPECIALCASES:
                    return new BTAPolicySpecialCasesProcessingBO(_uow);
                case FileProcessingType.FLIGHTSBOOKING:
                    return new FilghtsBookingProcessingBO(_uow);
                // Over budget
                case FileProcessingType.OVERBUDGET:
                    return new OverBudgetProcessingBO(_uow);
                case FileProcessingType.BUSINESSMODEL:
                    return new BusinessModelProcessingBO(_uow);
                case FileProcessingType.BUSINESSMODELUNITMAPPING:
                    return new BusinessModelProcessingUnitMappingBO(_uow);
                case FileProcessingType.SHIFTCODE:
                    return new ShiftCodeProcessingBO(_uow);
                case FileProcessingType.TARGETPLANSPECIALCASE:
                    return new TargetPlanSpecialProcessingBO(_uow);
                case FileProcessingType.BTAERRORMESSAGE:
                    return new BTAErrorMessageProcessingBO(_uow);
            }
            return null;
        }

        public Task<ResultDTO> ExportAsync(FileProcessingType type, QueryArgs parameters)
        {
            var fileProcessing = CreatedFileProcessing(type);
            return fileProcessing.ExportAsync(parameters);            
        }     

        public async Task<ResultDTO> ImportAsync(FileProcessingType type, FileStream fileStream)
        {
            var fileProcessing = CreatedFileProcessing(type);
            return await fileProcessing.ImportAsync(fileStream);
        }
    }
}