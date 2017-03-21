using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Tests.Helpers
{
    [TestClass]
    public class QuestionAnswerHelpersTests
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private QuestionAnswerHelper _questionAnswerHelper;

        [TestInitialize]
        public void Setup()
        {
            _commandDispatcher = A.Fake<ICommandDispatcher>();
            _queryDispatcher = A.Fake<IQueryDispatcher>();

            _questionAnswerHelper = new QuestionAnswerHelper(_commandDispatcher, _queryDispatcher);
        }

        [TestMethod]
        public void RemoveQuestionAnswer_GivenQuestionAnswer_CommandDispatcherShouldBeCalled()
        {
            var questionAnswer = new QuestionAnswer()
            {
                QuestionAnswerId = Guid.NewGuid()
            };

            _questionAnswerHelper.RemoveQuestionAnswer(questionAnswer);

            A.CallTo(() => _commandDispatcher.Dispatch(A<RemoveAnswerCommand>.That.Matches(x => x.QuestionAnswerId == questionAnswer.QuestionAnswerId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void AddQuestionAnswer_GivenInputs_CommandDispatcherShouldBeCalled()
        {
            var questionOptionId = Guid.NewGuid();
            const string furtherInfo = "furtherInfo";
            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                CurrentWorkflowQuestionId = Guid.NewGuid()
            };

            _questionAnswerHelper.AddQuestionAnswer(questionOptionId, furtherInfo, assessment);

            A.CallTo(() => _commandDispatcher.Dispatch(A<AddQuestionAnswerCommand>.That.Matches(x => x.AssessmentId == assessment.AssessmentId &&
                x.FurtherInformation == furtherInfo &&
                x.QuestionOptionId == questionOptionId &&
                x.WorkflowQuestionId == assessment.CurrentWorkflowQuestionId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void GetQuestionAnswer_GivenAssessment_QueryDispatcherShouldBeCalled()
        {
            var assessment = new Assessment()
            {
                AssessmentId = Guid.NewGuid(),
                CurrentWorkflowQuestionId = Guid.NewGuid()
            };

            _questionAnswerHelper.GetQuestionAnswer(assessment);

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswerByAssessmentAndQuestionIdQuery, QuestionAnswer>(A<QuestionAnswerByAssessmentAndQuestionIdQuery>.That.Matches(x => x.AssessmentId == assessment.AssessmentId &&
                x.WorkflowQuestionId == assessment.CurrentWorkflowQuestionId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void UpdateQuestionAnswer_GivenQuestionAnswerUpdates_CommandDispatcherShouldBeCalled()
        {
            var questionAnswerId = Guid.NewGuid();
            const string furtherInfo = "further";

            _questionAnswerHelper.UpdateQuestionAnswer(questionAnswerId, furtherInfo);

            A.CallTo(() => _commandDispatcher.Dispatch(A<UpdateQuestionAnswerCommand>.That.Matches(x => x.QuestionAnswerId == questionAnswerId &&
                x.FurtherInformation == furtherInfo))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void GetQuestionAnswer_GivenQuestionAnswerId_QueryDispatcherShouldBeCalled()
        {
            var questionAnswerId = Guid.NewGuid();

            _questionAnswerHelper.GetQuestionAnswer(questionAnswerId);

            A.CallTo(() => _queryDispatcher.Dispatch<QuestionAnswerByIdQuery, QuestionAnswer>(A<QuestionAnswerByIdQuery>.That.Matches(x => x.QuestionAnswerId == questionAnswerId))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
