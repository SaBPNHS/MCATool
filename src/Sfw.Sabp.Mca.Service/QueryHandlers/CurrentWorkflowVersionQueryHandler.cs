using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class InvalidCurrentWorkflowException : Exception { }

    public class CurrentWorkflowVersionQueryHandler : IQueryHandler<CurrentWorkflowQuery, WorkflowVersion>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CurrentWorkflowVersionQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public WorkflowVersion Retrieve(CurrentWorkflowQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            var workflowVersion = _unitOfWork.Context.Set<WorkflowVersion>().Where(x => x.ExpiredDate == query.ExpiredDate);

            if (workflowVersion.Count()!=1)
            {
                throw new InvalidCurrentWorkflowException();
            }

            return workflowVersion.First();
        }
    }
}
