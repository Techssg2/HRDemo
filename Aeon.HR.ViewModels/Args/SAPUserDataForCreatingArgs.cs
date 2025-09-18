using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class SAPUserDataForCreatingArgs
    {
        public SAPUserDataForCreatingArgs()
        {
        }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DeptCode { get; set; }
        public UserRole Role { get
            {
                return UserRole.Member;
            }
        }
        public LoginType Type { get
            {
                return LoginType.Membership;
            }
        }
        public string LoginName
        {
            get
            {
                string returnValue = string.Empty;
                try
                {
                    int iTemp = 0;
                    if (!string.IsNullOrEmpty(SAPCode) && int.TryParse(SAPCode, out iTemp) && iTemp > 0)
                    {
                        returnValue = iTemp.ToString();
                    }
                }
                catch
                {
                    returnValue = string.Empty;
                }
                return returnValue;
            }
        }
        // Ngày nhân viên bắt đầu vào làm AEON 
        public DateTime? StartDate { get; set; }
        // Extra Rest Day (Nghỉ bù thứ bảy) còn lại trong năm
        public double ErdRemain { get; set; }
        public string[] Permissions { get; set; }
        //Profile
        public Guid? ProfilePictureId { get; set; }
    }
}