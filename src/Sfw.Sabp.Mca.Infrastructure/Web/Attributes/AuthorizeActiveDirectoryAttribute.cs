using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Infrastructure.Providers;

namespace Sfw.Sabp.Mca.Infrastructure.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeActiveDirectoryAttribute : AuthorizeAttribute
    {
        private readonly IWindowsTokenRoleProviderWrapper _windowsTokenRoleProviderWrapper;
        private readonly IUserPrincipalProvider _userPrincipalProvider;

        private string _groups;

        public string Groups
        {
            get { return _groups; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _groups = value;
            }
        }

        public AuthorizeActiveDirectoryAttribute(IWindowsTokenRoleProviderWrapper windowsTokenRoleProviderWrapper, IUserPrincipalProvider userPrincipalProvider)
        {
            _windowsTokenRoleProviderWrapper = windowsTokenRoleProviderWrapper;
            _userPrincipalProvider = userPrincipalProvider;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext)) return false;

            if (String.IsNullOrEmpty(_groups))
                return true;

            var groups = _groups.Split(',').ToList();

            return groups.Any(@group => _windowsTokenRoleProviderWrapper.IsUserInRole(_userPrincipalProvider.CurrentUserName, @group));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            throw new HttpException((int)HttpStatusCode.Forbidden, filterContext.HttpContext.User.Identity.Name);
        }
    }
}
