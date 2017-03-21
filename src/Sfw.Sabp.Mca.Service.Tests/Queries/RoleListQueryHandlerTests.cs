using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Service.Tests.Helpers;
using System;
using System.Data.Entity;
using System.Linq;

namespace Sfw.Sabp.Mca.Service.Tests.Queries
{
    [TestClass]
    public class RoleListQueryHandlerTests
    {
        private RoleListQueryHandler _queryHandler;
        private IUnitOfWork _unitOfWork;
        private DbContext _fakeContext;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _fakeContext = A.Fake<DbContext>();

            A.CallTo(() => _unitOfWork.Context).Returns(_fakeContext);

            _queryHandler = new RoleListQueryHandler(_unitOfWork);
        }

        [TestMethod]
        public void Retrieve_GivenRoleListQueryIsNull_ArgumentNullExceptionExpected()
        {
            _queryHandler.Invoking(x => x.Retrieve(A<RoleListQuery>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Retrieve_GivenRoleListQuery_ContextShouldBeQueriedAndRolesReturned()
        {
            var id = 1;

            var set = new TestDbSet<Role> {new Role{RoleId = id, Description = "First"}};

            A.CallTo(() => _fakeContext.Set<Role>()).Returns(set);

            var query = new RoleListQuery { RoleId = id };

            var role = _queryHandler.Retrieve(query);

            role.Should().NotBeNull();
            role.Items.Count().Should().Be(1);
        }

        [TestMethod]
        public void Retrieve_GivenRoleListQueryAndNoItemsPresent_ContextShouldBeQueriedAndNumberOfRolesReturned()
        {

            var set = new TestDbSet<Role>();

            A.CallTo(() => _fakeContext.Set<Role>()).Returns(set);

            var query = new RoleListQuery();

            var role = _queryHandler.Retrieve(query);

            role.Items.Count().Should().Be(0);
        }
    }
}
