using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class TodoListArgs
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public object Object { get; set; }
        public string LoginName { get; set; }
        public DateTimeOffset? FromDueDate { get; set; }
        public DateTimeOffset? ToDueDate { get; set; }
        public DateTimeOffset? FromCreatedDate { get; set; }
        public DateTimeOffset? ToCreatedDate { get; set; }
        public string Keyword { get; set; }
        public string Module { get; set; }
    }
}