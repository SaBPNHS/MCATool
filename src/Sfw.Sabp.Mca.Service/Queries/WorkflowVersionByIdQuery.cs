using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class WorkflowVersionByIdQuery : IQuery
    {
        public Guid WorkflowVersionId { get; set; }
    }
}
