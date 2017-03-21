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
    public class AssessmentByIdQueryHandlerTests
    {
        private AssessmentByIdQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new AssessmentByIdQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentByIdIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<AssessmentByIdQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentByIdQuery_ContextShouldBeQueriedAndAssessmentReturned()
        {
            var id = Guid.NewGuid();

            var set = new TestDbSet<Assessment> {new Assessment() {AssessmentId = id}};

            A.CallTo(() => _fakeContext.Set<Assessment>()).Returns(set);

            var query = new AssessmentByIdQuery() {AssessmentId = id};

            var assessment = _queryHandler.Retrieve(query);

            assessment.Should().NotBeNull();
        }

        [TestMethod]
        public void Retrieve_GivenAssessmentByIdQueryAndItemDoesNotExist_ContextShouldBeQueriedAndNullReturned()
        {
            var id = Guid.NewGuid();

            var set = new TestDbSet<Assessment> { new Assessment() };

            A.CallTo(() => _fakeContext.Set<Assessment>()).Returns(set);

            var query = new AssessmentByIdQuery() { AssessmentId = id };

            var assessment = _queryHandler.Retrieve(query);

            assessment.Should().BeNull();
        }
    }
}
