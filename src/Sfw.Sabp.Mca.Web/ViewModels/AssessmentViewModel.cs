using System.Web.Mvc;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Web.ViewModels.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    [FluentValidation.Attributes.Validator(typeof(AssessmentViewModelValidator))]
    public class AssessmentViewModel
    {
        public AssessmentViewModel()
        {
            Patient = new PatientViewModel();
        }

        public Guid AssessmentId { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Date assessment started")]
        public DateTime DateAssessmentStarted { get; set; }

        public string StageDescriptionStyle { get; set; }

        public string StageDescription { get; set; }

        public string Stage1InfoText { get; set; }

        [Display(Name = "State the decision to be made")]
        public string Stage1DecisionToBeMade { get; set; }

        public string Stage1DecisionConfirmation { get; set; }

        [Display(Name = "Confirm the decision to be made is specific and clearly defined")]
        public bool Stage1DecisionClearlyMade { get; set; }

        public Guid? CurrentWorkflowQuestionId { get; set; }
        
        public string AssessorName { get; set; }

        public StatusViewModel Status { get; set; }

        public string PatientFirstName { get; set; }

        public PatientViewModel Patient { get; set; }

        public bool YesClicked { get; set; }

        public bool CanViewPdf { get; set; }

        public bool CanRestart { get; set; }

        public bool CanRevise { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        [Display(Name = "Your role")]
        public int RoleId { get; set; }

        [Display(Name = "Decision maker's name")]
        public string DecisionMaker { get; set; }

        public string PatientSummary { get; set; }
        public bool Terminated { get; set; }

        public string ClinicalSystemName { get; set; }
    }

    public class StatusViewModel
    {
        public int StatusId { get; set; }
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == "ReadyToComplete")
                    _description = "In progress";
                else if (value != null && _description!= value)
                {
                    _description = value;
                }
            }
        }

        public bool InProgress()
        {
            return StatusId == (int)AssessmentStatusEnum.InProgress;
        }

        public bool Break()
        {
            return StatusId == (int) AssessmentStatusEnum.Break;
        }

        public bool Complete()
        {
            return StatusId == (int)AssessmentStatusEnum.Complete;
        }

        public bool ReadyToComplete()
        {
            return StatusId == (int) AssessmentStatusEnum.ReadyToComplete;
        }
    }

    public class AssessmentListViewModel
    {
        public Guid PatientId { get; set; }

        public IEnumerable<AssessmentViewModel> Items { get; set; }

        public string ClinicalIdDescription { get; set; }
    }
}