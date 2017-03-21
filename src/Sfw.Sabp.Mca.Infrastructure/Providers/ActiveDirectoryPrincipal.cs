using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public class ActiveDirectoryPrincipal : IActiveDirectoryPrincipal
    {
        public PrincipalContext GetPrincipalContext(string domain)
        {
            return new PrincipalContext(ContextType.Domain, null, domain);
        }

        public UserPrincipal GetUserPrincipal(PrincipalContext context, string userName)
        {
            
            return UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName);
        }

        public Principal GetUserPrincipalByGroup(PrincipalContext context, UserPrincipal principal, string group)
        {
            return principal.GetGroups(context).FirstOrDefault(g => g.Name == @group);
        }

        public List<string> GetGroupNames(UserPrincipal principal)
        {
            var searchResult = principal.GetAuthorizationGroups();

            var groupNames = new List<string>();

            var iterGroup = searchResult.GetEnumerator();

            using (iterGroup)
            {
                while (iterGroup.MoveNext())
                {
                    try
                    {
                        var p = iterGroup.Current;
                        if (p.Name != null)
                        {
                            groupNames.Add(p.Name.Trim().ToLower());
                        }
                    }
                    catch (NoMatchingPrincipalException)
                    {
                    }
                }
            }

            return groupNames;
        }

        public string DisplayNameForCurrentUser()
        {
            var principalContext = new PrincipalContext(ContextType.Domain);
            var userPrincipal = GetUserPrincipal(principalContext, HttpContext.Current.User.Identity.Name);
            return userPrincipal.DisplayName;
        }
    }
}
