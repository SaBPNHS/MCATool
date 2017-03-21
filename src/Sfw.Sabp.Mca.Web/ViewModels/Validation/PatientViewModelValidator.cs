using FluentValidation;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class PatientViewModelValidator<T> : AbstractValidator<T> where T : PatientViewModel
    {
        public PatientViewModelValidator(IFutureDateValidator futureDateValidator, INhsValidator nhsValidator, IClinicalSystemIdDescriptionProvider clinicalSystemIdDescriptionProvider)
        {
            RuleFor(model => model.ClinicalSystemId)
                .NotEmpty()
                .WithMessage(string.Format("{0} is mandatory", clinicalSystemIdDescriptionProvider.GetDescription()))
                .Length(1, 50)
                .WithMessage(string.Format("{0} must be less than 50 characters", clinicalSystemIdDescriptionProvider.GetDescription()));

            RuleFor(model => model.FirstName)
                .NotEmpty()
                .WithMessage("First name is mandatory")
                .Length(1, 50)
                .WithMessage("First name must be less than 50 characters");

            RuleFor(model => model.LastName)
                .NotEmpty()
                .WithMessage("Last name is mandatory")
                .Length(1, 50)
                .WithMessage("Last name must be less than 50 characters"); ;

            RuleFor(model => model.DateOfBirthViewModel)
                .NotNull()
                .SetValidator(new DateOfBirthViewModelValidator(futureDateValidator));

            RuleFor(model => model.GenderId)
                .NotEmpty()
                .WithMessage("Gender is mandatory");

            RuleFor(model => model.NhsNumber)
                .Must(nhsValidator.Valid)
                .WithMessage("NHS number is not valid");
        }
    }
}