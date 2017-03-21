using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class AddAuditLogCommand:ICommand
    {
        public Guid AuditLogId { get; set; }
        public string User { get; set; }
        public DateTime EventDateTime { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string AuditData { get; set; }
    }
}
