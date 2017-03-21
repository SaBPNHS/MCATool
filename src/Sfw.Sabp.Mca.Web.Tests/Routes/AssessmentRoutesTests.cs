using System.Web;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sfw.Sabp.Mca.Web.Tests.Routes
{
    public class AssessmentRoutesTests
    {
        private RouteCollection _routes;

        [TestInitialize]
        public void Setup()
        {
            _routes = new RouteCollection();
            RouteConfig.RegisterRoutes(_routes);
        }

        [TestMethod]
        public void AssessmentCreateRoute_ShouldMapToAssessmentCreateAction()
        {
            var httpContext = HttpContextBase("~/Question/");

            var routeData = _routes.GetRouteData(httpContext);

            AssertCreateRouteValues(routeData);
        }

        [TestMethod]
        public void AssessmentCreateRoute_GivenPatientId_ShouldMapToAssessmentCreateActionWithParameter()
        {
            const string id = "4364CDC5-2863-457A-B4D2-E9EFB9B7A24A";

            var httpContext = HttpContextBase(string.Format("~/Assessment/Create/{0}", id));

            var routeData = _routes.GetRouteData(httpContext);

            AssertCreateRouteValues(routeData);

            routeData.Values["patientId"].Should().Be(id);
        }

        [TestMethod]
        public void AssessmentIndexRoute_GivenPatientId_ShouldMapToAssessmentIndexActionWithParameter()
        {
            const string id = "4364CDC5-2863-457A-B4D2-E9EFB9B7A24A";

            var httpContext = HttpContextBase(string.Format("~/Assessment/Index/{0}", id));

            var routeData = _routes.GetRouteData(httpContext);

            AssertIndexRouteValues(routeData);

            routeData.Values["patientId"].Should().Be(id);
        }

        [TestMethod]
        public void AssessmentCreateRoute_GivenInvalidPatientId_RouteShouldNotContainPatientId()
        {
            const string id = "1";

            var httpContext = HttpContextBase(string.Format("~/Assessment/Create/{0}", id));

            var routeData = _routes.GetRouteData(httpContext);

            routeData.Values.Should().NotContainKey("patientId");
        }

        [TestMethod]
        public void AssessmentIndexRoute_GivenInvalidPatientId_RouteShouldNotContainPatientId()
        {
            const string id = "1";

            var httpContext = HttpContextBase(string.Format("~/Assessment/Index/{0}", id));

            var routeData = _routes.GetRouteData(httpContext);

            routeData.Values.Should().NotContainKey("patientId");
        }


        [TestMethod]
        public void AssessmentRestartRoute_GivenAssessmentId_ShouldMapToAssessmentRestartActionWithParameter()
        {
            const string id = "4364CDC5-2863-457A-B4D2-E9EFB9B7A24A";

            var httpContext = HttpContextBase(string.Format("~/Assessment/Restart/{0}", id));

            var routeData = _routes.GetRouteData(httpContext);

            AssertRestartRouteValues(routeData);

            routeData.Values["assessmentId"].Should().Be(id);
        }

        #region private

        private void AssertCreateRouteValues(RouteData routeData)
        {
            routeData.Values["action"].Should().Be(MVC.Assessment.ActionNames.Create);
            routeData.Values["controller"].Should().Be(MVC.Assessment.Name);
        }

        private void AssertIndexRouteValues(RouteData routeData)
        {
            routeData.Values["action"].Should().Be(MVC.Assessment.ActionNames.Index);
            routeData.Values["controller"].Should().Be(MVC.Assessment.Name);
        }

        private void AssertRestartRouteValues(RouteData routeData)
        {
            routeData.Values["action"].Should().Be(MVC.Assessment.ActionNames.Restart);
            routeData.Values["controller"].Should().Be(MVC.Assessment.Name);
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
