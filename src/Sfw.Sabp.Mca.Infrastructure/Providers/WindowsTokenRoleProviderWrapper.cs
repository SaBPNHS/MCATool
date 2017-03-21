using System.Web.Security;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public class WindowsTokenRoleProviderWrapper : IWindowsTokenRoleProviderWrapper
    {
        public bool IsUserInRole(string userName, string role)
        {
            var provider = new WindowsTokenRoleProvider();

            return provider.IsUserInRole(userName, role);
        }
    }
}
