using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    /// <summary>
    /// Holds a new assessment
    /// </summary>
    public class AddAssessmentCommand : ICommand
    {
        public Guid AssessmentId { get; set; }
        
        public DateTime DateAssessmentStarted { get; set; }

        // Holds the decision about the patient to be made
        public string Stage1DecisionToBeMade { get; set; }

        // True if clinician has confirmed the decision is precise and specific
        public bool Stage1DecisionClearlyMade { get; set; }

        // holds the decision prompt
        public string Stage1InfoText { get; set; }
        //holds the question that asks the clinician if the decision is correct
        public string Stage1DecisionConfirmation { get; set; }

        public Guid CurrentWorkflowQuestionId { get; set; }

        public Guid WorkflowVersionId { get; set; }

        public int StatusId { get; set; }

        public Guid PatientId { get; set; }

        public DateTime DateAssessmentEnded { get; set; }

        public int RoleId { get; set; }

        public string DecisionMaker { get; set; }
    }
}
