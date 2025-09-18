using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class VoteArgs
    {
        public Guid ItemId { get; set; }
        public VoteType Vote { get; set; }
        public string Comment { get; set; }
    }
}