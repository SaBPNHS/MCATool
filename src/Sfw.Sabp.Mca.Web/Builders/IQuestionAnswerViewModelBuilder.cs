using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IQuestionAnswerViewModelBuilder
    {
        QuestionAnswerListViewModel BuildQuestionAnswerListViewModel(QuestionAnswers questionAnswers);
    }
}
