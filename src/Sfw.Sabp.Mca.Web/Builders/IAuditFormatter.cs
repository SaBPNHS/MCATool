using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IAuditFormatter
    {
        string AuditValues(IValueProvider valueProvider, string auditProperties);
    }
}
