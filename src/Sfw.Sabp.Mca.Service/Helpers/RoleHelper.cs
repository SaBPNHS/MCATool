using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;

namespace Sfw.Sabp.Mca.Service.Helpers
{
    public class RoleHelper : IRoleHelper
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public RoleHelper(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public Roles GetRoles()
        {
            var roleQuery = new RoleListQuery();

            return _queryDispatcher.Dispatch<RoleListQuery, Roles>(roleQuery);
        }
    }
}
