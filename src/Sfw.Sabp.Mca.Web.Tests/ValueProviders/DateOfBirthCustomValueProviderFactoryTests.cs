using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Web.ValueProviders;

namespace Sfw.Sabp.Mca.Web.Tests.ValueProviders
{
    [TestClass]
    public class DateOfBirthCustomValueProviderFactoryTests
    {
        private DateOfBirthCustomValueProviderFactory _dateOfBirthCustomValueProviderFactory;

        [TestInitialize]
        public void Setup()
        {
            _dateOfBirthCustomValueProviderFactory = new DateOfBirthCustomValueProviderFactory();
        }

        [TestMethod]
        public void GetValueProvider_GivenControllerIsPersonAndActionIsEditPost_DateOfBirthCustomValueProviderShouldBeReturned()
        {
            var result = _dateOfBirthCustomValueProviderFactory.GetValueProvider(ControllerContext(MVC.Person.Name, MVC.Person.ActionNames.Edit, "POST"));

            result.Should().BeOfType<DateOfBirthCustomValueProvider>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void GetValueProvider_GivenControllerIsNotPerson_NullShouldBeReturned()
        {
            var result = _dateOfBirthCustomValueProviderFactory.GetValueProvider(ControllerContext(MVC.Question.Name, MVC.Person.ActionNames.Edit, "POST"));

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetValueProvider_GivenActionIsNotEdit_NullShouldBeReturned()
        {
            var result = _dateOfBirthCustomValueProviderFactory.GetValueProvider(ControllerContext(MVC.Person.Name, MVC.Person.ActionNames.Index, "POST"));

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetValueProvider_GivenVerbIsNotPost_NullShouldBeReturned()
        {
            var result = _dateOfBirthCustomValueProviderFactory.GetValueProvider(ControllerContext(MVC.Person.Name, MVC.Person.ActionNames.Edit, "GET"));

            result.Should().BeNull();
        }

        #region private

        private ControllerContext ControllerContext(string controller, string action, string verb)
        {
            var controllerContext = A.Fake<ControllerContext>();
            var httpContext = A.Fake<HttpContextBase>();
            var httpRequest = A.Fake<HttpRequestBase>();

            A.CallTo(() => httpRequest.HttpMethod).Returns(verb);
            A.CallTo(() => httpContext.Request).Returns(httpRequest);
            A.CallTo(() => controllerContext.HttpContext).Returns(httpContext);

            var routeData = new RouteData();
            routeData.Values.Add("action", action);
            routeData.Values.Add("controller", controller);

            A.CallTo(() => controllerContext.RouteData).Returns(routeData);
            return controllerContext;
        }

        #endregion
    }
}
