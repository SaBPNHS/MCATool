using FluentValidation;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Web.ViewModels.Custom;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class AssessmentViewModelValidator : AbstractValidator<AssessmentViewModel>
    {
        public AssessmentViewModelValidator(IFutureDateValidator futureDateValidator)
        {
            RuleFor(model => model.Stage1DecisionToBeMade)
                .NotEmpty()
                .WithMessage("Decision to be made is mandatory")
                .Length(1, 1000)
                .WithMessage("Decision must be less than 1000 characters");

            RuleFor(model => model.DateAssessmentStarted)
                .Must(futureDateValidator.Valid)
                .WithMessage("Date assessment started must not be in the future");

            RuleFor(model => model.Stage1DecisionClearlyMade)
                .NotEqual(x => false)
                .WithMessage("Please confirm that the decision is clearly defined");

            RuleFor(model => model.RoleId)
                .NotEmpty()
                .WithMessage(ApplicationStringConstants.RoleIdMandatory);

            When(m => m.RoleId == (int)RoleIdEnum.Assessor, () => RuleFor(m => m.DecisionMaker)
                .NotEmpty()
                .WithMessage("Decision maker's name is mandatory")
                .Length(1, 50)
                .WithMessage("Decision maker's name must be less than 50 characters"));
        }
    }
}