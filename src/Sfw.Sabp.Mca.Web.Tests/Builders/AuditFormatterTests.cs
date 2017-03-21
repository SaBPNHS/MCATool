using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.Builders;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class AuditFormatterTests
    {
        private AuditFormatter _auditFormatter;
        private NameValueCollection _nameValueCollection;
        private NameValueCollectionValueProvider _nameValueProviderCollection;

        [TestInitialize]
        public void Setup()
        {
            _auditFormatter = new AuditFormatter();

            _nameValueCollection = new NameValueCollection();
            _nameValueProviderCollection = new NameValueCollectionValueProvider(_nameValueCollection, null);
        }

        [TestMethod]
        public void AuditValues_GivenValueDoesNotExistValueProvider_NullReferenceExceptionExpected()
        {
            _auditFormatter.Invoking(x => x.AuditValues(_nameValueProviderCollection, "auditMe")).ShouldThrow<NullReferenceException>();
        }

        [TestMethod]
        public void AuditValues_GivenValueExists_FormatShouldBeValid()
        {
             _nameValueCollection.Add("auditMe", "auditMeValue");

            var result = _auditFormatter.AuditValues(_nameValueProviderCollection, "auditMe");

            result.Should().Be("auditMe[auditMeValue]");
        }

        [TestMethod]
        public void AuditValues_GivenValueExistsAndOldValueExists_FormatShouldBeValid()
        {
            _nameValueCollection.Add("auditMe", "auditMeValue");
            _nameValueCollection.Add("currentAuditMe", "currentAuditMeValue");

            var result = _auditFormatter.AuditValues(_nameValueProviderCollection, "auditMe");

            result.Should().Be("auditMe[auditMeValue:currentAuditMeValue]");
        }

        [TestMethod]
        public void AuditValues_GivenMultipleValuesExistWithNoOldValues_FormatShouldBeValid()
        {
            _nameValueCollection.Add("auditMe", "auditMeValue");
            _nameValueCollection.Add("auditMeAgain", "auditMeValueAgain");

            var result = _auditFormatter.AuditValues(_nameValueProviderCollection, "auditMe,auditMeAgain");

            result.Should().Be("auditMe[auditMeValue],auditMeAgain[auditMeValueAgain]");
        }

        [TestMethod]
        public void AuditValues_GivenMultipleValuesExistWithOldValues_FormatShouldBeValid()
        {
            _nameValueCollection.Add("auditMe", "auditMeValue");
            _nameValueCollection.Add("currentAuditMe", "currentAuditMeValue");
            _nameValueCollection.Add("auditMeAgain", "auditMeValueAgain");
            _nameValueCollection.Add("currentAuditMeAgain", "currentAuditMeValueAgain");

            var result = _auditFormatter.AuditValues(_nameValueProviderCollection, "auditMe,auditMeAgain");

            result.Should().Be("auditMe[auditMeValue:currentAuditMeValue],auditMeAgain[auditMeValueAgain:currentAuditMeValueAgain]");
        }

        [TestMethod]
        public void AuditValues_GivenMixtureOfValuesThatExistSomeWithOldValues_FormatShouldBeValid()
        {
            _nameValueCollection.Add("auditMe1", "auditMe1Value");
            _nameValueCollection.Add("auditMe2", "auditMe2Value");
            _nameValueCollection.Add("currentAuditMe2", "currentAuditMe2Value");
            _nameValueCollection.Add("auditMe3", "auditMe3Value");
            _nameValueCollection.Add("auditMe4", "auditMe4Value");
            _nameValueCollection.Add("currentAuditMe4", "currentAuditMe4Value");

            var result = _auditFormatter.AuditValues(_nameValueProviderCollection, "auditMe1,auditMe2,auditMe3,auditMe4");

            result.Should().Be("auditMe1[auditMe1Value],auditMe2[auditMe2Value:currentAuditMe2Value],auditMe3[auditMe3Value],auditMe4[auditMe4Value:currentAuditMe4Value]");
        }
  
    }
}
