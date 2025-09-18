using Aeon.HR.ViewModels.CustomSection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.Common
{
    public static class ApiHelper
    {
        public static string GenerateKey(string combined)
        {
            GotadiSettingsSection gotadiSettings = ConfigHelper.GetGotadiConfig();
            Encoding ascii = Encoding.ASCII;
            HMACSHA256 hmac = new HMACSHA256(ascii.GetBytes(gotadiSettings.SaltKey));
            string calc_sig = Convert.ToBase64String(hmac.ComputeHash(ascii.GetBytes(combined)));
            return calc_sig;
        }

        public static string GenerateTime()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }
    }
}
