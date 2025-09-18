using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class UserDepartmentSyncHistoryViewModel
    {

        public Guid Id { get; set; }
        [StringLength(10)]
        public string Action { get; set; }
        public Guid DepartmentId { get; set; }
        [StringLength(50)]
        public string DepartmentCode { get; set; }
        [StringLength(255)]
        public string DepartmentName { get; set; }
        [StringLength(50)]
        public string DepartmentSapCode { get; set; }
        public Guid UserId { get; set; }
        [StringLength(255)]
        public string UserSapCode { get; set; }
        [StringLength(255)]
        public string LoginName { get; set; }
        [StringLength(255)]
        public string FullName { get; set; }
        public bool IsHeadCount { get; set; }
        public bool IsConcurrently { get; set; }
        public string ErrorList { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
