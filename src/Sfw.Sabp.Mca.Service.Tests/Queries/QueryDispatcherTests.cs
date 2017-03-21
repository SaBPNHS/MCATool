using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Sfw.Sabp.Mca.Core.Contracts;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Tests.Queries
{
    [TestClass]
    public class QueryDispatcherTests
    {
        private IKernel _kernel;
        private QueryDispatcher _dispatcher;
        private IQueryHandler<FakeQuery, FakeResult> _handler;

        [TestInitialize]
        public void Setup()
        {
            _kernel = new StandardKernel();

            _handler = A.Fake<IQueryHandler<FakeQuery, FakeResult>>();
            _kernel.Bind<IQueryHandler<FakeQuery, FakeResult>>().ToConstant(_handler);

            _dispatcher = new QueryDispatcher(_kernel);
        }

        [TestMethod]
        public void Retrieve_GivenHandlerNotMapped_ActivationExceptionShouldBeThrown()
        {
            _kernel.Unbind<IQueryHandler<FakeQuery, FakeResult>>();

            _dispatcher.Invoking(x => x.Dispatch<FakeQuery, FakeResult>(A<FakeQuery>._)).ShouldThrow<ActivationException>();
        }

        [TestMethod]
        public void Retrieve_GivenHandlerMapped_DispatcherRetrieveShouldBeCalled()
        {
            var query = GetFakeQuery();

            _dispatcher.Dispatch<FakeQuery, FakeResult>(query);

            A.CallTo(() => _handler.Retrieve(query)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Retrieve_GivenHandlerIsMapped_DispatcherShouldReturnResultsFromQuery()
        {
            var query = GetFakeQuery();
            var guid = Guid.NewGuid();
            query.Id = guid;

            var fakeResult = new FakeResult();
            A.CallTo(() => _handler.Retrieve(query)).Returns(fakeResult);

            var result = _dispatcher.Dispatch<FakeQuery, FakeResult>(query);

            result.ShouldBeEquivalentTo(fakeResult);
        }

        #region private

        private FakeQuery GetFakeQuery()
        {
            return new FakeQuery();
        }

        public class FakeQuery : IQuery
        {
            public Guid Id { get; set; }
        }

        public class FakeResult : IQueryResult
        {
            
        }

        #endregion
    }
}
