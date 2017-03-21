using System.Collections.Specialized;
using System.Configuration;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Infrastructure.Configuration;
using Sfw.Sabp.Mca.Infrastructure.Providers;

namespace Sfw.Sabp.Mca.Infrastructure.Tests.Providers
{
    [TestClass]
    public class UserRoleProviderTests
    {
        private UserRoleProvider _userRoleProvider;
        private IConfigurationManagerWrapper _configurationManagerWrapper;
        private IWindowsTokenRoleProviderWrapper _windowsTokenRoleProviderWrapper;
        private IUserPrincipalProvider _userPrincipalProvider;
        private string _administratorsgroup;
        private string _username;

        [TestInitialize]
        public void Setup()
        {
            _configurationManagerWrapper = A.Fake<IConfigurationManagerWrapper>();
            _windowsTokenRoleProviderWrapper = A.Fake<IWindowsTokenRoleProviderWrapper>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();

            _administratorsgroup = "administratorsgroup";
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.McaAdministratorsActiveDirectoryGroup, _administratorsgroup } });
            _username = "username";
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(_username);

            _userRoleProvider = new UserRoleProvider(_configurationManagerWrapper, _windowsTokenRoleProviderWrapper, _userPrincipalProvider);
        }

        [TestMethod]
        public void UserInAdministratorRole_GivenUserIsInRole_TrueShouldBeReturned()
        {
            A.CallTo(() => _windowsTokenRoleProviderWrapper.IsUserInRole(_username, _administratorsgroup)).Returns(true);

            _userRoleProvider.CurrentUserInAdministratorRole().Should().BeTrue();
        }

        [TestMethod]
        public void UserInAdministratorRole_GivenUserIsNotInRole_FalseShouldBeReturned()
        {
            A.CallTo(() => _windowsTokenRoleProviderWrapper.IsUserInRole(_username, _administratorsgroup)).Returns(false);

            _userRoleProvider.CurrentUserInAdministratorRole().Should().BeFalse();
        }

        [TestMethod]
        public void UserInAdministratorRole_GivenApplicationSettingIsNullEmpty_ConfigurationErrorsExceptionExpected()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection());

            _userRoleProvider.Invoking(x => x.CurrentUserInAdministratorRole()).ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void UserInAdministratorRole_GivenApplicationSettingIsEmpty_ConfigurationErrorsExceptionExpected()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.McaAdministratorsActiveDirectoryGroup, "" } });

            _userRoleProvider.Invoking(x => x.CurrentUserInAdministratorRole()).ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void UserInAdministratorRole_GivenApplicationSettingIsWhitespace_ConfigurationErrorsExceptionExpected()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.McaAdministratorsActiveDirectoryGroup, " " } });

            _userRoleProvider.Invoking(x => x.CurrentUserInAdministratorRole()).ShouldThrow<ConfigurationErrorsException>();
        }
    }
}
