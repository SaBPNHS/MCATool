using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class CurrentWorkflowQuery : IQuery
    {
        public DateTime? ExpiredDate { get { return null; } }
    }
}
