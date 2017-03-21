using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Infrastructure.Web.Render
{
    public interface IPartialViewRenderer
    {
        string RenderPartialViewToString(ControllerContext context, ViewDataDictionary viewData, TempDataDictionary tempData, string viewName, object model);
    }
}
