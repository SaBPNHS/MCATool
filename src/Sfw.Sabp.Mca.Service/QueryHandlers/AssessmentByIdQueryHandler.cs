using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class AssessmentByIdQueryHandler : IQueryHandler<AssessmentByIdQuery, Assessment>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssessmentByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Assessment Retrieve(AssessmentByIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return _unitOfWork.Context.Set<Assessment>().FirstOrDefault(x => x.AssessmentId == query.AssessmentId);
        }
    }
}
