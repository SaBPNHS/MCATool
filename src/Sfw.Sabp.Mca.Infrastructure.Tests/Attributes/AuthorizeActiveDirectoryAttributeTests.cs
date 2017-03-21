using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Infrastructure.Web.Attributes;

namespace Sfw.Sabp.Mca.Infrastructure.Tests.Attributes
{
    [TestClass]
    public class AuthorizeActiveDirectoryAttributeTests
    {
        private AuthorizeActiveDirectoryAttribute _authorizeActiveDirectoryAttribute;
        private IWindowsTokenRoleProviderWrapper _windowsTokenRoleProviderWrapper;
        private IUserPrincipalProvider _userPrincipalProvider;
        private AuthorizationContext _authorisationContext;

        [TestInitialize]
        public void Initialise()
        {
            _windowsTokenRoleProviderWrapper = A.Fake<IWindowsTokenRoleProviderWrapper>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();

            var context = A.Fake<HttpContextBase>();
            var request = A.Fake<HttpRequestBase>();
            var response = A.Fake<HttpResponseBase>();
            var session = A.Fake<HttpSessionStateBase>();
            var server = A.Fake<HttpServerUtilityBase>();
            var cache = A.Fake<HttpCachePolicyBase>();

            A.CallTo(() => context.Items).Returns(new Dictionary<string, string>());
            A.CallTo(() => request.Form).Returns(new NameValueCollection());
            A.CallTo(() => request.QueryString).Returns(new NameValueCollection());
            A.CallTo(() => response.Cache).Returns(cache);
            A.CallTo(() => context.Request).Returns(request);
            A.CallTo(() => context.Response).Returns(response);
            A.CallTo(() => context.Session).Returns(session);
            A.CallTo(() => context.Server).Returns(server);

            var principal = new GenericPrincipal(new GenericIdentity("user"), new string[] { });
            A.CallTo(() => context.User).Returns(principal);

            var actionDescriptor = A.Fake<ActionDescriptor>();
            var controller = A.Fake<ControllerBase>();
            var controllercontext = new ControllerContext(new RequestContext(context, new RouteData()), controller);

            _authorisationContext = new AuthorizationContext(controllercontext, actionDescriptor);

            _authorizeActiveDirectoryAttribute = new AuthorizeActiveDirectoryAttribute(_windowsTokenRoleProviderWrapper, _userPrincipalProvider);
        }

        [TestMethod]
        public void AuthorizeCore_GivenEmptyGroupProperty_AuthorisationShouldSucceed()
        {
            _authorizeActiveDirectoryAttribute.Groups = string.Empty;

            _authorizeActiveDirectoryAttribute.OnAuthorization(_authorisationContext);

            _authorisationContext.Result.Should().BeNull();
        }

        [TestMethod]
        public void AuthorizeCore_GivenPrincipalIsNotAMemberOfSpecifiedGroup_AuthorisationShouldFail()
        {
            const string groups = "group";
            _authorizeActiveDirectoryAttribute.Groups = groups;

            const string userName = "user";
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(userName);
            A.CallTo(() => _windowsTokenRoleProviderWrapper.IsUserInRole(userName, groups)).Returns(false);

            _authorizeActiveDirectoryAttribute.Invoking(x => x.OnAuthorization(_authorisationContext))
                .ShouldThrow<HttpException>()
                .Where(x => x.GetHttpCode() == 403);
        }

        [TestMethod]
        public void AuthorizeCore_GivenPrincipalIsAMemberOfSpecifiedGroup_AuthorisationShouldSucceed()
        {
            const string groups = "group";
            _authorizeActiveDirectoryAttribute.Groups = groups;

            const string userName = "user";
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(userName);
            A.CallTo(() => _windowsTokenRoleProviderWrapper.IsUserInRole(userName, groups)).Returns(true);

            _authorizeActiveDirectoryAttribute.OnAuthorization(_authorisationContext);

            _authorisationContext.Result.Should().BeNull();
        }

        [TestMethod]
        public void AuthorizeCore_GivenPrincipalIsAMemberOfSpecifiedGroups_AuthorisationShouldSucceed()
        {
            _authorizeActiveDirectoryAttribute.Groups = "group,group2";

            const string userName = "user";
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(userName);
            A.CallTo(() => _windowsTokenRoleProviderWrapper.IsUserInRole(userName, "group2")).Returns(true);

            _authorizeActiveDirectoryAttribute.OnAuthorization(_authorisationContext);

            _authorisationContext.Result.Should().BeNull();
        }

        [TestMethod]
        public void Groups_GivenNullValue_ArgumentNullExceptionExpected()
        {
            _authorizeActiveDirectoryAttribute.Invoking(x => x.Groups = null).ShouldThrow<ArgumentNullException>();
        }
    }
}
