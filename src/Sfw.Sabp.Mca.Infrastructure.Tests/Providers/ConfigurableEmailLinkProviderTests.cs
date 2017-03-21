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
    public class ConfigurableEmailLinkProviderTests
    {
        private ConfigurableEmailLinkProvider _configurableEmailLinkProvider;
        private IConfigurationManagerWrapper _configurationManagerWrapper;

        [TestInitialize]
        public void Setup()
        {
            _configurationManagerWrapper = A.Fake<IConfigurationManagerWrapper>();

            _configurableEmailLinkProvider = new ConfigurableEmailLinkProvider(_configurationManagerWrapper);
        }

        [TestMethod]
        public void GetEmailAddress_GivenValueIsNull_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection());

            _configurableEmailLinkProvider.Invoking(x => x.GetEmailAddress())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetEmailAddress_GivenValueIsEmpty_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.FeedbackEmailLink, "" } });

            _configurableEmailLinkProvider.Invoking(x => x.GetEmailAddress())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetEmailAddress_GivenValueIsWhitespace_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.FeedbackEmailLink, " " } });

            _configurableEmailLinkProvider.Invoking(x => x.GetEmailAddress())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetEmailAddress_GivenValueIsValid_ValueShouldBeReturned()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.FeedbackEmailLink, "emailaddress" } });

            var result = _configurableEmailLinkProvider.GetEmailAddress();

            result.Should().Be("emailaddress");
        }
    }
}
