using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Core.Constants;
using Sfw.Sabp.Mca.Infrastructure.Configuration;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Infrastructure.Web.Attributes;

namespace Sfw.Sabp.Mca.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeMcaUsersAttribute : AuthorizeActiveDirectoryAttribute
    {
        private readonly IConfigurationManagerWrapper _configurationManagerWrapper;

        public AuthorizeMcaUsersAttribute(IConfigurationManagerWrapper configurationManagerWrapper, IWindowsTokenRoleProviderWrapper windowsTokenRoleProviderWrapper, IUserPrincipalProvider userPrincipalProvider)
            : base(windowsTokenRoleProviderWrapper, userPrincipalProvider)
        {
            _configurationManagerWrapper = configurationManagerWrapper;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var mcaActiveDirectoryGroup = _configurationManagerWrapper.AppSettings[ApplicationSettingConstants.McaActiveDirectoryGroup];

            if (string.IsNullOrEmpty(mcaActiveDirectoryGroup))
                throw new ConfigurationErrorsException("McaActiveDirectoryGroup");            

            Groups = mcaActiveDirectoryGroup;

            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult { ViewName = MVC.Shared.Views.Unauthorised };
        }
    }
}
