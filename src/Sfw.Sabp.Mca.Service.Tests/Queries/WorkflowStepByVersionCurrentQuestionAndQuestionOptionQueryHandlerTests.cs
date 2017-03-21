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
    public class WorkflowStepByVersionCurrentQuestionAndQuestionOptionQueryHandlerTests
    {
        private WorkflowStepByVersionAndQuestionOptionQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new WorkflowStepByVersionAndQuestionOptionQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenAWorkflowStepByVersionAndQuestionOptionQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenWorkflowStepByVersionAndQuestionOptionQuery_WorkflowStepShouldBeReturnedReturned()
        {
            var workflowVersionId = Guid.NewGuid();
            var questionOptionId = Guid.NewGuid();
            var currentQuestionId = Guid.NewGuid();
            var alterQuestionId = Guid.NewGuid();
            var nextQuestionId = Guid.NewGuid();

            var set = new TestDbSet<WorkflowStep>
            {
                new WorkflowStep() {WorkflowVersionId = workflowVersionId, QuestionOptionId = questionOptionId, CurrentWorkflowQuestionId =  alterQuestionId},
                new WorkflowStep() {WorkflowVersionId = workflowVersionId, QuestionOptionId = questionOptionId, CurrentWorkflowQuestionId =  currentQuestionId, NextWorkflowQuestionId = nextQuestionId}
            };

            A.CallTo(() => _fakeContext.Set<WorkflowStep>()).Returns(set);

            var query = new WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery() { WorkflowVersionId = workflowVersionId, QuestionOptionId = questionOptionId, CurrentWorkflowQuestionId = currentQuestionId };

            var workflowStep = _queryHandler.Retrieve(query);

            workflowStep.Should().NotBeNull();
            workflowStep.NextWorkflowQuestionId.ShouldBeEquivalentTo(nextQuestionId);
        }


    }
}
