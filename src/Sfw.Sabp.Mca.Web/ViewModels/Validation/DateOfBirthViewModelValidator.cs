using FluentValidation;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class DateOfBirthViewModelValidator : AbstractValidator<DateOfBirthViewModel>
    {
        public DateOfBirthViewModelValidator(IFutureDateValidator futureDateValidator)
        {
            RuleFor(model => model.Day)
                .NotEmpty()
                .WithMessage("Day is mandatory");

            RuleFor(model => model.Month)
                .NotEmpty()
                .WithMessage("Month is mandatory");

            RuleFor(model => model.Year)
                .NotEmpty()
                .WithMessage("Year is mandatory");

            RuleFor(model => model.Date)
                .NotNull()
                .WithMessage("Selected date is not valid");

            When(x => x.Date != null,
                () => RuleFor(model => model.Date)
                        .Must(futureDateValidator.Valid) 
                        .WithMessage("Date of birth must not be in the future"));
        }
    }
}