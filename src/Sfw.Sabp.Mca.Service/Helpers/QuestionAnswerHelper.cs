using System;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public class QuestionAnswerHelper : IQuestionAnswerHelper
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public QuestionAnswerHelper(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        public void RemoveQuestionAnswer(QuestionAnswer questionAnswer)
        {
            _commandDispatcher.Dispatch(new RemoveAnswerCommand { QuestionAnswerId = questionAnswer.QuestionAnswerId });
        }

        public void AddQuestionAnswer(Guid questionOptionId, string furtherInformation, Assessment assessment)
        {
            _commandDispatcher.Dispatch(new AddQuestionAnswerCommand
            {
                AssessmentId = assessment.AssessmentId,
                QuestionOptionId = questionOptionId,
                WorkflowQuestionId = assessment.CurrentWorkflowQuestionId,
                FurtherInformation = furtherInformation
            });
        }

        public QuestionAnswer GetQuestionAnswer(Assessment assessment)
        {
            return _queryDispatcher.Dispatch<QuestionAnswerByAssessmentAndQuestionIdQuery, QuestionAnswer>(new QuestionAnswerByAssessmentAndQuestionIdQuery()
            {
                AssessmentId = assessment.AssessmentId,
                WorkflowQuestionId = assessment.CurrentWorkflowQuestionId
            });
        }

        public QuestionAnswer GetQuestionAnswer(Guid questionAnswerid)
        {
            return _queryDispatcher.Dispatch<QuestionAnswerByIdQuery, QuestionAnswer>(new QuestionAnswerByIdQuery()
            {
                QuestionAnswerId = questionAnswerid
            });
        }

        public void UpdateQuestionAnswer(Guid questionAnswerId, string furtherInformation)
        {
            _commandDispatcher.Dispatch(new UpdateQuestionAnswerCommand()
            {
                QuestionAnswerId = questionAnswerId,
                FurtherInformation = furtherInformation
            });
        }
    }
}
