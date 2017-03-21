using System.Web.Routing;

namespace Sfw.Sabp.Mca.Infrastructure.Web.Helper
{
    public interface IUrlHelper
    {
        string RouteUrl(RouteValueDictionary routeValues);
    }
}
