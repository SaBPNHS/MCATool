using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class AddQuestionAnswerCommand : ICommand
    {
        public Guid QuestionAnswerId { get; set; }

        public Guid AssessmentId { get; set; }

        public Guid WorkflowQuestionId { get; set; }

        public Guid QuestionOptionId { get; set; }

        public string FurtherInformation { get; set; }
    }
}
