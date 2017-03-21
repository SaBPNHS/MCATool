using FluentValidation;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class TerminatedViewModelValidator : AbstractValidator<TerminatedViewModel>
    {
        public TerminatedViewModelValidator()
        {
            RuleFor(model => model.TerminatedAssessmentReason)
                .NotEmpty()
                .WithMessage("Termination reason is mandatory")
                .Length(1, 150)
                .WithMessage("Termination reason must be less than 150 characters");
        }
    }
}