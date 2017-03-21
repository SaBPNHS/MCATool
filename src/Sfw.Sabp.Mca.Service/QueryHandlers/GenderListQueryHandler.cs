using System;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class GenderListQueryHandler : IQueryHandler<GenderListQuery, Genders>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenderListQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Genders Retrieve(GenderListQuery query)
        {
            if (query == null) throw new ArgumentNullException();           

            return new Genders
            {
                Items = _unitOfWork.Context.Set<Gender>()
            };
        }
    }
}

