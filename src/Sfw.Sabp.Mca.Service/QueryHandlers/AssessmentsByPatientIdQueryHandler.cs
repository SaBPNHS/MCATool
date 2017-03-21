using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class AssessmentsByPatientIdQueryHandler : IQueryHandler<AssessmentsByPatientIdQuery, Assessments>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssessmentsByPatientIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Assessments Retrieve(AssessmentsByPatientIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return new Assessments
            {
                Items = _unitOfWork.Context.Set<Assessment>().Where(a => a.Patient.PatientId == query.PatientId)
            };
           
        }
    }
}
