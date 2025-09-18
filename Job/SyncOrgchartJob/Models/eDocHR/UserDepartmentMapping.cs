using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SyncOrgchartJob.Models.eDocHR
{
    public class UserDepartmentMapping
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public SyncOrgchartJob.Enums.Group Role { get; set; } // là role hod hoặc checked
        public bool IsHeadCount { get; set; } // là người đứng đầu
        public string Note { get; set; }
        public bool? Authorizated { get; set; }
        public bool IsFromIT { get; set; }
        public bool IsEdoc { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? CreatedById { get; set; }

        #region Other Properties
        public string LoginName { get; set; }
        public string UserSAPCode { get; set; }
        public string DepartmentEdocSAPCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
        #endregion
        #region SAP Properties
        public string DepartmentSAPObjectSId { get; set; }
        public string DepartmentSAPObjectSName { get; set; }
        public bool IsConcurrently { get; set;}
        #endregion
        public NewDataUDM NewData { get; set;}
    }

    public class NewDataUDM
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public SyncOrgchartJob.Enums.Group Role { get; set; } 
        public bool IsHeadCount { get; set; }
        
        public string Note { get; set; }
        public bool? Authorizated { get; set; }
        
        public bool IsFromIT { get; set; }
        public string LoginName { get; set; }
        public string UserSAPCode { get; set; }
        public string DepartmentEdocSAPCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
        public bool Status { get; set; }
        
        public bool IsConcurrently { get; set; }
        public List<string> ErrorList { get; set; } = new List<string>();
    }
}