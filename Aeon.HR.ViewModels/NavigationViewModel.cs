using Aeon.HR.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static Aeon.HR.Data.Models.Navigation;

namespace Aeon.HR.ViewModels
{
    public class NavigationViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public NavigationType Type { get; set; }
        public string Module { get; set; }
        public string Description { get; set; }
        public string Departments { get; set; }
        public string Url { get; set; }
        public string Users { get; set; }
        public Guid? ProfilePictureId { get; set; }
        public string ProfilePicture { get; set; }
        public string Title_VI { get; set; }
        public string Title_EN { get; set; }
        public int CountChild { get; set; } // child 
        public string JsonChild { get; set; } // json child
        public double Priority { get; set; }
        public bool IsAD { get; set; }
        public bool IsMS { get; set; }
        public string Permissions { get; set; }
        public string JobGrades { get; set; }
        public string UserGroups { get; set; }
        public string NonUserGroups { get; set; }
        public List<Information> DepartmentList
        {
            get
            {
                if (Departments != null)
                {
                    return JsonConvert.DeserializeObject<List<Information>>(Departments);
                }
                else
                {
                    return null;
                }
            }
        }
        public List<Information> UsersList
        {
            get
            {
                if (Users != null)
                {
                    return JsonConvert.DeserializeObject<List<Information>>(Users);
                }
                else
                {
                    return null;
                }
            }
        }
        public Guid? ParentId { get; set; }
    }
}
