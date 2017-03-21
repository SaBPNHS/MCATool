namespace Sfw.Sabp.Mca.Web.ViewModels.Custom
{
    public interface IClinicalIdValidator
    {
        bool Unique(string clinicalId);
    }
}