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
    public class WorkflowVersionByIdQueryHandlerTests
    {
        private WorkflowVersionByIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new WorkflowVersionByIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenWorkflowVersionByIdIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<WorkflowVersionByIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenWorkflowVersionByIdQuery_ContextShouldBeQueriedAndWorkflowVersionReturned()
        {
            var id = Guid.NewGuid();

            var set = new TestDbSet<WorkflowVersion> {new WorkflowVersion() {WorkflowVersionId = id}};

            A.CallTo(() => _fakeContext.Set<WorkflowVersion>()).Returns(set);

            var query = new WorkflowVersionByIdQuery() {WorkflowVersionId = id};

            var workflowVersion = _queryHandler.Retrieve(query);

            workflowVersion.Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_GivenWorkflowVersionByIdQueryAndItemDoesNotExist_ContextShouldBeQueriedAndNullReturned()
        {
            var id = Guid.NewGuid();

            var set = new TestDbSet<WorkflowVersion> { new WorkflowVersion() };

            A.CallTo(() => _fakeContext.Set<WorkflowVersion>()).Returns(set);

            var query = new WorkflowVersionByIdQuery() { WorkflowVersionId = id };

            var workflowVersion = _queryHandler.Retrieve(query);

            workflowVersion.Should().BeNull();
        }
    }
}
