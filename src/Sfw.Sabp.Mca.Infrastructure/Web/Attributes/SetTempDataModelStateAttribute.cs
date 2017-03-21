using System;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Infrastructure.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SetTempDataModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            filterContext.Controller.TempData["ModelState"] = filterContext.Controller.ViewData.ModelState;
        }
    }
}
