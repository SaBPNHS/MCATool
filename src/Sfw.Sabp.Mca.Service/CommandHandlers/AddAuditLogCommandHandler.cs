using System;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class AddAuditLogCommandHandler : ICommandHandler<AddAuditLogCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddAuditLogCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(AddAuditLogCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            
            var auditLog = new AuditLog
            {
                AuditLogId=command.AuditLogId,                
                User= command.User,
                EventDateTime= command.EventDateTime,
                Action=command.Action,
                Controller=command.Controller,
                AuditData=command.AuditData             
            };

            _unitOfWork.Context.Set<AuditLog>().Add(auditLog);
        }
    }
}
