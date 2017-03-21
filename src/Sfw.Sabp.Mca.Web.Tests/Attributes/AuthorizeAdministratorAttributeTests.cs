using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Attributes;

namespace Sfw.Sabp.Mca.Web.Tests.Attributes
{
    [TestClass]
    public class AuthorizeAdministratorAttributeTests
    {
        private IUserRoleProvider _userRoleProvider;
        private AuthorizeAdministratorAttribute _authorizeAdministratorAttribute;
        private AuthorizationContext _filterContext;

        [TestInitialize]
        public void Setup()
        {
            _userRoleProvider = A.Fake<IUserRoleProvider>();

            var controllerContext = A.Fake<ControllerContext>();
            var actionDescriptor = A.Fake<ActionDescriptor>();

            _filterContext = new AuthorizationContext(controllerContext, actionDescriptor);
        }

        [TestMethod]
        public void AuthorizeCore_GivenUserIsNotAdministrator_UserShouldBeDirectedToUnauthorizedView()
        {
            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(false);

            _authorizeAdministratorAttribute = new AuthorizeAdministratorAttribute(_userRoleProvider);
            _authorizeAdministratorAttribute.OnAuthorization(_filterContext);

            var result = _filterContext.Result as RedirectToRouteResult;

            AssertPersonRoute(result);
        }

        [TestMethod]
        public void AuthorizeCore_GivenUserIsAdministrator_UserShouldHaveAccess()
        {
            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(true);

            _authorizeAdministratorAttribute = new AuthorizeAdministratorAttribute(_userRoleProvider);
            _authorizeAdministratorAttribute.OnAuthorization(_filterContext);

            var result = _filterContext.Result as ViewResult;

            result.Should().BeNull();
        }

        #region private

        private void AssertPersonRoute(RedirectToRouteResult result)
        {
            result.RouteValues["action"].Should().Be(MVC.Person.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Person.Name);
        }

        #endregion
    }
}
