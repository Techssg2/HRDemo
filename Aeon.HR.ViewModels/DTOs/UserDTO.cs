using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }             
        public UserRole Role { get; set; }
        public bool IsActivated { get; set; }
        public double ErdRemain { get; set; }
    }
}