using FluentValidation;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class DisclaimerViewModelValidator : AbstractValidator<DisclaimerViewModel>
    {
        public DisclaimerViewModelValidator()
        {
            RuleFor(model => model.IsAgreed)
                .Equal(true)
                .WithMessage("Please agree to the terms and conditions in order to continue.");
        }
    }
}