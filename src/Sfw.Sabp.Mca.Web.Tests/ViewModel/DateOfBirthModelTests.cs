using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.Tests.ViewModel
{
    [TestClass]
    public class DateOfBirthModelTests
    {
        [TestMethod]
        public void QuestionViewModel_ShouldContainQuestionViewModelValidatorAttribute()
        {
            typeof(DateOfBirthViewModel).Should().BeDecoratedWith<FluentValidation.Attributes.ValidatorAttribute>(x => x.ValidatorType == typeof(DateOfBirthViewModelValidator));
        }

        [TestMethod]
        public void DateOfBirth_GivenDayIsNull_NullShouldBeReturned()
        {
            var model = new DateOfBirthViewModel() {Month = 1, Year = 2015};

            model.Date.Should().Be(null);
        }

        [TestMethod]
        public void DateOfBirth_GivenMonthIsNull_NullShouldBeReturned()
        {
            var model = new DateOfBirthViewModel() {Day = 1, Year = 2015};

            model.Date.Should().Be(null);
        }

        [TestMethod]
        public void DateOfBirth_GivenYearIsNull_NullShouldBeReturned()
        {
            var model = new DateOfBirthViewModel() {Day = 1, Month = 1};

            model.Date.Should().Be(null);
        }

        [TestMethod]
        public void DateofBirth_GivenInvalidDate_NullShouldBeReturned()
        {
            var model = new DateOfBirthViewModel() {Day = 31, Month = 2, Year = 2015};

            model.Date.Should().Be(null);
        }

        [TestMethod]
        public void DateOfBirth_GivenValidDate_DateShouldBeReturned()
        {
            var model = new DateOfBirthViewModel() {Day = 1, Month = 1, Year = 2015};

            model.Date.Should().Be(new DateTime(2015, 1, 1));
        }
    }
}
