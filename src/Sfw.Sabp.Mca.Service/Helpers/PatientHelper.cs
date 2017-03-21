using System;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public class PatientHelper : IPatientHelper
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public PatientHelper(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public Patient GetPatient(Guid id)
        {
            return _queryDispatcher.Dispatch<PatientByIdQuery, Patient>(new PatientByIdQuery
            {
                PatientId = id
            });
        }
    }
}
