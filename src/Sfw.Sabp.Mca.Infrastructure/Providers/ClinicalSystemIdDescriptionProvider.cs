using System.Configuration;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Infrastructure.Configuration;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public class ClinicalSystemIdDescriptionProvider : IClinicalSystemIdDescriptionProvider
    {
        private readonly IConfigurationManagerWrapper _configurationManagerWrapper;

        public ClinicalSystemIdDescriptionProvider(IConfigurationManagerWrapper configurationManagerWrapper)
        {
            _configurationManagerWrapper = configurationManagerWrapper;
        }

        public string GetDescription()
        {
            var description = _configurationManagerWrapper.AppSettings[ApplicationSettingConstants.ClinicalSystemIdDescription];

            if (string.IsNullOrWhiteSpace(description))
                throw new ConfigurationErrorsException();

            return description;
        }

        public string GetName()
        {
            var description = _configurationManagerWrapper.AppSettings[ApplicationSettingConstants.ClinicalSystemName];

            if (string.IsNullOrWhiteSpace(description))
                throw new ConfigurationErrorsException();

            return description;
        }
    }
}
