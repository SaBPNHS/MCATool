using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    [FluentValidation.Attributes.Validator(typeof(ClinicalSystemViewModelValidator))]
    public class ClinicalSystemViewModel
    {
        public string ClinicalSystemIdText { get; set; }
    }
}