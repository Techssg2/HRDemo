using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aeon.HR.Data.Models
{
    public class LogHistory : IEntity
    {
        public string ModuleName { get; set; }
        public string TypeAction { get; set; }
        public string IdItem { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string Document { get; set; }
        public string Comment { get; set; }
        public string ErrorLog { get; set; }

        public Guid CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
        public string UserName { get; set; }     //luu lai ten login cua IT helpdesk
        public string FullName { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
