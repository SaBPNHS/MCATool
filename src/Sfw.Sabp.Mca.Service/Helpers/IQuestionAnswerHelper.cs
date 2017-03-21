using System;
using Sfw.Sabp.Mca.Model;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public interface IQuestionAnswerHelper
    {
        void RemoveQuestionAnswer(QuestionAnswer questionAnswer);
        void AddQuestionAnswer(Guid questionOptionId, string furtherInformation, Assessment assessment);
        QuestionAnswer GetQuestionAnswer(Assessment assessment);
        QuestionAnswer GetQuestionAnswer(Guid questionAnswerId);
        void UpdateQuestionAnswer(Guid questionAnswerId, string furtherInformation);
    }
}