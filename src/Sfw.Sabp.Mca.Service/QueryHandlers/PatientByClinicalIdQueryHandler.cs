using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class PatientByClinicalIdQueryHandler : IQueryHandler<PatientByClinicalIdQuery, Patients>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientByClinicalIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Patients Retrieve(PatientByClinicalIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return new Patients
            {
                Items = _unitOfWork.Context.Set<Patient>().Where(x => x.ClinicalSystemId == query.ClinicalId)
            };
        }
    }
}
