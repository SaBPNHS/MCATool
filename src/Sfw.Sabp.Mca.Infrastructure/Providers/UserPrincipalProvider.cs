using System.Web;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public class UserPrincipalProvider : IUserPrincipalProvider
    {
        public string CurrentUserName
        {
            get { return HttpContext.Current.User.Identity.Name; }
        }
    }
}
