using System;
using System.Web;
using System.Web.Routing;

namespace Sfw.Sabp.Mca.Infrastructure.Constraints
{
    public class NullableGuidConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var value = values[parameterName].ToString();
            Guid guid;
            return string.IsNullOrEmpty(value) || Guid.TryParse(value, out guid);
        }
    }
}
