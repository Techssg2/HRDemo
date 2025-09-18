using System;
using System.Collections.Generic;
using SyncOrgchartJob.Enums;

namespace SyncOrgchartJob.Models.eDocHR
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
        public string ParentSapCode { get; set; }
        public string ParentName { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public DepartmentType Type { get; set; }
        public Guid JobGradeId { get; set; }
        public Guid? BusinessModelId { get; set; }
        public Guid? ParentId { get; set; }
        public string SAPCode { get; set; }
        public string RTHReferenceNumber { get; set; }
        public Guid? RegionId { get; set; }
        public bool IsFromIT { get; set; }
        public bool IsEdoc1 { get; set; }
        public bool IsEdoc { get; set; }
        public bool IsDeleted { get; set; }
        public List<Department> Children { get; set; } = new List<Department>();
        public List<UserDepartmentMapping> Persons { get; set; } = new List<UserDepartmentMapping>();
        #region MyRegion
        public int Grade { get; set; }
        public string GradeTile { get; set; }
        public string RegionName { get; set; }
        #endregion
        #region Colum SAP
        public string SAPObjectId { get; set; }
        public string SAPObjectType { get; set; }
        public string SAPDepartmentParentName { get; set; }
        public string SAPDepartmentParentId { get; set; }
        public string SAPLevel { get; set; }
        public string SAPValidFrom { get; set; }
        public string SAPValidTo { get; set; }
        #endregion
        public NewData NewData { get; set; }
    }

    public class NewData
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public DepartmentType? Type { get; set; }
        public Guid JobGradeId { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentCode { get; set; }
        public string ParentSapCode { get; set; }
        public string ParentName { get; set; }
        public bool IsStore { get; set; }
        public int JobGrade { get; set; }
        public string JobGradeTitle { get; set; }
        public string SAPCode { get; set; }
        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }
        public string SAPObjectId { get; set; }
        public string SAPObjectType { get; set; }
        public string SAPDepartmentParentName { get; set; }
        public string SAPDepartmentParentId { get; set; }
        public string SAPLevel { get; set; }
        public string SAPValidFrom { get; set; }
        public string SAPValidTo { get; set; }
        public string PersonalArea { get; set; }
        public string SubArea { get; set; }
        public Guid? BusinessModelId { get; set; }
        public string BusinessModelCode { get; set; }
        public string BusinessModelName { get; set; }
        public List<string> ErrorList { get; set; }
        public bool Status { get; set; }
        public bool IsParentIsEdoc { get; set; }
    }
}