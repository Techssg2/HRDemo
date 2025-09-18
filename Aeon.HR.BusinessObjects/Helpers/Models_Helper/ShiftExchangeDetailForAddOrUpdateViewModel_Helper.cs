using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.Args;
using System;
using System.Collections.Generic;
using Aeon.HR.ViewModels;
using System.Linq;

namespace Aeon.HR.BusinessObjects
{
	#region OvertimeApplicationDetailArgs_Helper
	public static class ShiftExchangeDetailForAddOrUpdateViewModel_Helper
	{
		public static CheckingResult Check_ShiftExchange_Valid(this ShiftExchangeDetailForAddOrUpdateViewModel sExchangeDetail, IUnitOfWork unitOfWork, bool isCreateShiftExchange, bool forceViewAll = false)
		{
			CheckingResult returnValue = new CheckingResult();
			try
			{
				if (sExchangeDetail != null)
				{
					//Check date is well-form
					DateTime seDate = sExchangeDetail.ShiftExchangeDate.Date;
					string strDateFormat = "dd/MM/yyyy";
					if (seDate == DateTime.MinValue)
					{
						returnValue.success = false;
						returnValue.errorCode = "COMMON_DATE_TIME_WRONG_FORMAT";
						returnValue.message = seDate.ToString(strDateFormat);
						return returnValue;
					}

					//Check User
					User user = sExchangeDetail.UserId.GetUserById(unitOfWork, true);
					//chi validate user inactive khi tao phieu
					if ((user is null || !user.IsActivated) && isCreateShiftExchange)
					{
						returnValue.success = false;
						returnValue.errorCode = "COMMON_USER_NOT_FOUND";
						returnValue.message = user.SAPCode;
						return returnValue;
					}

                    // HR-970 OT: Không cho đăng ký OT nếu chưa có target plan completed
                    bool hasTargetPlanCompleted = user.HasTargetPlanCompleted(unitOfWork, seDate, true);
                    if (!hasTargetPlanCompleted)
                    {
                        returnValue.success = false;
                        returnValue.errorCode = "NEED_TO_COMPLETED_TARGET_PLAN_FIRST";
                        returnValue.message = $"{user.FullName} - {user.SAPCode}";
                        return returnValue;
                    }

                    //Check exist target plan  //AEON_658
                    TargetPlan targetPlan = user.GetTargetPlanByDateForTargetPlan(unitOfWork, seDate);
					if (targetPlan is null)
					{
						returnValue.success = false;
						returnValue.errorCode = "SHIFT_EXCHANGE_NEED_TO_COMPLETED_TARGET_PLAN_FIRST";
						//returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString("MM/dd/yyyy")}";
						returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString(strDateFormat)}";
						return returnValue;
					}
					//if (targetPlan is null || !targetPlan.Status.Equals(Const.Status.completed))
					//{
					//    returnValue.success = false;
					//    returnValue.errorCode = "SHIFT_EXCHANGE_NEED_TO_COMPLETED_TARGET_PLAN_FIRST";
					//    returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString("MM/dd/yyyy")}";
					//    return returnValue;
					//}

					//Check exist another Shift exchange on the same day
					List<ShiftExchangeApplication> seApplications = user.GetProcessingShiftExchangesByDate(unitOfWork, seDate);
					if (!(seApplications is null))
					{
						////Remove current item
						seApplications = seApplications.Where(x => x != null && !x.Id.Equals(sExchangeDetail.ShiftExchangeApplicationId) && !x.Status.ToLower().Equals("completed")).ToList();
						if (seApplications.Any())
						{
							returnValue.success = false;
							returnValue.errorCode = "SHIFT_EXCHANGE_EXIST_ANOTHER_SHIFT_EXCHANGE_SAME_DAY";
							//returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString("MM/dd/yyyy")} - {seApplications.First().ReferenceNumber}";
							returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString(strDateFormat)} - {seApplications.First().ReferenceNumber}";
							return returnValue;
						}
					}

					//Check exist the leave application on the same day
					List<LeaveApplication> leaveApplications = user.GetLeaveApplicationByDate(unitOfWork, seDate, true);
					if (!(leaveApplications is null) && leaveApplications.Count() > 0)
					{
						if (sExchangeDetail.CurrentShiftCode.IsShiftOfTarget2())
						{
							//In case, current shift code is target 2. Check leave code on that day is target 2 or not?
							leaveApplications = leaveApplications.Where(x => x.LeaveApplicationDetails.Any(y => !string.IsNullOrEmpty(y.LeaveCode) && y.LeaveCode.Equals(sExchangeDetail.CurrentShiftCode, StringComparison.OrdinalIgnoreCase))).ToList();
							if (!(leaveApplications is null) && leaveApplications.Count() > 0)
							{
								returnValue.success = false;
								returnValue.errorCode = "SHIFT_EXCHANGE_EXIST_LEAVE_APPLICATION_SAME_DAY";
								//returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString("MM/dd/yyyy")} - {leaveApplications.First().ReferenceNumber}";
								returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString(strDateFormat)} - {leaveApplications.First().ReferenceNumber}";
								return returnValue;
							}
						}
						else
						{
							//In case, current shift code is target 1.
							//Filter LeaveApplication Has Target 1 shift code
							leaveApplications = leaveApplications.Where(x => x.LeaveApplicationDetails.Any(y => !string.IsNullOrEmpty(y.LeaveCode) && (y.FromDate <= seDate && y.ToDate >= seDate) && !y.LeaveCode.IsShiftOfTarget2())).ToList();
							if (!(leaveApplications is null) && leaveApplications.Count() > 0)
							{
								returnValue.success = false;
								returnValue.errorCode = "SHIFT_EXCHANGE_EXIST_LEAVE_APPLICATION_SAME_DAY";
								//returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString("MM/dd/yyyy")} - {leaveApplications.First().ReferenceNumber}";
								returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString(strDateFormat)} - {leaveApplications.First().ReferenceNumber}";
								return returnValue;
							}
						}
					}

					returnValue.success = true;
					returnValue.errorCode = string.Empty;
					returnValue.message = string.Empty;
					return returnValue;
				}
				else
				{
					returnValue.success = false;
				}
			}
			catch
			{
				returnValue.success = false;
			}
			return returnValue;
		}

		//AEON_658
		public static CheckingResult Check_ShiftExchangeComplete_Valid(this ShiftExchangeDetailForAddOrUpdateViewModel sExchangeDetail, IUnitOfWork unitOfWork, bool forceViewAll = false)
		{
			CheckingResult returnValue = new CheckingResult();
			try
			{
				if (sExchangeDetail != null)
				{
					//Check date is well-form
					DateTime seDate = sExchangeDetail.ShiftExchangeDate.Date;
					string strDateFormat = "dd/MM/yyyy";
					if (seDate == DateTime.MinValue)
					{
						returnValue.success = false;
						returnValue.errorCode = "COMMON_DATE_TIME_WRONG_FORMAT";
						returnValue.message = seDate.ToString(strDateFormat);
						return returnValue;
					}

					//Check User
					User user = sExchangeDetail.UserId.GetUserById(unitOfWork, true);
					if (user is null || !user.IsActivated)
					{
						returnValue.success = false;
						returnValue.errorCode = "COMMON_USER_NOT_FOUND";
						returnValue.message = user.SAPCode;
						return returnValue;
					}

					//Check exist target plan
					TargetPlan targetPlan = user.GetTargetPlanByDateForTargetPlan(unitOfWork, seDate);
					if (targetPlan is null || !targetPlan.Status.Equals(Const.Status.completed))
					{
						returnValue.success = false;
						returnValue.errorCode = "TARGET_PLAN_MUST_BE_APPROVED_YOU_CAN_SUBMIT_SHIFT_EXCHANGE";
						//returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString("MM/dd/yyyy")}";
						returnValue.message = $"{user.FullName} - {user.SAPCode} - {seDate.ToString(strDateFormat)}";
						return returnValue;
					}

					returnValue.success = true;
					returnValue.errorCode = string.Empty;
					returnValue.message = string.Empty;
					return returnValue;
				}
				else
				{
					returnValue.success = false;
				}
			}
			catch
			{
				returnValue.success = false;
			}
			return returnValue;
		}
	}
	#endregion
}