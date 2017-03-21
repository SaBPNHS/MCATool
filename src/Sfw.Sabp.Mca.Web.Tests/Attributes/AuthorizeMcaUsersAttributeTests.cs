using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Infrastructure.Configuration;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Attributes;
using FakeItEasy;

namespace Sfw.Sabp.Mca.Web.Tests.Attributes
{
    [TestClass]
    public class AuthorizeOps1UsersAttributeTest
    {
        private AuthorizeMcaUsersAttribute _authoriseMcaUsersAttribute;
        private AuthorizationContext _authorisationContext;
        private IWindowsTokenRoleProviderWrapper _windowsTokenRoleProviderWrapper;
        private IUserPrincipalProvider _userPrincipalProvider;
        private IConfigurationManagerWrapper _configurationManagerWrapper;
        private const string GroupName = "mca";

        [TestInitialize]
        public void Initialise()
        {
            _windowsTokenRoleProviderWrapper = A.Fake<IWindowsTokenRoleProviderWrapper>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();
            _configurationManagerWrapper = ConfigurationManagerWrapper(GroupName);

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
            var controllerDescriptorMock = A.Fake<ControllerDescriptor>();
            A.CallTo(() => actionDescriptor.ControllerDescriptor).Returns(controllerDescriptorMock);
            A.CallTo(() => actionDescriptor.ActionName).Returns("action");

            var controller = A.Fake<ControllerBase>();

            var controllercontext = new ControllerContext(new RequestContext(context, new RouteData()), controller);

            _authorisationContext = new AuthorizationContext(controllercontext, actionDescriptor);

            _authoriseMcaUsersAttribute = new AuthorizeMcaUsersAttribute(_configurationManagerWrapper, _windowsTokenRoleProviderWrapper, _userPrincipalProvider);
        }

        [TestMethod]
        public void AuthorizeCore_GivenEmptyActiveDirectoryGroup_ExpectedConfigurationErrorsException()
        {
            var emptyConfigurationManager = A.Fake<IConfigurationManagerWrapper>();
            _authoriseMcaUsersAttribute = new AuthorizeMcaUsersAttribute(emptyConfigurationManager, _windowsTokenRoleProviderWrapper, _userPrincipalProvider);

            _authoriseMcaUsersAttribute.Invoking(x => x.OnAuthorization(_authorisationContext)).ShouldThrow<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void AuthorizeCore_GivenUnauthorisedUser_UnauthorisedViewShouldBeReturned()
        {
            const string userName = "user";
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(userName);
            A.CallTo(() => _windowsTokenRoleProviderWrapper.IsUserInRole(userName, GroupName)).Returns(false);

            _authoriseMcaUsersAttribute.OnAuthorization(_authorisationContext);
            var result = _authorisationContext.Result as ViewResult;

            result.ViewName.Should().BeSameAs(MVC.Shared.Views.Unauthorised);
        }

        [TestMethod]
        public void AuthorizeCore_GivenAuthorisedUser_UserShouldBeAuthorised()
        {
            const string userName = "user";
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(userName);
            A.CallTo(() => _windowsTokenRoleProviderWrapper.IsUserInRole(userName, GroupName)).Returns(true);

            _authoriseMcaUsersAttribute.OnAuthorization(_authorisationContext);
            
            _authorisationContext.Result.Should().BeNull();
        }

        private IConfigurationManagerWrapper ConfigurationManagerWrapper(string name)
        {
            var configuration = A.Fake<IConfigurationManagerWrapper>();
            var collection = new NameValueCollection();

            if (name != null)
            {
                collection.Add(new NameValueCollection() { { ApplicationSettingConstants.McaActiveDirectoryGroup, name } });
            }

            A.CallTo(() => configuration.AppSettings).Returns(collection);

            return configuration;
        }
    }
}
