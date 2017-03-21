using FluentValidation;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class EditPatientViewModelValidator : PatientViewModelValidator<EditPatientViewModel>
    {
        public EditPatientViewModelValidator(IFutureDateValidator futureDateValidator,
            IClinicalIdValidator clinicalIdValidator,
            INhsValidator nhsValidator,
            IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider)
            : base(futureDateValidator, nhsValidator, clinicalSystemIdDescriptionProvider)
        {
            When(model => model.CurrentClinicalSystemId != model.ClinicalSystemId,
                () => RuleFor(model => model.ClinicalSystemId)
                        .Must(clinicalIdValidator.Unique)
                        .WithMessage(string.Format("A person with this {0} already exists", clinicalSystemIdDescriptionProvider.GetDescription()))
                );

            When(model => model.NhsNumber != model.CurrentNhsNumber,
                () => RuleFor(model => model.NhsNumber)
                        .Must(nhsValidator.Unique)
                        .WithMessage("A person with this NHS Number already exists")
                );
        }
    }
}