using AutoMapper;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class AuditLogModelBuilder : IAuditLogModelBuilder
    {
        public AddAuditLogCommand BuildAddAuditLogCommand(AuditLogModel auditVm)
        {
            if (auditVm == null) throw new ArgumentNullException("auditVm");

            var command = Mapper.DynamicMap<AuditLogModel, AddAuditLogCommand>(auditVm);
            command.AuditLogId = Guid.NewGuid();

            return command;
        }
    }
}