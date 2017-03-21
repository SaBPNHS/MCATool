using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Attributes;

namespace Sfw.Sabp.Mca.Web.Tests.Attributes
{
    [TestClass]
    public class AgreedToDisclaimerAuthorizeAttributeTests
    {
        private AgreedToDisclaimerAuthorizeAttribute _agreedToDisclaimerAuthorizeAttribute;
        private IUserPrincipalProvider _userPrincipalProvider;
        private IQueryDispatcher _queryDispatcher;
        private AuthorizationContext _authorizationContext;

        [TestInitialize]
        public void Setup()
        {
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();
            _queryDispatcher = A.Fake<IQueryDispatcher>();

            var controllerContext = A.Fake<ControllerContext>();
            var actionDescriptor = A.Fake<ActionDescriptor>();

            _authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor);

            _agreedToDisclaimerAuthorizeAttribute = new AgreedToDisclaimerAuthorizeAttribute(_userPrincipalProvider, _queryDispatcher);
        }

        [TestMethod]
        public void AuthorizeCore_GivenDisclaimerHasBeenAgreed_UserShouldHaveAccess()
        {
            A.CallTo(() => _queryDispatcher.Dispatch<DisclaimerByUserQuery, Disclaimer>(A<DisclaimerByUserQuery>._))
                .Returns(new Disclaimer()
                {
                    IsAgreed = true
                });

            _agreedToDisclaimerAuthorizeAttribute.OnAuthorization(_authorizationContext);

            var result = _authorizationContext.Result as RedirectToRouteResult;

            result.Should().BeNull();
        }

        [TestMethod]
        public void AuthorizeCore_GivenDisclaimerHasNotBeenAgreed_UserShouldBeRedirectedToDisclaimerScreen()
        {
            A.CallTo(() => _queryDispatcher.Dispatch<DisclaimerByUserQuery, Disclaimer>(A<DisclaimerByUserQuery>._))
                .Returns(new Disclaimer()
                {
                    IsAgreed = false
                });

            _agreedToDisclaimerAuthorizeAttribute.OnAuthorization(_authorizationContext);

            AssertResult();
        }

        [TestMethod]
        public void AuthorizeCore_GivenDisclaimerIsNull_UserShouldBeRedirectedToDisclaimerScreen()
        {
            A.CallTo(() => _queryDispatcher.Dispatch<DisclaimerByUserQuery, Disclaimer>(A<DisclaimerByUserQuery>._)).Returns(null);

            _agreedToDisclaimerAuthorizeAttribute.OnAuthorization(_authorizationContext);

            AssertResult();
        }

        #region private

        private void AssertResult()
        {
            var result = _authorizationContext.Result as RedirectToRouteResult;

            result.RouteValues["controller"].Should().Be(MVC.Home.Name);
            result.RouteValues["action"].Should().Be(MVC.Home.ActionNames.Index);
        }

        #endregion
    }
}
