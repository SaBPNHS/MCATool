using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    [FluentValidation.Attributes.Validator(typeof(QuestionViewModelValidator))]
    public class QuestionViewModel
    {
        public string StageDescription { get; set; }

        [AllowHtml]
        public string Guidance { get; set; }

        [AllowHtml]
        public string Question { get; set; }

        public Guid? ChosenOption { get; set; }

        public bool DisplayGuidance
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Guidance);
            }
        }

        public bool DisplayOptions
        {
            get
            {
                return Options != null && Options.Count() > 1;
            }
        }

        public IEnumerable<OptionViewModel> Options { get; set; }

        public Guid AssessmentId { get; set; }

        public bool DisplayFurtherInformationQuestion { get; set; }

        public string FurtherInformationQuestion { get; set; }

        public string FurtherInformationAnswer { get; set; }

        public Guid PatientId { get; set; }

        public string StageDescriptionStyle { get; set; }

        public bool DisableBackButton { get; set; }

        public bool ReadOnly { get; set; }

        public bool DisableResetButton { get; set; }
        
        public string Stage1DecisionMade { get; set; }

        public bool DisplayStage1DecisionMade { get; set; }

        public Guid QuestionAnswerId { get; set; }
    }
}