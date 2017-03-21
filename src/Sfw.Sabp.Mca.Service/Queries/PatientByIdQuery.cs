using System;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class PatientByIdQuery : IQuery
    {
        public Guid PatientId { get; set; }
    }
}
