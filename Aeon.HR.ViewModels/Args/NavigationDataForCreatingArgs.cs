using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class NavigationDataForCreatingArgs
    {
        public Guid Id { get; set; }
        public NavigationType Type { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Module { get; set; }
        public object Users { get; set; }
        public object Departments { get; set; }
        public Guid? ProfilePictureId { get; set; }
        public virtual AttachmentFile ProfilePicture { get; set; }
        public string Title_VI { get; set; }
        public string Title_EN { get; set; }
        public Guid? ParentId { get; set; }
        public double Priority { get; set; }
        public bool IsAD { get; set; }
        public bool IsMS { get; set; }
        public object Permissions { get; set; }
        public object Jobgrades { get; set; }
        public string Name { get; set; }
        public object UserGroups { get; set; }
        public object NonUserGroups { get; set; }
    }
}
