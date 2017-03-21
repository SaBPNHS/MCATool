using System;
using System.Web.Mvc;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Logging;
using Sfw.Sabp.Mca.Infrastructure.Web.Attributes;

namespace Sfw.Sabp.Mca.Infrastructure.Tests.Attributes
{
    [TestClass]
    public class ErrorLoggerFilterTests
    {
        private ILogger _logger;
        private ErrorLoggerFilter _errorLoggerFilter;

        [TestInitialize]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();

            _errorLoggerFilter = new ErrorLoggerFilter(_logger);
        }

        [TestMethod]
        public void OnException_GivenException_ExceptionShouldBeLogged()
        {
            var exception = new Exception("message");

            var filterContext = new ExceptionContext()
            {
                Exception = exception
            };

            _errorLoggerFilter.OnException(filterContext);

            A.CallTo(() => _logger.ErrorException("message", exception)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
