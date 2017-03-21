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
    public class DisclaimerByUserQueryHandlerTests
    {
        private DisclaimerByUserQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new DisclaimerByUserQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenDisclaimerByUserQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<DisclaimerByUserQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenDisclaimerByUserQuery_ContextShouldBeQueriedAndDisclaimerReturned()
        {
            var assessorDomainName = "user";

            var set = new TestDbSet<Disclaimer> { new Disclaimer() { AssessorDomainName = "user" }, new Disclaimer() { AssessorDomainName = assessorDomainName } };

            A.CallTo(() => _fakeContext.Set<Disclaimer>()).Returns(set);

            var query = new DisclaimerByUserQuery() { AssessorDomainName= assessorDomainName};

            var disclaimer = _queryHandler.Retrieve(query);

            disclaimer.Should().NotBeNull();
        }

    }
}
