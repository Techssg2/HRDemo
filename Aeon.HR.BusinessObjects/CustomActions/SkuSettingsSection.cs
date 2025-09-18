using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.CustomSection
{
    public class SkuSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public string BaseUrl { get { return this["baseUrl"] as string; } }
        [ConfigurationProperty("header", IsRequired = true)]
        public SkuHeaderRequestElement Header
        {
            get { return this["header"] as SkuHeaderRequestElement; }
        }
    }
    public sealed class SkuHeaderRequestElement : ConfigurationElement
    {
        public SkuHeaderRequestElement() { }
        [ConfigurationProperty("format", IsRequired = true)]
        public string Format { get { return this["format"] as string; } }
        [ConfigurationProperty("contentType", IsRequired = true)]
        public string ContentType { get { return this["contentType"] as string; } }
        [ConfigurationProperty("acceptType", IsRequired = true)]
        public string AcceptType { get { return this["acceptType"] as string; } }
    }
}
