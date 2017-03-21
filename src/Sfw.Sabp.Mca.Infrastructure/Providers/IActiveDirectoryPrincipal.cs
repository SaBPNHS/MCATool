using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public interface IActiveDirectoryPrincipal
    {
        PrincipalContext GetPrincipalContext(string domain);
        UserPrincipal GetUserPrincipal(PrincipalContext context, string userName);
        Principal GetUserPrincipalByGroup(PrincipalContext context, UserPrincipal principal, string group);
        List<string> GetGroupNames(UserPrincipal principal);
        string DisplayNameForCurrentUser();
    }
}