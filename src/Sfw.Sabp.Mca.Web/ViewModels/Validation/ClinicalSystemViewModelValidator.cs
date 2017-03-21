using FluentValidation;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class ClinicalSystemViewModelValidator : AbstractValidator<ClinicalSystemViewModel>
    {
        public ClinicalSystemViewModelValidator()
        {
            RuleFor(model => model.ClinicalSystemIdText)
                .NotEmpty()
                .WithMessage("Clinical system id is not configured");
        }
    }
}