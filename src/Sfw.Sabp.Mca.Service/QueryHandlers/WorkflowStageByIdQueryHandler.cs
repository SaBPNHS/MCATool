using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class WorkflowStageByIdQueryHandler : IQueryHandler<WorkflowStageByIdQuery, WorkflowStage>
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowStageByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public WorkflowStage Retrieve(WorkflowStageByIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return _unitOfWork.Context.Set<WorkflowStage>().FirstOrDefault(x => x.WorkflowStageId == query.WorkflowStageId);
        }
    }
}
