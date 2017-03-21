using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class PatientByIdQueryHandler : IQueryHandler<PatientByIdQuery, Patient>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Patient Retrieve(PatientByIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return _unitOfWork.Context.Set<Patient>().FirstOrDefault(x => x.PatientId == query.PatientId);
        }
    }
}
