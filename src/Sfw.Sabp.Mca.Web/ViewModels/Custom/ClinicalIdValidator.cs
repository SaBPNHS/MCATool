using System.Linq;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Web.ViewModels.Custom
{
    public class ClinicalIdValidator : IClinicalIdValidator
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public ClinicalIdValidator(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public bool Unique(string clinicalId)
        {
            if (string.IsNullOrWhiteSpace(clinicalId)) return false;

            var patient = _queryDispatcher.Dispatch<PatientByClinicalIdQuery, Patients>(new PatientByClinicalIdQuery
            {
                ClinicalId = clinicalId.Trim()
            });

            return !patient.Items.Any();
        }
    }
}