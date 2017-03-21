using System.Web;
using System.Web.Routing;

namespace Sfw.Sabp.Mca.Infrastructure.Web.Helper
{
    public class UrlHelper : IUrlHelper
    {
        public string RouteUrl(RouteValueDictionary routeValues)
        {
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);

            return urlHelper.RouteUrl(routeValues);
        }
    }
}
