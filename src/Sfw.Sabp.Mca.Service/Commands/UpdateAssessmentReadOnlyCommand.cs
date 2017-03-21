using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class UpdateAssessmentReadOnlyCommand : ICommand
    {
        public Guid AssessmentId { get; set; }
        public bool ReadOnly { get; set; }
    }
}
