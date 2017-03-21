using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using System;
using System.Linq;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class QuestionAnswersByAssessmentIdQueryHandler : IQueryHandler<QuestionAnswersByAssessmentQuery, QuestionAnswers>
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionAnswersByAssessmentIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public QuestionAnswers Retrieve(QuestionAnswersByAssessmentQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return new QuestionAnswers
            {
                Items = _unitOfWork.Context.Set<QuestionAnswer>().Where(a => a.AssessmentId == query.Assessment.AssessmentId).OrderBy(x=>x.Created)
            };
        }
    }
}