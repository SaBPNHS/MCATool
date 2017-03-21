using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class UpdateAssessmentCompleteCommand : ICommand
    {
        public Guid AssessmentId { get; set; }

        public DateTime DateAssessmentEnded { get; set; }

        public string TerminatedAssessmentReason { get; set; }
    }
}
