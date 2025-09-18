using System;
using System.ComponentModel.DataAnnotations;
using Aeon.HR.Infrastructure.Interfaces;

namespace Aeon.HR.Data.Models.SyncLog
{
    public class UserSyncHistory : IEntity
    {
        public Guid Id { get; set; }
        [StringLength(10)]
        public string Action { get; set; }
        [StringLength(20)]
        public string SapCode { get; set; }
        [StringLength(255)]
        public string FullName { get; set; }
        public DateTime StartDate { get; set; }
        public bool Status { get; set; }
        public string ErrorList { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}