using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;

namespace Aeon.HR.Data.Models
{
    public class Navigation : SoftDeleteEntity
    {
        public string Name { get; set; }
        public NavigationType Type { get; set; }
        public string Description { get; set; }
        public string Departments { get; set; }
        public string Url { get; set; }
        public string Users { get; set; }
        public Guid? ProfilePictureId { get; set; }
        public virtual AttachmentFile ProfilePicture { get; set; }
        // Menu moi
        public string NameAdd { get; set; }
        public string Title_VI { get; set; }
        public string Title_EN { get; set; }
        public Guid? ParentId { get; set; }
        public double Priority { get; set; }
        // Group Users
        public bool IsAD { get; set; }
        public bool IsMS { get; set; }
        public string Permissions { get; set; }
        public string UserGroups { get; set; }
        public string NonUserGroups { get; set; }
        public string JobGrades { get; set; }
        public bool IsDefault { get; set; }
        public string Module { get; set; }
        // Thong tin cua phong ban
        public class Information
        {
            public string id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
        }
    }
}
