namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public interface IUserRoleProvider
    {
        bool CurrentUserInAdministratorRole();
    }
}