using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class QuestionAnswersByWorkflowStageIdQueryHandler : IQueryHandler<QuestionAnswersByWorkflowStageIdQuery, QuestionAnswers>
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionAnswersByWorkflowStageIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public QuestionAnswers Retrieve(QuestionAnswersByWorkflowStageIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return new QuestionAnswers
            {
                Items = _unitOfWork.Context.Set<QuestionAnswer>().Where(x => x.WorkflowQuestion.WorkflowStageId == query.WorkflowStageId && x.AssessmentId == query.AssessmentId)
            };
        }
    }
}