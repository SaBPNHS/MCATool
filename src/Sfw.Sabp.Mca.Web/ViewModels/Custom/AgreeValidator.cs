using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Queries;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Infrastructure.Providers;

namespace Sfw.Sabp.Mca.Web.ViewModels.Custom
{
    public class AgreeValidator : IAgreeValidator
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IUserPrincipalProvider _userPrincipalProvider;

        public AgreeValidator(IQueryDispatcher queryDispatcher, IUserPrincipalProvider userPrincipalProvider)
        {
            _queryDispatcher = queryDispatcher;
            _userPrincipalProvider = userPrincipalProvider;
        }

        public bool Unique(bool isAgreed)
        {
            var agree = _queryDispatcher.Dispatch<DisclaimerByUserQuery, Disclaimer>(new DisclaimerByUserQuery()
            {
                AssessorDomainName = _userPrincipalProvider.CurrentUserName
            });

            if (agree != null || isAgreed)
                return true;

            return false;
        }

    }
}