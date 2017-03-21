using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class PatientByNhsNumberQuery : IQuery
    {
        public decimal NhsNumber { get; set; }
    }
}
