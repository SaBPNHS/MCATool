using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class FutureDateValidatorTests
    {
        private FutureDateValidator _futureDateValidator;

        [TestInitialize]
        public void Setup()
        {
            var dateTimeProvider = A.Fake<IDateTimeProvider>();

            A.CallTo(() => dateTimeProvider.Now).Returns(DateTime.Now);

            _futureDateValidator = new FutureDateValidator(dateTimeProvider);
        }

        [TestMethod]
        public void Valid_GivenDateInTheFuture_ValidationShouldFail()
        {
            var result = _futureDateValidator.Valid(DateTime.Now.AddDays(1));

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenDateAsCurrentDate_ValidationShouldPass()
        {
            var result = _futureDateValidator.Valid(DateTime.Now);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Valid_GivenDateInThePast_ValidationShouldPass()
        {
            var result = _futureDateValidator.Valid(DateTime.Now);

            result.Should().BeTrue();
        }

    }
}
