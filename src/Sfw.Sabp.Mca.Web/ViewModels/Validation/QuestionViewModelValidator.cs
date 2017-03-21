using System.Linq;
using FluentValidation;

namespace Sfw.Sabp.Mca.Web.ViewModels.Validation
{
    public class QuestionViewModelValidator : AbstractValidator<QuestionViewModel>
    {
        public QuestionViewModelValidator()
        {
            When(m => m.Options!=null && m.Options.Any(), () => RuleFor(m => m.ChosenOption)
                .NotEmpty()
                .WithMessage("An option must be selected"));

            When(m =>
            {
                if (m.Options != null)
                {
                    var option = m.Options.FirstOrDefault(x => x.QuestionOptionId == m.ChosenOption);
                    return option != null && option.HasFurterQuestion && m.DisplayFurtherInformationQuestion;
                }
                return false;
            },() => RuleFor(m => m.FurtherInformationAnswer)
                        .NotEmpty()
                        .WithMessage("Please enter any further information"));
        }
    }
}