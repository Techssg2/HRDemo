using Aeon.HR.ViewModels.CustomSection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateUserInfoFromSapJobV2.src.ModelEntity
{
    public class SAPSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public string BaseUrl { get { return this["baseUrl"] as string; } }
        [ConfigurationProperty("header", IsRequired = true)]
        public SAPHeaderRequestElement Header
        {
            get { return this["header"] as SAPHeaderRequestElement; }
        }
        [ConfigurationProperty("authentication", IsRequired = true)]
        public SAPAuthenticationElement Authentication
        {
            get { return this["authentication"] as SAPAuthenticationElement; }
        }
        [ConfigurationProperty("groupData", IsRequired = true)]
        public SAPGroupDataCollection SAPGroupDataCollection
        {
            get { return this["groupData"] as SAPGroupDataCollection; }
        }
    }
}
