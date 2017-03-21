using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class AssessmentByIdQuery : IQuery
    {
        public Guid AssessmentId { get; set; }
    }
}
