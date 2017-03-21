using System.Collections.Specialized;
using System.Configuration;

namespace Sfw.Sabp.Mca.Infrastructure.Configuration
{
    public interface IConfigurationManagerWrapper
    {
        NameValueCollection AppSettings { get; }
    }

    public class ConfigurationManagerWrapper : IConfigurationManagerWrapper
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }
    }
}
