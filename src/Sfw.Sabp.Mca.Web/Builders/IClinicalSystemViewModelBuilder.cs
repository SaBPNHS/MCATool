using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public interface IClinicalSystemViewModelBuilder
    {
        ClinicalSystemViewModel BuildClinicalSystemText(string clinicalSystemIdText);
    }
}