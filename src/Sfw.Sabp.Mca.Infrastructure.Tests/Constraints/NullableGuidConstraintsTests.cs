using System;
using System.Web;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Constraints;

namespace Sfw.Sabp.Mca.Infrastructure.Tests.Constraints
{
    [TestClass]
    public class NullableGuidConstraintsTests
    {
        private NullableGuidConstraint _constraint;
        private HttpContextBase _httpContextBase;
        private string _parameterName;
        private Route _route;

        [TestInitialize]
        public void Setup()
        {
            _constraint = new NullableGuidConstraint();

            _httpContextBase = A.Fake<HttpContextBase>();
            _parameterName = "optionId";
            _route = A.Fake<Route>();
        }

        [TestMethod]
        public void Match_GivenEmptyValue_TrueShouldBeReturned()
        {
            var routeValues = new RouteValueDictionary { { _parameterName, "" } };

            var result = _constraint.Match(_httpContextBase, _route, _parameterName, routeValues, RouteDirection.IncomingRequest);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Match_GivenNullValue_NullReferenceExceptionExpected()
        {
            var routeValues = new RouteValueDictionary { { _parameterName, null } };

            _constraint.Invoking(x => x.Match(_httpContextBase, _route, _parameterName, routeValues, RouteDirection.IncomingRequest)).ShouldThrow<NullReferenceException>();
        }

        [TestMethod]
        public void Match_GivenNotGuidValue_FalseShouldBeReturned()
        {
            var routeValues = new RouteValueDictionary { { _parameterName, "value" } };

            var result = _constraint.Match(_httpContextBase, _route, _parameterName, routeValues, RouteDirection.IncomingRequest);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void Match_GivenGuidValue_TrueShouldBeReturned()
        {
            var routeValues = new RouteValueDictionary { { _parameterName, "F06F4BFF-1432-4DAC-BDF4-FB67B18CFB7C" } };

            var result = _constraint.Match(_httpContextBase, _route, _parameterName, routeValues, RouteDirection.IncomingRequest);

            result.Should().BeTrue();
        }
    }
}
