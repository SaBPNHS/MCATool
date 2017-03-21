using System;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IQuestionViewModelBuilder 
    {
        QuestionViewModel BuildQuestionViewModel(Assessment assessment, QuestionAnswer questionAnswer);
        QuestionViewModel UpdateQuestionViewModel(QuestionViewModel viewModel, Guid chosenValue);
    }
}
