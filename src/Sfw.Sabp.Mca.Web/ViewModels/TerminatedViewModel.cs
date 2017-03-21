using System;
using System.ComponentModel.DataAnnotations;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    [FluentValidation.Attributes.Validator(typeof(TerminatedViewModelValidator))]
    public class TerminatedViewModel
    {
        public Guid AssessmentId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date assessment ended")]
        public DateTime DateAssessmentEnded { get; set; }

        [Display(Name="Reason decision is no longer valid")]
        public string TerminatedAssessmentReason { get; set; }

        public bool TerminatedReasonRequired { get; set; }
    }
}