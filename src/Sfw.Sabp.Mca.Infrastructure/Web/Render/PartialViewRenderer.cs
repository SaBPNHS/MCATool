using System.IO;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Infrastructure.Web.Render
{
    public class PartialViewRenderer : IPartialViewRenderer
    {
        public string RenderPartialViewToString(ControllerContext context, ViewDataDictionary viewData, TempDataDictionary tempData, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            viewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
