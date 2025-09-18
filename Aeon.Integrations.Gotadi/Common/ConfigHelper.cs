using Aeon.HR.ViewModels.CustomSection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.Common
{
    public static class ConfigHelper
    {
        private static GotadiSettingsSection GOTADI_CONFIG = null;

        public static GotadiSettingsSection GetGotadiConfig()
        {
            GotadiSettingsSection returnValue = null;
            try
            {
                if(GOTADI_CONFIG is null)
                {
                    GOTADI_CONFIG = (GotadiSettingsSection)ConfigurationManager.GetSection("airTicketSettings");
                }
                returnValue = GOTADI_CONFIG;
            }
            catch
            {
            }
            return returnValue;
        }
    }
}
