using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.CustomSection
{
    public class GotadiSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public string BaseUrl { get { return this["baseUrl"] as string; } }
        [ConfigurationProperty("saltKey", IsRequired = true)]
        public string Timeouts { get { return this["timeouts"] as string; } }
        [ConfigurationProperty("timeouts", IsRequired = true)]
        public string SaltKey { get { return this["saltKey"] as string; } }
        [ConfigurationProperty("header", IsRequired = true)]
        public BTAHeaderRequestElement Header
        {
            get { return this["header"] as BTAHeaderRequestElement; }
        }
        [ConfigurationProperty("certificates", IsRequired = true)]
        public BTACertificateElement Certificates
        {
            get { return this["certificates"] as BTACertificateElement; }
        }
    }
    public sealed class BTAHeaderRequestElement : ConfigurationElement
    {
        public BTAHeaderRequestElement() { }
        [ConfigurationProperty("format", IsRequired = true)]
        public string Format { get { return this["format"] as string; } }
        [ConfigurationProperty("xRequestWith", IsRequired = true)]
        public string XRequestWith { get { return this["xRequestWith"] as string; } }
        [ConfigurationProperty("connection", IsRequired = true)]
        public string Connection { get { return this["connection"] as string; } }
        [ConfigurationProperty("pragma", IsRequired = true)]
        public string Pragma { get { return this["pragma"] as string; } }
        [ConfigurationProperty("cacheControl", IsRequired = true)]
        public string CacheControl { get { return this["cacheControl"] as string; } }
        [ConfigurationProperty("contentType", IsRequired = true)]
        public string ContentType { get { return this["contentType"] as string; } }
        [ConfigurationProperty("acceptType", IsRequired = true)]
        public string AcceptType { get { return this["acceptType"] as string; } }
        [ConfigurationProperty("apiKey", IsRequired = true)]
        public string APIKey { get { return this["apiKey"] as string; } }
        [ConfigurationProperty("accessCode", IsRequired = true)]
        public string AccessCode { get { return this["accessCode"] as string; } }
    }

    public sealed class BTACertificateElement : ConfigurationElement
    {
        public BTACertificateElement() { }
        [ConfigurationProperty("gotadi_public_key", IsRequired = true)]
        public string Gotadi_Public_Key { get { return this["gotadi_public_key"] as string; } }

        [ConfigurationProperty("aeon_bta_private_key", IsRequired = true)]
        public string AEON_BTA_Private_Key { get { return this["aeon_bta_private_key"] as string; } }

    }
}
