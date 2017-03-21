using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IAuditLogModelBuilder
    {
        AddAuditLogCommand BuildAddAuditLogCommand(AuditLogModel auditVm);
    }
}