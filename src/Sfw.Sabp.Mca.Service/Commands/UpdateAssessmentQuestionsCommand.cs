using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class UpdateAssessmentQuestionsCommand : ICommand
    {
        public Guid AssessmentId { get; set; }
        public Guid NextQuestionId { get; set; }
        public Guid? PreviousQuestionId { get; set; }
        public Guid? ResetWorkflowQuestionId { get; set; }
    }
}
