using FluentValidation;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class CreatePatientViewModelValidator : PatientViewModelValidator<CreatePatientViewModel>
    {
        public CreatePatientViewModelValidator(IFutureDateValidator futureDateValidator,
            IClinicalIdValidator clinicalIdValidator,
            INhsValidator nhsValidator,
            IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider)
            : base(futureDateValidator, nhsValidator, clinicalSystemIdDescriptionProvider)
        {
            RuleFor(model => model.NhsNumber)
                .Must(nhsValidator.Unique)
                .WithMessage("A person with this NHS Number already exists");

            RuleFor(model => model.ClinicalSystemId)
                .Must(clinicalIdValidator.Unique)
                .WithMessage(string.Format("A person with this {0} already exists", clinicalSystemIdDescriptionProvider.GetDescription()));
        }
    }
}