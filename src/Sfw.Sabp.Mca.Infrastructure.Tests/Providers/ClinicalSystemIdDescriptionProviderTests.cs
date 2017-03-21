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
    public class ClinicalSystemIdDescriptionProviderTests
    {
        private ClinicalSystemIdDescriptionProvider _clinicalSystemIdDescriptionProvider;
        private IConfigurationManagerWrapper _configurationManagerWrapper;

        [TestInitialize]
        public void Setup()
        {
            _configurationManagerWrapper = A.Fake<IConfigurationManagerWrapper>();

            _clinicalSystemIdDescriptionProvider = new ClinicalSystemIdDescriptionProvider(_configurationManagerWrapper);
        }

        [TestMethod]
        public void GetDescription_GivenValueIsNull_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection());

            _clinicalSystemIdDescriptionProvider.Invoking(x => x.GetDescription())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetDescription_GivenValueIsEmpty_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.ClinicalSystemIdDescription, "" } });

            _clinicalSystemIdDescriptionProvider.Invoking(x => x.GetDescription())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetDescription_GivenValueIsWhitespace_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.ClinicalSystemIdDescription, " " } });

            _clinicalSystemIdDescriptionProvider.Invoking(x => x.GetDescription())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetDescription_GivenValueIsValid_ValueShouldBeReturned()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.ClinicalSystemIdDescription, "description" } });

            var result = _clinicalSystemIdDescriptionProvider.GetDescription();

            result.Should().Be("description");
        }

        [TestMethod]
        public void GetName_GivenValueIsNull_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection());

            _clinicalSystemIdDescriptionProvider.Invoking(x => x.GetName())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetName_GivenValueIsEmpty_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.ClinicalSystemName, "" } });

            _clinicalSystemIdDescriptionProvider.Invoking(x => x.GetName())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetName_GivenValueIsWhitespace_ConfigurationExceptionShouldBeThrown()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.ClinicalSystemName, " " } });

            _clinicalSystemIdDescriptionProvider.Invoking(x => x.GetName())
                .ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void GetName_GivenValueIsValid_ValueShouldBeReturned()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection() { { ApplicationSettingConstants.ClinicalSystemName, "name" } });

            var result = _clinicalSystemIdDescriptionProvider.GetName();

            result.Should().Be("name");
        }
    }
}
