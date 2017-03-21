using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class QuestionAnswerByIdQueryHandler : IQueryHandler<QuestionAnswerByIdQuery, QuestionAnswer>
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionAnswerByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public QuestionAnswer Retrieve(QuestionAnswerByIdQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return _unitOfWork.Context.Set<QuestionAnswer>().FirstOrDefault(x => x.QuestionAnswerId == query.QuestionAnswerId);
        }
    }
}
