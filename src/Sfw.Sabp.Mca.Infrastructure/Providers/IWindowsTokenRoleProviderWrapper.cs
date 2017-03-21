namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public interface IWindowsTokenRoleProviderWrapper
    {
        bool IsUserInRole(string userName, string role);
    }
}
