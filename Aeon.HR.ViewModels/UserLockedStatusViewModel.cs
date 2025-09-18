using Aeon.HR.Infrastructure.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.ViewModels.BTA;

namespace Aeon.HR.ViewModels
{
    public class UserLockedStatusViewModel: UserListViewModel
    {
        public bool IsLoginLocked { get; set; }
    }
}