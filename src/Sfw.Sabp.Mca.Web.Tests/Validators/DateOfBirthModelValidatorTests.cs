using System;
using FakeItEasy;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.Validators
{
    [TestClass]
    public class DateOfBirthModelValidatorTests
    {
        private DateOfBirthViewModelValidator _dateOfBirthViewModelValidator;
        private IFutureDateValidator _futureDateValidator;

        [TestInitialize]
        public void Setup()
        {
            _futureDateValidator = A.Fake<IFutureDateValidator>();

            A.CallTo(() => _futureDateValidator.Valid(A<DateTime?>._)).Returns(true);

            _dateOfBirthViewModelValidator = new DateOfBirthViewModelValidator(_futureDateValidator);
        }

        [TestMethod]
        public void Valid_GivenDayHasNotBeenSpecified_ValidationShouldFail()
        {
            var model = new DateOfBirthViewModel()
            {
                Month = 1,
                Year = 2015
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenMonthHasNotBeenSpecified_ValidationShouldFail()
        {
            var model = new DateOfBirthViewModel()
            {
                Day = 1,
                Year = 2015
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenYearHasNotBeenSpecified_ValidationShouldFail()
        {
            var model = new DateOfBirthViewModel()
            {
                Day = 1,
                Month = 1
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenDateOfBirthIsInvalid_ValidationShouldFail()
        {
            var model = new DateOfBirthViewModel()
            {
                Day = 31,
                Month = 2,
                Year = 2015
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void Valid_GivenDateOfBirthIsValid_ValidationShouldPass()
        {
            var model = new DateOfBirthViewModel()
            {
                Day = 1,
                Month = 1,
                Year = 2015
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]

        public void Valid_GivenDateOfBirthIsInTheFuture_ValidationShouldFail()
        {
            A.CallTo(() => _futureDateValidator.Valid(A<DateTime?>._)).Returns(false);

            var model = new DateOfBirthViewModel()
            {
                Day = 1,
                Month = 1,
                Year = 2050
            };

            var result = ValidationResult(model);

            result.IsValid.Should().BeFalse();
        }

        #region private

        private ValidationResult ValidationResult(DateOfBirthViewModel model)
        {
            var result = _dateOfBirthViewModelValidator.Validate(model);
            return result;
        }

        #endregion
    }
}
