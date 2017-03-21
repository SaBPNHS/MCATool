using System;

namespace Sfw.Sabp.Mca.Service.Commands
{
    public class UpdateQuestionAnswerCommand : ICommand
    {
        public Guid QuestionAnswerId { get; set; }

        public string FurtherInformation  { get; set; }
    }
}
