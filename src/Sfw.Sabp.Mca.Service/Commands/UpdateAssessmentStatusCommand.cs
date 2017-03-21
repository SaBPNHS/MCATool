using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class UpdateAssessmentStatusCommand : ICommand
    {
        public Guid AssessmentId { get; set; }
        public int StatusId { get; set; }
    }
}
