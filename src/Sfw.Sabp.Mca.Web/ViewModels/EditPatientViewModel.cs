using System;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    [FluentValidation.Attributes.Validator(typeof(EditPatientViewModelValidator))]
    public class EditPatientViewModel : PatientViewModel
    {
        public string CurrentClinicalSystemId { get; set; }

        public decimal? CurrentNhsNumber { get; set; }

        public string CurrentFirstName { get; set; }

        public string CurrentLastName { get; set; }

        public string CurrentDateOfBirth { get; set; }

        public int CurrentGenderId { get; set; }
    }
}
