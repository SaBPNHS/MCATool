using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class WorkflowStepByVersionAndQuestionOptionQueryHandler : IQueryHandler<WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery, WorkflowStep>
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowStepByVersionAndQuestionOptionQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public WorkflowStep Retrieve(WorkflowStepByVersionCurrentQuestionAndQuestionOptionQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return _unitOfWork.Context.Set<WorkflowStep>().FirstOrDefault(x => x.QuestionOptionId == query.QuestionOptionId 
                && x.WorkflowVersionId == query.WorkflowVersionId
                && x.CurrentWorkflowQuestionId == query.CurrentWorkflowQuestionId);
        }
    }
}
