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
    public class AssessmentQuestionAnswersByWorkflowStageIdQueryHandler
    {
        private QuestionAnswersByWorkflowStageIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new QuestionAnswersByWorkflowStageIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentQuestionAnswersByWorkflowStageIdQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<QuestionAnswersByWorkflowStageIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentQuestionAnswersByWorkflowStageIdQuery_ContextShouldBeQueriedAndAssessmentReturned()
        {
            var workflowStageId = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();

            var set = new TestDbSet<QuestionAnswer>
            {
                new QuestionAnswer()
                {
                    AssessmentId = assessmentId,
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        WorkflowStageId = workflowStageId,
                        
                    }
                },
                new QuestionAnswer()
                {
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        WorkflowStageId = Guid.NewGuid()
                    }
                }
            };

            A.CallTo(() => _fakeContext.Set<QuestionAnswer>()).Returns(set);

            var query = new QuestionAnswersByWorkflowStageIdQuery() { WorkflowStageId = workflowStageId, AssessmentId = assessmentId };

            var answers = _queryHandler.Retrieve(query);

            answers.Items.Should().HaveCount(1);
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentQuestionAnswersByWorkflowStageIdQueryAndItemDoesNotExist_ContextShouldBeQueriedAndNullReturned()
        {
            var set = new TestDbSet<QuestionAnswer> { new QuestionAnswer() {Assessment = new Assessment(), WorkflowQuestion = new WorkflowQuestion() } };

            A.CallTo(() => _fakeContext.Set<QuestionAnswer>()).Returns(set);

            var query = new QuestionAnswersByWorkflowStageIdQuery() { WorkflowStageId = Guid.NewGuid(), AssessmentId = Guid.NewGuid()};

            var answers = _queryHandler.Retrieve(query);

            answers.Items.Should().HaveCount(0);
        }
    }
}
