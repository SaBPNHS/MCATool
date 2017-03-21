using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class QuestionAnswerByAssessmentAndQuestionIdQueryHandler : IQueryHandler<QuestionAnswerByAssessmentAndQuestionIdQuery, QuestionAnswer>
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionAnswerByAssessmentAndQuestionIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public QuestionAnswer Retrieve(QuestionAnswerByAssessmentAndQuestionIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return _unitOfWork.Context.Set<QuestionAnswer>().FirstOrDefault(x => x.AssessmentId == query.AssessmentId && x.WorkflowQuestionId == query.WorkflowQuestionId);
        }
    }
}
