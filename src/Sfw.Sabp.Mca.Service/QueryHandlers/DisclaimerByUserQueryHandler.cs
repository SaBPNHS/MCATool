using System;
using System.Linq;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class DisclaimerByUserQueryHandler : IQueryHandler<DisclaimerByUserQuery, Disclaimer>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DisclaimerByUserQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Disclaimer Retrieve(DisclaimerByUserQuery query)
        {
            if (query == null) throw new ArgumentNullException();

            return _unitOfWork.Context.Set<Disclaimer>().FirstOrDefault(x => x.AssessorDomainName == query.AssessorDomainName);
        }
    }
}
