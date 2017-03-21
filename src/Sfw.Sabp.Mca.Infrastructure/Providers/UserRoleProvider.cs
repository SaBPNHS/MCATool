using System.Configuration;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Infrastructure.Configuration;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public class UserRoleProvider : IUserRoleProvider
    {
        private readonly IConfigurationManagerWrapper _configurationManagerWrapper;
        private readonly IWindowsTokenRoleProviderWrapper _windowsTokenRoleProviderWrapper;
        private readonly IUserPrincipalProvider _userPrincipalProvider;

        public UserRoleProvider(IConfigurationManagerWrapper configurationManagerWrapper, IWindowsTokenRoleProviderWrapper windowsTokenRoleProviderWrapper, IUserPrincipalProvider userPrincipalProvider)
        {
            _configurationManagerWrapper = configurationManagerWrapper;
            _windowsTokenRoleProviderWrapper = windowsTokenRoleProviderWrapper;
            _userPrincipalProvider = userPrincipalProvider;
        }

        public bool CurrentUserInAdministratorRole()
        {
            var role = _configurationManagerWrapper.AppSettings[ApplicationSettingConstants.McaAdministratorsActiveDirectoryGroup];

            if(string.IsNullOrWhiteSpace(role))
                throw new ConfigurationErrorsException();

            return _windowsTokenRoleProviderWrapper.IsUserInRole(_userPrincipalProvider.CurrentUserName, role);
        }
    }
}
