using System.Web.Mvc;
using Ninject.Extensions.Logging;

namespace Sfw.Sabp.Mca.Infrastructure.Web.Attributes
{
    public class ErrorLoggerFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public ErrorLoggerFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext filterContext)
        {
            _logger.ErrorException(filterContext.Exception.Message, filterContext.Exception);
        }
    }
}
