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
    public class WorkflowStageByIdQueryHandlerTests
    {
        private WorkflowStageByIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new WorkflowStageByIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenWorkflowStageByIdQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<WorkflowStageByIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenWorkflowStageByIdQuery_ContextShouldBeQueriedAndAssessmentReturned()
        {
            var workflowStageId = Guid.NewGuid();

            var set = new TestDbSet<WorkflowStage> { new WorkflowStage() { WorkflowStageId = Guid.NewGuid() }, new WorkflowStage() { WorkflowStageId = workflowStageId } };

            A.CallTo(() => _fakeContext.Set<WorkflowStage>()).Returns(set);

            var query = new WorkflowStageByIdQuery() { WorkflowStageId = workflowStageId };

            var workflowStage = _queryHandler.Retrieve(query);

            workflowStage.Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_GivenGivenWorkflowStageByIdQueryAndItemDoesNotExist_ContextShouldBeQueriedAndNullReturned()
        {
            var set = new TestDbSet<WorkflowStage> { new WorkflowStage() };

            A.CallTo(() => _fakeContext.Set<WorkflowStage>()).Returns(set);

            var query = new WorkflowStageByIdQuery() { WorkflowStageId = Guid.NewGuid() };

            var workflowStage = _queryHandler.Retrieve(query);

            workflowStage.Should().BeNull();
        }
    }
}
