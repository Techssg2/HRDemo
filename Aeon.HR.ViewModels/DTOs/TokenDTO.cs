using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.DTOs
{
    public class TokenDTO
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public long Expires_in { get; set; }
    }
}