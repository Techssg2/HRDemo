using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;

namespace Aeon.HR.Data.Models
{
    public class Position : SoftDeleteEntity, IAutoNumber, IPermission, IRecruimentEntity
    {
        public Position()
        {
        }

        [Required]
        public string PositionName { get; set; }
        public Guid DeptDivisionId { get; set; }        //lấy từ Department ở Setting => từ field này lấy giá trị Job Grade
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public int ExpiredDay { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public bool HasBudget { get; set; } // Budget hoặc non-budget
        public int Quantity { get; set; }
        public Guid? AssignToId { get; set; }       //Id của Employee
        public string ReferenceNumber { get; set; }     //khi form được save thì hệ thống sẽ tạo ra Reference Number
        public Guid? RequestToHireId { get; set; }
        public string RequestToHireNumber { get; set; }
        public PositionStatus Status { get; set; }

        //Link1
        //  + Department trong setting
        //  + Applicant trong recruitment
        public virtual Department DeptDivision { get; set; }
        public virtual RequestToHire RequestToHire { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
        //  + User trong setting
        public virtual User AssignTo { get; set; }
    }
}