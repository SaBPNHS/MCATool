using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sfw.Sabp.Mca.Infrastructure.Providers;

namespace Sfw.Sabp.Mca.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAdministratorAttributeNinject : Attribute { }

    public class AuthorizeAdministratorAttribute : AuthorizeAttribute
    {
        private readonly IUserRoleProvider _userRoleProvider;

        public AuthorizeAdministratorAttribute(IUserRoleProvider userRoleProvider)
        {
            _userRoleProvider = userRoleProvider;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return _userRoleProvider.CurrentUserInAdministratorRole();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
                {
                    {"controller", MVC.Person.Name},
                    {"action", MVC.Person.ActionNames.Index}
                });
        }
    }
}