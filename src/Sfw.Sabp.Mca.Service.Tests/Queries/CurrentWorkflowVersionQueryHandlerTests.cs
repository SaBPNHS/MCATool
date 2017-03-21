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
    public class CurrentWorkflowVersionQueryHandlerTests
    {
        private CurrentWorkflowVersionQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new CurrentWorkflowVersionQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenCurrentWorkflowQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<CurrentWorkflowQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenCurrentWorkflowQueryReturnsInvalidResult_InvalidCurrentWorkflowExceptionExpected()
        {
            var set = new TestDbSet<WorkflowVersion> { new WorkflowVersion(), new WorkflowVersion() };

            A.CallTo(() => _fakeContext.Set<WorkflowVersion>()).Returns(set);

            _queryHandler.Invoking(x => x.Retrieve(new CurrentWorkflowQuery())).ShouldThrow<InvalidCurrentWorkflowException>();
        }

        [TestMethod]
        public void Retrieve_GivenCurrentWorkflowQueryReturnsValidResult_CurrentWorkflowVersionShouldBeReturned()
        {
            var workflowVersionId = Guid.NewGuid();

            var set = new TestDbSet<WorkflowVersion> { new WorkflowVersion() { WorkflowVersionId = workflowVersionId } };

            A.CallTo(() => _fakeContext.Set<WorkflowVersion>()).Returns(set);

            var workflowVersion = _queryHandler.Retrieve(new CurrentWorkflowQuery());

            workflowVersion.Should().NotBeNull();
            workflowVersion.WorkflowVersionId.Should().Be(workflowVersionId);
        }
    }
}
