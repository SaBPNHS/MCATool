using FluentValidation;
using Sfw.Sabp.Mca.Infrastructure.Providers;


namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class PatientSearchViewModelValidator : AbstractValidator<PatientSearchViewModel>
    {
        public PatientSearchViewModelValidator(IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider)
        {
            RuleFor(model => model.ClinicalSystemId)
                .NotEmpty()
                .WithMessage(string.Format("{0} is mandatory", clinicalSystemIdDescriptionProvider.GetDescription()));            
        }
    }
}