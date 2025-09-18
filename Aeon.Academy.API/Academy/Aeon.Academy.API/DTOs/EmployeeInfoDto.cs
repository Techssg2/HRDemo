using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class EmployeeInfoDto
    {
        public string SapCode { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }
        public string Position { get; set; }
    }
}