using System.Web.Mvc;
using System.Web.Routing;
using Sfw.Sabp.Mca.Infrastructure.Constraints;

namespace Sfw.Sabp.Mca.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("QuestionDefault",
                "Question/{assessmentId}",
                new { controller = MVC.Question.Name, action = MVC.Question.ActionNames.Index, assessmentId = UrlParameter.Optional},
                new { assessmentId = new GuidConstraint() },
                new []{ typeof(Controllers.QuestionController).Namespace}
            );

            routes.MapRoute("QuestionAction",
                "Question/{action}/{assessmentId}",
                new { controller = MVC.Question.Name, action = MVC.Question.ActionNames.Index, assessmentId = UrlParameter.Optional },
                new { assessmentId = new GuidConstraint() },
                new[] { typeof(Controllers.QuestionController).Namespace }
            );

            routes.MapRoute("Assessment",
               "Assessment/{action}/{id}",
               new { controller = MVC.Assessment.Name, action = MVC.Assessment.ActionNames.Restart, id = UrlParameter.Optional },
               new { id = new GuidConstraint() },
               new[] { typeof(Controllers.AssessmentController).Namespace }
           );

            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
