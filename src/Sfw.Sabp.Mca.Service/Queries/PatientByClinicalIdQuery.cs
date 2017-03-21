using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.Queries
{
    public class PatientByClinicalIdQuery : IQuery
    {
        public string ClinicalId { get; set; }
    }
}
