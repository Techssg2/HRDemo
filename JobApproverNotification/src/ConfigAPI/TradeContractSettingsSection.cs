using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApproverNotification.src.ConfigAPI
{
    public class TradeContractSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public string BaseUrl { get { return this["baseUrl"] as string; } }
        [ConfigurationProperty("header", IsRequired = true)]
        public TradeContractHeaderRequestElement Header
        {
            get { return this["header"] as TradeContractHeaderRequestElement; }
        }
        [ConfigurationProperty("authentication", IsRequired = true)]
        public TradeContractAuthenticationElement Authentication
        {
            get { return this["authentication"] as TradeContractAuthenticationElement; }
        }
    }
    public sealed class TradeContractHeaderRequestElement : ConfigurationElement
    {
        public TradeContractHeaderRequestElement() { }
        [ConfigurationProperty("format", IsRequired = true)]
        public string Format { get { return this["format"] as string; } }
        [ConfigurationProperty("contentType", IsRequired = true)]
        public string ContentType { get { return this["contentType"] as string; } }
        [ConfigurationProperty("acceptType", IsRequired = true)]
        public string AcceptType { get { return this["acceptType"] as string; } }
    }
    public sealed class TradeContractAuthenticationElement : ConfigurationElement
    {
        public TradeContractAuthenticationElement() { }
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
