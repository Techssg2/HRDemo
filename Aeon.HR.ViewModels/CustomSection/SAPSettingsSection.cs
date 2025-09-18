using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.CustomSection
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
    public sealed class SAPHeaderRequestElement : ConfigurationElement
    {
        public SAPHeaderRequestElement() { }
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
    public sealed class SAPAuthenticationElement : ConfigurationElement
    {
        public SAPAuthenticationElement() { }
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
    public sealed class SAPGroupDataItemElement : ConfigurationElement
    {
        public SAPGroupDataItemElement() { }
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
        }
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return this["value"] as string; }
        }
    }
    public sealed class SAPGroupDataCollection : ConfigurationElementCollection
    {
        public SAPGroupDataCollection() { }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SAPGroupDataItemElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SAPGroupDataItemElement)element).Name;
        }
        protected override string ElementName
        {
            get
            {
                return "groupData";
            }
        }
        public new SAPGroupDataItemElement this[string name]
        {
            get { return (SAPGroupDataItemElement)BaseGet(name); }
        }

    }
}
