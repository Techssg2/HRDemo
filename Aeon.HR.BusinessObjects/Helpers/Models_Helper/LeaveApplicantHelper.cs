using Aeon.HR.BusinessObjects.DataHandlers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Data;
using Newtonsoft.Json;
using System.Reflection;
using Aeon.HR.ViewModels;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.BusinessObjects
{
    #region Leave Applicant Helper
    public static class LeaveApplicantHelper
    {
        public static List<LeaveApplication> GetOverlappingLeaveApplications(this LeaveApplicantDetailDTO leaveApplicantDetail, string userSAPCODE, IUnitOfWork uow)
        {
            List<LeaveApplication> returnValue = new List<LeaveApplication>();
            try
            {
                var ignoreStatus = new string[] { "Rejected", "Cancelled", "Draft" };
                List<LeaveApplicationDetail> overlappingLeaveApplications = uow.GetRepository<LeaveApplicationDetail>().FindBy(x => x.LeaveApplication.UserSAPCode == userSAPCODE
                &&
                    (
                        (x.FromDate <= leaveApplicantDetail.FromDate && leaveApplicantDetail.FromDate <= x.ToDate)
                        ||
                        (x.FromDate <= leaveApplicantDetail.ToDate && leaveApplicantDetail.ToDate <= x.ToDate)
                    )
                && !ignoreStatus.Contains(x.LeaveApplication.Status)
                ).ToList();
                returnValue = overlappingLeaveApplications.Select(x=>x.LeaveApplication).Distinct().ToList();
            }
            catch
            {
                returnValue = new List<LeaveApplication>();
            }
            return returnValue;
        }

        public static List<LeaveApplication> GetOverlappingLeaveApplications(this List<LeaveApplicantDetailDTO> leaveDetails, string userSAPCODE, IUnitOfWork uow)
        {
            List<LeaveApplication> returnValue = new List<LeaveApplication>();
            try
            {
                foreach (LeaveApplicantDetailDTO cLeaveDetails in leaveDetails)
                {
                    List<LeaveApplication> leaveApplications = cLeaveDetails.GetOverlappingLeaveApplications(userSAPCODE, uow);
                    if(leaveApplications.Any())
                    {
                        returnValue.AddRange(leaveApplications);
                    }
                }
            }
            catch
            {
                returnValue = new List<LeaveApplication>();
            }
            return returnValue;
        }
    }
    #endregion
}
