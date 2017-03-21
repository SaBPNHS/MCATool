using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Web.ValueProviders;

namespace Sfw.Sabp.Mca.Web.Tests.ValueProviders
{
    [TestClass]
    public class DateOfBirthCustomValueProviderTests
    {
        private DateOfBirthCustomValueProvider _dateOfBirthCustomValueProvider;

        [TestMethod]
        public void ContainsPrefix_ShouldReturnFalse()
        {
            _dateOfBirthCustomValueProvider = new DateOfBirthCustomValueProvider(A.Fake<ControllerContext>());

            var result = _dateOfBirthCustomValueProvider.ContainsPrefix(A<string>._);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GetValue_GivenKeyIsNotDateOfBirthKey_NullShouldBeReturned()
        {
            _dateOfBirthCustomValueProvider = new DateOfBirthCustomValueProvider(A.Fake<ControllerContext>());

            var result = _dateOfBirthCustomValueProvider.GetValue("key");

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetValue_GivenKeyIsDateOfBirthKey_ValueProviderResultShouldBeReturned()
        {
            var controllerContext = A.Fake<ControllerContext>();
            var httpContext = A.Fake<HttpContextBase>();
            var httpRequest = A.Fake<HttpRequestBase>();

            A.CallTo(() => httpContext.Request).Returns(httpRequest);
            A.CallTo(() => controllerContext.HttpContext).Returns(httpContext);
            A.CallTo(() => httpRequest.Form).Returns(new NameValueCollection()
            {
                {ApplicationStringConstants.DateofBirthViewModelDayKey, "1"},
                {ApplicationStringConstants.DateofBirthViewModelMonthKey, "1"},
                {ApplicationStringConstants.DateofBirthViewModelYearKey, "2015"}
            });

            _dateOfBirthCustomValueProvider = new DateOfBirthCustomValueProvider(controllerContext);

            var result = _dateOfBirthCustomValueProvider.GetValue(ApplicationStringConstants.DateOfBirthValueKey);

            result.AttemptedValue.Should().Be(new DateTime(2015, 1, 1).ToShortDateString());
            result.RawValue.Should().Be(new DateTime(2015, 1, 1));
        }

    }
}
