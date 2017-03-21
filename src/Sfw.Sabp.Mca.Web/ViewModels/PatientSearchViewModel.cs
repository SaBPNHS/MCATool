using Sfw.Sabp.Mca.Web.Attributes;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;
using System.Collections.Generic;
using System.Linq;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    [FluentValidation.Attributes.Validator(typeof(PatientSearchViewModelValidator))]
    public class PatientSearchViewModel
    {
        public IEnumerable<PatientViewModel> Items { get; set; }
        
        [ClinicalSystemIdDisplay]
        public string ClinicalSystemId { get; set; }

        public bool HasResult()
        {
            return Items != null && Items.Any();
        }

        public bool CanCreate()
        {
            return Items != null && Items.Count() >= 0;
        }

        public bool DisplayEmptySearchResultsMessage()
        {
            return Items != null && !Items.Any();
        }

        public string ClinicalIdDescription { get; set; }

        public bool CanEdit { get; set; }
    }
}