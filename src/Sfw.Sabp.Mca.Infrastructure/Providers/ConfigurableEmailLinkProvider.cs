using Sfw.Sabp.Mca.Core.Constants;
using System.Configuration;
using Sfw.Sabp.Mca.Infrastructure.Configuration;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public class ConfigurableEmailLinkProvider : IConfigurableEmailLinkProvider
    {
        private readonly IConfigurationManagerWrapper _configurationManagerWrapper;

        public ConfigurableEmailLinkProvider(IConfigurationManagerWrapper configurationManagerWrapper)
        {
            _configurationManagerWrapper = configurationManagerWrapper;
        }

        public string GetEmailAddress()
        {
            var description = _configurationManagerWrapper.AppSettings[ApplicationSettingConstants.FeedbackEmailLink];

            if (string.IsNullOrWhiteSpace(description))
                throw new ConfigurationErrorsException();

            return description;
        }
    }
}
