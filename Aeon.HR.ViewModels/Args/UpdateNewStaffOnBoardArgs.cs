using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class UpdateNewStaffOnBoardArgs
    {
        public Guid ApplicantId { get; set; }
        public Guid ApplicantStatusId { get; set; }

    }
}