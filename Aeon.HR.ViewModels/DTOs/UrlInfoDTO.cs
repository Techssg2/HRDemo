using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class UrlInfoDTO
    {
        public string Url { get; set; }
        public StringContent Content { get; set; }
        public string SourceRequest { get; set; }
    }
}
