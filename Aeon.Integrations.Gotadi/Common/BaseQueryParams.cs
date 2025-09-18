using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.Common
{
    public abstract class BaseQueryParams
    {
        [AliasAs("time")]
        public string Time
        {
            get
            {
                return ApiHelper.GenerateTime();
            }
        }

        protected string GenerateKey(string combined)
        {
            return ApiHelper.GenerateKey(combined);
        }

        [AliasAs("key")]
        public abstract string Key { get; }
    }
}
