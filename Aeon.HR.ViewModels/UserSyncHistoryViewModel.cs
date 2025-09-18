using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class UserSyncHistoryViewModel
    {
        public Guid Id { get; set; }
        [StringLength(10)]
        public string Action { get; set; }
        [StringLength(20)]
        public string SapCode { get; set; }
        [StringLength(255)]
        public string FullName { get; set; }
        public DateTime StartDate { get; set; }
        public string ErrorList { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
