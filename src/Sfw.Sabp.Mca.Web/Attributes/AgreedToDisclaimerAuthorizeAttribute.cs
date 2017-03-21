using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AgreedToDisclaimerAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IUserPrincipalProvider _userPrincipalProvider;
        private readonly IQueryDispatcher _queryDispatcher;

        public AgreedToDisclaimerAuthorizeAttribute(IUserPrincipalProvider userPrincipalProvider, IQueryDispatcher queryDispatcher)
        {
            _userPrincipalProvider = userPrincipalProvider;
            _queryDispatcher = queryDispatcher;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var disclaimer = _queryDispatcher.Dispatch<DisclaimerByUserQuery, Disclaimer>(new DisclaimerByUserQuery()
            {
                AssessorDomainName = _userPrincipalProvider.CurrentUserName
            });

            return disclaimer !=null && disclaimer.IsAgreed;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", MVC.Home.Name },
                    { "action", MVC.Home.ActionNames.Index }
                }
            );
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AgreedToDisclaimerAuthorizeAttributeNinject : Attribute { }
}