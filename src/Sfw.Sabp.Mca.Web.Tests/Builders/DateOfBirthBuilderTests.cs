using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Infrastructure.Configuration;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class DateOfBirthBuilderTests
    {
        private DateOfBirthBuilder _dateOfBirthBuilder;
        private IConfigurationManagerWrapper _configurationManagerWrapper;
        private int _numberOfYears;

        [TestInitialize]
        public void Setup()
        {
            _configurationManagerWrapper = A.Fake<IConfigurationManagerWrapper>();

            _numberOfYears = 100;
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection(){
                {ApplicationSettingConstants.DateOfBirthNumberOfYearsToDisplay, _numberOfYears.ToString()}
            });

            _dateOfBirthBuilder = new DateOfBirthBuilder(_configurationManagerWrapper);
        }

        [TestMethod]
        public void BuildDateOfBirthViewModel_ShouldReturnDateOfBirthViewModel()
        {
            var result = _dateOfBirthBuilder.BuildDateOfBirthViewModel(null);

            result.Should().BeOfType<DateOfBirthViewModel>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void BuildDateOfBirthViewModel_ShouldReturnListOfDays()
        {
            var result = _dateOfBirthBuilder.BuildDateOfBirthViewModel(null);

            AssertItemZero(result.Days, "Day");
            AssertDaysOrMonths(result.Days.ToList(), 31);
        }

        [TestMethod]
        public void BuildDateOfBirthViewModel_ShouldReturnListOfMonths()
        {
            var result = _dateOfBirthBuilder.BuildDateOfBirthViewModel(null);

            AssertItemZero(result.Months, "Month");

            AssertDaysOrMonths(result.Months.ToList(), 12);
        }

        [TestMethod]
        public void BuildDateOfBirthViewModel_ShouldReturnListOfYears()
        {
            var result = _dateOfBirthBuilder.BuildDateOfBirthViewModel(null);

            AssertItemZero(result.Years, "Year");

            result.Years.Count().Should().Be(_numberOfYears + 2);

            var currentYear = DateTime.Now.Year;

            for (var i = 0; i <= _numberOfYears; i++)
            {
                var text = (currentYear - i).ToString();
                result.Years.ElementAt(i + 1).ShouldBeEquivalentTo(new SelectListItem() { Text = text, Value = text });
            }
        }

        [TestMethod]
        public void BuildDateofBirthViewModel_GivenNumberOfYearsIsEmpty_ConfigurationErrorsExceptionExpected()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection(){
                {ApplicationSettingConstants.DateOfBirthNumberOfYearsToDisplay, ""}
            });

            _dateOfBirthBuilder.Invoking(x => x.BuildDateOfBirthViewModel(null)).ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void BuildDateofBirthViewModel_GivenNumberOfYearsIsWhitespace_ConfigurationErrorsExceptionExpected()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection(){
                {ApplicationSettingConstants.DateOfBirthNumberOfYearsToDisplay, " "}
            });

            _dateOfBirthBuilder.Invoking(x => x.BuildDateOfBirthViewModel(null)).ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void BuildDateofBirthViewModel_GivenNumberOfYearsIsNull_ConfigurationErrorsExceptionExpected()
        {
            A.CallTo(() => _configurationManagerWrapper.AppSettings).Returns(new NameValueCollection());

            _dateOfBirthBuilder.Invoking(x => x.BuildDateOfBirthViewModel(null)).ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void BuildDateofBirthViewModel_GivenSelectedDate_RelevantSelectListItemsShouldHaveSelectedPropertySet()
        {
            var result = _dateOfBirthBuilder.BuildDateOfBirthViewModel(new DateTime(2015, 1, 2));

            result.Days.Count(x => x.Selected).Should().Be(1);
            result.Days.First(x => x.Selected).Text.Should().Be("2");
            result.Days.First(x => x.Selected).Value.Should().Be("2");
            result.Months.Count(x => x.Selected).Should().Be(1);
            result.Months.First(x => x.Selected).Text.Should().Be("1");
            result.Months.First(x => x.Selected).Value.Should().Be("1");
            result.Years.Count(x => x.Selected).Should().Be(1);
            result.Years.First(x => x.Selected).Text.Should().Be("2015");
            result.Years.First(x => x.Selected).Value.Should().Be("2015");
        }

        [TestMethod]
        public void BuildDateofBirthViewModel_GivenSelectedDate_DatePropertiesShouldBeSet()
        {
            var result = _dateOfBirthBuilder.BuildDateOfBirthViewModel(new DateTime(2015, 1, 2));

            result.Day.Should().Be(2);
            result.Month.Should().Be(1);
            result.Year.Should().Be(2015);
        }

        #region private

        private void AssertItemZero(IEnumerable<SelectListItem> items, string description)
        {
            items.ElementAt(0).ShouldBeEquivalentTo(new SelectListItem() { Text = description, Value = "" });
        }

        private void AssertDaysOrMonths(List<SelectListItem> items, int numberOfItems)
        {
            items.Count.Should().Be(numberOfItems + 1);
            for (int i = 1; i <= numberOfItems; i++)
            {
                items.ElementAt(i).ShouldBeEquivalentTo(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
        }

        #endregion
    }
}
