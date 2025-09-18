using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class UserListAPIViewModel
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActivated { get; set; }
        public UserRole Role { get; set; }
        public Gender Gender { get; set; }
    }
}
