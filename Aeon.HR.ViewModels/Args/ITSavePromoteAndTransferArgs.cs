using Aeon.HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ITSavePromoteAndTransferArgs
    {
        public string ReferenceNumber { get; set; }
        public string NewWorkLocationName { get; set; }
    }
}
