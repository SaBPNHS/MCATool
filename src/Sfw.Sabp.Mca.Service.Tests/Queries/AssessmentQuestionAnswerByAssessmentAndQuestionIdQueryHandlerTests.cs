using System;
using System.Data.Entity;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Service.Tests.Helpers;

namespace Sfw.Sabp.Mca.Service.Tests.Queries
{
    [TestClass]
    public class AssessmentQuestionAnswerByAssessmentAndQuestionIdQueryHandlerTests
    {
        private QuestionAnswerByAssessmentAndQuestionIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new QuestionAnswerByAssessmentAndQuestionIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentQuestionAnswerByAssessmentAndQuestionIdQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<QuestionAnswerByAssessmentAndQuestionIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentQuestionAnswerByAssessmentAndQuestionIdQuery_ContextShouldBeQueriedAndAssessmentReturned()
        {
            var assessmentId = Guid.NewGuid();
            var workflowQuestionId = Guid.NewGuid();

            var set = new TestDbSet<QuestionAnswer> { new QuestionAnswer() { AssessmentId = assessmentId, WorkflowQuestionId = workflowQuestionId} };

            A.CallTo(() => _fakeContext.Set<QuestionAnswer>()).Returns(set);

            var query = new QuestionAnswerByAssessmentAndQuestionIdQuery() { AssessmentId = assessmentId, WorkflowQuestionId = workflowQuestionId};

            var questionAnswer = _queryHandler.Retrieve(query);

            questionAnswer.Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_AssessmentQuestionAnswerByAssessmentAndQuestionIdQueryAndItemDoesNotExist_ContextShouldBeQueriedAndNullReturned()
        {
            var assessmentId = Guid.NewGuid();
            var workflowQuestionId = Guid.NewGuid();

            var set = new TestDbSet<QuestionAnswer> { new QuestionAnswer() };

            A.CallTo(() => _fakeContext.Set<QuestionAnswer>()).Returns(set);

            var query = new QuestionAnswerByAssessmentAndQuestionIdQuery() { AssessmentId = assessmentId, WorkflowQuestionId = workflowQuestionId };

            var questionAnswer = _queryHandler.Retrieve(query);

            questionAnswer.Should().BeNull();
        }
    }
}
