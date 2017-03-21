using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class WorkflowStageByIdQuery : IQuery
    {
        public Guid WorkflowStageId { get; set; }
    }
}
