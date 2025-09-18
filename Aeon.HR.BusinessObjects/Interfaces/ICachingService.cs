using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ICachingService
    {
        IDictionary<string, List<MasterExternalDataViewModel>> MasterExternalDatas { get; set; }     
        IDictionary<string, string> MasterDataInformation { get; }
        string MasterDataName { get; set; }


    }
}
