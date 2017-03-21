using System.Web;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sfw.Sabp.Mca.Web.Tests.Routes
{
    [TestClass]
    public class QuestionRoutesTests
    {
        private RouteCollection _routes;

        [TestInitialize]
        public void Setup()
        {
            _routes = new RouteCollection();
            RouteConfig.RegisterRoutes(_routes);
        }

        [TestMethod]
        public void DefaultQuestionRoute_ShouldMapToQuestionIndexAction()
        {
            var httpContext = HttpContextBase("~/Question/");

            var routeData = _routes.GetRouteData(httpContext);

            AssertRouteValues(routeData);
        }

        [TestMethod]
        public void DefaultQuestionRoute_GivenAssessmentId_ShouldMapToQuestionIndexActionWithParameter()
        {
            const string id = "4364CDC5-2863-457A-B4D2-E9EFB9B7A24A";

            var httpContext = HttpContextBase(string.Format("~/Question/{0}", id));

            var routeData = _routes.GetRouteData(httpContext);

            AssertRouteValues(routeData);

            routeData.Values["assessmentId"].Should().Be(id);
        }

        [TestMethod]
        public void ActionQuestionRoute_GivenAssessmentId_ShouldMapToQuestionActionWithParameter()
        {
            const string id = "4364CDC5-2863-457A-B4D2-E9EFB9B7A24A";

            var httpContext = HttpContextBase(string.Format("~/Question/Edit/{0}", id));

            var routeData = _routes.GetRouteData(httpContext);

            routeData.Values["action"].Should().Be(MVC.Question.ActionNames.Edit);
            routeData.Values["controller"].Should().Be(MVC.Question.Name);
            routeData.Values["assessmentId"].Should().Be(id);
        }

        [TestMethod]
        public void DefaultQuestionRoute_GivenInvalidAssessmentId_RouteShouldNotContainAssessementId()
        {
            const string id = "1";

            var httpContext = HttpContextBase(string.Format("~/Question/{0}", id));

            var routeData = _routes.GetRouteData(httpContext);

            routeData.Values.Should().NotContainKey("assessmentId");
        }

        #region private

        private void AssertRouteValues(RouteData routeData)
        {
            routeData.Values["action"].Should().Be(MVC.Question.ActionNames.Index);
            routeData.Values["controller"].Should().Be(MVC.Question.Name);
        }

        private HttpContextBase HttpContextBase(string path)
        {
            var httpContext = A.Fake<HttpContextBase>();
            A.CallTo(() => httpContext.Request.AppRelativeCurrentExecutionFilePath).Returns(path);
            return httpContext;
        }

        #endregion
    }
}
