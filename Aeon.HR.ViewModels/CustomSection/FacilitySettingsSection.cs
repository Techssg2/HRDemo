using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.CustomSection
{
    public class FacilitySettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public string BaseUrl { get { return this["baseUrl"] as string; } }
        [ConfigurationProperty("header", IsRequired = true)]
        public FacilityHeaderRequestElement Header
        {
            get { return this["header"] as FacilityHeaderRequestElement; }
        }
        [ConfigurationProperty("authentication", IsRequired = true)]
        public FacilityAuthenticationElement Authentication
        {
            get { return this["authentication"] as FacilityAuthenticationElement; }
        }
    }
    public sealed class FacilityHeaderRequestElement : ConfigurationElement
    {
        public FacilityHeaderRequestElement() { }
        [ConfigurationProperty("format", IsRequired = true)]
        public string Format { get { return this["format"] as string; } }
        [ConfigurationProperty("xRequestWith", IsRequired = true)]
        public string XRequestWith { get { return this["xRequestWith"] as string; } }
        [ConfigurationProperty("sapClient", IsRequired = true)]
        public string SapClient { get { return this["sapClient"] as string; } }
        [ConfigurationProperty("contentType", IsRequired = true)]
        public string ContentType { get { return this["contentType"] as string; } }
        [ConfigurationProperty("acceptType", IsRequired = true)]
        public string AcceptType { get { return this["acceptType"] as string; } }
        [ConfigurationProperty("Token", IsRequired = true)]
        public string Token { get { return this["Token"] as string; } }
    }
    public sealed class FacilityAuthenticationElement : ConfigurationElement
    {
        public FacilityAuthenticationElement() { }
        [ConfigurationProperty("username", IsRequired = true)]
        public string UserName
        {
            get { return this["username"] as string; }
        }
        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return this["password"] as string; }
        }

    }
}
