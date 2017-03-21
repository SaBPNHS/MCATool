using Sfw.Sabp.Mca.Web.ViewModels.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    [FluentValidation.Attributes.Validator(typeof(DisclaimerViewModelValidator))]
    public class DisclaimerViewModel
    {
        [Display(Name = "I agree with the terms and conditions.")]
        public bool IsAgreed { get; set; }

        public Guid DisclaimerId { get; set; }

        public bool TermsAndConditionsEnabled { get; set; }
    }    
}