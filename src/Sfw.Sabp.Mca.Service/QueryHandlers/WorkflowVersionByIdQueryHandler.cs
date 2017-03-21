using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class WorkflowVersionByIdQueryHandler : IQueryHandler<WorkflowVersionByIdQuery, WorkflowVersion>
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowVersionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public WorkflowVersion Retrieve(WorkflowVersionByIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return _unitOfWork.Context.Set<WorkflowVersion>().FirstOrDefault(x => x.WorkflowVersionId == query.WorkflowVersionId);
        }
    }
}
