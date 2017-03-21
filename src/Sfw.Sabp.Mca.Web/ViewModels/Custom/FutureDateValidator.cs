using System;
using Sfw.Sabp.Mca.Infrastructure.Providers;

namespace Sfw.Sabp.Mca.Web.ViewModels.Custom
{
    public class FutureDateValidator : IFutureDateValidator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public FutureDateValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        
        public bool Valid(DateTime? datetime)
        {
            return datetime.HasValue && ValidateDate(datetime.Value);
        }

        private bool ValidateDate(DateTime datetime)
        {
            return datetime.Date <= _dateTimeProvider.Now.Date;
        }

        public bool Valid(DateTime datetime)
        {
            return ValidateDate(datetime);
        }
    }
}