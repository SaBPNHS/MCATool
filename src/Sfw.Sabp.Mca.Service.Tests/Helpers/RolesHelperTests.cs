using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Tests.Helpers
{
    [TestClass]
    public class RolesHelperTests
    {
        private IQueryDispatcher _queryDispatcher;

        private RoleHelper _roleHelper;

        [TestInitialize]
        public void Setup()
        {
            _queryDispatcher = A.Fake<IQueryDispatcher>();

            _roleHelper = new RoleHelper(_queryDispatcher);
        }

        [TestMethod]
        public void GetRoles_RolesShouldBeReturned()
        {
            A.CallTo(() => _queryDispatcher.Dispatch<RoleListQuery, Roles>(A<RoleListQuery>._)).Returns(new Roles());

            var result = _roleHelper.GetRoles();

            result.Should().BeOfType<Roles>();
        }

        [TestMethod]
        public void GetRoles_QueryDispatcherShouldBeCalled()
        {
            _roleHelper.GetRoles();

            A.CallTo(() => _queryDispatcher.Dispatch<RoleListQuery, Roles>(A<RoleListQuery>._)).MustHaveHappened();
        }
    }
}
