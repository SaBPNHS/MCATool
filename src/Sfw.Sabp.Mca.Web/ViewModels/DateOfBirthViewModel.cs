using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    [FluentValidation.Attributes.Validator(typeof(DateOfBirthViewModelValidator))]
    public class DateOfBirthViewModel
    {
        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public IEnumerable<SelectListItem> Days { get; set; }

        public IEnumerable<SelectListItem> Months { get; set; }

        public IEnumerable<SelectListItem> Years { get; set; }

        public DateTime? Date
        {
            get
            {
                try
                {
                    if (!Day.HasValue || !Month.HasValue || !Year.HasValue)
                    {
                        return null;
                    }
                    return new DateTime(Year.Value, Month.Value, Day.Value);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
        }
    }
}