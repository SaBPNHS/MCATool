namespace Sfw.Sabp.Mca.Web.ViewModels.Custom
{
    public interface INhsValidator
    {
        bool Valid(decimal? nhsNumber);
        bool Unique(decimal? nhsNumber);
    }
}