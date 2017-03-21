using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Infrastructure.Configuration;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class DateOfBirthBuilder : IDateOfBirthBuilder
    {
        private readonly IConfigurationManagerWrapper _configurationManagerWrapper;

        public DateOfBirthBuilder(IConfigurationManagerWrapper configurationManagerWrapper)
        {
            _configurationManagerWrapper = configurationManagerWrapper;
        }

        public DateOfBirthViewModel BuildDateOfBirthViewModel(DateTime? selectedDate)
        {
            int? year = null;
            int? month = null;
            int? day = null;

            if (selectedDate.HasValue)
            {
                year = selectedDate.Value.Year;
                month = selectedDate.Value.Month;
                day = selectedDate.Value.Day;
            }
            
            return new DateOfBirthViewModel()
            {
                Days = BuildOrderedNumbers(31, "Day", day),
                Months = BuildOrderedNumbers(12, "Month", month),
                Years = BuildYears(year),
                Day = day,
                Month = month,
                Year = year
            };
        }

        #region private

        private IEnumerable<SelectListItem> BuildOrderedNumbers(int numberOfItems, string description, int? selected)
        {
            return BuildEmptySelectList(description).Union(CreateRangeSelectListItems(1, numberOfItems, selected));
        }

        private IEnumerable<SelectListItem> BuildYears(int? selected)
        {
            var years =_configurationManagerWrapper.AppSettings[ApplicationSettingConstants.DateOfBirthNumberOfYearsToDisplay];

            if (string.IsNullOrWhiteSpace(years)) throw new ConfigurationErrorsException();

            var yearsNumber = Convert.ToInt32(years);

            var currentYear = DateTime.Now.Year;

            return BuildEmptySelectList("Year").Union(CreateRangeSelectListItems(currentYear - yearsNumber, yearsNumber + 1, selected).OrderByDescending(x => Convert.ToInt32(x.Value)));
        }

        private static IEnumerable<SelectListItem> CreateRangeSelectListItems(int start, int count, int? selected)
        {
            return Enumerable.Range(start, count).Select(item => new SelectListItem { Text = item.ToString(), Value = item.ToString(), Selected = item == selected});
        }

        private IEnumerable<SelectListItem> BuildEmptySelectList(string description)
        {
            return new List<SelectListItem>
            {
                new SelectListItem {Text = description, Value = ""}
            };
        }

        #endregion
    }
}