using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateUserInfoFromSapUpdate.src;

namespace UpdateUserInfoFromSapJobV2
{
    public class Program
    {
        static void Main(string[] args)
        {
            new UpdateUserInfoFromSap().Run().GetAwaiter().GetResult();
        }
    }
}
