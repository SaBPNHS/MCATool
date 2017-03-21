using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class ClinicalSystemViewModelBuilder : IClinicalSystemViewModelBuilder
    {
        public ClinicalSystemViewModel BuildClinicalSystemText(string clinicalSystemIdText)
        {
            return new ClinicalSystemViewModel() {ClinicalSystemIdText = clinicalSystemIdText};
        }
    }
}