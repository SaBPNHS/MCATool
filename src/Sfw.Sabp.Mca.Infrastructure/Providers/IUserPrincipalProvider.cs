namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public interface IUserPrincipalProvider
    {
        string CurrentUserName { get; }
    }
}