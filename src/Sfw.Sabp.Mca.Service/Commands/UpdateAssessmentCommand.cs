using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class UpdateAssessmentCommand : ICommand
    {
        public Guid AssessmentId { get; set; }

        public DateTime DateAssessmentStarted { get; set; }

        public string Stage1DecisionToBeMade { get; set; }

        public int RoleId { get; set; }
        public string DecisionMaker { get; set; }
    }
}
