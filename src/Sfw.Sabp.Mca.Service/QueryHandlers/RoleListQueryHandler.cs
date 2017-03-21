using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using System;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class RoleListQueryHandler : IQueryHandler<RoleListQuery, Roles>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleListQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Roles Retrieve(RoleListQuery query)
        {
            if (query == null) throw new ArgumentNullException();           

            return new Roles
            {
                Items = _unitOfWork.Context.Set<Role>()
            };
        }
    }
}
