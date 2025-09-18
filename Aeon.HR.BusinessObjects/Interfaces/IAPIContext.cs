using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IAPIContext
    {
        string CurrentUser { get;  }
        bool ValidateContext(HttpRequestHeaders headers, string uri);
    }
}
